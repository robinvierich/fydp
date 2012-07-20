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
using Regis.Plugins.Interfaces;

namespace FFTViewerPlugin
{
    [Export(typeof(IPlugin))]
    public partial class FFTViewerControl : UserControl, IPlugin, IPartImportsSatisfiedNotification
    {
        public FFTViewerControl()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(FFTViewerControl_Loaded);
            Unloaded += new RoutedEventHandler(FFTViewerControl_Unloaded);
        }

        
        private void thisFFFViewerControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ViewModel.ControlActualHeight = this.ActualHeight;
            ViewModel.ControlActualWidth = this.ActualWidth;
        }

        public void OnImportsSatisfied()
        {
            DataContext = ViewModel;
        }

        void FFTViewerControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null)
                return;

            ViewModel.StopReadingFFT();
        }

        void FFTViewerControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null)
                return;

            ViewModel.StartReadingFFT();
        }

        [Import]
        private FFTViewerViewModel ViewModel
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
            get { return "FFTViewerPlugin"; }
        }

        public string FriendlyPluginName
        {
            get { return "FFT Viewer"; }
        }
        #endregion

        

        
    }
}
