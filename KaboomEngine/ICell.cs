using System;

namespace Com.Revo.Games.KaboomEngine {
    /// <summary>
    /// Represents a cell in a <see cref="ICellCollection"/> of an <see cref="IField"/>
    /// </summary>
    public interface ICell {
        /// <summary>
        /// The x-coordinate of the cell.
        /// </summary>
        int X { get; }
        /// <summary>
        /// The y-coordinate of the cell.
        /// </summary>
        int Y { get; }
        /// <summary>
        /// Number of adjacent cells containing mines.
        /// This property is only valid if the cell has
        /// already been uncovered (hence <see cref="IsOpen"/> is <code>true</code>).
        /// </summary>
        int AdjacentMines { get; }
        /// <summary>
        /// Tells if this cell contains a mine.
        /// This property is only valid if the cell has
        /// already been uncovered (hence <see cref="IsOpen"/> is <code>true</code>)
        /// or the containing minesweeperField's <see cref="IField.State"/> is no longer <see cref="FieldState.Sweeping"/>.
        /// </summary>
        bool IsMine { get; }
        /// <summary>
        /// Tells if this cell has already been uncovered.
        /// </summary>
        bool IsOpen { get; }
        /// <summary>
        /// Gets or sets wether this cell has a flag on it.
        /// This property is only valid as long as the cell is not uncovered.
        /// </summary>
        bool IsFlagged { get; set; }
        /// <summary>
        /// Raised if the cell's state has been changed.
        /// </summary>
        event EventHandler CellChanged;
        /// <summary>
        /// Uncovers this cell.
        /// This call is delegated to the <see cref="IField.Uncover(int,int)"/> method of the
        /// <see cref="IField"/> owning this cell. See there for what may happen if
        /// this method is called.
        /// </summary>
        void Uncover();
    }
}
