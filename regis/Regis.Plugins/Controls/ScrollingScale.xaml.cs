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
using System.Windows.Threading;

namespace Regis.Plugins.Controls
{
    public partial class ScrollingScale : UserControl
    {
        DispatcherTimer _timer;
        Queue<Note> _noteQueue;


        public ScrollingScale() {
            InitializeComponent();
            NotesPlayed.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(NotesPlayed_CollectionChanged);

            _timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(10) };
            _timer.Tick += new EventHandler(_timer_Tick);

            _noteQueue = new Queue<Note>();
        }

        void _timer_Tick(object sender, EventArgs e) {
            foreach (Note note in _noteQueue.OrderBy(n => n.startTime)) {
                AddNote(note.Text, Brushes.Green);
            }

            AddNote("="); // staff
        }

        private void AddNote(string noteStr, Brush foreground=null) {
            if (foreground == null)
                foreground = Brushes.Gray;

            Run newRun = new Run(noteStr) { Foreground = foreground };
            staffTxt.Inlines.Add(newRun);
        }


        public void Start() {
            _timer.Start();
        }

        public void Stop() {
            _timer.Stop();
        }

        #region BPM

        public double BPM {
            get { return (double)GetValue(BPMProperty); }
            set { SetValue(BPMProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BPM.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BPMProperty =
            DependencyProperty.Register("BPM", typeof(double), typeof(ScrollingScale), new UIPropertyMetadata(0d));

        #endregion

        #region CurrentTime

        public DateTime CurrentTime {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(ScrollingScale), new UIPropertyMetadata(default(DateTime)));

        #endregion

        #region SecondsPerWidth

        static double _initialSecondsPerWidth = 5; // [sec] / this.Width [px]

        /// <summary>
        /// # of seconds that pass to get from beginning of staff to end (one 'page' or 'line' of the staff).
        /// </summary>
        public double SecondsPerWidth {
            get { return (double)GetValue(SecondsPerWidthProperty); }
            set { SetValue(SecondsPerWidthProperty, value); }
        }

        public static readonly DependencyProperty SecondsPerWidthProperty =
            DependencyProperty.Register("SecondsPerWidth", typeof(double), typeof(ScrollingScale), new UIPropertyMetadata(_initialSecondsPerWidth));

        #endregion

        #region NotesToPlay

        public ObservableCollection<Note> NotesToPlay {
            get { return (ObservableCollection<Note>)GetValue(NotesToPlayProperty); }
            set { SetValue(NotesToPlayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NotesToPlay.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NotesToPlayProperty =
            DependencyProperty.Register("NotesToPlay", typeof(ObservableCollection<Note>), typeof(ScrollingScale), 
                                        new UIPropertyMetadata(new ObservableCollection<Note>(), new PropertyChangedCallback(NotesToPlay_PropertyChanged)));
        
        private static void NotesToPlay_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ScrollingScale me = d as ScrollingScale;
            if (me == null) return;

            ObservableCollection<Note> oldNotes = e.OldValue as ObservableCollection<Note>;
            if (oldNotes != null)
                oldNotes.CollectionChanged -= me.NotesToPlay_CollectionChanged;

            ObservableCollection<Note> newNotes = e.NewValue as ObservableCollection<Note>;
            if (newNotes != null)
                newNotes.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(me.NotesToPlay_CollectionChanged);
        }

        void NotesToPlay_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            if (e.NewItems == null) return;

            foreach (Note note in e.NewItems.OfType<Note>().OrderBy(n => n.startTime)) {
                // TODO: Figure out how to draw notes based on time
            }
        }
        #endregion

        #region NotesPlayed
        public ObservableCollection<Note> NotesPlayed {
            get { return (ObservableCollection<Note>)GetValue(NotesPlayedProperty); }
            set { SetValue(NotesPlayedProperty, value); }
        }

        public static readonly DependencyProperty NotesPlayedProperty =
            DependencyProperty.Register("NotesPlayed", typeof(ObservableCollection<Note>), typeof(ScrollingScale), 
                                        new UIPropertyMetadata(new ObservableCollection<Note>(), new PropertyChangedCallback(NotesPlayed_PropertyChanged)));

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

        void NotesPlayed_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            if (e.NewItems == null) return;

            foreach (Note note in e.NewItems) {
                // TODO: Figure out how to draw notes based on time
                // We will eventually need to quantize the notes to 32nd, 16th, etc. based on time signature.
            }
        }
        #endregion

    }
}
