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
using System.Windows;

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
            NotifyPropertyChanged(_MenuPlugins_ChangedEventArgs);
            NotifyPropertyChanged(_ButtonPlugins_ChangedEventArgs);

            LoadPluginCommand.Execute("FFTViewerPlugin");
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

        #region ButtonPlugins
        private static PropertyChangedEventArgs _ButtonPlugins_ChangedEventArgs = new PropertyChangedEventArgs("ButtonPlugins");
        public ObservableCollection<IPlugin> ButtonPlugins {
            get { 
                return _pluginService.ButtonPlugins; 
            }
        }
        #endregion

        #region MenuPlugins
        private static PropertyChangedEventArgs _MenuPlugins_ChangedEventArgs = new PropertyChangedEventArgs("MenuPlugins");
        public ObservableCollection<IPlugin> MenuPlugins {
            get { 
                return _pluginService.MenuPlugins; 
            }
        }
        #endregion


        void _pluginService_PluginLoaded(object sender, PluginLoadedEventArgs e)
        {
            if (e.Plugin.PluginName == "TunerPlugin") {
                if (TunerPlugin == null)
                {
                    TunerPlugin = e.Plugin;
                }
                else {
                    TunerPlugin = null;
                }
                return;
            }

            if (e.Plugin.PluginName == "FFTViewerPlugin") {
                FFTPlugin = e.Plugin;
                return;
            }

            if(e.Plugin.PluginName == "AchievementsPlugin") {
                Window w = new Window();
                w.Content = e.Plugin.GetVisualContent();
                w.SizeToContent = SizeToContent.WidthAndHeight;
                w.Left = 0;
                w.Top = 0;
                w.ShowDialog();

                w.Content = null;
                return;
            }

            CurrentPlugin = e.Plugin;
        }
    }
}
