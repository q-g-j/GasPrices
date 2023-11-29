using System;
using GasPrices.Store;
using GasPrices.ViewModels;

namespace GasPrices.Services;

public class ResultsNavigationService(
    ResultsNavigationStore resultsNavigationStore,
    Func<Type, ViewModelBase> viewModelCreator)
{
    public void Navigate<TViewModel, TPageTransition>()
    {
        resultsNavigationStore.CurrentPageTransition = typeof(TPageTransition);
        resultsNavigationStore.CurrentViewModel = viewModelCreator(typeof(TViewModel));
    }
}