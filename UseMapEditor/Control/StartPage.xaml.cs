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

namespace UseMapEditor.Control
{
    /// <summary>
    /// StartPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class StartPage : UserControl
    {
        public MainWindow mainWindow;

        public StartPage()
        {
            InitializeComponent();

            LastOpenFIleRefresh();
        }
        public void LastOpenFIleRefresh()
        {
            LastOpendPanel.Children.Clear();

            string[] FileList = Global.WindowTool.GetLastOpenfile();

            for (int i = FileList.Length - 1; i >= 0; i--)
            {
                if(FileList[i] != "")
                {
                    Button button = new Button();

                    button.Padding = new Thickness(4);
                    button.Height = double.NaN;
                    button.Content = new LastOpenFileControl(FileList[i]);
                    button.Tag = FileList[i];
                    button.Style = (Style)Application.Current.Resources["MaterialDesignOutlinedButton"];

                    button.Click += Button_Click;

                    LastOpendPanel.Children.Add(button);
                }        
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string filepath = (string)((Button)sender).Tag;

            mainWindow.OpenMapCommand(filepath);
        }

        private void NewMapBtn_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.NewMapCommand();
        }

        private void OpenMapBtn_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.OpenMapCommand();
        }
    }
}
