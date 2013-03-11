using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Plugins.Interfaces;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;

namespace Regis.Services.Impl
{
    [Export(typeof(IPersistenceService))]
    public class DataContractPersistenceService : IPersistenceService
    {
        #region Save/Load (IPersistanceService)
        public void Save<T>(T obj, string filePath, bool overwrite) {

            if (overwrite && File.Exists(filePath)) {
                File.Delete(filePath);
            }

            XDocument file = Serialize<T>(obj);
            file.Save(filePath);
        }


        public T Load<T>(string filePath) {

            if (!File.Exists(filePath))
                throw new Exception(String.Format("{0} doesn't exist!", filePath));

            XDocument file = XDocument.Load(filePath);
            T obj = Deserialize<T>(file);

            return obj;
        }
        #endregion

        #region Serialize/Deserialize (using DataContractSerializer)
        public XDocument Serialize<T>(T source) {
            XDocument target = new XDocument();
            DataContractSerializer s = new DataContractSerializer(typeof(T));
            using (XmlWriter xw = target.CreateWriter()) {
                s.WriteObject(xw, source);
            }

            return target;
        }

        public T Deserialize<T>(XDocument source) {
            DataContractSerializer s = new DataContractSerializer(typeof(T));
            T toReturn;

            using (XmlReader xr = source.CreateReader()) {
                toReturn = (T)s.ReadObject(xr);
            }

            return toReturn;
        }
        #endregion
    }
}
