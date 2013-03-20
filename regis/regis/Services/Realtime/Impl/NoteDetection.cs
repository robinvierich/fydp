using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Plugins.Interfaces;
using System.ComponentModel.Composition;
using System.Threading;
using Regis.Plugins.Models;
using Regis.AudioCapture;

namespace Regis.Services.Realtime.Impl
{
    //[Export(typeof(INoteDetectionService))]
    //[Export(typeof(INoteDetectionSource))]
    public class NoteDetection: INoteDetectionService, INoteDetectionSource
    {

        [Import]
        IFFTSource _fftSource;

        Thread _noteDetectionThread;
        bool _runThread = false;


        public NoteDetection() {
            _noteDetectionThread = new Thread(NoteDetectionThread);
        }


        private double GetFrequency(double[] fftBins, int index){
            int sampleRate = AudioCaptureSettings.SampleRate / AudioCapture.AudioCaptureSettings.BufferSkip;

            double freqPerBin = sampleRate / fftBins.Length;

            int startIndex = Math.Max(index - 10, 0);
            int endIndex = Math.Min(index + 10, fftBins.Length);

            double sum = 0;
            double freq = 0;

           for (int i = startIndex; i < endIndex; i++) {
               sum += fftBins[i];
           }

           for (int i = startIndex; i < endIndex; i++) {
               freq += (fftBins[i] / sum) * (i * freqPerBin);
           }

           return Math.Max(freq, 0);
        }

        private int[] FindPeakIndicesAndRemoveHarmonics(ref double[] fftBins) {
            List<int> peakIndices = new List<int>();

            double differenceThreshold = 10;

            for (int i = 1; i < fftBins.Length; i++) {

                // find peak
                if (fftBins[i] - fftBins[i - 1] > differenceThreshold) {
                    peakIndices.Add(i);

                    // remove harmonics
                    for ( int j = i * 2; j < fftBins.Length; j = j * 2){
                        fftBins[j] = 0;
                    }
                }
            }

            return peakIndices.ToArray();
        }



        private void NoteDetectionThread() {
            //FFTPower prevfftPower = null;

            double[] powerBins = null;
            double[] prevPowerBins = null;
            double[] diffPowerBins = null;

            bool[] impulseDetectedBins = null;

            double startImpulseThreshold = 1e18;
            double endImpulseThreshold = 1e17;
           
            while (_runThread) {
                FFTPower fftPower;
                if (!_fftSource.FFTQueue.TryPeek(out fftPower))
                    continue;

                powerBins = fftPower.PowerBins;

                if (diffPowerBins == null)
                    diffPowerBins = new double[fftPower.PowerBins.Length];

                if (impulseDetectedBins == null)
                    impulseDetectedBins = new bool[fftPower.PowerBins.Length];

                if (prevPowerBins == null) {
                    prevPowerBins = powerBins;
                    continue;
                }

                List<Note> notes = new List<Note>();


                int[] peakIndices = FindPeakIndicesAndRemoveHarmonics(ref powerBins);

                for (int i = 0; i < powerBins.Length; i++) {
                    double diff = powerBins[i] - prevPowerBins[i];

                    if (diff > startImpulseThreshold && !impulseDetectedBins[i]) {

                        double frequency = GetFrequency(powerBins, i);
                        if (frequency > 1600)
                            continue;

                        impulseDetectedBins[i] = true;
                        notes.Add(new Note() { startTime = DateTime.Now, frequency = frequency});
                    }

                    if (impulseDetectedBins[i] && diff < endImpulseThreshold) {
                        impulseDetectedBins[i] = false;
                    }
                }

                if (notes.Count > 0)
                    Raise_NotesDetected(notes.ToArray());

                prevPowerBins = powerBins;
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

        
    }
}
