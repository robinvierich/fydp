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
using Regis.Plugins.Models;

namespace RegisTunerPlugin
{
    /// <summary>
    /// Interaction logic for TunerControl.xaml
    /// </summary>
    /// 
    [Export(typeof(IPlugin))]
    public partial class TunerControl : UserControl, IPlugin
    {
        public TunerControl()
        {
            InitializeComponent();
        }

        #region IPlugin
        
        public FrameworkElement GetVisualContent()
        {
            return this;
        }


        public string PluginName
        {
            get
            {
                return "TunerPlugin";
            }
        }

        public void Load()
        {
            
        }

        public string FriendlyPluginName
        {
            get { return "Tuner"; }
        }


        public NoteDetectionAlgorithm Algorithm
        {
            get { throw new NotImplementedException(); }
        }
        #endregion
    }
}
