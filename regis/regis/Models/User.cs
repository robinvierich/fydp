using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Regis.Models
{
    [DataContract]
    public class User
    {
        private string _userName;
        
        [DataMember]
	    public string UserName
	    {
		    get { return _userName;}
		    set { _userName = value;}
	    }

        //propfull tab tab
        //Sample Stuff
        //private double[] powerBins;
        //public double[] PowerBins
        //{
        //    get { return powerBins; }
        //    set { powerBins = value; }
        //}
    }
}
