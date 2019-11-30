using System;

namespace Com.Revo.Games.KaboomEngine
{
    /// <summary>
    /// A static provider for for Kaboom fields (of type <see cref="IKaboomField"/>.
    /// </summary>
    public static class KaboomFieldProvider
    {
        /// <summary>
        /// Provides an <see cref="IKaboomField"/> using a standard Minesweeper behaviour.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="numberOfMines"></param>
        /// <returns>A fresh Kaboom field with the specified configuration and a default Minesweeper behaviour.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="width"/> and <paramref name="height"/> must be between 1 and 1000 and the number of mines cannot exceed the number of cells.</exception>
        public static IKaboomField CreateMinesweeperField(int width, int height, int numberOfMines) => new MinesweeperField(width, height, numberOfMines);
    }
}
