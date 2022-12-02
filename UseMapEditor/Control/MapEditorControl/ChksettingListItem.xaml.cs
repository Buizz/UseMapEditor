using Data.Map;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using UseMapEditor.FileData;
using UseMapEditor.Global;
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
            string foldername = AppDomain.CurrentDomain.BaseDirectory + @"Data\Excel";

            ExcelData excelData = new ExcelData(mapEditor);
            if (!Directory.Exists(foldername))
            {
                Directory.CreateDirectory(foldername);
            }

            string fname = foldername + @"\" + Guid.NewGuid().ToString() + ".xlsx";

            excelData.SaveExcel(fname, excelType);

            WindowTool.NewExcelProcess(mapEditor, fname);
            excelData.Dispos();
        }

        private void LoadExcel_Click(object sender, RoutedEventArgs e)
        {
            ExcelData excelData = new ExcelData(mapEditor);
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel|*.xlsx";

            if ((bool)openFileDialog.ShowDialog())
            {
                excelData.LoadExcel(openFileDialog.FileName, excelType);
            }
            excelData.Dispos();
        }

        private void SaveExcel_Click(object sender, RoutedEventArgs e)
        {
            ExcelData excelData = new ExcelData(mapEditor);
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel|*.xlsx";

            if ((bool)saveFileDialog.ShowDialog())
            {
                excelData.SaveExcel(saveFileDialog.FileName, excelType);
            }
            excelData.Dispos();
        }
    }
}
