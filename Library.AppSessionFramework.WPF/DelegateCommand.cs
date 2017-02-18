using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace PhillipScottGivens.Library.AppSessionFramework.WPF
{
    public class DelegateCommand : ICommand
    {
        private readonly Action action;
        public DelegateCommand(Action action)
        {
            this.action = action;

            // HACK: Removes the 
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }

        bool ICommand.CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
        
        void ICommand.Execute(object parameter)
        {
            action();
        }
    }
}
