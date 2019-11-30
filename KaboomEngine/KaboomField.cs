using System;

namespace Com.Revo.Games.KaboomEngine
{
    class KaboomField : IKaboomField {
        public int Width { get; }
        public int Height { get; }
        public int NumberOfMines { get; }
        public CellCollection Cells { get; }
        public FieldState State => FieldState.Sweeping;
        public event EventHandler StateChanged;

        public KaboomField(int width, int height, int numberOfMines)
        {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
            if (width < 1) throw new ArgumentOutOfRangeException(nameof(width), width, "The field must contain at least one column.");
            if (width > 1000) throw new ArgumentOutOfRangeException(nameof(width), width, "The field can not contain more than 1000 columns.");
            if (height < 1) throw new ArgumentOutOfRangeException(nameof(height), height, "The field must contain at least one row.");
            if (height > 1000) throw new ArgumentOutOfRangeException(nameof(height), height, "The field can not contain more than 1000 row.");
            if (numberOfMines > width * height) throw new ArgumentOutOfRangeException(nameof(numberOfMines), numberOfMines, "The number of mines cannot be larger than the number of cells.");
#pragma warning restore CA1303 // Do not pass literals as localized parameters

            Width = width;
            Height = height;
            NumberOfMines = numberOfMines;

            Cells = new CellCollection(this, Width, Height);
            foreach (var cell in Cells)
                cell.CellChanged += (sender, e) => StateChanged?.Invoke(this, e);
        }
        public void Uncover(int x, int y)
        {
        }
    }
}
