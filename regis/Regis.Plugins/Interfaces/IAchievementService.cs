using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Plugins.Models;

namespace Regis.Plugins.Interfaces
{

    public interface IAchievementService
    {
        void SetAchievement(Achievement achievement); 
        event EventHandler<NewAchievementEventArgs> NewAchievement;
    }

    public class NewAchievementEventArgs : EventArgs
    {
        public NewAchievementEventArgs(Achievement achievement) {
            Achievement = achievement;
        }


        public Achievement Achievement { get; private set; }
    }
}
