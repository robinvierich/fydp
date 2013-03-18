using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Plugins.Interfaces;
using Regis.Plugins.Models;
using System.ComponentModel.Composition;

namespace Regis.Services.Impl
{
    [Export(typeof(IFeedbackService))]
    public class FeedbackService : IFeedbackService
    {
        public Feedback GetFeedback(IList<Note> playedNotes, IList<Note> goalNotes) {
            string feedbackString = string.Empty;

            if (playedNotes.Count > goalNotes.Count) {
                throw new NotImplementedException("Played notes and goal notes must be the same length");
            }

            for (int i = 0; i < playedNotes.Count; i++) {
                if (playedNotes[i].startTime - goalNotes[i].startTime > TimeSpan.FromSeconds(0.01)) {
                    feedbackString += "Your timing was a bit off ";
                }
            }

            return new Feedback() { FeedbackString = feedbackString };
        }
    }
}
