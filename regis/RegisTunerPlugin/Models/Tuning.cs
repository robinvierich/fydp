using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace RegisTunerPlugin.Models
{
    public class Tuning
    {
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        private ObservableCollection<GuitarString> _guitarStrings;
        public ObservableCollection<GuitarString> GuitarStrings
        {
            get
            {
                return _guitarStrings;
            }

            set
            {
                _guitarStrings = value;
            }

        }

    }
}
