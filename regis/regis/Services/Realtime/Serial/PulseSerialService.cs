using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Models;
using System.IO.Ports;
using System.Threading;
using System.IO;
using System.ComponentModel.Composition;

namespace Regis.Services.Realtime.SerialImpl
{
    [Export(typeof(ISerialService<Pulse>))]
    public class PulseSerialService : SerialServiceBase<Pulse>
    {
        protected override Pulse Handle_DataReceived(byte[] newData) {
            throw new NotImplementedException();
        }

        protected override void Handle_EOF() {
            throw new NotImplementedException();
        }

        protected override void Handle_ErrorReceived(SerialErrorReceivedEventArgs e) {
            throw new NotImplementedException();
        }
    }
}
