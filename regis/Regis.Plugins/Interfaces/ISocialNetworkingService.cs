using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regis.Plugins.Interfaces
{    public enum AuthorizationStatus
    {
        LoggedOut,
        AuthorizingPin,
        LoggedIn,
    }

    public class TwitterAuthEventArgs: EventArgs
    {
        public AuthorizationStatus newStatus;
        public TwitterAuthEventArgs(AuthorizationStatus newStatus)
        {
            this.newStatus = newStatus;
        }
    }

    public interface ISocialNetworkingService
    {
        void GetTwitterPin();
        void AuthTwitter(string pin);
        event EventHandler<TwitterAuthEventArgs> TwitterAuthUpdated;

        void PostToTwitter(string value);
        void PostToFacebook(string value);
        void AuthFacebook(string pin);
    }
}
