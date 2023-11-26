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
using SettingsFile;

namespace GasPrices.Extensions
{
    public static class HostBuilderConfigureServicesExtensions
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

                // Add the ViewLocator service:
                services.AddTransient<ViewLocatorService>();

                // Add ViewModel navigation services:
                services.AddTransient<MainNavigationService>();
                services.AddTransient<ResultsNavigationService>();

                // Add View factory function for the ViewLocator:
                services.AddTransient<Func<Type, Control?>>(sp =>
                    type => sp.GetRequiredService(type) as Control);

                // Add ViewModel factory function:
                services.AddTransient<Func<Type, ViewModelBase?>>(sp =>
                    type => sp.GetRequiredService(type) as ViewModelBase);
                
                // Add API clients:
                services.AddTransient<IGasPricesClient, TankerkönigClient>();
                services.AddTransient<IMapClient, OpenStreetMapClient>();

                // Add settings file handlers:
                services.AddTransient(sp =>
                {
                    var globals = sp.GetRequiredService<GlobalsStore>();
                    return new SettingsFileWriter(globals.SettingsFolderFullPath, globals.SettingsFileFullPath);
                });
                services.AddTransient(sp =>
                {
                    var globals = sp.GetRequiredService<GlobalsStore>();
                    return new SettingsFileReader(globals.SettingsFileFullPath);
                });
            });
            return hostBuilder;
        }
    }
}