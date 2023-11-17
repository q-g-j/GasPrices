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
using System.Runtime.CompilerServices;
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
        private readonly SettingsFileWriter _settingsFileWriter;
        private Settings? _settings = null;

        [ObservableProperty]
        private string street = string.Empty;

        [ObservableProperty]
        private string postalCode = string.Empty;

        [ObservableProperty]
        private string city = string.Empty;

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
            SettingsFileReader settingsFileReader,
            SettingsFileWriter settingsFileWriter)
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
            _settingsFileWriter = settingsFileWriter;

            if (OperatingSystem.IsAndroid())
            {
                GeolocationButtonIsVisible = true;
            }

            Task.Run(async () =>
            {
                await ProcessApiKeyAsync();
                await ProcessSettingsAsync();
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
            catch (FeatureNotSupportedException)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException)
            {
                // Handle not enabled on device exception
            }
            catch (PermissionException)
            {
                // Handle permission exception
            }
            catch (Exception)
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
                    Street = address.Street!;
                    PostalCode = address.PostalCode!;
                    City = address.City!;
                }
            }
        }

        [RelayCommand]
        public async Task SearchCommand()
        {
            SearchButtonIsEnabled = false;
            var address = new Address(Street, City, PostalCode);
            var coords = await _mapClient.GetCoordsAsync(address);
            if (coords is null)
            {
                SearchButtonIsEnabled = true;
                return;
            }

            var stations = await _gasPricesClient.GetStationsAsync(
                _settings?.TankerkönigApiKey!,
                coords,
                Distance);
            if (stations is null)
            {
                SearchButtonIsEnabled = true;
                return;
            }

            _searchResultStore.Stations = stations;
            await SaveCurrentAddressAsync();
            _resultsNavigationService.Navigate();
        }

        [RelayCommand]
        public async Task OpenSettingsCommand()
        {
            await SaveCurrentAddressAsync();
            _settingsNavigationService.Navigate();
        }

        public override void Dispose()
        {
        }

        private async Task ProcessApiKeyAsync()
        {
            _settings = await _settingsFileReader.ReadAsync();
            if (_settings == null && string.IsNullOrEmpty(_settings?.TankerkönigApiKey))
            {
                SearchButtonIsEnabled = false;
                var warning = new StringBuilder();
                warning.Append("Es wurde kein Tankerkönig-API-Schlüssel\n");
                warning.Append("gefunden!\n\n");
                warning.Append("Bitte zu den Einstellungen wechseln.");
                WarningText = warning.ToString();
                WarningTextIsVisible = true;
            }
        }

        private async Task ProcessSettingsAsync()
        {
            _settings = await _settingsFileReader.ReadAsync();

            if (_searchResultStore.SelectedGasType != null)
            {
                GasTypeSelectedItem = GasTypes.FirstOrDefault(gt => gt.ToString() == _searchResultStore.SelectedGasType.ToString());
            }
            else if (_settings != null && _settings.LastKnownGasType != null)
            {
                GasTypeSelectedItem = GasTypes.FirstOrDefault(gt => gt.ToString() == _settings.LastKnownGasType.ToString());
            }

            if (_searchResultStore.Distance != null)
            {
                Distance = _searchResultStore.Distance.Value;
            }
            else if (_settings != null && _settings.LastKnownDistance != null)
            {
                Distance = _settings.LastKnownDistance.Value;
            }

            if (_searchResultStore.Address != null)
            {
                Street = _searchResultStore.Address.Street!;
                PostalCode = _searchResultStore.Address.PostalCode!;
                City = _searchResultStore.Address.City!;
            }
            else if (_settings != null)
            {
                Street = _settings.LastKnownStreet ?? string.Empty;
                PostalCode = _settings.LastKnownPostalCode ?? string.Empty;
                City = _settings.LastKnownCity ?? string.Empty;
            }
        }

        private async Task SaveCurrentAddressAsync()
        {
            var address = new Address(Street, City, PostalCode);
            _searchResultStore.Address = address;
            _searchResultStore.SelectedGasType = GasTypeSelectedItem;
            _searchResultStore.Distance = Distance;

            var settings = await _settingsFileReader.ReadAsync();
            if (settings == null)
            {
                settings = new Settings();
            }

            settings.LastKnownStreet = Street;
            settings.LastKnownCity = City;
            settings.LastKnownPostalCode = PostalCode;
            settings.LastKnownDistance = Distance;
            settings.LastKnownGasType = GasTypeSelectedItem?.ToString();
            await _settingsFileWriter.WriteAsync(settings);
        }
    }
}
