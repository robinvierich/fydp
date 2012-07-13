using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Regis.Models;
using System.ComponentModel.Composition;
using System.Threading;

namespace Regis.Services.Realtime
{
    [Export(typeof(IFFTService))]
    [Export(typeof(IFFTSource))]
    public class FFTAlgorithm : IFFTSource, IFFTService
    {
        private ConcurrentQueue<FFTCalculation> _fftQueue = new ConcurrentQueue<FFTCalculation>();
        public ConcurrentQueue<FFTCalculation> FFTQueue { get { return _fftQueue; }}

        [Import]
        private ISampleSource _sampleSource = null;

        Thread _calculationThread;
        bool _stopCalculating;

        public void Start(FFTArgs args)
        {
            if (_calculationThread != null)
                return;

            _stopCalculating = false;

            _calculationThread = new Thread(new ThreadStart(CalculateFFT));
            _calculationThread.Start();
        }

        public void Stop()
        {
            if (_calculationThread == null)
                return;

            _stopCalculating = true;
            _calculationThread.Join();
        }

        private void CalculateFFT()
        {
            while (!_stopCalculating)
            {
                SampleCollection sampleCollection;
                _sampleSource.SampleCollectionQueue.TryDequeue(out sampleCollection);

                if (sampleCollection == null)
                    continue;

                float[] samples = sampleCollection.Samples;

                // TODO: Perform calculation

                FFTCalculation fftCalc = new FFTCalculation();
                _fftQueue.Enqueue(fftCalc);
            }
        }
    }
}
