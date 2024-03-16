using Avalonia.Controls.Templates;
using Avalonia.Controls;
using OpenSpritpreise.ViewModels;
using System;

namespace OpenSpritpreise.Services;

public class ViewLocatorService(Func<Type, Control> viewCreator) : IDataTemplate
{
    public static bool SupportsRecycling => false;

    public Control Build(object? data)
    {
        var name = data?.GetType().FullName?.Replace("ViewModel", "View");
        var type = Type.GetType(name!);
        if (type == null)
        {
            return new TextBlock { Text = "Not Found: " + name };
        }

        try
        {
            var view = viewCreator(type);
            return view;
        }
        catch (Exception ex)
        {
            return new TextBlock { Text = ex.GetType() + ", " + ex.Message };
        }
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}