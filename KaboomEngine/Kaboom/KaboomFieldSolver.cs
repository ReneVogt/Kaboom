using System;
using System.Collections.Generic;
using System.Linq;
using Com.Revo.Games.KaboomEngine.Wrapper;
using JetBrains.Annotations;
using Microsoft.SolverFoundation.Solvers;

namespace Com.Revo.Games.KaboomEngine.Kaboom 
{
    class KaboomFieldSolver : ISolveKaboomField
    {
        readonly IProvideRandom random;
        readonly IGenerateConstraints constraintsGenerator;
        readonly IKaboomSatSolver satSolver;

        KaboomField field;
        Cell<KaboomState> cellToOpen;
        List<Cell<KaboomState>> openBorderCells;
        List<Cell<KaboomState>> closedBorderCells;
        List<Cell<KaboomState>> closedBorderCellsExclusiveToOpenedCell;
        Dictionary<Cell<KaboomState>, List<Cell<KaboomState>>> openToClosedBorderCells;
        List<Cell<KaboomState>> undefinedBorderCells;

        int minimumMines, maximumMines;

        Dictionary<int, Cell<KaboomState>> boolIDsToCell;
        Dictionary<Cell<KaboomState>, int> cellsToBoolID;
        List<Literal[]> constraints;

        SatSolution chosenSolution;
        Dictionary<Cell<KaboomState>, List<bool>> literalsByCell;

        public KaboomFieldSolver([NotNull] IGenerateConstraints constraintsGenerator, [NotNull] IProvideRandom random, [NotNull] IKaboomSatSolver satSolver)
        {
            this.constraintsGenerator = constraintsGenerator ?? throw new ArgumentNullException(nameof(constraintsGenerator));
            this.random = random ?? throw new ArgumentNullException(nameof(random));
            this.satSolver = satSolver ?? throw new ArgumentNullException(nameof(satSolver));
        }

