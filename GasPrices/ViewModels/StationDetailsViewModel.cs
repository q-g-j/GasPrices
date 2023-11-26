using Avalonia;
using Avalonia.Animation;
using CommunityToolkit.Mvvm.Input;
using GasPrices.PageTransitions;
using GasPrices.Services;
using GasPrices.Store;

namespace GasPrices.ViewModels;

public partial class StationDetailsViewModel : ViewModelBase
{
    #region constructors

    public StationDetailsViewModel()
    {
    }

    #endregion constructors

    public StationDetailsViewModel(NavigationService navigationService, AppStateStore appStateStore)
    {
        _navigationService = navigationService;
        _appStateStore = appStateStore;

        ((App)Application.Current!).BackPressed += OnBackPressed;
    }

    #region private fields

    private readonly NavigationService? _navigationService;
    private readonly AppStateStore? _appStateStore;

    #endregion private fields
    
    #region commands

    [RelayCommand]
    public void BackCommand()
    {
        OnBackPressed();
    }
    #endregion commands

    #region private methods

    private void OnBackPressed()
    {
        _navigationService!.Navigate<ResultsViewModel, SlideRightPageTransition>();
    }

    #endregion private methods


    #region public overrides

    public override void Dispose()
    {
        ((App)Application.Current!).BackPressed -= OnBackPressed;
    }

    #endregion public overrides
}