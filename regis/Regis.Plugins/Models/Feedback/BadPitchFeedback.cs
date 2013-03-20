using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regis.Plugins.Models
{
    public class BadPitchFeedback : Feedback
    {
        int semitoneDifference = 0;

        public BadPitchFeedback(int semitoneDifference) {
            this.semitoneDifference = semitoneDifference;
        }

        public int SemitoneDifference {
            get { return semitoneDifference; }
        }

        public override string FeedbackString {
            get { return "Bad Note"; }
        }
    }
}
