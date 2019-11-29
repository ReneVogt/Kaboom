using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Com.Revo.Games.Kaboom.ViewModels.Com.Aki.WpfCommons.Bindings;
using Com.Revo.Games.KaboomEngine;
using JetBrains.Annotations;

namespace Com.Revo.Games.Kaboom.ViewModels {
    public sealed class KaboomBoardModel : INotifyPropertyChanged
    {
        readonly IKaboomEngine engine;
        
        public string EngineState =>
            engine.State switch {
                KaboomEngineState.Exploded => "You lost!",
                KaboomEngineState.Solved => "You win!",
                _ => $"Mines: {Cells.SelectMany(row => row).Count(cell => cell.State == KaboomCellState.Flagged)}/{engine.NumberOfMines}"};
        public ObservableCollection<ObservableCollection<KaboomCellModel>> Cells { get; }

        public CustomCommand<KaboomCellClickEventArgs> CellClickedCommand { get; }

        public KaboomBoardModel()
            : this(9, 9, 10) { }
        public KaboomBoardModel(int width, int height, int numberOfMines)
        {
            engine = new KaboomEngineFactory().CreateEngine(width, height, numberOfMines);
            var cells = new ObservableCollection<ObservableCollection<KaboomCellModel>>();
            for (int y = 0; y < engine.Height; y++)
            {
                ObservableCollection<KaboomCellModel> row = new ObservableCollection<KaboomCellModel>();
                for (int x = 0; x < engine.Width; x++)
                    row.Add(new KaboomCellModel(engine.Cells[x, y]));
                cells.Add(row);
            }

            Cells = cells;
            
            CellClickedCommand = new CustomCommand<KaboomCellClickEventArgs>(OnCellClicked);
        }

        private void OnCellClicked(KaboomCellClickEventArgs e)
        {
            if (engine.State != KaboomEngineState.Sweeping) return;

            switch(e?.ClickedCell?.State)
            {
                case KaboomCellState.Empty:
                case KaboomCellState.Mine: 
                    return;
                case KaboomCellState.Flagged:
                    if (e.ClickType != KaboomCellClickType.Right) return;
                    e.ClickedCell.State = KaboomCellState.Closed;
                    break;
                case KaboomCellState.Closed:
                    if (e.ClickType == KaboomCellClickType.Right)
                        e.ClickedCell.State = KaboomCellState.Flagged;
                    else
                        engine.Open(e.ClickedCell.X, e.ClickedCell.Y);
                    break;
                default:
                    if (e?.ClickType != KaboomCellClickType.Double) return;
                    OpenWherePossible(e.ClickedCell);
                    break;
            }

            UpdateCellStates();
        }
        private void OpenWherePossible(KaboomCellModel cell)
        {
            var adjacentCells = engine.GetCellsAdjacentTo(cell.X, cell.Y).Select(c => Cells[c.y][c.x]).ToArray();
            if (adjacentCells.Count(c => c.State == KaboomCellState.Flagged) != cell.AdjacentMines) return;

            foreach (var c in adjacentCells.Where(c => c.State == KaboomCellState.Closed))
                engine.Open(c.X, c.Y);
        }
        private void UpdateCellStates()
        {
            for (int y = 0; y < engine.Height; y++)
            for (int x = 0; x < engine.Width; x++)
            {
                var engineCell = engine.Cells[x, y];
                var cellModel = Cells[y][x];

                if (!engineCell.IsOpen)
                {
                    if (engine.State == KaboomEngineState.Sweeping || cellModel.State == KaboomCellState.Flagged || !engineCell.IsMine) continue;
                    cellModel.State = engine.State == KaboomEngineState.Exploded ? KaboomCellState.Mine : KaboomCellState.Flagged;
                    continue;
                }

                if (engineCell.IsMine)
                {
                    cellModel.State = KaboomCellState.Mine;
                    continue;
                }

                cellModel.State = cellModel.AdjacentMines switch
                {
                    1 => KaboomCellState.Neighbours1,
                    2 => KaboomCellState.Neighbours2,
                    3 => KaboomCellState.Neighbours3,
                    4 => KaboomCellState.Neighbours4,
                    5 => KaboomCellState.Neighbours5,
                    6 => KaboomCellState.Neighbours6,
                    7 => KaboomCellState.Neighbours7,
                    8 => KaboomCellState.Neighbours8,
                    _ => KaboomCellState.Empty
                };
            }
            OnPropertyChanged(nameof(EngineState));
        }
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
    }
}
