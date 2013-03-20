using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Regis.Plugins.Interfaces;
using Regis.Plugins.Models;
using System.Threading;
using System.Collections.Concurrent;
using Regis.Plugins.Statics;
using System.Windows.Threading;

namespace Regis.Services.Realtime.Impl
{
    //[Export(typeof(INoteDetectionSource))]
    //[Export(typeof(INoteDetectionService))]
    public class OffTuneNoteService : INoteDetectionSource, INoteDetectionService
    {
        List<double> validFreqs = NoteDictionary.NoteDict.Keys.ToList();

        ConcurrentQueue<Note> _noteQueue = new ConcurrentQueue<Note>();
        private bool _detecting = false;

        private Thread _noteThread;

        public OffTuneNoteService() {
            _noteThread = new Thread(NoteThread);
            _noteThread.SetApartmentState(ApartmentState.STA);
        }

        int i = -12;
        public void NoteThread() {
            while (_detecting) {

                i++;
                if (i > 12)
                    i = -12;

                Note note = new Note() { frequency = 440 + i, startTime = DateTime.Now, endTime = DateTime.Now + TimeSpan.FromSeconds(0.1) };

                Raise_NotesDetected(new Note[1] { note });

                Thread.Sleep(50);
            }
        }

        public void Start(SimpleNoteDetectionArgs args) {
            Stop();
            _detecting = true;
            _noteThread.Start();
        }

        public void Stop() {
            _detecting = false;
            if (_noteThread.IsAlive)
                _noteThread.Join();
        }

        private void Raise_NotesDetected(Note[] notes) {
            EventHandler<NotesDetectedEventArgs> h = NotesDetected;
            if (h == null) return;

            h(this, new NotesDetectedEventArgs(notes));
        }

        public event EventHandler<NotesDetectedEventArgs> NotesDetected;
    }
}
