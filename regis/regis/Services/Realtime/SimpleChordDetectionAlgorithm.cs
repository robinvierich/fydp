using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Plugins.Interfaces;
using Regis.Plugins.Models;
using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using System.Threading;
using Regis.Models;

namespace Regis.Services.Realtime
{
    [Export(typeof(IChordDetectionSource))]
    [Export(typeof(IChordDetectionService))]
    public class SimpleChordDetectionAlgorithm : IChordDetectionSource, IChordDetectionService
    {
        private ConcurrentQueue<Note[]> _chordQueue = new ConcurrentQueue<Note[]>();
        public ConcurrentQueue<Note[]> ChordQueue { get { return _chordQueue; } }

        [Import]
        private IFFTSource _fftSource = null;

        private Thread _noteDetectionThread;
        private bool _stopDetecting;

        private double _sampleRate = AudioCapture.AudioCaptureSettings.SampleRate;
        private double _samples = AudioCapture.AudioCaptureSettings.BufferSize;
        private double _step;
        private double _noiseFloor = AudioCapture.AudioCaptureSettings.NoiseFloor;//500000000000000;

        public void Start(SimpleChordDetectionArgs args)
        {
            if (_noteDetectionThread != null)
                return;

            _stopDetecting = false;
            _step = _sampleRate / _samples;

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

        private int _maxNoteQueueSize = 5;

        private double _previousTotalPower;
        private const double MAX_POWER_DECAY = 100000;

        private void DetectNotes()
        {
            while (!_stopDetecting)
            {
                FFTCalculation fftCalc;
                if (!_fftSource.FFTQueue.TryPeek(out fftCalc))
                    continue;

                double[] powerBins = fftCalc.PowerBins;
                double freq = 0;
                int startIndex = 0;

                List<Note> _noteList = new List<Note>();
                while (startIndex + 2 <= _samples)
                {
                    Tuple<double, int> data = DetectFrequency_Mirrored(powerBins, startIndex);
                    freq = data.Item1;
                    startIndex = data.Item2;

                    Note note = new Note();
                    note.timeStamp = DateTime.Now;
                    note.frequency = freq;
                    note.closestRealNoteFrequency = Regis.Plugins.Statics.NoteDictionary.GetClosestRealNoteFrequency(freq);
                    _noteList.Add(note);
                    if (startIndex < 0)
                        break;
                }
                Note[] notes = _noteList.ToArray<Note>();
                //Console.WriteLine(notes[0].closestRealNoteFrequency);

                Console.WriteLine(notes[0].frequency);
                _chordQueue.Enqueue(notes);
                if (_chordQueue.Count > _maxNoteQueueSize)
                {
                    Note[] dqnotes;
                    _chordQueue.TryDequeue(out dqnotes);
                }
            }
        }

        private Tuple<double, int> DetectFrequency_Mirrored(double[] inputArray, int startIndex)
        {
            Tuple<int, int, int> index = MinMaxCalc(inputArray, startIndex);
            startIndex = index.Item3;

            if (startIndex > _samples)
                return new Tuple<double, int>(0, startIndex);

            double sum = 0;
            double freq = 0;
            int minIndex = index.Item2 - index.Item1;
            int maxIndex = index.Item2;

            for (int i = Math.Max(minIndex, 0); i < maxIndex; i++)
            {
                sum += inputArray[i];
            }

            for (int i = Math.Max(minIndex, 0); i < maxIndex; i++)
            {
                freq += (inputArray[i] / sum) * (i * _step);
            }


            return new Tuple<double, int>(freq, startIndex);
        }

        private Tuple<int, int, int> MinMaxCalc(double[] inputArray, int startIndex)
        {
            double max = 0;
            double min = 0;
            int maxIndex = startIndex;
            int minIndex = startIndex;

           // _noiseFloor = 1000 * inputArray[0];

            for (int i = startIndex; i < _samples; i++)
            {
                if (i < 0)
                {
                    startIndex = Convert.ToInt32(_samples + 1);
                    break;
                }

                double element = inputArray[i];

                if (element < _noiseFloor)
                    element = 0;

                if (element >= max)
                {
                    max = element;
                    min = element;
                    maxIndex = i;
                    startIndex = i;
                }
                else if (element < min)
                {
                    min = element;
                    minIndex = i;
                    startIndex = i;                    
                }
                else
                {
                    startIndex = i;
                    break;
                }
            }

            
            return new Tuple<int, int, int>(maxIndex, minIndex, startIndex);
        }
    }
}
