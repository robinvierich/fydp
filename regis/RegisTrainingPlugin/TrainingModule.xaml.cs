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

namespace RegisTrainingPlugin
{

    [Export(typeof(IPlugin))]
    public partial class TrainingModule : UserControl, IPlugin, IPartImportsSatisfiedNotification
    {
        public TrainingModule() {
            InitializeComponent();
        }

        [Import]
        private TrainingViewModel ViewModel {
            get;
            set;
        }

        public void OnImportsSatisfied() {
            DataContext = ViewModel;
        }

        public void Load() {
        }

        public FrameworkElement GetVisualContent() {
            return this;
        }

        public string PluginName {
            get {
                return "TrainingPlugin"; 
            }
        }

        public string FriendlyPluginName {
            get { return "Training Plugin"; }

        }

        public string PluginIcon {
            get { return "/Regis;component/Images/Playalong.png"; }
        }

        public PluginLayout Layout {
            get { return PluginLayout.Button; }
        }

        
    }
}
