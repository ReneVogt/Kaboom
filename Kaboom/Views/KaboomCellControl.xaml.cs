using System.Windows;
using System.Windows.Input;
using Com.Revo.Games.Kaboom.ViewModels;
using Com.Revo.Games.Kaboom.ViewModels.Com.Aki.WpfCommons.Bindings;

namespace Com.Revo.Games.Kaboom.Views
{
    /// <summary>
    /// Interaktionslogik für KaboomCellControl.xaml
    /// </summary>
    public sealed partial class KaboomCellControl
    {
        public static readonly DependencyProperty CellClickedProperty =
            DependencyProperty.Register(nameof(CellClicked), typeof(ICommand), typeof(KaboomCellControl), new UIPropertyMetadata(null));

        public CustomCommand<KaboomCellClickType> CellClicked
        {
            get => (CustomCommand<KaboomCellClickType>)GetValue(CellClickedProperty);
            set => SetValue(CellClickedProperty, value);
        }
        public KaboomCellControl()
        {
            InitializeComponent();
        }
        private void RaiseCellClickedEvent(KaboomCellClickType clickType)
        {
            var command = CellClicked;
            if (command?.CanExecute(clickType) == true)
                command.Execute(clickType);
        }
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse because FxCop claims to know better
            // ReSharper disable once HeuristicUnreachableCode because FxCop claims to know better
            if (e == null) return;
            if (e.ChangedButton != MouseButton.Left && e.ChangedButton != MouseButton.Right) return;
            e.Handled = true;
            RaiseCellClickedEvent(e.ChangedButton == MouseButton.Left ? KaboomCellClickType.Left: KaboomCellClickType.Right);
        }
    }
}
