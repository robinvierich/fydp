using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlueWave.Interop.Asio;
using System.Threading;
using System.Collections.Concurrent;
using Regis.Models;
using System.ComponentModel.Composition;

namespace Regis.Services.Realtime
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
            _sampleCollectionSize = AudioCapture.AudioCapture.SampleCollectionSize;

            if (!args.Driver.InputChannels.Contains(args.Channel))
                throw new Exception("Input channel must be in driver.InputChannels");

            // The order here is key
            // Create buffers before selecting the channel
            _currentDriver.CreateBuffers(false);
            _currentInputChannel = args.Driver.InputChannels.Where(x => x.Name == args.Channel.Name).Single();
            
            args.Driver.BufferUpdate += new EventHandler(driver_BufferUpdate);
            _currentDriver.Start();
        }

        public void Stop()
        {
            if (_currentDriver == null)
                return;

            _currentDriver.BufferUpdate -= new EventHandler(driver_BufferUpdate);

            // read buffers
            //readBuffers();

            _currentDriver.DisposeBuffers();
            _currentDriver.Stop();
        }

        public void ReleaseDriver()
        {
            if (_currentDriver == null)
                return;

            _currentDriver.Release();
        }

        int destoffset = 0;
        SampleCollection currentSampleCollection;

        // this handler reads buffer data
        void driver_BufferUpdate(object sender, EventArgs e)
        {
            if (destoffset == 0)
            {
                currentSampleCollection = new SampleCollection();
                currentSampleCollection.Samples = new long[_sampleCollectionSize];
            }

            int readLength = Math.Min(_currentInputChannel.BufferSize, _sampleCollectionSize - destoffset);
            readBuffers(ref currentSampleCollection.Samples, 
                        destoffset,
                        readLength);

            destoffset += readLength;
            if (destoffset < _sampleCollectionSize)
                return;

            destoffset = 0;
            _sampleCollectionQueue.Enqueue(currentSampleCollection);
        }

        private void readBuffers(ref long[] toArray, int destoffset, int length)
        {
            // NOTE: may need to optimize this loop
            for (int i = 0; i < length; i++)
                toArray[i + destoffset] = (long)_currentInputChannel[i];
        }
    }
}
