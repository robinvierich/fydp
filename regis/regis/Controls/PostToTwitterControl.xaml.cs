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

namespace Regis.Controls
{
    /// <summary>
    /// Interaction logic for PostTwitterControl.xaml
    /// </summary>
    [Export(typeof(IPlugin))]
    public partial class PostToTwitterControl : UserControl, IPlugin
    {
        public PostToTwitterControl()
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
            get { return "PostToTwitterControl"; }
        }

        public string FriendlyPluginName
        {
            get { return "Post to Twitter"; }
        }
    }
}
