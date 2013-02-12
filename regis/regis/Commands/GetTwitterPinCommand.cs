using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.ComponentModel.Composition;
using Regis.Plugins.Interfaces;

namespace Regis.Commands
{
    [Export(typeof(GetTwitterPinCommand))]
    public class GetTwitterPinCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }
        [Import]
        private ISocialNetworkingService _socialNetworkingService;
        private Queue<object> requestQueue = new Queue<object>();

        public void Execute(object parameter)
        {
            if (_socialNetworkingService == null)
            {
                requestQueue.Enqueue(parameter);
            }

            _socialNetworkingService.GetTwitterPin();
        }

        public void OnImportsSatisfied()
        {
            object param = requestQueue.Dequeue();
            while (param != null) {
                param = requestQueue.Dequeue();
                Execute(param);
            }
        }
        public event EventHandler CanExecuteChanged;
    }
}
