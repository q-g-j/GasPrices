using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using GasPrices.Extensions;
using GasPrices.Models;
using GasPrices.PageTransitions;
using GasPrices.Services;
using GasPrices.Store;
using SettingsHandling;

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

        InitializeStations().FireAndForget();
    }

    #endregion constructors

    #region private fields

    private readonly ISettingsReader? _settingsReader;
    private readonly ISettingsWriter? _settingsWriter;
    private readonly NavigationService<ResultsNavigationStore>? _resultsNavigationService;
    private readonly AppStateStore? _appStateStore;
    private GasType? _gasType;
    private string? _sortBy;

    #endregion private fields

    #region bindable properties

    [ObservableProperty] private List<DisplayStation>? _stations;
    [ObservableProperty] private int _selectedIndex = -1;
    [ObservableProperty] private int _selectedSortingIndex = 1;
    [ObservableProperty] private int _selectedGasTypeIndex;
    [ObservableProperty] private ObservableCollection<GasType>? _gasTypes;

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
        _gasType = GasTypes![value];

        var stations = _appStateStore!.Stations!
            .Where(s => s is { E5: > 0, E10: > 0, Diesel: > 0 })
            .Select(station => new DisplayStation(station, _gasType))
            .ToList();

        SortStations(stations);

        UpdateSettingsAsync().FireAndForget();
    }


    partial void OnSelectedSortingIndexChanged(int value)
    {
        _sortBy = value switch
        {
            0 => "Name",
            1 => "Price",
            2 => "Distance",
            _ => "Price"
        };

        SortStations([.. Stations]);

        UpdateSettingsAsync().FireAndForget();
    }

    #endregion OnPropertyChanged handlers

    #region private methods

    private async Task InitializeStations()
    {
        _sortBy = "Price";
        _gasType = new GasType("E5");

        var settings = await _settingsReader!.ReadAsync();

        if (!string.IsNullOrEmpty(settings!.SortBy))
        {
            _sortBy = settings.SortBy;
        }

        if (!string.IsNullOrEmpty(settings.GasType))
        {
            _gasType = new GasType(settings.GasType);
        }

        var sortingIndex = _sortBy switch
        {
            "Name" => 0,
            "Price" => 1,
            "Distance" => 2,
            _ => 1
        };

        var stations = _appStateStore!.Stations!
            .Where(s => s is { E5: > 0, E10: > 0, Diesel: > 0 })
            .Select(station => new DisplayStation(station, new GasType(_gasType!.ToString())))
            .ToList();

        SortStations(stations);

        SelectedGasTypeIndex = GasTypes!.IndexOf(GasTypes.FirstOrDefault(gt => gt.ToString() == _gasType.ToString())!);
        SelectedSortingIndex = sortingIndex;
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

    private void SortStations(IEnumerable<DisplayStation> stations)
    {
        Stations = _sortBy switch
        {
            "Name" => [.. stations.OrderBy(s => s.Name)],
            "Price" => [.. stations.OrderBy(s => s.Price)],
            "Distance" => [.. stations.OrderBy(s => s.Distance)],
            _ => Stations
        };
    }

    #endregion

    #region public overrides

    public override void Dispose()
    {
    }

    #endregion public overrides
}