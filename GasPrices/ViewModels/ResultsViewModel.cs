using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GasPrices.Models;
using GasPrices.Services;
using GasPrices.Store;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Animation;
using GasPrices.PageTransitions;

namespace GasPrices.ViewModels
{
    public partial class ResultsViewModel : ViewModelBase
    {
        #region constructors

        public ResultsViewModel()
        {
        }

        public ResultsViewModel(
            NavigationService navigationService,
            AppStateStore appStateStore)
        {
            _navigationService = navigationService;
            _appStateStore = appStateStore;

            Stations = [];
            foreach (var station in appStateStore.Stations!.Where(s => s is { E5: > 0, E10: > 0, Diesel: > 0 }))
            {
                Stations.Add(new DisplayStation(station, appStateStore.SelectedGasType!));
            }

            ((App)Application.Current!).BackPressed += OnBackPressed;
        }

        #endregion constructors

        #region private fields

        private readonly NavigationService? _navigationService;
        private readonly AppStateStore? _appStateStore;

        #endregion privat fields

        #region bindable properties

        [ObservableProperty] private ObservableCollection<DisplayStation>? _stations;
        [ObservableProperty] private int _selectedIndex = -1;

        #endregion bindable properties

        #region commands

        [RelayCommand]
        public void InitializedCommand()
        {
            if (_appStateStore!.IsFromStationDetailsView)
            {
                Task.Run(async () =>
                {
                    await Task.Delay(200);
                    SelectedIndex = _appStateStore!.SelectedStationIndex;
                    await Task.Delay(600);
                    SelectedIndex = -1;
                });
            }
        }

        [RelayCommand]
        public void BackCommand()
        {
            _navigationService!.Navigate<AddressSelectionViewModel, CrossFade>();
        }

        #endregion commands

        #region private methods

        private void OnBackPressed()
        {
            _navigationService!.Navigate<AddressSelectionViewModel, CrossFade>();
        }

        #endregion private methods

        #region public overrides

        public override void Dispose()
        {
            ((App)Application.Current!).BackPressed -= OnBackPressed;
        }

        #endregion public overrides

        partial void OnSelectedIndexChanged(int value)
        {
            if (_appStateStore!.IsFromStationDetailsView)
            {
                _appStateStore!.IsFromStationDetailsView = false;
                return;
            }
            
            _appStateStore!.SelectedStation = Stations![value];
            _appStateStore!.SelectedStationIndex = value;
            _navigationService!.Navigate<StationDetailsViewModel, SlideLeftPageTransition>();
        }
    }
}