using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Regis.Plugins.Interfaces;

namespace Regis.Controls
{
    [TemplatePart(Name="PART_ContentContainer", Type = typeof(ContentPresenter))]
    public class PluginContainer : Control
    {
        ContentPresenter _contentPresenter;

        static PluginContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PluginContainer), new FrameworkPropertyMetadata(typeof(PluginContainer)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _contentPresenter = GetTemplateChild("PART_ContentContainer") as ContentPresenter;
            if (_contentPresenter == null)
            {
                throw new Exception("PluginContainer's template must include a ContentPresenter called \"PART_ContentContainer\"");
            }
        }

        public IPlugin Plugin
        {
            get { return (IPlugin)GetValue(PluginProperty); }
            set { SetValue(PluginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PluginProperty =
            DependencyProperty.Register("Plugin", typeof(IPlugin), typeof(PluginContainer), new UIPropertyMetadata(null, new PropertyChangedCallback(ContentPropertyChanged)));

        
        static void ContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PluginContainer me = d as PluginContainer;
            if (me == null) return;

            IPlugin newPlugin = e.NewValue as IPlugin;
            if (e.NewValue == null)
            {
                me._contentPresenter.Content = null;
                return;
            }

            me._contentPresenter.Content = newPlugin.GetVisualContent();
        }


    }
}
