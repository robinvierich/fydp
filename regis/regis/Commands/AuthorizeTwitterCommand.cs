using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.ComponentModel.Composition;
using Regis.Plugins.Interfaces;

namespace Regis.Commands
{
    [Export(typeof(AuthorizeTwitterCommand))]
    public class AuthorizeTwitterCommand: ICommand, IPartImportsSatisfiedNotification
    {
        public bool CanExecute(object parameter)
        {
            return !string.IsNullOrWhiteSpace(parameter as string);
        }

        public event EventHandler CanExecuteChanged;
        public void Raise_CanExecuteChanged()
        {
            EventHandler h = CanExecuteChanged;
            if (h == null) return;

            h(this, new EventArgs());
        }

        [Import]
        private ISocialNetworkingService _socialNetworkingService = null;
        private Queue<object> requestQueue = new Queue<object>();

        public void Execute(object parameter)
        {
            if (_socialNetworkingService == null)
            {
                requestQueue.Enqueue(parameter);
            }

            _socialNetworkingService.AuthTwitter(parameter as string);
        }

        public void OnImportsSatisfied()
        {
            try
            {
                object param = requestQueue.Dequeue();
                while (param != null)
                {
                    param = requestQueue.Dequeue();
                    Execute(param);
                }

            }
            catch { }
        }
    }
}
