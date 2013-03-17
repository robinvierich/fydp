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
    /// Interaction logic for StaffControl.xaml
    /// </summary>
    public partial class StaffControl : UserControl
    {
        public const double C5Top = 90;
        public const double DistanceBetweenStaffLines = 20;

        public const double AboveLedgerLineTop = 40;
        public const double BelowLedgerLineTop = 160;

        public const double FullControlTime = 5000d; // ms - time from start of staff to end of staff.. 
                                                     // Notes are actually ordered relatively (not based on absolute time), but this will work for now..

        public const double FullControlWidth = 800; // px

        public StaffControl() {
            InitializeComponent();
        }

        #region Notes (Dependency Property)

        public ObservableCollection<Note> Notes {
            get { return (ObservableCollection<Note>)GetValue(NotesProperty); }
            set { SetValue(NotesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Notes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NotesProperty =
            DependencyProperty.Register("Notes", typeof(ObservableCollection<Note>), typeof(StaffControl),
                new FrameworkPropertyMetadata(
                    new ObservableCollection<Note>(), 
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    new PropertyChangedCallback(Notes_PropertyChanged)
                ));

        #endregion

        #region StartTime (Dependency Property)

        public DateTime StartTime {
            get { return (DateTime)GetValue(StartTimeProperty); }
            set { SetValue(StartTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartTimeProperty =
            DependencyProperty.Register("StartTime", typeof(DateTime), typeof(StaffControl), new UIPropertyMetadata(DateTime.Now));

        #endregion

        #region CurrentTime (Dependency Property)

        public DateTime CurrentTime {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(StaffControl), 
                new FrameworkPropertyMetadata(
                    DateTime.Now, 
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    new PropertyChangedCallback(CurrentTime_PropertyChanged)
                ));

        private static void CurrentTime_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            StaffControl me = d as StaffControl;
            double x = me.GetLeftFromTime((me.CurrentTime - me.StartTime).TotalMilliseconds);
            me.timeLine.X1 = x;
            me.timeLine.X2 = x;
        }

        #endregion

        #region Property Changed Handlers

        private static void Notes_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            StaffControl me = d as StaffControl;

            if (e.OldValue != null) {

                me.RemoveNotes(e.OldValue as IEnumerable<Note>);

                ObservableCollection<Note> notes = e.OldValue as ObservableCollection<Note>;
                if (notes != null)
                    notes.CollectionChanged -= me.notes_CollectionChanged;
            }

            if (e.NewValue != null) {

                me.AddNotes(e.NewValue as IEnumerable<Note>);

                ObservableCollection<Note> notes = e.NewValue as ObservableCollection<Note>;
                if (notes != null)
                    notes.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(me.notes_CollectionChanged);
            }
        }
        #endregion

        void notes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            if (e.OldItems != null)
                RemoveNotes(e.OldItems.Cast<Note>());

            if (e.NewItems != null)
                AddNotes(e.NewItems.Cast<Note>());
        }

        private void RemoveNotes(IEnumerable<Note> notes) {
            foreach (Note note in notes) {
                NoteControl noteControl = (NoteControl)rootCanvas.Children.Cast<UIElement>()
                                                          .Where(elem => elem is NoteControl 
                                                              && ((NoteControl)elem).Note == note)
                                                          .SingleOrDefault();

                if (noteControl == null) return;
                rootCanvas.Children.Remove(noteControl);
            }
        }


        private double GetLeftFromTime(double milliseconds) {
            return (milliseconds / FullControlTime) * FullControlWidth;
        }


        private void AddNotes(IEnumerable<Note> notes) {
            foreach (Note n in notes) {
                NoteControl noteControl = new NoteControl() { Note = n };
                double t = (n.startTime - this.StartTime).TotalMilliseconds;

                Canvas.SetLeft(noteControl, GetLeftFromTime(t));

                rootCanvas.Children.Add(noteControl);
            }
        }
    }
}
