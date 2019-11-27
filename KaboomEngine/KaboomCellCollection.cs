using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Com.Revo.Games.KaboomEngine {
    sealed class KaboomCellCollection : IKaboomCellCollection
    {
        readonly IKaboomCell[,] cells;
        internal KaboomCellCollection(int width, int height)
        {
            cells = new IKaboomCell[width, height];
        }
        public int Width => cells.GetLength(0);
        public int Height => cells.GetLength(1);

        public IKaboomCell this[int x, int y]
        {
            get => cells[x, y];
            internal set => cells[x, y] = value;
        }
        public IEnumerator<IKaboomCell> GetEnumerator() => Enumerable.Range(0, Height).SelectMany(y => Enumerable.Range(0, Width).Select(x => cells[x, y])).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
