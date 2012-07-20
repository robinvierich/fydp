using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regis.Plugins.Models
{
    public struct Note
    {
        public DateTime timeStamp;
        public double frequency;
        public double closestRealNoteFrequency;
    }
}
