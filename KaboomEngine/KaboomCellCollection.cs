using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Com.Revo.Games.KaboomEngine {
    sealed class KaboomCellCollection : IKaboomCellCollection
    {
        readonly KaboomCell[][] cells;
        internal KaboomCellCollection(int width, int height)
        {
            if (width < 1) throw new ArgumentException("There must be at least one column.", nameof(width));
            if (height < 1) throw new ArgumentException("There must be at least one row.", nameof(height));
            Width = width;
            Height = height;
            cells = Enumerable.Range(0, Height).Select(i => new KaboomCell[Width]).ToArray();
        }
        public int Width {get; }
        public int Height { get; }

        public IKaboomCell this[int x, int y]
        {
            get => cells[y][x];
            internal set => cells[y][x] = (KaboomCell)value;
        }
        public IEnumerator<IKaboomCell> GetEnumerator() => cells.SelectMany(row => row).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
