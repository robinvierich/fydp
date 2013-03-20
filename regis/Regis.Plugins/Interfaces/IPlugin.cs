using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Regis.Plugins.Models;

namespace Regis.Plugins.Interfaces
{
    public enum PluginLayout
    {
        Button,
        Menu,
        Panel
    }

    public interface IPlugin
    {
        void Load();
        FrameworkElement GetVisualContent();
        string PluginName { get; }
        string FriendlyPluginName { get; }
        string PluginIcon { get; }
        PluginLayout Layout { get; }
    }
}
