using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Regis.Plugins;
using Regis.Services;
using System.Windows.Input;
using Regis.Plugins.Models;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Regis.Base.ViewModels;
using Regis.Commands;

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
            this.PropertyChanged += new PropertyChangedEventHandler(MainWindowViewModel_PropertyChanged);
            _pluginService.PluginLoaded += new EventHandler<PluginLoadedEventArgs>(_pluginService_PluginLoaded);
            NotifyPropertyChanged(_PluginsChangedArgs);
        }

        void MainWindowViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentPlugin")
            {
                LoadPluginCommand.Execute(CurrentPlugin);
            }
        }


        [Import]
        public LoadPluginCommand LoadPluginCommand
        {
            get;
            private set;
        }

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
            CurrentPlugin = e.Plugin;
        }
    }
}
