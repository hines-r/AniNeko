using Caliburn.Micro;
using AniNeko.Views;
using System.Windows.Input;
using System.Linq;
using System.Windows.Controls;

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

                _animeListViewModel.SortAnimeByStatus(); // Properly sorts the list after specifying the sort method
            }
        }

        private void StopSearching()
        {
            _animeListViewModel.IsSearching = false;          
            _animeListViewModel.SortAnimeByStatus();
        }

        public ICommand SearchCommand => new RelayCommand(ExecuteSearch);

        private void ExecuteSearch(object _input)
        {
            string input = _input.ToString(); // Gets the text from the search bar
            AnimeListViewModel animeVM = _animeListViewModel;
            
            // Cancels the search if there is no input
            if (string.IsNullOrEmpty(input))
            {
                StopSearching();
                return;
            }

            // Searches the anime list for entries that contain the input and places it in a new bindable collection
            var filteredList = animeVM.Animes.Where(anime => anime.AnimeName.ToLower().Contains(input.ToLower()));

            animeVM.IsSearching = true;

            foreach (var anime in animeVM.Animes)
            {
                // If the anime is within the filtered list, make it visible and flag it for searching
                // If not, hide it and make sure it isn't flagged for searching
                if (filteredList.Contains(anime))
                {
                    anime.IsFlaggedForSearch = true;
                    anime.Hidden = false;
                }
                else
                {
                    anime.IsFlaggedForSearch = false;
                    anime.Hidden = true;
                }
            }

            // Sorts the list by status when a search query is entered
            // This ensures if the user enters a search under a sorting tab, the list will still be sorted
            if (Selected != 0)
                animeVM.SortAnimeByStatus();
        }

        public ICommand CancelSearchCommand => new RelayCommand(ExecuteCancelSearch);

        private void ExecuteCancelSearch(object _textBox)
        {
            TextBox searchBox = _textBox as TextBox;

            // Sets the text within the search bar to nothing
            searchBox.Text = "";
            StopSearching();
        }
    }
}
