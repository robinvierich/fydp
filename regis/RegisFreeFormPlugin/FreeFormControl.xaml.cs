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
            get { return "FreeFormMode"; }
        }

        #endregion

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            StopFreeform();
            _freeformThread = new Thread(new ThreadStart(StartFreeform));
            _freeformThread.Start();
        }

        private void StartFreeform()
        {
            _runningFreeform = true;
            string[] noteStaff = new string[19];
            noteStaff[0] = "&amp;=";
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
            index = 2; 
            while (_runningFreeform)
            {
                Note[] prevNote = null;
                Note[] curNote;
                // "&amp;= == = == = == = == = == = == = == = == = == ||" 
                

                if (!noteSource.NoteQueue.TryPeek(out curNote))
                    continue;

                if (curNote == prevNote)
                    continue;

                prevNote = curNote;

                noteStaff[index] = NoteDictionary.NoteDict[curNote[0].closestRealNoteFrequency].ToString();

                noteBlock.Text = noteStaff.ToString();

                index += 2;

                if (index >= 18)
                    index = 2; 
            }
        }
    }
}
