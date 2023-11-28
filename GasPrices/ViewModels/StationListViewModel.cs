using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GasPrices.Models;
using GasPrices.PageTransitions;
using GasPrices.Services;
using GasPrices.Store;
using SettingsFile;

namespace GasPrices.ViewModels;

public partial class StationListViewModel : ViewModelBase
{
    #region constructors

    public StationListViewModel()
    {
    }


    public StationListViewModel(
        ResultsNavigationService resultsNavigationService,
        AppStateStore appStateStore,
        SettingsFileReader settingsFileReader,
        SettingsFileWriter settingsFileWriter)
    {
        _resultsNavigationService = resultsNavigationService;
        _appStateStore = appStateStore;
        _settingsFileReader = settingsFileReader;
        _settingsFileWriter = settingsFileWriter;

        var sortBy = "Price";
        var settings = _settingsFileReader!.Read();
        if (!string.IsNullOrEmpty(settings!.SortBy))
        {
            sortBy = settings!.SortBy;
        }

        _stations = appStateStore.Stations!
            .Where(s => s is { E5: > 0, E10: > 0, Diesel: > 0 })
            .Select(station => new DisplayStation(station, appStateStore.SelectedGasType!))
            .ToList();

        var sortingIndex = sortBy switch
        {
            "Name" => 0,
            "Price" => 1,
            "Distance" => 2,
            _ => 1
        };

        SelectedGasType = _appStateStore!.SelectedGasType!.ToString()!;

        SelectedSortingIndex = sortingIndex;
    }

    #endregion constructors

    #region private fields

    private readonly SettingsFileReader? _settingsFileReader;
    private readonly SettingsFileWriter? _settingsFileWriter;
    private readonly ResultsNavigationService? _resultsNavigationService;
    private readonly AppStateStore? _appStateStore;

    #endregion private fields

    #region bindable properties

    [ObservableProperty] private List<DisplayStation>? _stations;
    [ObservableProperty] private int _selectedIndex = -1;
    [ObservableProperty] private int _selectedSortingIndex = -1;
    [ObservableProperty] private string _selectedGasType;

    #endregion bindable properties

    #region OnPropertyChanged handlers

    partial void OnSelectedIndexChanged(int value)
    {
        if (_appStateStore!.IsFromStationDetailsView)
        {
            if (SelectedIndex == -1)
            {
                _appStateStore!.IsFromStationDetailsView = false;
            }
            else
            {
                Task.Run(() =>
                {
                    Thread.Sleep(400);
                    SelectedIndex = -1;
                });
            }

            return;
        }

        _appStateStore!.SelectedStation = Stations![value];
        _appStateStore!.SelectedStationIndex = value;
        _resultsNavigationService!.Navigate<StationDetailsViewModel, SlideLeftPageTransition>();
    }

    partial void OnSelectedSortingIndexChanged(int value)
    {
        var sortBy = value switch
        {
            0 => "Name",
            1 => "Price",
            2 => "Distance",
            _ => "Price"
        };

        SortStations([..Stations], sortBy!);

        Task.Run(async () =>
        {
            var settings = await _settingsFileReader!.ReadAsync();
            settings!.SortBy = sortBy;
            await _settingsFileWriter!.WriteAsync(settings);
        });
    }

    #endregion OnPropertyChanged handlers

    #region commands

    [RelayCommand]
    public void InitializedCommand()
    {
        if (_appStateStore!.IsFromStationDetailsView)
        {
            Task.Run(() =>
            {
                Thread.Sleep(150);
                SelectedIndex = _appStateStore!.SelectedStationIndex;
            });
        }
    }

    #endregion commands

    #region private methods

    private void SortStations(IEnumerable<DisplayStation> stations, string sortBy)
    {
        Stations = sortBy switch
        {
            "Name" => stations.OrderBy(s => s.Name).ToList(),
            "Price" => stations.OrderBy(s => s.Price).ToList(),
            "Distance" => stations.OrderBy(s => s.Distance).ToList(),
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