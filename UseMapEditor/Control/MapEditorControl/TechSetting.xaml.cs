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
using static UseMapEditor.Control.MapEditor;

namespace UseMapEditor.Control.MapEditorControl
{
    /// <summary>
    /// TechSetting.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TechSetting : UserControl
    {
        private MapEditor mapEditor;
        public void SetMapEditor(MapEditor _mapEditor)
        {
            mapEditor = _mapEditor;

            codeselecter.SelectionChanged += Codeselecter_SelectionChanged;
            codeselecter.SetCodeType(Codetype.Tech, mapEditor);


            CurrentBinding = mapEditor.mapDataBinding.techDataBindings[0];
            ContentPanel.DataContext = CurrentBinding;
        }



        private DataBinding.TechDataBinding CurrentBinding;
        private void Codeselecter_SelectionChanged(object sender, EventArgs e)
        {
            int Selectindex = (int)sender;

            if (Selectindex != -1)
            {
                ContentPanel.Visibility = Visibility.Visible;
            }
            else
            {
                ContentPanel.Visibility = Visibility.Hidden;
            }
            if (Selectindex == -1)
            {
                return;
            }

            CurrentBinding = mapEditor.mapDataBinding.techDataBindings[Selectindex];
            ContentPanel.DataContext = CurrentBinding;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CurrentBinding.AddDEFAULTCOLOR();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CurrentBinding.AddPLAYERCOLOR(0);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            CurrentBinding.AddPLAYERCOLOR(1);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            CurrentBinding.AddPLAYERCOLOR(2);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            CurrentBinding.AddPLAYERCOLOR(3);
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            CurrentBinding.AddPLAYERCOLOR(4);
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            CurrentBinding.AddPLAYERCOLOR(5);
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            CurrentBinding.AddPLAYERCOLOR(6);
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            CurrentBinding.AddPLAYERCOLOR(7);
        }
        public TechSetting()
        {
            InitializeComponent();
        }
    }
}
