using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Com.Revo.Games.KaboomEngine
{
    /// <summary>
    /// Provides extension methods for the <see cref="IField"/> interface.
    /// </summary>
    public static class FieldExtensions
    {
        /// <summary>
        /// Enumerates the coordinates of the cells adjacent to the cell with the
        /// specified coordinates.
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to work on.</param>
        /// <param name="x">The x-coordinate of the current cell.</param>
        /// <param name="y">The y-coordinate of the current cell.</param>
        /// <returns>An enumeration of the coordinates of the cells adjacent to the specified cell.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="field"/> cannot be <code>null</code>.</exception>
        /// <exception cref="IndexOutOfRangeException">The specified cell is outside the field.</exception>
        public static IEnumerable<(int x, int y)> GetCoordinatesAdjacentTo([NotNull] this IField field, int x, int y)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

#pragma warning disable CA1303 // Do not pass literals as localized parameters
            if (x < 0) throw new IndexOutOfRangeException($"{nameof(x)} must be at least zero!");
            if (x >= field.Width) throw new IndexOutOfRangeException($"{nameof(x)} must be less than {field.Width}!");
            if (y < 0) throw new IndexOutOfRangeException($"{nameof(y)} must be at least zero!");
            if (y >= field.Height) throw new IndexOutOfRangeException($"{nameof(y)} must be less than {field.Height}!");
#pragma warning restore CA1303 // Do not pass literals as localized parameters

            return Iterator();

            IEnumerable<(int x, int y)> Iterator()
            {
                if (x > 0 && y > 0) yield return (x - 1, y - 1);
                if (y > 0) yield return (x, y - 1);
                if (x < field.Width - 1 && y > 0) yield return (x + 1, y - 1);

                if (x > 0) yield return (x - 1, y);
                if (x < field.Width - 1) yield return (x + 1, y);

                if (x > 0 && y < field.Height - 1) yield return (x - 1, y + 1);
                if (y < field.Height - 1) yield return (x, y + 1);
                if (x < field.Width - 1 && y < field.Height - 1) yield return (x + 1, y + 1);
            }        }
    }
}
