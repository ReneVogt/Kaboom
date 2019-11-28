using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Com.Revo.Games.KaboomEngine
{
    /// <summary>
    /// Provides extension methods for the <see cref="IKaboomEngine"/> interface.
    /// </summary>
    public static class KaboomEngineExtensions
    {
        /// <summary>
        /// Enumerates the coordinates of the cells adjacent to the cell with the
        /// specified coordinates.
        /// </summary>
        /// <param name="engine">The engine the cells are in.</param>
        /// <param name="x">The x-coordinate of the current cell.</param>
        /// <param name="y">The y-coordinate of the current cell.</param>
        /// <returns>An enumeration of the coordinates of the cells adjacent to the specified cell.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="engine"/> was <code>null</code>.</exception>
        /// <exception cref="IndexOutOfRangeException">The specified cell is outside the engine.</exception>
        public static IEnumerable<(int x, int y)> GetCellsAdjacentTo([NotNull] this IKaboomEngine engine, int x, int y)
        {
            if (engine == null)
                throw new ArgumentNullException(nameof(engine));
            if (x < 0) throw new IndexOutOfRangeException($"{nameof(x)} must be at least zero!");
            if (x >= engine.Width) throw new IndexOutOfRangeException($"{nameof(x)} must be less than {engine.Width}!");
            if (y < 0) throw new IndexOutOfRangeException($"{nameof(y)} must be at least zero!");
            if (y >= engine.Height) throw new IndexOutOfRangeException($"{nameof(y)} must be less than {engine.Height}!");

            if (x > 0 && y > 0) yield return (x - 1, y - 1);
            if (y > 0) yield return (x, y - 1);
            if (x < engine.Width - 1 && y > 0) yield return (x + 1, y - 1);

            if (x > 0) yield return (x - 1, y);
            if (x < engine.Width - 1) yield return (x + 1, y);

            if (x > 0 && y < engine.Height - 1) yield return (x - 1, y + 1);
            if (y < engine.Height - 1) yield return (x, y + 1);
            if (x < engine.Width - 1 && y < engine.Height - 1) yield return (x + 1, y + 1);
        }
    }
}
