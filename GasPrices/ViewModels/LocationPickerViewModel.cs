using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GasPrices.Services;
using GasPrices.Store;

namespace GasPrices.ViewModels
{
    public partial class LocationPickerViewModel : ViewModelBase
    {
        #region constructors

        public LocationPickerViewModel()
        {
        }
        
        public LocationPickerViewModel(
            NavigationService navigationService,
            AppStateStore appStateStore)
        {
            _navigationService = navigationService;
            _appStateStore = appStateStore;

            ApplyButtonIsEnabled = _appStateStore.CoordsFromMapClient != null;

            ((App)Application.Current!).BackPressed += OnBackPressed;
        }
        #endregion constructors

        #region private fields
        private readonly NavigationService? _navigationService;
        private readonly AppStateStore? _appStateStore;
        #endregion private fields

        #region bindable properties
        [ObservableProperty]
        private bool applyButtonIsEnabled;
        #endregion bindable properties

        #region commands
        [RelayCommand]
        public void ApplyCommand()
        {
            _navigationService.Navigate<AddressSelectionViewModel>();
        }

        [RelayCommand]
        public void BackCommand()
        {
            _appStateStore.CoordsFromMapClient = null;
            _navigationService.Navigate<AddressSelectionViewModel>();
        }
        #endregion commands

        #region private methods
        private void OnBackPressed()
        {
            _appStateStore.CoordsFromMapClient = null;
            _navigationService.Navigate<AddressSelectionViewModel>();
        }
        #endregion private methods

        #region public overrides
        public override void Dispose()
        {
            ((App)Application.Current!).BackPressed -= OnBackPressed;
        }
        #endregion public overrides
    }
}
