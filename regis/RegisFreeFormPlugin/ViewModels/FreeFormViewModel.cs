using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Base.ViewModels;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Regis.Plugins.Statics;

namespace RegisFreeFormPlugin.ViewModels
{
    [Export]
    class FreeFormViewModel : BaseViewModel
    {
        private double[] _frequencies; 
        public FreeFormViewModel()
        {
            _frequencies = NoteDictionary.NoteDict.Keys.ToArray<double>();
        }
    }
}
