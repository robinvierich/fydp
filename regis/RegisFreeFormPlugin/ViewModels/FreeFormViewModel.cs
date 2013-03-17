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
    class FreeFormViewModel : BaseViewModel
    {
        [Import]
        private INoteDetectionSource _noteSource;
        private DispatcherTimer _noteTimer;

        private double[] _frequencies;
        public FreeFormViewModel() {
            _frequencies = NoteDictionary.NoteDict.Keys.ToArray<double>();
            _noteTimer = new DispatcherTimer() {
                Interval = TimeSpan.FromMilliseconds(20)
            };

            _noteTimer.Tick += new EventHandler(_noteTimer_Tick);

            NotesPlayed = new ObservableCollection<Note>();
        }

        void _noteTimer_Tick(object sender, EventArgs e) {
            foreach (Note note in _noteSource.GetNotes()) {
                NotesPlayed.Add(note);
            }
        }

        internal void Start() {
            _noteTimer.Start();
        }


        internal void Stop() {
            if (_noteTimer.IsEnabled)
                _noteTimer.Stop();
        }

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

        [DataMember]
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
