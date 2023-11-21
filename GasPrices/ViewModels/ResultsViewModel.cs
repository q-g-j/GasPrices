using ApiClients.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GasPrices.Models;
using GasPrices.Services;
using GasPrices.Store;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace GasPrices.ViewModels
{
    public partial class ResultsViewModel : ViewModelBase
    {
        public ResultsViewModel(
            NavigationService navigationService,
            AppStateStore appStateStore)
        {
            _navigationService = navigationService;
            _appStateStore = appStateStore;
            Stations = [];
            foreach (var station in _appStateStore.Stations!.Where(s => s.E5 > 0 && s.E10 > 0 && s.Diesel > 0))
            {
                Stations.Add(new DisplayStation(station, _appStateStore.SelectedGasType!));
            }
            PriceFor = appStateStore!.SelectedGasType!.ToString()!;


            ((App)Application.Current!).BackPressed += OnBackPressed;
        }

        private readonly NavigationService _navigationService;
        private readonly AppStateStore _appStateStore;


        [ObservableProperty]
        private ObservableCollection<DisplayStation>? stations;

        [ObservableProperty]
        private int selectedIndex = -1;

        [ObservableProperty]
        private object? selectedItem = null;

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
        public void TappedCommand()
        {
            SelectedIndex = -1;
        }

        [RelayCommand]
        public void StationsSelectionChangedCommand(object o)
        {
            if (o is SelectionChangedEventArgs e)
            {
                if (e.AddedItems.Count > 0)
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
                else
                {
                    DetailsIsVisible = false;
                }
            }
        }

        [RelayCommand]
        public void StationsSortingCommand()
        {
            if (SelectedItem != null) return;

            if (Stations?.Count > 0)
            {
                SelectedItem = 0;
                SelectedItem = -1;
            }
        }

        [RelayCommand]
        public async Task OpenInMapCommand()
        {
            if (SelectedItem is DisplayStation selectedStation)
            {
                var url = $"https://www.google.com/maps/search/{selectedStation.GetUriData()}";
                await OpenBrowser(new Uri(url));
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
            ((App)Application.Current!).BackPressed -= OnBackPressed;
        }

        private async Task OpenBrowser(Uri uri)
        {
            if (OperatingSystem.IsAndroid())
            {
                try
                {
                    await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
                }
                catch (Exception)
                {
                }
            }
            else
            {
                try
                {
                    var processStartInfo = new ProcessStartInfo
                    {
                        FileName = uri.ToString(),
                        UseShellExecute = true
                    };

                    Process.Start(processStartInfo);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
