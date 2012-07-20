using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Regis.Commands;
using Regis.Plugins.Interfaces;
using TweetSharp;
using System.Diagnostics;

namespace Regis.Services
{
    [Export(typeof(ISocialNetworkingService))]
    public class SocialNetworkingService : ISocialNetworkingService
    {
        TwitterService service;
        OAuthRequestToken requestToken;

        private OAuthAccessToken _accessToken = null;
        private string consumerKey = "zNYoMCNvLihkyufESnZg";
        private string consumerSecret = "p8AQdU6Gw1gcctl2rt7ka7qffMflttbm0hquLN40";

        public SocialNetworkingService()
        {
            // Pass your credentials to the service
            service = new TwitterService(consumerKey, consumerSecret);
        }

        public void AuthTwitter1()
        {
            // Step 1 - Retrieve an OAuth Request Token
            requestToken = service.GetRequestToken();

            // Step 2 - Redirect to the OAuth Authorization URL
            Uri uri = service.GetAuthorizationUri(requestToken);
            Process.Start(uri.ToString());
        }

        public void AuthTwitter2(string strPin)
        {
            // Step 3 - Exchange the Request Token for an Access Token
            string verifier = strPin; // <-- This is input into your application by your user
            _accessToken = service.GetAccessToken(requestToken, verifier);

            // Step 4 - User authenticates using the Access Token
            service.AuthenticateWith(_accessToken.Token, _accessToken.TokenSecret);
            IEnumerable<TwitterStatus> mentions = service.ListTweetsMentioningMe();
        }

        public void PostToTwitter(string value)
        {
            service.AuthenticateWith(consumerKey, consumerSecret, _accessToken.Token, _accessToken.TokenSecret);
                TwitterStatus status = service.SendTweet(value);
        }

        public void PostToFacebook(string value)
        {
            
        }
    }
}
