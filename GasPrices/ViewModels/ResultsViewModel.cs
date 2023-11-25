using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GasPrices.Models;
using GasPrices.Services;
using GasPrices.Store;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Animation;
using GasPrices.PageTransitions;
using Xamarin.Essentials;

namespace GasPrices.ViewModels
{
    public partial class ResultsViewModel : ViewModelBase
    {
        #region constructors

        public ResultsViewModel()
        {
        }

        public ResultsViewModel(
            NavigationService navigationService,
            AppStateStore appStateStore)
        {
            _navigationService = navigationService;
            _appStateStore = appStateStore;

            Stations = [];
            foreach (var station in appStateStore.Stations!.Where(s => s is { E5: > 0, E10: > 0, Diesel: > 0 }))
            {
                Stations.Add(new DisplayStation(station, appStateStore.SelectedGasType!));
            }

            if (appStateStore.IsFromStationDetailsView)
            {
                Task.Run(async () =>
                {
                    SelectedIndex = appStateStore.LastSelectedStationIndex;
                    appStateStore.LastSelectedStationIndex = -1;
                    await Task.Delay(500);
                    appStateStore.IsFromStationDetailsView = true;
                    SelectedIndex = -1;
                });
            }

            ((App)Application.Current!).BackPressed += OnBackPressed;
        }

        #endregion constructors

        #region private fields

        private readonly NavigationService? _navigationService;
        private readonly AppStateStore? _appStateStore;

        #endregion privat fields

        #region bindable properties

        [ObservableProperty] private ObservableCollection<DisplayStation>? _stations;
        [ObservableProperty] private int _selectedIndex = -1;
        [ObservableProperty] private object _selectedItem;
        #endregion bindable properties

        #region commands

        [RelayCommand]
        public void StationsSelectionChangedCommand(object o)
        {
            if (_appStateStore!.IsFromStationDetailsView)
            {
                _appStateStore!.IsFromStationDetailsView = false;
                return;
            }

            if (o is not SelectionChangedEventArgs e) return;
            _appStateStore!.LastSelectedStationIndex = ((ListBox)e.Source!).SelectedIndex;
            _appStateStore!.LastSelectedStation = ((ListBox)e.Source!).SelectedItem as DisplayStation;
            _navigationService!.Navigate<StationDetailsViewModel, SlideLeftPageTransition>();
        }

        [RelayCommand]
        public void BackCommand()
        {
            _navigationService!.Navigate<AddressSelectionViewModel, CrossFade>();
        }

        #endregion commands

        #region private methods

        private void OnBackPressed()
        {
            _navigationService!.Navigate<AddressSelectionViewModel, CrossFade>();
        }
        #endregion private methods

        #region public overrides

        public override void Dispose()
        {
            ((App)Application.Current!).BackPressed -= OnBackPressed;
        }

        #endregion public overrides
    }
}