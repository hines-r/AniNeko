using Caliburn.Micro;
using AniNeko.Views;
using MaterialDesignThemes.Wpf;
using System;
using System.Windows.Input;
using System.Linq;
using AniNeko.Models;

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

        public ICommand SearchCommand => new RelayCommand(Search);

        private void Search(object o)
        {
            AnimeListView view = (AnimeListView)_animeListViewModel.GetView();
            string input = o.ToString(); // Gets the text from the text box

            // Sets the data grid item source to the original collection
            if (string.IsNullOrEmpty(input))
            {
                view.MyDataGrid.ItemsSource = _animeListViewModel.Animes;
                return;
            }

            // Searches the anime list for entries that contain the input and places it in a new bindable collection
            var filteredList = _animeListViewModel.Animes.Where(anime => anime.AnimeName.ToLower().Contains(input.ToLower()));
            BindableCollection<AnimeModel> newList = new BindableCollection<AnimeModel>(filteredList);

            // Sets the data grids item source to the new list
            view.MyDataGrid.ItemsSource = newList;
        }
    }
}
