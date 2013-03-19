using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace RegisPlayAlongPlugin
{
    public class StopPlayAlongCommand : ICommand
    {
        public bool CanExecute(object parameter) {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter) {
            PlayAlongViewModel pavm = (PlayAlongViewModel)parameter;
            pavm.Stop();
        }
    }
}
