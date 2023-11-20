using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GasPrices.Services;
using GasPrices.Store;

namespace GasPrices.ViewModels
{
    public partial class LocationPickerViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;
        private readonly AppStateStore _appStateStore;

        public LocationPickerViewModel(
            NavigationService navigationService,
            AppStateStore appStateStore)
        {
            _navigationService = navigationService;
            _appStateStore = appStateStore;

            ((App)Application.Current!).BackPressed += OnBackPressed;
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
            _appStateStore.Coords = null;
            _navigationService.Navigate<AddressSelectionViewModel>();
        }

        private void OnBackPressed()
        {
            _appStateStore.Coords = null;
            _navigationService.Navigate<AddressSelectionViewModel>();
        }

        public override void Dispose()
        {
            ((App)Application.Current!).BackPressed -= OnBackPressed;
        }
    }
}
