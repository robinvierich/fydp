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

namespace RegisPlayAlongPlugin
{
    [Export(typeof(IPlugin))]
    public partial class PlayAlongControl : UserControl, IPartImportsSatisfiedNotification, IPlugin
    {
        public PlayAlongControl() {
            InitializeComponent();
        }

        public void OnImportsSatisfied() {
            DataContext = ViewModel;
        }

        [Import]
        public PlayAlongViewModel ViewModel {
            get;
            set;
        }

        public void Load() {
        }

        public FrameworkElement GetVisualContent() {
            return this;
        }

        public string PluginName {
            get { return "PlayAlongControl"; }
        }

        public string FriendlyPluginName {
            get { return "Play Along"; }
        }

        public string PluginIcon
        {
            get { return "/Regis;component/Images/Playalong.png"; }
        }

        public PluginLayout Layout {
            get { return PluginLayout.Button; }
        }

        private void StaffControl_StaffEndReached(object sender, Regis.Plugins.Controls.EndOfStaffEventArgs e) {
            e.Cancel = true;
            ViewModel.Start();
        }
    }
}
