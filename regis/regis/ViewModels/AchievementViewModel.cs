using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Base.ViewModels;
using Regis.Plugins.Interfaces;
using System.ComponentModel;
using System.ComponentModel.Composition;

namespace Regis.ViewModels
{
    [Export]
    class AchievementViewModel:BaseViewModel, IPartImportsSatisfiedNotification
    {
        [Import]
        IAchievementService _achievementService;

        public void OnImportsSatisfied()
        {
            _achievementService.NewAchievement += new EventHandler<NewAchievementEventArgs>(_achievementService_NewAchievement);
        }

        void _achievementService_NewAchievement(object sender, NewAchievementEventArgs e)
        {
            AchvPicSrc = e.Achievement.Image;
            AchvTxt = e.Achievement.String;
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


    }
}
