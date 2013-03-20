using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Regis.Plugins.Models;

namespace Regis.Plugins.Interfaces
{
    public interface IFFTSource
    {
        ConcurrentQueue<FFTPower> FFTQueue { get; }
        int FFTSize;
    }
}
