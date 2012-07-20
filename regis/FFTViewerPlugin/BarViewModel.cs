using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Regis.Base.ViewModels;

namespace FFTViewerPlugin
{
    public class BarViewModel: BaseViewModel
    {
        #region Width
        private double _Width;
        private static PropertyChangedEventArgs _Width_ChangedEventArgs = new PropertyChangedEventArgs("Width");

        public double Width
        {
            get { return _Width; }
            set
            {
                _Width = value;
                NotifyPropertyChanged(_Width_ChangedEventArgs);
            }
        }
        #endregion

        #region Height
        private double _Height;
        private static PropertyChangedEventArgs _Height_ChangedEventArgs = new PropertyChangedEventArgs("Height");

        public double Height
        {
            get { return _Height; }
            set
            {
                _Height = value;
                NotifyPropertyChanged(_Height_ChangedEventArgs);
            }
        }
        #endregion
    }
}
