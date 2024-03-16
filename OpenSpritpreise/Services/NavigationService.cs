using System;
using OpenSpritpreise.PageTransitions;
using OpenSpritpreise.Store;
using OpenSpritpreise.ViewModels;

namespace OpenSpritpreise.Services;

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