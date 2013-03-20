using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regis.Services
{
    public interface IWaveFileService
    {
        void SaveWavFile(string fileName, byte[] data);
        byte[] LoadWavFile(string fileName);
    }
}
