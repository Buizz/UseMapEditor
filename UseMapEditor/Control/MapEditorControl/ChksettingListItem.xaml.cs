using Data.Map;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using static Data.Map.MapData;

namespace UseMapEditor.Control.MapEditorControl
{
    /// <summary>
    /// ChksettingListItem.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ChksettingListItem : UserControl
    {
        private MapEditor mapEditor;

        public ChksettingListItem()
        {
            InitializeComponent();
        }
        public ExcelData.ExcelType excelType;

        public void Init(ExcelData.ExcelType excelType, MapEditor mapEditor)
        {
            this.excelType = excelType;
            this.mapEditor = mapEditor;
            Header.Text = ExcelData.GetHeader(excelType);
        }

        private void OpenWithExcel_Click(object sender, RoutedEventArgs e)
        {
            ExcelData excelData = new ExcelData(mapEditor);

            string fname = AppDomain.CurrentDomain.BaseDirectory + @"temp.xlsx"; ;

            excelData.SaveExcel(fname, excelType);

            Process.Start(fname);
        }

        private void LoadExcel_Click(object sender, RoutedEventArgs e)
        {
            ExcelData excelData = new ExcelData(mapEditor);
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "엑셀|*.xlsx";

            if ((bool)openFileDialog.ShowDialog())
            {
                excelData.LoadExcel(openFileDialog.FileName, excelType);
            }
        }

        private void SaveExcel_Click(object sender, RoutedEventArgs e)
        {
            ExcelData excelData = new ExcelData(mapEditor);
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "엑셀|*.xlsx";

            if ((bool)saveFileDialog.ShowDialog())
            {
                excelData.SaveExcel(saveFileDialog.FileName, excelType);
            }
        }
    }
}
