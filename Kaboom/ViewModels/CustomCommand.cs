using System;
using System.Windows.Input;
using JetBrains.Annotations;

namespace Com.Revo.Games.Kaboom.ViewModels
{
    namespace Com.Aki.WpfCommons.Bindings
    {
        /// <summary>
        /// Provides an easy way to bind commands to view model methods
        /// </summary>
        public class CustomCommand : ICommand
        {
            private bool canExecute;
            private readonly Action action;

            /// <summary>
            /// Gets or sets a flag indicating wether this command can be executed or not.
            /// </summary>
            public bool Executable
            {
                get => canExecute;
                set
                {
                    if (canExecute == value) return;
                    canExecute = value;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }

            /// <summary>
            /// Creates a new <see cref="CustomCommand"/> instance.
            /// </summary>
            /// <param name="action">The <see cref="Action"/> to be executed by this command.</param>
            /// <param name="canExecute">The initial state of the command. Optional, default value is <code>true</code>.</param>
            public CustomCommand([NotNull] Action action, bool canExecute = true)
            {
                this.action = action ?? throw new ArgumentNullException(nameof(action));
                this.canExecute = canExecute;
            }

            /// <summary>
            /// Indicates if the command can be executed.
            /// </summary>
            /// <param name="parameter">This parameter is ignored.</param>
            /// <returns><code>true</code> if the command can be executed, <code>false</code> if not.</returns>
            public bool CanExecute(object parameter) => canExecute;
            /// <summary>
            /// Executes the command.
            /// </summary>
            /// <param name="parameter">This parameter is ignored.</param>
            public void Execute(object parameter) => action();
            /// <summary>
            /// Raised if the state of the command (<see cref="Executable"/> and <see cref="CanExecute"/>) changed.
            /// </summary>
            public event EventHandler CanExecuteChanged;
        }

        /// <summary>
        /// Provides an easy way to bind commands to view model methods
        /// </summary>
        /// <typeparam name="T">The type of the commmand parameter.</typeparam>
        public class CustomCommand<T> : ICommand
        {
            private bool canExecute;
            private readonly Action<T> action;

            /// <summary>
            /// Gets or sets a flag indicating wether this command can be executed or not.
            /// </summary>
            public bool Executable
            {
                get => canExecute;
                set
                {
                    if (canExecute == value) return;
                    canExecute = value;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }

            /// <summary>
            /// Creates a new <see cref="CustomCommand"/> instance.
            /// </summary>
            /// <param name="action">The <see cref="Action"/> to be executed by this command.</param>
            /// <param name="canExecute">The initial state of the command. Optional, default value is <code>true</code>.</param>
            public CustomCommand([NotNull] Action<T> action, bool canExecute = true)
            {
                this.action = action ?? throw new ArgumentNullException(nameof(action));
                this.canExecute = canExecute;
            }

            /// <summary>
            /// Indicates if the command can be executed.
            /// </summary>
            /// <param name="parameter">This parameter is ignored.</param>
            /// <returns><code>true</code> if the command can be executed, <code>false</code> if not.</returns>
            public bool CanExecute(object parameter) => canExecute;
            /// <summary>
            /// Executes the command.
            /// </summary>
            /// <param name="parameter">This parameter is passed to the view model action.</param>
            public void Execute(object parameter) => action((T)parameter);
            /// <summary>
            /// Raised if the state of the command (<see cref="Executable"/> and <see cref="CanExecute"/>) changed.
            /// </summary>
            public event EventHandler CanExecuteChanged;
        }
    }
}
