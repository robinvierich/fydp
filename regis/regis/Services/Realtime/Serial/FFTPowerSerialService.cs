using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Plugins.Models;
using System.Collections.Concurrent;
using System.ComponentModel.Composition;

namespace Regis.Services.Realtime.SerialImpl
{
    [Export(typeof(ISerialService<FFTPower>))]
    public class FFTPowerSerialService : SerialServiceBase<FFTPower>
    {
        protected override FFTPower Handle_DataReceived(byte[] newData) {
            // TODO: convert the byte[] to FFTPower struct.

            FFTPower outData = new FFTPower() {
                PowerBins = new double[newData.Length]//!!!! THIS IS JUST AN EMPTY ARRAY NOW
            };
            return outData;
        }

        protected override void Handle_EOF() {
            throw new NotImplementedException();
        }

        protected override void Handle_ErrorReceived(System.IO.Ports.SerialErrorReceivedEventArgs e) {
            throw new NotImplementedException();
        }
    }
}
