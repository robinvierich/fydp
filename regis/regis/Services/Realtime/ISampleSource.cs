using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Models;
using System.Collections.Concurrent;

namespace Regis.Services.Realtime
{
    public interface ISampleSource
    {
        ConcurrentQueue<SampleCollection> SampleCollectionQueue { get; }
    }
}
