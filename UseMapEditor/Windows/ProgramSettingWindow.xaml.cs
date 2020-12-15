using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace UseMapEditor.Windows
{
    /// <summary>
    /// ProgramSettingWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProgramSettingWindow : MetroWindow
    {
        public ProgramSettingWindow()
        {
            InitializeComponent();
        }
        string tempfolder = AppDomain.CurrentDomain.BaseDirectory + @"\Data\temp";
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.IO.Directory.Exists(tempfolder))
            {
                System.IO.Directory.CreateDirectory(tempfolder);
            }


         



            Global.Setting.LoadSetting();



            PaletteHelper paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();
            if (Global.Setting.Vals[Global.Setting.Settings.Program_IsDark] == "true")
            {
                theme.SetBaseTheme(Theme.Dark);
                paletteHelper.SetTheme(theme);
                DarkToggleBtn.IsChecked = true;
            }
            else
            {
                theme.SetBaseTheme(Theme.Light);
                paletteHelper.SetTheme(theme);
                DarkToggleBtn.IsChecked = false;
            }



            SCPathTB.Text = Global.Setting.Vals[Global.Setting.Settings.Program_StarCraftPath];


            if (Global.Setting.Vals[Global.Setting.Settings.Program_GRPLoad] == "true")
            {
                GrpLoadBtn.Content = "그래픽 로드";
                GrpLoadBtn.IsEnabled = false;
                ButtonProgressAssist.SetIsIndeterminate(GrpLoadBtn, true);
                GrpLoad();
            }
            else
            {
                GrpLoadBtn.Content = "그래픽 전 처리";
            }




            //Global.WindowTool.LoadGrp();


            //MainWindow main = new MainWindow();
            //main.Show();
            //Close();
        }


        private void Dataprocessing()
        {
            //데이터를 모두 처리하는 함수
            BackgroundWorker databgWorker = new BackgroundWorker();

            databgWorker.RunWorkerCompleted += DatabgWorker_RunWorkerCompleted;
            databgWorker.DoWork += DatabgWorker_DoWork;


            databgWorker.RunWorkerAsync();




        }

        private void DatabgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(4000);
        }

        private void DatabgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Global.Setting.Vals[Global.Setting.Settings.Program_GRPLoad] = "true";
                GrpLoadBtn.Content = "그래픽 로드";
                GrpLoad();
            }
            else
            {
                ErrorText.Text = "그래픽 전처리에 실패했습니다.";
                GrpLoadBtn.IsEnabled = true;
                ButtonProgressAssist.SetIsIndeterminate(GrpLoadBtn, false);
            }
        }



        private void GrpLoad()
        {
            //기본 데이터를 불러오는 함수

            this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {





                GrpLoadBtn.Content = "그래픽 로드 완료";
                GrpLoadBtn.IsEnabled = false;
                ButtonProgressAssist.SetIsIndeterminate(GrpLoadBtn, false);
            }));



            //BackgroundWorker grploadWorker = new BackgroundWorker();

            //grploadWorker.RunWorkerCompleted += GrploadWorker_RunWorkerCompleted;
            //grploadWorker.DoWork += GrploadWorker_DoWork;


            //grploadWorker.RunWorkerAsync();

        }

        private void GrploadWorker_DoWork(object sender, DoWorkEventArgs e)
        {


        }

        private void GrploadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                GrpLoadBtn.Content = "그래픽 로드 완료";
                GrpLoadBtn.IsEnabled = false;
                ButtonProgressAssist.SetIsIndeterminate(GrpLoadBtn, false);
            }
            else
            {
                ErrorText.Text = "그래픽 로드에 실패했습니다.\n" + e.Error.ToString();
                GrpLoadBtn.IsEnabled = true;
                ButtonProgressAssist.SetIsIndeterminate(GrpLoadBtn, false);
            }
        }

        private void DarkToggleBtn_Checked(object sender, RoutedEventArgs e)
        {
            Global.Setting.Vals[Global.Setting.Settings.Program_IsDark] = "true";

            PaletteHelper paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();
            theme.SetBaseTheme(Theme.Dark);
            paletteHelper.SetTheme(theme);
        }

        private void DarkToggleBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            Global.Setting.Vals[Global.Setting.Settings.Program_IsDark] = "false";

            PaletteHelper paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();
            theme.SetBaseTheme(Theme.Light);
            paletteHelper.SetTheme(theme);
        }

        private void SCMapBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GrpLoadBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
