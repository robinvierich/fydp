using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlueWave.Interop.Asio;
using System.Threading;

namespace Regis.AudioCapture.Services
{
    public class AsioSamplingService: IAsioSamplingService
    {
        public void StartSampling(AsioDriver driver, uint sampleRate)
        {
            if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
                throw new Exception("Thread must run in STA for ASIO sampling to work");


        }

        /// <summary>
        /// Starts sampling audio from a specific device.
        /// </summary>
        /// <param name="driver">The ASIO driver to use</param>
        /// <param name="inputChannel">The input channel to use (if the driver has multiple channels)</param>
        /// <param name="sampleRate">How fast to sample (samples/sec)</param

        private void StartSampling(AsioDriver driver, Channel inputChannel, uint sampleRate)
        {
            driver.BufferUpdate += new EventHandler(driver_BufferUpdate);
        }

        void driver_BufferUpdate(object sender, EventArgs e)
        {
            
        }

    }
}
