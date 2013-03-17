using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using RegisFreeFormPlugin.ViewModels;

namespace RegisFreeFormPlugin
{
    public class StartFreeFormCommand : ICommand
    {
        public bool CanExecute(object parameter) {
            return true;
        }

        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter) {
            FreeFormViewModel ffvm = parameter as FreeFormViewModel;
            ffvm.Start();
        }
    }
}
