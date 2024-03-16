using CommunityToolkit.Mvvm.ComponentModel;

namespace OpenSpritpreise.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    public abstract void Dispose();
}