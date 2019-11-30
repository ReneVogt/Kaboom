using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.Revo.Games.KaboomEngine 
{
    /// <summary>
    /// Represents a Kaboom mine field.
    /// </summary>
    public sealed class Field
    {
        FieldState state = FieldState.Sweeping;
        static readonly Random random = new Random();

        /// <summary>
        /// The width of this Kaboom field.
        /// </summary>
        public int Width { get; }
        /// <summary>
        /// The height of this Kaboom field.
        /// </summary>
        public int Height { get; }
        /// <summary>
        /// Number of mines on this Kaboom field.
        /// </summary>
        public int NumberOfMines { get; }
        /// <summary>
        /// The cells of this Kaboom field.
        /// </summary>
        public CellCollection Cells { get; }
        /// <summary>
        /// The state of this Kaboom field.
        /// </summary>
        public FieldState State
        {
            get => state;
            set
            {
                if (state == value) return;
                state = value;
                StateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raised if the state of this field has changed.
        /// </summary>
        public event EventHandler StateChanged;

        /// <summary>
        /// Creates a new Kaboom field of the specified size
        /// and containing the specified number of mines.
        /// </summary>
        /// <param name="width">The width of the field to create.</param>
        /// <param name="height">The height of the field to create.</param>
        /// <param name="numberOfMines">The number of mines the field should contain.</param>
        public Field(int width, int height, int numberOfMines)
        {
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
                cell.AdjacentMines = GetCoordinatesAdjacentTo(cell.X, cell.Y).Count(c => Cells[c.x, c.y].IsMine);

        }
        /// <summary>
        /// Uncovers cells depending on the state of the cell at the specified location.
        /// If the cell is closed and flagged, nothing will happen.
        /// If the cell is closed and not flagged, it will be uncovered.
        /// If the cell contains a mine, you are dead.
        /// If it does not contain a mine and none of the adjacent cells contains mine,
        /// the adjacent cells will be uncovered, too.
        /// If this cell has already been uncovered, all(!) flag-satisfied cells will be opened.
        /// That is, for all cells that are opened and have an adjacent mine count equal to the
        /// number of adjacent flagged cells, the not-flagged adjacent cells will be uncovered.
        /// </summary>
        /// <param name="x">The x-coordinate of the cell to uncover.</param>
        /// <param name="y">The y-coordinate of the cell to uncover.</param>
        /// <exception cref="IndexOutOfRangeException">The <paramref name="x"/>- and/or <paramref name="y"/>-coordinate was outside the bounds of this field.</exception>
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
                                                      closedNeighbours = GetCoordinatesAdjacentTo(cell.X, cell.Y)
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

        /// <summary>
        /// Enumerates the coordinates of the cells adjacent to the cell with the
        /// specified coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate of the current cell.</param>
        /// <param name="y">The y-coordinate of the current cell.</param>
        /// <returns>An enumeration of the coordinates of the cells adjacent to the specified cell.</returns>
        /// <exception cref="IndexOutOfRangeException">The specified cell is outside the engine.</exception>
        public IEnumerable<(int x, int y)> GetCoordinatesAdjacentTo(int x, int y)
        {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
            if (x < 0) throw new IndexOutOfRangeException($"{nameof(x)} must be at least zero!");
            if (x >= Width) throw new IndexOutOfRangeException($"{nameof(x)} must be less than {Width}!");
            if (y < 0) throw new IndexOutOfRangeException($"{nameof(y)} must be at least zero!");
            if (y >= Height) throw new IndexOutOfRangeException($"{nameof(y)} must be less than {Height}!");
#pragma warning restore CA1303 // Do not pass literals as localized parameters

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
