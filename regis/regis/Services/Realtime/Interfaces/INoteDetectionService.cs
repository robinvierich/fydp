using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regis.Services.Realtime
{
    public class SimpleNoteDetectionArgs
    {

    }

    interface INoteDetectionService: IRealtimeService<SimpleNoteDetectionArgs>
    {
    }
}
