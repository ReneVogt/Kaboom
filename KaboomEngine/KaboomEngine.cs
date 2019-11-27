using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.Revo.Games.KaboomEngine {
    sealed class KaboomEngine : IKaboomEngine
    {
        readonly Random random = new Random();
        readonly KaboomCellCollection cells;
        public int Width { get; }
        public int Height { get; }
        public int NumberOfMines { get; }
        public IKaboomCellCollection Cells => cells;
        public KaboomEngineState State { get; private set; }
        public KaboomEngine(int width, int height, int numberOfMines)
        {
            Width = width;
            Height = height;
            NumberOfMines = numberOfMines;
            cells = new KaboomCellCollection(width, height);
            Reset();
        }
        public KaboomEngineState Open(int x, int y)
        {
            if (State == KaboomEngineState.Solved) throw new InvalidOperationException("This board has already been solved.");
            if (State == KaboomEngineState.Exploded) throw new InvalidOperationException("This board is already exploded.");

            var cell = (KaboomCell)cells[x, y];
            if (cell.IsOpen) throw new ArgumentException($"The cell at ({x}, {y}) has already been opened.");

            if (cell.IsMine)
                return State = KaboomEngineState.Exploded;

            OpenAdjacent(x, y);

            return State = cells.All(c => c.IsMine || c.IsOpen) 
                ? KaboomEngineState.Solved
                : KaboomEngineState.Sweeping;
        }
        public void Reset()
        {
            for(int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                cells[x, y] = new KaboomCell {X = x, Y = y};

            for (int mine = 0; mine < NumberOfMines; mine++)
            {
                rand:
                int x = random.Next(Width);
                int y = random.Next(Height);
                var cell = (KaboomCell)cells[x, y];
                if (cell.IsMine) goto rand;
                cell.IsMine = true;
            }

            foreach (var cell in cells.OfType<KaboomCell>())
                cell.AdjacentMines = GetAdjacentCells(cell.X, cell.Y).Count(neighbour => cells[neighbour.x, neighbour.y].IsMine);
        }
        void OpenAdjacent(int x, int y)
        {
            var cell = (KaboomCell)Cells[x, y];
            if (cell.IsOpen) return;

            cell.IsOpen = true;
            if (cell.AdjacentMines > 0) return;

            foreach((int neighbourX, int neighbourY) in GetAdjacentCells(x, y))
                OpenAdjacent(neighbourX, neighbourY);
        }
        IEnumerable<(int x, int y)> GetAdjacentCells(int x, int y)
        {
            if (x > 0 && y > 0) yield return (x - 1, y - 1);
            if (y > 0) yield return (x, y - 1);
            if (x < Width - 1 && y > 0) yield return (x + 1, y - 1);

            if (x > 0) yield return (x - 1, y);
            if (x < Width - 1) yield return (x + 1, y);

            if (x > 0 && y < Height - 1) yield return (x - 1, y + 1);
            if (y < Height - 1) yield return (x, y + 1);
            if (x < Width - 1 && y < Height - 1) yield return (x + 1, y + 1);
        }
    }
}
