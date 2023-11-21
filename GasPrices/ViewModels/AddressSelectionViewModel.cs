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
            AppStateStore appStateStore,
            SettingsFileReader settingsFileReader,
            SettingsFileWriter settingsFileWriter,
            NavigationService navigationService)
        {
            _navigationService = navigationService;

            _mapClient = mapClient;
            _appStateStore = appStateStore;
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
        private readonly AppStateStore _appStateStore;
        private readonly IMapClient _mapClient;
        private readonly IGasPricesClient _gasPricesClient;
        private readonly SettingsFileReader _settingsFileReader;
        private readonly SettingsFileWriter _settingsFileWriter;
        private Settings? _settings = null;
        private bool _hasStreetFocus = false;
        private bool _hasPostalCodeFocus = false;
        private bool _hasCityFocus = false;
        private CancellationTokenSource? _cancellationTokenSource;
        #endregion private fields

        #region private properties
        private int RadiusInt
        {
            get
            {
                var isDigit = int.TryParse(Radius, out int intValue);
                return isDigit ? intValue : 5;
            }
        }
        #endregion

        #region bindable properties
        [ObservableProperty]
        private string street = string.Empty;

        [ObservableProperty]
        private string postalCode = string.Empty;

        [ObservableProperty]
        private string city = string.Empty;

        [ObservableProperty]
        private string radius = "5";

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

            Xamarin.Essentials.Location? location = null;

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
                _appStateStore.CoordsFromMapClient = new Coords(location.Latitude, location.Longitude);
                await ProcessCoordsAsync();
            }
        }

        [RelayCommand]
        public async Task LocationPickerCommand()
        {
            await SaveCurrentAddressAsync();
            _appStateStore.CoordsFromMapClient = await GetCoordsFromAddressFields();
            _navigationService.Navigate<LocationPickerViewModel>();
        }

        [RelayCommand]
        public async Task SearchCommand()
        {
            if (Radius == "0")
            {
                ShowWarning("Der Umkreis muss mindestens 1 km betragen!", 5000);
                return;
            }
            var isDigit = int.TryParse(Radius, out int _);
            if (!isDigit)
            {
                ShowWarning("Ungültige Eingabe für den Umkreis!", 5000);
                return;
            }

            SearchButtonIsEnabled = false;
            ProgressRingIsActive = true;

            Coords? coords;

            if (_appStateStore.CoordsFromMapClient != null)
            {
                coords = _appStateStore.CoordsFromMapClient;
            }
            else
            {
                coords = await GetCoordsFromAddressFields();
            }

            if (coords != null)
            {
                var stations = await _gasPricesClient.GetStationsAsync(_settings!.TankerkönigApiKey!, coords, RadiusInt);
                if (stations != null && stations?.Count > 0)
                {
                    _appStateStore!.Stations = stations;
                    await SaveCurrentAddressAsync();
                    _appStateStore.CoordsFromMapClient = null;
                    _navigationService.Navigate<ResultsViewModel>();
                }
                else
                {
                    ShowWarning("Es wurden keine Tankstellen gefunden!", 5000);
                }
            }
            else
            {
                ShowWarning("Es wurde keine gültige Adresse eingegeben!", 5000);
            }

            ProgressRingIsActive = false;
            SearchButtonIsEnabled = true;
        }

        [RelayCommand]
        public async Task OpenSettingsCommand()
        {
            await SaveCurrentAddressAsync();
            _navigationService.Navigate<SettingsViewModel>();
        }

        [RelayCommand]
        public async Task KeyDownCommand(object o)
        {
            var e = o as KeyEventArgs;
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
            if (_settings == null || string.IsNullOrEmpty(_settings?.TankerkönigApiKey))
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

            if (_appStateStore.SelectedGasType != null)
            {
                GasTypeSelectedItem = GasTypes.FirstOrDefault(gt => gt.ToString() == _appStateStore.SelectedGasType.ToString());
            }
            else if (_settings != null && _settings.LastKnownGasType != null)
            {
                GasTypeSelectedItem = GasTypes.FirstOrDefault(gt => gt.ToString() == _settings.LastKnownGasType.ToString());
            }

            if (_appStateStore.Distance != null)
            {
                Radius = _appStateStore.Distance.Value.ToString();
            }
            else if (_settings != null && _settings.LastKnownRadius != null)
            {
                Radius = _settings.LastKnownRadius;
            }

            if (_appStateStore.CoordsFromMapClient != null) return;

            if (_settings?.LastKnownLatitude != null && _settings.LastKnownLongitude != null)
            {
                _appStateStore.CoordsFromMapClient = new Coords(
                    _settings.LastKnownLatitude.Value, _settings.LastKnownLongitude.Value);
                MapCoordinates = _appStateStore.CoordsFromMapClient!.ToString();
                MapCoordinatesIsVisible = true;
            }

            if (_appStateStore.Address != null)
            {
                Street = _appStateStore.Address.Street!;
                PostalCode = _appStateStore.Address.PostalCode!;
                City = _appStateStore.Address.City!;
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
            if (_appStateStore.CoordsFromMapClient == null)
            {
                return;
            }

            ProgressRingIsActive = true;
            var address = await _mapClient.GetAddressAsync(_appStateStore.CoordsFromMapClient!);
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
                MapCoordinates = _appStateStore.CoordsFromMapClient!.ToString();
                MapCoordinatesIsVisible = true;
                _appStateStore.Address = address;
                Street = _appStateStore.Address?.Street!;
                PostalCode = _appStateStore.Address?.PostalCode!;
                City = _appStateStore.Address?.City!;
            }

            if (isWrongPosition)
            {
                ShowWarning(wrongPosWarningMsg.ToString(), 5000);
            }
        }

        private async Task SaveCurrentAddressAsync(Coords? coords = null)
        {
            var address = new Address(Street, City, PostalCode);
            _appStateStore.Address = address;
            _appStateStore.SelectedGasType = GasTypeSelectedItem;
            _appStateStore.Distance = RadiusInt;

            var settings = await _settingsFileReader.ReadAsync();
            settings ??= new Settings();

            settings.LastKnownStreet = Street;
            settings.LastKnownCity = City;
            settings.LastKnownPostalCode = PostalCode;
            settings.LastKnownRadius = RadiusInt.ToString();
            settings.LastKnownGasType = GasTypeSelectedItem?.ToString();

            if (_appStateStore.CoordsFromMapClient != null)
            {
                settings.LastKnownLatitude = _appStateStore.CoordsFromMapClient?.Latitude;
                settings.LastKnownLongitude = _appStateStore.CoordsFromMapClient?.Longitude;
            }
            else
            {
                if (coords != null)
                {
                    settings.LastKnownLatitude = coords.Latitude;
                    settings.LastKnownLongitude = coords.Longitude;
                }
                else
                {
                    settings.LastKnownLatitude = null;
                    settings.LastKnownLongitude = null;
                }
            }

            await _settingsFileWriter.WriteAsync(settings);
        }

        private void ResetCachedCoords()
        {
            if (_hasStreetFocus || _hasPostalCodeFocus || _hasCityFocus)
            {
                _appStateStore.CoordsFromMapClient = null;
                MapCoordinates = null;
                MapCoordinatesIsVisible = false;
            }
        }

        private async Task<Coords?> GetCoordsFromAddressFields()
        {
            var address = new Address(Street, City, PostalCode);
            var coords = await _mapClient.GetCoordsAsync(address);

            return coords;
        }

        private void ShowWarning(string message, int duration)
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource = null;
            }

            _cancellationTokenSource = new CancellationTokenSource();

            var token = _cancellationTokenSource.Token;
            Task.Run(async () =>
            {
                WarningText = message;
                WarningTextIsVisible = true;

                await Task.Delay(duration);

                token.ThrowIfCancellationRequested();

                WarningText = string.Empty;
                WarningTextIsVisible = false;
            }, token);
        }
        #endregion private methods

        #region public methods
        public override void Dispose()
        {
        }
        #endregion public methods
    }
}
