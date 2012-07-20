using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Regis.Services.Realtime;
using Regis.Composition;
using System.ComponentModel.Composition;

namespace Regis
{
    public partial class App : Application
    {
        bool loaded = false;

        [Import]
        IFFTService _fftService = null;

        [Import]
        INoteDetectionService _noteDetectionService = null;

        [Import]
        IAsioSamplingService _asioSamplingService = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Importer.Compose(this);

            // TODO: Fill this in with correct args;
            _fftService.Start(new FFTArgs() { MaxQueueSize = 3 });
            _noteDetectionService.Start(new SimpleNoteDetectionArgs());
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            
            _asioSamplingService.Stop();
            _asioSamplingService.ReleaseDriver();

            _fftService.Stop();
            _noteDetectionService.Stop();
        }
    }
}
