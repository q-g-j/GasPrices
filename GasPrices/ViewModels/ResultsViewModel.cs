using System;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GasPrices.Services;
using GasPrices.Store;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using ExCSS;
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
            MainNavigationService mainNavigationService,
            ResultsNavigationService resultsNavigationService,
            ResultsNavigationStore resultsNavigationStore)
        {
            _mainNavigationService = mainNavigationService;
            _resultsNavigationService = resultsNavigationService;
            _resultsNavigationStore = resultsNavigationStore;

            var timeSpan300 = TimeSpan.FromMilliseconds(300);
            var crossFade = new CrossFade(timeSpan300)
            {
                FadeOutEasing = new QuadraticEaseIn()
            };
            var slideLeft = new SlideLeftPageTransition(timeSpan300);
            var slideRight = new SlideRightPageTransition(timeSpan300);

            CurrentPageTransition = new CompositePageTransition();
            CurrentPageTransition.PageTransitions.Add(crossFade);
            CurrentPageTransition.PageTransitions.Add(slideLeft);
            
            _resultsNavigationStore.CurrentViewModelChanged += () =>
            {
                CurrentViewModel = _resultsNavigationStore.CurrentViewModel;
                if (_resultsNavigationStore.CurrentPageTransition == typeof(SlideLeftPageTransition))
                {
                    CurrentPageTransition.PageTransitions[1] = slideLeft;
                }
                else
                {
                    CurrentPageTransition.PageTransitions[1] = slideRight;
                }
            };
            
            _resultsNavigationService.Navigate<StationListViewModel, SlideLeftPageTransition>();
            
            ((App)Application.Current!).BackPressed += OnBackPressed;
        }

        #endregion constructors

        #region private fields

        private readonly MainNavigationService? _mainNavigationService;
        private readonly ResultsNavigationService? _resultsNavigationService;
        private readonly ResultsNavigationStore? _resultsNavigationStore;

        #endregion privat fields

        #region bindable properties

        [ObservableProperty] private ViewModelBase? _currentViewModel;
        [ObservableProperty] private CompositePageTransition? _currentPageTransition;

        #endregion bindable properties

        #region commands

        [RelayCommand]
        public void BackCommand()
        {
            OnBackPressed();
        }

        #endregion commands

        #region private methods

        private void OnBackPressed()
        {
            if (CurrentViewModel!.GetType() == typeof(StationDetailsViewModel))
            {
                _resultsNavigationService!.Navigate<StationListViewModel, SlideRightPageTransition>();
            }
            else
            {
                _mainNavigationService!.Navigate<AddressSelectionViewModel, CrossFade>();
            }
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