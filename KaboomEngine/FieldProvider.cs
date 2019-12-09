using System;
using Com.Revo.Games.KaboomEngine.Kaboom;
using Com.Revo.Games.KaboomEngine.Minesweeper;
using Com.Revo.Games.KaboomEngine.Wrapper;

namespace Com.Revo.Games.KaboomEngine
{
    /// <summary>
    /// A static provider for for Kaboom fields (of type <see cref="IField"/>.
    /// </summary>
    public static class FieldProvider
    {
        /// <summary>
        /// Provides an <see cref="IField"/> using a standard Minesweeper behaviour.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="numberOfMines"></param>
        /// <returns>A fresh Kaboom field with the specified configuration and a default Minesweeper behaviour.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="width"/> and <paramref name="height"/> must be between 1 and 1000 and the number of mines cannot exceed the number of cells.</exception>
        public static IField CreateMinesweeperField(int width, int height, int numberOfMines) => new MinesweeperField(width, height, numberOfMines);
        /// <summary>
        /// Provides an <see cref="IField"/> using the Kaboom behaviour (you lose when you guess, but you won't fail if you're forced to guess).
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="numberOfMines"></param>
        /// <returns>A fresh Kaboom field with the specified configuration.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="width"/> and <paramref name="height"/> must be between 1 and 1000 and the number of mines cannot exceed the number of cells.</exception>
        public static IField CreateKaboomField(int width, int height, int numberOfMines) => new KaboomField(width, height, numberOfMines, new KaboomFieldSolver(new ConstraintsGenerator(), new RandomProvider()));
    }
}
