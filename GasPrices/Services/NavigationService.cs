using GasPrices.Store;
using GasPrices.ViewModels;
using System;
using Avalonia.Animation;
using GasPrices.PageTransitions;

namespace GasPrices.Services
{
    public class NavigationService
    {
        private readonly NavigationStore _navigationStore;
        private readonly Func<Type, ViewModelBase> _viewModelCreator;

        public NavigationService(NavigationStore navigationStore, Func<Type, ViewModelBase> viewModelCreator)
        {
            _navigationStore = navigationStore;
            _viewModelCreator = viewModelCreator;
        }

        public void Navigate<TViewModel, TPageTransition>()
        {
            _navigationStore.CurrentPageTransition = typeof(TPageTransition);
            _navigationStore.CurrentViewModel = _viewModelCreator(typeof(TViewModel));
        }
    }
}