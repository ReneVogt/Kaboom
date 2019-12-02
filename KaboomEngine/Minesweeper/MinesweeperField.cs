using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.Revo.Games.KaboomEngine.Minesweeper 
{
    sealed class MinesweeperField : Field<object>
    {
        static readonly Random random = new Random();

        public MinesweeperField(int width, int height, int numberOfMines) : base(width, height,numberOfMines)
        {
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
        protected override void OnCellStateChanged(object sender, EventArgs e)
        {
        }
        public override void Uncover(int x, int y)
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
            do
            {
                var cellsToUncover = Cells.Where<Cell<object>>(cell => cell.IsOpen)
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

            } while (State == FieldState.Sweeping);
        }
        void CheckState()
        {
            if (State != FieldState.Sweeping) return;
            if (Cells.Any<Cell<object>>(cell => cell.IsOpen && cell.IsMine))
            {
                State = FieldState.Exploded;
                return;
            }

            if (Cells.All<Cell<object>>(cell => cell.IsOpen || cell.IsMine))
                State = FieldState.Solved;
        }
    }
}
