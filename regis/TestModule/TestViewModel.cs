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

        private DispatcherTimer timer;

        public TestViewModel() {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += new EventHandler(timer_Tick);
            Notes = new ObservableCollection<Note>();
        }

        void timer_Tick(object sender, EventArgs e) {
            Note[] notes = _ns.GetNotes();
            foreach (Note note in notes) {
                this.Notes.Add(note);
            }
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


        public void OnImportsSatisfied() {
            timer.Start();
        }
    }
}
