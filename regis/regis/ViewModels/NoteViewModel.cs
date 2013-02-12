using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Base.ViewModels;
using Regis.Plugins.Models;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Regis.ViewModels
{
    public class NoteViewModel: BaseViewModel
    {

        #region properties
        #region StartTime
        private DateTime _StartTime;
        private static PropertyChangedEventArgs _StartTime_ChangedEventArgs = new PropertyChangedEventArgs("StartTime");

        [DataMember]
        public DateTime StartTime
        {
            get { return _StartTime; }
            set
            {
                _StartTime = value;
                NotifyPropertyChanged(_StartTime_ChangedEventArgs);
                NotifyPropertyChanged(_Duration_ChangedEventArgs);
            }
        }
        #endregion

        #region EndTime
        private DateTime _EndTime;
        private static PropertyChangedEventArgs _EndTime_ChangedEventArgs = new PropertyChangedEventArgs("EndTime");

        [DataMember]
        public DateTime EndTime
        {
            get { return _EndTime; }
            set
            {
                _EndTime = value;
                NotifyPropertyChanged(_EndTime_ChangedEventArgs);
                NotifyPropertyChanged(_Duration_ChangedEventArgs);
            }
        }
        #endregion

        #region Frequency
        private double _Frequency;
        private static PropertyChangedEventArgs _Frequency_ChangedEventArgs = new PropertyChangedEventArgs("Frequency");

        [DataMember]
        public double Frequency
        {
            get { return _Frequency; }
            set
            {
                _Frequency = value;
                NotifyPropertyChanged(_Frequency_ChangedEventArgs);
            }
        }
        #endregion

        #region ClosestRealNoteFrequency
        private double _ClosestRealNoteFrequency;
        private static PropertyChangedEventArgs _ClosestRealNoteFrequency_ChangedEventArgs = new PropertyChangedEventArgs("ClosestRealNoteFrequency");

        [DataMember]
        public double ClosestRealNoteFrequency
        {
            get { return _ClosestRealNoteFrequency; }
            set
            {
                _ClosestRealNoteFrequency = value;
                NotifyPropertyChanged(_ClosestRealNoteFrequency_ChangedEventArgs);
            }
        }
        #endregion

        #region Duration
        private static PropertyChangedEventArgs _Duration_ChangedEventArgs = new PropertyChangedEventArgs("Duration");
        public TimeSpan Duration
        {
            get { return EndTime - StartTime; }
        }
        #endregion
        #endregion

        #region public methods

        public void SetNote(Note note)
        {
            StartTime = note.startTime;
            EndTime = note.endTime;
            Frequency = note.frequency;
            ClosestRealNoteFrequency = note.frequency;
        }        
        #endregion



    }
}
