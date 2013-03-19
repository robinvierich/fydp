using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Plugins.Interfaces;
using Regis.Plugins.Models;
using System.ComponentModel.Composition;

namespace Regis.Services.Impl
{
    [Export(typeof(IAchievementService))]
    public class AchievementService: IAchievementService
    {
        private List<Achievement> _achievements;

        AchievementService() {
            _achievements = new List<Achievement>();
        }

        public void SetAchievement(Achievement achievement) {
            _achievements.Add(achievement);
            Raise_NewAchievement(achievement);
        }

        public event EventHandler<NewAchievementEventArgs> NewAchievement;

        private void Raise_NewAchievement(Achievement achievement) {
            EventHandler<NewAchievementEventArgs> h = NewAchievement;
            if (h == null) return;

            h(this, new NewAchievementEventArgs(achievement));
        }
    }
}
