using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GasPrices.Models;
using GasPrices.PageTransitions;
using GasPrices.Services;
using GasPrices.Store;

namespace GasPrices.ViewModels;

public partial class StationListViewModel : ViewModelBase
{
    #region constructors

    public StationListViewModel()
    {
    }


    public StationListViewModel(
        ResultsNavigationService resultsNavigationService,
        AppStateStore appStateStore)
    {
        _resultsNavigationService = resultsNavigationService;
        _appStateStore = appStateStore;

        Stations = [];
        foreach (var station in appStateStore.Stations!.Where(s => s is { E5: > 0, E10: > 0, Diesel: > 0 }))
        {
            Stations.Add(new DisplayStation(station, appStateStore.SelectedGasType!));
        }
    }

    #endregion constructors

    #region private fields

    private readonly ResultsNavigationService? _resultsNavigationService;
    private readonly AppStateStore? _appStateStore;

    #endregion private fields


    #region bindable properties

    [ObservableProperty] private ObservableCollection<DisplayStation>? _stations;
    [ObservableProperty] private int _selectedIndex = -1;

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

    #endregion OnPropertyChanged handlers

    #region commands

    [RelayCommand]
    public void InitializedCommand()
    {
        if (_appStateStore!.IsFromStationDetailsView)
        {
            
            Task.Run(() =>
            {
                Thread.Sleep(100);
                SelectedIndex = _appStateStore!.SelectedStationIndex;
            });
        }
    }

    #endregion commands

    #region public overrides

    public override void Dispose()
    {
    }

    #endregion public overrides
}