using Caliburn.Micro;
using AniNeko.Views;
using MaterialDesignThemes.Wpf;
using System;
using System.Windows.Input;

namespace AniNeko.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        private int _selectedTabIndex; // The selected index of the tab control
        private AnimeListViewModel _animeListViewModel;

        public ShellViewModel()
        {
            Init();
        }

        public void Init()
        {
            // Sets the active item to be displayed in the content control
            // to the anime list view
            _animeListViewModel = new AnimeListViewModel();
            ActivateItem(_animeListViewModel);
        }

        public int Selected
        {
            get { return _selectedTabIndex; }
            set
            {              
                _selectedTabIndex = value;

                // Depending on which tab is selected, changes the sorting method for the bindable collection
                // within the anime list view model
                if (_selectedTabIndex == 0)
                    _animeListViewModel.SortType = AnimeListViewModel.SortMethod.All;
                else if (_selectedTabIndex == 1)
                    _animeListViewModel.SortType = AnimeListViewModel.SortMethod.Watching;
                else if (_selectedTabIndex == 2)
                    _animeListViewModel.SortType = AnimeListViewModel.SortMethod.Completed;
                else if (_selectedTabIndex == 3)
                    _animeListViewModel.SortType = AnimeListViewModel.SortMethod.PlanToWatch;
                else if (_selectedTabIndex == 4)
                    _animeListViewModel.SortType = AnimeListViewModel.SortMethod.Dropped;
            }
        }
    }
}
