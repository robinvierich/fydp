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
using Regis.Plugins;

namespace RegisTrainingModule
{
    /// <summary>
    /// Interaction logic for TrainingControl.xaml
    /// </summary>
    
    [Export(typeof(IPlugin))]
    public partial class TrainingControl : UserControl, IPlugin
    {
        public TrainingControl()
        {
            InitializeComponent();
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
            get { return "TrainingControlPlugin"; }
        }

        public string FriendlyPluginName
        {
            get { return "TrainingModule"; }
        }

        public NoteDetectionAlgorithm Algorithm
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
