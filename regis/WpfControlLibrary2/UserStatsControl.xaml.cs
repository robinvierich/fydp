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

namespace RegisUserStatsPlugin
{
    /// <summary>
    /// Interaction logic for UserStatsControl.xaml
    /// </summary>

    [Export(typeof(IPlugin))]
    public partial class UserStatsControl : UserControl, IPlugin, IPartImportsSatisfiedNotification
    {
        public UserStatsControl()
        {
            InitializeComponent();
        }

        public void OnImportsSatisfied()
        {
            DataContext = ViewModel;
        }

        [Import]
        public UserStatsViewModel ViewModel
        {
            get;
            set;
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
            get { return "UserStatsPlugin"; }
        }

        public string FriendlyPluginName
        {
            get { return "User Statistics"; }
        }

        #endregion
    }
}
