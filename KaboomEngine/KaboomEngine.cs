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
            if (State != KaboomEngineState.Sweeping) return State;

            var cell = (KaboomCell)cells[x, y];
            if (cell.IsOpen) return State;

            if (cell.IsMine)
            {
                cell.IsOpen = true;
                return State = KaboomEngineState.Exploded;
            }

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

            HashSet<(int x, int y)> used = new HashSet<(int x, int y)>();

            for (int mine = 0; mine < NumberOfMines; mine++)
            {
                rand:
                int x = random.Next(Width);
                int y = random.Next(Height);
                if (!used.Add((x, y))) goto rand;
                var cell = (KaboomCell)cells[x, y];
                cell.IsMine = true;
            }

            foreach (var cell in cells.OfType<KaboomCell>())
                cell.AdjacentMines = this.GetCellsAdjacentTo(cell.X, cell.Y).Count(neighbour => cells[neighbour.x, neighbour.y].IsMine);
        }
        void OpenAdjacent(int x, int y)
        {
            var cell = (KaboomCell)Cells[x, y];
            if (cell.IsOpen) return;

            cell.IsOpen = true;
            if (cell.AdjacentMines > 0) return;

            foreach((int neighbourX, int neighbourY) in this.GetCellsAdjacentTo(x, y))
                OpenAdjacent(neighbourX, neighbourY);
        }
    }
}
