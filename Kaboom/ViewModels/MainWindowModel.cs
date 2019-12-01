using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Markup;
using Com.Revo.Games.Kaboom.ViewModels.Com.Aki.WpfCommons.Bindings;
using Com.Revo.Games.Kaboom.Views;
using JetBrains.Annotations;

namespace Com.Revo.Games.Kaboom.ViewModels
{
    public sealed class MainWindowModel : MarkupExtension, INotifyPropertyChanged
    {
        KaboomBoardModel board;
        public bool UseMinesweeper { get; set; } = true;
        public bool UseKaboom { get; set; }
        public CustomCommand BeginnerCommand { get; }
        public CustomCommand AdvancedCommand { get; }
        public CustomCommand ExpertCommand { get; }
        public CustomCommand UserDefinedCommand { get; }
        public KaboomBoardModel Board
        {
            get => board;
            set
            {
                if (Equals(value, board))
                    return;
                board = value;
                OnPropertyChanged();
            }
        }
        public MainWindowModel()
        {
            BeginnerCommand = new CustomCommand(StartBeginnerGame);
            AdvancedCommand = new CustomCommand(StartAdvancedGame);
            ExpertCommand = new CustomCommand(StartExpertGame);
            UserDefinedCommand = new CustomCommand(StartUserDefinedGame);
            StartBeginnerGame();
        }
        private void StartBeginnerGame()
        {
            StartGame(9, 9, 10);
        }
        private void StartAdvancedGame()
        {
            StartGame(16, 16, 40);
        }
        private void StartExpertGame()
        {
            StartGame(30, 16, 99);
        }
        private void StartUserDefinedGame()
        {
            var dlg = new DlgCustomGame {Owner = Application.Current.MainWindow};
            if (dlg.ShowDialog() != true) return;
            StartGame(dlg.CustomGame.Width, dlg.CustomGame.Height, dlg.CustomGame.NumberOfMines);
        }
        private void StartGame(int width, int height, int numberOfMines)
        {
            Board = new KaboomBoardModel(width, height, numberOfMines, UseMinesweeper);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
