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
using Regis.Plugins;
using System.ComponentModel.Composition;
using Regis.Plugins.Models;
using RegisTunerPlugin.Models;

namespace RegisTunerPlugin
{
    /// <summary>
    /// Interaction logic for TunerControl.xaml
    /// </summary>
    /// 
    [Export(typeof(IPlugin))]
    public partial class TunerControl : UserControl, IPlugin
    {
        public TunerControl()
        {
            InitializeComponent();
            LoadTunings();
        }

        List<Tuning> tuningList = new List<Tuning>();

        private void LoadTunings()
        {
            StreamReader readFile = new StreamReader(@"C:\regis\fydp\regis\RegisTunerPlugin\TunerPluginFiles\tunings.cfg");
            while (true)
            {
                string line = readFile.ReadLine();

                if (line == "#end")
                    break;

                Tuning tuning = new Tuning();
                GuitarString[] guitarString = new GuitarString[6]; 
                tuning._tuningName = line;

                for (int i = 0; i < 6; i++)
                {
                    line = readFile.ReadLine();
                    string[] lineParts = line.Split(',');
                    guitarString[i]._stringName = lineParts[0];
                    guitarString[i]._frequency = Convert.ToDouble(lineParts[1]);
                }

                line = readFile.ReadLine();
                tuning._guitarStrings = guitarString;
                tuningList.Add(tuning);
            }

            this.tuningBox.ItemsSource = tuningList;
            this.tuningBox.DisplayMemberPath = "_tuningName";
            //this.tuningBox.SelectedValuePath = "_tuningName";
            StartTuner();
        }

        private void StartTuner()
        {
            Tuner(82.407,1);
            return;
        }

        private void Tuner(double targetFreq, int stringNum)
        {
            double delta = 0;
            double display = 50;
            double currentFreq = 79;
            double scaleFactor;

            switch (stringNum)
            {
                case 1:
                    scaleFactor = 5;
                    break;
                default:
                    scaleFactor = 5;
                    break;
            }

            this.targetFreqBox.Text = String.Format("{0}", targetFreq);

            //while (true)
            //{
                this.currentFreqBox.Text = String.Format("{0}", currentFreq);
                
                delta = currentFreq - targetFreq;

                display = delta*(50/scaleFactor) + 50;
                this.tunerBar.Value = display;

            //}
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

        public NoteDetectionAlgorithm Algorithm
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
