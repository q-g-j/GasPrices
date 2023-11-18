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
    public class NavigationService(NavigationStore navigationStore, Func<Type, ViewModelBase> viewModelCreator)
    {
        private readonly NavigationStore _navigationStore = navigationStore;
        private readonly Func<Type, ViewModelBase> _viewModelCreator = viewModelCreator;

        public void Navigate<TViewModel>()
        {
            _navigationStore.CurrentViewModel = _viewModelCreator(typeof(TViewModel));
        }
    }
}
