using Caliburn.Micro;
using AniNeko.Models;
using AniNeko.Views;
using MaterialDesignThemes.Wpf;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

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

        public Dictionary<SortMethod, string> _statusDictionary = new Dictionary<SortMethod, string>()
        {
            { SortMethod.Completed, "Completed" },
            { SortMethod.Watching, "Watching" },
            { SortMethod.PlanToWatch, "Plan to Watch" },
            { SortMethod.Dropped, "Dropped" }
        };

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

            foreach (var anime in Animes)
            {
                if (anime.WatchStatus != _statusDictionary.GetValueOrDefault(SortType))
                {
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

        public ICommand RunEditAnimeDialogCommand => new RelayCommand(ExecuteEditAnimeDialog);

        private async void ExecuteEditAnimeDialog(object o)
        {
            var view = new EditAnimeDialogView
            {
                DataContext = new AnimeModel()
            };

            if (SelectedAnime != null)
            {
                // Populates the edit dialog view with values from selected anime
                view.NameHeader.Text = "Edit " + SelectedAnime.AnimeName;
                view.AnimeName.Text = SelectedAnime.AnimeName;
                view.WatchStatus.SelectedItem = SelectedAnime.WatchStatus;
                view.CurrentEpisode.Text = SelectedAnime.CurrentEpisode.ToString();
                view.TotalEpisodes.Text = SelectedAnime.TotalEpisodes.ToString();
                view.RatingBar.Value = SelectedAnime.Rating;

                var result = await DialogHost.Show(view, "RootDialog");

                if (result.ToString() == "Accept")
                {
                    // Sets the values of the selected anime to the new values within the dialog
                    SelectedAnime.AnimeName = view.AnimeName.Text;
                    SelectedAnime.WatchStatus = view.WatchStatus.SelectedItem.ToString();

                    if (int.TryParse(view.CurrentEpisode.Text, out int currentEpisode))
                        SelectedAnime.CurrentEpisode = currentEpisode;
                    else
                        SelectedAnime.CurrentEpisode = 0;

                    if (int.TryParse(view.TotalEpisodes.Text, out int totalEpisodes))
                        SelectedAnime.TotalEpisodes = totalEpisodes;
                    else
                        SelectedAnime.TotalEpisodes = 0;

                    SelectedAnime.Rating = view.RatingBar.Value;

                    // Updates the selected anime within the SQLite database
                    SQLiteDataAccess.UpdateAnime(SelectedAnime);
                }
                else if (result.ToString() == "Remove")
                {
                    RemoveAnime(SelectedAnime);
                }

                // If cancel is pressed, just closes the dialog
            }
        }

        private void RemoveAnime(AnimeModel animeToRemove)
        {
            // Removes the selection from the SQLite database
            SQLiteDataAccess.DeleteAnime(animeToRemove);

            // Removes the selection from the bindable collection
            Animes.Remove(animeToRemove);
            NotifyOfPropertyChange(() => TotalAnimes);
        }

        public ICommand RemoveContextCommand => new RelayCommand(ExecuteRemoveContext);

        private void ExecuteRemoveContext(object o)
        {
            IList selectedItems = o as IList;
            List<AnimeModel> collection = new List<AnimeModel>(selectedItems.Cast<AnimeModel>());

            foreach (var anime in collection)
            {
                RemoveAnime(anime);
            }
        }
    }
}
