using System;
using GasPrices.PageTransitions;
using GasPrices.Store;
using GasPrices.ViewModels;

namespace GasPrices.Services;

public class NavigationService<TNavigationStore>(
    TNavigationStore navigationStore,
    Func<Type, ViewModelBase> viewModelCreator,
    Func<Type, ICustomPageTransition> pageTransitionCreator)
    where TNavigationStore : NavigationStoreBase
{
    public void Navigate<TViewModel, TPageTransition>()
        where TViewModel : ViewModelBase
        where TPageTransition : ICustomPageTransition, new()
    {
        navigationStore.CurrentPageTransition = pageTransitionCreator(typeof(TPageTransition));
        navigationStore.CurrentViewModel = viewModelCreator(typeof(TViewModel));
    }
}