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

        double maxTimeDiffForMatch = 0.1; // s

        private Dictionary<Note, Note> GetMatches(IList<Note> playedNotes, IList<Note> goalNotes){
            Dictionary<Note, Note> matches = new Dictionary<Note, Note>();

            // look for notes where time difference is within a threshold (maxTimeDiffForMatch)
            foreach (Note goalNote in goalNotes) {
                IList<Note> matchedNotes = playedNotes.Where(playedNote => {
                    return Math.Abs((playedNote.startTime - goalNote.startTime).TotalSeconds) < maxTimeDiffForMatch;
                }).ToList();

                if (matchedNotes.Count > 0) {
                    int i = 0;
                }
            }

            return matches;
        }

        public Feedback GetFeedback(IList<Note> playedNotes, IList<Note> goalNotes) {
            string feedbackString = string.Empty;

            Dictionary<Note, Note> matches = GetMatches(playedNotes, goalNotes);
            if (matches.Count > 0) {
                int i = 0;
            }

            return new Feedback() { FeedbackString = feedbackString };
        }
    }
}
