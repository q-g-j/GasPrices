using System;
using Avalonia.Animation;
using CommunityToolkit.Mvvm.ComponentModel;
using GasPrices.PageTransitions;
using GasPrices.Store;

namespace GasPrices.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    #region constructors

    public MainViewModel()
    {
    }

    public MainViewModel(MainNavigationStore mainNavigationStore)
    {
        var crossFade = new CrossFade(TimeSpan.FromMilliseconds(500));
        var slideLeft = new SlideLeftPageTransition(TimeSpan.FromMilliseconds(300));
        var slideRight = new SlideRightPageTransition(TimeSpan.FromMilliseconds(300));

        _mainNavigationStore = mainNavigationStore;
        _currentViewModel = _mainNavigationStore.CurrentViewModel;

        _compositePageTransition = new CompositePageTransition();
        _compositePageTransition.PageTransitions.Add(crossFade);

        _mainNavigationStore.CurrentViewModelChanged += () =>
        {
            if (_mainNavigationStore.CurrentPageTransition == typeof(CrossFade))
            {
                CurrentPageTransition = crossFade;
            }
            else if (_mainNavigationStore.CurrentPageTransition == typeof(SlideLeftPageTransition))
            {
                CurrentPageTransition = slideLeft;
            }
            else if (_mainNavigationStore.CurrentPageTransition == typeof(SlideRightPageTransition))
            {
                CurrentPageTransition = slideRight;
            }
            CurrentViewModel = _mainNavigationStore!.CurrentViewModel;
        };
    }

    #endregion constructors

    #region private fields

    private readonly MainNavigationStore? _mainNavigationStore;

    #endregion private fields

    #region bindable properties

    [ObservableProperty] private ViewModelBase? _currentViewModel;
    [ObservableProperty] private CompositePageTransition? _compositePageTransition;

    [ObservableProperty] private IPageTransition? _currentPageTransition;

    #endregion bindable properties

    #region public overrides

    public override void Dispose()
    {
    }

    #endregion public overrides
}