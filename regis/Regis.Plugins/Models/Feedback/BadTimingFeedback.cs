using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regis.Plugins.Models
{
    public class BadTimingFeedback: Feedback
    {
        string fastOrSlow = string.Empty;
        double timeDifference = 0;

        public BadTimingFeedback(double timeDifference)
        {
            this.timeDifference = timeDifference;
            fastOrSlow = timeDifference < 0 ? "fast" : "slow";
        }

        public override string FeedbackString {
            get {
                return String.Format("You were {0}", fastOrSlow);
            }
           
        }
    }
}
