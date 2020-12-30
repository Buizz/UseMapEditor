using Data;
using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using UseMapEditor.FileData;
using UseMapEditor.Windows;

namespace UseMapEditor.Dialog
{
    /// <summary>
    /// ProgramStart.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProgramStart : MetroWindow
    {
        public ProgramStart()
        {
            InitializeComponent();
        }

        public static string tempfolder = AppDomain.CurrentDomain.BaseDirectory + @"Data\temp";
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.IO.Directory.Exists(tempfolder))
            {
                System.IO.Directory.CreateDirectory(tempfolder);
            }


            Process[] procs = Process.GetProcessesByName("usemapeditor");
            // 두번 이상 실행되었을 때 처리할 내용을 작성합니다.
            if (procs.Length > 1)
            {
                //MessageBox.Show("프로그램이 이미 실행되고 있습니다.\n다시 한번 확인해주시기 바랍니다.");

                uint rv = 0;
                Random rd = new Random();


                if (Global.WindowTool.OpenedFilePath != null)
                {
                    //파일 연결하기
                    rv = (uint) rd.Next(0, 10000);

                    StreamWriter sw = new StreamWriter(tempfolder + @"\openPath");
                    sw.Write(Global.WindowTool.OpenedFilePath);
                    sw.Close();
                }
                else
                {
                    //일반으로 열기
                    rv = (uint) rd.Next(30000, 40000);
                }

                PostMessage((IntPtr)HWND_BROADCAST, message, (uint)handle, rv);
                Close();
                return;
            }

            

            Global.Setting.LoadSetting();






            Global.WindowTool.scdata = new SCData();
            Global.WindowTool.iscript = new Iscript(AppDomain.CurrentDomain.BaseDirectory + @"\Data\iscript.bin", false);
            Global.WindowTool.iscriptX = new Iscript(AppDomain.CurrentDomain.BaseDirectory + @"\Data\iscriptx.bin", true);


            Global.WindowTool.stat_txt = new tblreader(AppDomain.CurrentDomain.BaseDirectory + @"\Data\tbls\stat_txt.tbl");
            Global.WindowTool.stat_txt_kor_eng = new tblreader(AppDomain.CurrentDomain.BaseDirectory + @"\Data\tbls\stat_txt_kor_eng.tbl");
            Global.WindowTool.stat_txt_kor_kor = new tblreader(AppDomain.CurrentDomain.BaseDirectory + @"\Data\tbls\stat_txt_kor_kor.tbl");



            Global.WindowTool.programStart = this;



            PaletteHelper paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();
            if (Global.Setting.Vals[Global.Setting.Settings.Program_IsDark] == "true")
            {
                theme.SetBaseTheme(Theme.Dark);
            }
            else
            {
                theme.SetBaseTheme(Theme.Light);
            }
            paletteHelper.SetTheme(theme);



            this.WindowState = WindowState.Normal;
            int W = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width; //모니터 스크린 가로크기
            int H = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height; //모니터 스크린 세로크기

            this.Left = (W - this.Width) / 2;
            this.Top = (H - this.Height) / 2;

            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += delegate (object _sender, DoWorkEventArgs _e)
            {
                System.Threading.Thread.Sleep(500);
            };

            backgroundWorker.RunWorkerCompleted += delegate (object _sender, RunWorkerCompletedEventArgs _e)
            {
                if (!File.Exists(Global.Setting.Vals[Global.Setting.Settings.Program_StarCraftPath]))
                {
                    //스타크래프트 파일이 존재하지 않을 경우
                    MsgDialog msg = new MsgDialog("StarCraft 실행파일을 지정하겠습니까?\n지정하지 않으면 그래픽을 로드 하지 않습니다.", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    msg.ShowDialog();
                    if (msg.msgresult == MessageBoxResult.No)
                    {
                        LoadCmp();
                        return;
                    }
                    else
                    {
                        OpenFileDialog openFileDialog = new OpenFileDialog();
                        openFileDialog.Filter = "스타크래프트 실행파일|StarCraft Launcher.exe";

                        if ((bool)openFileDialog.ShowDialog())
                        {
                            Global.Setting.Vals[Global.Setting.Settings.Program_StarCraftPath] = openFileDialog.FileName;
                        }
                        else
                        {
                            //다이어로그 취소함
                            LoadCmp();
                            return;
                        }
                    }
                }

                if (Global.Setting.Vals[Global.Setting.Settings.Program_GRPLoad] == "false")
                {
                    //GRP전처리 과정을 수행하지 않음.
                    this.Visibility = Visibility.Collapsed;
                    MsgDialog msg = new MsgDialog("그래픽 전처리 과정을 수행하겠습니까?\n이 작업은 몇분 정도 걸릴 수 있습니다.\n지정하지 않으면 그래픽을 로드 하지 않습니다.", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    msg.ShowDialog();
                    this.Visibility = Visibility.Visible;
                    if (msg.msgresult == MessageBoxResult.No)
                    {
                        LoadCmp();
                        return;
                    }
                    else
                    {
                        this.Visibility = Visibility.Collapsed;
                        Preprocessing preprocessing = new Preprocessing();
                        preprocessing.ShowDialog();
                        this.Visibility = Visibility.Visible;
                        if (!preprocessing.IsClose)
                        {
                            LoadCmp();
                            return;
                        }
                        else
                        {
                            Global.Setting.Vals[Global.Setting.Settings.Program_GRPLoad] = "true";
                        }
                    }
                }




                //GrpTestLoad.Children.Clear();


                LoadCmp();
            };

            backgroundWorker.RunWorkerAsync();
        }

        private void LoadCmp()
        {

            Global.WindowTool.LoadGrp();
            GrpTestLoad.Children.Add(Global.WindowTool.MapViewer);


            this.WindowState = WindowState.Normal;
            LoadComplete = true;
            MainWindow main = new MainWindow();
            main.Show();
            main.Focus();
            Visibility = Visibility.Collapsed;
        }





        private bool LoadComplete;



        private uint message;
        private IntPtr handle;
        public const uint HWND_BROADCAST = 0xffff;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern uint RegisterWindowMessage(string lpString);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            handle = new WindowInteropHelper(this).Handle;
            message = RegisterWindowMessage("OpenCommand");
            ComponentDispatcher.ThreadFilterMessage += new ThreadMessageEventHandler(ComponentDispatcher_ThreadFilterMessage);
        }


        uint lastmessage = 20000;
        void ComponentDispatcher_ThreadFilterMessage(ref MSG msg, ref bool handled)
        {
            if (msg.message == message && msg.wParam != handle)
            {
                uint resultmessage = (uint)msg.lParam;
                if (resultmessage != lastmessage)
                {
                    lastmessage = resultmessage;

                    if (LoadComplete)
                    {
                        if (resultmessage <= 20000)
                        {
                            //파일 연결
                            StreamReader sr = new StreamReader(tempfolder + @"\openPath");
                            Global.WindowTool.OpenedFilePath = sr.ReadLine().Trim();
                            sr.Close();
                        }
                        MainWindow main = new MainWindow();
                        main.Show();

                    }
                    else
                    {
                        MsgDialog errorDialog = new MsgDialog("프로그램이 로딩 중에는 창을 열 수 없습니다.", MessageBoxButton.OK, MessageBoxImage.Error);
                        errorDialog.ShowDialog();
                    }
                }
            }
        }

    }
}
