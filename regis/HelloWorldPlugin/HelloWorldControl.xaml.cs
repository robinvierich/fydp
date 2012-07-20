using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using Regis.Plugins.Interfaces;

namespace HelloWorldPlugin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    
    

    [Export(typeof(IPlugin))]
    public partial class HelloWorldControl : UserControl, IPlugin
    {
        [Import]
        private ISocialNetworkingService _socialNetworkingService = null;

        public HelloWorldControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)e.OriginalSource;
            b.Content = "HELLO WORLD";
        }


        private void btnSendTweet_Click(object sender, RoutedEventArgs e)
        {
            _socialNetworkingService.PostToTwitter(this.txtTweet.Text);
        }

        private void btnFacebook_Click(object sender, RoutedEventArgs e)
        {
            _socialNetworkingService.PostToFacebook(this.txtFacebook.Text);
        }

        #region IPlugin
        public void Load()
        {
            
        }

        public FrameworkElement GetVisualContent()
        {
            return this;
        }

        public string PluginName
        {
            get 
            {
                return "HelloWorldPlugin";
            }
        }

        public string FriendlyPluginName
        {
            get
            {
                return "Social Media";
            }
        }
        #endregion

    }
}
