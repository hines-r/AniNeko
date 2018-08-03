using Caliburn.Micro;
using AniNeko.Models;
using AniNeko.Views;
using MaterialDesignThemes.Wpf;
using System;
using System.Windows.Input;

namespace AniNeko.ViewModels
{
    public class AnimeListViewModel : Screen
    {
        private BindableCollection<AnimeModel> _animes;
        private AnimeModel _selectedAnime;
        public SortMethod _sortType;

        public enum SortMethod
        {
            All,
            Watching,
            Completed,
            PlanToWatch,
            Dropped
        }

        public AnimeListViewModel()
        {
            LoadAnimeList();
        }

        public void LoadAnimeList()
        {
            // Populates the bindable collection with all animes within
            // the SQLite database upon initilization
            _animes = new BindableCollection<AnimeModel>();
            var list = SQLiteDataAccess.LoadAnime();

            foreach (var anime in list)
            {
                _animes.Add(anime);
                NotifyOfPropertyChange(() => TotalAnimes);
            }
        }

        public SortMethod SortType
        {
            get { return _sortType; }
            set
            {
                _sortType = value;
                NotifyOfPropertyChange(() => SortType);
                SortAnimeByStatus();
            }
        }

        public void SortAnimeByStatus()
        {
            // Resets all animes within the data grid before sorting again
            // This allows previously hidden items to be visible again
            foreach (var anime in _animes)
            {
                anime.Hidden = false;
            }

            if (SortType == SortMethod.Watching)
            {
                foreach (var anime in _animes)
                {
                    if (anime.WatchStatus != "Watching")
                        anime.Hidden = true;
                }
            }
            else if (SortType == SortMethod.Completed)
            {
                foreach (var anime in _animes)
                {
                    if (anime.WatchStatus != "Completed")
                        anime.Hidden = true;
                }
            }
            else if (SortType == SortMethod.PlanToWatch)
            {
                foreach (var anime in _animes)
                {
                    if (anime.WatchStatus != "Plan to Watch")
                        anime.Hidden = true;
                }
            }
            else if (SortType == SortMethod.Dropped)
            {
                foreach (var anime in _animes)
                {
                    if (anime.WatchStatus != "Dropped")
                        anime.Hidden = true;
                }
            }

            // Counts total number of visible animes
            NotifyOfPropertyChange(() => TotalAnimes);
        }

        private void AddToAnimeList(AnimeModel animeToAdd)
        {
            // Saves the anime to the database
            // Sets the id of the anime to the database version
            SQLiteDataAccess.SaveAnime(animeToAdd);
            animeToAdd.Id = SQLiteDataAccess.GetLastRecord().Id;

            // Inserts an anime in the first index of the bindable collection
            // This makes it appear at the top of the data grid
            _animes.Insert(0, animeToAdd);
            NotifyOfPropertyChange(() => TotalAnimes);
        }

        public string TotalAnimes
        {
            get
            {
                int count = 0;

                // Returns the total number of animes within the bindable collection
                foreach (var anime in _animes)
                {
                    if (!anime.Hidden)
                        count++;
                }

                return "Total: " + count;
            }
        }

        public BindableCollection<AnimeModel> Animes
        {
            get { return _animes; }
            set
            {
                _animes = value;
                NotifyOfPropertyChange(() => Animes);
            }
        }

        public AnimeModel SelectedAnime
        {
            get { return _selectedAnime; }
            set
            {
                _selectedAnime = value;
                NotifyOfPropertyChange(() => SelectedAnime);
            }
        }

        public ICommand RunAddAnimeDialogCommand => new RelayCommand(ExecuteRunAddAnimeDialog);

        private async void ExecuteRunAddAnimeDialog(object o)
        {
            var view = new AddAnimeDialogView
            {
                DataContext = new AnimeModel()
            };

            var result = await DialogHost.Show(view, "RootDialog");

            // If the user clicks the accept button, returns true for the command parameter
            // Closes the dialog if cancel is pressed by returning false
            if (result.Equals(true))
            {
                AnimeModel animeToAdd = new AnimeModel();
                animeToAdd.AnimeName = view.AnimeName.Text;

                // Checks to see if the input is valid
                if (view.WatchStatus.SelectedItem != null)
                    animeToAdd.WatchStatus = view.WatchStatus.SelectedItem.ToString();

                if (int.TryParse(view.CurrentEpisode.Text, out int currentEpisode))
                    animeToAdd.CurrentEpisode = currentEpisode;

                if (int.TryParse(view.TotalEpisodes.Text, out int totalEpisodes))
                    animeToAdd.TotalEpisodes = totalEpisodes;

                animeToAdd.Rating = view.Rating.Value;

                // Adds the anime to the bindable collection and sql database
                AddToAnimeList(animeToAdd);
            }
        }
    }
}
