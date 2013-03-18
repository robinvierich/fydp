using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Base.ViewModels;
using System.Collections.ObjectModel;
using Regis.Plugins.Models;
using System.ComponentModel;
using System.ComponentModel.Composition;

namespace RegisPlayAlongPlugin
{
    [Export]
    public class PlayAlongViewModel : BaseViewModel 
    {
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
    }
}
