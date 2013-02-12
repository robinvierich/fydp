using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Regis.Plugins.Interfaces;
using Regis.Services.Impl;

namespace Regis.Services
{
    public interface IPluginService
    {
        void LoadPlugin(string pluginName);
        event EventHandler<PluginLoadedEventArgs> PluginLoaded;
        ObservableCollection<IPlugin> Plugins { get; }
    }

    public class PluginLoadedEventArgs : EventArgs
    {
        IPlugin _plugin;
        public PluginLoadedEventArgs(IPlugin plugin) {
            _plugin = plugin;
        }

        public IPlugin Plugin {
            get {
                return _plugin;
            }
        }
    }
}
