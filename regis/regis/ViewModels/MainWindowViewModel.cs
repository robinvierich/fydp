using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Regis.Services;
using System.Windows.Input;
using Regis.Plugins.Models;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Regis.Base.ViewModels;
using Regis.Commands;
using Regis.Plugins.Interfaces;

namespace Regis.ViewModels
{
    [Export]
    public class MainWindowViewModel : BaseViewModel, IPartImportsSatisfiedNotification
    {
        [Import]
        private IPluginService _pluginService = null;
        private IPlugin _currentPlugin;

        public void OnImportsSatisfied()
        {
            _pluginService.PluginLoaded += new EventHandler<PluginLoadedEventArgs>(_pluginService_PluginLoaded);
            NotifyPropertyChanged(_PluginsChangedArgs);

            LoadPluginCommand.Execute("FFTViewerPlugin");
            LoadPluginCommand.Execute("TunerPlugin");
        }

        [Import]
        public LoadPluginCommand LoadPluginCommand
        {
            get;
            private set;
        }

        #region TunerPlugin
        private IPlugin _TunerPlugin;
        private static PropertyChangedEventArgs _TunerPlugin_ChangedEventArgs = new PropertyChangedEventArgs("TunerPlugin");

        public IPlugin TunerPlugin {
            get { return _TunerPlugin; }
            set {
                _TunerPlugin = value;
                NotifyPropertyChanged(_TunerPlugin_ChangedEventArgs);
            }
        }
        #endregion

        #region FFTPlugin
        private IPlugin _FFTPlugin;
        private static PropertyChangedEventArgs _FFTPlugin_ChangedEventArgs = new PropertyChangedEventArgs("FFTPlugin");

        public IPlugin FFTPlugin {
            get { return _FFTPlugin; }
            set {
                _FFTPlugin = value;
                NotifyPropertyChanged(_FFTPlugin_ChangedEventArgs);
            }
        }
        #endregion

        private PropertyChangedEventArgs _currentPluginChangedArgs = new PropertyChangedEventArgs("CurrentPlugin");
        public IPlugin CurrentPlugin
        {
            get
            {
                return _currentPlugin;
            }

            private set
            {
                _currentPlugin = value;
                NotifyPropertyChanged(_currentPluginChangedArgs);
            }
        }

        private PropertyChangedEventArgs _PluginsChangedArgs = new PropertyChangedEventArgs("Plugins");
        public ObservableCollection<IPlugin> Plugins
        {
            get
            {
                return _pluginService.Plugins;
            }
        }

        void _pluginService_PluginLoaded(object sender, PluginLoadedEventArgs e)
        {
            if (e.Plugin.PluginName == "TunerPlugin") {
                TunerPlugin = e.Plugin;
                return;
            }

            if (e.Plugin.PluginName == "FFTViewerPlugin") {
                FFTPlugin = e.Plugin;
                return;
            }


            CurrentPlugin = e.Plugin;
        }
    }
}
