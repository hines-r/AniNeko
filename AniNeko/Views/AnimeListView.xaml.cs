using AniNeko.Models;
using AniNeko.ViewModels;
using MaterialDesignThemes.Wpf;
using System.Windows.Controls;
using System.Windows.Input;

namespace AniNeko.Views
{
    /// <summary>
    /// Interaction logic for DataGridView.xaml
    /// </summary>
    public partial class AnimeListView : UserControl
    {
        public AnimeListView()
        {
            InitializeComponent();
        }

        private async void Animes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var view = new EditAnimeDialogView
            {
                DataContext = new AnimeModel()
            };

            AnimeModel selection = (AnimeModel)MyDataGrid.SelectedItem;

            if (selection != null)
            {
                // Populates the edit dialog view with values from selected anime
                view.NameHeader.Text = "Edit " + selection.AnimeName;
                view.AnimeName.Text = selection.AnimeName;
                view.WatchStatus.SelectedItem = selection.WatchStatus;
                view.CurrentEpisode.Text = selection.CurrentEpisode.ToString();
                view.TotalEpisodes.Text = selection.TotalEpisodes.ToString();
                view.RatingBar.Value = selection.Rating;

                var result = await DialogHost.Show(view, "RootDialog");

                if (result.ToString() == "Accept")
                {
                    // Sets the values of the selected anime to the new values within the dialog
                    selection.AnimeName = view.AnimeName.Text;
                    selection.WatchStatus = view.WatchStatus.SelectedItem.ToString();

                    if (int.TryParse(view.CurrentEpisode.Text, out int currentEpisode))
                        selection.CurrentEpisode = currentEpisode;
                    else
                        selection.CurrentEpisode = 0;

                    if (int.TryParse(view.TotalEpisodes.Text, out int totalEpisodes))
                        selection.TotalEpisodes = totalEpisodes;
                    else
                        selection.TotalEpisodes = 0;

                    selection.Rating = view.RatingBar.Value;

                    // Updates the selected anime within the SQLite database
                    SQLiteDataAccess.UpdateAnime(selection);
                }
                else if (result.ToString() == "Remove")
                {
                    // Removes the selection from the bindable collection within
                    // the data grids data context (AnimeListViewModel)
                    AnimeListViewModel dataContext = (AnimeListViewModel)MyDataGrid.DataContext;
                    dataContext.Animes.Remove(selection);
                    dataContext.UpdateAnimeCount();

                    // Removes the selection from the SQLite database
                    SQLiteDataAccess.DeleteAnime(selection);
                }

                // If cancel is pressed, just closes the dialog
            }

        }
    }
}
