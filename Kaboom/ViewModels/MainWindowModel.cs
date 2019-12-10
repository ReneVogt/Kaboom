using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Markup;
using Com.Revo.Games.Kaboom.Properties;
using Com.Revo.Games.Kaboom.ViewModels.Com.Aki.WpfCommons.Bindings;
using Com.Revo.Games.Kaboom.Views;
using JetBrains.Annotations;

namespace Com.Revo.Games.Kaboom.ViewModels
{
    public sealed class MainWindowModel : MarkupExtension, INotifyPropertyChanged
    {
        KaboomBoardModel board;
        bool debugChecked;
        public CustomCommand ExitCommand { get; }
        public CustomCommand RestartCommand { get; }
        public CustomCommand BeginnerCommand { get; }
        public CustomCommand AdvancedCommand { get; }
        public CustomCommand ExpertCommand { get; }
        public CustomCommand UserDefinedCommand { get; }
        public CustomCommand AboutCommand { get; }
        public bool BeginnerChecked { get; private set; }
        public bool AdvancedChecked { get; private set; }
        public bool ExpertChecked { get; private set; }
        public bool CustomChecked { get; private set; }
        public bool DebugChecked
        {
            get => debugChecked;
            set
            {
                if (value == debugChecked)
                    return;
                debugChecked = value;
                if (board != null) Board.DebugMode = debugChecked;
                Settings.Default.Debug = value;
                Settings.Default.Save();
                OnPropertyChanged();
            }
        }
        public KaboomBoardModel Board
        {
            get => board;
            set
            {
                if (Equals(value, board))
                    return;
                board = value;
                if (board != null) board.DebugMode = DebugChecked;
                OnPropertyChanged();
            }
        }
        public MainWindowModel()
        {
            BeginnerCommand = new CustomCommand(StartBeginnerGame);
            AdvancedCommand = new CustomCommand(StartAdvancedGame);
            ExpertCommand = new CustomCommand(StartExpertGame);
            UserDefinedCommand = new CustomCommand(StartUserDefinedGame);
            RestartCommand = new CustomCommand(RestartGame);
            ExitCommand = new CustomCommand(() => Environment.Exit(0));
            AboutCommand = new CustomCommand(OnAbout);
            BeginnerChecked = Settings.Default.Beginner;
            AdvancedChecked = Settings.Default.Advanced;
            ExpertChecked = Settings.Default.Expert;
            CustomChecked = Settings.Default.Custom;
            DebugChecked = Settings.Default.Debug;
            RestartGame();
        }
        private void RestartGame()
        {
            StartGame(Settings.Default.Width, Settings.Default.Height, Settings.Default.Mines);
        }
        private void StartBeginnerGame()
        {
            BeginnerChecked = true;
            CustomChecked = AdvancedChecked = ExpertChecked = false;
            StartGame(9, 9, 10);
        }
        private void StartAdvancedGame()
        {
            AdvancedChecked = true;
            CustomChecked = BeginnerChecked = ExpertChecked = false;
            StartGame(16, 16, 40);
        }
        private void StartExpertGame()
        {
            ExpertChecked = true;
            CustomChecked = AdvancedChecked = BeginnerChecked = false;
            StartGame(30, 16, 99);
        }
        private void StartUserDefinedGame()
        {
            var dlg = new DlgCustomGame
            {
                Owner = Application.Current.MainWindow
            };
            dlg.CustomGame.Width = Settings.Default.Width;
            dlg.CustomGame.Height = Settings.Default.Height;
            dlg.CustomGame.NumberOfMines = Settings.Default.Mines;
            if (dlg.ShowDialog() != true) return;
            CustomChecked = true;
            BeginnerChecked = AdvancedChecked = ExpertChecked = false;

            StartGame(dlg.CustomGame.Width, dlg.CustomGame.Height, dlg.CustomGame.NumberOfMines);
        }
        private void StartGame(int width, int height, int numberOfMines)
        {
            Settings.Default.Width = width;
            Settings.Default.Height = height;
            Settings.Default.Mines = numberOfMines;
            Settings.Default.Beginner = BeginnerChecked;
            Settings.Default.Advanced = AdvancedChecked;
            Settings.Default.Expert = ExpertChecked;
            Settings.Default.Custom = CustomChecked;
            Settings.Default.Save();
            OnPropertyChanged(nameof(BeginnerChecked));
            OnPropertyChanged(nameof(AdvancedChecked));
            OnPropertyChanged(nameof(ExpertChecked));
            OnPropertyChanged(nameof(CustomChecked));

            Board = new KaboomBoardModel(width, height, numberOfMines) {DebugMode = DebugChecked};
        }
        private void OnAbout()
        {
            MessageBox.Show("Not yet implemented!");
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
