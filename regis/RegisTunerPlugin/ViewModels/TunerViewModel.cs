using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Base.ViewModels;
using System.IO;
using RegisTunerPlugin.Models;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel;

namespace RegisTunerPlugin.ViewModels
{
    [Export]
    public class TunerViewModel : BaseViewModel
    {

        public TunerViewModel() {
            Tunings = new ObservableCollection<Tuning>();
            LoadTunings();
        }

        #region CurrentFrequency
        private double _CurrentFrequency;
        private static PropertyChangedEventArgs _CurrentFrequency_ChangedEventArgs = new PropertyChangedEventArgs("CurrentFrequency");

        public double CurrentFrequency {
            get { return _CurrentFrequency; }
            set {
                _CurrentFrequency = value;
                NotifyPropertyChanged(_CurrentFrequency_ChangedEventArgs);
            }
        }
        #endregion


        private ObservableCollection<Tuning> _tunings;
        public ObservableCollection<Tuning> Tunings
        {
            get { return _tunings; }
            private set { _tunings = value; }
        }

        //private double _display;
        //public double Display
        //{
        //    get { return _display; }
        //    private set { _display = value; }
        //}

        private void LoadTunings()
        {
            try
            {
                StreamReader readFile = new StreamReader(Environment.CurrentDirectory + "\\Config\\tunings.cfg");
                while (true)
                {
                    string line = readFile.ReadLine();

                    if (line == "#end")
                        break;

                    Tuning tuning = new Tuning();
                    ObservableCollection<GuitarString> guitarStrings = new ObservableCollection<GuitarString>();
                    tuning.Name = line;

                    while (true)
                    {
                        line = readFile.ReadLine();

                        if (line == String.Empty)
                            break;

                        GuitarString guitarString = new GuitarString();
                        string[] lineParts = line.Split(',');
                        guitarString.StringName = lineParts[0];
                        guitarString.Frequency = Convert.ToDouble(lineParts[1]);
                        guitarString.StringNum = Convert.ToInt32(lineParts[2]);
                        guitarStrings.Add(guitarString);
                    }

                    tuning.GuitarStrings = guitarStrings;
                    Tunings.Add(tuning);  
                }
                Console.WriteLine("DEBUG::REGIS:: tunings.cfg => Loaded");
            }
            catch { }
        }


    }
}
