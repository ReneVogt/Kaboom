using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Markup;
using JetBrains.Annotations;

namespace Com.Revo.Games.Kaboom.ViewModels
{
    public class CustomGameViewModel : MarkupExtension, INotifyPropertyChanged
    {
        int width = 20, height = 20, numberOfMine = 50;
        public int Width
        {
            get => width;
            set
            {
                if (value == width) return;
                width = value;
                Validate();
                OnPropertyChanged();
            }
        }
        public int Height
        {
            get => height;
            set
            {
                if (value == height) return;
                height = value;
                Validate();
                OnPropertyChanged();
            }
        }

        public int NumberOfMines
        {
            get => numberOfMine;
            set
            {
                if (value == numberOfMine) return;
                numberOfMine = value;
                Validate();
                OnPropertyChanged();
            }
        }

        public bool IsValid { get; private set; } = true;

        void Validate()
        {
            bool valid = width > 0 && width <= 1000 && height > 0 && height <= 1000 && numberOfMine <= width * height;
            if (valid == IsValid) return;
            IsValid = valid;
            OnPropertyChanged(nameof(IsValid));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
