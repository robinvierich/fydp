using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace Regis.Services.Realtime
{
    /// <summary>
    /// Base interface for realtime services (constant polling/interrupts) 
    /// </summary>
    /// <typeparam name="T">Type of args to pass to Start() method.</typeparam>
    interface IRealtimeService<T>
    {
        void Start(T args);
        void Stop();
    }
}
