using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UseMapEditor.Control
{
    /// <summary>
    /// MapEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MapEditor : UserControl
    {
        public Data.MapData mapdata;



        public MapEditor(string _filepath)
        {
            InitializeComponent();

            mapdata = new Data.MapData(_filepath);
        }



        private void ChangeView()
        {
            Global.WindowTool.ChangeView(this);
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            ChangeView();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ChangeView();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            ChangeView();
        }








        //private void MainGrid_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    if(TestGlobal.pcontrol == null)
        //    {
        //        TestGlobal.pcontrol = MainGrid;
        //        MainGrid.Children.Add(TestGlobal.DrawCtrl);
        //    }
        //    else
        //    {
        //        SaveControlImage(TestGlobal.DrawCtrl, "test.png");
        //        TestGlobal.pcontrol.Children.Clear();
        //        TestGlobal.pcontrol = MainGrid;
        //        MainGrid.Children.Add(TestGlobal.DrawCtrl);
        //    }
        //}

        //private void MainGrid_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    if (TestGlobal.pcontrol == MainGrid)
        //    {
        //        TestGlobal.pcontrol.Children.Clear();
        //    }
        //}


    }
}
