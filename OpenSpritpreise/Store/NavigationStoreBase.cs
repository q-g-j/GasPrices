using System;
using OpenSpritpreise.PageTransitions;
using OpenSpritpreise.ViewModels;

namespace OpenSpritpreise.Store;

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