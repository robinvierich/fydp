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

namespace TestModule
{
    /// <summary>
    /// Interaction logic for TestControl.xaml
    /// </summary>
    public partial class TestControl : UserControl, IPartImportsSatisfiedNotification, IPlugin
    {
        public TestControl() {
            InitializeComponent();
        }

        [Import]
        public TestViewModel ViewModel {
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
                return "TestPlugin";
            }
        }

        public string FriendlyPluginName {
            get {
                return "Test Plugin";
            }
        }
    }
}
