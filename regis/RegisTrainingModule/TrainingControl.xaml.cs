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
using Regis.Plugins.Interfaces;
using System.Threading;
using RegisTrainingModule.Models;
using RegisTrainingModule.ViewModels;
using System.Windows.Threading;
using Regis.Plugins.Models;

namespace RegisTrainingModule
{
    /// <summary>
    /// Interaction logic for TrainingControl.xaml
    /// </summary>
    
    [Export(typeof(IPlugin))]
    public partial class TrainingControl : UserControl, IPlugin
    {
        Thread _trainingThread;
        bool _runningTraining;

        [Import]
        private INoteDetectionSource noteSource;

        public TrainingControl()
        {
            InitializeComponent();
        }

        [Import]
        private TrainingViewModel ViewModel
        {
            get;
            set;
        }

        public void OnImportsSatisfied()
        {
            DataContext = ViewModel;
        }

        #region IPlugin

        public void Load()
        {
            
        }

        public FrameworkElement GetVisualContent()
        {
            return this;
        }

        public string PluginName
        {
            get { return "TrainingControlPlugin"; }
        }

        public string FriendlyPluginName
        {
            get { return "TrainingModule"; }
        }

        #endregion

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            StopTraining();
            _trainingThread = new Thread(new ThreadStart(StartTraining));
            _trainingThread.Start();
        }

        private void StartTraining()
        {
            _runningTraining = true;
            RunTraining();
        }

        private void StopTraining()
        {
            if (_trainingThread == null)
                return;

            _runningTraining = false;
            _trainingThread.Join();
        }

        private void RunTraining()
        {
            while(_runningTraining)
            {
                for (int i = 0; i < 8; i++)
                {
                    string _arrow = "arrow" + i.ToString();
                    string _green = "green" + i.ToString();
                    Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Render,
                        new Action<string>(ArrowShow),
                        _arrow);

                    while (true)
                    {
                        Note[] notes;
                        if (!noteSource.NoteQueue.TryPeek(out notes))
                            continue;

                        if (notes[0].closestRealNoteFrequency == 0)
                            continue;

                        if (notes[0].closestRealNoteFrequency == ViewModel.TrainingModules[0].TargetFreq[i])
                        {
                            Application.Current.Dispatcher.Invoke(
                                DispatcherPriority.Render,
                                new Action<string>(GreenShow),
                                _green);

                            break;
                        }
                   }

                    Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Render,
                        new Action<string>(ArrowHide),
                        _arrow);
                }

                StopTraining();
            }
        }

        private void GreenShow(string greenIndex)
        {
            ((FrameworkElement)this.FindName(greenIndex)).Visibility = Visibility.Visible;
        }

        private void ArrowShow(string arrowIndex)
        {
            ((FrameworkElement)this.FindName(arrowIndex)).Visibility = Visibility.Visible;
        }

        private void ArrowHide(string arrowIndex)
        {
            ((FrameworkElement)this.FindName(arrowIndex)).Visibility = Visibility.Hidden;
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            StopTraining();
        }
    }
}
