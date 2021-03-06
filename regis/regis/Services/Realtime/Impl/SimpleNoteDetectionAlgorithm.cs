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

        private double _sampleRate = AudioCapture.AudioCaptureSettings.SampleRate / AudioCapture.AudioCaptureSettings.BufferSkip;
        private double _samples = AudioCapture.AudioCaptureSettings.BufferSize * AudioCapture.AudioCaptureSettings.BufferModifier;
        private double _step;
        private double _noiseFloor = AudioCapture.AudioCaptureSettings.NoiseFloor;//500000000000000;
        private int _subPeaks = AudioCapture.AudioCaptureSettings.SubPeaks;
        private double _highImpulseCutoff = AudioCapture.AudioCaptureSettings.HighImpulseCutoff;
        private double _lowImpulseCutoff = AudioCapture.AudioCaptureSettings.LowImpulseCutoff;
        private bool _impulseAlg = false;
        private double[] _powerBinsOld, _powerBinsDelta;
        private double _powerBinsDeltaCutoff = 0;
        private double _lastMax = 1;
        private double[] _xorPowerBins = new double[AudioCapture.AudioCaptureSettings.BufferSize * AudioCapture.AudioCaptureSettings.BufferModifier];

        double freq = 0;
        double[] freqArray;
        double currentTotalPower;
        double[] xorPowerBins;
        double[] impulseArray;
        double max;
        int index;
        int[] indexArray;
        double[] maxArray;
        double[] powerBins;
        double[,] stringFreq;
        private int _maxNoteQueueSize = 5;
        double _cutoffFrequency;
        double _impulseCutoff;

        private double _previousTotalPower;
        private const double MAX_POWER_DECAY = 100000;

        double rootNote;
        double _relativePower;
        int rootIndex;

        public event EventHandler<NotesDetectedEventArgs> NotesDetected;

        private Timer _eventTimer;

        #region Constructors and Stuff
        public SimpleNoteDetectionAlgorithm()
        {
            NoteQueue = new ConcurrentQueue<Note[]>();
            _step = _sampleRate / _samples;
            _eventTimer = new Timer(new TimerCallback(eventTimer_tick), null, 0, 500);
            stringFreq = new double[6, 2] { { 82.407 - (2 * _step), 293.67 + (2 * _step) }, { 110 - (2 * _step), 392.00 + (2 * _step) }, { 146.83 - (2 * _step), 523.25 + (2 * _step) }, { 196 - (2 * _step), 698.46 + (2 * _step) }, { 246.94 - (2 * _step), 880.00 + (2 * _step) }, { 329.63 - (2 * _step), 1174.66 + (2 * _step) } };
            _cutoffFrequency = stringFreq[0, 0];
            //_xorPowerBins = NoiseFilter();            
        }
        
        private void eventTimer_tick(object state)
        {
            Note[] notes;
            if (NoteQueue.TryDequeue(out notes))
            {
                Raise_NotesDetected(notes);
            }
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
            _noteDetectionThread = new Thread(new ThreadStart(ImpulseDetectNotes));

            _noteDetectionThread.Start();
        }

        public void Stop()
        {
            if (_noteDetectionThread == null)
                return;

            _stopDetecting = true;
            _noteDetectionThread.Join();
        }
        #endregion

        #region Filters
        //Creates array of binary values to show which bins contain valid notes
        private double[] NoiseFilter()
        {
            int size = (int)(_samples / 2);
            double[] xorPowerBins = new double[size];
            for (int i = 0; i < size; i++)
            {
                
                if ((i == 0) || ((i * _step) < 75))
                    xorPowerBins[i] = 1;
                else if (true)
                    xorPowerBins[i] = 0;
                else if ((i * _step) <= NoteDictionary.GetClosestNoteFrequency(i * _step) && ((i + 1) * _step) >= NoteDictionary.GetClosestNoteFrequency(i * _step))
                {
                    xorPowerBins[i] = 0;
                    xorPowerBins[i + 1] = 0;
                }
                //else if (i == Math.Floor(NoteDictionary.GetClosestNoteFrequency(i * _step)) || i == Math.Ceiling(NoteDictionary.GetClosestNoteFrequency(i * _step)))
                //xorPowerBins[i] = 1;
                else if (xorPowerBins[i] != 0)
                    xorPowerBins[i] = 1;
            }
            return xorPowerBins;
        }
        
        

        //Creates array which will create an array which will remove all harmonic octaves from a root note.
        private double[] HarmonicOctaveFilter(double[] xorArray, int rootIndex)
        {
            //rootNote = NoteDictionary.GetClosestNoteFrequency(freq);
            //rootIndex = (int)Math.Floor(freq / _step);
            if (rootIndex == 0)
                return xorArray;

            for (int i = rootIndex * 2; i < (_sampleRate / 2) / _step; i = i * 2)
            {
                //if ((i * _step) <= NoteDictionary.GetClosestNoteFrequency(i * _step) && ((i + 1) * _step) >= NoteDictionary.GetClosestNoteFrequency(i * _step))
                //{
                    xorArray[i-1] = 0;
                    xorArray[i] = 0;
                    xorArray[i + 1] = 0;
                //}
            }
            return xorArray;
        }
        #endregion

        #region Handler
        private void Raise_NotesDetected(Note[] notes) {
            EventHandler<NotesDetectedEventArgs> h = NotesDetected;
            if (h == null) return;

            h(this, new NotesDetectedEventArgs(notes));
        }
        #endregion

        #region Multi Impulse Detection
        private void MultiImpulseDetectNotes()
        {
            while (!_stopDetecting)
            {
                FFTPower fftCalc;
                if (!_fftSource.FFTQueue.TryPeek(out fftCalc))
                    continue;

                if (xorPowerBins == null)
                    xorPowerBins = new double[_xorPowerBins.Length];

                if (powerBins == null)
                    powerBins = new double[AudioCapture.AudioCaptureSettings.BufferSize * AudioCapture.AudioCaptureSettings.BufferModifier];

                if (freqArray == null)
                    freqArray = new double[6];

                if (indexArray == null)
                    indexArray = new int[6];

                if (maxArray == null)
                    maxArray = new double[6];

                if (_powerBinsDelta == null)
                    _powerBinsDelta = new double[powerBins.Length];

                if (_powerBinsOld == null)
                    _powerBinsOld = new double[powerBins.Length];

                if (impulseArray == null)
                    impulseArray = new double[6];

                _relativePower = 0;

                Array.Clear(xorPowerBins, 0, xorPowerBins.Length);

                for (int p = 0; p < 6; p++)
                {
                    maxArray[p] = 0;
                    freqArray[p] = 0;
                    indexArray[p] = 0;
                    impulseArray[p] = 0;

                    for (int i = (int)(stringFreq[p, 0] / _step); i < (stringFreq[p, 1] / _step); i++)
                    {
                        powerBins[i] = (fftCalc.PowerBins[i] * (1 - xorPowerBins[i]));
                        powerBins[i + 1] = (fftCalc.PowerBins[i + 1] * (1 - xorPowerBins[i + 1]));
                        _powerBinsDelta[i] = (fftCalc.PowerBins[i] - _powerBinsOld[i]) * (1 - xorPowerBins[i]);

                        if (powerBins[i] < _noiseFloor)
                        {
                            powerBins[i] = 0;
                            powerBins[i + 1] = 0;
                            _powerBinsDelta[i] = 0;
                        }

                        _relativePower = powerBins[Math.Max(i - 1, 0)] + powerBins[Math.Max(i + 1, 0)] + powerBins[Math.Max(i, 0)];

                        if ((maxArray[p] < _relativePower))
                        {
                            maxArray[p] = _relativePower;
                            indexArray[p] = i;
                            if (_powerBinsDelta[i] > 0)
                                impulseArray[p] = _powerBinsDelta[i];
                            //xorPowerBins[i - 2] = 1;
                            //xorPowerBins[i - 1] = 1;
                            //xorPowerBins[i] = 1;
                            //xorPowerBins[i + 1] = 1;
                            //xorPowerBins[i + 2] = 1;
                        }
                        else if (maxArray[p] > _relativePower)
                        {
                            if (indexArray[p] > 1)
                            {
                                for (int j = indexArray[p]; (j < (((_sampleRate / 2) / _step) - 2)) && (j < indexArray[p] * 6); j = j + indexArray[p])
                                {
                                    xorPowerBins[j - 2] = 1;
                                    xorPowerBins[j - 1] = 1;
                                    xorPowerBins[j] = 1;
                                    xorPowerBins[j + 1] = 1;
                                    xorPowerBins[j + 2] = 1;
                                }
                            }
                            stringFreq[Math.Min((p + 1), 5), 0] = (index * _step);
                            break;
                        }
                    }
                }

                for (int j = 0; j < 6; j++)
                {
                    if (j == 0)
                        _impulseCutoff = _lowImpulseCutoff;
                    else
                        _impulseCutoff = _highImpulseCutoff;
                    if (impulseArray[j] > _lowImpulseCutoff)
                    {
                        Note[] notes = new Note[6];
                        for (int i = 0; i < 6; i++)
                        {
                            freqArray[i] = DetectFrequency_Mirrored(fftCalc.PowerBins, indexArray[i] - _subPeaks, indexArray[i] + _subPeaks);
                            notes[i] = new Note();
                            notes[i].startTime = DateTime.Now;
                            notes[i].frequency = freqArray[i];
                        }
                        Raise_NotesDetected(notes);
                        break;
                    }
                }

                Buffer.BlockCopy(fftCalc.PowerBins, 0, _powerBinsOld, 0, _powerBinsOld.Length * sizeof(double));
                //NoteQueue.Enqueue(notes);
            }
        }
        #endregion

        #region Multi Detection
        private void MultiDetectNotes()
        {
            while (!_stopDetecting)
            {
                FFTPower fftCalc;
                if (!_fftSource.FFTQueue.TryPeek(out fftCalc))
                    continue;

                if (xorPowerBins == null)
                    xorPowerBins = new double[_xorPowerBins.Length];

                if (powerBins == null)
                    powerBins = new double[AudioCapture.AudioCaptureSettings.BufferSize * AudioCapture.AudioCaptureSettings.BufferModifier];

                if (freqArray == null)
                    freqArray = new double[6];

                if (indexArray == null)
                    indexArray = new int[6];

                if (maxArray == null)
                    maxArray = new double[6];

                _relativePower = 0;

                Array.Clear(xorPowerBins, 0, xorPowerBins.Length);

                for (int p = 0; p < 6; p++)
                {
                    maxArray[p] = 0;
                    freqArray[p] = 0;
                    indexArray[p] = 0;

                    for (int i = (int)(stringFreq[p, 0] / _step); i < (stringFreq[p, 1] / _step); i++)
                    {
                        powerBins[i] = (fftCalc.PowerBins[i] * (1 - xorPowerBins[i]));
                        powerBins[i + 1] = (fftCalc.PowerBins[i + 1] * (1 - xorPowerBins[i + 1]));

                        if (powerBins[i] < _noiseFloor)
                        {
                            powerBins[i] = 0;
                            powerBins[i + 1] = 0;
                        }

                        _relativePower = powerBins[Math.Max(i - 1, 0)] + powerBins[Math.Max(i + 1, 0)] + powerBins[Math.Max(i, 0)];

                        if ((maxArray[p] < _relativePower))
                        {
                            maxArray[p] = _relativePower;
                            indexArray[p] = i;

                            //xorPowerBins[i - 2] = 1;
                            //xorPowerBins[i - 1] = 1;
                            //xorPowerBins[i] = 1;
                            //xorPowerBins[i + 1] = 1;
                            //xorPowerBins[i + 2] = 1;
                        }
                        else if (maxArray[p] > _relativePower)
                        {
                            if (indexArray[p] > 1)
                            {
                                for (int j = indexArray[p]; (j < (((_sampleRate / 2) / _step) - 2)) && (j < indexArray[p] * 6); j = j + indexArray[p])
                                {
                                    xorPowerBins[j - 2] = 1;
                                    xorPowerBins[j - 1] = 1;
                                    xorPowerBins[j] = 1;
                                    xorPowerBins[j + 1] = 1;
                                    xorPowerBins[j + 2] = 1;
                                }
                            }
                            break;
                        }
                    }
                }
                

                Note[] notes = new Note[6];
                for (int i = 0; i < 6; i++)
                {
                    freqArray[i] = DetectFrequency_Mirrored(fftCalc.PowerBins, indexArray[i] - _subPeaks, indexArray[i] + _subPeaks);
                    notes[i] = new Note();
                    notes[i].startTime = DateTime.Now;
                    notes[i].frequency = freqArray[i];
                }
                Raise_NotesDetected(notes);
                //NoteQueue.Enqueue(notes);
            }
        }
        #endregion

        #region Improved Simple Detection
        private void SimpleDetectNotesImproved()
        {
            while (!_stopDetecting)
            {
                FFTPower fftCalc;
                if (!_fftSource.FFTQueue.TryPeek(out fftCalc))
                    continue;

                if (powerBins == null)
                    powerBins = new double[AudioCapture.AudioCaptureSettings.BufferSize * AudioCapture.AudioCaptureSettings.BufferModifier];

                freq = 0;
                max = 0;
                index = 0;
                _relativePower = 0;

                if (xorPowerBins == null)
                    xorPowerBins = new double[_xorPowerBins.Length];

                currentTotalPower = powerBins.Sum();
                //Buffer.BlockCopy(_xorPowerBins, 0, xorPowerBins, 0, _xorPowerBins.Length * sizeof(double));

                Array.Clear(xorPowerBins, 0, xorPowerBins.Length);
                for (int i = 0; i < ((_sampleRate / 2) / _step); i++)//(fftCalc.PowerBins.Length / 2); i++)
                {
                    if ((i == 0) || ((i * _step) < 75))
                        powerBins[i] = 0;
                    else
                        powerBins[i] = (fftCalc.PowerBins[i] * (1 - xorPowerBins[i]));

                    if (powerBins[i] < _noiseFloor)
                    {
                        powerBins[i] = 0;
                    }

                    _relativePower = powerBins[Math.Max(i - 1, 0)] + powerBins[Math.Max(i + 1, 0)] + powerBins[Math.Max(i, 0)];

                    if ((max < _relativePower))
                    {
                        max = _relativePower;
                        index = i;
                    }
                    else if (max > _relativePower)
                    {
                        if (index > 1)
                        {
                            for (int j = index * 2; (j < (((_sampleRate / 2) / _step) - 2)) || (j < index * 6); j = j + index)
                            {
                                //xorPowerBins[j - 2] = 1;
                                xorPowerBins[j - 1] = 1;
                                xorPowerBins[j] = 1;
                                xorPowerBins[j + 1] = 1;
                                //xorPowerBins[j + 2] = 1;
                            }
                        }
                    }
                    //powerBins[i] = powerBins[i] / fftCalc.PowerBins.Max() * 1000;
                }

                //_noiseFloor = 0.7 * _lastMax;

                //freq = DetectFrequency2(powerBins, index);
                freq = DetectFrequency_Mirrored(fftCalc.PowerBins, index - _subPeaks, index + _subPeaks);
                //System.Diagnostics.Debug.Write(freq + "\n");

                Note[] notes = new Note[1] { new Note() };
                notes[0].startTime = DateTime.Now;
                notes[0].frequency = freq;

                //Console.WriteLine(notes[0].closestRealNoteFrequency);

                NoteQueue.Enqueue(notes);
                //if (NoteQueue.Count > _maxNoteQueueSize)
                //{
                //    Note[] dqnotes;
                //    NoteQueue.TryDequeue(out dqnotes);
                //}
            }
        }
        #endregion

        #region Simple Detection
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
        #endregion
        double impulse;
        #region Impulse Detection
        private void ImpulseDetectNotes()
        {
            while (!_stopDetecting)
            {
                FFTPower fftCalc;
                if (!_fftSource.FFTQueue.TryPeek(out fftCalc))
                    continue;

                if (powerBins == null)
                    powerBins = new double[AudioCapture.AudioCaptureSettings.BufferSize * AudioCapture.AudioCaptureSettings.BufferModifier];

                if (_powerBinsDelta == null)
                    _powerBinsDelta = new double[powerBins.Length];

                if (_powerBinsOld == null)
                    _powerBinsOld = new double[powerBins.Length];

                if (xorPowerBins == null)
                    xorPowerBins = new double[_xorPowerBins.Length];

                Array.Clear(xorPowerBins, 0, xorPowerBins.Length);

                freq = 0;
                max = 0;
                index = 0;
                _relativePower = 0;
                impulse = 0;

                for (int i = (int)(stringFreq[0, 0] / _step); i < (stringFreq[5, 1] / _step); i++)
                {
                    powerBins[i] = (fftCalc.PowerBins[i] * (1 - xorPowerBins[i]));
                    powerBins[i + 1] = (fftCalc.PowerBins[i + 1] * (1 - xorPowerBins[i + 1]));
                    _powerBinsDelta[i] = (fftCalc.PowerBins[i] - _powerBinsOld[i]) * (1 - xorPowerBins[i]);

                    if (powerBins[i] < _noiseFloor)
                    {
                        powerBins[i] = 0;
                        powerBins[i + 1] = 0;
                        _powerBinsDelta[i] = 0;
                    }

                    _relativePower = powerBins[Math.Max(i - 1, 0)] + powerBins[Math.Max(i + 1, 0)] + powerBins[Math.Max(i, 0)];


                    if ((max < _relativePower))
                    {
                        max = _relativePower;
                        index = i;
                        if (_powerBinsDelta[i] > 0)
                            impulse = _powerBinsDelta[i];
                    }
                    else if (max > _relativePower)
                    {
                        if (index > 1)
                        {
                            for (int j = index; (j < (((_sampleRate / 2) / _step) - 2)) && (j < index * 6); j = j + index)
                            {
                                xorPowerBins[j - 2] = 1;
                                xorPowerBins[j - 1] = 1;
                                xorPowerBins[j] = 1;
                                xorPowerBins[j + 1] = 1;
                                xorPowerBins[j + 2] = 1;
                            }
                        }
                        break;
                    }
                }

                Buffer.BlockCopy(fftCalc.PowerBins, 0, _powerBinsOld, 0, _powerBinsOld.Length * sizeof(double));

                _impulseCutoff = _lowImpulseCutoff;
                //_impulseCutoff = _highImpulseCutoff;
                if (impulse> _lowImpulseCutoff)
                {
                    Note[] notes = new Note[6];
                    for (int i = 0; i < 6; i++)
                    {
                        freq = DetectFrequency_Mirrored(fftCalc.PowerBins, index - _subPeaks, index + _subPeaks);
                        notes[i] = new Note();
                        notes[i].startTime = DateTime.Now;
                        notes[i].frequency = freq;
                    }
                    Raise_NotesDetected(notes);
                }
            }
        }
        #endregion

        #region Frequency Calc
        private double DetectFrequency_Mirrored(double[] inputArray, int minIndex = 0, int maxIndex = 0)
        {

            if ((maxIndex == inputArray.Length - 1) || (minIndex == 0))
            {
                return 0;
            }
            if (maxIndex >= inputArray.Length)
                maxIndex = inputArray.Length - 1;
            if (minIndex < 0)
                minIndex = 0;

            double sum = 0;
            double freq = 0;

            for (int i = Math.Max(minIndex, 0); i <= maxIndex; i++)
            {
                sum += inputArray[i];
            }

            for (int i = Math.Max(minIndex, 0); i <= maxIndex; i++)
            {
                freq += (inputArray[i] / sum) * (i * _step);
            }
            if (freq > _cutoffFrequency)
                return freq;
            else
                return 0;
        }
        #endregion

    }
}
