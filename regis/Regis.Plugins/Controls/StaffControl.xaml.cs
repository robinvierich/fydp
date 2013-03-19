﻿using System;
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
using System.Threading;

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

        private static Color goalNoteColor = Color.FromArgb(100, 0, 0, 0);

        private int _resizedCount = 1;

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

        #region GoalNotes (Dependency Property)



        public ObservableCollection<Note> GoalNotes {
            get { return (ObservableCollection<Note>)GetValue(GoalNotesProperty); }
            set { SetValue(GoalNotesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GoalNotes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GoalNotesProperty =
            DependencyProperty.Register("GoalNotes", typeof(ObservableCollection<Note>), typeof(StaffControl), 
                new FrameworkPropertyMetadata(
                    new ObservableCollection<Note>(),
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    new PropertyChangedCallback(GoalNotes_PropertyChanged)
                ));

        
        #endregion

        #region StartTime (Dependency Property)

        public DateTime StartTime {
            get {
                return (DateTime)this.GetValue(StaffControl.StartTimeProperty);
            }
            set {
                this.SetValue(StaffControl.StartTimeProperty, value);
            }
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

            if ((me.CurrentTime - me.StartTime).TotalMilliseconds > FullControlTime * me._resizedCount) {
                me._resizedCount++;
                me.rootCanvas.Width += FullControlWidth;
                me.Dispatcher.Invoke(new Action(() => {
                    me.scrollViewer.ScrollToRightEnd();
                }));
            }

            double x = me.GetLeftFromTime((me.CurrentTime - me.StartTime).TotalMilliseconds) + NoteControl.NoteWidth/2d;
            me.timeLine.X1 = x;
            me.timeLine.X2 = x;
        }

        #endregion

        #region Property Changed Handlers
        private static void Notes_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ObservableCollection<Note> oldNotes = e.OldValue as ObservableCollection<Note>;
            ObservableCollection<Note> newNotes = e.NewValue as ObservableCollection<Note>;
            StaffControl me = d as StaffControl;

            if (oldNotes != null) {
                oldNotes.CollectionChanged -= me.notes_CollectionChanged;
                me.RemoveNotes(oldNotes);
            }

            if (newNotes != null) {
                me.AddNotes(newNotes, Colors.Black);
                newNotes.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(me.notes_CollectionChanged);
            }
        }



        private static void GoalNotes_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ObservableCollection<Note> oldNotes = e.OldValue as ObservableCollection<Note>;
            ObservableCollection<Note> newNotes = e.NewValue as ObservableCollection<Note>;
            StaffControl me = d as StaffControl;

            if (oldNotes != null) {
                oldNotes.CollectionChanged -= me.goalNotes_CollectionChanged;
                me.RemoveNotes(oldNotes);
            }

            if (newNotes != null) {
                me.AddNotes(newNotes, goalNoteColor);
                newNotes.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(me.goalNotes_CollectionChanged);
            }
        }
        #endregion

        void goalNotes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            this.Dispatcher.Invoke(new Action(() => {
                if (e.OldItems != null)
                    RemoveNotes(e.OldItems.Cast<Note>());

                if (e.NewItems != null)
                    AddNotes(e.NewItems.Cast<Note>(), goalNoteColor);
            }));
        }

        void notes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            this.Dispatcher.Invoke(new Action(() => {
                if (e.OldItems != null)
                    RemoveNotes(e.OldItems.Cast<Note>());

                if (e.NewItems != null)
                    AddNotes(e.NewItems.Cast<Note>(), Colors.Black);
            }));
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


        private void AddNotes(IEnumerable<Note> notes, Color color) {
            foreach (Note n in notes) {
                NoteControl noteControl = new NoteControl() { Note = n, NoteBrush = new SolidColorBrush(color) };
                double t = (n.startTime - this.StartTime).TotalMilliseconds;

                Canvas.SetLeft(noteControl, GetLeftFromTime(t));

                rootCanvas.Children.Add(noteControl);
            }
        }
    }
}
