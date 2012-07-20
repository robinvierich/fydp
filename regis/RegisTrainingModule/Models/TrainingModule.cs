using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegisTrainingModule.Models
{
    class TrainingModule
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private double[] _targetFreq;
        public double[] TargetFreq
        {
            get { return _targetFreq; }
            set { _targetFreq = value; }
        }
    }
}
