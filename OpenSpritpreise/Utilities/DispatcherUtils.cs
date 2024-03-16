using Avalonia.Threading;
using System;
using System.Threading.Tasks;

namespace OpenSpritpreise.Utilities;

public static class DispatcherUtils
{
    public static void Invoke(Action action)
    {
        var dispatchObject = Dispatcher.UIThread;
        if (dispatchObject.CheckAccess())
        {
            action();
        }
        else
        {
            dispatchObject.InvokeAsync(action, DispatcherPriority.Normal).Wait();
        }
    }
}