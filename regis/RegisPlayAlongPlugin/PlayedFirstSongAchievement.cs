using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Plugins.Models;

namespace RegisPlayAlongPlugin
{
    public class PlayedFirstSongAchievement : Achievement
    {

        static int _Count;

        static PlayedFirstSongAchievement() {
            _Count++;
        }

        public override string String {
            get { return "Played first song!"; }
        }

        public override int Count {
            get { return _Count; }
        }

        public override string Image
        {
            get { return "D:\\GitHub\\fydp\\regis\\regis\\Images\\REGISlogo.png"; }
        }
    }
}
