using System;
using GasPrices.Store;
using GasPrices.ViewModels;

namespace GasPrices.Services;

public class ResultsNavigationService
{
    private readonly ResultsNavigationStore _resultsNavigationStore;
    private readonly Func<Type, ViewModelBase> _viewModelCreator;

    public ResultsNavigationService(ResultsNavigationStore resultsNavigationStore, Func<Type, ViewModelBase> viewModelCreator)
    {
        _resultsNavigationStore = resultsNavigationStore;
        _viewModelCreator = viewModelCreator;
    }

    public void Navigate<TViewModel, TPageTransition>()
    {
        _resultsNavigationStore.CurrentPageTransition = typeof(TPageTransition);
        _resultsNavigationStore.CurrentViewModel = _viewModelCreator(typeof(TViewModel));
    }
}