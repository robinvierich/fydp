using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlueWave.Interop.Asio;

namespace Regis.Services.Realtime
{
    public class AsioSamplingServiceArgs
    {
        public AsioDriver Driver { get; set; }
        public Channel Channel { get; set; }
        public uint SamplingRate { get; set; }
    }

    interface IAsioSamplingService : IRealtimeService<AsioSamplingServiceArgs>
    {
    }
}
