using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Regis.Plugins.Models
{
    public class TimeSignature
    {
        public TimeSignature() {
        }

        ///// <summary>
        ///// Constructs a time signature from a string.
        ///// </summary>
        ///// <param name="fromStr">String of format "topNumber/bottomNumber"</param>
        //public TimeSignature(string fromStr) {
        //    if (!fromStr.Contains('/'))
        //        throw new ArgumentException("fromStr should be formatted: \"topNum/bottomNum\" (ex: 4/4)");

        //    Regex.IsMatch(fromStr, @"[\d]+/[\d]+");
        //}

        #region TopNumber
        private uint _TopNumber;

        [DataMember]
        public uint TopNumber {
            get { return _TopNumber; }
            set {
                _TopNumber = value;
            }
        }
        #endregion

        #region BottomNumber
        private uint _BottomNumber;

        [DataMember]
        public uint BottomNumber {
            get { return _BottomNumber; }
            set {
                _BottomNumber = value;
            }
        }
        #endregion
    }
}
