using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Com.Revo.Games.Kaboom.ViewModels.Com.Aki.WpfCommons.Bindings;
using Com.Revo.Games.KaboomEngine;
using JetBrains.Annotations;

namespace Com.Revo.Games.Kaboom.ViewModels
{
    public class KaboomCellModel : INotifyPropertyChanged
    {
        readonly Cell cell;
        public int X => cell.X;
        public int Y => cell.Y;
        public int AdjacentMines => cell.AdjacentMines;
        public CustomCommand<KaboomCellClickType> ClickCommand { get; }

        public KaboomCellState State =>
            cell.IsOpen
                ? cell.IsMine
                      ? KaboomCellState.Mine
                      : KaboomCellState.Open
                : cell.IsFlagged
                    ? KaboomCellState.Flagged
                    : KaboomCellState.Closed;

        public KaboomCellModel()
            : this(null) { }
        public KaboomCellModel([NotNull] Cell cell)
        {
            this.cell = cell ?? throw new ArgumentNullException(nameof(cell));
            this.cell.CellChanged += (sender, e) => OnPropertyChanged(nameof(State));
            ClickCommand = new CustomCommand<KaboomCellClickType>(OnClicked);
        }
        private void OnClicked(KaboomCellClickType clickType)
        {
            if (clickType == KaboomCellClickType.Left)
                cell.Uncover();
            else
                cell.IsFlagged = !cell.IsFlagged;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
