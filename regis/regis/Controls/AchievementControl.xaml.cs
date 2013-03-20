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
using Regis.ViewModels;
using System.ComponentModel.Composition;
using Regis.Composition;

namespace Regis.Controls
{
    /// <summary>
    /// Interaction logic for AchievementControl.xaml
    /// </summary>
    
    public partial class AchievementControl: IPartImportsSatisfiedNotification
    {
        public AchievementControl()
        {
            InitializeComponent();
            Importer.Compose(this);
        }

        [Import]
        private AchievementViewModel ViewModel
        {
            get;
            set;
        }

        public void OnImportsSatisfied()
        {
            DataContext = ViewModel;
        }
    }
}
