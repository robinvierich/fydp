using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Base.ViewModels;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Regis.Plugins.Statics;
using Regis.Plugins.Interfaces;
using Regis.Plugins.Models;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;

namespace RegisFreeFormPlugin.ViewModels
{
    [Export]
    class FreeFormViewModel : BaseViewModel, IPartImportsSatisfiedNotification
    {
        [Import]
        private INoteDetectionSource _noteSource;
        //private DispatcherTimer _noteTimer;

        public void OnImportsSatisfied() {
        }

        public void _noteSource_NotesDetected(object sender, NotesDetectedEventArgs e) {
            foreach (Note note in e.Notes) { 
                NotesPlayed.Add(note);
            }
            CurrentTime = DateTime.Now;
        }

        private double[] _frequencies;
        public FreeFormViewModel() {
            _frequencies = NoteDictionary.NoteDict.Keys.ToArray<double>();
            //_noteTimer = new DispatcherTimer() {
            //    Interval = TimeSpan.FromMilliseconds(20)
            //};

            //_noteTimer.Tick += new EventHandler(_noteTimer_Tick);

            NotesPlayed = new ObservableCollection<Note>();
            CurrentTime = DateTime.Now;

            
        }

        public void Reset() {
            this.NotesPlayed.Clear();
            StartTime = DateTime.Now;
            CurrentTime = DateTime.Now;
        }

        internal void Start() {
            Reset();
            _noteSource.NotesDetected += new EventHandler<NotesDetectedEventArgs>(_noteSource_NotesDetected);
        }


        internal void Stop() {
            _noteSource.NotesDetected -= _noteSource_NotesDetected;
        }

        #region CurrentTime
        private DateTime _CurrentTime;
        private static PropertyChangedEventArgs _CurrentTime_ChangedEventArgs = new PropertyChangedEventArgs("CurrentTime");

        public DateTime CurrentTime {
            get { return _CurrentTime; }
            set {
                _CurrentTime = value;
                NotifyPropertyChanged(_CurrentTime_ChangedEventArgs);
            }
        }
        #endregion

        #region StartTime
        private DateTime _StartTime;
        private static PropertyChangedEventArgs _StartTime_ChangedEventArgs = new PropertyChangedEventArgs("StartTime");

        public DateTime StartTime {
            get { return _StartTime; }
            set {
                _StartTime = value;
                NotifyPropertyChanged(_StartTime_ChangedEventArgs);
            }
        }
        #endregion

        #region StartFreeFormCommand
        private StartFreeFormCommand _StartFreeFormCommand = new StartFreeFormCommand();
        public StartFreeFormCommand StartFreeFormCommand {
            get { return _StartFreeFormCommand; }
            set {
                _StartFreeFormCommand = value;
            }
        }
        #endregion

        #region StopFreeFormCommand
        private StopFreeFormCommand _StopFreeFormCommand = new StopFreeFormCommand();
        public StopFreeFormCommand StopFreeFormCommand {
            get { return _StopFreeFormCommand; }
            set {
                _StopFreeFormCommand = value;
            }
        }
        #endregion

        #region PlayedNotes
        private ObservableCollection<Note> _PlayedNotes;
        private static PropertyChangedEventArgs _PlayedNotes_ChangedEventArgs = new PropertyChangedEventArgs("PlayedNotes");

        public ObservableCollection<Note> NotesPlayed {
            get { return _PlayedNotes; }
            set {
                _PlayedNotes = value;
                NotifyPropertyChanged(_PlayedNotes_ChangedEventArgs);
            }
        }
        #endregion


    }
}
