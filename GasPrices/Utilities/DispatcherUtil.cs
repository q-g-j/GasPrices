using Avalonia.Threading;
using System;

namespace GasPrices.Utilities
{
    public static class DispatcherUtil
    {
        public static void Invoke(Action action)
        {
            Dispatcher dispatchObject = Dispatcher.UIThread;
            if (dispatchObject == null || dispatchObject.CheckAccess())
            {
                action();
            }
            else
            {
                dispatchObject.InvokeAsync(action, DispatcherPriority.Normal).Wait();
            }
        }
    }
}
