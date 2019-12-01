using System;

namespace Com.Revo.Games.KaboomEngine.Kaboom
{
    sealed class KaboomField : Field<KaboomState>
    {
        public KaboomField(int width, int height, int numberOfMines) : base(width, height, numberOfMines)
        {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
            if (width < 1) throw new ArgumentOutOfRangeException(nameof(width), width, "The field must contain at least one column.");
            if (width > 1000) throw new ArgumentOutOfRangeException(nameof(width), width, "The field can not contain more than 1000 columns.");
            if (height < 1) throw new ArgumentOutOfRangeException(nameof(height), height, "The field must contain at least one row.");
            if (height > 1000) throw new ArgumentOutOfRangeException(nameof(height), height, "The field can not contain more than 1000 row.");
            if (numberOfMines > width * height) throw new ArgumentOutOfRangeException(nameof(numberOfMines), numberOfMines, "The number of mines cannot be larger than the number of cells.");
#pragma warning restore CA1303 // Do not pass literals as localized parameters
        }
        public override void Uncover(int x, int y)
        {
        }
        protected override void OnCellStateChanged(object sender, EventArgs e)
        {
        }
    }
}
