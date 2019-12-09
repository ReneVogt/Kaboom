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

        public KaboomFieldSolver([NotNull] IGenerateConstraints constraintsGenerator, [NotNull] IProvideRandom random)
        {
            this.constraintsGenerator = constraintsGenerator ?? throw new ArgumentNullException(nameof(constraintsGenerator));
            this.random = random ?? throw new ArgumentNullException(nameof(random));
        }
        public void Solve([NotNull] KaboomField field, int x, int y)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));
            if (x < 0 || x >= field.Width)
                throw new ArgumentOutOfRangeException(nameof(x), x, "The x-coordinate must not be off the field.");
            if (y < 0 || y >= field.Height)
                throw new ArgumentOutOfRangeException(nameof(y), y, "The y-coordinate must not be off the field.");

            ResetInternalCellStates(field);
            var cellToOpen = OpenCell(field, x, y);
            var (closedBorderCells, openBorderCells, openToClosedBorderCells) = DetermineBorder(field);
            var closedBorderCellsExclusiveToOpenedCell = openToClosedBorderCells[cellToOpen]
                                                         .Where(neighbour => neighbour.Neighbours.All(n => !n.IsOpen || n == cellToOpen))
                                                         .ToList();

            SetObviousMines(openToClosedBorderCells, cellToOpen, closedBorderCells);

            // set states of closed border cells
            var undefinedBorderCells = closedBorderCells.Where(cell => cell.State == KaboomState.None).ToList();
            undefinedBorderCells.ForEach(cell => cell.State = KaboomState.Indeterminate);

            // determine border mine count range
            var hiddenCells = field.Cells.Where<Cell<KaboomState>>(cell => !cell.IsOpen).Except(closedBorderCells).ToList();
            int knownMinesCount = field.Cells.Count<Cell<KaboomState>>(cell => cell.IsMine);
            int minimumMines = Math.Max(0, field.NumberOfMines - knownMinesCount - hiddenCells.Count);
            int maximumMines = Math.Min(undefinedBorderCells.Count, field.NumberOfMines - knownMinesCount);

            // Create variable IDs for sat solver
            var boolIDsToCell = undefinedBorderCells.Select((cell, i) => (cell, i)).ToDictionary(entry => entry.i, entry => entry.cell);
            var cellsToBoolID = boolIDsToCell.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

            // create constraints
            List<Literal[]> constraints = new List<Literal[]>();
            foreach(var cell in openBorderCells.Where(cell => cell != cellToOpen))
            {
                var neighbours = openToClosedBorderCells[cell];
                int knownMines = neighbours.Count(neighbour => neighbour.State == KaboomState.Mine);
                int needed = cell.AdjacentMines - knownMines;

                var undefinedNeighbours = neighbours.Where(neighbour => neighbour.State == KaboomState.Indeterminate).ToList();
                if (undefinedNeighbours.Count == 0) continue;

                constraints.AddRange(constraintsGenerator.GenerateConstraints(undefinedNeighbours.Count, needed, undefinedNeighbours.Select(n => cellsToBoolID[n]).ToArray()));
            }

            if (closedBorderCellsExclusiveToOpenedCell.Count > 0)
            {
                var constraint = closedBorderCellsExclusiveToOpenedCell
                                 .SelectMany(cell => new[] {new Literal(cellsToBoolID[cell], false), new Literal(cellsToBoolID[cell], true)})
                                 .ToArray();
                if (constraint.Length > 0)
                    constraints.Add(constraint);
            }

            if (constraints.Count == 0)
            {
                cellToOpen.AdjacentMines = cellToOpen.Neighbours.Count(neighbour => neighbour.IsMine);
                return;
            }

            // get the solutions
            var solutions = (from solution in SatSolver.Solve(new SatSolverParams(), cellsToBoolID.Count, constraints)
                             let mines = solution.Literals.Count(literal => literal.Sense)
                             where mines >= minimumMines && mines <= maximumMines
                             select solution).ToList();

            var solutionsByOpenedCellValue = solutions.GroupBy(solution =>
                                                                   cellToOpen.Neighbours.Cast<Cell<KaboomState>>()
                                                                             .Count(neighbour => neighbour.State == KaboomState.Indeterminate &&
                                                                                                 solution.Literals.First(
                                                                                                             literal =>
                                                                                                                 literal.Var == cellsToBoolID[
                                                                                                                     neighbour])
                                                                                                         .Sense))
                                                      .ToDictionary(g => g.Key, g => g.ToList());

            Dictionary<Cell<KaboomState>, List<bool>> literalsByCell;
            SatSolution chosenSolution;
            if (solutionsByOpenedCellValue.Count > 0)
            {
                var indices = solutionsByOpenedCellValue.Keys.ToList();
                int chosenSolutionIndex = indices[random.Next(indices.Count)];
                var chosenSolutions = solutionsByOpenedCellValue[chosenSolutionIndex];

                literalsByCell = chosenSolutions.SelectMany(solution => solution.Literals)
                                                .GroupBy(literal => literal.Var)
                                                .ToDictionary(g => boolIDsToCell[g.Key], g => g.Select(literal => literal.Sense).ToList());
                chosenSolution = chosenSolutions[random.Next(chosenSolutions.Count)];
            }
            else
            {
                literalsByCell = solutions.SelectMany(solution => solution.Literals)
                                                .GroupBy(literal => literal.Var)
                                                .ToDictionary(g => boolIDsToCell[g.Key], g => g.Select(literal => literal.Sense).ToList());
                 chosenSolution = solutions[random.Next(solutions.Count)];
            }

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

            // Set solution values
            foreach (var literal in chosenSolution.Literals.Where(literal => literal.Sense))
            {
                if (boolIDsToCell.TryGetValue(literal.Var, out var cell))
                    cell.IsMine = true;
            }

            // Scatter rest of mines randomly
            var possibleMineCoordinates = hiddenCells.Select(cell => (cell.X, cell.Y)).ToList();
            for (int i = field.Cells.Count<Cell<KaboomState>>(cell => cell.IsMine); i<field.NumberOfMines; i++)
            {
                int index = random.Next(possibleMineCoordinates.Count);
                (int row, int column) = possibleMineCoordinates[index];
                field.Cells[row, column].IsMine = true;
                possibleMineCoordinates.RemoveAt(index);
            }

            cellToOpen.AdjacentMines = cellToOpen.Neighbours.Count(neighbour => neighbour.IsMine);
        }
        static void ResetInternalCellStates(KaboomField field)
        {
            foreach (var cell in field.Cells)
            {
                if (cell.IsOpen || cell.State == KaboomState.Indeterminate)
                    cell.State = KaboomState.None;
                cell.IsMine = cell.State == KaboomState.Mine;
            }
        }
        static Cell<KaboomState> OpenCell(KaboomField field, int x, int y)
        {
            var cellToOpen = field.Cells[x, y];

            // check if guessing was forced
            var freeCells = field.Cells.Where<Cell<KaboomState>>(cell => cell.State == KaboomState.Free).ToList();
            cellToOpen.State = cellToOpen.State == KaboomState.Mine ||
                               freeCells.Count > 0 && !freeCells.Contains(cellToOpen)
                                   ? KaboomState.Mine
                                   : KaboomState.None;
            cellToOpen.IsMine = cellToOpen.State == KaboomState.Mine;
            cellToOpen.IsOpen = true;
            return cellToOpen;
        }
        (List<Cell<KaboomState>> closedBorderCells, List<Cell<KaboomState>> openBorderCells, Dictionary<Cell<KaboomState>, List<Cell<KaboomState>>>
            openToClosedBorderCells)
            DetermineBorder(KaboomField field)
        {
            var closedBorderCells = field.Cells
                                         .Where<Cell<KaboomState>>(
                                             cell => !cell.IsOpen && cell.Neighbours.Any(neighbour => neighbour.IsOpen))
                                         .Distinct()
                                         .ToList();

            var openBorderCells = field.Cells
                                       .Where<Cell<KaboomState>>(cell => cell.IsOpen &&
                                                                         cell.Neighbours.Any(neighbour => !neighbour.IsOpen))
                                       .Distinct()
                                       .ToList();

            var openToClosedBorderCells =
                openBorderCells.ToDictionary(openCell => openCell,
                                             openCell => openCell.Neighbours.Cast<Cell<KaboomState>>()
                                                                 .Where(neighbour => !neighbour.IsOpen).ToList());

            return (closedBorderCells, openBorderCells, openToClosedBorderCells);
        }
        static void SetObviousMines(Dictionary<Cell<KaboomState>, List<Cell<KaboomState>>> openToClosedBorderCells, Cell<KaboomState> cellToOpen, List<Cell<KaboomState>> closedBorderCells)
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
    }
}
