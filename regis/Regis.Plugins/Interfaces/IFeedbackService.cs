﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Plugins.Models;

namespace Regis.Plugins.Interfaces
{
    public interface IFeedbackService
    {
        Feedback GetFeedback(IList<Note> playedNotes, IList<Note> goalNotes);
    }
}
