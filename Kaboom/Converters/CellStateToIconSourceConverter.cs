using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using Com.Revo.Games.Kaboom.ViewModels;

namespace Com.Revo.Games.Kaboom.Converters
{
    public class CellStateToIconSourceConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider) => this;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is KaboomCellState state
                ? state switch
                {
                    //KaboomCellState.Empty => "../Icons/empty.ico",
                    KaboomCellState.Mine => "../Icons/mine.ico",
                    KaboomCellState.Neighbours1 => "../Icons/one.ico",
                    KaboomCellState.Neighbours2 => "../Icons/two.ico",
                    KaboomCellState.Neighbours3 => "../Icons/three.ico",
                    KaboomCellState.Neighbours4 => "../Icons/four.ico",
                    KaboomCellState.Neighbours5 => "../Icons/five.ico",
                    KaboomCellState.Neighbours6 => "../Icons/six.ico",
                    KaboomCellState.Neighbours7 => "../Icons/seven.ico",
                    KaboomCellState.Neighbours8 => "../Icons/eight.ico",
                    KaboomCellState.Closed => "../Icons/tile.ico",
                    KaboomCellState.Flagged => "../Icons/flag.ico",
                    _ => null
                }
                : null;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new InvalidOperationException();
    }
}
