using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Base.ViewModels;
using Regis.Plugins.Interfaces;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Threading;

namespace Regis.ViewModels
{
    [Export]
    class AchievementViewModel:BaseViewModel, IPartImportsSatisfiedNotification
    {
        [Import]
        IAchievementService _achievementService;
        DispatcherTimer _timer;

        public void OnImportsSatisfied()
        {
            _achievementService.NewAchievement += new EventHandler<NewAchievementEventArgs>(_achievementService_NewAchievement);
            AchievementPopup = "Collapsed";
            _timer = new DispatcherTimer() {Interval = TimeSpan.FromSeconds(4)};
            _timer.Tick += new EventHandler(_timer_Tick);
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            AchievementPopup = "Collapsed";
            _timer.Stop();
        }

        void _achievementService_NewAchievement(object sender, NewAchievementEventArgs e)
        {
            AchvPicSrc = e.Achievement.Image;
            AchvTxt = e.Achievement.String;
            AchievementPopup = "Visible";
            _timer.Start();
            

        }

        private string _AchvPicSrc;
        private PropertyChangedEventArgs _AchvPicSrc_ChangedArgs = new PropertyChangedEventArgs("AchvPicSrc");
        public string AchvPicSrc
        {
            get { 
                return _AchvPicSrc; 
            }
            set { 
                _AchvPicSrc = value;
                NotifyPropertyChanged(_AchvPicSrc_ChangedArgs);
            }
        }

        private string _AchvTxt;
        private static PropertyChangedEventArgs _AchvTxt_ChangedArgs = new PropertyChangedEventArgs("AchvTxt");
        public string AchvTxt
        {
            get
            {
                return _AchvTxt;
            }
            set
            {
                _AchvTxt = value;
                NotifyPropertyChanged(_AchvTxt_ChangedArgs);
            }
        }

        private string _AchievementPopup;
        private static PropertyChangedEventArgs _AchievementPopup_Changed = new PropertyChangedEventArgs("AchievementPopup");
        public string AchievementPopup
        {
            get {return _AchievementPopup;}
            set
            {
                _AchievementPopup = value;
                NotifyPropertyChanged(_AchievementPopup_Changed);
            }
        }
        


    }
}
