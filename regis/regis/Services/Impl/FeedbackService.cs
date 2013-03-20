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

        double maxTimeDiffForMatch = 0.2; // s

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

        private double _goodTiming = 0.05;
        private double _mediumTiming = 0.1;

        private Feedback GetTimingFeedback(Note goalNote, Note closestNote) {
            double diff = (closestNote.startTime - goalNote.startTime).TotalSeconds;
            double absDiff = Math.Abs(diff);

            string slowOrFastStr = diff > 0 ? "slow" : "fast"; // user is slow if difference is positive (played after goal note)

            if (absDiff < _goodTiming) {
                return new GoodTimingFeedback(diff) { Note = closestNote };
            } else if (absDiff < _mediumTiming) {
                return new MediumTimingFeedback(diff) { Note = closestNote };
            } else {
                return new BadTimingFeedback(diff) { Note = closestNote };
            }
        }

        private Feedback GetPitchFeedback(Note goalNote, Note closestNote) {
            if (goalNote.Semitone == closestNote.Semitone)
                return new GoodPitchFeedback();
               
            int diff = closestNote.Semitone - goalNote.Semitone;
            return new BadPitchFeedback(diff);
        }


        Dictionary<Note, List<Feedback>> IFeedbackService.GetFeedback(IList<Note> allPlayedNotes, IList<Note> goalNotes) {
            string feedbackString = string.Empty;

            Dictionary<Note, List<Note>> matches = GetMatches(allPlayedNotes, goalNotes);
            Dictionary<Note, List<Feedback>> allFeedback = new Dictionary<Note, List<Feedback>>();

            // loop through each match
            foreach (KeyValuePair<Note, List<Note>> match in matches.Where(match => match.Value.Count > 0)) {
                Note goalNote = match.Key;
                List<Note> playedNotes = match.Value;
                Note closestNote = FindClosestNote(goalNote, playedNotes);

                List<Feedback> feedback = new List<Feedback>();

                feedback.Add(GetTimingFeedback(goalNote, closestNote));
                feedback.Add(GetPitchFeedback(goalNote, closestNote));

                allFeedback.Add(closestNote, feedback);
            }

            return allFeedback;
        }
    }
}
