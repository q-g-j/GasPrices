using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace OpenSpritpreise.ViewModels;

public abstract class ViewModelBase : ObservableObject, IDisposable
{
    public abstract void Dispose();
}