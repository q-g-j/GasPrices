using ApiClients;
using ApiClients.Models;
using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GasPrices.Models;
using GasPrices.Services;
using GasPrices.Store;
using SettingsFile.Models;
using SettingsFile.SettingsFile;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace GasPrices.ViewModels
{
    public partial class AddressSelectionViewModel : ViewModelBase
    {
        //private readonly NavigationService<ResultsViewModel> _resultsNavigationService;
        //private readonly NavigationService<SettingsViewModel> _settingsNavigationService;
        //private readonly NavigationService<LocationPickerViewModel> _locationPickerNavigationService;
        private readonly NavigationService _navigationService;
        private readonly SearchResultStore _searchResultStore;
        private readonly IMapClient _mapClient;
        private readonly IGasPricesClient _gasPricesClient;
        private readonly SettingsFileReader _settingsFileReader;
        private readonly SettingsFileWriter _settingsFileWriter;
        private Settings? _settings = null;

        public AddressSelectionViewModel(
            //NavigationService<SettingsViewModel> settingsNavigationService,
            //NavigationService<ResultsViewModel> resultsNavigationService,
            //NavigationService<LocationPickerViewModel> locationPickerNavigationService,
            IMapClient mapClient,
            IGasPricesClient gasPricesClient,
            SearchResultStore searchResultStore,
            SettingsFileReader settingsFileReader,
            SettingsFileWriter settingsFileWriter,
            NavigationService navigationService)
        {
            //_settingsNavigationService = settingsNavigationService;
            //_resultsNavigationService = resultsNavigationService;
            //_locationPickerNavigationService = locationPickerNavigationService;
            _navigationService = navigationService;

            _mapClient = mapClient;
            _searchResultStore = searchResultStore;
            _gasPricesClient = gasPricesClient;
            _settingsFileReader = settingsFileReader;
            _settingsFileWriter = settingsFileWriter;

            gasTypes =
            [
                new GasType("E5"),
                new GasType("E10"),
                new GasType("Diesel")
            ];

            gasTypeSelectedItem = gasTypes[0];

            if (OperatingSystem.IsAndroid())
            {
                GeolocationButtonIsVisible = true;
                LocationPickerButtonGridColumn = 1;
            }

            Task.Run(async () =>
            {
                await ProcessApiKeyAsync();
                await ProcessSettingsAsync();
            });
        }

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
        private bool locationPickerButtonIsEnabled = true;

        [ObservableProperty]
        private int locationPickerButtonGridColumn = 0;

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
                coords = new Coords(location.Latitude, location.Longitude);
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
        public async Task LocationPickerCommand()
        {
            await SaveCurrentAddressAsync();
            _searchResultStore.Coords = await _mapClient.GetCoordsAsync(_searchResultStore.Address!);
            _navigationService.Navigate<LocationPickerViewModel>();
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

            var stations = await _gasPricesClient.GetStationsAsync(_settings!.TankerkönigApiKey!, coords, Distance);
            if (stations is null)
            {
                SearchButtonIsEnabled = true;
                return;
            }

            _searchResultStore.Stations = stations;
            await SaveCurrentAddressAsync();
            _navigationService.Navigate<ResultsViewModel>();
        }

        [RelayCommand]
        public async Task OpenSettingsCommand()
        {
            await SaveCurrentAddressAsync();
            _navigationService.Navigate<SettingsViewModel>();
        }

        [RelayCommand]
        public async Task KeyDownCommand(object sender)
        {
            var e = sender as KeyEventArgs;
            if (e?.Key == Key.Enter || e?.Key == Key.Return)
            {
                if (SearchButtonIsEnabled)
                {
                    e.Handled = true;
                    await SearchCommand();
                }
            }
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
        public override void Dispose()
        {
        }
    }
}
