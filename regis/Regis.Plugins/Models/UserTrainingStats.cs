using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Regis.Plugins.Models
{
    [DataContract]
    public class UserTrainingStats
    {
        private int _totalNotesPlayed;
        [DataMember]
        public int TotalNotesPlayed
        {
            get { return _totalNotesPlayed; }
            set { _totalNotesPlayed = value; }
        }

        private double _percentCorrectNotes;
        [DataMember]
        public double PercentCorrectNotes
        {
            get { return _percentCorrectNotes; }
            set { _percentCorrectNotes = value; }
        }

        private DateTime _timeStamp;
        [DataMember]
        public DateTime TimeStamp
        {
            get { return _timeStamp; }
            set { _timeStamp = value; }
        }

        private DateTime _startTimeStamp;
        [DataMember]
        public DateTime startTimeStamp
        {
            get { return _startTimeStamp; }
            set { _startTimeStamp = value; }
        }

    }
}
