using Avalonia;
using Avalonia.Animation;
using CommunityToolkit.Mvvm.ComponentModel;
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

    public StationDetailsViewModel(NavigationService? navigationService)
    {
        _navigationService = navigationService;
    }
    #endregion constructors

    #region private fields
    private readonly NavigationService? _navigationService;
    #endregion private fields 
    
    #region bindable properties
    #endregion bindable properties
    
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