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
using System.Windows.Threading;

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

        [Import]
        private INoteDetectionSource noteSource;

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
            //double display = 50;
            //double currentFreq;
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
                Note[] notes;
                if (!noteSource.NoteQueue.TryDequeue(out notes))
                    continue;

                if (notes[0].closestRealNoteFrequency == 0)
                    continue;
                else if ((targetFreq - scaleFactor) > notes[0].frequency || notes[0].frequency > (targetFreq + scaleFactor))
                    continue;

                double currentFreq = notes[0].frequency;

                Application.Current.Dispatcher.Invoke(
                    DispatcherPriority.Render,
                    new Action<double>(setFreqText),
                    currentFreq);
                
                delta = currentFreq - targetFreq;

                double display = delta*(50/scaleFactor) + 50;

                Application.Current.Dispatcher.Invoke(
                    DispatcherPriority.Render,
                    new Action<double>(setTunerBarValue), 
                    display);

                Thread.Sleep(100);
                
            }
        }

        private void setFreqText(double freq)
        {
            currentFreqBox.Text = String.Format("{0}", freq);
        }

        private void setTunerBarValue(double val)
        {
            tunerBar.Value = val;
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
