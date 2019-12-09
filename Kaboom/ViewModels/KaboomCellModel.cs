using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Com.Revo.Games.Kaboom.ViewModels.Com.Aki.WpfCommons.Bindings;
using Com.Revo.Games.KaboomEngine;
using Com.Revo.Games.KaboomEngine.Kaboom;
using JetBrains.Annotations;

namespace Com.Revo.Games.Kaboom.ViewModels
{
    public class KaboomCellModel : INotifyPropertyChanged
    {
        readonly ICell cell;
        readonly KaboomBoardModel boardModel;
        int adjacentMines;
        KaboomDebugState debugState = KaboomDebugState.None;

        public int X => cell.X;
        public int Y => cell.Y;
        public int AdjacentMines
        {
            get => adjacentMines;
            set
            {
                if (adjacentMines == value) return;
                adjacentMines = value;
                OnPropertyChanged();
            }
        }
        public CustomCommand<KaboomCellClickType> ClickCommand { get; }

        public KaboomCellState State =>
            cell.IsOpen
                ? cell.IsMine
                      ? KaboomCellState.Mine
                      : KaboomCellState.Open
                : cell.IsFlagged
                    ? KaboomCellState.Flagged
                    : KaboomCellState.Closed;

        public KaboomDebugState DebugState
        {
            get => boardModel.DebugMode ? debugState : KaboomDebugState.None;
            private set
            {
                if (value == debugState)
                    return;
                debugState = value;
                OnPropertyChanged();
            }
        }

        public KaboomCellModel([NotNull] ICell cell, [NotNull] KaboomBoardModel boardModel)
        {
            this.cell = cell ?? throw new ArgumentNullException(nameof(cell));
            this.boardModel = boardModel ?? throw new ArgumentNullException(nameof(boardModel));
            this.boardModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(boardModel.DebugMode))
                    OnPropertyChanged(nameof(DebugState));
            };
            this.cell.CellChanged += (sender, e) =>
            {
                if (!(sender is ICell changedCell)) return;
                if (changedCell is Cell<KaboomState> changedKaboomCell)
                {
                    DebugState = changedCell.IsOpen || changedKaboomCell.State == KaboomState.None
                                         ? KaboomDebugState.None
                                         : changedKaboomCell.State == KaboomState.Mine
                                             ? KaboomDebugState.Mine
                                             : changedKaboomCell.State == KaboomState.Free
                                                 ? KaboomDebugState.Free
                                                 : KaboomDebugState.Indeterminate;
                }

                AdjacentMines = changedCell.AdjacentMines;
                OnPropertyChanged(nameof(State));
            };
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
