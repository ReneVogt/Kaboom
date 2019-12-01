using System;

namespace Com.Revo.Games.KaboomEngine
{
    sealed class Cell<TState> : ICell
    {
        readonly IField field;
        bool flagged;

        public int X { get; }
        public int Y { get; }
        public int AdjacentMines { get; internal set; }
        public bool IsMine { get; internal set; }
        public bool IsOpen { get; private set; }
        public bool IsFlagged
        {
            get => flagged;
            set
            {
                if (IsOpen || value == flagged || field.State != FieldState.Sweeping) return;
                flagged = value;
                CellChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        internal TState State { get; set; }
        public event EventHandler CellChanged;

        internal Cell(IField field, int x, int y)
        {
            X = x;
            Y = y;
            this.field = field;
            this.field.StateChanged += (sender, e) =>
            {
                if (!IsMine || IsOpen || flagged) return;
                if (field.State == FieldState.Exploded)
                    IsOpen = true;
                else if (field.State == FieldState.Solved)
                    flagged = true;
                else return;
                CellChanged?.Invoke(this, EventArgs.Empty);
            };
        }

        public void Uncover()
        {
            field.Uncover(X, Y);
        }

        internal void UncoverInternal(int adjacentMines)
        {
            if (IsOpen) return;
            IsOpen = true;
            AdjacentMines = adjacentMines;
            flagged = false;
            CellChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}