using System.Collections.ObjectModel;
using Com.Revo.Games.Kaboom.ViewModels;

namespace Com.Revo.Games.Kaboom.DesignerSampleData
{
    public static class SampleData
    {
        public static MainWindowModel MainWindow{ get; }
        public static KaboomBoardModel Board { get; }
        public static KaboomCellModel CellClosed { get; } = new KaboomCellModel {State = KaboomCellState.Closed};
        public static KaboomCellModel CellEmpty { get; } = new KaboomCellModel {State = KaboomCellState.Empty};
        public static KaboomCellModel CellOne { get; } = new KaboomCellModel { State = KaboomCellState.Neighbours1};
        public static KaboomCellModel CellTwo { get; } = new KaboomCellModel { State = KaboomCellState.Neighbours2 };
        public static KaboomCellModel CellThree { get; } = new KaboomCellModel { State = KaboomCellState.Neighbours3 };
        public static KaboomCellModel CellFour { get; } = new KaboomCellModel { State = KaboomCellState.Neighbours4 };
        public static KaboomCellModel CellFive { get; } = new KaboomCellModel { State = KaboomCellState.Neighbours5 };
        public static KaboomCellModel CellSix { get; } = new KaboomCellModel { State = KaboomCellState.Neighbours6 };
        public static KaboomCellModel CellSeven { get; } = new KaboomCellModel { State = KaboomCellState.Neighbours7 };
        public static KaboomCellModel CellEight { get; } = new KaboomCellModel { State = KaboomCellState.Neighbours8 };
        public static KaboomCellModel CellBomb { get; } = new KaboomCellModel { State = KaboomCellState.Mine };
        public static KaboomCellModel CellFlagged{ get; } = new KaboomCellModel { State = KaboomCellState.Flagged };

        static SampleData()
        {
            Board = new KaboomBoardModel
            {
                Cells =
                {
                    new ObservableCollection<KaboomCellModel>
                    {
                        new KaboomCellModel {State = KaboomCellState.Closed},
                        new KaboomCellModel {State = KaboomCellState.Closed},
                        new KaboomCellModel {State = KaboomCellState.Closed},
                        new KaboomCellModel {State = KaboomCellState.Closed},
                        new KaboomCellModel {State = KaboomCellState.Closed},
                        new KaboomCellModel {State = KaboomCellState.Closed},
                        new KaboomCellModel {State = KaboomCellState.Closed},
                        new KaboomCellModel {State = KaboomCellState.Closed},
                        new KaboomCellModel {State = KaboomCellState.Closed},
                        new KaboomCellModel {State = KaboomCellState.Closed}
                    },
                    new ObservableCollection<KaboomCellModel>
                    {
                        new KaboomCellModel {State = KaboomCellState.Empty},
                        new KaboomCellModel {State = KaboomCellState.Empty},
                        new KaboomCellModel {State = KaboomCellState.Empty},
                        new KaboomCellModel {State = KaboomCellState.Empty},
                        new KaboomCellModel {State = KaboomCellState.Empty},
                        new KaboomCellModel {State = KaboomCellState.Empty},
                        new KaboomCellModel {State = KaboomCellState.Empty},
                        new KaboomCellModel {State = KaboomCellState.Empty},
                        new KaboomCellModel {State = KaboomCellState.Empty},
                        new KaboomCellModel {State = KaboomCellState.Empty}
                    },
                    new ObservableCollection<KaboomCellModel>
                    {
                        new KaboomCellModel {State = KaboomCellState.Neighbours1},
                        new KaboomCellModel {State = KaboomCellState.Neighbours2},
                        new KaboomCellModel {State = KaboomCellState.Neighbours3},
                        new KaboomCellModel {State = KaboomCellState.Neighbours4},
                        new KaboomCellModel {State = KaboomCellState.Neighbours5},
                        new KaboomCellModel {State = KaboomCellState.Neighbours6},
                        new KaboomCellModel {State = KaboomCellState.Neighbours7},
                        new KaboomCellModel {State = KaboomCellState.Neighbours8},
                        new KaboomCellModel {State = KaboomCellState.Mine},
                        new KaboomCellModel {State = KaboomCellState.Flagged}
                    },
                    new ObservableCollection<KaboomCellModel>
                    {
                        new KaboomCellModel {State = KaboomCellState.Neighbours1},
                        new KaboomCellModel {State = KaboomCellState.Neighbours2},
                        new KaboomCellModel {State = KaboomCellState.Neighbours3},
                        new KaboomCellModel {State = KaboomCellState.Neighbours4},
                        new KaboomCellModel {State = KaboomCellState.Neighbours5},
                        new KaboomCellModel {State = KaboomCellState.Neighbours6},
                        new KaboomCellModel {State = KaboomCellState.Neighbours7},
                        new KaboomCellModel {State = KaboomCellState.Neighbours8},
                        new KaboomCellModel {State = KaboomCellState.Mine},
                        new KaboomCellModel {State = KaboomCellState.Flagged}
                    },
                    new ObservableCollection<KaboomCellModel>
                    {
                        new KaboomCellModel {State = KaboomCellState.Neighbours1},
                        new KaboomCellModel {State = KaboomCellState.Neighbours2},
                        new KaboomCellModel {State = KaboomCellState.Neighbours3},
                        new KaboomCellModel {State = KaboomCellState.Neighbours4},
                        new KaboomCellModel {State = KaboomCellState.Neighbours5},
                        new KaboomCellModel {State = KaboomCellState.Neighbours6},
                        new KaboomCellModel {State = KaboomCellState.Neighbours7},
                        new KaboomCellModel {State = KaboomCellState.Neighbours8},
                        new KaboomCellModel {State = KaboomCellState.Mine},
                        new KaboomCellModel {State = KaboomCellState.Flagged}
                    },
                    new ObservableCollection<KaboomCellModel>
                    {
                        new KaboomCellModel {State = KaboomCellState.Neighbours1},
                        new KaboomCellModel {State = KaboomCellState.Neighbours2},
                        new KaboomCellModel {State = KaboomCellState.Neighbours3},
                        new KaboomCellModel {State = KaboomCellState.Neighbours4},
                        new KaboomCellModel {State = KaboomCellState.Neighbours5},
                        new KaboomCellModel {State = KaboomCellState.Neighbours6},
                        new KaboomCellModel {State = KaboomCellState.Neighbours7},
                        new KaboomCellModel {State = KaboomCellState.Neighbours8},
                        new KaboomCellModel {State = KaboomCellState.Mine},
                        new KaboomCellModel {State = KaboomCellState.Flagged}
                    },
                    new ObservableCollection<KaboomCellModel>
                    {
                        new KaboomCellModel {State = KaboomCellState.Closed},
                        new KaboomCellModel {State = KaboomCellState.Closed},
                        new KaboomCellModel {State = KaboomCellState.Closed},
                        new KaboomCellModel {State = KaboomCellState.Closed},
                        new KaboomCellModel {State = KaboomCellState.Closed},
                        new KaboomCellModel {State = KaboomCellState.Closed},
                        new KaboomCellModel {State = KaboomCellState.Closed},
                        new KaboomCellModel {State = KaboomCellState.Closed},
                        new KaboomCellModel {State = KaboomCellState.Closed},
                        new KaboomCellModel {State = KaboomCellState.Closed}
                    }
                }
            };
            MainWindow = new MainWindowModel {Board = Board};
        }
}
}
