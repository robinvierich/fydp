using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Plugins.Interfaces;
using Regis.Plugins.Models;
using Regis.Plugins.Statics;
using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows.Threading;

namespace Regis.Services.Realtime.Impl
{
    [Export(typeof(INoteDetectionSource))]
    [Export(typeof(INoteDetectionService))]
    public class RandomNoteService : INoteDetectionSource, INoteDetectionService
    {
        List<double> validFreqs = NoteDictionary.NoteDict.Keys.ToList();
        private Thread _noteThread; 

        int i = 0;

        public RandomNoteService() {
            _noteThread = new Thread(NoteThread);
        }

        ConcurrentQueue<Note> _noteQueue = new ConcurrentQueue<Note>();
        private bool _detecting = false;

        public void NoteThread() {

            while (_detecting) {
                if (i > 12)
                    i = 0;

                int semitone = i + 48;
                i++;

                //Note randomNote = new Note() { Semitone = semitone, startTime = DateTime.Now, endTime = DateTime.Now + TimeSpan.FromSeconds(0.1) };
                //_noteQueue.Enqueue(randomNote);

                Note note = new Note() { Semitone = semitone, startTime = DateTime.Now, endTime = DateTime.Now + TimeSpan.FromSeconds(0.1) };
                //_noteQueue.TryDequeue(out note);

                Raise_NotesDetected(new Note[1] { note });

                Thread.Sleep(200);
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
