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
using UseMapEditor.FileData;

namespace UseMapEditor.Control.MapEditorControl
{
    /// <summary>
    /// ChkSetting.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ChkSetting : UserControl
    {
        public ChkSetting()
        {
            InitializeComponent();
        }

        public void SetMapEditor(MapEditor mapEditor)
        {
            MainList.Children.Clear();

            foreach (ExcelData.ExcelType item in Enum.GetValues(typeof(ExcelData.ExcelType)))
            {
                if(item == ExcelData.ExcelType.Code)
                {
                    continue;
                }
                ChksettingListItem chksettingListItem = new ChksettingListItem();

                chksettingListItem.Init(item, mapEditor);

                MainList.Children.Add(chksettingListItem);
            }
        }
    }
}
