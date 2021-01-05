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
using static Data.Map.MapData;
using static UseMapEditor.Control.MapEditor;

namespace UseMapEditor.Control.MapEditorControl
{
    /// <summary>
    /// UnitSetting.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UnitSetting : UserControl
    {
        private MapEditor mapEditor;
        public void SetMapEditor(MapEditor _mapEditor)
        {
            mapEditor = _mapEditor;

            codeselecter.SelectionChanged += Codeselecter_SelectionChanged;
            codeselecter.SetCodeType(Codetype.Unit, mapEditor);

            CurrentBinding = mapEditor.mapDataBinding.unitdataBindings[0];
            UnitPanel.DataContext = CurrentBinding;
        }

        private DataBinding.UnitDataBinding CurrentBinding;
        private void Codeselecter_SelectionChanged(object sender, EventArgs e)
        {
            int Selectindex = (int)sender;

            if(Selectindex != -1)
            {
                UnitPanel.Visibility = Visibility.Visible;
            }
            else
            {
                UnitPanel.Visibility = Visibility.Hidden;
            }
            if (Selectindex == -1)
            {
                return;
            }

            CurrentBinding = mapEditor.mapDataBinding.unitdataBindings[Selectindex];
            UnitPanel.DataContext = CurrentBinding;
            //MessageBox.Show(((int)sender).ToString());
        }

        public UnitSetting()
        {
            InitializeComponent();
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
    }
}
