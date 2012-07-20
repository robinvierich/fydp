using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Regis.Commands;
using Regis.Plugins.Interfaces;
using TweetSharp;
using System.Diagnostics;
using Facebook;
using System.Dynamic;

namespace Regis.Services
{
    [Export(typeof(ISocialNetworkingService))]
    public class SocialNetworkingService : ISocialNetworkingService
    {
        TwitterService service;
        FacebookClient FService;
        OAuthRequestToken requestToken;

        private OAuthAccessToken _accessToken = null;
        private string consumerKey = "zNYoMCNvLihkyufESnZg";
        private string consumerSecret = "p8AQdU6Gw1gcctl2rt7ka7qffMflttbm0hquLN40";
        private string faceConsumerKey = "331770083574694";
        private string faceConsumerSecret = "c9b23ec43741ff8d4ce3245f24ec6390";
        private string faceAccess = "AAAEtvj36g6YBAGG2R0Ee2OW8c4QdBZA7DyOTZC6vKdhO5t6VygznleB9CF5Lpjk3e8VWIM8wojeXFPKZCjtftf18JQEJz5QFZBDd2W42ygZDZD";

        public SocialNetworkingService()
        {
            // Pass your credentials to the service
            service = new TwitterService(consumerKey, consumerSecret);
            FService = new FacebookClient(faceAccess);
        }

        private Process browserProc;
        public void AuthTwitter1()
        {
            // Step 1 - Retrieve an OAuth Request Token
            requestToken = service.GetRequestToken();

            // Step 2 - Redirect to the OAuth Authorization URL
            Uri uri = service.GetAuthorizationUri(requestToken);

            browserProc = new Process();
            browserProc.StartInfo = new ProcessStartInfo(uri.ToString());
            browserProc.Start();
            
        }

        public void AuthTwitter2(string strPin)
        {
            // Step 3 - Exchange the Request Token for an Access Token
            string verifier = strPin; // <-- This is input into your application by your user
            _accessToken = service.GetAccessToken(requestToken, verifier);
            try
            {
                browserProc.Kill();
            }
            catch
            {
            }
        }

        public void PostToTwitter(string value)
        {
            service.AuthenticateWith(consumerKey, consumerSecret, _accessToken.Token, _accessToken.TokenSecret);
            TwitterStatus status = service.SendTweet(value);
        }

        public void PostToFacebook(string message)
        {
            dynamic parameters = new ExpandoObject();
            parameters.message = message;
            FService.Post("me/feed",parameters);
        }
    }
}
