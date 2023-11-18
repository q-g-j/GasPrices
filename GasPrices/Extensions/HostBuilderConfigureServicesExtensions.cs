using ApiClients;
using ApiClients.Models;
using GasPrices.Services;
using GasPrices.Store;
using GasPrices.ViewModels;
using GasPrices.Views;
using HttpClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SettingsFile.SettingsFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasPrices.Extensions
{
    public static class HostBuilderConfigureServicesExtensions
    {
        public static IHostBuilder AddServices(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((context, services) =>
            {
                services.AddHttpClient();
                services.AddSingleton<HttpClientRepository>();
                services.AddSingleton(new Globals("GasPricesApp", "settings.json"));
                services.AddSingleton<NavigationStore>();
                services.AddSingleton<SearchResultStore>();
                services.AddTransient<ViewLocator>();

                // Add ViewModel factory functions:
                services.AddTransient<Func<AddressSelectionViewModel>>(services => () =>
                    services.GetRequiredService<AddressSelectionViewModel>());
                services.AddTransient<Func<ResultsViewModel>>(services => () =>
                    services.GetRequiredService<ResultsViewModel>());
                services.AddTransient<Func<SettingsViewModel>>(services => () =>
                    services.GetRequiredService<SettingsViewModel>());
                services.AddTransient<Func<LocationPickerViewModel>>(services => () =>
                    services.GetRequiredService<LocationPickerViewModel>());

                // Add ViewModel navigation services:
                services.AddTransient<NavigationService<AddressSelectionViewModel>>();
                services.AddTransient<NavigationService<SettingsViewModel>>();
                services.AddTransient<NavigationService<ResultsViewModel>>();
                services.AddTransient<NavigationService<LocationPickerViewModel>>();

                // Add ViewModels:
                services.AddTransient<MainViewModel>();
                services.AddTransient<AddressSelectionViewModel>();
                services.AddTransient<LocationPickerViewModel>();
                services.AddTransient<ResultsViewModel>();
                services.AddTransient<SettingsViewModel>();

                // Add API clients:
                services.AddTransient<IGasPricesClient, TankerkönigClient>();
                services.AddTransient<IMapClient, OpenStreetMapClient>();

                // Add settings file handlers:
                services.AddTransient(services =>
                {
                    var globals = services.GetRequiredService<Globals>();
                    return new SettingsFileWriter(globals.SettingsFolderFullPath, globals.SettingsFileFullPath);
                });
                services.AddTransient(service =>
                {
                    var globals = service.GetRequiredService<Globals>();
                    return new SettingsFileReader(globals.SettingsFileFullPath);
                });
            });
            return hostBuilder;
        }
    }
}
