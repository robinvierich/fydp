﻿using System;
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
using RegisTunerPlugin.ViewModels;
using System.ComponentModel.Composition;

namespace RegisTunerPlugin
{
    public partial class NeedleTunerControl : UserControl, IPartImportsSatisfiedNotification
    {
        public NeedleTunerControl() {
            InitializeComponent();
        }

        [Import]
        private NeedleTunerViewModel ViewModel {
            get;
            set;
        }

        public void OnImportsSatisfied() {
            DataContext = ViewModel;
        }
    }
}