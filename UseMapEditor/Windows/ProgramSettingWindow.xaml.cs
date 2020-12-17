using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
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
                RenderSetting.IsEnabled = true;
            }
            else
            {
                RenderSetting.IsEnabled = false;
                ErrorText.Text = "그래픽이 로드되지 않았습니다.";
            }


            MaxFrameTB.Text = Global.Setting.Vals[Global.Setting.Settings.Render_MaxFrame];
            UseVFRCB.IsChecked = (Global.Setting.Vals[Global.Setting.Settings.Render_UseVFR] == "true");



            //Global.WindowTool.LoadGrp();


            //MainWindow main = new MainWindow();
            //main.Show();
            //Close();
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
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "스타크래프트 실행파일|StarCraft Launcher.exe";

            if ((bool)openFileDialog.ShowDialog())
            {
                Global.Setting.Vals[Global.Setting.Settings.Program_StarCraftPath] = openFileDialog.FileName;
                SCPathTB.Text = openFileDialog.FileName;
                ErrorText.Text = "그래픽을 다시 로드하려면 프로그램을 껐다켜야 합니다.";
                Global.Setting.Vals[Global.Setting.Settings.Program_GRPLoad] = "false";
            }
            else
            {
                return;
            }
        }


        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            int mf;
            if (int.TryParse(MaxFrameTB.Text, out mf))
            {
                Global.Setting.Vals[Global.Setting.Settings.Render_MaxFrame] = mf.ToString();
            }
            Global.Setting.Vals[Global.Setting.Settings.Render_UseVFR] = UseVFRCB.IsChecked.ToString().ToLower();
        }
    }
}
