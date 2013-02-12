using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Base.ViewModels;
using System.ComponentModel;
using System.Windows.Threading;
using Regis.Plugins.Statics;
using System.ComponentModel.Composition;
using Regis.Plugins.Interfaces;
using Regis.Plugins.Models;

namespace RegisTunerPlugin.ViewModels
{

    public class NeedleTunerViewModel : BaseViewModel
    {
        [Import]
        private INoteDetectionSource _noteSource;

        public NeedleTunerViewModel() {
            DispatcherTimer timer = new DispatcherTimer() {
                Interval = TimeSpan.FromSeconds(0.5),
            };
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e) {
            Note[] notes = _noteSource.GetNotes();

            GoalFrequency = NoteDictionary.GetClosestNoteFrequency(Frequency);

        }


        #region Frequency
        private double _Frequency;
        private static PropertyChangedEventArgs _Frequency_ChangedEventArgs = new PropertyChangedEventArgs("Frequency");

        public double Frequency {
            get { return _Frequency; }
            set {
                _Frequency = value;
                NotifyPropertyChanged(_Frequency_ChangedEventArgs);
            }
        }
        #endregion

        #region GoalFrequency
        private double _GoalFrequency;
        private static PropertyChangedEventArgs _GoalFrequency_ChangedEventArgs = new PropertyChangedEventArgs("GoalFrequency");

        public double GoalFrequency {
            get { return _GoalFrequency; }
            set {
                _GoalFrequency = value;
                NotifyPropertyChanged(_GoalFrequency_ChangedEventArgs);
            }
        }
        #endregion

        #region NeedleLength
        private double _NeedleLength = 100; // px
        private static PropertyChangedEventArgs _NeedleLength_ChangedEventArgs = new PropertyChangedEventArgs("NeedleLength");

        public double NeedleLength {
            get { return _NeedleLength; }
            set {
                _NeedleLength = value;
                NotifyPropertyChanged(_NeedleLength_ChangedEventArgs);
            }
        }
        #endregion

    }
}
