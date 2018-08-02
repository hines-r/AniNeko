using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace AniNeko.Views
{
    /// <summary>
    /// Interaction logic for AddAnimeDialogView.xaml
    /// </summary>
    public partial class AddAnimeDialogView : UserControl
    {
        public AddAnimeDialogView()
        {
            InitializeComponent();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
