using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Plugins.Interfaces;
using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using System.Threading;
using Regis.Plugins.Models;

namespace Regis.Services.Realtime.Impl
{
    //[Export(typeof(INoteDetectionSource))]
    //[Export(typeof(INoteDetectionService))]
    public class SimpleNoteDetectionAlgorithm : INoteDetectionSource, INoteDetectionService
    {
        private ConcurrentQueue<Note[]> NoteQueue { get; set; }

        [Import]
        private IFFTSource _fftSource = null;

        private Thread _noteDetectionThread;
        private bool _stopDetecting;

        private double _sampleRate = AudioCapture.AudioCaptureSettings.SampleRate;
        private double _samples = AudioCapture.AudioCaptureSettings.BufferSize;
        private double _step;
        private double _noiseFloor = AudioCapture.AudioCaptureSettings.NoiseFloor;//500000000000000;

        public void Start(SimpleNoteDetectionArgs args)
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
                FFTPower fftCalc;
                if (!_fftSource.FFTQueue.TryPeek(out fftCalc))
                    continue;

                double[] powerBins = fftCalc.PowerBins;
                double freq = 0;

                //Start Note Detection
                double currentTotalPower = powerBins.Sum();
                //if (currentTotalPower < _previousTotalPower) // if our power is less than thy previous power by at least MAX_POWER_DECAY, ignore the rest
                //{
                //    freq = 0;
                //}
                //else
                //{
                freq = DetectFrequency_Mirrored(powerBins);
                //}
                //Console.WriteLine(freq);
                _previousTotalPower = currentTotalPower;

                Note[] notes = new Note[1] { new Note() };
                notes[0].startTime = DateTime.Now;
                notes[0].frequency = freq;
               
                //Console.WriteLine(notes[0].closestRealNoteFrequency);
                NoteQueue.Enqueue(notes);
                if (NoteQueue.Count > _maxNoteQueueSize)
                {
                    Note[] dqnotes;
                    NoteQueue.TryDequeue(out dqnotes);
                }
            }
        }


        //private double CalcFreq(double[] inputArray)
        //{
        //    bool foundMax = false;
        //    double sum = 0;
        //    double freq = 0;

        //    int lowIdx = -1;
        //    int highIdx = -1;

        //    for (int i = 1; i < inputArray.Length; i++)
        //    {
        //        double item = inputArray[i];
        //        double prevItem = inputArray[i - 1];

        //        if (item < _noiseFloor)
        //            continue;

        //        if (lowIdx == -1)
        //            lowIdx = i;

        //        sum += item;
        //        if (foundMax == false && item <= prevItem)
        //        {
        //            foundMax = true;
        //        }

        //        if (foundMax == true && item >= prevItem)
        //        {
        //            highIdx = i;
        //            break;
        //        }
        //    }

        //    if (lowIdx == -1 || highIdx == -1)
        //        return 0;

        //    for (int i = lowIdx; i <= highIdx; i++)
        //    {
        //        freq += (inputArray[i] / sum) * (i * _step);
        //    }

        //    return freq;
        //}

        private double DetectFrequency_Mirrored(double[] inputArray)
        {
            Tuple<int, int> index = MinMaxCalc(inputArray);

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


            return freq;
        }

        private Tuple<int, int> MinMaxCalc(double[] inputArray)
        {
            double max = 0;
            double min = 0;
            int maxIndex = 0;
            int minIndex = 0;

           // _noiseFloor = 1000 * inputArray[0];

            for (int i = 0; i < _samples; i++)
            {
                double element = inputArray[i];

                if (element < _noiseFloor)
                    element = 0;

                if (element >= max)
                {
                    max = element;
                    min = element;
                    maxIndex = i;
                }
                else if (element < min)
                {
                    min = element;
                    minIndex = i;
                }
                else
                {
                    break;
                }
            }

            return new Tuple<int, int>(maxIndex, minIndex);
        }

        public Note[] GetNotes() {
            Note[] toReturn;
            NoteQueue.TryPeek(out toReturn);
            return toReturn;
        }
    }
}
