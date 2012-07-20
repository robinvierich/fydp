using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Regis.Plugins.Models;

namespace Regis.Plugins.Interfaces
{
    public interface IPlugin
    {
        void Load();
        FrameworkElement GetVisualContent();
        string PluginName { get; }
        string FriendlyPluginName { get; }
    }
}
