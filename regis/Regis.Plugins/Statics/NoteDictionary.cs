using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Regis.Plugins.Statics
{
    public static class NoteDictionary
    {
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

            Console.WriteLine("DEBUG::REGIS:: TrebleDict.dict  => Loaded");
        }

        public static Dictionary<double, char> NoteDict
        {
            get;
            private set;
        }
        

    }
}
