using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Regis.Services;
using Regis.Plugins;

namespace Regis.Commands
{
    public class LoadPluginCommand : ICommand
    {
        IPluginService _pluginService;

        public LoadPluginCommand(IPluginService pluginService)
        {
            _pluginService = pluginService;
        }


        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            string pluginName = parameter as string;
            _pluginService.LoadPlugin(pluginName);
        }
    }
}
