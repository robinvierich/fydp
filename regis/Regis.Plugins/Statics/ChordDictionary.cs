using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Regis.Plugins.Statics;

namespace Regis.Plugins.Statics
{
    public static class ChordDictionary
    {
        static ChordDictionary()
        {
            ChordList = new List<Chord>();

            StreamReader readFile = new StreamReader(Environment.CurrentDirectory + "\\ChordDict.dict");

            while (true)
            {
                Chord chord = new Chord();

                string line = readFile.ReadLine();

                if (line == "#end")
                    break;

                string[] lineParts = line.Split(',');
                
                chord.Name = lineParts[0];
                int numNotes = Convert.ToInt32(lineParts[1]);

                List<double> frequencies = new List<double>();
                for (int i = 0; i < numNotes; i++)
                {
                    line = readFile.ReadLine();

                    if (line == "#chord")
                        break;

                    frequencies.Add(Convert.ToDouble(line));                    
                }

                chord.Frequencies = frequencies;

                line = readFile.ReadLine();
                chord.CharValue = Convert.ToChar(line);

                line = readFile.ReadLine();

                ChordList.Add(chord);
            }

            Console.WriteLine("DEBUG::REGIS:: ChordDict.dict  => Loaded");
        }

        public static List<Chord> ChordList
        {
            get;
            private set;
        }
    }
}