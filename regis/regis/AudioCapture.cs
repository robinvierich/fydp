using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlueWave.Interop.Asio;

namespace Regis.AudioCapture
{
    public static class AudioCaptureSettings
    {
        public static AsioDriver LoadedDriver = null;
        public static int SampleRate = 44100;
        public static int BufferSize = 2048;
        public static double NoiseFloor = 6E+16; // power - based on FFT
    }
}
