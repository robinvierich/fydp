using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Base.ViewModels;
using System.ComponentModel;
using Regis.Commands;
using System.ComponentModel.Composition;
using Regis.Plugins.Interfaces;

namespace Regis.ViewModels
{
    [Export(typeof(AuthorizeTwitterViewModel))]
    public class AuthorizeTwitterViewModel: BaseViewModel, IPartImportsSatisfiedNotification
    {

        [Import]
        private ISocialNetworkingService _socialNetworkingService;

        public void OnImportsSatisfied()
        {
            _socialNetworkingService.TwitterAuthUpdated += new EventHandler<TwitterAuthEventArgs>(_socialNetworkingService_TwitterAuthUpdated);
        }

        private void _socialNetworkingService_TwitterAuthUpdated(object sender, TwitterAuthEventArgs args)
        {
            AuthStatus = args.newStatus;
        }

        #region AuthorizationStatus
        private AuthorizationStatus _AuthorizationStatus;
        private static PropertyChangedEventArgs _AuthorizationStatus_ChangedEventArgs = new PropertyChangedEventArgs("AuthStatus");

        public AuthorizationStatus AuthStatus
        {
            get { return _AuthorizationStatus; }
            set
            {
                _AuthorizationStatus = value;
                NotifyPropertyChanged(_AuthorizationStatus_ChangedEventArgs);
            }
        }
        #endregion

        #region Pin
        private string _Pin;
        private static PropertyChangedEventArgs _Pin_ChangedEventArgs = new PropertyChangedEventArgs("Pin");

        public string Pin
        {
            get { return _Pin; }
            set
            {
                _Pin = value;
                NotifyPropertyChanged(_Pin_ChangedEventArgs);
                AuthorizeTwitter.Raise_CanExecuteChanged();
            }
        }
        #endregion

        #region AuthorizeTwitter
        [Import]
        private AuthorizeTwitterCommand _AuthorizeTwitter;
        private static PropertyChangedEventArgs _AuthorizeTwitter_ChangedEventArgs = new PropertyChangedEventArgs("AuthorizeTwitter");

        public AuthorizeTwitterCommand AuthorizeTwitter
        {
            get { return _AuthorizeTwitter; }
            set
            {
                _AuthorizeTwitter = value;
                NotifyPropertyChanged(_AuthorizeTwitter_ChangedEventArgs);
            }
        }
        #endregion

        #region GetTwitterPin
        [Import]
        private GetTwitterPinCommand _GetTwitterPin;
        private static PropertyChangedEventArgs _GetTwitterPin_ChangedEventArgs = new PropertyChangedEventArgs("GetTwitterPin");

        public GetTwitterPinCommand GetTwitterPin
        {
            get { return _GetTwitterPin; }
            set
            {
                _GetTwitterPin = value;
                NotifyPropertyChanged(_GetTwitterPin_ChangedEventArgs);
            }
        }
        #endregion

    }
}
