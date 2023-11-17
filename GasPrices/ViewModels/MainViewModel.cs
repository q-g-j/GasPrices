using CommunityToolkit.Mvvm.ComponentModel;
using GasPrices.Store;

namespace GasPrices.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly NavigationStore? _navigationStore;


    [ObservableProperty]
    private ViewModelBase? currentViewModel;

    public MainViewModel()
    {
    }

    public MainViewModel(NavigationStore navigationStore)
    {
        _navigationStore = navigationStore;
        currentViewModel = _navigationStore.CurrentViewModel;

        _navigationStore.CurrentViewModelChanged += () =>
            CurrentViewModel = _navigationStore.CurrentViewModel;
    }

    public override void Dispose()
    {
    }
}
