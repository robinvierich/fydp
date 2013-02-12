using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Regis.Base.ViewModels;

namespace Regis.ViewModels
{
    public class PostToTwitterViewModel : BaseViewModel
    {
        #region RemainingCharacters
        private uint _RemainingCharacters;
        private static PropertyChangedEventArgs _RemainingCharacters_ChangedEventArgs = new PropertyChangedEventArgs("RemainingCharacters");

        public uint RemainingCharacters
        {
            get { return _RemainingCharacters; }
            set
            {
                _RemainingCharacters = value;
                NotifyPropertyChanged(_RemainingCharacters_ChangedEventArgs);
            }
        }
        #endregion
    }
}
