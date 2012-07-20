using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Regis.Plugins.Interfaces;

namespace Regis.Services
{
    public interface IPluginService
    {
        void LoadPlugin(string pluginName);
        event EventHandler<PluginLoadedEventArgs> PluginLoaded;
        ObservableCollection<IPlugin> Plugins { get; }
    }
}
