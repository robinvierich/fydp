using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Base.ViewModels;
using System.Collections.ObjectModel;
using Regis.Plugins.Models;
using System.ComponentModel;
using System.ComponentModel.Composition;
using Regis.Plugins.Interfaces;

namespace RegisPlayAlongPlugin
{
    [Export]
    public class PlayAlongViewModel : BaseViewModel 
    {
        [Import]
        INoteDetectionSource _noteSource;

        public PlayAlongViewModel() {
            PlayedNotes = new ObservableCollection<Note>();
            GoalNotes = new ObservableCollection<Note>();
        }

        private ObservableCollection<Note> GetTwinkleTwinkleNotes(){ 
            ObservableCollection<Note> notes = new ObservableCollection<Note>();

            DateTime t = DateTime.Now;

            // semitone 48 == C4
            notes.Add(new Note() { Semitone = 48, startTime = t, endTime = t + TimeSpan.FromSeconds(0.1) });
            t += TimeSpan.FromSeconds(0.5);

            // semitone 48 == C4
            notes.Add(new Note() { Semitone = 48, startTime = t, endTime = t + TimeSpan.FromSeconds(0.1) });
            t += TimeSpan.FromSeconds(0.5);

            // semitone 55 == G4
            notes.Add(new Note() { Semitone = 55, startTime = t, endTime = t + TimeSpan.FromSeconds(0.1) });
            t += TimeSpan.FromSeconds(0.5);

            // semitone 55 == G4
            notes.Add(new Note() { Semitone = 55, startTime = t, endTime = t + TimeSpan.FromSeconds(0.1) });
            t += TimeSpan.FromSeconds(0.5);

            // semitone 57 == A5
            notes.Add(new Note() { Semitone = 57, startTime = t, endTime = t + TimeSpan.FromSeconds(0.1) });
            t += TimeSpan.FromSeconds(0.5);

            // semitone 57 == A5
            notes.Add(new Note() { Semitone = 57, startTime = t, endTime = t + TimeSpan.FromSeconds(0.1) });
            t += TimeSpan.FromSeconds(0.5);

            // semitone 48 == C4
            notes.Add(new Note() { Semitone = 48, startTime = t, endTime = t + TimeSpan.FromSeconds(0.1) });
            t += TimeSpan.FromSeconds(0.5);

            return notes;
        }



        #region StartPlayAlongCommand
        private StartPlayAlongCommand _StartPlayAlongCommand = new StartPlayAlongCommand();
        public StartPlayAlongCommand StartPlayAlongCommand {
            get { return _StartPlayAlongCommand; }
        }
        #endregion


        #region StopPlayAlongCommand
        private StopPlayAlongCommand _StopPlayAlongCommand = new StopPlayAlongCommand();
        public StopPlayAlongCommand StopPlayAlongCommand {
            get { return _StopPlayAlongCommand; }
        }
        #endregion


        public void Start() {
            _noteSource.NotesDetected += new EventHandler<NotesDetectedEventArgs>(_noteSource_NotesDetected);
            StartTime = DateTime.Now;
            CurrentTime = DateTime.Now;
            GoalNotes = GetTwinkleTwinkleNotes();
        }

        public void Stop() {
            _noteSource.NotesDetected -= _noteSource_NotesDetected;
            GoalNotes.Clear();
        }

        void _noteSource_NotesDetected(object sender, NotesDetectedEventArgs e) {
            foreach (Note n in e.Notes) {
                PlayedNotes.Add(n);
            }
        }

        #region GoalNotes
        private ObservableCollection<Note> _GoalNotes;
        private static PropertyChangedEventArgs _GoalNotes_ChangedEventArgs = new PropertyChangedEventArgs("GoalNotes");

        public ObservableCollection<Note> GoalNotes {
            get { return _GoalNotes; }
            set {
                _GoalNotes = value;
                NotifyPropertyChanged(_GoalNotes_ChangedEventArgs);
            }
        }
        #endregion

        #region PlayedNotes
        private ObservableCollection<Note> _PlayedNotes;
        private static PropertyChangedEventArgs _PlayedNotes_ChangedEventArgs = new PropertyChangedEventArgs("PlayedNotes");

        public ObservableCollection<Note> PlayedNotes {
            get { return _PlayedNotes; }
            set {
                _PlayedNotes = value;
                NotifyPropertyChanged(_PlayedNotes_ChangedEventArgs);
            }
        }
        #endregion

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
    }
}
