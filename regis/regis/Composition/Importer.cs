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
        public Importer()
        {
        }

        public static AggregateCatalog _mainCatalog;
        public static CompositionContainer _container;

        public static void Compose(object root)
        {
            if (_mainCatalog == null)
            {
                _mainCatalog = new AggregateCatalog();

                //Adds all the parts found in the assemblies that are located in the plugin directory
                _mainCatalog.Catalogs.Add(new DirectoryCatalog(System.IO.Path.GetDirectoryName(Regis.Strings.PluginPath)));
                _mainCatalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
            }

            if (_container == null)
            {
                _container = new CompositionContainer(_mainCatalog);
                _container.ComposeExportedValue("DefaultNoteStream", 0);
            }

            _container.ComposeParts(root);
        }
    }
}
