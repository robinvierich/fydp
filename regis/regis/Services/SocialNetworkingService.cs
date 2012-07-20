using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Regis.Commands;
using Regis.Plugins.Interfaces;

namespace Regis.Services
{
    [Export(typeof(ISocialNetworkingService))]
    public class SocialNetworkingService : ISocialNetworkingService
    {
        public void PostToTwitter(string value)
        {
            
        }

        public void PostToFacebook(string value)
        {
            
        }
    }
}
