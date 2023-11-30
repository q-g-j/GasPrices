using System;
using GasPrices.PageTransitions;
using GasPrices.ViewModels;

namespace GasPrices.Store;

public class NavigationStoreBase
{
    private ViewModelBase? _currentViewModel;
    public ICustomPageTransition? CurrentPageTransition { get; set; }

    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel!;
        set
        {
            _currentViewModel?.Dispose();
            _currentViewModel = value;
            OnCurrentViewModelChanged();
        }
    }

    public event Action? CurrentViewModelChanged;

    private void OnCurrentViewModelChanged()
    {
        CurrentViewModelChanged?.Invoke();
    }
}