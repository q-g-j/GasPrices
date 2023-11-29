using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using GasPrices.Extensions;
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

        SelectedGasType = _appStateStore!.SelectedGasType!.ToString()!;

        InitializeStations().FireAndForget();
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
    [ObservableProperty] private string? _selectedGasType;

    #endregion bindable properties

    #region OnPropertyChanged handlers

    partial void OnSelectedIndexChanged(int value)
    {
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

        if (Stations == null)
        {
            var stations = _appStateStore!.Stations!
                .Where(s => s is { E5: > 0, E10: > 0, Diesel: > 0 })
                .Select(station => new DisplayStation(station, _appStateStore!.SelectedGasType!))
                .ToList();

            SortStations(stations, sortBy);
        }
        else
        {
            SortStations([..Stations], sortBy);
        }

        UpdateSettingsAsync(sortBy).FireAndForget();
    }

    #endregion OnPropertyChanged handlers

    #region private methods

    private async Task InitializeStations()
    {
        var sortBy = "Price";
        var settings = await _settingsFileReader!.ReadAsync();
        if (!string.IsNullOrEmpty(settings!.SortBy))
        {
            sortBy = settings.SortBy;
        }

        var sortingIndex = sortBy switch
        {
            "Name" => 0,
            "Price" => 1,
            "Distance" => 2,
            _ => 1
        };

        SelectedSortingIndex = sortingIndex;
    }

    private async Task UpdateSettingsAsync(string sortBy)
    {
        try
        {
            var settings = await _settingsFileReader!.ReadAsync();
            settings!.SortBy = sortBy;
            await _settingsFileWriter!.WriteAsync(settings);
        }
        catch (Exception)
        {
            // ignore
        }
    }

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