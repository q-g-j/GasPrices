using System;
using GasPrices.ViewModels;

namespace GasPrices.Store;

public class ResultsNavigationStore
{
    private ViewModelBase? _currentViewModel;
    public Type? CurrentPageTransition { get; set; }

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