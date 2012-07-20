using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regis.Services.Realtime
{
    public class SimpleChordDetectionArgs
    {

    }

    interface IChordDetectionService: IRealtimeService<SimpleChordDetectionArgs>
    {
    }
}
