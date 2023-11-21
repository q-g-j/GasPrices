using ApiClients;
using ApiClients.Models;
using Avalonia.Controls;
using GasPrices.Services;
using GasPrices.Store;
using GasPrices.ViewModels;
using GasPrices.Views;
using HttpClient;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
                // Add Singletons (global variables and store):
                services.AddSingleton(new Globals("GasPricesApp", "settings.json"));
                services.AddSingleton<NavigationStore>();
                services.AddSingleton<AppStateStore>();

                // Add HttpClient functionality:
                services.AddHttpClient();
                services.AddTransient<HttpClientRepository>();

                // Add Views:
                services.AddTransient<AddressSelectionView>();
                services.AddTransient<ResultsView>();
                services.AddTransient<SettingsView>();
                services.AddTransient<LocationPickerView>();

                // Add ViewModels:
                services.AddTransient<MainViewModel>();
                services.AddTransient<AddressSelectionViewModel>();
                services.AddTransient<LocationPickerViewModel>();
                services.AddTransient<ResultsViewModel>();
                services.AddTransient<SettingsViewModel>();

                // Add the ViewLocator service:
                services.AddTransient<ViewLocatorService>();
                
                // Add ViewModel navigation service:
                services.AddTransient<NavigationService>();

                // Add View factory function for the ViewLocator:
                services.AddTransient<Func<Type, Control>>(serviceProvider => type =>
                {
                    var view = serviceProvider.GetRequiredService(type) as Control;
                    return view ?? throw new ArgumentException("Wrong View type");
                });

                // Add ViewModel factory function:
                services.AddTransient<Func<Type, ViewModelBase>>(serviceProvider => type =>
                {
                    var viewModel = serviceProvider.GetRequiredService(type) as ViewModelBase;
                    return viewModel ?? throw new ArgumentException("Wrong ViewModel type");
                });

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
