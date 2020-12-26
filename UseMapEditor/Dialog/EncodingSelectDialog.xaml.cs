using Data.Map;
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

namespace UseMapEditor.Dialog
{
    /// <summary>
    /// EncodingSelectDialog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EncodingSelectDialog : MetroWindow
    {
        MapData mapData;

        Encoding encoding;

        Encoding[] encodinglist = { Encoding.UTF8, Encoding.GetEncoding(949) };
        public EncodingSelectDialog(MapData _mapData)
        {
            InitializeComponent();

            mapData = _mapData;
            encoding = _mapData.ENCODING;


            for (int i = 0; i < encodinglist.Length; i++)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = encodinglist[i].EncodingName;
                comboBoxItem.Tag = encodinglist[i].CodePage;

                encodingcb.Items.Add(comboBoxItem);
                if (encoding.CodePage == encodinglist[i].CodePage)
                {
                    encodingcb.SelectedIndex = encodingcb.Items.Count - 1;
                }
            }

        }
        
        public void ResetPreviewText()
        {
            StrPreview.Text = "";
            for (int i = 0; i < Math.Min( 20, mapData.BYTESTRx.Count); i++)
            {
                StrPreview.AppendText(encoding.GetString(mapData.BYTESTRx[i]) + "\n");
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            encoding = Encoding.GetEncoding((int)((ComboBoxItem)encodingcb.SelectedItem).Tag);
            ResetPreviewText();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mapData.ENCODING = encoding;
            Close();
        }
    }
}
