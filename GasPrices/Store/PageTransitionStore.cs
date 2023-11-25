using System;
using Avalonia.Animation;

namespace GasPrices.Store;

public class PageTransitionStore
{
    private IPageTransition? _currentPageTransition;
    public IPageTransition CurrentPageTransition
    {
        get => _currentPageTransition!;
        set
        {
            _currentPageTransition = value;
            OnCurrentPageTransitionChanged();
        }
    }

    public event Action? CurrentPageTransitionChanged;

    private void OnCurrentPageTransitionChanged()
    {
        CurrentPageTransitionChanged?.Invoke();
    }
}