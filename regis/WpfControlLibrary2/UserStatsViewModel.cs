using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Base.ViewModels;
using System.ComponentModel;
using Regis.Plugins.Interfaces;
using System.ComponentModel.Composition;
using Regis.Plugins.Models;

namespace RegisUserStatsPlugin
{
    [Export]
    public class UserStatsViewModel: BaseViewModel, IPartImportsSatisfiedNotification
    {
        [Import]
        private IUserService _userService = null;

        #region CurrentUser
        private User _currentUser;
        private static PropertyChangedEventArgs _CurrentUser_ChangedEventArgs = new PropertyChangedEventArgs("CurrentUser");

        public User CurrentUser
        {
            get { return _currentUser; }
            set
            {
                _currentUser = value;
                NotifyPropertyChanged(_CurrentUser_ChangedEventArgs);
            }
        }
        #endregion


        public void OnImportsSatisfied()
        {
            CurrentUser = _userService.GetCurrentUser();
        }
    }
}
