using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Regis.Services;
using Regis.Plugins;
using System.ComponentModel.Composition;

namespace Regis.Commands
{
    [Export]
    public class LoadPluginCommand : ICommand
    {
        [Import]
        IPluginService _pluginService;

        public LoadPluginCommand()
        {
        }


        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged = delegate { };

        public void Execute(object parameter)
        {
            string pluginName = parameter as string;
            _pluginService.LoadPlugin(pluginName);
        }
    }
}
