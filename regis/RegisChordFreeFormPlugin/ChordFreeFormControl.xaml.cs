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
using RegisChordFreeFormPlugin.ViewModels;
using Regis.Plugins.Interfaces;
using System.Threading;
using Regis.Plugins.Models;
using Regis.Plugins.Statics;
using System.Windows.Threading;


namespace RegisChordFreeFormPlugin
{
    /// <summary>
    /// Interaction logic for FreeFormControl.xaml
    /// </summary>

    [Export(typeof(IPlugin))]
    public partial class ChordFreeFormControl : UserControl, IPlugin, IPartImportsSatisfiedNotification
    {
        Thread _chordFreeformThread;
        bool _runningChordFreeform = false;
        List<Chord> _chordList;
        
        public ChordFreeFormControl()
        {
            InitializeComponent();
            _chordList = ChordDictionary.ChordList;
            Unloaded += new RoutedEventHandler(ChordFreeFormControl_Unloaded);
        }

        void ChordFreeFormControl_Unloaded(object sender, RoutedEventArgs e)
        {
            StopFreeform();
        }

        [Import]
        private IChordDetectionSource chordSource;

        [Import]
        private ChordFreeFormViewModel ViewModel
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
            get { return "ChordFreeFormPlugin"; }
        }

        public string FriendlyPluginName
        {
            get { return "ChordFreeFormMode"; }
        }

        public string PluginIcon
        {
            get
            {
                return "";
            }
        }

        #endregion

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            StopFreeform();
            _chordFreeformThread = new Thread(new ThreadStart(StartFreeform));
            _chordFreeformThread.Start();
        }

        private void StartFreeform()
        {
            _runningChordFreeform = true;

            string _chord = ""; //Replace This
            
            RunFreeform(_chord);
        }

        private void StopFreeform()
        {
            if (_chordFreeformThread == null)
                return;

            _runningChordFreeform = false;
            _chordFreeformThread.Join();
        }

        private void RunFreeform(string _chord)
        {

            // Fix this too
            int index;
            Note[] prevNote = new Note[3];
            Note[] curNote;
            int noteCount = 0;

            while (_runningChordFreeform)
            {             
                if (!chordSource.ChordQueue.TryPeek(out curNote))
                    continue;
                noteCount = 0;
                for (int j = 0; j < ChordDictionary.ChordList.Count; j++)
                {
                    
                    for (int i = 0; i < curNote.Count(); i++)
                    {
                        bool test = (ChordDictionary.ChordList[j].Notes.Where(n => n.ClosestRealNoteFrequency == curNote[i].ClosestRealNoteFrequency).Count() > 0);
                        if (test == true)
                            noteCount++;
                    }

                    if ((noteCount / ChordDictionary.ChordList[j].Notes.Count) >= 0.5)
                    {
                        _chord = ChordDictionary.ChordList[j].CharValue.ToString();
                        break;
                    }
                    else
                    {
                        _chord = "";
                    }
                }

                if (_chord == "")
                    continue;

                Application.Current.Dispatcher.Invoke(
                    DispatcherPriority.Render,
                    new Action<string>(updateStaff),
                    _chord);
            }
        }
        private void updateStaff(string staff)
        {
            noteBlock.Text = staff;
        }


        public PluginLayout Layout {
            get { throw new NotImplementedException(); }
        }
    }
}
