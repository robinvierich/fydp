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
using System.Collections.ObjectModel;
using Regis.Plugins.Models;

namespace Regis.Plugins.Controls
{
    /// <summary>
    /// Interaction logic for ScrollingScale.xaml
    /// </summary>
    public partial class ScrollingScale : UserControl
    {
        public ObservableCollection<Note> NotesPlayed {
            get { return (ObservableCollection<Note>)GetValue(NotesPlayedProperty); }
            set { SetValue(NotesPlayedProperty, value); }
        }

        public static readonly DependencyProperty NotesPlayedProperty =
            DependencyProperty.Register("NotesPlayed", 
                typeof(ObservableCollection<Note>), 
                typeof(ScrollingScale), 
                new UIPropertyMetadata(new ObservableCollection<Note>(), 
                    new PropertyChangedCallback(NotesPlayed_PropertyChanged)));

        private static void NotesPlayed_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ScrollingScale me = d as ScrollingScale;
            if (me == null) return;

            ObservableCollection<Note> oldNotes = e.OldValue as ObservableCollection<Note>;
            if (oldNotes != null)
                oldNotes.CollectionChanged -= me.NotesPlayed_CollectionChanged;

            ObservableCollection<Note> newNotes = e.NewValue as ObservableCollection<Note>;
            if (newNotes != null)
                newNotes.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(me.NotesPlayed_CollectionChanged);
        }

        public DateTime CurrentTime {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(ScrollingScale), new UIPropertyMetadata(default(DateTime)));


        public ScrollingScale() {
            InitializeComponent();
            NotesPlayed.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(NotesPlayed_CollectionChanged);
        }

        void NotesPlayed_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            if (e.NewItems == null) return;

            foreach (Note note in e.NewItems) {
                // TODO: Figure out how to draw notes based on time
                // We will eventually need to quantize the notes to 32nd, 16th, etc. based on time signature.
            }
        }
    }
}
