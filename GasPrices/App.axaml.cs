using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using GasPrices.Extensions;
using GasPrices.Services;
using GasPrices.ViewModels;
using GasPrices.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using GasPrices.PageTransitions;
using GasPrices.Store;
using Serilog;

namespace GasPrices;

public class App : Application
{
    public App()
    {
    }

    public event Action? BackPressed;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var host = new HostBuilder()
            .AddServices()
            .AddMainView(ApplicationLifetime is IClassicDesktopStyleApplicationLifetime)
            .Build();

        var viewLocator = host.Services.GetService<ViewLocatorService>();
        DataTemplates.Add(viewLocator!);

        var navigationService = host.Services.GetRequiredService<NavigationService<MainNavigationStore>>();
        navigationService.Navigate<AddressSelectionViewModel, CustomCrossFadePageTransition>();

        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktop:
                desktop.MainWindow = host.Services.GetService<MainWindow>();
                break;
            case ISingleViewApplicationLifetime singleViewPlatform:
                singleViewPlatform.MainView = host.Services.GetService<MainView>();
                break;
        }

        base.OnFrameworkInitializationCompleted();
    }

    public void OnBackPressed()
    {
        BackPressed?.Invoke();
    }

    public bool IsBackPressedSubscribed()
    {
        return BackPressed != null;
    }
}