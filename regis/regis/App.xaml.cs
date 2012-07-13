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

        // Run after the main window is loaded - (there are other cases too - dealt with by (loaded == true) check)
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if (loaded) 
                return;

            loaded = true;

            Importer.Compose(this);
            Importer.Compose(Application.Current.MainWindow);

            // TODO: Fill this in with correct args;
            _fftService.Start(new FFTArgs());
            _noteDetectionService.Start(new SimpleNoteDetectionArgs());
        }


        [Import]
        IAsioSamplingService _asioSamplingService = null;

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            _asioSamplingService.Stop();
            _fftService.Stop();
            _noteDetectionService.Stop();
        }
    }
}
