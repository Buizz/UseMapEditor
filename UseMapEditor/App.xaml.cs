using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace UseMapEditor
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //MainWindow wnd = new MainWindow();
            if (e.Args.Length == 1)
                Global.WindowTool.OpenedFilePath = e.Args[0];
            //wnd.Show();
        }


        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMessage = string.Format("An unhandled exception occurred: {0}\n{1}", e.Exception.Message, e.Exception.ToString());
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            // OR whatever you want like logging etc. MessageBox it's just example
            // for quick debugging etc.
            e.Handled = false;
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Global.Setting.SaveSetting();
        }
    }
}
