using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regis.Plugins.Interfaces
{
    public interface IPersistenceService
    {
        void Save<T>(T obj, string filePath, bool overwrite);
        T Load<T>(string filePath);
    }
}
