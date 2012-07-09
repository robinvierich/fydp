using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Plugins;
using System.Collections.ObjectModel;

namespace Regis.Services
{
    public interface IPluginService
    {
        void LoadPlugin(string pluginName);
        event EventHandler<PluginLoadedEventArgs> PluginLoaded;
        ObservableCollection<IPlugin> Plugins { get; }
    }
}
