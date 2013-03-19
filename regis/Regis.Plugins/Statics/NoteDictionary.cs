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
            {16.35, "C0"},
            {17.32, "Db0"},
            {18.35, "D0"},
            {19.45, "Eb0"},
            {20.60, "E0"},
            {21.83, "F0"},
            {23.12, "Gb0"},
            {24.50, "G0"},
            {25.96, "Ab0"},
            {27.50, "A0"},
            {29.14, "Bb0"},
            {30.87, "B0"},
            {32.70, "C1"},
            {34.65, "Db1"},
            {36.71, "D1"},
            {38.89, "Eb1"},
            {41.20, "E1"},
            {43.65, "F1"},
            {46.25, "Gb1"},
            {49.00, "G1"},
            {51.91, "Ab1"},
            {55.00, "A1"},
            {58.27, "Bb1"},
            {61.74, "B1"},
            {65.41, "C2"},
            {69.30, "Db2"},
            {73.42, "D2"},
            {77.78, "Eb2"},
            {82.4069, "E2"},
            {87.3071, "F2"},
            {92.4986, "Gb2"},
            {97.9989, "G2"},
            {103.826, "Ab2"},
            {110.00, "A2"},
            {116.541, "Bb2"},
            {123.471, "B2"},
            {130.813, "C3"},
            {138.591, "Db3"},
            {146.832, "D3"},
            {155.563, "Eb3"},
            {164.814, "E3"},
            {174.614, "F3"},
            {184.997, "Gb3"},
            {195.998, "G3"},
            {207.652, "Ab3"},
            {220.00, "A3"},
            {233.082, "Bb3"},
            {246.942, "B3"},
            {261.626, "C4"},
            {277.183, "Db4"},
            {293.665, "D4"},
            {311.127, "Eb4"},
            {329.628, "E4"},
            {349.228, "F4"},
            {369.994, "Gb4"},
            {391.995, "G4"},
            {415.305, "Ab4"},
            {440.00, "A4"},
            {466.164, "Bb4"},
            {493.883, "B4"},
            {523.251, "C5"},
            {554.365, "Db5"},
            {587.33, "D5"},
            {622.254, "Eb5"},
            {659.255, "E5"},
            {698.456, "F5"},
            {739.989, "Gb5"},
            {783.991, "G5"},
            {830.609, "Ab5"},
            {880.00, "A5"},
            {932.328, "Bb5"},
            {987.767, "B5"},
            {1046.5, "C6"},
            {1108.73, "Db6"},
            {1174.66, "D6"},
            {1244.51, "Eb6"},
            {1318.51, "E6"},
            {1396.91, "F6"},
            {1479.98, "Gb6"},
            {1567.98, "G6"},
            {1661.22, "Ab6"},
            {1760.00, "A6"},
            {1864.66, "Bb6"},
            {1975.53, "B6"},
            {2093.00, "C7"},
            {2217.46, "Db7"},
            {2349.32, "D7"},
            {2489.02, "Eb7"},
            {2637.02, "E7"},
            {2793.83, "F7"},
            {2959.96, "Gb7"},
            {3135.96, "G7"},
            {3322.44, "Ab7"},
            {3520.00, "A7"},
            {3729.31, "Bb7"},
            {3951.07, "B7"},
            {4186.01, "C8"},
            {4434.92, "Db8"},
            {4698.64, "D8"},
            {4978.03, "Eb8"},
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
