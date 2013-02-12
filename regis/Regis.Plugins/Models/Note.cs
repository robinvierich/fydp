using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Plugins.Statics;
using Regis.Base;
using System.ComponentModel;

namespace Regis.Plugins.Models
{
    public class Note
    {
        public DateTime startTime;
        public DateTime endTime;
        public double frequency;

        public double ClosestRealNoteFrequency {
            get {
                return NoteDictionary.GetClosestNoteFrequency(frequency);
            }
        }

        public string Name {
            get {
                return NoteDictionary.GetNoteName(ClosestRealNoteFrequency);
            }
        }

        private PropertyChangedEventArgs _Duration_PropertyChangedEventArgs_ = new PropertyChangedEventArgs("Duration");
        public TimeSpan Duration {
            get {
                return endTime - startTime;
            }
        }

       public uint Semitone {
            get {
                return SemitoneFromFrequency(frequency);
            }
            set {
                frequency = FrequencyFromSemitone(value);
            }
        }

        /// <summary>
        /// Returns the semitone # in the chromatic scale (semitone #0 = C0, semitone #1 = C#0, etc.).
        /// 
        /// Assumes equal temperment, A4 = 440Hz, and
        /// f = 440 * 2^[(n-48)/12]
        /// n = 12 * log2(f/440) + 48
        /// </summary>
        public static uint SemitoneFromFrequency(double freq) {
                return (uint)Math.Round(12 * Math.Log(freq / 440d, 2) + 48);
        }

        public static double FrequencyFromSemitone(uint semitone) {
            return 440 * Math.Pow(2, (semitone - 48) / 12d);
        }
    }

    public static class NoteExtensions
    {

    }
}