        public void Solve([NotNull] KaboomField kaboomField, int x, int y)
        {
            if (kaboomField == null)
                throw new ArgumentNullException(nameof(kaboomField));
            if (x < 0 || x >= kaboomField.Width)
                throw new ArgumentOutOfRangeException(nameof(x), x, "The x-coordinate must not be off the kaboomField.");
            if (y < 0 || y >= kaboomField.Height)
                throw new ArgumentOutOfRangeException(nameof(y), y, "The y-coordinate must not be off the kaboomField.");

            Initialize(kaboomField, x, y);
            if (cellToOpen.IsMine) return;

            SetObviouslyFreeAndMinedCells();

            // set states of closed border cells
            undefinedBorderCells = closedBorderCells.Where(cell => cell.State == KaboomState.None).ToList();
            undefinedBorderCells.ForEach(cell => cell.State = KaboomState.Indeterminate);

            // determine border mine count range
            var hiddenCells = kaboomField.Cells.Where<Cell<KaboomState>>(cell => !cell.IsOpen).Except(closedBorderCells).ToList();
            int knownMinesCount = kaboomField.Cells.Count<Cell<KaboomState>>(cell => cell.IsMine);
            minimumMines = Math.Max(0, kaboomField.NumberOfMines - knownMinesCount - hiddenCells.Count);
            maximumMines = Math.Min(undefinedBorderCells.Count, kaboomField.NumberOfMines - knownMinesCount);

            // Create variable IDs for sat solver
            boolIDsToCell = undefinedBorderCells.Select((cell, i) => (cell, i)).ToDictionary(entry => entry.i, entry => entry.cell);
            cellsToBoolID = boolIDsToCell.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

            CreateConstraints();

            if (constraints.Count == 0)
            {
                cellToOpen.AdjacentMines = cellToOpen.Neighbours.Count(neighbour => neighbour.IsMine);
                return;
            }

            DetermineSolutions();
            SetSolutionToField();
        }
        void Initialize(KaboomField f, int x, int y)
        {
            field = f;
            cellToOpen = field.Cells[x, y];

            var freeCells = field.Cells.Where<Cell<KaboomState>>(cell => cell.State == KaboomState.Free).ToList();
            cellToOpen.State = cellToOpen.State == KaboomState.Mine ||
                               freeCells.Count > 0 && !freeCells.Contains(cellToOpen)
                                   ? KaboomState.Mine
                                   : KaboomState.None;
            cellToOpen.IsMine = cellToOpen.State == KaboomState.Mine;
            cellToOpen.IsOpen = true;

            if (cellToOpen.IsMine) return;

            openBorderCells = field.Cells.Where<Cell<KaboomState>>(cell => cell.IsOpen && cell.Neighbours.Any(neighbour => !neighbour.IsOpen))
                                   .ToList();
            closedBorderCells = field.Cells.Where<Cell<KaboomState>>(cell => !cell.IsOpen && cell.Neighbours.Any(neighbour => neighbour.IsOpen))
                                     .ToList();
            openToClosedBorderCells = openBorderCells.ToDictionary(
                cell => cell,
                cell => cell.Neighbours.Cast<Cell<KaboomState>>().Where(neighbour => !neighbour.IsOpen).ToList());

            closedBorderCellsExclusiveToOpenedCell = cellToOpen.Neighbours.Cast<Cell<KaboomState>>()
                                                               .Where(neighbour => !neighbour.IsOpen &&
                                                                                   neighbour.Neighbours.All(n => !n.IsOpen || n == cellToOpen))
                                                               .ToList();

            foreach (var cell in field.Cells)
            {
                if (cell.IsOpen || cell.State == KaboomState.Indeterminate)
                    cell.State = KaboomState.None;
                cell.IsMine = cell.State == KaboomState.Mine;
            }
        }
        void SetObviouslyFreeAndMinedCells()
        {
            // Set obvious mine fields (number of adjacent mines equals number of adjacent closed cells)
            foreach (Cell<KaboomState> mineNeighbour in openToClosedBorderCells.Where(kvp =>
                                                                                          kvp.Key != cellToOpen &&
                                                                                          kvp.Key.AdjacentMines == kvp.Value.Count)
                                                                               .SelectMany(kvp => kvp.Value))
            {
                mineNeighbour.State = KaboomState.Mine;
                mineNeighbour.IsMine = true;
            }

            // Now set "obviously" free cells (number of adjacent mines from above fulfills a neighbouring open cell's adjacent mine count.
            foreach (Cell<KaboomState> closedCell in from closedCell in closedBorderCells
                                                     where closedCell.State != KaboomState.Mine
                                                     let openNeighbours =
                                                         closedCell.Neighbours.Where(cell => cell.IsOpen && cell != cellToOpen).ToList()
                                                     where openNeighbours.Any(openNeighbour =>
                                                                                  openNeighbour.AdjacentMines ==
                                                                                  openNeighbour.Neighbours.Count(
                                                                                      n => n != closedCell && !n.IsOpen && n.IsMine))
                                                     select closedCell)
                closedCell.State = KaboomState.Free;
        }
        void CreateConstraints()
        {
            constraints = new List<Literal[]>();
            foreach (var cell in openBorderCells.Where(cell => cell != cellToOpen))
            {
                var neighbours = openToClosedBorderCells[cell];
                // use all neighbours in case of having just opened a mine
                int knownMines = cell.Neighbours.Cast<Cell<KaboomState>>().Count(neighbour => neighbour.State == KaboomState.Mine);
                int needed = cell.AdjacentMines - knownMines;

                var undefinedNeighbours = neighbours.Where(neighbour => neighbour.State == KaboomState.Indeterminate).ToList();
                if (undefinedNeighbours.Count == 0) continue;

                constraints.AddRange(constraintsGenerator.GenerateConstraints(undefinedNeighbours.Count, needed,
                                                                              undefinedNeighbours.Select(n => cellsToBoolID[n]).ToArray()));
            }

            if (closedBorderCellsExclusiveToOpenedCell.Count > 0)
            {
                var constraint = closedBorderCellsExclusiveToOpenedCell
                                 .SelectMany(cell => new[] { new Literal(cellsToBoolID[cell], false), new Literal(cellsToBoolID[cell], true) })
                                 .ToArray();
                if (constraint.Length > 0)
                    constraints.Add(constraint);
            }
        }
        void DetermineSolutions()
        {
            var solutions = satSolver.Solve(constraints, cellsToBoolID.Count, minimumMines, maximumMines);
            chosenSolution = solutions[random.Next(solutions.Count)];

            int adjacentMinesToOpenedCell = OpenedCellMineCount(chosenSolution);

            literalsByCell = solutions.Where(solution => OpenedCellMineCount(solution) == adjacentMinesToOpenedCell)
                                      .SelectMany(solution => solution.Literals)
                                      .GroupBy(literal => literal.Var)
                                      .ToDictionary(g => boolIDsToCell[g.Key],
                                                    g => g.Select(literal => literal.Sense).ToList());

            int OpenedCellMineCount(SatSolution solution) =>
                cellToOpen.Neighbours.Cast<Cell<KaboomState>>()
                          .Count(neighbour => !neighbour.IsOpen && (neighbour.State == KaboomState.Mine ||
                                                                    cellsToBoolID.TryGetValue(neighbour, out var id) && 
                                                                    solution.Literals.FirstOrDefault(literal => id == literal.Var).Sense));

        }
        void SetSolutionToField()
        {
            // check if the solutions define some cells
            foreach (var cell in undefinedBorderCells)
            {
                if (!literalsByCell.TryGetValue(cell, out var literals)) continue;
                if (literals.All(l => l))
                {
                    cell.State = KaboomState.Mine;
                    cell.IsMine = true;
                }
                else if (literals.All(l => !l))
                    cell.State = KaboomState.Free;
            }

            // determine new adjacent mines value for opened cell
            cellToOpen.AdjacentMines = cellToOpen.Neighbours.Cast<Cell<KaboomState>>()
                                                 .Count(neighbour => neighbour.IsMine || chosenSolution
                                                                                         .Literals.FirstOrDefault(
                                                                                             literal => boolIDsToCell[literal.Var] == neighbour)
                                                                                         .Sense);
        }
    }
}
