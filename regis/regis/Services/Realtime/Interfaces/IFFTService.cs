using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regis.Services.Realtime
{
    public class FFTArgs
    {
        public uint FFTSize { get; set; }
    }

    interface IFFTService: IRealtimeService<FFTArgs>
    {
    }
}
