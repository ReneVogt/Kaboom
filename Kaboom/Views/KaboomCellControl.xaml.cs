using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Com.Revo.Games.Kaboom.ViewModels;

namespace Com.Revo.Games.Kaboom.Views
{
    /// <summary>
    /// Interaktionslogik für KaboomCellControl.xaml
    /// </summary>
    public sealed partial class KaboomCellControl
    {
        public static readonly DependencyProperty CellClickedProperty =
            DependencyProperty.Register(nameof(CellClicked), typeof(ICommand), typeof(KaboomCellControl), new UIPropertyMetadata(null));

        public ICommand CellClicked
        {
            get => (ICommand)GetValue(CellClickedProperty);
            set => SetValue(CellClickedProperty, value);
        }
        public KaboomCellControl()
        {
            InitializeComponent();
        }
        private void RaiseCellClickedEvent(KaboomCellClickType type)
        {
            var command = CellClicked;
            var e = new KaboomCellClickEventArgs(DataContext as KaboomCellModel, type);
            if (command?.CanExecute(e) == true)
                command.Execute(e);
        }
        bool clickEventRaised;
        protected override async void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (clickEventRaised) return;
            clickEventRaised = true;
            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    RaiseCellClickedEvent(e.RightButton == MouseButtonState.Pressed ? KaboomCellClickType.Both : KaboomCellClickType.Left);
                    break;
                case MouseButton.Right:
                    RaiseCellClickedEvent(e.LeftButton == MouseButtonState.Pressed ? KaboomCellClickType.Both : KaboomCellClickType.Right);
                    break;
                default:
                    return;
            }

            e.Handled = true;
            await Task.Delay(250);
            clickEventRaised = false;
        }
    }
}
