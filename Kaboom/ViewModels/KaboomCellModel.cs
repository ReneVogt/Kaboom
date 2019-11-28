using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Com.Revo.Games.Kaboom.ViewModels
{
    public class KaboomCellModel : INotifyPropertyChanged
    {
        KaboomCellState state;
        public KaboomCellState State
        {
            get => state;
            set
            {
                if (value == state)
                    return;
                state = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
