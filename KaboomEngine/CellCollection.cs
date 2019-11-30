using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Com.Revo.Games.KaboomEngine {
    /// <summary>
    /// Represents a two-dimensional readonly collection
    /// of Kaboom cells.
    /// </summary>
    public sealed class CellCollection : IEnumerable<Cell>
    {
        readonly Cell[][] cells;
        internal CellCollection(int x, int y) => cells = Enumerable.Range(0, x).Select(i => new Cell[y]).ToArray();

        /// <summary>
        /// Gets the Kaboom cell at the specified coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate of the cell.</param>
        /// <param name="y">The x-coordinate of the cell.</param>
        /// <returns>The cell (<see cref="Cell"/> at the specified location.</returns>
        /// <exception cref="IndexOutOfRangeException">The <paramref name="x"/>- and/or <paramref name="y"/>-coordinate are outside the bounds of this field.</exception>
        public Cell this[int x, int y]
        {
            get => cells[x][y];
            internal set => cells[x][y] = value;
        }

        /// <inheritdoc />
        public IEnumerator<Cell> GetEnumerator() => cells.SelectMany(column => column).GetEnumerator();
        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
