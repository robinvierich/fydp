using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace Regis.Services.Realtime
{
    interface IRealtimeService<T>
    {
        void Start(T args);
        void Stop();
    }
}
