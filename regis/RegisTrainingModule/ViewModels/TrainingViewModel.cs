using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Base.ViewModels;
using RegisTrainingModule.Models;
using System.ComponentModel.Composition;
using System.Collections.ObjectModel;
using System.IO;

namespace RegisTrainingModule.ViewModels
{
    [Export]
    class TrainingViewModel : BaseViewModel
    {
        public TrainingViewModel()
        {
            TrainingModules = new ObservableCollection<TrainingModule>();
            LoadTraining();
        }

        private ObservableCollection<TrainingModule> _trainingModules;
        public ObservableCollection<TrainingModule> TrainingModules
        {
            get { return _trainingModules; }
            private set { _trainingModules = value; }
        }

        private void LoadTraining()
        {

                StreamReader readFile = new StreamReader(Environment.CurrentDirectory + "\\training_modules.cfg");
                while (true)
                {
                    string line = readFile.ReadLine();

                    if (line == "#end")
                        break;

                    TrainingModule module = new TrainingModule();
                    module.TargetFreq = new double[8];
                    module.Name = line;

                    for(int i = 0; i < 8; i++)
                    {
                        line = readFile.ReadLine();

                        module.TargetFreq[i] = Convert.ToDouble(line);
                    }

                    TrainingModules.Add(module);
                    line = readFile.ReadLine();
                }
                Console.WriteLine("DEBUG::REGIS:: training_modules.cfg => Loaded");

        }
    }
}
