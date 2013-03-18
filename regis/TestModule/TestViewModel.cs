using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Plugins.Interfaces;
using System.ComponentModel.Composition;
using Regis.Plugins.Models;
using System.Collections.ObjectModel;
using Regis.Base.ViewModels;
using System.Windows.Threading;



namespace TestModule
{
    [Export]
    public class TestViewModel : BaseViewModel, IPartImportsSatisfiedNotification
    {
        [Import]
        private INoteDetectionSource _ns;

        public TestViewModel() {
            Notes = new ObservableCollection<Note>();
        }

        private ObservableCollection<Note> _Notes;
        public ObservableCollection<Note> Notes {
            get {
                return _Notes;
            }
            set {
                _Notes = value;
                NotifyPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Notes"));
            }
        }

        public void _ns_NotesDetected(object sender, NotesDetectedEventArgs e) {
            foreach (Note note in e.Notes) {
                this.Notes.Add(note);
            }
        }

        public void OnImportsSatisfied() {
            _ns.NotesDetected += new EventHandler<NotesDetectedEventArgs>(_ns_NotesDetected);
        }
    }
}
