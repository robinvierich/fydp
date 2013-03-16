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
        public static int BufferSize = 4096;
        public static double NoiseFloor = 6E+11; //6E+15; // power - based on FFT
        public static int BufferModifier = 1;
    }
}
