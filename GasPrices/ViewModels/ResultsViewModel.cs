using System;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GasPrices.Services;
using GasPrices.Store;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using GasPrices.PageTransitions;

namespace GasPrices.ViewModels;

public partial class ResultsViewModel : ViewModelBase
{
    #region constructors

    public ResultsViewModel()
    {
    }

    public ResultsViewModel(
        MainNavigationService mainNavigationService,
        ResultsNavigationService resultsNavigationService,
        ResultsNavigationStore resultsNavigationStore)
    {
        _mainNavigationService = mainNavigationService;
        _resultsNavigationService = resultsNavigationService;
        _resultsNavigationStore = resultsNavigationStore;

        var timeSpan400 = TimeSpan.FromMilliseconds(400);
        var crossFade = new CrossFade(timeSpan400)
        {
            FadeOutEasing = new QuadraticEaseIn()
        };
        var slideLeft = new SlideLeftPageTransition(timeSpan400);
        var slideRight = new SlideRightPageTransition(timeSpan400);

        CurrentPageTransition = new CompositePageTransition();
        CurrentPageTransition.PageTransitions.Add(crossFade);
        CurrentPageTransition.PageTransitions.Add(slideLeft);

        _resultsNavigationStore.CurrentViewModelChanged += () =>
        {
            CurrentViewModel = _resultsNavigationStore.CurrentViewModel;
            if (_resultsNavigationStore.CurrentPageTransition == typeof(SlideLeftPageTransition))
            {
                CurrentPageTransition.PageTransitions[1] = slideLeft;
            }
            else
            {
                CurrentPageTransition.PageTransitions[1] = slideRight;
            }
        };

        _resultsNavigationService.Navigate<StationListViewModel, SlideLeftPageTransition>();

        if (OperatingSystem.IsAndroid())
        {
            BackButtonIsVisible = false;
        }

        ((App)Application.Current!).BackPressed += OnBackPressed;
    }

    #endregion constructors

    #region private fields

    private readonly MainNavigationService? _mainNavigationService;
    private readonly ResultsNavigationService? _resultsNavigationService;
    private readonly ResultsNavigationStore? _resultsNavigationStore;

    #endregion privat fields

    #region bindable properties

    [ObservableProperty] private ViewModelBase? _currentViewModel;
    [ObservableProperty] private CompositePageTransition? _currentPageTransition;
    [ObservableProperty] private bool _backButtonIsVisible = true;

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
        if (CurrentViewModel!.GetType() == typeof(StationDetailsViewModel))
        {
            _resultsNavigationService!.Navigate<StationListViewModel, SlideRightPageTransition>();
        }
        else
        {
            _mainNavigationService!.Navigate<AddressSelectionViewModel, CrossFade>();
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