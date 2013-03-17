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

            me.UpdatePositionAndLedgerLines();
        }

        #endregion

        private void UpdatePositionAndLedgerLines() {
            UpdateTopPosition();
            RedrawLedgerLines(Canvas.GetTop(this));
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
            else if (semitoneInOneOctave < 2)
                shift = 1;
            else if (semitoneInOneOctave < 4)
                shift = 2;
            else if (semitoneInOneOctave < 6)
                shift = 3;
            else if (semitoneInOneOctave < 7)
                shift = 4;
            else if (semitoneInOneOctave < 9)
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

            if (top >= StaffControl.BelowLedgerLineTop)
                diff = top - StaffControl.BelowLedgerLineTop;

            else if (top <= StaffControl.AboveLedgerLineTop)
                diff = StaffControl.AboveLedgerLineTop - top;

            linesToDraw = (int)(diff / StaffControl.DistanceBetweenStaffLines) + 1;

            double x1 = -5d;
            double x2 = this.ActualWidth + 5d;

            for (int i = 0; i < linesToDraw; i++) {
                Line ledgerLine = new Line() {
                    X1 = x1,
                    Y1 = top,
                    X2 = x2,
                    Y2 = top
                };

                noteCanvas.Children.Add(ledgerLine);

                if (top >= StaffControl.BelowLedgerLineTop)
                    top += StaffControl.DistanceBetweenStaffLines;

                else if (top <= StaffControl.AboveLedgerLineTop)
                    top -= StaffControl.DistanceBetweenStaffLines;
            }
        }
    }

    public enum LedgerLineLocation
    {
        AboveStaff,
        BelowStaff
    }
}
