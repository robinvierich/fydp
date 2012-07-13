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
    [Export(typeof(ISampleSource))]
    [Export(typeof(IAsioSamplingService))]
    public class AsioSamplingService : ISampleSource, IAsioSamplingService
    {
        private AsioDriver _currentDriver;
        private Channel _currentInputChannel;
        private ConcurrentQueue<SampleCollection> _sampleCollectionQueue;

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

            // Flush buffers
            flushBuffers();

            _currentDriver.DisposeBuffers();
            _currentDriver.Stop();
            //_currentDriver.Release(); This needs to happen on Application Exit to prevent hangs.. need to figure this out.
        }

        // this handler flushes buffer data
        void driver_BufferUpdate(object sender, EventArgs e)
        {
            flushBuffers();
        }

        private void flushBuffers()
        {
            SampleCollection samples = new SampleCollection();
            samples.Samples = new float[_currentInputChannel.BufferSize];

            // NOTE: may need to optimize this loop
            for (int i = 0; i < _currentInputChannel.BufferSize; i++)
                samples.Samples[i] = _currentInputChannel[i];

            _sampleCollectionQueue.Enqueue(samples);
        }

        
    }
}
