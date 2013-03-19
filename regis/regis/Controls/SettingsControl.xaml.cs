using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using Regis.ViewModels;
using Regis.Commands;
using BlueWave.Interop.Asio;
using Regis.Services.Realtime;
using Regis.Plugins.Interfaces;

namespace Regis.Controls
{
    [Export(typeof(IPlugin))]
    public partial class SettingsControl : UserControl, IPlugin, IPartImportsSatisfiedNotification
    {
        public SettingsControl()
        {
            InitializeComponent();
        }

        public void OnImportsSatisfied()
        {
            DataContext = ViewModel;
        }

        private void asioDriverComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 1) return;
            InstalledDriver driver = e.AddedItems[0] as InstalledDriver;

            LoadDriverCommand cmd = new LoadDriverCommand();
            LoadDriverCommandArgs args = new LoadDriverCommandArgs(driver);

            cmd.Execute(args);
        }

        // Talking directly to the ViewModel like this is not the best.
        // Usually we want to use bindings and such, but this is simplest for something like this.
        private void channelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 1) return;
            Channel channel = e.AddedItems[0] as Channel;

            AsioSamplingServiceArgs args = new AsioSamplingServiceArgs()
            {
                Channel = channel,
                Driver = ViewModel.LoadedDriver,
            };

            ViewModel.RestartSamplingServiceCommand.Execute(args);
        }
        
        [Import]
        private SettingsViewModel ViewModel
        {
            get;
            set;
        }

        #region IPlugin
        public void Load() { }

        public FrameworkElement GetVisualContent()
        {
            return this;
        }

        public string PluginName
        {
            get { return "AsioSettingsPlugin"; }
        }

        public string FriendlyPluginName
        {
            get { return "Settings"; }
        }

        #endregion


        public PluginLayout Layout {
            get { return PluginLayout.Menu; }
        }
    }
}
