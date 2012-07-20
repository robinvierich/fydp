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
                    ((FrameworkElement)this.FindName("arrow" + i.ToString())).Visibility = Visibility.Visible;

                    //while (true)
                    //{
                        //TODO
                        //poll for current note in note detection queue

                        //if note matched targetFreq
                            //((FrameworkElement)this.FindName("green" + i.ToString())).Visibility = Visibility.Visible;
                            //break
                   //}

                    ((FrameworkElement)this.FindName("arrow" + i.ToString())).Visibility = Visibility.Hidden;
                }
            }
        }
    }
}
