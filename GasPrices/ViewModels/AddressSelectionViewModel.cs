using ApiClients;
using ApiClients.Models;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GasPrices.Models;
using GasPrices.Services;
using GasPrices.Store;
using MsBox.Avalonia;
using SettingsFile.Models;
using SettingsFile.SettingsFile;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace GasPrices.ViewModels
{
    public partial class AddressSelectionViewModel : ViewModelBase
    {
        private readonly NavigationService<ResultsViewModel> _resultsNavigationService;
        private readonly NavigationService<SettingsViewModel> _settingsNavigationService;
        private readonly SearchResultStore _searchResultStore;
        private readonly IMapClient _mapClient;
        private readonly IGasPricesClient _gasPricesClient;
        private readonly SettingsFileReader _settingsFileReader;
        private Settings? _settings = null;

        [ObservableProperty]
        private string street = string.Empty;

        [ObservableProperty]
        private string postalCode = string.Empty;

        [ObservableProperty]
        private string city = string.Empty;

        [ObservableProperty]
        private string country = string.Empty;

        [ObservableProperty]
        private int distance = 5;

        [ObservableProperty]
        private ObservableCollection<GasType> gasTypes;

        [ObservableProperty]
        private GasType? gasTypeSelectedItem;

        [ObservableProperty]
        private bool geolocationButtonIsVisible = false;

        [ObservableProperty]
        private bool geolocationButtonIsEnabled = true;

        [ObservableProperty]
        private bool searchButtonIsEnabled = true;

        [ObservableProperty]
        private bool progressRingIsActive = false;

        [ObservableProperty]
        private bool warningTextIsVisible = false;

        [ObservableProperty]
        private bool errorTextIsVisible = false;

        [ObservableProperty]
        private string warningText = string.Empty;

        [ObservableProperty]
        private string errorText = string.Empty;

        public AddressSelectionViewModel(
            NavigationService<SettingsViewModel> settingsNavigationService,
            NavigationService<ResultsViewModel> resultsNavigationService,
            IMapClient mapClient,
            IGasPricesClient gasPricesClient,
            SearchResultStore searchResultStore,
            SettingsFileReader settingsFileReader)
        {
            _settingsNavigationService = settingsNavigationService;
            _resultsNavigationService = resultsNavigationService;

            gasTypes =
            [
                new GasType("E5"),
                new GasType("E10"),
                new GasType("Diesel"),
            ];

            gasTypeSelectedItem = gasTypes[0];
            _mapClient = mapClient;
            _searchResultStore = searchResultStore;
            _gasPricesClient = gasPricesClient;
            _settingsFileReader = settingsFileReader;

            if (searchResultStore.SelectedGasType != null)
            {
                GasTypeSelectedItem = GasTypes.FirstOrDefault(gt => gt.ToString() == searchResultStore.SelectedGasType.ToString());
            }

            if (searchResultStore.Distance != null)
            {
                Distance = searchResultStore.Distance.Value;
            }

            if (searchResultStore.Address != null)
            {
                Street = searchResultStore.Address.Street;
                PostalCode = searchResultStore.Address.PostalCode;
                City = searchResultStore.Address.City;
                Country = searchResultStore.Address.Country;
            }

            if (OperatingSystem.IsAndroid())
            {
                GeolocationButtonIsVisible = true;
            }

            Task.Run(async () =>
            {
                _settings = await _settingsFileReader.ReadAsync();
                if (_settings == null && string.IsNullOrEmpty(_settings?.TankerKönigApiKey))
                {
                    SearchButtonIsEnabled = false;
                    var warning = new StringBuilder();
                    warning.Append("Es wurde kein Tankerkönig-API-Schlüssel\n");
                    warning.Append("gefunden!\n\n");
                    warning.Append("Bitte zu den Einstellungen wechseln.");
                    WarningText = warning.ToString();
                    WarningTextIsVisible = true;
                }
            });
        }

        [RelayCommand]
        public async Task GeolocationCommand()
        {
            GeolocationButtonIsEnabled = false;
            ProgressRingIsActive = true;

            Coords? coords;
            Location? location = null;

            try
            {
                location = await Geolocation.GetLocationAsync();
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to get location
            }
            finally
            {
                ProgressRingIsActive = false;
                GeolocationButtonIsEnabled = true;
            }

            if (location != null)
            {
                coords = new Coords(location.Longitude, location.Latitude);
                var address = await _mapClient.GetAddressAsync(coords);
                if (address != null)
                {
                    Street = address.Street;
                    PostalCode = address.PostalCode;
                    City = address.City;
                    Country = address.Country;
                }
            }
        }

        [RelayCommand]
        public async Task SearchCommand()
        {
            SearchButtonIsEnabled = false;
            var address = new Address(Street, City, PostalCode, Country);
            var coords = await _mapClient.GetCoordsAsync(address);
            if (coords is null)
            {
                SearchButtonIsEnabled = true;
                return;
            }

            var stations = await _gasPricesClient.GetStationsAsync(
                _settings?.TankerKönigApiKey!,
                coords,
                Distance);
            if (stations is null)
            {
                SearchButtonIsEnabled = true;
                return;
            }

            _searchResultStore.Stations = stations;
            SaveCurrentAddress();
            _resultsNavigationService.Navigate();
        }

        [RelayCommand]
        public void OpenSettingsCommand()
        {
            SaveCurrentAddress();
            _settingsNavigationService.Navigate();
        }

        public override void Dispose()
        {
        }

        private void SaveCurrentAddress()
        {
            var address = new Address(Street, City, PostalCode, Country);
            _searchResultStore.Address = address;
            _searchResultStore.SelectedGasType = GasTypeSelectedItem;
            _searchResultStore.Distance = Distance;
        }
    }
}
