using MahApps.Metro.Controls;
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

namespace UseMapEditor
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        public MainWindow()
        {
            InitializeComponent();

            string[] maps = {"Asuna","Marin","Medic"};

            for (int i = 0; i < 3; i++)
            {
                TabItem titem = new TabItem();
                titem.Header = maps[i];
                titem.Content = new MapEditor(maps[i]);

                MainTab.Items.Add(titem);
            }
        }
    }
}
