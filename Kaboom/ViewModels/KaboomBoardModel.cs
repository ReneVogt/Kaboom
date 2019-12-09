using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Com.Revo.Games.KaboomEngine;
using JetBrains.Annotations;

namespace Com.Revo.Games.Kaboom.ViewModels {
    public sealed class KaboomBoardModel : INotifyPropertyChanged
    {
        readonly IField field;
        bool debugMode;

        public bool DebugMode
        {
            get => debugMode;
            set
            {
                if (value == debugMode)
                    return;
                debugMode = value;
                OnPropertyChanged();
            }
        }
        public string State =>
            field.State switch {
                FieldState.Exploded => "You lost!",
                FieldState.Solved => "You win!",
                _ => $"{field.NumberOfMines - Cells.SelectMany(row => row).Count(cell => cell.State == KaboomCellState.Flagged):D3}"};
        public Brush StateColor =>
            field.State == FieldState.Exploded ? Brushes.Red : Brushes.Green;
        public List<List<KaboomCellModel>> Cells { get; }

        public KaboomBoardModel()
            : this(9, 9, 10) { }
        public KaboomBoardModel(int width, int height, int numberOfMines)
        {
            field = FieldProvider.CreateKaboomField(width, height, numberOfMines);
            field.StateChanged += (sender, e) =>
            {
                OnPropertyChanged(nameof(State));
                OnPropertyChanged(nameof(StateColor));
            };

            Cells = Enumerable.Range(0, field.Height)
                              .Select(y => Enumerable.Range(0, field.Width)
                                                     .Select(x => CreateCellModel(x, y))
                                                     .ToList())
                              .ToList();

            KaboomCellModel CreateCellModel(int x, int y)
            {
                var cell = field.Cells[x, y];
                var model = new KaboomCellModel(cell, this);
                cell.CellChanged += (sender, e) =>
                {
                    OnPropertyChanged(nameof(State));
                    OnPropertyChanged(nameof(StateColor));
                };
                return model;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
    }
}
