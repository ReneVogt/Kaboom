using JetBrains.Annotations;

namespace Com.Revo.Games.KaboomEngine {
    /// <summary>
    /// Kaboom engine factory interface. A Kaboom engine factory should
    /// provide an engine (<see cref="IKaboomEngine"/>) for a given board configuration.
    /// </summary>
    public interface IKaboomEngineFactory
    {
        /// <summary>
        /// Creates a <see cref="IKaboomEngine"/> for the given board configuration.
        /// </summary>
        /// <param name="width">The width of the Kaboom board.</param>
        /// <param name="height">The height of the Kaboom board.</param>
        /// <param name="numberOfMines">The number of mines hidden on the Kaboom board.</param>
        /// <returns>A freshly initialized Kaboom board with the given settings.</returns>
        [NotNull] IKaboomEngine CreateEngine(int width, int height, int numberOfMines);
    }
}
