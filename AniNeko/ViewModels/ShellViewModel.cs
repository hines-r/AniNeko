using Caliburn.Micro;
using AniNeko.Views;
using MaterialDesignThemes.Wpf;
using System;
using System.Windows.Input;

namespace AniNeko.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        private int _selected; // The selected index of the tab control
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
            get { return _selected; }
            set
            {              
                _selected = value;

                // Depending on which tab is selected, changes the sorting method for the bindable collection
                // within the anime list view model
                if (_selected == 0)
                    _animeListViewModel.SortType = AnimeListViewModel.SortMethod.All;
                else if (_selected == 1)
                    _animeListViewModel.SortType = AnimeListViewModel.SortMethod.Watching;
                else if (_selected == 2)
                    _animeListViewModel.SortType = AnimeListViewModel.SortMethod.Completed;
                else if (_selected == 3)
                    _animeListViewModel.SortType = AnimeListViewModel.SortMethod.PlanToWatch;
                else if (_selected == 4)
                    _animeListViewModel.SortType = AnimeListViewModel.SortMethod.Dropped;
            }
        }
    }
}
