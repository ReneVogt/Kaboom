using System;
using System.Linq;
using JetBrains.Annotations;

namespace Com.Revo.Games.KaboomEngine.Kaboom
{
    sealed class KaboomField : Field<KaboomState>
    {
        readonly ISolveKaboomField solver;
        public KaboomField(int width, int height, int numberOfMines, [NotNull] ISolveKaboomField solver) : base(width, height, numberOfMines)
        {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
            if (width < 1) throw new ArgumentOutOfRangeException(nameof(width), width, "The field must contain at least one column.");
            if (width > 1000) throw new ArgumentOutOfRangeException(nameof(width), width, "The field can not contain more than 1000 columns.");
            if (height < 1) throw new ArgumentOutOfRangeException(nameof(height), height, "The field must contain at least one row.");
            if (height > 1000) throw new ArgumentOutOfRangeException(nameof(height), height, "The field can not contain more than 1000 row.");
            if (numberOfMines > width * height) throw new ArgumentOutOfRangeException(nameof(numberOfMines), numberOfMines, "The number of mines cannot be larger than the number of cells.");
            this.solver = solver ?? throw new ArgumentNullException(nameof(solver));
#pragma warning restore CA1303 // Do not pass literals as localized parameters

            foreach (var cell in Cells)
            {
                cell.AdjacentMines = -1;
                cell.State = KaboomState.None;
            }
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
                solver.Solve(this, cell.X, cell.Y);
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
                var cellsToUncover = Cells.Where<Cell<KaboomState>>(cell => cell.IsOpen)
                                          .Select(cell =>
                                                      new
                                                      {
                                                          cell,
                                                          closedNeighbours = cell.Neighbours
                                                                                 .Where(neighbour => !neighbour.IsOpen)
                                                                                 .ToArray()
                                                      })
                                          .Where(x => x.cell.AdjacentMines == 0)//x.closedNeighbours.Count(neighbour => neighbour.IsFlagged))
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
            if (Cells.Any<Cell<KaboomState>>(cell => cell.IsOpen && cell.IsMine))
            {
                State = FieldState.Exploded;
                return;
            }

            if (Cells.All<Cell<KaboomState>>(cell => cell.IsOpen || cell.IsMine))
                State = FieldState.Solved;
        }

        protected override void OnCellStateChanged(object sender, EventArgs e)
        {
        }
    }
}
