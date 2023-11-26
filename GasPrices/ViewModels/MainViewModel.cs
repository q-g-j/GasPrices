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
        var dummyCrossFade = new DummyCrossFadePageTransition(TimeSpan.FromMilliseconds(500));
        var slideLeft = new SlideLeftPageTransition(TimeSpan.FromMilliseconds(300));
        var slideRight = new SlideRightPageTransition(TimeSpan.FromMilliseconds(300));

        _mainNavigationStore = mainNavigationStore;
        _currentViewModel = _mainNavigationStore.CurrentViewModel;

        _compositePageTransition = new CompositePageTransition();
        _compositePageTransition.PageTransitions.Add(crossFade);

        _mainNavigationStore.CurrentViewModelChanged += () =>
        {
            if (CompositePageTransition!.PageTransitions.Count > 1)
            {
                CompositePageTransition!.PageTransitions.RemoveAt(1);
            }

            if (_mainNavigationStore.CurrentPageTransition == typeof(CrossFade))
            {
                CompositePageTransition!.PageTransitions[0] = crossFade;
            }
            else if (_mainNavigationStore.CurrentPageTransition == typeof(SlideLeftPageTransition))
            {
                CompositePageTransition!.PageTransitions[0] = dummyCrossFade;
                CompositePageTransition!.PageTransitions.Add(slideLeft);
            }
            else if (_mainNavigationStore.CurrentPageTransition == typeof(SlideRightPageTransition))
            {
                CompositePageTransition!.PageTransitions[0] = dummyCrossFade;
                CompositePageTransition!.PageTransitions.Add(slideRight);
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

    #endregion bindable properties

    #region public overrides

    public override void Dispose()
    {
    }

    #endregion public overrides
}