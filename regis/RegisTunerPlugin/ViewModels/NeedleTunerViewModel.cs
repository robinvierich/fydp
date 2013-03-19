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
    [Export]
    public class NeedleTunerViewModel : BaseViewModel, IPartImportsSatisfiedNotification
    {
        [Import]
        private INoteDetectionSource _noteSource = null;

        public void OnImportsSatisfied() {
            _noteSource.NotesDetected += new EventHandler<NotesDetectedEventArgs>(_noteSource_NotesDetected);
        }

        public void _noteSource_NotesDetected(object sender, NotesDetectedEventArgs e) {
            Note note = e.Notes.Last();


            if (note.Semitone < 1)
                return;
                
               // throw new NotImplementedException(" note is too low for current tuner implementation (can be fixed)");

            Note prevNote = new Note { Semitone = note.Semitone - 1 };

            FrequencyRange = note.ClosestRealNoteFrequency - prevNote.ClosestRealNoteFrequency;

            GoalFrequency = note.ClosestRealNoteFrequency;
            Frequency = note.frequency;
        }

        public NeedleTunerViewModel() {
        }

        #region MinFrequency
        private static PropertyChangedEventArgs _MinFrequency_ChangedEventArgs = new PropertyChangedEventArgs("MinFrequency");

        public double MinFrequency {
            get { return GoalFrequency - FrequencyRange; }
        }
        #endregion

        #region MaxFrequency
        private static PropertyChangedEventArgs _MaxFrequency_ChangedEventArgs = new PropertyChangedEventArgs("MaxFrequency");

        public double MaxFrequency {
            get { return GoalFrequency + FrequencyRange; }
        }
        #endregion


        double width = 300;

        #region X
        private static PropertyChangedEventArgs _X_ChangedEventArgs = new PropertyChangedEventArgs("X");

        public double X {
            get {
                return (Frequency - GoalFrequency) * width / (FrequencyRange) + width/2d; 
            }
        }
        #endregion

        #region FrequencyRange
        private double _FrequencyRange;
        private static PropertyChangedEventArgs _FrequencyRange_ChangedEventArgs = new PropertyChangedEventArgs("FrequencyRange");

        public double FrequencyRange {
            get { return _FrequencyRange; }
            set {
                _FrequencyRange = value;
                NotifyPropertyChanged(_FrequencyRange_ChangedEventArgs);
                NotifyPropertyChanged(_MaxFrequency_ChangedEventArgs);
                NotifyPropertyChanged(_MinFrequency_ChangedEventArgs);
            }
        }
        #endregion

        #region Frequency
        private double _Frequency;
        private static PropertyChangedEventArgs _Frequency_ChangedEventArgs = new PropertyChangedEventArgs("Frequency");

        public double Frequency {
            get { return _Frequency; }
            set {
                _Frequency = value;
                NotifyPropertyChanged(_Frequency_ChangedEventArgs);
                NotifyPropertyChanged(_X_ChangedEventArgs);
                NotifyPropertyChanged(_MaxFrequency_ChangedEventArgs);
                NotifyPropertyChanged(_MinFrequency_ChangedEventArgs);
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
                NotifyPropertyChanged(_X_ChangedEventArgs);
                NotifyPropertyChanged(_MaxFrequency_ChangedEventArgs);
                NotifyPropertyChanged(_MinFrequency_ChangedEventArgs);
            }
        }
        #endregion

        #region NeedleLength
        private double _NeedleLength = 100d;
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
