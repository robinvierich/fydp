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

        private double _sampleRate = AudioCapture.AudioCapture.SampleRate;
        private double _samples = AudioCapture.AudioCapture.BufferSize;
        private double _step;
        private double _noiseFloor = 0;

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

        private void DetectNotes()
        {
            while (!_stopDetecting)
            {
                FFTCalculation fftCalc;
                if (!_fftSource.FFTQueue.TryPeek(out fftCalc))
                    continue;

                double[] powerBins = fftCalc.PowerBins;

                //Start Note Detection
                double freq = NoteCalc(powerBins);
                //End Note Detection

                Note[] notes = new Note[1];
                notes[0].timeStamp = DateTime.Now;
                notes[0].frequency = freq;
                notes[0].closestRealNoteFrequency = Regis.Plugins.Statics.NoteDictionary.GetClosestRealNoteFrequency(freq);

                //Console.WriteLine(notes[0].closestRealNoteFrequency);

                _noteQueue.Enqueue(notes);
            }
        }


        //bool foundMax = false;
        //    for (int i = 1; i < inputArray.Length; i++)
        //    {
        //        double item = inputArray[i];
        //        double prevItem = inputArray[i - 1];

        //        if (item - _noiseFloor < 0)
        //            continue;

        //        sum += item;
        //        if (foundMax == false && item <= prevItem)
        //        {
        //            foundMax = true;
        //        }

        //        if (foundMax == true && item >= prevItem)
        //        {
        //            break;
        //        }
        //    }


        private double NoteCalc(double[] inputArray)
        {
            Tuple<int, int> index = MinMaxCalc(inputArray);

            double sum = 0;
            double freq = 0;
            int minIndex = index.Item2 - index.Item1;
            int maxIndex = index.Item2;

            for (int i = minIndex; i < maxIndex; i++)
            {
                sum += inputArray[i];
            }

            for (int i = minIndex; i < maxIndex; i++)
            {
                freq += (inputArray[i] / sum) * (i * _step);
            }

            Console.WriteLine(freq);
            return freq;
        }

        private Tuple<int, int> MinMaxCalc(double[] inputArray)
        {
            double max = 0;
            double min = 0;
            int maxIndex = 0;
            int minIndex = 0;

            _noiseFloor = 100 * inputArray[0];

            for (int i = 0; i < _samples; i++)
            {
                if (inputArray[i] < _noiseFloor)
                    continue;

                if (inputArray[i] >= max)
                {
                    max = inputArray[i];
                    min = inputArray[i];
                    maxIndex = i;
                }
                else if (inputArray[i] <= min)
                {
                    min = inputArray[i];
                    minIndex = i;
                }
                else
                {
                    break;
                }
            }

            return new Tuple<int, int>(maxIndex, minIndex);
        }

        
    }
}
