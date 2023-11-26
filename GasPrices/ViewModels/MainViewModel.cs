using System;
using Avalonia.Animation;
using CommunityToolkit.Mvvm.ComponentModel;
using GasPrices.PageTransitions;
using GasPrices.Store;

namespace GasPrices.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
    }

    public MainViewModel(NavigationStore navigationStore)
    {
        var crossFade = new CrossFade(TimeSpan.FromMilliseconds(500));
        var dummyCrossFade = new DummyCrossFadePageTransition(TimeSpan.FromMilliseconds(500));
        var slideLeft = new SlideLeftPageTransition(TimeSpan.FromMilliseconds(300));
        var slideRight = new SlideRightPageTransition(TimeSpan.FromMilliseconds(300));
        
        _navigationStore = navigationStore;
        _currentViewModel = _navigationStore.CurrentViewModel;

        _compositePageTransition = new CompositePageTransition();
        _compositePageTransition.PageTransitions.Add(crossFade);

        _navigationStore.CurrentViewModelChanged += () =>
        {
            if (CompositePageTransition!.PageTransitions.Count > 1)
            {
                CompositePageTransition!.PageTransitions.RemoveAt(1);
            }
            if (_navigationStore.CurrentPageTransition == typeof(CrossFade))
            {
                CompositePageTransition!.PageTransitions[0] = crossFade;
            }
            else if (_navigationStore.CurrentPageTransition == typeof(SlideLeftPageTransition))
            {
                CompositePageTransition!.PageTransitions[0] = dummyCrossFade;
                CompositePageTransition!.PageTransitions.Add(slideLeft);
            }
            else if (_navigationStore.CurrentPageTransition == typeof(SlideRightPageTransition))
            {
                CompositePageTransition!.PageTransitions[0] = dummyCrossFade;
                CompositePageTransition!.PageTransitions.Add(slideRight);
            }

            CurrentViewModel = _navigationStore!.CurrentViewModel;
        };
    }

    private readonly NavigationStore? _navigationStore;

    [ObservableProperty] private ViewModelBase? _currentViewModel;
    [ObservableProperty] private CompositePageTransition? _compositePageTransition;

    public override void Dispose()
    {
    }
}