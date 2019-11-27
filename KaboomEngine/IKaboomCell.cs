namespace Com.Revo.Games.KaboomEngine {
    /// <summary>
    /// Represents a cell of the Kaboom board
    /// </summary>
    public interface IKaboomCell
    {
        /// <summary>
        /// The x-coordinate of this cell.
        /// </summary>
        int X { get; }
        /// <summary>
        /// The y-coordinate of this cell.
        /// </summary>
        int Y { get; }
        /// <summary>
        /// The number of adjacent cells that contain mines.
        /// </summary>
        int AdjacentMines { get; }
        /// <summary>
        /// Gets wether this cell contains a mine or not.
        /// </summary>
        bool IsMine { get; }
        /// <summary>
        /// The state (open or covered) of this cell.
        /// </summary>
        bool IsOpen { get; }
    }
}
