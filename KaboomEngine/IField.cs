using System;

namespace Com.Revo.Games.KaboomEngine {
    /// <summary>
    /// Represents a Kaboom game field.
    /// </summary>
    public interface IField {
        /// <summary>
        /// The width of this Kaboom field.
        /// </summary>
        int Width { get; }
        /// <summary>
        /// The height of this Kaboom field.
        /// </summary>
        int Height { get; }
        /// <summary>
        /// Number of mines on this Kaboom field.
        /// </summary>
        int NumberOfMines { get; }
        /// <summary>
        /// The cells of this Kaboom field.
        /// </summary>
        ICellCollection Cells { get; }
        /// <summary>
        /// The state of this Kaboom field.
        /// </summary>
        FieldState State { get; }
        /// <summary>
        /// Raised if the state of this field has changed.
        /// </summary>
        event EventHandler StateChanged;
        /// <summary>
        /// Uncovers cells depending on the state of the cell at the specified location.
        /// If the cell is closed and flagged, nothing will happen.
        /// If the cell is closed and not flagged, it will be uncovered.
        /// If the cell contains a mine, you are dead.
        /// If it does not contain a mine and none of the adjacent cells contains mine,
        /// the adjacent cells will be uncovered, too.
        /// If this cell has already been uncovered, all(!) flag-satisfied cells will be opened.
        /// That is, for all cells that are opened and have an adjacent mine count equal to the
        /// number of adjacent flagged cells, the not-flagged adjacent cells will be uncovered.
        /// </summary>
        /// <param name="x">The x-coordinate of the cell to uncover.</param>
        /// <param name="y">The y-coordinate of the cell to uncover.</param>
        /// <exception cref="IndexOutOfRangeException">The <paramref name="x"/>- and/or <paramref name="y"/>-coordinate was outside the bounds of this field.</exception>
        void Uncover(int x, int y);
    }
}