using Avalonia.Threading;
using System;
using System.Threading.Tasks;

namespace GasPrices.Utilities;

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
    public static async Task InvokeAsync(Action action)
    {
        var dispatchObject = Dispatcher.UIThread;
        if (dispatchObject.CheckAccess())
        {
            action();
        }
        else
        {
            await dispatchObject.InvokeAsync(action, DispatcherPriority.Normal);
        }
    }
}