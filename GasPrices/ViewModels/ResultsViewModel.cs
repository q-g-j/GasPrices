using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GasPrices.Models;
using GasPrices.Services;
using GasPrices.Store;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace GasPrices.ViewModels
{
    public partial class ResultsViewModel : ViewModelBase
    {
        private readonly NavigationService<AddressSelectionViewModel> _addressSelectionNavigationService;
        private readonly SearchResultStore _searchResultStore;

        public ResultsViewModel(
            NavigationService<AddressSelectionViewModel> addressSelectionNavigationService,
            SearchResultStore searchResultStore)
        {
            _addressSelectionNavigationService = addressSelectionNavigationService;
            _searchResultStore = searchResultStore;
            Stations = [];
            foreach (var station in _searchResultStore.Stations!)
            {
                Stations.Add(new DisplayStation(station, _searchResultStore.SelectedGasType!));
            }
            PriceFor = searchResultStore!.SelectedGasType!.ToString()!;

            ((App)Avalonia.Application.Current!).BackPressed += OnBackPressed;
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
            _addressSelectionNavigationService.Navigate();
        }

        private void OnBackPressed()
        {
            _addressSelectionNavigationService.Navigate();
        }

        public override void Dispose()
        {
            ((App)Avalonia.Application.Current!).BackPressed -= OnBackPressed;
        }
    }
}
