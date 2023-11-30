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
        ResultsNavigationStore resultsNavigationStore,
        NavigationService<MainNavigationStore> mainNavigationService,
        NavigationService<ResultsNavigationStore> resultsNavigationService)
    {
        _resultsNavigationStore = resultsNavigationStore;
        _mainNavigationService = mainNavigationService;
        _resultsNavigationService = resultsNavigationService;

        _resultsNavigationStore.CurrentViewModelChanged += () =>
            CurrentViewModel = _resultsNavigationStore.CurrentViewModel;

        var crossFade = new CrossFade(TimeSpan.FromMilliseconds(400))
        {
            FadeOutEasing = new QuadraticEaseIn()
        };

        CurrentPageTransition = new CompositePageTransition();
        CurrentPageTransition.PageTransitions.Add(crossFade);
        CurrentPageTransition.PageTransitions.Add(_slideLeft);

        _resultsNavigationService.Navigate<StationListViewModel, SlideLeftPageTransition>();

        if (OperatingSystem.IsAndroid())
        {
            BackButtonIsVisible = false;
        }

        ((App)Application.Current!).BackPressed += OnBackPressed;
    }

    #endregion constructors

    #region private fields

    private readonly ResultsNavigationStore? _resultsNavigationStore;
    private readonly NavigationService<MainNavigationStore>? _mainNavigationService;
    private readonly NavigationService<ResultsNavigationStore>? _resultsNavigationService;
    private readonly SlideLeftPageTransition _slideLeft = new(TimeSpan.FromMilliseconds(400));
    private readonly SlideRightPageTransition _slideRight = new(TimeSpan.FromMilliseconds(400));

    #endregion privat fields

    #region bindable properties

    [ObservableProperty] private ViewModelBase? _currentViewModel;
    [ObservableProperty] private CompositePageTransition? _currentPageTransition;
    [ObservableProperty] private bool _backButtonIsVisible = true;

    #endregion bindable properties

    #region OnPropertyChanged handlers

    partial void OnCurrentViewModelChanged(ViewModelBase? value)
    {
        if (value == null) return;
        if (CurrentPageTransition == null) return;

        if (value.GetType() == typeof(StationListViewModel))
        {
            CurrentPageTransition.PageTransitions[1] = _slideRight;
        }
        else
        {
            CurrentPageTransition.PageTransitions[1] = _slideLeft;
        }
    }

    #endregion OnPropertyChanged handlers

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