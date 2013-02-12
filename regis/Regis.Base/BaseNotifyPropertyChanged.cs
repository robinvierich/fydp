using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Regis.Base
{
    public class BaseNotifyPropertyChanged: INotifyPropertyChanged
    {
        protected void NotifyPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventHandler h = PropertyChanged;
            if (h == null)
                return;

            h(this, args);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
