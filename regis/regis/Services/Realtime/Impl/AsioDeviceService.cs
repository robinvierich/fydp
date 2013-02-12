using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlueWave.Interop.Asio;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Regis.AudioCapture.Services.Impl
{
    public static class AsioDeviceService
    {

        public static ObservableCollection<InstalledDriver> GetAsioDrivers()
        {
            ObservableCollection<InstalledDriver> toReturn = new ObservableCollection<InstalledDriver>();
            try
            {
                foreach (InstalledDriver driver in AsioDriver.InstalledDrivers)
                {
                    toReturn.Add(driver);
                }
            }
            catch (NullReferenceException e)
            {
                Debug.WriteLine("No ASIO drivers found.");
            }

            return toReturn;
        }

        public static void LoadDriver(InstalledDriver driver)
        {
            if (AudioCaptureSettings.LoadedDriver != null)
            {
                AudioCaptureSettings.LoadedDriver.Release();
            }

            // Load the driver, set the sample rate, and temporarily create buffers so we can enumerate the input channels
            try
            {
                AudioCaptureSettings.LoadedDriver = AsioDriver.SelectDriver(driver);
                AudioCaptureSettings.LoadedDriver.SetSampleRate(AudioCaptureSettings.SampleRate);
                AudioCaptureSettings.LoadedDriver.CreateBuffers(false, -1);
            }
            catch (ApplicationException)
            {
                throw new Exception("Could not initialize driver");
            }

            AudioCaptureSettings.LoadedDriver.DisposeBuffers();

            Raise_DriverLoaded(AudioCaptureSettings.LoadedDriver);
        }

        #region DriverLoaded Event
        public static event EventHandler<DriverLoadedEventArgs> DriverLoaded;
        private static void Raise_DriverLoaded(AsioDriver driver)
        {
            EventHandler<DriverLoadedEventArgs> h = DriverLoaded;
            if (h == null) return;

            h(null, new DriverLoadedEventArgs(driver));
        }
        #endregion
    }

    public class DriverLoadedEventArgs : EventArgs
    {
        public DriverLoadedEventArgs(AsioDriver driver)
        {
            Driver = driver;
        }

        public AsioDriver Driver { get; private set;}
    }

}
