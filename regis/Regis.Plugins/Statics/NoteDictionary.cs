using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Regis.Plugins.Statics
{
    public static class NoteDictionary
    {
        private static IDictionary<double, string> _noteNameDict = new Dictionary<double, string>() {
            {0.0, "Null"},
            //{16.35, "C0"},
            //{17.32, "C#0/Db0"},
            //{18.35, "D0"},
            //{19.45, "D#0/Eb0"},
            //{20.60, "E0"},
            //{21.83, "F0"},
            //{23.12, "F#0/Gb0"},
            //{24.50, "G0"},
            //{25.96, "G#0/Ab0"},
            //{27.50, "A0"},
            //{29.14, "A#0/Bb0"},
            //{30.87, "B0"},
            //{32.70, "C1"},
            //{34.65, "C#1/Db1"},
            //{36.71, "D1"},
            //{38.89, "D#1/Eb1"},
            //{41.20, "E1"},
            //{43.65, "F1"},
            //{46.25, "F#1/Gb1"},
            //{49.00, "G1"},
            //{51.91, "G#1/Ab1"},
            //{55.00, "A1"},
            //{58.27, "A#1/Bb1"},
            //{61.74, "B1"},
            //{65.41, "C2"},
            //{69.30, "C#2/Db2"},
            //{73.42, "D2"},
            //{77.78, "D#2/Eb2"},
            {82.41, "E2"},
            {87.31, "F2"},
            {92.50, "F#2/Gb2"},
            {98.00, "G2"},
            {103.83, "G#2/Ab2"},
            {110.00, "A2"},
            {116.54, "A#2/Bb2"},
            {123.47, "B2"},
            {130.81, "C3"},
            {138.59, "C#3/Db3"},
            {146.83, "D3"},
            {155.56, "D#3/Eb3"},
            {164.81, "E3"},
            {174.61, "F3"},
            {185.00, "F#3/Gb3"},
            {196.00, "G3"},
            {207.65, "G#3/Ab3"},
            {220.00, "A3"},
            {233.08, "A#3/Bb3"},
            {246.94, "B3"},
            {261.63, "C4"},
            {277.18, "C#4/Db4"},
            {293.66, "D4"},
            {311.13, "D#4/Eb4"},
            {329.63, "E4"},
            {349.23, "F4"},
            {369.99, "F#4/Gb4"},
            {392.00, "G4"},
            {415.30, "G#4/Ab4"},
            {440.00, "A4"},
            {466.16, "A#4/Bb4"},
            {493.88, "B4"},
            {523.25, "C5"},
            {554.37, "C#5/Db5"},
            {587.33, "D5"},
            {622.25, "D#5/Eb5"},
            {659.26, "E5"},
            {698.46, "F5"},
            {739.99, "F#5/Gb5"},
            {783.99, "G5"},
            {830.61, "G#5/Ab5"},
            {880.00, "A5"},
            {932.33, "A#5/Bb5"},
            {987.77, "B5"},
            {1046.50, "C6"},
            {1108.73, "C#6/Db6"},
            {1174.66, "D6"},
            {1244.51, "D#6/Eb6"},
            {1318.51, "E6"},
            {1396.91, "F6"},
            {1479.98, "F#6/Gb6"},
            {1567.98, "G6"},
            {1661.22, "G#6/Ab6"},
            {1760.00, "A6"},
            {1864.66, "A#6/Bb6"},
            {1975.53, "B6"},
            {2093.00, "C7"},
            {2217.46, "C#7/Db7"},
            {2349.32, "D7"},
            {2489.02, "D#7/Eb7"},
            {2637.02, "E7"},
            {2793.83, "F7"},
            {2959.96, "F#7/Gb7"},
            {3135.96, "G7"},
            {3322.44, "G#7/Ab7"},
            {3520.00, "A7"},
            {3729.31, "A#7/Bb7"},
            {3951.07, "B7"},
            {4186.01, "C8"},
            {4434.92, "C#8/Db8"},
            {4698.64, "D8"},
            {4978.03, "D#8/Eb8"},
        };

        private static List<double> _sortedKeys = new List<double>();
        static NoteDictionary()
        {
            NoteDict = new Dictionary<double,char>();

            StreamReader readFile = new StreamReader(Environment.CurrentDirectory + "\\Config\\TrebleDict.dict");

            while (true)
            {
                string line = readFile.ReadLine();

                if (line == "#end")
                    break;

                string[] lineParts = line.Split(',');

                NoteDict.Add(Convert.ToDouble(lineParts[0]), Convert.ToChar(lineParts[1])); 
            }

            _sortedKeys = NoteDict.Keys.ToList();
            _sortedKeys.Sort();

            Console.WriteLine("DEBUG::REGIS:: TrebleDict.dict  => Loaded");
        }

        public static Dictionary<double, char> NoteDict
        {
            get;
            private set;
        }

        public static string GetNoteName(double freq) {
            return _noteNameDict[freq];
        }

        public static char GetClosestNoteChar(double freq)
        {
            return NoteDict[NoteDict.Keys.Min(keyFreq => Math.Abs(keyFreq - freq))];
        }

        public static double GetClosestNoteFrequency(double freq)
        {
            if (_sortedKeys.Count != NoteDict.Keys.Count)
            {
                _sortedKeys = NoteDict.Keys.ToList();
                _sortedKeys.Sort();
            }

            for (int i = 1; i < _sortedKeys.Count; i++)
            {
                if (_sortedKeys[i] > freq)
                {
                    return Math.Abs(freq - _sortedKeys[i]) < Math.Abs(freq - _sortedKeys[i-1]) ? _sortedKeys[i] : _sortedKeys[i-1];
                }
            }

            return 0;
        }
    }
}
