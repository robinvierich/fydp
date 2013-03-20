using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regis.Plugins.Models
{
    public class GoodPitchFeedback: Feedback
    {
        public override string FeedbackString {
            get { return "Correct Note!"; }
        }
    }
}
