using Dragablz;
using Dragablz.Dockablz;
using MahApps.Metro.Controls;
using Microsoft.Win32;
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
using UseMapEditor.Control;
using MaterialDesignThemes.Wpf;
using System.Windows.Threading;

namespace UseMapEditor
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
       public void SetWindowName()
       {
            string pName = "USEMAPEDITOR V" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            if (mapeditor.IsLoad)
            {
                if (mapeditor.IsDirty)
                {
                    Title = mapeditor.mapdata.SafeFileName + "* - " + pName;
                }
                else
                {
                    Title = mapeditor.mapdata.SafeFileName + " - " + pName;
                }
            }
            else
            {
                Title = pName;
            }
       }






        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (mapeditor.IsLoad)
            {
                bool result = CloseMapCommand();

                if (!result)
                {
                    e.Cancel = true;
                }
            }
        }




        private void NewMapBtn_Click(object sender, RoutedEventArgs e)
        {
            NewMapCommand();
            //NewMap();
        }

        private void OpenMapBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenMapCommand();
            //OpenMap();
        }

        private void SaveMapBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveMapCommand();
        }

        private void SaveAsMapBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveAsMapCommand();
        }

        private void CloseMapBtn_Click(object sender, RoutedEventArgs e)
        {
            CloseMapCommand();
        }

        private void NewWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            NewWindowCommand();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            ExitCommand();
        }





        public void NewMapCommand()
        {
            if (mapeditor.IsLoad)
            {
                //로드 되었을 경우 종료를 먼저 함.
                if (!CloseMapCommand())
                {
                    //종료를 안함
                    return;
                }
            }


            mapcreate.Visibility = Visibility.Visible;
            mapeditor.Visibility = Visibility.Collapsed;
            startpage.Visibility = Visibility.Collapsed;

            SetWindowName();


            //bool result = mapeditor.NewMap();
            //if (result)
            //{

            //}
            //SetWindowName();
            //return result;
        }


        public bool OpenMapCommand(string filename = "")
        {
            string mapname;
            if (filename == "")
            {
                mapname = UseMapEditor.Global.WindowTool.OpenMap();
                if(mapname == "")
                {
                    return false;
                }
            }
            else
            {
                mapname = filename;
            }


            if (mapeditor.IsLoad)
            {
                //로드 되었을 경우 종료를 먼저 함.
                if (!CloseMapCommand())
                {
                    //종료를 안함
                    return false;
                }
            }


            bool result = mapeditor.LoadMap(mapname);
            if (result)
            {
                mapcreate.Visibility = Visibility.Collapsed;
                mapeditor.Visibility = Visibility.Visible;
                startpage.Visibility = Visibility.Collapsed;
            }
            SetWindowName();
            return result;
        }
        public bool SaveMapCommand()
        {
            if (!mapeditor.IsLoad)
            {
                return false;
            }

            bool result = mapeditor.SaveMap();
            if (result)
            {

            }
            SetWindowName();
            return result;
        }
        public bool SaveAsMapCommand()
        {
            if (!mapeditor.IsLoad)
            {
                return false;
            }

            bool result = mapeditor.SaveMap("SaveAs");
            if (result)
            {

            }
            SetWindowName();
            return result;
        }
        public bool CloseMapCommand()
        {
            if (!mapeditor.IsLoad)
            {
                return false;
            }

            bool result = mapeditor.CloseMap();
            if (result)
            {
                mapcreate.Visibility = Visibility.Collapsed;
                mapeditor.Visibility = Visibility.Collapsed;
                startpage.Visibility = Visibility.Visible;
            }
            SetWindowName();
            return result;
        }





        public void NewWindowCommand()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        public void ExitCommand()
        {
            Close();
        }






        public MainWindow()
        {
            InitializeComponent();

            mapcreate.mainWindow = this;
            mapeditor.mainWindow = this;
            startpage.mainWindow = this;
            SetWindowName();



            if (Global.WindowTool.OpenedFilePath != null)
            {
                OpenMapCommand(Global.WindowTool.OpenedFilePath);
                Global.WindowTool.OpenedFilePath = null;
            }
        }






        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            int a = 0;
            a += 1;



        }

        private void ConnectExecMenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = System.AppDomain.CurrentDomain.BaseDirectory + @"Data\ExtensionRegister.exe";
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.Verb = "runas";
            p.StartInfo.Arguments = System.AppDomain.CurrentDomain.BaseDirectory + "UseMapEditor.exe" + ",scx,scp";

            p.Start();
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            Global.WindowTool.CloseProgram();
        }
    }
}
