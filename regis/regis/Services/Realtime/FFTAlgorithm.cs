using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Regis.Models;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using AForge.Math;

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

                Complex[] fftArray = Array.ConvertAll(sampleCollection.Samples, new Converter<long, Complex>(x => (Complex)x));

                FourierTransform.FFT(fftArray, FourierTransform.Direction.Forward);

                FFTCalculation fftCalc = new FFTCalculation();

                fftCalc.PowerBins = new double[sampleCollection.Samples.Length];
                fftCalc.PowerBins = fftArray.Select(x => x.SquaredMagnitude).ToArray();

                _fftQueue.Enqueue(fftCalc);

                #region FFTW (not currently in use)
                // Faster fftw stuff
                //
                // TODO: We really should make a c++ project so that we don't have to use P/Invoke (i.e. call fftw directly)
                // This should significantly speed up the FFT
                //
                //
                //long[] sampleArray = sampleCollection.Samples;
                //long[] outputArray = new long[sampleCollection.Samples.Length];
                //
                //// Unsafe allows us to use pointers
                //unsafe
                //{
                //    // fixed makes the GC not move the pointer data
                //    fixed (long* input = sampleArray)
                //    {
                //        fixed (long* output = outputArray)
                //        {
                //            IntPtr inputPtr = (IntPtr)input;
                //            IntPtr outputPtr = (IntPtr)output;

                //            IntPtr plan = fftw.dft_1d(1024, inputPtr, outputPtr, fftw_direction.Forward, fftw_flags.Estimate);

                //            fftw.execute(plan);

                //            fftw.free(inputPtr);
                //            fftw.free(outputPtr);
                //            fftw.destroy_plan(plan);
                //        }
                //    }
                //}
                #endregion

            }
        }


        
    }
}
