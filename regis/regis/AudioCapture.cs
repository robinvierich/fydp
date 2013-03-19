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
        public static int SampleRate = 44100; //44100
        public static int BufferSize = 128; //4096
        public static double NoiseFloor = 6E+15; //6E+15; // power - based on FFT
        public static int BufferModifier = 4;
        public static int FFTPerBuffer = 2;
        public static int BufferSkip = 16; //1
        public static int SubPeaks = 2;
    }
}
