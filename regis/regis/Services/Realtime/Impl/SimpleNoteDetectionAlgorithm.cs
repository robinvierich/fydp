﻿using System;
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

        private double _sampleRate = AudioCapture.AudioCaptureSettings.SampleRate;
        private double _samples = AudioCapture.AudioCaptureSettings.BufferSize * AudioCapture.AudioCaptureSettings.BufferModifier;
        private double _step;
        private double _noiseFloor = AudioCapture.AudioCaptureSettings.NoiseFloor;//500000000000000;

        private bool _impulseAlg = true;
        private double[] _powerBinsOld, _powerBinsDelta;
        private double _powerBinsDeltaCutoff = 2E+0;
        private double[] _xorPowerBins = new double[AudioCapture.AudioCaptureSettings.BufferSize * AudioCapture.AudioCaptureSettings.BufferModifier];

        public SimpleNoteDetectionAlgorithm()
        {
            NoteQueue = new ConcurrentQueue<Note[]>();

            for (int i = 0; i < AudioCapture.AudioCaptureSettings.BufferSize * AudioCapture.AudioCaptureSettings.BufferModifier / 2; i++)
            {
                _step = _sampleRate / _samples;

                if (i == 0)
                    _xorPowerBins[i] = 0;
                else if (i == Math.Floor(NoteDictionary.GetClosestNoteFrequency(i * _step)) || i == Math.Ceiling(NoteDictionary.GetClosestNoteFrequency(i * _step)))
                    _xorPowerBins[i] = 1;
                else
                    _xorPowerBins[i] = 0;
            }
        }

        public void Start(SimpleNoteDetectionArgs args)
        {
            if (_noteDetectionThread != null)
                return;

            _stopDetecting = false;
            _step = _sampleRate / _samples;
            if (_impulseAlg)
                _noteDetectionThread = new Thread(new ThreadStart(ImpulseDetectNotes));
            else
                _noteDetectionThread = new Thread(new ThreadStart(SimpleDetectNotes));

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

        private void SimpleDetectNotes()
        {
            while (!_stopDetecting)
            {
                FFTPower fftCalc;
                if (!_fftSource.FFTQueue.TryPeek(out fftCalc))
                    continue;

                double[] powerBins = new double[AudioCapture.AudioCaptureSettings.BufferSize * AudioCapture.AudioCaptureSettings.BufferModifier];
                fftCalc.PowerBins.CopyTo(powerBins, 0);
                double freq = 0;

                //Start Note Detection
                double currentTotalPower = powerBins.Sum();
                //if (currentTotalPower < _previousTotalPower) // if our power is less than thy previous power by at least MAX_POWER_DECAY, ignore the rest
                //{
                //    freq = 0;
                //}/
                //else
                //{
                double max = 0;
                int index = 0;
                for (int i = 0; i < powerBins.Length / 2; i++)
                {
                    powerBins[i] = powerBins[i] * _xorPowerBins[i];
                    if (powerBins[i] < _noiseFloor)
                        powerBins[i] = 0;
                    if ((max < powerBins[i]) && (i > 75))
                    {
                        max = powerBins[i];
                        index = i;
                    }
                    powerBins[powerBins.Length - 1 - i] = 0;
                }

                index = index;
                max = max;

                freq = DetectFrequency_Mirrored(powerBins);
                //}
                System.Diagnostics.Debug.Write(freq + "\n");
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

        private void ImpulseDetectNotes()
        {
            while (!_stopDetecting)
            {
                FFTPower fftCalc;
                if (!_fftSource.FFTQueue.TryPeek(out fftCalc))
                    continue;

                double[] powerBins = new double[AudioCapture.AudioCaptureSettings.BufferSize * AudioCapture.AudioCaptureSettings.BufferModifier];
                _powerBinsDelta = new double[powerBins.Length];
                fftCalc.PowerBins.CopyTo(powerBins, 0);

                if (_powerBinsOld == null)
                {
                    _powerBinsOld = new double[AudioCapture.AudioCaptureSettings.BufferSize * AudioCapture.AudioCaptureSettings.BufferModifier];
                    powerBins.CopyTo(_powerBinsOld, 0);
                    continue;
                }

                double freq = 0;

                //Start Note Detection
                double currentTotalPower = powerBins.Sum();
                //if (currentTotalPower < _previousTotalPower) // if our power is less than thy previous power by at least MAX_POWER_DECAY, ignore the rest
                //{
                //    freq = 0;
                //}/
                //else
                //{
                double max = 0;
                int index = 0;
                for (int i = 0; i < powerBins.Length / 2; i++)
                {
                    powerBins[i] = powerBins[i] * _xorPowerBins[i];
                    _powerBinsDelta[i] = (powerBins[i] - _powerBinsOld[i]);

                    if (powerBins[i] < _noiseFloor)
                    {
                        powerBins[i] = 0;
                        _powerBinsDelta[i] = 0;
                    }
                    if (_powerBinsDelta[i] < _powerBinsDeltaCutoff)
                        _powerBinsDelta[i] = 0;

                    double _relativePower = _powerBinsDelta[Math.Max(i - 1, 0)] + _powerBinsDelta[Math.Max(i + 1, 0)] + _powerBinsDelta[Math.Max(i, 0)];
                    if ((max < _relativePower) && (_powerBinsDelta[i] > 0) )
                    {
                        max = _relativePower;
                        index = i;
                    }
                    powerBins[powerBins.Length - 1 - i] = 0;
                    _powerBinsDelta[powerBins.Length - 1 - i] = 0;
                }

                index = index;
                max = max;

                //freq = DetectFrequency2(powerBins, index);
                freq = DetectFrequency_Mirrored(fftCalc.PowerBins, index - 1, index + 1);
                System.Diagnostics.Debug.Write(freq + "\n");

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
                
                powerBins.CopyTo(_powerBinsOld, 0);
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

        public Note[] GetNotes()
        {
            Note[] toReturn;
            NoteQueue.TryPeek(out toReturn);
            return toReturn;
        }
    }
}
