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
using RegisFreeFormPlugin.ViewModels;


namespace RegisFreeFormPlugin
{
    /// <summary>
    /// Interaction logic for FreeFormControl.xaml
    /// </summary>

    [Export(typeof(IPlugin))]
    public partial class FreeFormControl : UserControl, IPlugin, IPartImportsSatisfiedNotification
    {
        public FreeFormControl()
        {
            InitializeComponent();
        }

        [Import]
        private FreeFormViewModel ViewModel
        {
            get;
            set;
        }

        public void OnImportsSatisfied()
        {
            DataContext = ViewModel;
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
            get { return "FreeFormPlugin"; }
        }

        public string FriendlyPluginName
        {
            get { return "FreeFormMode"; }
        }

        public NoteDetectionAlgorithm Algorithm
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
