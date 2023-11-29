using GasPrices.Store;
using GasPrices.ViewModels;
using System;

namespace GasPrices.Services;

public class MainNavigationService(
    MainNavigationStore mainNavigationStore,
    Func<Type, ViewModelBase> viewModelCreator)
{
    public void Navigate<TViewModel, TPageTransition>()
    {
        mainNavigationStore.CurrentPageTransition = typeof(TPageTransition);
        mainNavigationStore.CurrentViewModel = viewModelCreator(typeof(TViewModel));
    }
}