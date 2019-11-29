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

        KaboomCellClickEventArgs leftClickArgs, rightClickArgs, doubleClickArgs;

        public ICommand CellClicked
        {
            get => (ICommand)GetValue(CellClickedProperty);
            set => SetValue(CellClickedProperty, value);
        }
        public KaboomCellControl()
        {
            DataContextChanged += (sender, e) =>
            {
                leftClickArgs = new KaboomCellClickEventArgs(e.NewValue as KaboomCellModel, KaboomCellClickType.Left);
                rightClickArgs = new KaboomCellClickEventArgs(e.NewValue as KaboomCellModel, KaboomCellClickType.Right);
                doubleClickArgs = new KaboomCellClickEventArgs(e.NewValue as KaboomCellModel, KaboomCellClickType.Double);
            };
            InitializeComponent();
        }
        private void RaiseCellClickedEvent(KaboomCellClickEventArgs e)
        {
            var command = CellClicked;
            if (command?.CanExecute(e) == true)
                command.Execute(e);
        }
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (e.ChangedButton != MouseButton.Left) return;
            e.Handled = true;
            RaiseCellClickedEvent(doubleClickArgs);
        }
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.ChangedButton != MouseButton.Left && e.ChangedButton != MouseButton.Right) return;
            e.Handled = true;
            RaiseCellClickedEvent(e.ChangedButton == MouseButton.Left ? leftClickArgs : rightClickArgs);
        }
    }
}
