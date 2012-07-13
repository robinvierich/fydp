using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlueWave.Interop.Asio;
using System.Collections.ObjectModel;

namespace Regis.AudioCapture.Services
{
    public static class AsioDeviceService
    {

        public static ObservableCollection<InstalledDriver> GetAsioDrivers()
        {
            ObservableCollection<InstalledDriver> toReturn = new ObservableCollection<InstalledDriver>();
            foreach (InstalledDriver driver in AsioDriver.InstalledDrivers)
            {
                toReturn.Add(driver);
            }

            return toReturn;
        }

        public static void LoadDriver(InstalledDriver driver, uint sampleRate)
        {
            if (AudioCapture.LoadedDriver != null)
            {
                AudioCapture.LoadedDriver.Release();
            }

            // Load the driver, set the sample rate, and temporarily create buffers so we can enumerate the input channels
            try
            {
                AudioCapture.LoadedDriver = AsioDriver.SelectDriver(driver);
                AudioCapture.LoadedDriver.SetSampleRate(sampleRate);
                AudioCapture.LoadedDriver.CreateBuffers(false);
            }
            catch (ApplicationException)
            {
                throw new Exception("Could not initialize driver");
            }

            AudioCapture.LoadedDriver.DisposeBuffers();

            Raise_DriverLoaded(AudioCapture.LoadedDriver);
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
