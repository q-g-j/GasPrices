using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GasPrices.PageTransitions;
using GasPrices.Services;
using GasPrices.Store;
using Xamarin.Essentials;

namespace GasPrices.ViewModels;

public partial class StationDetailsViewModel : ViewModelBase
{
    #region constructors
    public StationDetailsViewModel(AppStateStore? appStateStore)
    {
        _appStateStore = appStateStore;
    }

    public StationDetailsViewModel(NavigationService? navigationService, AppStateStore? appStateStore)
    {
        _navigationService = navigationService;
        _appStateStore = appStateStore;
    }
    #endregion constructors

    #region private fields
    private readonly NavigationService? _navigationService;
    private readonly AppStateStore? _appStateStore;
    #endregion private fields 
    
    #region bindable properties
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
    public Task OpenInMapCommand()
    {
        var url = $"https://www.google.com/maps/search/{_appStateStore!.LastSelectedStation!.GetUriData()}";
        return OpenBrowser(new Uri(url));
    }
    
    [RelayCommand]
    public void BackCommand()
    {
        OnBackPressed();
    }
    #endregion commands

    #region private methods
    private void OnBackPressed()
    {
        _appStateStore!.IsFromStationDetailsView = true;
        _navigationService!.Navigate<ResultsViewModel, SlideRightPageTransition>();
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