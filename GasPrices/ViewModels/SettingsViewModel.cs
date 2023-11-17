using ApiClients;
using ApiClients.Models;
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
        private readonly NavigationService<AddressSelectionViewModel> _addressSelectionNavigationService;
        private readonly SettingsFileReader? _settingsFileReader;
        private readonly SettingsFileWriter? _settingsFileWriter;
        private readonly IGasPricesClient _gasPricesClient;

        public SettingsViewModel(
            NavigationService<AddressSelectionViewModel> addressSelectionNavigationService,
            SettingsFileReader? settingsFileReader,
            SettingsFileWriter? settingsFileWriter,
            IGasPricesClient gasPricesClient)
        {
            _addressSelectionNavigationService = addressSelectionNavigationService;
            _settingsFileReader = settingsFileReader;
            _settingsFileWriter = settingsFileWriter;
            _gasPricesClient = gasPricesClient;

            ((App)Avalonia.Application.Current!).BackPressed += OnBackPressed;

            Task.Run(async () =>
            {
                var settings = await _settingsFileReader!.ReadAsync();
                if (settings != null && !string.IsNullOrEmpty(settings.TankerKönigApiKey))
                {
                    TankerKönigApiKey = settings.TankerKönigApiKey;
                }
            });
        }

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
        public async Task ValidateCommand()
        {
            var coords = new Coords(48.135788, 11.601314);

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
                TankerKönigApiKey = TankerKönigApiKey
            };
            await _settingsFileWriter!.WriteAsync(settings);
            _addressSelectionNavigationService.Navigate();
        }

        [RelayCommand]
        public void CancelCommand()
        {
            _addressSelectionNavigationService.Navigate();
        }

        partial void OnTankerKönigApiKeyChanged(string value)
        {
            ValidateButtonIsEnabled = !string.IsNullOrEmpty(value);
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
