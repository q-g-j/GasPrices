using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GasPrices.Models;
using GasPrices.Store;
using Xamarin.Essentials;

namespace GasPrices.ViewModels;

public partial class StationDetailsViewModel : ViewModelBase
{
    #region constructors

    public StationDetailsViewModel()
    {
    }

    public StationDetailsViewModel(AppStateStore appStateStore)
    {
        _appStateStore = appStateStore;
        _appStateStore.IsFromStationDetailsView = true;
        
        Station = appStateStore.SelectedStation;
    }

    #endregion constructors

    #region private fields

    private readonly AppStateStore? _appStateStore;

    #endregion private fields

    #region bindable properties

    [ObservableProperty] private DisplayStation? _station;

    #endregion bindable properties

    #region commands

    [RelayCommand]
    public async Task OpenInBrowser()
    {
        var url = $"https://www.google.com/maps/search/{Station!.GetUriData()}";
        await OpenBrowser(new Uri(url));
    }

    #endregion commands

    #region private methods

    private static async Task OpenBrowser(Uri uri)
    {
        if (OperatingSystem.IsAndroid())
        {
            try
            {
                await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception _)
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
            }
        }
    }

    #endregion private methods

    #region public overrides

    public override void Dispose()
    {
    }

    #endregion public overrides
}