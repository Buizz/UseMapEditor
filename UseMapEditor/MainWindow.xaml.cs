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
using UseMapEditor.Windows;
using System.ComponentModel;
using UseMapEditor.Dialog;

namespace UseMapEditor
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MapEditor mapeditor;
        public bool MapIsLoad()
        {
            if(mapeditor == null)
            {
                return false;
            }


            return mapeditor.IsLoad;
        }


       public void SetWindowName()
       {
            string pName = "USEMAPEDITOR V" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            if (MapIsLoad())
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
            if (MapIsLoad())
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



        public void CancleNewMapCommand()
        {
            mapcreate.Visibility = Visibility.Collapsed;
            mapEditorGrid.Children.Clear();
            startpage.Visibility = Visibility.Visible;

            SetWindowName();
        }

        public void NewMapCommand()
        {
            if (MapIsLoad())
            {
                //로드 되었을 경우 종료를 먼저 함.
                if (!CloseMapCommand())
                {
                    //종료를 안함
                    return;
                }
            }


            mapcreate.Visibility = Visibility.Visible;
            mapEditorGrid.Children.Clear();
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


            if (MapIsLoad())
            {
                //로드 되었을 경우 종료를 먼저 함.
                if (!CloseMapCommand())
                {
                    //종료를 안함
                    return false;
                }
            }
            mapeditor = new MapEditor();
            mapeditor.mainWindow = this;

            bool result = mapeditor.LoadMap(mapname);
            if (result)
            {
                mapcreate.Visibility = Visibility.Collapsed;
                mapEditorGrid.Children.Add(mapeditor);
                startpage.Visibility = Visibility.Collapsed;
                Global.WindowTool.AddLastOpenFile(mapname);
                this.WindowState = WindowState.Maximized;
            }
            SetWindowName();
            return result;
        }


        public bool SaveMapCommand()
        {
            if (!MapIsLoad())
            {
                return false;
            }

            bool result = mapeditor.SaveMap();
            if (result)
            {

            }
            else
            {
                Dialog.MsgDialog msgDialog = new MsgDialog("저장에 실패했습니다.", MessageBoxButton.OK, MessageBoxImage.Error);
                msgDialog.ShowDialog();
            }
            SetWindowName();
            return result;
        }
        public bool SaveAsMapCommand()
        {
            if (!MapIsLoad())
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
            if (!MapIsLoad())
            {
                return false;
            }

            bool result = mapeditor.CloseMap();
            if (result)
            {
                mapcreate.Visibility = Visibility.Collapsed;
                mapEditorGrid.Children.Clear();
                startpage.Visibility = Visibility.Visible;
                startpage.LastOpenFIleRefresh();
                if(mapeditor.ClassTriggerEditorTabItem.trigEditPlus != null)
                {
                    if (!mapeditor.ClassTriggerEditorTabItem.trigEditPlus.IsClosed)
                    {
                        mapeditor.ClassTriggerEditorTabItem.trigEditPlus.Close();
                        mapeditor.EnableWindow();
                    }
                }


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

        private void ProgramSetting_Click(object sender, RoutedEventArgs e)
        {
            ProgramSettingWindow programSettingWindow = new ProgramSettingWindow();
            programSettingWindow.ShowDialog();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
            int W = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width; //모니터 스크린 가로크기
            int H = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height; //모니터 스크린 세로크기

            this.Left = (W - this.Width) / 2;
            this.Top = (H - this.Height) / 2;
        }

        private void MetroWindow_Activated(object sender, EventArgs e)
        {
            if(mapeditor != null)
            {
                if (mapeditor.ClassTriggerEditorTabItem.trigEditPlus != null)
                {

                    if (!mapeditor.ClassTriggerEditorTabItem.trigEditPlus.IsClosed)
                    {
                        Window window = mapeditor.ClassTriggerEditorTabItem.trigEditPlus;

                        if (!window.IsVisible)
                        {
                            window.Show();
                        }

                        if (window.WindowState == WindowState.Minimized)
                        {
                            window.WindowState = WindowState.Normal;
                        }

                        window.Activate();
                        window.Topmost = true;  // important
                        window.Topmost = false; // important
                        window.Focus();         // important

                    }
                }
            }
        }
    }
}
