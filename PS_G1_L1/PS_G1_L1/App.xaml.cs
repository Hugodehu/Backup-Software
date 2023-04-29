using PS_G1_L1.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Haley.Utils;
using Newtonsoft.Json;
using PS_G1_L1.Model;
using PS_G1_L1.utilities;
using System.IO;
using System.Xml.Serialization;
using PS_G1_L1.Constant;
using System.Diagnostics;

namespace PS_G1_L1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Process proc = Process.GetCurrentProcess();
            int count = Process.GetProcesses().Where(p =>
                p.ProcessName == proc.ProcessName).Count();

            if (count > 1)
            {
                MessageBox.Show("Already an instance is running...");
                App.Current.Shutdown();
            }
            LangUtils.Register(typeof(Language.Resources).Assembly, "PS_G1_L1.Language.Resources");
            InstallerViewModel.Instantiate();
            ChangeCulture(UserConst.UserLanguage.ToString());

            var _wndw = new MainWindow();
            _wndw.ShowDialog();
        }

        public static void ChangeCulture(string code)
        {
            LangUtils.ChangeCulture(code);
        }
    }
}
