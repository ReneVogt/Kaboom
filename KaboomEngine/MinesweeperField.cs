using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.Revo.Games.KaboomEngine 
{
    internal sealed class MinesweeperField : IKaboomField
    {
        FieldState state = FieldState.Sweeping;
        static readonly Random random = new Random();

        public int Width { get; }
        public int Height { get; }
        public int NumberOfMines { get; }
        public CellCollection Cells { get; }
        public FieldState State
        {
            get => state;
            private set
            {
                if (state == value) return;
                state = value;
                StateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler StateChanged;

        public MinesweeperField(int width, int height, int numberOfMines)
        {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
            if (width < 1) throw new ArgumentOutOfRangeException(nameof(width), width, "The field must contain at least one column.");
            if (width > 1000) throw new ArgumentOutOfRangeException(nameof(width), width, "The field can not contain more than 1000 columns.");
            if (height < 1) throw new ArgumentOutOfRangeException(nameof(height), height, "The field must contain at least one row.");
            if (height > 1000) throw new ArgumentOutOfRangeException(nameof(height), height, "The field can not contain more than 1000 row.");
            if (numberOfMines > width*height) throw new ArgumentOutOfRangeException(nameof(numberOfMines), numberOfMines, "The number of mines cannot be larger than the number of cells.");
#pragma warning restore CA1303 // Do not pass literals as localized parameters
            
            Width = width;
            Height = height;
            NumberOfMines = numberOfMines;

            Cells = new CellCollection(Width, Height);
            for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                var cell = new Cell(this, x, y);
                Cells[x, y] = cell;
                cell.CellChanged += (sender, e) => StateChanged?.Invoke(this, e);
            }

            HashSet<(int x, int y)> used = new HashSet<(int x, int y)>();

            for (int mine = 0; mine < NumberOfMines; mine++)
            {
                rand:
                int x = random.Next(Width);
                int y = random.Next(Height);
                if (!used.Add((x, y))) goto rand;
                Cells[x, y].IsMine = true;
            }

            foreach (var cell in Cells)
                cell.AdjacentMines = this.GetCoordinatesAdjacentTo(cell.X, cell.Y).Count(c => Cells[c.x, c.y].IsMine);

        }
        public void Uncover(int x, int y)
        {
            Uncover(x, y, true);
        }
        void Uncover(int x, int y, bool cascade)
        {
            if (State != FieldState.Sweeping) return;

            var cell = Cells[x, y];
            if (!cell.IsOpen && cell.IsFlagged) return;

            if (!cell.IsOpen)
            {
                cell.UncoverInternal(cell.AdjacentMines);
                if (cell.IsMine)
                {
                    State = FieldState.Exploded;
                    return;
                }
            }

            if (cascade) OpenCascade();
            CheckState();
        }
        void OpenCascade()
        {
            cascade:
            var cellsToUncover = Cells.Where(cell => cell.IsOpen)
                                      .Select(cell =>
                                                  new
                                                  {
                                                      cell,
                                                      closedNeighbours = this.GetCoordinatesAdjacentTo(cell.X, cell.Y)
                                                                         .Select(c => Cells[c.x, c.y])
                                                                         .Where(neighbour => !neighbour.IsOpen)
                                                                         .ToArray()
                                                  })
                                      .Where(x => x.cell.AdjacentMines == x.closedNeighbours.Count(neighbour => neighbour.IsFlagged))
                                      .SelectMany(x => x.closedNeighbours.Where(neighbour => !neighbour.IsFlagged))
                                      .Distinct()
                                      .ToList();
            if (cellsToUncover.Count == 0) return;
            cellsToUncover.ForEach(cell => Uncover(cell.X, cell.Y, false));
            goto cascade;
        }
        void CheckState()
        {
            if (State != FieldState.Sweeping) return;
            if (Cells.Any(cell => cell.IsOpen && cell.IsMine))
            {
                State = FieldState.Exploded;
                return;
            }

            if (Cells.All(cell => cell.IsOpen || cell.IsMine))
                State = FieldState.Solved;
        }
    }
}
