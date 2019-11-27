namespace Com.Revo.Games.KaboomEngine
{
    /// <inheritdoc />
    public sealed class KaboomEngineFactory : IKaboomEngineFactory
    {
        /// <inheritdoc />
        public IKaboomEngine CreateEngine(int width, int height, int numberOfMines) => new KaboomEngine(width, height, numberOfMines);
    }
}
