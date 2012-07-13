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
using Regis.Plugins;
using System.ComponentModel.Composition;

namespace HelloWorldPlugin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    [Export(typeof(IPlugin))]
    public partial class HelloWorldControl : UserControl, IPlugin
    {
        public HelloWorldControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)e.OriginalSource;
            b.Content = "HELLO WORLD";
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
            get { return "HelloWorldPlugin"; }
        }

        public string FriendlyPluginName
        {
            get { return "Hello World"; }
        }


        public NoteDetectionAlgorithm Algorithm
        {
            get { throw new NotImplementedException(); }
        }
        #endregion
    }
}
