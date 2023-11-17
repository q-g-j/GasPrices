using ApiClients;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using GasPrices.Extensions;
using GasPrices.Services;
using GasPrices.Store;
using GasPrices.ViewModels;
using GasPrices.Views;
using HttpClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace GasPrices;

public partial class App : Application
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

        var viewLocator = _host?.Services.GetService<ViewLocator>();
        DataTemplates.Add(viewLocator!);

        var navigationService = _host?.Services.GetRequiredService<NavigationService<AddressSelectionViewModel>>();
        navigationService!.Navigate();

        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = _host?.Services.GetService<MainWindow>();
            var mainViewModel = _host?.Services.GetService<MainViewModel>();
            desktop.MainWindow = mainWindow;
            desktop.MainWindow!.DataContext = mainViewModel;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            var mainView = _host?.Services.GetService<MainView>();
            var mainViewModel = _host?.Services.GetService<MainViewModel>();
            singleViewPlatform.MainView = mainView;
            singleViewPlatform.MainView!.DataContext = mainViewModel;
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
