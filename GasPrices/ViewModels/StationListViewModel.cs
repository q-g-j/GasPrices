using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using GasPrices.Extensions;
using GasPrices.Models;
using GasPrices.PageTransitions;
using GasPrices.Services;
using GasPrices.Store;
using SettingsHandling;
using SettingsHandling.Models;

namespace GasPrices.ViewModels;

public partial class StationListViewModel : ViewModelBase
{
    #region constructors

    public StationListViewModel()
    {
    }

    public StationListViewModel(
        NavigationService<ResultsNavigationStore> resultsNavigationService,
        AppStateStore appStateStore,
        ISettingsReader settingsReader,
        ISettingsWriter settingsWriter)
    {
        _resultsNavigationService = resultsNavigationService;
        _appStateStore = appStateStore;
        _settingsReader = settingsReader;
        _settingsWriter = settingsWriter;

        _gasTypes =
        [
            new GasType("E5"),
            new GasType("E10"),
            new GasType("Diesel")
        ];

        _listBoxFadingDuration = TimeSpan.FromMilliseconds(150);
        _fadeOutTimer = new DispatcherTimer { Interval = _listBoxFadingDuration };
        _fadeInTimer = new DispatcherTimer { Interval = _listBoxFadingDuration };

        _fadeOutTimer.Tick += (_, _) =>
        {
            _fadeOutTimer.Stop();
            ListBoxFadeOut = false;
            UpdateStations();
            ListBoxFadeIn = true;
            _fadeInTimer.Start();
        };

        _fadeInTimer.Tick += (_, _) =>
        {
            _fadeInTimer.Stop();
            ListBoxFadeIn = false;
        };

        InitializeStations().FireAndForget();
    }

    #endregion constructors

    #region private fields

    private readonly ISettingsReader? _settingsReader;
    private readonly ISettingsWriter? _settingsWriter;
    private Settings? _settings;
    private readonly NavigationService<ResultsNavigationStore>? _resultsNavigationService;
    private readonly AppStateStore? _appStateStore;
    private GasType? _gasType;
    private string? _sortBy;
    private bool _isFirstRun = true;
    private readonly DispatcherTimer? _fadeOutTimer;
    private readonly DispatcherTimer? _fadeInTimer;
    private List<DisplayStation>? _stationsTemp;

    #endregion private fields

    #region bindable properties

    [ObservableProperty] private ObservableCollection<DisplayStation>? _stations;
    [ObservableProperty] private int _selectedIndex = -1;
    [ObservableProperty] private int _selectedSortingIndex = -1;
    [ObservableProperty] private int _selectedGasTypeIndex = -1;
    [ObservableProperty] private ObservableCollection<GasType>? _gasTypes;
    [ObservableProperty] private TimeSpan _listBoxFadingDuration;
    [ObservableProperty] private bool _listBoxFadeOut;
    [ObservableProperty] private bool _listBoxFadeIn;

    #endregion bindable properties

    #region OnPropertyChanged handlers

    partial void OnSelectedIndexChanged(int value)
    {
        _appStateStore!.SelectedStation = Stations![value];
        _appStateStore!.SelectedStationIndex = value;
        _resultsNavigationService!
            .Navigate<StationDetailsViewModel,
                CustomCompositePageTransition<CustomCrossFadePageTransition, SlideLeftPageTransition>>();
    }

    partial void OnSelectedGasTypeIndexChanged(int value)
    {
        if (_isFirstRun) return;

        _gasType = GasTypes![value];

        _stationsTemp = _appStateStore!.Stations!
            .Where(s => s is { E5: > 0, E10: > 0, Diesel: > 0 })
            .Select(station => new DisplayStation(station, _gasType))
            .ToList();

        SortStations();

        UpdateSettingsAsync().FireAndForget();
    }


    partial void OnSelectedSortingIndexChanged(int value)
    {
        if (_isFirstRun) return;

        _sortBy = value switch
        {
            0 => "Name",
            1 => "Price",
            2 => "Distance",
            _ => "Price"
        };

        SortStations();

        UpdateSettingsAsync().FireAndForget();
    }

    #endregion OnPropertyChanged handlers

    #region private methods

    private async Task InitializeStations()
    {
        _sortBy = "Price";
        _gasType = new GasType("E5");

        _settings = await _settingsReader!.ReadAsync();

        if (!string.IsNullOrEmpty(_settings!.SortBy))
        {
            _sortBy = _settings.SortBy;
        }

        if (!string.IsNullOrEmpty(_settings.GasType))
        {
            _gasType = new GasType(_settings.GasType);
        }

        var sortingIndex = _sortBy switch
        {
            "Name" => 0,
            "Price" => 1,
            "Distance" => 2,
            _ => 1
        };

        _stationsTemp = new List<DisplayStation>(_appStateStore!.Stations!
            .Where(s => s is { E5: > 0, E10: > 0, Diesel: > 0 })
            .Select(station => new DisplayStation(station, new GasType(_gasType!.ToString()))).ToList());

        SelectedGasTypeIndex = GasTypes!.IndexOf(GasTypes.FirstOrDefault(gt => gt.ToString() == _gasType.ToString())!);
        SelectedSortingIndex = sortingIndex;

        SortStations();

        _isFirstRun = false;
    }

    private async Task UpdateSettingsAsync()
    {
        try
        {
            var settings = await _settingsReader!.ReadAsync();
            settings!.GasType = _gasType!.ToString();
            settings.SortBy = _sortBy;
            await _settingsWriter!.WriteAsync(settings);
        }
        catch (Exception)
        {
            // ignore
        }
    }

    private void SortStations()
    {
        if (OperatingSystem.IsBrowser())
        {
            UpdateStations();
            _isFirstRun = false;
            return;
        }

        if (!_isFirstRun)
        {
            ListBoxFadeIn = false;
            ListBoxFadeOut = true;
            _fadeOutTimer!.Start();
        }
        else
        {
            UpdateStations();
            _isFirstRun = false;
        }
    }

    private void UpdateStations()
    {
        _stationsTemp = _sortBy switch
        {
            "Name" => [.. _stationsTemp!.OrderBy(s => s.Brand)],
            "Price" => [.. _stationsTemp!.OrderBy(s => s.Price)],
            "Distance" => [.. _stationsTemp!.OrderBy(s => s.Distance)],
            _ => _stationsTemp
        };

        Stations = new ObservableCollection<DisplayStation>(_stationsTemp!);
    }

    #endregion

    #region public overrides

    public override void Dispose()
    {
    }

    #endregion public overrides
}