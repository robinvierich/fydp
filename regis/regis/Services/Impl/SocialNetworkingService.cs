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

namespace Regis.Services.Impl
{
    [Export(typeof(ISocialNetworkingService))]
    public class SocialNetworkingService : ISocialNetworkingService
    {
        private TwitterService _twitter;
        private FacebookClient _fb;

        private OAuthRequestToken _twtrRequestToken;
        private OAuthAccessToken _twtrAccessToken;

        private const string _twtrConsumerKey = "zNYoMCNvLihkyufESnZg";
        private const string _twtrConsumerSecret = "p8AQdU6Gw1gcctl2rt7ka7qffMflttbm0hquLN40";

        private const string _fbAppId = @"323830247717227";
        private const string _fbAppSecret = @"db46aa98e570af177df8316dd351c3ed";
        private const string _fbRedirectUrl = "http://localhost/Facebook/oauth/oauth-redirect.aspx";
        private const string _fbPermissions = "user_about_me,read_stream,publish_stream"; // Set your permissions here

        public SocialNetworkingService()
        {
            // Pass your credentials to the service
            _twitter = new TwitterService(_twtrConsumerKey, _twtrConsumerSecret);
            _fb = new FacebookClient() {
                AppId = _fbAppId,
                AppSecret = _fbAppSecret
            };
        }

        #region ISocialNetworkingService
        #region Twitter
        /// <summary>
        /// Will launch browser and prompt user to authorize. After this, user will get a pin that must then be entered into our software.
        /// </summary>
        public void GetTwitterPin()
        {
            // Step 1 - Retrieve an OAuth Request Token
            _twtrRequestToken = _twitter.GetRequestToken();

            // Step 2 - Redirect to the OAuth Authorization URL
            Uri uri = _twitter.GetAuthorizationUri(_twtrRequestToken);

            Process browserProc;
            browserProc = new Process();
            browserProc.StartInfo = new ProcessStartInfo(uri.ToString());
            browserProc.Start();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pin">Input by user from twitter.com</param>
        public void AuthTwitter(string pin)
        {
            // Step 3 - Exchange the Request Token for an Access Token
            Raise_TwitterAuthUpdated(AuthorizationStatus.AuthorizingPin);
            _twtrAccessToken = _twitter.GetAccessToken(_twtrRequestToken, pin);

            // Step 4 - User authenticates using the Access Token
            _twitter.AuthenticateWith(_twtrAccessToken.Token, _twtrAccessToken.TokenSecret);
        }

        public void PostToTwitter(string text)
        {
            if (_twtrAccessToken == null)
            {
                GetTwitterPin();
                return;
            }
            _twitter.AuthenticateWith(_twtrConsumerKey, _twtrConsumerSecret, _twtrAccessToken.Token, _twtrAccessToken.TokenSecret);
            TwitterStatus status = _twitter.SendTweet(text);
        }
        #endregion

        #region Facebook
        public void AuthFacebook(string pin)
        {
            throw new NotImplementedException();
        }
        public void PostToFacebook(string message)
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion
        
        public event EventHandler<TwitterAuthEventArgs> TwitterAuthUpdated;
        private void Raise_TwitterAuthUpdated(AuthorizationStatus newStatus)
        {
            TwitterAuthEventArgs args = new TwitterAuthEventArgs(newStatus);
            EventHandler<TwitterAuthEventArgs> h = TwitterAuthUpdated;
            if (h == null) return;

            h(this, args);
        }
    }
}
