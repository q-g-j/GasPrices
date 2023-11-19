using ApiClients;
using ApiClients.Models;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GasPrices.Services;
using SettingsFile.Models;
using SettingsFile.SettingsFile;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace GasPrices.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;
        private readonly SettingsFileReader? _settingsFileReader;
        private readonly SettingsFileWriter? _settingsFileWriter;
        private readonly IGasPricesClient _gasPricesClient;

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

        private bool isValidated = false;

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

            var stations = await _gasPricesClient.GetStationsAsync(TankerKönigApiKey, coords, 1);
            if (stations == null)
            {
                await Task.Run(() =>
                {
                    NoticeTitleText = "Fehler:";
                    NoticeText = "Der API-Schlüssel wurde nicht angenommen!";
                    Dispatcher.UIThread.Invoke(() =>
                        NoticeTextColor = new SolidColorBrush(Color.Parse("Orange")));
                    NoticeTextIsVisible = true;
                    Thread.Sleep(2000);
                    NoticeTextIsVisible = false;
                    NoticeTitleText = string.Empty;
                    NoticeText = string.Empty;
                });
            }
            else
            {
                SaveButtonIsEnabled = true;

                await Task.Run(() =>
                {
                    NoticeTitleText = "Erfolg:";
                    NoticeText = "Der API-Schlüssel wurde angenommen!";
                    Dispatcher.UIThread.Invoke(() =>
                        NoticeTextColor = new SolidColorBrush(Color.Parse("Green")));
                    NoticeTextIsVisible = true;
                    isValidated = true;
                    Thread.Sleep(2000);
                    NoticeTextIsVisible = false;
                    NoticeTitleText = string.Empty;
                    NoticeText = string.Empty;
                });
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

        partial void OnTankerKönigApiKeyChanged(string value)
        {
            ValidateButtonIsEnabled = !string.IsNullOrEmpty(value);
        }

        protected void OnBackPressed()
        {
            _navigationService.Navigate<AddressSelectionViewModel>();
        }

        public override void Dispose()
        {
            ((App)Application.Current!).BackPressed -= OnBackPressed;
        }
    }
}
