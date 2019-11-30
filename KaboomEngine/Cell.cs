using System;

namespace Com.Revo.Games.KaboomEngine
{
    /// <summary>
    /// Represents a cell on a Kaboom field.
    /// </summary>
    public sealed class Cell
    {
        readonly Field field;
        bool flagged;

        /// <summary>
        /// The x-coordinate of the cell.
        /// </summary>
        public int X { get; }
        /// <summary>
        /// The y-coordinate of the cell.
        /// </summary>
        public int Y { get; }
        /// <summary>
        /// Number of adjacent cells containing mines.
        /// This property is only valid if the cell has
        /// already been uncovered (hence <see cref="IsOpen"/> is <code>true</code>).
        /// </summary>
        public int AdjacentMines { get; internal set; }
        /// <summary>
        /// Tells if this cell contains a mine.
        /// This property is only valid if the cell has
        /// already been uncovered (hence <see cref="IsOpen"/> is <code>true</code>)
        /// or the containing field's <see cref="Field.State"/> is no longer <see cref="FieldState.Sweeping"/>.
        /// </summary>
        public bool IsMine { get; internal set; }
        /// <summary>
        /// Tells if this cell has already been uncovered.
        /// </summary>
        public bool IsOpen { get; private set; }
        /// <summary>
        /// Gets or sets wether this cell has a flag on it.
        /// This property is only valid as long as the cell is not uncovered.
        /// </summary>
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
        /// <summary>
        /// Raised if the cell's state has been changed.
        /// </summary>
        public event EventHandler CellChanged;

        internal Cell(Field field, int x, int y)
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

        /// <summary>
        /// Uncovers this cell.
        /// This call is delegated to the <see cref="Field.Uncover(int,int)"/> method of the
        /// <see cref="Field"/> owning this cell. See there for what may happen if
        /// this method is called.
        /// </summary>
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