using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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
using Regis.Plugins.Models;
using RegisTunerPlugin.Models;
using RegisTunerPlugin.ViewModels;
using System.Threading;
using Regis.Plugins.Interfaces;

namespace RegisTunerPlugin
{
    /// <summary>
    /// Interaction logic for TunerControl.xaml
    /// </summary>
    /// 
    [Export(typeof(IPlugin))]
    public partial class TunerControl : UserControl, IPlugin, IPartImportsSatisfiedNotification
    {
        Thread _tunerThread;
        bool _runningTuner = false;

        public TunerControl()
        {
            InitializeComponent();
            
        }

        [Import]
        private TunerViewModel ViewModel
        {
            get;
            set;
        }

        public void OnImportsSatisfied()
        {
            DataContext = ViewModel;
        }

        private void stringBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GuitarString gs = stringBox.SelectedItem as GuitarString;
            if (gs == null) return;

            StopTuner();
            _tunerThread = new Thread(new ParameterizedThreadStart(StartTuner));
            _tunerThread.Start(gs);
        }

        private void StartTuner(object param)
        {
            GuitarString gs = param as GuitarString;

            _runningTuner = true;
            RunTuner(gs.Frequency, gs.StringNum);
        }

        private void StopTuner()
        {
            if (_tunerThread == null) 
                return;

            _runningTuner = false;
            _tunerThread.Join();
        }

        private void RunTuner(double targetFreq, int stringNum)
        {
            double delta = 0;
            double display = 50;
            double currentFreq;
            double scaleFactor;

            switch (stringNum)
            {
                case 0:
                    scaleFactor = 4;
                    break;
                case 1:
                    scaleFactor = 5;
                    break;
                case 2:
                    scaleFactor = 6;
                    break;
                case 3:
                    scaleFactor = 9;
                    break;
                case 4:
                    scaleFactor = 11;
                    break;
                case 5:
                    scaleFactor = 14;
                    break;
                case 6:
                    scaleFactor = 20;
                    break;
                default:
                    scaleFactor = 5;
                    break;
            }

            while (_runningTuner)
            {

                // Todo: make this work



                //this.currentFreqBox.Text = String.Format("{0}", currentFreq);
                
                //delta = currentFreq - targetFreq;

                //display = delta*(50/scaleFactor) + 50;
                //this.tunerBar.Value = display;
            }
        }

        #region IPlugin
        
        public FrameworkElement GetVisualContent()
        {
            return this;
        }


        public string PluginName
        {
            get
            {
                return "TunerPlugin";
            }
        }

        public void Load()
        {
            
        }

        public string FriendlyPluginName
        {
            get { return "Tuner"; }
        }

        #endregion

        private void stopTunerButton_Click(object sender, RoutedEventArgs e)
        {
            StopTuner();
        }
    }
}
