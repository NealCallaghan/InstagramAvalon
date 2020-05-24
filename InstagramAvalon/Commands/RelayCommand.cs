namespace InstagramAvalon.Commands
{
    using System;
    using System.Windows.Input;

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _methodToExecute;
        private readonly Func<bool> _canExecuteEvaluator;

        public RelayCommand(Action<object> action, Func<bool> canExecuteEvaluator)
        {
            _methodToExecute = action;
            _canExecuteEvaluator = canExecuteEvaluator;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        { 
            _methodToExecute(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}