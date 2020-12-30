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

namespace UseMapEditor.Control.MapEditorControl
{
    /// <summary>
    /// PlayerSettingPanel.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PlayerSettingPanel : UserControl
    {
        public PlayerSettingPanel()
        {
            InitializeComponent();
        }
        public void SetMapEditor(MapEditor mapEditor)
        {
            PlayerPanel.Children.Clear();

            for (int i = 0; i < 8; i++)
            {
                PlayerPanel.Children.Add(new PlayerSetting(mapEditor, i));
            }
            //PlayerPanel.Children.Add(new Separator());

            //for (int i = 8; i < 12; i++)
            //{
            //    PlayerPanel.Children.Add(new PlayerSetting(mapEditor, i));
            //}
        }
    }
}
