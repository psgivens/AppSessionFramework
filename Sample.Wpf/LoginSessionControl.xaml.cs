﻿using PhillipScottGivens.Library.AppSessionFramework.WPF;
using Sample.Sessions.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sample.Wpf {
    /// <summary>
    /// Interaction logic for LoginSessionControl.xaml
    /// </summary>
    [SessionTemplate(typeof(LoginSession))]
    public partial class LoginSessionControl : UserControl {
        public LoginSessionControl() {
            InitializeComponent();
        }
    }
}
