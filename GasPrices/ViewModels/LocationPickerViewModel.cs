using Avalonia;
using CommunityToolkit.Mvvm.Input;
using GasPrices.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasPrices.ViewModels
{
    public partial class LocationPickerViewModel : ViewModelBase
    {
        private readonly NavigationService<AddressSelectionViewModel> _addressSelectionNavigationService;

        public LocationPickerViewModel(NavigationService<AddressSelectionViewModel> addressSelectionNavigationService)
        {
            _addressSelectionNavigationService = addressSelectionNavigationService;
        }

        [RelayCommand]
        public void BackCommand()
        {
            _addressSelectionNavigationService.Navigate();
        }

        private void OnBackPressed()
        {
            _addressSelectionNavigationService.Navigate();
        }

        public override void Dispose()
        {
            ((App)Application.Current!).BackPressed -= OnBackPressed;
        }
    }
}
