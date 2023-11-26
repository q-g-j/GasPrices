using GasPrices.Store;
using GasPrices.ViewModels;
using System;

namespace GasPrices.Services
{
    public class MainNavigationService
    {
        private readonly MainNavigationStore _mainNavigationStore;
        private readonly Func<Type, ViewModelBase> _viewModelCreator;

        public MainNavigationService(MainNavigationStore mainNavigationStore, Func<Type, ViewModelBase> viewModelCreator)
        {
            _mainNavigationStore = mainNavigationStore;
            _viewModelCreator = viewModelCreator;
        }

        public void Navigate<TViewModel, TPageTransition>()
        {
            _mainNavigationStore.CurrentPageTransition = typeof(TPageTransition);
            _mainNavigationStore.CurrentViewModel = _viewModelCreator(typeof(TViewModel));
        }
    }
}