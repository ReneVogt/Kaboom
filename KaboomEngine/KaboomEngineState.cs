namespace Com.Revo.Games.KaboomEngine {
    /// <summary>
    /// Describes a Kaboom board state.
    /// </summary>
    public enum KaboomEngineState
    {
        /// <summary>
        /// Sweeping is in progress.
        /// </summary>
        Sweeping,
        /// <summary>
        /// The board is solved.
        /// </summary>
        Solved,
        /// <summary>
        /// The board exploded by opening a mine cell.
        /// </summary>
        Exploded
    }
}
