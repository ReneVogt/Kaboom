namespace Com.Revo.Games.KaboomEngine
{
    sealed class KaboomCell : IKaboomCell
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int AdjacentMines { get; set; }
        public bool IsMine { get; set; }
        public bool IsOpen { get; set; }
    }
}