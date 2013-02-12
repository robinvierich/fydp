using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Services.Realtime.SerialImpl;
using System.ComponentModel.Composition;

namespace Regis.Services.Realtime.Serial
{
    [Export(typeof(ISerialService<byte[]>))]
    public class ByteArraySerialService : SerialServiceBase<byte[]>
    {
        protected override byte[] Handle_DataReceived(byte[] newData) {
            byte[] toReturn = new byte[newData.Length];
            newData.CopyTo(toReturn, 0);
            return toReturn;
        }

        protected override void Handle_EOF() {
        }

        protected override void Handle_ErrorReceived(System.IO.Ports.SerialErrorReceivedEventArgs e) {
        }
    }
}
