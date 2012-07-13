using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Regis.Services.Realtime;
using System.ComponentModel.Composition;

namespace Regis.Commands
{
    [Export]
    public class RestartSamplingServiceCommand: ICommand
    {
        [Import]
        private IAsioSamplingService _asioSamplingService = null;

        public bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        public event EventHandler CanExecuteChanged = delegate { };

        public void Execute(object parameter)
        {
            _asioSamplingService.Stop();

            AsioSamplingServiceArgs args = parameter as AsioSamplingServiceArgs;
            _asioSamplingService.Start(args);
        }
    }
}
