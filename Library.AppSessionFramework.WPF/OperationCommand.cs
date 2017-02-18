using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhillipScottGivens.Library.AppSessionFramework.WPF
{
    // TypeConverter is not used, only because I do not have any 
    // controls looking for an OperationCommand
    [TypeConverter(typeof(OperationCommandConverter))]
    public class OperationCommand : ICommand
    {
        static OperationCommand()
        {
            SessionManager.RegisterOperationConverter(
                operation => new OperationCommand(operation));
        }

        public static void Register()
        {
        }

        private Operation value;

        public OperationCommand(Operation value)
        {
            // TODO: Complete member initialization
            this.value = value;
            value.AvailabilityNotifier.ChangeNotificationSent += (s, e) =>
            {
                if (CanExecuteChanged != null) 
                    CanExecuteChanged(s, e);
            };
        }

        public bool CanExecute(object parameter)
        {
            return value.IsAvailable;
        }

        public event EventHandler CanExecuteChanged;
        

        public void Execute(object parameter)
        {
            value.Execute();
        }
    }
}
