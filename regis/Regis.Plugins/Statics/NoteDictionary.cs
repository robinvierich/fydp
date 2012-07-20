using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Regis.Plugins.Statics
{
    public static class NoteDictionary
    {
        private static List<double> _sortedKeys = new List<double>();
        static NoteDictionary()
        {
            NoteDict = new Dictionary<double,char>();

            StreamReader readFile = new StreamReader(Environment.CurrentDirectory + "\\TrebleDict.dict");

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

        public static char GetClosestNote(double freq)
        {
            return NoteDict[NoteDict.Keys.Min(keyFreq => Math.Abs(keyFreq - freq))];
        }

        public static double GetClosestRealNoteFrequency(double freq)
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
