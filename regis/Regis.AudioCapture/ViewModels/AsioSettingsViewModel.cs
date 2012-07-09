using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Base.ViewModels;
using System.Collections.ObjectModel;
using BlueWave.Interop.Asio;
using System.ComponentModel;
using Regis.AudioCapture.Services;

namespace Regis.AudioCapture.ViewModels
{
    public class AsioSettingsViewModel: BaseViewModel
    {

        public AsioSettingsViewModel()
        {
            AsioDeviceService ads = new AsioDeviceService();
            AsioDrivers = ads.GetAsioDrivers();
            ads.DriverLoaded += new EventHandler<DriverLoadedEventArgs>(AsioDeviceService_DriverLoaded);
        }

        void AsioDeviceService_DriverLoaded(object sender, DriverLoadedEventArgs e)
        {
            LoadedDriver = e.Driver;
        }

        private ObservableCollection<InstalledDriver> _AsioDrivers;
        private PropertyChangedEventArgs _AsioDriversChangedEventArgs = new PropertyChangedEventArgs("AsioDrivers");
        public ObservableCollection<InstalledDriver> AsioDrivers
        {
            get { return _AsioDrivers; }
            private set { 
                _AsioDrivers = value;
                NotifyPropertyChanged(_AsioDriversChangedEventArgs);
            }
        }


        private AsioDriver _LoadedDriver;
        private PropertyChangedEventArgs _LoadedDriverChangedEventArgs = new PropertyChangedEventArgs("LoadedDriver");
        public AsioDriver LoadedDriver
        {
            get
            {
                return _LoadedDriver;
            }

            private set
            {
                _LoadedDriver = value;
                NotifyPropertyChanged(_LoadedDriverChangedEventArgs);
            }
        }

    }
}
