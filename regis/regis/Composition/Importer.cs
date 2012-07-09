using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using Regis.Plugins.Models;

namespace Regis.Composition
{
    public class Importer
    {
        NoteStream _noteStream;

        public Importer()
        {
            _noteStream = new NoteStream();
        }

        public void Compose(object root)
        {

            var catalog = new AggregateCatalog();

            //Adds all the parts found in all assemblies in 
            //the same directory as the executing program
            catalog.Catalogs.Add(new DirectoryCatalog(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)));
            catalog.Catalogs.Add(new DirectoryCatalog(System.IO.Path.GetDirectoryName(Regis.Strings.PluginPath)));
            catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));


            var container = new CompositionContainer(catalog);
            container.ComposeExportedValue("DefaultNoteStream", _noteStream);
            container.ComposeParts(root);
        }

    

    }
}
