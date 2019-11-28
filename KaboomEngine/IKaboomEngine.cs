using System;

namespace Com.Revo.Games.KaboomEngine
{
    /// <summary>
    /// Kaboom engine interface.
    /// </summary>
    public interface IKaboomEngine
    {
        /// <summary>
        /// The width of the Kaboom board.
        /// </summary>
        int Width { get; }
        /// <summary>
        /// The height of the Kaboom board.
        /// </summary>
        int Height { get; }
        /// <summary>
        /// The number of mines on the Kaboom board.
        /// </summary>
        int NumberOfMines { get; }
        /// <summary>
        /// The cells on this Kaboom board.
        /// </summary>
        IKaboomCellCollection Cells { get; }
        /// <summary>
        /// The state of the Kaboom board.
        /// </summary>
        KaboomEngineState State { get; }
        /// <summary>
        /// Tries to open the cell at the given position and returns the resulting <see cref="KaboomEngineState"/>.
        /// If the board is already solved or exploded or if the specified cell has already been opened, nothing will happen.
        /// </summary>
        /// <param name="x">The x-coordinate of the cell to open.</param>
        /// <param name="y">The y-coordinate of the cell to open.</param>
        /// <returns>The resulting <see cref="KaboomEngineState"/>. <see cref="KaboomEngineState.Sweeping"/> if more work is to do,
        /// <see cref="KaboomEngineState.Solved"/> if the board is solved or
        /// <see cref="KaboomEngineState.Exploded"/> if the specified cell contained a mine.</returns>
        /// <exception cref="IndexOutOfRangeException"><paramref name="x"/> and/or <paramref name="y"/> are outside the board.</exception>
        KaboomEngineState Open(int x, int y);
        /// <summary>
        /// Resets the board to it's inital state.
        /// </summary>
        void Reset();
    }
}
