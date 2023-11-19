using ApiClients;
using ApiClients.Models;
using Avalonia.Input;
using Avalonia.Interactivity;
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
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace GasPrices.ViewModels
{
    public partial class AddressSelectionViewModel : ViewModelBase
    {
        #region constructors
        public AddressSelectionViewModel(
            IMapClient mapClient,
            IGasPricesClient gasPricesClient,
            SearchResultStore searchResultStore,
            SettingsFileReader settingsFileReader,
            SettingsFileWriter settingsFileWriter,
            NavigationService navigationService)
        {
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
                await ProcessCoordsAsync();
                await ProcessSettingsAsync();
            });
        }
        #endregion constructors

        #region private fields
        private readonly NavigationService _navigationService;
        private readonly SearchResultStore _searchResultStore;
        private readonly IMapClient _mapClient;
        private readonly IGasPricesClient _gasPricesClient;
        private readonly SettingsFileReader _settingsFileReader;
        private readonly SettingsFileWriter _settingsFileWriter;
        private Settings? _settings = null;
        private bool _hasStreetFocus = false;
        private bool _hasPostalCodeFocus = false;
        private bool _hasCityFocus = false;
        #endregion private fields

        #region bindable properties
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
        private string? mapCoordinates;

        [ObservableProperty]
        private bool mapCoordinatesIsVisible;

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
        #endregion bindable properties

        #region commands
        [RelayCommand]
        public void StreetFocusChangedCommand(object value) => _hasStreetFocus = value.ToString() == "True";

        [RelayCommand]
        public void PostalCodeFocusChangedCommand(object value) => _hasPostalCodeFocus = value.ToString() == "True";

        [RelayCommand]
        public void CityFocusChangedCommand(object value) => _hasCityFocus = value.ToString() == "True";

        [RelayCommand]
        public async Task GeolocationCommand()
        {
            MapCoordinates = string.Empty;
            MapCoordinatesIsVisible = false;

            GeolocationButtonIsEnabled = false;
            ProgressRingIsActive = true;

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
                _searchResultStore.Coords = new Coords(location.Latitude, location.Longitude);
                await ProcessCoordsAsync();
            }
        }

        [RelayCommand]
        public async Task LocationPickerCommand()
        {
            await SaveCurrentAddressAsync();
            _navigationService.Navigate<LocationPickerViewModel>();
        }

        [RelayCommand]
        public async Task SearchCommand()
        {
            SearchButtonIsEnabled = false;

            Coords? coords;

            if (_searchResultStore.Coords != null)
            {
                coords = _searchResultStore.Coords;
            }
            else
            {
                var address = new Address(Street, City, PostalCode);
                ProgressRingIsActive = true;
                coords = await _mapClient.GetCoordsAsync(address);
                ProgressRingIsActive = false;
                if (coords is null)
                {
                    SearchButtonIsEnabled = true;
                    return;
                }
            }

            var stations = await _gasPricesClient.GetStationsAsync(_settings!.TankerkönigApiKey!, coords, Distance);
            if (stations is null)
            {
                SearchButtonIsEnabled = true;
                return;
            }

            _searchResultStore!.Stations = stations;
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
        #endregion commands

        #region OnPropertyChangedHandlers
        partial void OnStreetChanged(string value) => ResetCachedCoords();
        partial void OnCityChanged(string value) => ResetCachedCoords();
        partial void OnPostalCodeChanged(string value) => ResetCachedCoords();
        #endregion OnPropertyChangedHandlers

        #region private methods
        private async Task ProcessApiKeyAsync()
        {
            _settings = await _settingsFileReader.ReadAsync();
            if (_settings == null && string.IsNullOrEmpty(_settings?.TankerkönigApiKey))
            {
                SearchButtonIsEnabled = false;
                var warning = new StringBuilder();
                warning.Append("Es wurde kein Tankerkönig-API-Schlüssel gefunden!\n\n");
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

            if (_searchResultStore.Coords != null) return;

            if (_settings?.LastKnownLatitude != null && _settings.LastKnownLongitude != null)
            {
                _searchResultStore.Coords = new Coords(
                    _settings.LastKnownLatitude.Value, _settings.LastKnownLongitude.Value);
                MapCoordinates = _searchResultStore.Coords!.ToString();
                MapCoordinatesIsVisible = true;
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

        private async Task ProcessCoordsAsync()
        {
            if (_searchResultStore.Coords == null)
            {
                return;
            }

            ProgressRingIsActive = true;
            var address = await _mapClient.GetAddressAsync(_searchResultStore.Coords!);
            ProgressRingIsActive = false;

            bool isWrongPosition = false;
            var wrongPosWarningMsg = new StringBuilder();
            if (address == null)
            {
                isWrongPosition = true;
                wrongPosWarningMsg.Append("Es konnte keine Addresse zu der gewünschten Position ermittelt werden. ");
                wrongPosWarningMsg.Append("Bitte eine andere Position oder Adresse wählen.");
            }
            else if (address.Country != "Deutschland")
            {
                isWrongPosition = true;
                wrongPosWarningMsg.Append("Diese App kann nur Tankstellen in Deutschland anzeigen. ");
                wrongPosWarningMsg.Append("Bitte eine andere Position oder Adresse wählen.");
            }
            else
            {
                MapCoordinates = _searchResultStore.Coords!.ToString();
                MapCoordinatesIsVisible = true;
                _searchResultStore.Address = address;
                Street = _searchResultStore.Address?.Street!;
                PostalCode = _searchResultStore.Address?.PostalCode!;
                City = _searchResultStore.Address?.City!;
            }

            if (isWrongPosition)
            {
                await Task.Run(() =>
                {
                    WarningText = wrongPosWarningMsg.ToString();
                    WarningTextIsVisible = true;
                    Thread.Sleep(5000);
                    WarningTextIsVisible = false;
                    WarningText = string.Empty;
                });
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
            settings.LastKnownLatitude = _searchResultStore.Coords?.Latitude;
            settings.LastKnownLongitude = _searchResultStore.Coords?.Longitude;

            _searchResultStore.Coords = null;
            await _settingsFileWriter.WriteAsync(settings);
        }

        private void ResetCachedCoords()
        {
            if (_hasStreetFocus || _hasPostalCodeFocus || _hasCityFocus)
            {
                _searchResultStore.Coords = null;
                MapCoordinates = null;
                MapCoordinatesIsVisible = false;
            }
        }
        #endregion private methods

        #region public methods
        public override void Dispose()
        {
        }
        #endregion public methods
    }
}
