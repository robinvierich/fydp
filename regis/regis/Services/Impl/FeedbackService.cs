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

        private Dictionary<Note, List<Note>> GetMatches(IList<Note> playedNotes, IList<Note> goalNotes){
            Dictionary<Note, List<Note>> matches = new Dictionary<Note, List<Note>>();

            // look for notes where time difference is within a threshold (maxTimeDiffForMatch)
            foreach (Note goalNote in goalNotes) {
                List<Note> matchedNotes = playedNotes.Where(playedNote => {
                    return Math.Abs((playedNote.startTime - goalNote.startTime).TotalSeconds) < maxTimeDiffForMatch;
                }).ToList();

                matches[goalNote] = matchedNotes;
            }

            return matches;
        }


        private Note FindClosestNote(Note goalNote, IList<Note> playedNotes) {
            Note closestNote = playedNotes[0];
            double minDiff = Math.Abs((closestNote.startTime - goalNote.startTime).TotalSeconds);
            foreach (Note playedNote in playedNotes) {
                double diff = Math.Abs((closestNote.startTime - goalNote.startTime).TotalSeconds);
                if (diff < minDiff) {
                    minDiff = diff;
                    closestNote = playedNote;
                }
            }
            return closestNote;
        }

        private Feedback GetTimingFeedback(Note goalNote, IList<Note> playedNotes) {
            Note closestNote = FindClosestNote(goalNote, playedNotes);

            double diff = (closestNote.startTime - goalNote.startTime).TotalSeconds;
            double absDiff = Math.Abs(diff);

            string slowOrFastStr = diff > 0 ? "slow" : "fast"; // user is slow if difference is positive (played after goal note)

            if (absDiff < 0.03) {
                return new Feedback() { FeedbackString = "Good timing!" };
            } else if (absDiff < 0.05) {
                return new Feedback() { FeedbackString = String.Format("You were just a little bit {0}", slowOrFastStr) };
            } else {
                return new Feedback() { FeedbackString = String.Format("You were {0}", slowOrFastStr) };
            }
        }

        public List<Feedback> GetFeedback(IList<Note> allPlayedNotes, IList<Note> goalNotes) {
            string feedbackString = string.Empty;

            List<Feedback> feedback = new List<Feedback>();

            Dictionary<Note, List<Note>> matches = GetMatches(allPlayedNotes, goalNotes);

            // loop through each match
            foreach (KeyValuePair<Note, List<Note>> match in matches.Where(match => match.Value.Count > 0)) {
                Note goalNote = match.Key;
                List<Note> playedNotes = match.Value;

                feedback.Add(GetTimingFeedback(goalNote, playedNotes));
            }

            return feedback;
        }
    }
}
