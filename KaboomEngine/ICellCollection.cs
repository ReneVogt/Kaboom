using System;
using System.Collections.Generic;

namespace Com.Revo.Games.KaboomEngine {
    /// <summary>
    /// Represents a collection of cells in a <see cref="IField"/>.
    /// </summary>
    public interface ICellCollection : IEnumerable<ICell>
    {
        /// <summary>
        /// Gets the Kaboom cell at the specified coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate of the cell.</param>
        /// <param name="y">The x-coordinate of the cell.</param>
        /// <returns>The cell (<see cref="ICell"/> at the specified location.</returns>
        /// <exception cref="IndexOutOfRangeException">The <paramref name="x"/>- and/or <paramref name="y"/>-coordinate are outside the bounds of this field.</exception>
        ICell this[int x, int y] { get; }
    }
}