using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Base.ViewModels;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Regis.ViewModels
{
    public class FrequencyVsTimeViewModel: BaseViewModel
    {
        #region Value
        private double _Value;
        private static PropertyChangedEventArgs _Value_ChangedEventArgs = new PropertyChangedEventArgs("Value");

        [DataMember]
        public double Value
        {
            get { return _Value; }
            set
            {
                _Value = value;
                NotifyPropertyChanged(_Value_ChangedEventArgs);
            }
        }
        #endregion
        
    }
}
