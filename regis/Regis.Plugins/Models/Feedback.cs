using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regis.Plugins.Models
{
    public class Feedback
    {
        public string FeedbackString {
            get;
            set;
        }

        public override string ToString() {
            return FeedbackString;
        }
    }
}
