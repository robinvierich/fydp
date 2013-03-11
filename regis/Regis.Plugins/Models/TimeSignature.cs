using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Regis.Plugins.Models
{
    public class TimeSignature
    {
        #region BeatsPerBar
        private uint _BeatsPerBar;

        [DataMember]
        public uint BeatsPerBar {
            get { return _BeatsPerBar; }
            set {
                _BeatsPerBar = value;
            }
        }
        #endregion

        #region BeatValue
        private uint _BeatValue;

        [DataMember]
        public uint BeatValue {
            get { return _BeatValue; }
            set {
                _BeatValue = value;
            }
        }
        #endregion
    }
}
