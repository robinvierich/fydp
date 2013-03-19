using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Plugins.Interfaces;
using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using System.Threading;
using Regis.Plugins.Models;
using Regis.Base;
using Regis.Plugins.Statics;

namespace Regis.Services.Realtime.Impl
{
    [Export(typeof(INoteDetectionSource))]
    [Export(typeof(INoteDetectionService))]
    public class SimpleNoteDetectionAlgorithm : INoteDetectionSource, INoteDetectionService
    {
        private ConcurrentQueue<Note[]> NoteQueue { get; set; }

        [Import]
        private IFFTSource _fftSource = null;

        private Thread _noteDetectionThread;
        private bool _stopDetecting;

        private double _sampleRate = AudioCapture.AudioCaptureSettings.SampleRate / AudioCapture.AudioCaptureSettings.BufferSkip;
        private double _samples = AudioCapture.AudioCaptureSettings.BufferSize * AudioCapture.AudioCaptureSettings.BufferModifier;
        private double _step;
        private double _noiseFloor = AudioCapture.AudioCaptureSettings.NoiseFloor;//500000000000000;

        private bool _impulseAlg = false;
        private double[] _powerBinsOld, _powerBinsDelta;
        private double _powerBinsDeltaCutoff = 0;
        private double _lastMax = 1;
        private double[] _xorPowerBins = new double[AudioCapture.AudioCaptureSettings.BufferSize * AudioCapture.AudioCaptureSettings.BufferModifier];

        public event EventHandler<NotesDetectedEventArgs> NotesDetected;

        public SimpleNoteDetectionAlgorithm()
        {
            NoteQueue = new ConcurrentQueue<Note[]>();
            _step = _sampleRate / _samples;
            _xorPowerBins = NoiseFilter();            
        }

