using System;
using System.Threading.Tasks;


namespace OpenSpritpreise.Extensions;

public static class TaskExtensions
{
    public static void FireAndForget(this Task task)
    {
        try
        {
            Task.Run(async () => await task);
        }
        catch (Exception)
        {
            // ignore
        }
    }
}