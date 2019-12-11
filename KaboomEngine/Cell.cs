using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Com.Revo.Games.KaboomEngine
{
    [ExcludeFromCodeCoverage]
    sealed class Cell<TState> : ICell
    {
        bool flagged;
        bool isOpen;
        bool isMine;
        int adjacentMines;
        TState state;

        public IField Field { get; }
        public int X { get; }
        public int Y { get; }
        public int AdjacentMines
        {
            get => adjacentMines;
            internal set
            {
                if (adjacentMines == value) return;
                adjacentMines = value;
                RaiseCellChangedEvent();
            }
        }
        public bool IsMine
        {
            get => isMine;
            internal set
            {
                if (isMine == value) return;
                isMine = value;
                RaiseCellChangedEvent();
            }
        }
        public bool IsOpen
        {
            get => isOpen;
            internal set
            {
                if (isOpen == value) return;
                isOpen = value;
                if (isOpen) flagged = false;
                RaiseCellChangedEvent();
            }
        }
        public bool IsFlagged
        {
            get => flagged;
            set
            {
                if (IsOpen || value == flagged || Field.State != FieldState.Sweeping) return;
                flagged = value;
                RaiseCellChangedEvent();
            }
        }
        public IEnumerable<ICell> Neighbours => Field.GetCoordinatesAdjacentTo(X, Y).Select(c => Field.Cells[c.x, c.y]);
        internal TState State
        {
            get => state;
            set
            {
                if (state?.Equals(value) == true) return;
                state = value;
                RaiseCellChangedEvent();
            }
        }
        public event EventHandler CellChanged;

        internal Cell(IField field, int x, int y)
        {
            X = x;
            Y = y;
            Field = field;
            Field.StateChanged += (sender, e) =>
            {
                if (!IsMine || IsOpen || flagged) return;
                if (field.State == FieldState.Exploded)
                    IsOpen = true;
                else if (field.State == FieldState.Solved)
                    flagged = true;
                else return;
                RaiseCellChangedEvent();
            };
        }

        public void Uncover()
        {
            Field.Uncover(X, Y);
        }

        public override string ToString() => $"Cell({X}, {Y}): {(IsOpen ? "Open" : "Closed")} {(IsMine ? "Mine" : "Free")} {AdjacentMines} ({State})";

        internal void RaiseCellChangedEvent() => CellChanged?.Invoke(this, EventArgs.Empty);
    }
}