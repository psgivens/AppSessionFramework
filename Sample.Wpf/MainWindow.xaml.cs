using PhillipScottGivens.Library.AppSessionFramework.ComponentModel;
using PhillipScottGivens.Library.AppSessionFramework.WPF;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [SessionTemplate(typeof(MainSession))]
    public partial class MainWindow : Window {
        static MainWindow() {
            // This list can be generated for us with a code generator. 
            SessionTypeDescriptionProvider<MainSession>.Register();
            SessionTypeDescriptionProvider<SubSession>.Register();
            SessionTypeDescriptionProvider<LoginSession>.Register();

            SessionResolver.Initialize();
        }

        private readonly Sample.Sessions.MainSession _mainSession = Sample.Sessions.AppRoot.MainSession;

        public MainWindow() {
            OperationCommand.Register();
            InitializeComponent();
            DataContext = _mainSession;
        }
    }
}
