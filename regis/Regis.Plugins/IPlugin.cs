using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Regis.Plugins.Models;

namespace Regis.Plugins
{
    public interface IPlugin
    {
        void Load(NoteStream noteStream);
        FrameworkElement GetVisualContent();
        string PluginName { get; }
        string FriendlyPluginName { get; }
        NoteDetectionAlgorithm Algorithm { get; }
    }
}
