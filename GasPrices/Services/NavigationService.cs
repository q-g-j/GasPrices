using GasPrices.Store;
using GasPrices.ViewModels;
using GasPrices.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasPrices.Services
{
    public class NavigationService<TViewModel> where TViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly Func<TViewModel> _getViewModelFromDI;

        public NavigationService(NavigationStore navigationStore, Func<TViewModel> viewModelCreator)
        {
            _navigationStore = navigationStore;
            _getViewModelFromDI = viewModelCreator;
        }

        public void Navigate()
        {
            _navigationStore.CurrentViewModel?.Dispose();
            _navigationStore.CurrentViewModel = _getViewModelFromDI();
        }
    }
}
