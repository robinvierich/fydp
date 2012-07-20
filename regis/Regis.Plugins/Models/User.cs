using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace Regis.Plugins.Models
{
    [DataContract]
    public class User
    {
        private string _name;
        
        [DataMember]
	    public string Name
	    {
		    get { return _name;}
		    set { _name = value;}
	    }


        private ObservableCollection<UserTrainingStats> _trainingStats = new ObservableCollection<UserTrainingStats>();

        [DataMember]
        public ObservableCollection<UserTrainingStats> TrainingStats
        {
            get { return _trainingStats; }
            set { _trainingStats = value; }
        }

        
    }
}
