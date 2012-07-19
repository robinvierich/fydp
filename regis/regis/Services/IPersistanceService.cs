using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regis.Services
{
    public interface IPersistanceService
    {
        void Save<T>(T obj, string filePath, bool overwrite);
        T Load<T>(string filePath);
    }
}
