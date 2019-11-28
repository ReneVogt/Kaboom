using System.ComponentModel;
using System.Runtime.CompilerServices;
using Com.Revo.Games.KaboomEngine;
using JetBrains.Annotations;

namespace Com.Revo.Games.Kaboom.ViewModels
{
    public class KaboomCellModel : INotifyPropertyChanged
    {
        public int X { get; }
        public int Y { get; }
        public int AdjacentMines { get; }

        KaboomCellState state;
        public KaboomCellState State
        {
            get => state;
            set
            {
                if (value == state)
                    return;
                state = value;
                OnPropertyChanged();
            }
        }

        public KaboomCellModel()
            : this(null) { }
        public KaboomCellModel(IKaboomCell cell)
        {
            X = cell?.X ?? -1;
            Y = cell?.Y ?? -1;
            AdjacentMines = cell?.AdjacentMines ?? 0;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
