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
using System.Windows.Shapes;
using UseMapEditor.Control;

namespace UseMapEditor.Dialog
{
    /// <summary>
    /// SaveAskDailog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SaveAskDailog : MetroWindow
    {
        public MessageBoxResult dialogResult = MessageBoxResult.Cancel;


        public SaveAskDailog(MapEditor mapEditor)
        {
            InitializeComponent();

            InforText.Text = $"변경 내용을 {mapEditor.mapdata.SafeFileName}에 저장하시겠습니까?";
        }

        private void YesBtn_Click(object sender, RoutedEventArgs e)
        {
            dialogResult = MessageBoxResult.Yes;
            Close();
        }

        private void NoBtn_Click(object sender, RoutedEventArgs e)
        {
            dialogResult = MessageBoxResult.No;
            Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            dialogResult = MessageBoxResult.Cancel;
            Close();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SaveBtn.Focus();
        }
    }
}
