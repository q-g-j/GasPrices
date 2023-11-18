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
                services.AddSingleton<ViewLocator>();

                // Add ViewModel creator functions:
                services.AddSingleton<Func<AddressSelectionViewModel>>(services => () =>
                    services.GetRequiredService<AddressSelectionViewModel>());
                services.AddSingleton<Func<ResultsViewModel>>(services => () =>
                    services.GetRequiredService<ResultsViewModel>());
                services.AddSingleton<Func<SettingsViewModel>>(services => () =>
                    services.GetRequiredService<SettingsViewModel>());

                // Add ViewModel navigation services:
                services.AddSingleton<NavigationService<AddressSelectionViewModel>>();
                services.AddSingleton<NavigationService<SettingsViewModel>>();
                services.AddSingleton<NavigationService<ResultsViewModel>>();

                // Add ViewModels:
                services.AddTransient<MainViewModel>();
                services.AddTransient<AddressSelectionViewModel>();
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
