using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.IO;
using System.Collections.Concurrent;

namespace Regis.Services.Realtime.SerialImpl
{
    /*
     TODO: Make this non-abstract and add 'where T : ISerializable'
    
     This way, the data can deserialize itself and we don't need to make new services for each data type.
     */

    public abstract class SerialServiceBase<T> : ISerialService<T>
    {
        protected SerialPort _serialPort;
        private byte[] _serialBuffer;

        public void Start(SerialPort args) {
            if (args == null)
                throw new ArgumentNullException("args");

            _serialPort = args;
            _serialBuffer = new byte[args.ReadBufferSize];
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(_serialPort_DataReceived);
            _serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(_serialPort_ErrorReceived);
            _serialPort.Open();
        }


        public void Stop() {
            if (_serialPort == null) return;
            _serialPort.Close();
            _serialPort.DataReceived -= _serialPort_DataReceived;
            _serialPort.ErrorReceived -= _serialPort_ErrorReceived;
            _serialPort.Dispose();
        }

        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e) {
            switch (e.EventType) {
                case SerialData.Chars:
                    _serialPort.Read(_serialBuffer, 0, _serialPort.ReadBufferSize);
                    T data = Handle_DataReceived(_serialBuffer);
                    Raise_DataReceived(data);
                    break;

                case SerialData.Eof:
                    Handle_EOF();
                    break;

                default:
                    throw new IOException("Serial port received malformed EventType.");
            }
        }

        private void _serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e) {
            Handle_ErrorReceived(e);
        }

        protected abstract T Handle_DataReceived(byte[] newData);
        protected abstract void Handle_EOF();
        protected abstract void Handle_ErrorReceived(SerialErrorReceivedEventArgs e);

        public event EventHandler<SerialServiceDataEventArgs<T>> DataReceived;
        private void Raise_DataReceived(T data) {
            EventHandler<SerialServiceDataEventArgs<T>> h = DataReceived;
            if (h == null) return;
            h(this, new SerialServiceDataEventArgs<T>(data));
        }
    }
}
