using Com.Revo.Games.Kaboom.ViewModels;

namespace Com.Revo.Games.Kaboom.Views
{
    /// <summary>
    /// Interaktionslogik für KaboomBoardControl.xaml
    /// </summary>
    public partial class KaboomBoardControl
    {
        public KaboomBoardModel ViewModel
        {
            get => DataContext as KaboomBoardModel;
            set => DataContext = value;
        }
        public KaboomBoardControl()
        {
            InitializeComponent();
        }
    }
}
