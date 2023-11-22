using ApiClients;
using ApiClients.Models;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GasPrices.Services;
using HttpClient.Exceptions;
using SettingsFile.Models;
using SettingsFile.SettingsFile;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GasPrices.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        #region constructors
        public SettingsViewModel(
            NavigationService navigationService,
            SettingsFileReader? settingsFileReader,
            SettingsFileWriter? settingsFileWriter,
            IGasPricesClient gasPricesClient)
        {
            _navigationService = navigationService;
            _settingsFileReader = settingsFileReader;
            _settingsFileWriter = settingsFileWriter;
            _gasPricesClient = gasPricesClient;

            ((App)Application.Current!).BackPressed += OnBackPressed;

            Task.Run(async () =>
            {
                var settings = await _settingsFileReader!.ReadAsync();
                if (settings != null && !string.IsNullOrEmpty(settings.TankerkönigApiKey))
                {
                    TankerKönigApiKey = settings.TankerkönigApiKey;
                }
            });
        }
        #endregion constructors

        #region private fields
        private readonly NavigationService _navigationService;
        private readonly SettingsFileReader? _settingsFileReader;
        private readonly SettingsFileWriter? _settingsFileWriter;
        private readonly IGasPricesClient _gasPricesClient;

        private CancellationTokenSource? _cancellationTokenSource;
        private bool isValidated = false;
        #endregion private fields

        #region bindable properties
        [ObservableProperty]
        private string tankerKönigApiKey = string.Empty;

        [ObservableProperty]
        private bool validateButtonIsEnabled = false;

        [ObservableProperty]
        private bool saveButtonIsEnabled = false;

        [ObservableProperty]
        private string noticeTitleText = string.Empty;

        [ObservableProperty]
        private string noticeText = string.Empty;

        [ObservableProperty]
        private IBrush noticeTextColor = new SolidColorBrush(Color.Parse("Orange"));

        [ObservableProperty]
        private bool noticeTextIsVisible = false;

        [ObservableProperty]
        private bool progressRingIsActive = false;
        #endregion bindable properties

        #region commands
        [RelayCommand]
        public async Task KeyDownCommand(object sender)
        {
            var e = sender as KeyEventArgs;
            if (e?.Key == Key.Enter || e?.Key == Key.Return)
            {
                e.Handled = true;
                if (isValidated)
                {
                    await SaveCommand();
                }
                else if (ValidateButtonIsEnabled)
                {
                    await ValidateCommand();
                }
            }
        }

        [RelayCommand]
        public async Task ValidateCommand()
        {
            var coords = new Coords(11.601314, 48.135788);

            List<Station>? stations;

            ProgressRingIsActive = true;

            try
            {
                stations = await _gasPricesClient.GetStationsAsync(TankerKönigApiKey, coords, 1);

                if (stations == null)
                {
                    ShowWarning(true, "Der API-Schlüssel wurde nicht angenommen!", 2000);
                }
                else
                {
                    isValidated = true;
                    SaveButtonIsEnabled = true;
                    ShowWarning(false, "Der API-Schlüssel wurde angenommen!", 2000);                 
                }
            }
            catch (HttpClientException ex)
            {
                var message = new StringBuilder();
                message.AppendLine("Keine Verbindung zu Tankerkönig.de!\n");
                message.AppendLine("Fehlermeldung:");
                message.Append(ex.Message);
                ShowWarning(true, message.ToString(), 5000);
            }
            catch (BadStatuscodeException ex)
            {
                var message = new StringBuilder();
                message.AppendLine("Keine Verbindung zu Tankerkönig.de!\n");
                message.Append($"HTTP-Status-Code: {ex.StatusCode.ToString()}");
                ShowWarning(true, message.ToString(), 5000);
            }
            catch (Exception)
            {
                ShowWarning(true, "Keine Verbindung zu Tankerkönig.de!", 5000);
            }
            finally
            {
                ProgressRingIsActive = false;
            }
        }

        [RelayCommand]
        public async Task SaveCommand()
        {
            var settings = new Settings
            {
                TankerkönigApiKey = TankerKönigApiKey
            };
            await _settingsFileWriter!.WriteAsync(settings);
            _navigationService.Navigate<AddressSelectionViewModel>();
        }

        [RelayCommand]
        public void CancelCommand()
        {
            _navigationService.Navigate<AddressSelectionViewModel>();
        }
        #endregion commands

        #region private methods
        private void ShowWarning(bool isError, string message, int duration)
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource = null;
            }

            _cancellationTokenSource = new CancellationTokenSource();

            if (isError )
            {
                NoticeTextColor = new SolidColorBrush(Color.Parse("Orange"));
                NoticeTitleText = "Fehler:";
            }
            else
            {
                NoticeTextColor = new SolidColorBrush(Color.Parse("Green"));
                NoticeTitleText = "Erfolg:";
            }

            var token = _cancellationTokenSource.Token;
            Task.Run(async () =>
            {
                NoticeText = message;
                NoticeTextIsVisible = true;

                await Task.Delay(duration);

                token.ThrowIfCancellationRequested();

                NoticeTitleText = string.Empty;
                NoticeText = string.Empty;
                NoticeTextIsVisible = false;
            }, token);
        }

        private void OnBackPressed()
        {
            _navigationService.Navigate<AddressSelectionViewModel>();
        }
        #endregion private methods

        #region OnPropertyChanged handlers
        partial void OnTankerKönigApiKeyChanged(string value)
        {
            ValidateButtonIsEnabled = !string.IsNullOrEmpty(value);
        }
        #endregion OnPropertyChanged handlers

        #region public overrides
        public override void Dispose()
        {
            ((App)Application.Current!).BackPressed -= OnBackPressed;
        }
        #endregion public overrides
    }
}
