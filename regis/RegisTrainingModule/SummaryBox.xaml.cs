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

namespace RegisTrainingModule
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class SummaryBox : UserControl
    {
        [Import]
        private ISocialNetworkingService _socialNetworkingService;
        
        public SummaryBox()
        {
            InitializeComponent();
            Regis.Composition.Importer.Compose(this);
        }

        private void btnPostFacebook_Click(object sender, RoutedEventArgs e)
        {
            _socialNetworkingService.PostToFacebook("I just successfully played the G major scale on my guitar, using R.E.G.I.S");
        }

        private void btnPostTwitter_Click(object sender, RoutedEventArgs e)
        {
            _socialNetworkingService.PostToTwitter("I just successfully played the G major scale on my guitar, using R.E.G.I.S #REGISRocks");
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {

            DependencyObject parent = this;
            do
            {
                parent = VisualTreeHelper.GetParent(parent);

            } while (parent != null && !(parent is Window));


            Window parentWindow = parent as Window;
            if (parentWindow == null) return;

            parentWindow.Close();
            
        }


    }
}
