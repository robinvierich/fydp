using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.IO.Ports;

namespace Regis.Services.Realtime
{
    public class SerialServiceDataEventArgs<T> : EventArgs
    {
        public SerialServiceDataEventArgs(T data) {
            Data = data;
        }

        public T Data { get; private set; }
    }

    interface ISerialService<OutT> : IRealtimeService<SerialPort>
    {
        event EventHandler<SerialServiceDataEventArgs<OutT>> DataReceived;
    }
}
