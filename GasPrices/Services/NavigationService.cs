using System;
using GasPrices.Store;
using GasPrices.ViewModels;

namespace GasPrices.Services;

public class NavigationService<TNavigationStore>(
    TNavigationStore navigationStore,
    Func<Type, ViewModelBase> viewModelCreator) where TNavigationStore : NavigationStoreBase
{
    public void Navigate<TViewModel, TPageTransition>()
    {
        navigationStore.CurrentPageTransition = typeof(TPageTransition);
        navigationStore.CurrentViewModel = viewModelCreator(typeof(TViewModel));
    }
}