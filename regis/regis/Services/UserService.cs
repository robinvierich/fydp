using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Regis.Plugins.Interfaces;
using Regis.Plugins.Models;

namespace Regis.Services
{
    [Export (typeof(IUserService))]
    public class UserService: IUserService
    {
        private User _currentUser;

        [Import]
        private IPersistanceService _persistanceService = null;

        public User GetCurrentUser()
        {
            if (_currentUser == null)
            {
                try
                {
                    _currentUser = _persistanceService.Load<User>("user.xml");
                }
                catch 
                {
                    _currentUser = new User() { Name = "Main User" };
                }
            }

            return _currentUser;
        }

        public void SaveCurrentUser()
        {
            if (_currentUser == null)
                return;

            _persistanceService.Save<User>(_currentUser, "user.xml", true);
        }
    }
}
