using ApiClients.Models;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
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
        private readonly SearchResultStore _searchResultStore;
        private readonly Coords _oldCoords;

        public LocationPickerViewModel(
            NavigationService navigationService,
            SearchResultStore searchResultStore)
        {
            _navigationService = navigationService;
            _searchResultStore = searchResultStore;
            _oldCoords = searchResultStore.Coords!;
        }

        [ObservableProperty]
        private bool applyButtonIsEnabled;

        [RelayCommand]
        public void ApplyCommand()
        {
            _navigationService.Navigate<AddressSelectionViewModel>();
        }

        [RelayCommand]
        public void BackCommand()
        {
            _searchResultStore.Coords = _oldCoords;
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
