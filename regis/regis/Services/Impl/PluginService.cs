using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Regis.Plugins.Models;
using System.Collections.ObjectModel;
using Regis.Composition;
using Regis.Plugins.Interfaces;

namespace Regis.Services.Impl
{
    public class PluginLoadException : Exception
    {
        public PluginLoadException(string exceptionString) : base(exceptionString)
        {
        }
    }

    [Export(typeof(IPluginService))]
    public class PluginService : IPluginService//, IPartImportsSatisfiedNotification
    {
        [ImportMany]
        ObservableCollection<IPlugin> _plugins = null;

        public void LoadPlugin(string pluginName)
        {
            if (string.IsNullOrEmpty(pluginName))
                throw new ArgumentNullException("plugin");

            IPlugin pluginToLoad = _plugins.Where(plugin => plugin.PluginName == pluginName).SingleOrDefault();

            if (pluginToLoad == null)
                throw new PluginLoadException("Cannot find plugin with name: " + pluginName);

            //Importer.Compose(pluginToLoad);
            pluginToLoad.Load();
            
            RaisePluginLoaded(pluginToLoad);
        }

        public ObservableCollection<IPlugin> Plugins
        {
            get
            {
                return _plugins;
            }
        }

        public ObservableCollection<IPlugin> ButtonPlugins {
            get {
                return new ObservableCollection<IPlugin>(_plugins.Where(p => p.Layout == PluginLayout.Button));
            }
        }

        public ObservableCollection<IPlugin> MenuPlugins {
            get {
                return new ObservableCollection<IPlugin>(_plugins.Where(p => p.Layout == PluginLayout.Menu));
            }
        }

        #region Events
        private void RaisePluginLoaded(IPlugin plugin)
        {
            EventHandler<PluginLoadedEventArgs> h = PluginLoaded;
            if (h == null) return;

            h(this, new PluginLoadedEventArgs(plugin));
        }
        public event EventHandler<PluginLoadedEventArgs> PluginLoaded;
        #endregion

        //public void OnImportsSatisfied()
        //{
        //    ObservableCollection<IPlugin> plugins =;
            
        //}
    }
}
