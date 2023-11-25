using System;
using Avalonia.Animation;
using CommunityToolkit.Mvvm.ComponentModel;
using GasPrices.Store;

namespace GasPrices.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly NavigationStore? _navigationStore;
    private readonly PageTransitionStore? _pageTransitionStore;

    [ObservableProperty] private ViewModelBase? _currentViewModel;

    [ObservableProperty] private IPageTransition? _currentPageTransition;

    public MainViewModel()
    {
    }

    public MainViewModel(NavigationStore navigationStore, PageTransitionStore pageTransitionStore)
    {
        _navigationStore = navigationStore;
        _pageTransitionStore = pageTransitionStore;
        _currentViewModel = _navigationStore.CurrentViewModel;

        _navigationStore.CurrentViewModelChanged += () =>
            CurrentViewModel = _navigationStore.CurrentViewModel;

        _pageTransitionStore.CurrentPageTransitionChanged += () =>
            CurrentPageTransition = _pageTransitionStore.CurrentPageTransition;
    }

    public override void Dispose()
    {
    }
}
