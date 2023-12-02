using System;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using Avalonia.Animation;
using CommunityToolkit.Mvvm.ComponentModel;
using GasPrices.Store;
using GasPrices.Utilities;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Serilog.ILogger;

namespace GasPrices.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    #region constructors

    public MainViewModel()
    {
    }

    public MainViewModel(MainNavigationStore mainNavigationStore, ILogger<MainViewModel> logger)
    {
        _mainNavigationStore = mainNavigationStore;
        _logger = logger;
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
    private readonly ILogger<MainViewModel> _logger;

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