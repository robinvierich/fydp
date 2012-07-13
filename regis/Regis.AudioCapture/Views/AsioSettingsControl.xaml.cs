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
using Regis.Plugins;
using System.ComponentModel.Composition;
using Regis.AudioCapture.ViewModels;
using Regis.AudioCapture.Commands;
using BlueWave.Interop.Asio;

namespace Regis.AudioCapture.Views
{
    [Export(typeof(IPlugin))]
    public partial class AsioSettingsControl : UserControl, IPlugin
    {
        public AsioSettingsControl()
        {
            InitializeComponent();
            DataContext = new AsioSettingsViewModel();
        }

        public void Load(Plugins.Models.NoteStream noteStream){}

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
            get { return "Asio Settings"; }
        }

        private void asioDriverComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 1) return;
            InstalledDriver driver = e.AddedItems[0] as InstalledDriver;

            LoadDriverCommand cmd = new LoadDriverCommand();
            LoadDriverCommandArgs args = new LoadDriverCommandArgs(driver, 48000);

            cmd.Execute(args);
        }


        public NoteDetectionAlgorithm Algorithm
        {
            get { throw new NotImplementedException(); }
        }
    }
}
