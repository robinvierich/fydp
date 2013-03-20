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
using Regis.Plugins.Models;

namespace Regis.Plugins.Controls
{
    /// <summary>
    /// Interaction logic for NoteControl.xaml
    /// </summary>
    public partial class NoteControl : UserControl
    {
        public NoteControl() {
            InitializeComponent();
        }

        public const double NoteWidth = 30d;
        public const double NoteHeight = 20d;

        #region Note (Dependency Property)

        public Note Note {
            get { return (Note)GetValue(NoteProperty); }
            set { SetValue(NoteProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Note.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NoteProperty =
            DependencyProperty.Register("Note", typeof(Note), typeof(NoteControl),
            new UIPropertyMetadata(new Note(), new PropertyChangedCallback(Note_PropertyChanged)));


        private static void Note_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            NoteControl me = d as NoteControl;
            if (me == null) return;

            Note note = e.NewValue as Note;
            if (note == null) return;
            if (note.Semitone < 1) return;


            me.DataContext = note;
            me.UpdatePositionAndLedgerLines();

            

            if (note.IsFlat)
                me.FlatVisibility = Visibility.Visible;
            else
                me.FlatVisibility = Visibility.Hidden;
        }

        #endregion


        public Visibility FlatVisibility {
            get { return (Visibility)GetValue(FlatVisibilityProperty); }
            set { SetValue(FlatVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FlatVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FlatVisibilityProperty =
            DependencyProperty.Register("FlatVisibility", typeof(Visibility), typeof(NoteControl), new UIPropertyMetadata(Visibility.Hidden));



        //#region NoteBrush (Dependency Property)
        //public Brush NoteBrush {
        //    get { return (Brush)GetValue(NoteBrushProperty); }
        //    set { SetValue(NoteBrushProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for NoteBrush.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty NoteBrushProperty =
        //    DependencyProperty.Register("NoteBrush", typeof(Brush), typeof(NoteControl), 
        //        new UIPropertyMetadata(Brushes.Black, new PropertyChangedCallback(NoteBrush_PropertyChanged))
        //    );

        //private static void NoteBrush_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        //    NoteControl me = d as NoteControl;
        //    Brush newBrush = e.NewValue as Brush;

        //    me.noteEllipse.Fill = newBrush;
        //    me.noteEllipse.Stroke = newBrush;

        //    me.noteStem.Fill = newBrush;
        //    me.noteStem.Stroke = newBrush;
        //}
        //#endregion

        private void UpdatePositionAndLedgerLines() {
            Dispatcher.BeginInvoke(new Action ( () => {
                UpdateTopPosition();
                RedrawLedgerLines(Canvas.GetTop(this));
            }));
        }

        private void UpdateTopPosition() {
            

            int diff = Note.Semitone - Note.C5Semitone;

            int shift = 0;

            int octaves = diff / 12;
            

            int semitoneInOneOctave = Note.Semitone % 12;
            if (diff < 0 && semitoneInOneOctave > 0)
                octaves -= 1;

            // TODO: Fix this
            // There's definitely a better way, hardcoded for now
            if (semitoneInOneOctave < 1)
                shift = 0;
            else if (semitoneInOneOctave < 3)
                shift = 1;
            else if (semitoneInOneOctave < 5)
                shift = 2;
            else if (semitoneInOneOctave < 6)
                shift = 3;
            else if (semitoneInOneOctave < 8)
                shift = 4;
            else if (semitoneInOneOctave < 10)
                shift = 5;
            else if (semitoneInOneOctave <= 11)
                shift = 6;

            double space = (StaffControl.DistanceBetweenStaffLines / 2d);
            double top = -(shift * space) + -octaves * (7 * space) + StaffControl.C5Top;

            Canvas.SetTop(this, top);
        }

        private void RedrawLedgerLines(double top) {

            int linesToDraw = 0;
            double diff = 0;

            if (top >= StaffControl.BelowLedgerLineTop) {
                diff = top - StaffControl.BelowLedgerLineTop;
            } else if (top <= StaffControl.AboveLedgerLineTop) {
                diff = StaffControl.AboveLedgerLineTop - top;
            } else {
                return;
            }

            linesToDraw = (int)(diff / StaffControl.DistanceBetweenStaffLines) + 1;

            double x1 = -3d;
            double x2 = this.noteCanvas.Width + 3d;



            double y = diff + this.noteCanvas.Height/2d;
            if (top >= StaffControl.BelowLedgerLineTop)
                y = -y + this.noteCanvas.Height;

            for (int i = 0; i < linesToDraw; i++) {

                //Binding brushBinding = new Binding("NoteBrush") { FallbackValue = Brushes.Black };

                Line ledgerLine = new Line() {
                    X1 = x1,
                    Y1 = y,
                    X2 = x2,
                    Y2 = y,
                    StrokeThickness = 2,
                    Stroke = Brushes.Black
                };

                //ledgerLine.SetBinding(Line.StrokeProperty, brushBinding);

                noteCanvas.Children.Add(ledgerLine);

                if (top >= StaffControl.BelowLedgerLineTop)
                    y += StaffControl.DistanceBetweenStaffLines;

                else if (top <= StaffControl.AboveLedgerLineTop)
                    y -= StaffControl.DistanceBetweenStaffLines;

                
            }
        }
    }

    public enum LedgerLineLocation
    {
        AboveStaff,
        BelowStaff
    }
}
