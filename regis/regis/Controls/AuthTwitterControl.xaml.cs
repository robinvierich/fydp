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
using Regis.Plugins.Interfaces;
using System.ComponentModel.Composition;
using Regis.ViewModels;

namespace Regis.Controls
{
    /// <summary>
    /// Control to connect to and authorize twitter
    /// </summary>
    [Export(typeof(IPlugin))]
    public partial class AuthTwitterControl : UserControl, IPlugin, IPartImportsSatisfiedNotification
    {
        public AuthTwitterControl()
        {
            InitializeComponent();
        }

        public void Load()
        {
        }

        public FrameworkElement GetVisualContent()
        {
            return this;
        }

        public string PluginName
        {
            get { return "AuthTwitterControl"; }
        }

        public string FriendlyPluginName
        {
            get { return "Login to Twitter"; }
        }

        [Import(typeof(AuthorizeTwitterViewModel))]
        private AuthorizeTwitterViewModel ViewModel
        {
            get;
            set;
        }

        public void OnImportsSatisfied()
        {
            DataContext = ViewModel;
        }
    }
}
