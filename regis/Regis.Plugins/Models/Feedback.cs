using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regis.Plugins.Models
{
    public abstract class Feedback
    {
        public abstract string FeedbackString {
            get;
        }

        public Note Note {
            get;
            set;
        }

        public override string ToString() {
            return FeedbackString;
        }
    }
}
