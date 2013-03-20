using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Plugins.Models;

namespace RegisPlayAlongPlugin
{
    class SocialMediaAchievement:Achievement
    {
        static int _count;

        public override string String
        {
            get { return "Shared to Social Media"; }
        }

        public override int Count
        {
            get { return _count; }
        }

        public override string Image
        {
            get { return "REGISlogo.png"; }
        }
    }
}
