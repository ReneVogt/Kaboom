namespace Com.Revo.Games.KaboomEngine 
{
    /// <summary>
    /// Represents the state of a Kaboom field (<see cref="IKaboomField"/>).
    /// </summary>
    public enum FieldState
    {
        /// <summary>
        /// Sweeping is in progress.
        /// </summary>
        Sweeping,
        /// <summary>
        /// The field has been solved.
        /// </summary>
        Solved,
        /// <summary>
        /// The field is exploded.
        /// </summary>
        Exploded
    }
}
