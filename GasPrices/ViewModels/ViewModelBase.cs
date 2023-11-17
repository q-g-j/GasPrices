using CommunityToolkit.Mvvm.ComponentModel;

namespace GasPrices.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    public abstract void Dispose();
}
