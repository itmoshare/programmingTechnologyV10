using System;
using System.ComponentModel;
using System.Windows.Input;

namespace MySorts.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public class Command : ICommand
        {
            private Func<object, bool> _canExecute;
            private Action _processCommand;

            public Command(Func<object, bool> canExecute, Action processCommand)
            {
                _canExecute = canExecute;
                _processCommand = processCommand;
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }


            public bool CanExecute(object parameter)
            {
                return _canExecute(parameter);
            }

            public void Execute(object parameter)
            {
                _processCommand();
            }
        }
    }
}