        public void Start(SimpleNoteDetectionArgs args)
        {
            if (_noteDetectionThread != null)
                return;

            _stopDetecting = false;
            _step = _sampleRate / _samples;
            //if (_impulseAlg)
                //_noteDetectionThread = new Thread(new ThreadStart(ImpulseDetectNotes));
            //else
            _noteDetectionThread = new Thread(new ThreadStart(SimpleDetectNotesImproved));

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


        //Creates array of binary values to show which bins contain valid notes
        private double[] NoiseFilter()
        {
            int size = (int)(_samples / 2);
            double[] xorPowerBins = new double[size];
            for (int i = 0; i < size; i++)
            {
                if ((i == 0) || ((i * _step) < 70))
                    xorPowerBins[i] = 0;
                else if ((i * _step) <= NoteDictionary.GetClosestNoteFrequency(i * _step) && ((i + 1) * _step) >= NoteDictionary.GetClosestNoteFrequency(i * _step))
                    xorPowerBins[i] = 1;
                //else if (i == Math.Floor(NoteDictionary.GetClosestNoteFrequency(i * _step)) || i == Math.Ceiling(NoteDictionary.GetClosestNoteFrequency(i * _step)))
                //xorPowerBins[i] = 1;
                else
                    xorPowerBins[i] = 0;
            }
            return xorPowerBins;
        }

        //Creates array which will create an array which will remove all harmonic octaves from a root note.
        private double[] HarmonicOctaveFilter(double[] xorArray, double freq)
        {
            double rootNote = NoteDictionary.GetClosestNoteFrequency(freq);
            int rootIndex = (int)Math.Floor(freq / _step);

            for (int i = rootIndex * 2; i < xorArray.Length; i = i * 2)
            {
                if ((i * _step) <= NoteDictionary.GetClosestNoteFrequency(i * _step) && ((i + 1) * _step) >= NoteDictionary.GetClosestNoteFrequency(i * _step))
                {
                    xorArray[Math.Max(i - 1, 0)] = 0;
                    xorArray[i] = 0;
                    xorArray[Math.Min(i + 1, xorArray.Length - 1)] = 0;
                }
            }
            return xorArray;
        }



        private void Raise_NotesDetected(Note[] notes) {
            EventHandler<NotesDetectedEventArgs> h = NotesDetected;
            if (h == null) return;

            h(this, new NotesDetectedEventArgs(notes));
        }

        double freq = 0;
        double currentTotalPower;
        double[] xorPowerBins;
        double max;
        int index;
        private void SimpleDetectNotesImproved()
        {
            while (!_stopDetecting)
            {
                FFTPower fftCalc;
                if (!_fftSource.FFTQueue.TryPeek(out fftCalc))
                    continue;

                double[] powerBins = new double[AudioCapture.AudioCaptureSettings.BufferSize * AudioCapture.AudioCaptureSettings.BufferModifier];

                freq = 0;
                max = 0;
                index = 0;

                if (xorPowerBins == null)
                    xorPowerBins = new double[_xorPowerBins.Length];

                currentTotalPower = powerBins.Sum();
                Buffer.BlockCopy(_xorPowerBins, 0, xorPowerBins, 0, _xorPowerBins.Length * sizeof(double));

                for (int i = 0; i < (1500 / _step); i++)//(fftCalc.PowerBins.Length / 2); i++)
                {
                    powerBins[i] = (fftCalc.PowerBins[i]);// * xorPowerBins[i]);

                    if (powerBins[i] < _noiseFloor)
                    {
                        powerBins[i] = 0;
                    }

                    double _relativePower = powerBins[Math.Max(i - 1, 0)] + powerBins[Math.Max(i + 1, 0)] + powerBins[Math.Max(i, 0)];
                    if ((max < _relativePower))
                    {
                        max = _relativePower;
                        index = i;
                        //xorPowerBins = HarmonicOctaveFilter(xorPowerBins, index * _step);
                    }
                    powerBins[i] = powerBins[i] / fftCalc.PowerBins.Max() * 1000;
                    powerBins[powerBins.Length - 1 - i] = 0;
                }



                //_noiseFloor = 0.7 * _lastMax;


                //freq = DetectFrequency2(powerBins, index);
                freq = DetectFrequency_Mirrored(fftCalc.PowerBins, index - 3, index + 3);
                //System.Diagnostics.Debug.Write(freq + "\n");

                Note[] notes = new Note[1] { new Note() };
                notes[0].startTime = DateTime.Now;
                notes[0].frequency = freq;

                //Console.WriteLine(notes[0].closestRealNoteFrequency);

                Raise_NotesDetected(notes);

                //NoteQueue.Enqueue(notes);
                //if (NoteQueue.Count > _maxNoteQueueSize)
                //{
                //    Note[] dqnotes;
                //    NoteQueue.TryDequeue(out dqnotes);
                //}
            }
        }



        private double DetectFrequency2(double[] inputArray, int index = 0)
        {      
            double freq =(index * _step);

            return freq;
        }


        private double DetectFrequency_Mirrored(double[] inputArray, int minIndex = 0, int maxIndex = 0)
        {
            
            if ((maxIndex == 0) && (minIndex == 0))
            {
                Tuple<int, int> index = MinMaxCalc(inputArray);
                minIndex = index.Item1 - (index.Item2 - index.Item1);
                maxIndex = index.Item2;
            }

            double sum = 0;
            double freq = 0;    

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

            //_noiseFloor = 1000 * inputArray[0];

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

        //private void SimpleDetectNotes()
        //{
        //    while (!_stopDetecting)
        //    {
        //        FFTPower fftCalc;
        //        if (!_fftSource.FFTQueue.TryPeek(out fftCalc))
        //            continue;

        //        double[] powerBins = new double[AudioCapture.AudioCaptureSettings.BufferSize * AudioCapture.AudioCaptureSettings.BufferModifier];
        //        fftCalc.PowerBins.CopyTo(powerBins, 0);
        //        double freq = 0;

        //        //Start Note Detection
        //        double currentTotalPower = powerBins.Sum();
        //        //if (currentTotalPower < _previousTotalPower) // if our power is less than thy previous power by at least MAX_POWER_DECAY, ignore the rest
        //        //{
        //        //    freq = 0;
        //        //}/
        //        //else
        //        //{
        //        double max = 0;
        //        int index = 0;
        //        for (int i = 0; i < powerBins.Length / 2; i++)
        //        {
        //            powerBins[i] = powerBins[i] * _xorPowerBins[i];
        //            if (powerBins[i] < _noiseFloor)
        //                powerBins[i] = 0;
        //            if ((max < powerBins[i]) && (i > 75))
        //            {
        //                max = powerBins[i];
        //                index = i;
        //            }
        //            powerBins[powerBins.Length - 1 - i] = 0;
        //        }

        //        index = index;
        //        max = max;

        //        freq = DetectFrequency_Mirrored(powerBins);

                
        //        //}
        //        System.Diagnostics.Debug.Write(freq + "\n");
        //        _previousTotalPower = currentTotalPower;

        //        Note[] notes = new Note[1] { new Note() };
        //        notes[0].startTime = DateTime.Now;
        //        notes[0].frequency = freq;

        //        //Console.WriteLine(notes[0].closestRealNoteFrequency);
        //        NoteQueue.Enqueue(notes);
        //        if (NoteQueue.Count > _maxNoteQueueSize)
        //        {
        //            Note[] dqnotes;
        //            NoteQueue.TryDequeue(out dqnotes);
        //        }
        //    }
        //}
        //private void ImpulseDetectNotes()
        //{
        //    while (!_stopDetecting)
        //    {
        //        FFTPower fftCalc;
        //        if (!_fftSource.FFTQueue.TryPeek(out fftCalc))
        //            continue;

        //        double[] powerBins = new double[AudioCapture.AudioCaptureSettings.BufferSize * AudioCapture.AudioCaptureSettings.BufferModifier];
        //        _powerBinsDelta = new double[powerBins.Length];
        //        //fftCalc.PowerBins.CopyTo(powerBins, 0);

        //        if (_powerBinsOld == null)
        //        {
        //            _powerBinsOld = new double[AudioCapture.AudioCaptureSettings.BufferSize * AudioCapture.AudioCaptureSettings.BufferModifier];
        //            powerBins.CopyTo(_powerBinsOld, 0);
        //            continue;
        //        }

        //        double freq = 0;

        //        //Start Note Detection
        //        double currentTotalPower = powerBins.Sum();
        //        //if (currentTotalPower < _previousTotalPower) // if our power is less than thy previous power by at least MAX_POWER_DECAY, ignore the rest
        //        //{
        //        //    freq = 0;
        //        //}/
        //        //else
        //        //{

        //        double max = 0;
        //        int index = 0;

        //        for (int i = 0; i < (fftCalc.PowerBins.Length / 2); i++)
        //        {
        //            if (i < (80 / _step)) 
        //            {
        //                powerBins[i] = 0;
        //            }
        //            else  
        //            {
        //                powerBins[i] = powerBins[i] * _xorPowerBins[i];
        //                _powerBinsDelta[i] = (powerBins[i] - _powerBinsOld[i]);

        //                if (powerBins[i] < _noiseFloor)
        //                {
        //                    powerBins[i] = 0;
        //                    _powerBinsDelta[i] = 0;
        //                }
        //                if (_powerBinsDelta[i] < _powerBinsDeltaCutoff)
        //                    _powerBinsDelta[i] = 0;

        //                double _relativePower = _powerBinsDelta[Math.Max(i - 1, 0)] + _powerBinsDelta[Math.Max(i + 1, 0)] + _powerBinsDelta[Math.Max(i, 0)];
        //                if ((max < _relativePower) && (_powerBinsDelta[i] > 0) )
        //                {
        //                    max = _relativePower;
        //                    index = i;
        //                }
        //                powerBins[powerBins.Length - 1 - i] = 0;
        //                _powerBinsDelta[powerBins.Length - 1 - i] = 0;
        //            }
        //        }


        //        index = index;
        //        max = max;

        //        //freq = DetectFrequency2(powerBins, index);
        //        freq = DetectFrequency_Mirrored(fftCalc.PowerBins, index - 1, index + 1);
        //        //System.Diagnostics.Debug.Write(freq + "\n");

        //        Note[] notes = new Note[1] { new Note() };
        //        notes[0].startTime = DateTime.Now;
        //        notes[0].frequency = freq;

        //        //Console.WriteLine(notes[0].closestRealNoteFrequency);
        //        NoteQueue.Enqueue(notes);
        //        if (NoteQueue.Count > _maxNoteQueueSize)
        //        {
        //            Note[] dqnotes;
        //            NoteQueue.TryDequeue(out dqnotes);
        //        }
                
        //        powerBins.CopyTo(_powerBinsOld, 0);
        //    }
        //}

    }
}
