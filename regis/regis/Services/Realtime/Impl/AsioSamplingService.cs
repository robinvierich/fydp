using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlueWave.Interop.Asio;
using System.Threading;
using System.Collections.Concurrent;
using Regis.Models;
using System.ComponentModel.Composition;
using Regis.AudioCapture;
using System.Threading.Tasks;

namespace Regis.Services.Realtime.Impl
{

    /// <summary>
    /// Samples audio (PCM) using ASIO drivers
    /// </summary>
    [Export(typeof(ISampleSource))]
    [Export(typeof(IAsioSamplingService))]
    public class AsioSamplingService : ISampleSource, IAsioSamplingService
    {
        private AsioDriver _currentDriver;
        private Channel _currentInputChannel;
        private ConcurrentQueue<SampleCollection> _sampleCollectionQueue;
        private int _sampleCollectionSize;
        private int _index = 0;
        private int _size = AudioCapture.AudioCaptureSettings.BufferModifier;

        public AsioSamplingService()
        {
            _sampleCollectionQueue = new ConcurrentQueue<SampleCollection>();
        }

        public ConcurrentQueue<SampleCollection> SampleCollectionQueue
        {
            get { return _sampleCollectionQueue; }
        }

        /// <summary>
        /// Starts sampling audio from a specific device.
        /// </summary>
        public void Start(AsioSamplingServiceArgs args)
        {
            if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
                throw new Exception("Thread must run in STA for ASIO sampling to work");

            _currentDriver = args.Driver;

            //if (!args.Driver.InputChannels.Contains(args.Channel))
            //    throw new Exception("Input channel must be in driver.InputChannels");

            // The order here is key
            // Create buffers before selecting the channel
            _currentDriver.CreateBuffers(true, AudioCapture.AudioCaptureSettings.BufferSize);
            _currentInputChannel = args.Driver.InputChannels.Where(x => x.Name == args.Channel.Name).Single();

            args.Driver.BufferUpdate += new EventHandler(driver_BufferUpdate);
            _currentDriver.Start();
        }

        public void Stop()
        {
            if (_currentDriver == null)
                return;

            _currentDriver.BufferUpdate -= new EventHandler(driver_BufferUpdate);

            _currentDriver.DisposeBuffers();
            _currentDriver.Stop();
        }

        public void ReleaseDriver()
        {
            if (_currentDriver == null)
                return;

            _currentDriver.Release();
        }

        // this handler reads buffer data
        void driver_BufferUpdate(object sender, EventArgs e)
        {
            SampleCollection sampleColl = new SampleCollection();
            sampleColl.Samples = new long[ (_currentInputChannel.BufferSize) ];

            readBuffers(ref sampleColl.Samples);

            if (_size == 1)
            {
                _sampleCollectionQueue.Enqueue(sampleColl);
            }
            else
            {
                SampleCollection sampleColl2 = new SampleCollection();
                sampleColl2.Samples = new long[(_currentInputChannel.BufferSize * _size)];

                for (int i = 0; i < _currentInputChannel.BufferSize * (_size - 1); i++)
                {
                    sampleColl2.Samples[i] = sampleColl2.Samples[i + _currentInputChannel.BufferSize];
                }
                for (int i = _currentInputChannel.BufferSize * (_size - 1); i < _currentInputChannel.BufferSize * _size; i++)
                {
                    sampleColl2.Samples[i] = sampleColl.Samples[i - _currentInputChannel.BufferSize * (_size - 1)];
                }

                _sampleCollectionQueue.Enqueue(sampleColl2);
            }
        }

        private void readBuffers(ref long[] toArray)
        {
            // NOTE: may need to optimize this loop
            for (int i = 0; i < _currentInputChannel.BufferSize; i++)
                toArray[i] = (long)_currentInputChannel[(i)];
        }
    }
}
