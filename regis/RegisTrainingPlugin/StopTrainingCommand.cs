using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace RegisTrainingPlugin
{
    public class StopTrainingCommand: ICommand
    {
        public bool CanExecute(object parameter) {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter) {
            TrainingViewModel vm = parameter as TrainingViewModel;
            vm.Stop();
        }
    }
}
