using Avalonia;
using Avalonia.Controls;
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
using Avalonia.Animation;
using GasPrices.PageTransitions;
using Xamarin.Essentials;

namespace GasPrices.ViewModels
{
    public partial class ResultsViewModel : ViewModelBase
    {
        #region constructors

        public ResultsViewModel()
        {
        }
        
        public ResultsViewModel(
            NavigationService navigationService,
            AppStateStore appStateStore)
        {
            _navigationService = navigationService;
            
            Stations = [];
            foreach (var station in appStateStore.Stations!.Where(s => s is { E5: > 0, E10: > 0, Diesel: > 0 }))
            {
                Stations.Add(new DisplayStation(station, appStateStore.SelectedGasType!));
            }
            PriceFor = appStateStore.SelectedGasType!.ToString()!;


            ((App)Application.Current!).BackPressed += OnBackPressed;
        }
        #endregion constructors

        #region private fields
        private readonly NavigationService? _navigationService;
        #endregion privat fields

        #region bindable properties
        [ObservableProperty] private ObservableCollection<DisplayStation>? _stations;
        [ObservableProperty] private int _selectedIndex = -1;
        [ObservableProperty] private object? _selectedItem;
        [ObservableProperty] private string _priceFor = "Preis";
        [ObservableProperty] private bool _detailsIsVisible;
        [ObservableProperty] private string _detailsName = string.Empty;
        [ObservableProperty] private string _detailsBrand = string.Empty;
        [ObservableProperty] private string _detailsStreet = string.Empty;
        [ObservableProperty] private string _detailsCity = string.Empty;
        [ObservableProperty] private string _detailsE5 = string.Empty;
        [ObservableProperty] private string _detailsE10 = string.Empty;
        [ObservableProperty] private string _detailsDiesel = string.Empty;
        #endregion bindable properties

        #region commands
        [RelayCommand]
        public void TappedCommand()
        {
            SelectedIndex = -1;
        }

        [RelayCommand]
        public void StationsSelectionChangedCommand(object o)
        {
            if (o is not SelectionChangedEventArgs) return;
            _navigationService!.Navigate<StationDetailsViewModel, SlideLeftPageTransition>();
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
            _navigationService!.Navigate<AddressSelectionViewModel, CrossFade>();
        }
        #endregion commands

        #region private methods
        private void OnBackPressed()
        {
            _navigationService!.Navigate<AddressSelectionViewModel, CrossFade>();
        }

        private static async Task OpenBrowser(Uri uri)
        {
            if (OperatingSystem.IsAndroid())
            {
                try
                {
                    await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
                }
                catch (Exception)
                {
                    // ignore
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
                    // ignore
                }
            }
        }
        #endregion private methods

        #region public overrides
        public override void Dispose()
        {
            ((App)Application.Current!).BackPressed -= OnBackPressed;
        }
        #endregion public overrides
    }
}
