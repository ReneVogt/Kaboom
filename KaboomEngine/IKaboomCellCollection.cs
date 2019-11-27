using System;
using System.Collections.Generic;

namespace Com.Revo.Games.KaboomEngine {
    /// <summary>
    /// A two-dimensional read-only collection of Kaboom board cells.
    /// (<see cref="IKaboomCell"/>).
    /// </summary>
    public interface IKaboomCellCollection : IEnumerable<IKaboomCell>
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
        /// Returns the Kaboom cell (<see cref="IKaboomCell"/> at the specified location.
        /// </summary>
        /// <param name="x">The x-coordinate of the cell.</param>
        /// <param name="y">The y-coordinate of the cell.</param>
        /// <returns>The <see cref="IKaboomCell"/> representing the cell at the specified location.</returns>
        /// <exception cref="IndexOutOfRangeException"><paramref name="x"/> and/or <paramref name="y"/> were outside of this collection.</exception>
        IKaboomCell this[int x, int y] { get; }
    }
}
