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
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using Regis.ViewModels;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using Regis.Plugins.Models;
using Regis.Composition;

namespace Regis.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Importer _importer;

        public MainWindow()
        {
            InitializeComponent();
            _importer = new Importer();
            _importer.Compose(this);

            DataContext = ViewModel;
        }

        [Import(typeof(MainWindowViewModel))]
        public MainWindowViewModel ViewModel
        {
            get;
            private set;
        }

    }
}
