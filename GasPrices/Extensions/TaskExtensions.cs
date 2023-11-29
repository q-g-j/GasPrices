using System;
using System.Threading.Tasks;


namespace GasPrices.Extensions;

public static class TaskExtensions
{
    public static async void FireAndForget(this Task task)
    {
        try
        {
            await task;
        }
        catch (Exception)
        {
            // ignore
        }
    }
}