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
            //Random r = new Random();
            //int semitone = 60;//r.Next(36, 37);
            while (_detecting) {
                if (i > 12)
                    i = 0;

                int semitone = i + 48;
                i++;

                Note randomNote = new Note() { Semitone = semitone, startTime = DateTime.Now, endTime = DateTime.Now + TimeSpan.FromSeconds(0.1) };
                _noteQueue.Enqueue(randomNote);
                if (_noteQueue.Count > 10) {
                    Note note;
                    _noteQueue.TryDequeue(out note);
                }

                Thread.Sleep(10);
            }
        }

        public Note[] GetNotes() {
            Note note;
            _noteQueue.TryPeek(out note);
            if (note != null)
                return new Note[1] { note };
            else
                return new Note[0];
        }

        public void Start(SimpleNoteDetectionArgs args) {
            Stop();
            _detecting = true;
            _noteThread.Start(); 
        }

        public void Stop() {
            if (_noteThread.IsAlive)
                _noteThread.Join();
            _detecting = false;
        }
    }
}
