using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Base.ViewModels;
using System.Collections.ObjectModel;
using BlueWave.Interop.Asio;
using System.ComponentModel;
using Regis.AudioCapture.Services;
using Regis.Services.Realtime;
using System.ComponentModel.Composition;
using Regis.Commands;
using Regis.AudioCapture.Services.Impl;

namespace Regis.ViewModels
{
    public enum RawInputType
    {
        PIC,
        Audio
    }

    [Export]
    public class SettingsViewModel: BaseViewModel
    {
        public SettingsViewModel()
        {
            AsioDrivers = AsioDeviceService.GetAsioDrivers();
            
            // TODO: This will leak because AsioDeviceService is static. We should either
            // use a weak reference here or make AsioDeviceService a singleton.
            AsioDeviceService.DriverLoaded += new EventHandler<DriverLoadedEventArgs>(AsioDeviceService_DriverLoaded);
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

        #region InputType
        private RawInputType _InputType;
        private static PropertyChangedEventArgs _InputType_ChangedEventArgs = new PropertyChangedEventArgs("InputType");

        public RawInputType InputType {
            get { return _InputType; }
            set {
                _InputType = value;
                NotifyPropertyChanged(_InputType_ChangedEventArgs);
            }
        }
        #endregion

        #region SerialSettings
        [Import]
        public SerialSettingsViewModel SerialSettings {
            get;
            set;
        }
        #endregion

        #region SelectedChannel
        private Channel _SelectedChannel;
        private static PropertyChangedEventArgs _SelectedChannel_ChangedEventArgs = new PropertyChangedEventArgs("SelectedChannel");

        public Channel SelectedChannel
        {
            get { return _SelectedChannel; }
            set
            {
                _SelectedChannel = value;
                NotifyPropertyChanged(_SelectedChannel_ChangedEventArgs);
            }
        }
        #endregion


        #region RestartSamplingServiceCommand
        private RestartSamplingServiceCommand _RestartSamplingServiceCommand;
        private static PropertyChangedEventArgs _RestartSamplingServiceCommand_ChangedEventArgs = new PropertyChangedEventArgs("RestartSamplingServiceCommand");

        [Import]
        public RestartSamplingServiceCommand RestartSamplingServiceCommand
        {
            get { return _RestartSamplingServiceCommand; }
            set
            {
                _RestartSamplingServiceCommand = value;
                NotifyPropertyChanged(_RestartSamplingServiceCommand_ChangedEventArgs);
            }
        }
        #endregion
    }
}
