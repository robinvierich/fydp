using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Regis.Base.ViewModels;

namespace FFTViewerPlugin
{
    public class FFTBinViewModel : BaseViewModel
    {
        #region BinNumber
        private int _BinNumber;
        private static PropertyChangedEventArgs _BinNumber_ChangedEventArgs = new PropertyChangedEventArgs("BinNumber");

        public int BinNumber
        {
            get { return _BinNumber; }
            set
            {
                _BinNumber = value;
                NotifyPropertyChanged(_BinNumber_ChangedEventArgs);
            }
        }
        #endregion

        #region Power
        private double _Power;
        private static PropertyChangedEventArgs _Power_ChangedEventArgs = new PropertyChangedEventArgs("Power");

        public double Power
        {
            get { return _Power; }
            set
            {
                _Power = value;
                NotifyPropertyChanged(_Power_ChangedEventArgs);
            }
        }
        #endregion
    }
}
