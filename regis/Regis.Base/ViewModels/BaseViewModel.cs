using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Regis.Base.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        protected BaseViewModel()
        {
        }

        protected void NotifyPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventHandler h = PropertyChanged;
            if (h == null) return;

            h(this, args);
        }

        

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
