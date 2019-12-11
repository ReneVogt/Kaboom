using System;
using System.Diagnostics.CodeAnalysis;

namespace Com.Revo.Games.KaboomEngine 
{
    [ExcludeFromCodeCoverage]
    abstract class Field<TState> : IField
    {
        FieldState state = FieldState.Sweeping;

        public int Width { get; }
        public int Height { get; }
        /// <inheritdoc />
        public int NumberOfMines { get; }
        internal CellCollection<TState> Cells { get; }
        ICellCollection IField.Cells => Cells;
        public FieldState State
        {
            get => state;
            protected set
            {
                if (state == value) return;
                state = value;
                StateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler StateChanged;

        protected Field(int width, int height, int numberOfMines)
        {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
            if (width < 1) throw new ArgumentOutOfRangeException(nameof(width), width, "The field must contain at least one column.");
            if (width > 1000) throw new ArgumentOutOfRangeException(nameof(width), width, "The field can not contain more than 1000 columns.");
            if (height < 1) throw new ArgumentOutOfRangeException(nameof(height), height, "The field must contain at least one row.");
            if (height > 1000) throw new ArgumentOutOfRangeException(nameof(height), height, "The field can not contain more than 1000 row.");
            if (numberOfMines > width * height)
                throw new ArgumentOutOfRangeException(nameof(numberOfMines), numberOfMines,
                                                      "The number of mines cannot be larger than the number of cells.");
#pragma warning restore CA1303 // Do not pass literals as localized parameters

            Width = width;
            Height = height;
            NumberOfMines = numberOfMines;

            Cells = new CellCollection<TState>(this, Width, Height);
            foreach (var cell in Cells)
                cell.CellChanged += OnCellStateChanged;

        }
        public abstract void Uncover(int x, int y);
        protected abstract void OnCellStateChanged(object sender, EventArgs e);

    }
}
