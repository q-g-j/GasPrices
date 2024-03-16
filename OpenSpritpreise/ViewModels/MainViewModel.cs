using Avalonia.Animation;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenSpritpreise.Store;

namespace OpenSpritpreise.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    #region constructors

    public MainViewModel()
    {
    }

    public MainViewModel(MainNavigationStore mainNavigationStore)
    {
        _mainNavigationStore = mainNavigationStore;
        _currentViewModel = _mainNavigationStore.CurrentViewModel;

        _mainNavigationStore.CurrentViewModelChanged += () =>
        {
            CurrentPageTransition = mainNavigationStore.CurrentPageTransition;
            CurrentViewModel = _mainNavigationStore!.CurrentViewModel;
        };
    }

    #endregion constructors

    #region private fields

    private readonly MainNavigationStore? _mainNavigationStore;

    #endregion private fields

    #region bindable properties

    [ObservableProperty] private ViewModelBase? _currentViewModel;
    [ObservableProperty] private IPageTransition? _currentPageTransition;

    #endregion bindable properties

    #region public overrides

    public override void Dispose()
    {
    }

    #endregion public overrides
}