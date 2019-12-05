using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.SolverFoundation.Solvers;

namespace Com.Revo.Games.KaboomEngine.Kaboom 
{
    class KaboomFieldSolver : ISolveKaboomField
    {
        readonly Random random = new Random();
        public void Solve([NotNull] KaboomField field, (int x, int y) coordinatesToOpen)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            // Reset internal states
            foreach (var cell in field.Cells)
            {
                if (cell.IsOpen)
                    cell.State = KaboomState.None;
                else if (cell.State == KaboomState.Indeterminate)
                    cell.State = KaboomState.None;
            }

            // Check for knwon free cells to determine if the player just blowed themselfes up
            var cellToOpen = field.Cells[coordinatesToOpen.x, coordinatesToOpen.y];
            var freeCells = field.Cells.Where<Cell<KaboomState>>(cell => !cell.IsOpen && cell.State == KaboomState.Free).ToList();
            cellToOpen.State = freeCells.Count > 0 && !freeCells.Contains(cellToOpen) ? KaboomState.Mine : KaboomState.Free;
            cellToOpen.IsOpen = true;

            // determine border cells
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

            // set states of closed border cells
            foreach (var cell in closedBorderCells.Where(cell => cell.State == KaboomState.None))
                cell.State = KaboomState.Indeterminate;

            // Create a dictionary from open border cells to their closed neighbours
            var openToClosedBorderCells =
                openBorderCells.ToDictionary(openCell => openCell, openCell => openCell.Neighbours.Cast<Cell<KaboomState>>().Where(neighbour => !neighbour.IsOpen).ToList());

            // Set obvious mine fields (number of adjacent mines equals number of adjacent closed cells)
            foreach (Cell<KaboomState> mineNeighbour in openToClosedBorderCells.Where(kvp => kvp.Key != cellToOpen && kvp.Key.AdjacentMines == kvp.Value.Count).SelectMany(kvp => kvp.Value))
                mineNeighbour.State = KaboomState.Mine;

            // Now set "obviously" free cells (number of adjacent mines from above fulfills a neighbouring open cell's adjacent mine count.
            foreach (Cell<KaboomState> closedCell in from closedCell in closedBorderCells
                                                     let openNeighbours = closedCell.Neighbours.Where(cell => cell.IsOpen && cell != cellToOpen).ToList()
                                                     where openNeighbours.Any(openNeighbour =>
                                                                                  openNeighbour.AdjacentMines ==
                                                                                  openNeighbour.Neighbours.Count(n => n != closedCell && !n.IsOpen && n.IsMine))
                                                     select closedCell)
                closedCell.State = KaboomState.Free;

            // Create variable IDs for sat solver
            var undefinedBorderCells = closedBorderCells.Where(cell => cell.State == KaboomState.Indeterminate).ToList();
            var boolIDsToCell = undefinedBorderCells.Select((cell, i) => (cell, i)).ToDictionary(x => x.i, x => x.cell);
            var cellsToBoolID = boolIDsToCell.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

            // create constraints
            List<Literal[]> constraints = new List<Literal[]>();
            foreach(var cell in openBorderCells.Where(cell => cell != cellToOpen))
            {
                var neighbours = cell.Neighbours.Cast<Cell<KaboomState>>().ToArray();
                int knownMines = neighbours.Count(neighbour => neighbour.State == KaboomState.Mine);
                int needed = cell.AdjacentMines - knownMines;
                if (needed == 0) continue;

                var undefinedNeighbours = neighbours.Where(neighbour => neighbour.State == KaboomState.Indeterminate).ToList();
                constraints.AddRange(ConstraintGenerator.GenerateConstraints(undefinedNeighbours.Count, needed, undefinedNeighbours.Select(n => cellsToBoolID[n]).ToArray()));
            }

            if (constraints.Count == 0)
            {
                var undefinedNeighbours = cellToOpen.Neighbours.Cast<Cell<KaboomState>>().Where(neighbour => neighbour.State == KaboomState.Indeterminate).ToList();
                int[] IDs = undefinedNeighbours.Select(n => cellsToBoolID[n]).ToArray();
                int needed = random.Next(undefinedNeighbours.Count + 1);
                constraints.AddRange(ConstraintGenerator.GenerateConstraints(undefinedNeighbours.Count, needed, IDs));
            }

            // determine 
            int hiddenCells = field.Cells.Where<Cell<KaboomState>>(cell => !cell.IsOpen).Except(closedBorderCells).Count();
            int knownMinesCount = field.Cells.Count<Cell<KaboomState>>(cell => cell.State == KaboomState.Mine);
            int minimumMines = field.NumberOfMines - knownMinesCount - hiddenCells;
            int maximumMines = field.NumberOfMines - knownMinesCount;
            if (cellToOpen.State == KaboomState.Mine)
            {
                minimumMines -= 1;
                maximumMines -= 1;
            }

            var solutions = (from solution in SatSolver.Solve(new SatSolverParams(), cellsToBoolID.Count, constraints)
                             let mines = solution.Literals.Count(literal => literal.Sense)
                             where mines >= minimumMines && mines <= maximumMines
                             select solution).ToList();
            var literalsByCell = solutions.SelectMany(solution => solution.Literals)
                                          .GroupBy(literal => literal.Var)
                                          .ToDictionary(g => boolIDsToCell[g.Key], g => g.Select(literal => literal.Sense).ToList());
            foreach (var cell in undefinedBorderCells)
            {
                var literals = literalsByCell[cell];
                if (literals.All(l => l))
                    cell.State = KaboomState.Mine;
                else if (literals.All(l => !l))
                    cell.State = KaboomState.Free;
            }

            foreach (var cell in field.Cells.Where<Cell<KaboomState>>(cell => cell.State == KaboomState.Mine))
                cell.IsMine = true;

            var chosenSolution = solutions[random.Next(solutions.Count)];

            cellToOpen.AdjacentMines = cellToOpen.Neighbours.Cast<Cell<KaboomState>>()
                                                 .Count(cell => cell.State == KaboomState.Mine ||
                                                                cellsToBoolID.TryGetValue(cell, out var id) &&
                                                                chosenSolution.Literals.First(Literal => Literal.Var == id).Sense);
        }
    }
}
