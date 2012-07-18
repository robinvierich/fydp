using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegisTunerPlugin.Models
{
    public class GuitarString
    {
        private string _stringName;
        public string StringName
        {
            get { return _stringName; }
            set { _stringName = value; }
        }
        private double _frequency;

        public double Frequency
        {
            get { return _frequency; }
            set { _frequency = value; }
        }

        private int _stringNum;
        public int StringNum
        {
            get { return _stringNum; }
            set { _stringNum = value; }
        }

    }
}
