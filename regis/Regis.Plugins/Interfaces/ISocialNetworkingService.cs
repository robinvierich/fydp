using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regis.Plugins.Interfaces
{
    public interface ISocialNetworkingService
    {
        void AuthTwitter1();
        void AuthTwitter2(string strPin);
        void PostToTwitter(string value);
        void PostToFacebook(string value);
        void FacebookAuth();
    }
}
