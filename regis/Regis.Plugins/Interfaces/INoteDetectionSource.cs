using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Regis.Plugins.Models;

namespace Regis.Plugins.Interfaces
{
    public interface INoteDetectionSource
    {
        event EventHandler<NotesDetectedEventArgs> NotesDetected;
    }

    public class NotesDetectedEventArgs: EventArgs
    {
        public NotesDetectedEventArgs(Note[] notes) {
            Notes = notes;
        }

        public Note[] Notes { get; private set; }
    }
}
