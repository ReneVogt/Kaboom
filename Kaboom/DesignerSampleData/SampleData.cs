using Com.Revo.Games.Kaboom.ViewModels;

namespace Com.Revo.Games.Kaboom.DesignerSampleData
{
    public static class SampleData
    {
        public static MainWindowModel MainWindow { get; }
        public static KaboomBoardModel Board { get; }
        public static KaboomCellModel Cell { get; }
        static SampleData()
        {
            Board = new KaboomBoardModel();
            Cell = Board.Cells[0][0];
            MainWindow = new MainWindowModel { Board = Board };
        }
    }
}
