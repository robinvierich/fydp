using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regis.Plugins.Models
{
    public abstract class Achievement
    {
        public abstract string String {
            get;
        }

        public abstract int Count { 
            get;
        }
    }
}
