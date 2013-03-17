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
using System.ComponentModel.Composition;
using RegisFreeFormPlugin.ViewModels;
using Regis.Plugins.Interfaces;
using System.Threading;
using Regis.Plugins.Models;
using Regis.Plugins.Statics;
using System.Windows.Threading;


namespace RegisFreeFormPlugin
{
    /// <summary>
    /// Interaction logic for FreeFormControl.xaml
    /// </summary>

    [Export(typeof(IPlugin))]
    public partial class FreeFormControl : UserControl, IPlugin, IPartImportsSatisfiedNotification
    {
        Thread _freeformThread;
        bool _runningFreeform = false;

        public FreeFormControl()
        {
            InitializeComponent();
        }

        [Import]
        private INoteDetectionSource noteSource;

        [Import]
        private FreeFormViewModel ViewModel
        {
            get;
            set;
        }

        public void OnImportsSatisfied()
        {
            DataContext = ViewModel;
        }

        #region IPlugin

        public void Load()
        {
            
        }

        public FrameworkElement GetVisualContent()
        {
            return this;
        }

        public string PluginName
        {
            get { return "FreeFormPlugin"; }
        }

        public string FriendlyPluginName
        {
            get { return "Free Form"; }
        }

        #endregion

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            staff.StartTime = DateTime.Now;
        }

        private void StartFreeform()
        {
            _runningFreeform = true;
            string[] noteStaff = new string[19];
            noteStaff[0] = "&=";
            for (int count = 1; count < 18; count++)
            {
                if (count % 2 == 1) 
                    noteStaff[count] = "==";
                else
                    noteStaff[count] = "=";
            }
            noteStaff[18] = "||";
            RunFreeform(noteStaff);
        }

        private void StopFreeform()
        {
            if (_freeformThread == null)
                return;

            _runningFreeform = false;
            _freeformThread.Join();
        }

        private void RunFreeform(string[] noteStaff)
        {
            int index;
            Note[] prevNote = new Note[3];
            Note[] curNotes;
            index = 1; 
            while (_runningFreeform)
            {
                
                
                // "&amp;= == = == = == = == = == = == = == = == = == ||" 
                curNotes = noteSource.GetNotes();
                if (curNotes == null)
                    continue;

                if (curNotes[0].ClosestRealNoteFrequency == prevNote[0].ClosestRealNoteFrequency)
                    continue;
                else if (curNotes[0].ClosestRealNoteFrequency == 0)
                    continue;

                prevNote = curNotes;
       
                noteStaff[index] = NoteDictionary.NoteDict[curNotes[0].ClosestRealNoteFrequency].ToString();
                String mystaff = string.Join("", noteStaff);
                Application.Current.Dispatcher.Invoke(
                    DispatcherPriority.Render,
                    new Action<string>(updateStaff),
                    mystaff);

                //noteBlock.Text = noteStaff.ToString();

                index += 2;

                if (index >= 18)
                    index = 1; 
            }
        }
        private void updateStaff(string staff)
        {
            //noteBlock.Text = staff;
        }
    }
}
