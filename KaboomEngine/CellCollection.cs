using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Com.Revo.Games.KaboomEngine {
    sealed class CellCollection<TState> : ICellCollection, IEnumerable<Cell<TState>>
    {
        readonly Cell<TState>[][] cells;
        internal CellCollection(IField field, int width, int height) => cells = Enumerable.Range(0, width)
                                                                            .Select(x => Enumerable.Range(0, height)
                                                                                                   .Select(y => new Cell<TState>(field, x, y))
                                                                                                   .ToArray())
                                                                            .ToArray();

        ICell ICellCollection.this[int x, int y] => cells[x][y];
        public Cell<TState> this[int x, int y] => cells[x][y];

        public IEnumerator<Cell<TState>> GetEnumerator() => cells.SelectMany(column => column).GetEnumerator();

        IEnumerator<ICell> IEnumerable<ICell>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
