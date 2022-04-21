using System;
using System.Windows.Input;

namespace yyy
{
    public class RelayCommand : ICommand
    {
        private Action<object> _execute;
        private Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        ///*Call --> new RelayCommand(MethodNameTypeVoid, MethodNameTypeboolean);*/
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            this._execute = (o => execute());
            this._canExecute = (o => canExecute());
        }

        public RelayCommand(Action execute, bool canExecute)
        {
            this._execute = (o => execute());
            this._canExecute = (o => canExecute);
        }

        public RelayCommand(Action execute)
        {
            this._execute = (o => execute());
            this._canExecute = (o => true);
        }

        [Obsolete]
        ///*Call --> new RelayCommand(o => { Execute(); }, o => CanExecute());*/
        //public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        //{
        //    this.execute = execute;
        //    this.canExecute = canExecute;
        //}

        public bool CanExecute(object parameter)
        {
            return this._canExecute == null || this._canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this._execute(parameter);
        }
    }

}
