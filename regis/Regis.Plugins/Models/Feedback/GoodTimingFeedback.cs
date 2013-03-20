using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regis.Plugins.Models
{
    public class GoodTimingFeedback: Feedback
    {
        string fastOrSlow = string.Empty;
        double timeDifference = 0;

        public GoodTimingFeedback(double timeDifference)
        {
            this.timeDifference = timeDifference;
            fastOrSlow = timeDifference < 0 ? "fast" : "slow";
        }

        public override string FeedbackString {
            get {
                return "Good Timing!";
            }
           
        }
    }
}
