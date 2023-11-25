using GasPrices.Store;
using GasPrices.ViewModels;
using System;
using Avalonia.Animation;

namespace GasPrices.Services
{
    public class NavigationService(
        NavigationStore navigationStore,
        PageTransitionStore pageTransitionStore,
        Func<Type, ViewModelBase> viewModelCreator,
        Func<Type, IPageTransition> pageTransitionCreator
        )
    {
        public void Navigate<TViewModel, TPageTransition>()
        {
            pageTransitionStore.CurrentPageTransition = pageTransitionCreator(typeof(TPageTransition));
            navigationStore.CurrentViewModel = viewModelCreator(typeof(TViewModel));
        }
    }
}
