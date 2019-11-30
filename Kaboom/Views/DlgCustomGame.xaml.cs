using System.Windows;
using Com.Revo.Games.Kaboom.ViewModels;

namespace Com.Revo.Games.Kaboom.Views
{
    /// <summary>
    /// Interaktionslogik für DlgCustomGame.xaml
    /// </summary>
    public partial class DlgCustomGame
    {
        public CustomGameViewModel CustomGame => DataContext as CustomGameViewModel;
        public DlgCustomGame()
        {
            InitializeComponent();
        }
        void OnOkClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
