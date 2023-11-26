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
using Avalonia.Animation;

namespace GasPrices;

public class App : Application
{
    private readonly IHost? _host;

    public event Action? BackPressed;

    public App()
    {
        _host = new HostBuilder()
            .AddServices()
            .Build();
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        _host?.Start();

        var viewLocator = _host?.Services.GetService<ViewLocatorService>();
        DataTemplates.Add(viewLocator!);

        var navigationService = _host?.Services.GetRequiredService<MainNavigationService>();
        navigationService?.Navigate<AddressSelectionViewModel, CrossFade>();
        
        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = _host?.Services.GetService<MainViewModel>()
        };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = _host?.Services.GetService<MainViewModel>()
        };
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
