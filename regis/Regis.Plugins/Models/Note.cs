using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Plugins.Statics;
using Regis.Base;
using System.ComponentModel;
using Regis.Base.ViewModels;
using System.Windows.Media;

namespace Regis.Plugins.Models
{
    public class Note: BaseViewModel
    {
        public DateTime startTime;
        public DateTime endTime;
        public double frequency;

        public const int C5Semitone = 60;

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

        public TimeSpan Duration {
            get {
                return endTime - startTime;
            }
        }

        #region NoteBrush
        private Brush _NoteBrush = Brushes.Black;
        private static PropertyChangedEventArgs _NoteBrush_ChangedEventArgs = new PropertyChangedEventArgs("NoteBrush");

        public Brush NoteBrush {
            get { return _NoteBrush; }
            set {
                _NoteBrush = value;
                NotifyPropertyChanged(_NoteBrush_ChangedEventArgs);
            }
        }
        #endregion

        /// <summary>
        /// Semitone number. 
        /// 0 = C0, 16.35Hz
        /// </summary>
        public int Semitone {
            get {
                return SemitoneFromFrequency(frequency);
            }
            set {
                frequency = FrequencyFromSemitone(value);
            }
        }

        public bool IsFlat {
            get {
                int semitoneInScale = Semitone % 12;
                return semitoneInScale == 1 || semitoneInScale == 3 ||
                    semitoneInScale == 6 || semitoneInScale == 8 ||
                    semitoneInScale == 10;
            }
        }
        

        public double GetQuantizedNoteLength(double bpm, TimeSignature timeSig) {
            double bps = bpm / 60d;
            double beats = Duration.Seconds * bps;
            return beats / timeSig.BottomNumber;
        }

        /// <summary>
        /// Returns the semitone # in the chromatic scale (semitone #0 = C0, semitone #1 = C#0, etc.).
        /// 
        /// Assumes equal temperment, A4 = 440Hz, and
        /// f = 440 * 2^[(n-48)/12]
        /// n = 12 * log2(f/440) + 48
        /// </summary>
        public static int SemitoneFromFrequency(double freq) {
            return (int)Math.Round(12 * Math.Log(freq / 440d, 2) + 48);
        }

        public static double FrequencyFromSemitone(int semitone) {
            return 440 * Math.Pow(2, (semitone - 48) / 12d);
        }
    }
}
