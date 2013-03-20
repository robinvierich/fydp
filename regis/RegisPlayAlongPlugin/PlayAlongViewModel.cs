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
using System.Windows.Threading;
using System.Windows.Media;

namespace RegisPlayAlongPlugin
{
    [Export]
    public class PlayAlongViewModel : BaseViewModel 
    {
        [Import]
        INoteDetectionSource _noteSource = null;

        private static Color goalNoteColor = Color.FromArgb(100, 0, 0, 0);

        [Import]
        IFeedbackService _feedbackService = null;

        DispatcherTimer _timer;
        DispatcherTimer _feedbackTimer;

        [Import]
        IAchievementService _achievement;


        public PlayAlongViewModel() {
            PlayedNotes = new ObservableCollection<Note>();
            GoalNotes = new ObservableCollection<Note>();
            Notes = new ObservableCollection<Note>();


            _timer = new DispatcherTimer() {
                Interval = TimeSpan.FromMilliseconds(20)
            };
            _timer.Tick += new EventHandler(_timer_Tick);

            _feedbackTimer = new DispatcherTimer() {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _feedbackTimer.Tick += new EventHandler(_feedbackTimer_Tick);
        }

        void _timer_Tick(object sender, EventArgs e) {
            CurrentTime = DateTime.Now;
        }

        void _feedbackTimer_Tick(object sender, EventArgs e) {
            if (_feedbackService == null) return;

            Dictionary<Note, List<Feedback>> noteFeedback = _feedbackService.GetFeedback(PlayedNotes, GoalNotes);


            foreach (KeyValuePair<Note, List<Feedback>> kvp in noteFeedback) {

                Note note = kvp.Key as Note;
                List<Feedback> feedbackList = kvp.Value;

                if (feedbackList.OfType<BadPitchFeedback>().Count() > 0) {
                    note.NoteBrush = Brushes.Red;
                    continue;
                } 
                

                if (feedbackList.OfType<GoodTimingFeedback>().Count() > 0) {
                    note.NoteBrush = Brushes.Green;
                } else if (feedbackList.OfType<MediumTimingFeedback>().Count() > 0) {
                    note.NoteBrush = Brushes.Yellow;
                } else if (feedbackList.OfType<BadTimingFeedback>().Count() > 0) {
                    note.NoteBrush = Brushes.Red;
                }
            }
        }

        public void Start() {
            StartTime = DateTime.Now;
            CurrentTime = DateTime.Now;

            Notes.Clear();
            PlayedNotes.Clear();
            GoalNotes = GetTwinkleTwinkleNotes();

            foreach (Note goalNote in GoalNotes)
            {
                Notes.Add(goalNote);
            }

            _timer.Start();
            _feedbackTimer.Start();

            _noteSource.NotesDetected += new EventHandler<NotesDetectedEventArgs>(_noteSource_NotesDetected);
        }

        public void Stop() {
            _timer.Stop();
            _feedbackTimer.Stop();
            _noteSource.NotesDetected -= _noteSource_NotesDetected;
        }

        void _noteSource_NotesDetected(object sender, NotesDetectedEventArgs e) {
            foreach (Note n in e.Notes) {
                PlayedNotes.Add(n);
                Notes.Add(n);
            }
        }
        private ObservableCollection<Note> GetTwinkleTwinkleNotes(){ 
            ObservableCollection<Note> notes = new ObservableCollection<Note>();

            DateTime t = DateTime.Now;

            // semitone 48 == C4
            // semitone 48 == C4
            t += TimeSpan.FromSeconds(0.5);

            // semitone 48 == C4
            // semitone 48 == C4
            t += TimeSpan.FromSeconds(0.5);

            // semitone 55 == G4
            // semitone 55 == G4
            t += TimeSpan.FromSeconds(0.5);

            // semitone 55 == G4
            // semitone 55 == G4
            t += TimeSpan.FromSeconds(0.5);

            // semitone 57 == A5
            // semitone 57 == A5
            t += TimeSpan.FromSeconds(0.5);

            // semitone 57 == A5
            // semitone 57 == A5
            t += TimeSpan.FromSeconds(0.5);

            // semitone 48 == C4
            // semitone 48 == C4
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



            PlayedNotes.Clear();
            GoalNotes = GetTwinkleTwinkleNotes();

            _timer.Start();
            _feedbackTimer.Start();

            _noteSource.NotesDetected += new EventHandler<NotesDetectedEventArgs>(_noteSource_NotesDetected);
        }
        //PlayedFirstSongAchievement achievement; 
        public void Stop() {
            _timer.Stop();
            _feedbackTimer.Stop();
            _noteSource.NotesDetected -= _noteSource_NotesDetected;
            _achievement.SetAchievement(new PlayedFirstSongAchievement());
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


        #region Notes
        private ObservableCollection<Note> _Notes;
        private static PropertyChangedEventArgs _Notes_ChangedEventArgs = new PropertyChangedEventArgs("Notes");

        public ObservableCollection<Note> Notes {
            get { return _Notes; }
            private set {
                _Notes = value;
                NotifyPropertyChanged(_Notes_ChangedEventArgs);
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
