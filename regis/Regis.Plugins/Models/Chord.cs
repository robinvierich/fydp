using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regis.Plugins.Models
{
    public class Chord
    {
        private string _name;
        public string Name {
            get {
                return _name;
            }
            set {
                _name = value;
            }
        }

        private IList<Note> _Notes;
        public IList<Note> Notes {
            get { return _Notes; }
            set { _Notes = value; }
        }

        private char _charValue;
        public char CharValue {
            get { return _charValue; }
            set { _charValue = value; }
        }

    }
}
