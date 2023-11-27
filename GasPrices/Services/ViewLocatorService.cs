using Avalonia.Controls.Templates;
using Avalonia.Controls;
using GasPrices.ViewModels;
using System;

namespace GasPrices.Services
{
    public class ViewLocatorService(Func<Type, Control> viewCreator) : IDataTemplate
    {
        private readonly Func<Type, Control> _viewCreator = viewCreator;

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
                var view = _viewCreator(type);
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
}
