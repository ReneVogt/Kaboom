using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Com.Revo.Games.KaboomEngine;
using JetBrains.Annotations;

namespace Com.Revo.Games.Kaboom.ViewModels {
    public sealed class KaboomBoardModel : INotifyPropertyChanged
    {
        readonly Field field;
        
        public string State =>
            field.State switch {
                FieldState.Exploded => "You lost!",
                FieldState.Solved => "You win!",
                _ => $"Mines: {Cells.SelectMany(row => row).Count(cell => cell.State == KaboomCellState.Flagged)}/{field.NumberOfMines}"};
        public ObservableCollection<ObservableCollection<KaboomCellModel>> Cells { get; }

        public KaboomBoardModel()
            : this(9, 9, 10) { }
        public KaboomBoardModel(int width, int height, int numberOfMines)
        {
            field = new Field(width, height, numberOfMines);
            field.StateChanged += (sender, e) => OnPropertyChanged(nameof(State));
            var cells = new ObservableCollection<ObservableCollection<KaboomCellModel>>();
            for (int y = 0; y < field.Height; y++)
            {
                ObservableCollection<KaboomCellModel> row = new ObservableCollection<KaboomCellModel>();
                for (int x = 0; x < field.Width; x++)
                    row.Add(new KaboomCellModel(field.Cells[x, y]));
                cells.Add(row);
            }

            Cells = cells;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
    }
}
