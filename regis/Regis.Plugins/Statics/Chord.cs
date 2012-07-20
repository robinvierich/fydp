using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regis.Plugins.Statics
{
    public class Chord
    {
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        private List<double> _frequencies;
        public List<double> Frequencies
        {
            get
            {
                return _frequencies;
            }

            set
            {
                _frequencies = value;
            }

        }

        private char _charValue;
        public char CharValue
        {
            get { return _charValue; }
            set { _charValue = value; }
        }

    }
}
