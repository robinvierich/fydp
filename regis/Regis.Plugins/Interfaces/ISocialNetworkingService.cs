using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regis.Plugins.Interfaces
{
    public interface ISocialNetworkingService
    {
        void PostToTwitter(string value);
        void PostToFacebook(string value);
    }
}
