using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Automa
{
    public class DelegateCommand : ICommand
    {
        private Action<object> action_;
        private Func<object, bool> canExecuteAction_;

        public DelegateCommand(Action<object> executeAction, Func<object, bool> canExecuteAction)
        {
            action_ = executeAction;
            canExecuteAction_ = canExecuteAction;
        }

        public bool CanExecute(object parameter)
        {
            return canExecuteAction_(parameter);
        }
        
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            action_(parameter);
        }
    }
}
