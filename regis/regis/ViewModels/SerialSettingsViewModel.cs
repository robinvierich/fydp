using System;
using System.Collections.Generic;
using System.Linq;
using Regis.Base.ViewModels;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.ComponentModel.Composition;
using Regis.Services.Realtime;
using System.Text;
using Regis.Composition;

namespace Regis.ViewModels
{
    [Export]
    public class SerialSettingsViewModel : BaseViewModel
    {
        [Import(typeof(ISerialService<byte[]>))]
        ISerialService<byte[]> _serialService;

        SerialSettingsViewModel() {
            LocalComPorts = new ObservableCollection<string>(SerialPort.GetPortNames());
        }

        #region LocalComPorts
        private ObservableCollection<string> _LocalComPorts;
        private static PropertyChangedEventArgs _LocalComPorts_ChangedEventArgs = new PropertyChangedEventArgs("LocalComPorts");

        public ObservableCollection<string> LocalComPorts {
            get { return _LocalComPorts; }
            set {
                _LocalComPorts = value;
                NotifyPropertyChanged(_LocalComPorts_ChangedEventArgs);
            }
        }
        #endregion

        void _serialService_DataReceived(object sender, SerialServiceDataEventArgs<byte[]> e){
            byte[] bytes = e.Data;
            ASCIIEncoding encoding = new ASCIIEncoding();
            ComPortOutput += encoding.GetString(bytes);
        }

        #region ComPortName
        private string _ComPortName;
        private static PropertyChangedEventArgs _ComPort_ChangedEventArgs = new PropertyChangedEventArgs("ComPortName");

        public string ComPortName {
            get { return _ComPortName; }
            set {
                _ComPortName = value;
                CurrentPort = new SerialPort(_ComPortName);
                NotifyPropertyChanged(_ComPort_ChangedEventArgs);
            }
        }
        #endregion

        #region ComPortOutput
        private string _ComPortOutput;
        private static PropertyChangedEventArgs _ComPortOutput_ChangedEventArgs = new PropertyChangedEventArgs("ComPortOutput");

        public string ComPortOutput {
            get { return _ComPortOutput; }
            set {
                _ComPortOutput = value;
                NotifyPropertyChanged(_ComPortOutput_ChangedEventArgs);
            }
        }
        #endregion

        #region CurrentPort
        private SerialPort _CurrentPort;
        private static PropertyChangedEventArgs _CurrentPort_ChangedEventArgs = new PropertyChangedEventArgs("CurrentPort");

        public SerialPort CurrentPort {
            get { return _CurrentPort; }
            set {
                _CurrentPort = value;
                _serialService.DataReceived -= _serialService_DataReceived;
                _serialService.Stop();
                _serialService.Start(CurrentPort);
                _serialService.DataReceived += new EventHandler<SerialServiceDataEventArgs<byte[]>>(_serialService_DataReceived);
                NotifyPropertyChanged(_CurrentPort_ChangedEventArgs);
            }
        }
        #endregion
    }
}
