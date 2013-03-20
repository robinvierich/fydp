using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Plugins.Interfaces;
using System.ComponentModel.Composition;
using System.Threading;
using Regis.Plugins.Models;

namespace Regis.Services.Realtime.Impl
{
    public class NoteDetection: INoteDetectionService, INoteDetectionSource, IPartImportsSatisfiedNotification
    {

        [Import]
        IFFTSource _fftSource;

        Thread _noteDetectionThread;
        bool _runThread = false;


        public NoteDetection() {
            _noteDetectionThread = new Thread(NoteDetectionThread);
        }

       

        private void NoteDetectionThread() {
            FFTPower prevfftPower = null;
            double[] diffPowerBins = null;
           
            while (_runThread) {
                FFTPower fftPower;
                if (!_fftSource.FFTQueue.TryDequeue(out fftPower))
                    continue;

                if (diffPowerBins == null)
                    diffPowerBins = new double[fftPower.PowerBins.Length];

                if (prevfftPower == null)
                    continue;




                prevfftPower = fftPower;
            }

        }

        public void Start(SimpleNoteDetectionArgs args) {
            if (_fftSource == null)
                throw new Exception("FFT source is null");

            _runThread = true;
            _noteDetectionThread.Start();
        }

        public void Stop() {
            _runThread = false;
            _noteDetectionThread.Join();
        }

        private void Raise_NotesDetected(Note[] notes) {
            EventHandler<NotesDetectedEventArgs> h = NotesDetected;
            if (h == null) return;

            h(this, new NotesDetectedEventArgs(notes));
        }

        public event EventHandler<NotesDetectedEventArgs> NotesDetected;

        public void OnImportsSatisfied() {
            throw new NotImplementedException();
        }
    }
}
