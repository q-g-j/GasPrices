using ApiClients.Models;
using Avalonia;
using CommunityToolkit.Mvvm.Input;
using GasPrices.Services;
using GasPrices.Store;
using Mapsui;
using Mapsui.Projections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasPrices.ViewModels
{
    public partial class LocationPickerViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;
        private readonly SearchResultStore? _searchResultStore;

        public LocationPickerViewModel(
            NavigationService navigationService, SearchResultStore? searchResultStore)
        {
            _navigationService = navigationService;
            _searchResultStore = searchResultStore;
        }

        [RelayCommand]
        public void HomeCommand(object sender)
        {

        }

        [RelayCommand]
        public void InfoCommand(object sender)
        {
            var a = sender as MapInfoEventArgs;
            var pos = SphericalMercator.ToLonLat(a.MapInfo?.WorldPosition!);
            var coords = new Coords(pos.Y, pos.X);
            _searchResultStore!.Coords = coords;
            _searchResultStore!.AreCoordsFromMap = true;
            _navigationService.Navigate<AddressSelectionViewModel>();
        }

        [RelayCommand]
        public void BackCommand()
        {
            _navigationService.Navigate<AddressSelectionViewModel>();
        }

        private void OnBackPressed()
        {
            _navigationService.Navigate<AddressSelectionViewModel>();
        }

        public override void Dispose()
        {
            ((App)Application.Current!).BackPressed -= OnBackPressed;
        }
    }
}
