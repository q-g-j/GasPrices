using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GasPrices.Models;
using GasPrices.Services;
using GasPrices.Store;
using System.Collections.ObjectModel;

namespace GasPrices.ViewModels
{
    public partial class ResultsViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;
        private readonly AppStateStore _appStateStore;

        public ResultsViewModel(
            NavigationService navigationService,
            AppStateStore appStateStore)
        {
            _navigationService = navigationService;
            _appStateStore = appStateStore;
            Stations = [];
            foreach (var station in _appStateStore.Stations!)
            {
                Stations.Add(new DisplayStation(station, _appStateStore.SelectedGasType!));
            }
            PriceFor = appStateStore!.SelectedGasType!.ToString()!;

            ((App)Application.Current!).BackPressed += OnBackPressed;
        }

        [ObservableProperty]
        private ObservableCollection<DisplayStation>? stations;

        [ObservableProperty]
        private string priceFor = "Preis";

        [ObservableProperty]
        private bool detailsIsVisible = false;

        [ObservableProperty]
        private string detailsName = "";

        [ObservableProperty]
        private string detailsBrand = "";

        [ObservableProperty]
        private string detailsStreet = "";

        [ObservableProperty]
        private string detailsCity = "";

        [ObservableProperty]
        private string detailsE5 = "";

        [ObservableProperty]
        private string detailsE10 = "";

        [ObservableProperty]
        private string detailsDiesel = "";

        [RelayCommand]
        public void StationsSelectionChangedCommand(object o)
        {
            var e = o as SelectionChangedEventArgs;
            if (e != null)
            {
                var station = e.AddedItems[0] as DisplayStation;
                DetailsName = station!.Name;
                DetailsBrand = station!.Brand;
                DetailsStreet = station!.Street;
                DetailsCity = station!.PostalCode + " " + station!.City;
                DetailsE5 = station!.E5;
                DetailsE10 = station!.E10;
                DetailsDiesel = station!.Diesel;

                DetailsIsVisible = true;
            }
        }

        [RelayCommand]
        public void BackCommand()
        {
            _navigationService.Navigate<AddressSelectionViewModel>();
        }

        private void OnBackPressed()
        {
            _navigationService.Navigate<AddressSelectionViewModel>();
        }

        public override void Dispose()
        {
            ((App)Avalonia.Application.Current!).BackPressed -= OnBackPressed;
        }
    }
}
