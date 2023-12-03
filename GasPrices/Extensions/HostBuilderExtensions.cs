using ApiClients;
using Avalonia.Controls;
using GasPrices.Services;
using GasPrices.Store;
using GasPrices.ViewModels;
using GasPrices.Views;
using HttpClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using GasPrices.PageTransitions;
using SettingsHandling;

namespace GasPrices.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder AddServices(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((_, services) =>
            {
                // Add Singletons (global variables and store):
                services.AddSingleton(new GlobalsStore("GasPricesApp", "settings.json"));
                services.AddSingleton<MainNavigationStore>();
                services.AddSingleton<ResultsNavigationStore>();
                services.AddSingleton<AppStateStore>();

                // Add HttpClient functionality:
                services.AddHttpClient();
                services.AddTransient<HttpClientRepository>();

                // Add Views:
                services.AddTransient<AddressSelectionView>();
                services.AddTransient<LocationPickerView>();
                services.AddTransient<ResultsView>();
                services.AddTransient<StationListView>();
                services.AddTransient<StationDetailsView>();
                services.AddTransient<SettingsView>();

                // Add ViewModels:
                services.AddSingleton<MainViewModel>();
                services.AddTransient<AddressSelectionViewModel>();
                services.AddTransient<LocationPickerViewModel>();
                services.AddTransient<ResultsViewModel>();
                services.AddTransient<StationListViewModel>();
                services.AddTransient<StationDetailsViewModel>();
                services.AddTransient<SettingsViewModel>();

                // Add PageTransitions:
                services.AddTransient<CustomCrossFadePageTransition>();
                services
                    .AddTransient<
                        CustomCompositePageTransition<CustomCrossFadePageTransition, SlideLeftPageTransition>>();
                services
                    .AddTransient<
                        CustomCompositePageTransition<CustomCrossFadePageTransition, SlideRightPageTransition>>();

                // Add the ViewLocator service:
                services.AddSingleton<ViewLocatorService>();

                // Add ViewModel navigation services:
                services.AddSingleton<NavigationService<MainNavigationStore>>();
                services.AddSingleton<NavigationService<ResultsNavigationStore>>();

                // Add View factory function for the ViewLocator:
                services.AddSingleton<Func<Type, Control?>>(sp =>
                    type => sp.GetRequiredService(type) as Control);

                // Add ViewModel factory function:
                services.AddSingleton<Func<Type, ViewModelBase?>>(sp =>
                    type => sp.GetRequiredService(type) as ViewModelBase);

                // Add PageTransition factory function:
                services.AddSingleton<Func<Type, ICustomPageTransition?>>(sp =>
                    type => sp.GetRequiredService(type) as ICustomPageTransition);

                // Add API clients:
                services.AddTransient<IGasPricesClient, TankerkönigClient>();
                services.AddTransient<IMapClient, OpenStreetMapClient>();

                // Add settings file handlers:
                services.AddTransient<ISettingsWriter>(sp =>
                {
                    var globals = sp.GetRequiredService<GlobalsStore>();
                    if (OperatingSystem.IsBrowser())
                    {
                        return new SettingsLocalStorageWriter();
                    }

                    return new SettingsFileWriter(globals.SettingsFolderFullPath, globals.SettingsFileFullPath);
                });
                services.AddTransient<ISettingsReader>(sp =>
                {
                    var globals = sp.GetRequiredService<GlobalsStore>();
                    if (OperatingSystem.IsBrowser())
                    {
                        return new SettingsLocalStorageReader();
                    }

                    return new SettingsFileReader(globals.SettingsFileFullPath);
                });
            });
            return hostBuilder;
        }
    }
}