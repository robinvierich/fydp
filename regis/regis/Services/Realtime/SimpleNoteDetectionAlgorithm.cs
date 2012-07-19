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

        private double sps = 6857;
        private double samples = 1024;
        private double step;

        public void Start(SimpleNoteDetectionArgs args)
        {
            if (_noteDetectionThread != null)
                return;

            _stopDetecting = false;
            step = 2 * sps / samples;

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

                //Start Note Detection
                double freq = NoteCalc(powerBins);
                //End Note Detection

                Note[] notes = new Note[1];
                notes[0]._timeStamp = DateTime.Now;
                notes[0]._frequency = freq;

                _noteQueue.Enqueue(notes);
            }
        }

        private double NoteCalc(double[] inputArray)
        {
            //Tuple<int, int> index = MinMaxCalc(inputArray);

            int width = 10;

            double max = inputArray.Max();

            int highIdx = Array.IndexOf(inputArray, max) + width/2;
            int lowIdx = highIdx - (width / 2);

            lowIdx = Math.Max(0, lowIdx);
            highIdx = Math.Min(256, highIdx);

            double sum = 0;
            double freq = 0;

            for (int i = lowIdx; i < highIdx; i++)
            {
                sum += inputArray[i];
            }

            for (int i = lowIdx; i < highIdx; i++)
            {
                freq += (inputArray[i] / sum) * (i * step);
            }

            Console.WriteLine(freq);
            return freq;
        }

        //private Tuple<int, int> MinMaxCalc(double[] inputArray)
        //{
        //    double max = 0;
        //    double min = 0;
        //    int maxIndex = 0;
        //    int minIndex = 0;

        //    for (int i = 0; i < 256; i++)
        //    {
        //        if (inputArray[i] >= max)
        //        {
        //            max = inputArray[i];
        //            min = inputArray[i];
        //            maxIndex = i;
        //        }
        //        else if (inputArray[i] <= min)
        //        {
        //            min = inputArray[i];
        //            minIndex = i;
        //        }
        //        else
        //        {
        //            break;
        //        }
        //    }

        //    return new Tuple<int, int>(maxIndex, minIndex);
        //}

        
    }
}
