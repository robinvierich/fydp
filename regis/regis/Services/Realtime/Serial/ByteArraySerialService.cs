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

            float[] floatArray = new float[newData.Length/2];

            int j = 0;
            for (int i = 1; i < newData.Length; i += 2) {
                byte b0 = newData[i-1];
                byte b1 = newData[i];

                j = i / 2; // floor
                floatArray[j] = (float)(Math.Pow(2, -15) * (short)(b1 << 8 | b0));
            }

            double[] powerArray = new double[floatArray.Length/2];
            for (int i = 1; i < floatArray.Length; i+=2) {
                j = i / 2; // floor
                powerArray[j] = Math.Sqrt(Math.Pow(floatArray[i-1], 2) + Math.Pow(floatArray[i], 2));
            }


            return toReturn;
        }

        protected override void Handle_EOF() {
        }

        protected override void Handle_ErrorReceived(System.IO.Ports.SerialErrorReceivedEventArgs e) {
        }
    }
}
