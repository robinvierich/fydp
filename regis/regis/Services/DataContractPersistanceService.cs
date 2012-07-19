using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;
using System.ComponentModel.Composition;

namespace Regis.Services
{
    [Export (typeof(IPersistanceService))] 
    public class DataContractPersistanceService: IPersistanceService
    {
        public void Save<T>(T obj, string filePath, bool overwrite)
        {
            if (overwrite && File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            XDocument file = Serialize<T>(obj);
            file.Save(filePath);
        }

        public T Load<T>(string filePath)
        {
            if (!File.Exists(filePath))
                throw new Exception("File doesn't exist!");

            XDocument file = XDocument.Load(filePath);
            return Deserialize<T>(file);

        }


        private XDocument Serialize<T>(T source)
        {
            XDocument target = new XDocument();
            DataContractSerializer s = new DataContractSerializer(typeof(T));
            using (XmlWriter xw = target.CreateWriter())
            {
                s.WriteObject(xw, source);
            }

            return target;
        }


        private T Deserialize<T>(XDocument source)
        {
            DataContractSerializer s = new DataContractSerializer(typeof(T));
            T toReturn;

            using (XmlReader xr = source.CreateReader())
            {
                toReturn = (T)s.ReadObject(xr);
            }

            return toReturn;
        }

    }
}
