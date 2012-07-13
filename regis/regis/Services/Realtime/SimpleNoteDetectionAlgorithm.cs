using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Plugins.Models;
using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using System.Threading;
using Regis.Models;

namespace Regis.Services.Realtime
{
    [Export(typeof(INoteDetectionSource))]
    [Export(typeof(INoteDetectionService))]
    public class SimpleNoteDetectionAlgorithm : INoteDetectionSource, INoteDetectionService
    {
        private ConcurrentQueue<Note[]> _noteQueue = new ConcurrentQueue<Note[]>();
        public ConcurrentQueue<Note[]> NoteQueue { get { throw new NotImplementedException(); }}

        [Import]
        private IFFTSource _fftSource = null;

        private Thread _noteDetectionThread;
        private bool _stopDetecting;

        public void Start(SimpleNoteDetectionArgs args)
        {
            if (_noteDetectionThread != null)
                return;

            _stopDetecting = false;

            _noteDetectionThread = new Thread(new ThreadStart(DetectNotes));
            _noteDetectionThread.Start();
        }

        public void Stop()
        {
            if (_noteDetectionThread == null)
                return;

            _stopDetecting = true;
            _noteDetectionThread.Join();
        }

        private void DetectNotes()
        {
            while (!_stopDetecting)
            {
                FFTCalculation fftCalc;
                _fftSource.FFTQueue.TryDequeue(out fftCalc);

                if (fftCalc == null)
                    continue;

                double[] powerBins = fftCalc.PowerBins;

                // TODO: Perform calculation

                Note[] notes = new Note[6];
                _noteQueue.Enqueue(notes);
            }
        }

        
    }
}
