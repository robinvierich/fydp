using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Base.ViewModels;
using System.ComponentModel.Composition;
using Regis.Plugins.Interfaces;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using Regis.Plugins.Models;
using System.ComponentModel;
using System.Windows.Media;

namespace RegisTrainingPlugin
{
    [Export]
    public class TrainingViewModel : BaseViewModel, IPartImportsSatisfiedNotification
    {
        [Import]
        INoteDetectionSource _noteSource = null;

        private static Color goalNoteColor = Color.FromArgb(100, 0, 0, 0);

        [Import]
        IFeedbackService _feedbackService = null;


        DispatcherTimer _feedbackTimer;

        [Import]
        IAchievementService _achievement;

        public void OnImportsSatisfied() {
            //_noteSource.NotesDetected += new EventHandler<NotesDetectedEventArgs>(_noteSource_NotesDetected);
        }

        int lastGoalNoteIdx = 0;

        void _noteSource_NotesDetected(object sender, NotesDetectedEventArgs e) {
            if (CurrentGoalNote == null) return;

            int idx = GoalNotes.IndexOf(CurrentGoalNote);

            for (int i = Notes.Count - 1; i > lastGoalNoteIdx; i--) {
                Notes.RemoveAt(i);
            }
            
            foreach (Note n in e.Notes) {
                Notes.Add(new Note() { Semitone = n.Semitone, startTime = CurrentTime });

                if (n.Semitone == CurrentGoalNote.Semitone) {
                    CurrentGoalNote.NoteBrush = Brushes.Green;
                    Notes.Last().NoteBrush = Brushes.Green;
                    if (idx >= GoalNotes.Count - 1) {
                        Stop();
                        return;
                    }

                    CurrentGoalNote = GoalNotes[idx + 1];
                }

                //PlayedNotes.Add(n);
                //Notes.Add(n);
            }

            

        }

        public TrainingViewModel() {
            PlayedNotes = new ObservableCollection<Note>();
            GoalNotes = new ObservableCollection<Note>();
            Notes = new ObservableCollection<Note>();

            _feedbackTimer = new DispatcherTimer() {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _feedbackTimer.Tick += new EventHandler(_feedbackTimer_Tick);
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

        private ObservableCollection<Note> GetTwinkleTwinkleNotes() {
            ObservableCollection<Note> notes = new ObservableCollection<Note>();

            DateTime t = DateTime.Now + TimeSpan.FromSeconds(0.5);

            // semitone 48 == C4
            notes.Add(new Note() { Semitone = 48, startTime = t, NoteBrush = new SolidColorBrush(goalNoteColor) });
            t += TimeSpan.FromSeconds(0.5);

            // semitone 48 == C4
            notes.Add(new Note() { Semitone = 48, startTime = t, NoteBrush = new SolidColorBrush(goalNoteColor) });
            t += TimeSpan.FromSeconds(0.5);

            // semitone 55 == G4
            notes.Add(new Note() { Semitone = 55, startTime = t, NoteBrush = new SolidColorBrush(goalNoteColor) });
            t += TimeSpan.FromSeconds(0.5);

            // semitone 55 == G4
            notes.Add(new Note() { Semitone = 55, startTime = t, NoteBrush = new SolidColorBrush(goalNoteColor) });
            t += TimeSpan.FromSeconds(0.5);

            // semitone 57 == A5
            notes.Add(new Note() { Semitone = 57, startTime = t, NoteBrush = new SolidColorBrush(goalNoteColor) });
            t += TimeSpan.FromSeconds(0.5);

            // semitone 57 == A5
            notes.Add(new Note() { Semitone = 57, startTime = t, NoteBrush = new SolidColorBrush(goalNoteColor) });
            t += TimeSpan.FromSeconds(0.5);

            // semitone 48 == C4
            notes.Add(new Note() { Semitone = 48, startTime = t, NoteBrush = new SolidColorBrush(goalNoteColor) });
            t += TimeSpan.FromSeconds(0.5);

            return notes;
        }

        private int addedHandlers = 0;

        public void Start() {

            if (addedHandlers > 0) {
                _noteSource.NotesDetected -= _noteSource_NotesDetected;
                addedHandlers--;
            }

            StartTime = DateTime.Now;
            CurrentTime = DateTime.Now;

            Notes.Clear();
            PlayedNotes.Clear();



            GoalNotes = GetTwinkleTwinkleNotes();

            lastGoalNoteIdx = GoalNotes.Count - 1;

            CurrentGoalNote = GoalNotes[0];

            foreach (Note goalNote in GoalNotes) {
                Notes.Add(goalNote);
            }


            _feedbackTimer.Start();

            _noteSource.NotesDetected += new EventHandler<NotesDetectedEventArgs>(_noteSource_NotesDetected);
            addedHandlers++;
        }

        private Note _CurrentGoalNote;
        private Note CurrentGoalNote {
            get { return _CurrentGoalNote; }
            set {
                _CurrentGoalNote = value;
                CurrentTime = value.startTime;
            }
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

        public void Stop() {
            _feedbackTimer.Stop();

            _noteSource.NotesDetected -= _noteSource_NotesDetected;
            addedHandlers--;
        }

        #region StartTrainingCommand
        private StartTrainingCommand _StartTrainingCommand = new StartTrainingCommand();
        public StartTrainingCommand StartTrainingCommand {
            get { return _StartTrainingCommand; }
        }
        #endregion

        #region StopTrainingCommand
        private StopTrainingCommand _StopTrainingCommand = new StopTrainingCommand();
        public StopTrainingCommand StopTrainingCommand {
            get { return _StopTrainingCommand; }
        }
        #endregion

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


    }
}
