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
using Regis.Plugins.Models;

namespace RegisTrainingModule
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class SummaryBox : UserControl, IPartImportsSatisfiedNotification
    {
        [Import]
        private ISocialNetworkingService _socialNetworkingService;

        [Import]
        private IUserService _userService;

        public SummaryBox()
        {
            InitializeComponent();
            Regis.Composition.Importer.Compose(this);
    
        }

        private void btnPostFacebook_Click(object sender, RoutedEventArgs e)
        {
            string poststr = "";
            poststr += "I have played ";
            poststr += txtPercentCorrect.Text;
            poststr += "% successfull notes of ";
            poststr += txtNotesPlayed.Text;
            poststr += " total notes on R.E.G.I.S ";
            _socialNetworkingService.PostToFacebook(poststr);
        }

        private void btnPostTwitter_Click(object sender, RoutedEventArgs e)
        {
            string tweetstr = "";
            tweetstr += "I have played ";
            tweetstr += txtPercentCorrect.Text;
            tweetstr += "% successfull notes of ";
            tweetstr += txtNotesPlayed.Text;
            tweetstr += " total notes on R.E.G.I.S ";
            _socialNetworkingService.PostToTwitter(tweetstr);
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



        public void OnImportsSatisfied()
        {
            User currentUser = _userService.GetCurrentUser();

            if (currentUser.TrainingStats.Count == 0)
                return;

            UserTrainingStats trainingStats = currentUser.TrainingStats[currentUser.TrainingStats.Count -1];

            txtDate.Text = trainingStats.TimeStamp.ToString();
            txtNotesPlayed.Text = trainingStats.TotalNotesPlayed.ToString();
            txtPercentCorrect.Text = trainingStats.PercentCorrectNotes.ToString();
        }
    }
}
