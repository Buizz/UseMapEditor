using Data.Map;
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
using UseMapEditor.Control.MapEditorControl;
using UseMapEditor.FileData;

namespace UseMapEditor.Windows
{
    /// <summary>
    /// TrigEditPlus.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TrigEditPlus : Window
    {
        MapEditor mapEditor;
        TriggerEditor triggerEditor;

        bool IsTrigger;
        public TrigEditPlus(MapEditor mapEditor, TriggerEditor triggerEditor) 
        {
            InitializeComponent();

            this.mapEditor = mapEditor;
            this.triggerEditor = triggerEditor;
            this.IsTrigger = true;
            //Title = mapEditor.mapdata.FilePath;

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < mapEditor.mapdata.Triggers.Count; i++)
            {
                mapEditor.mapdata.Triggers[i].GetTEPText(stringBuilder);
            }


            CodeEditor.Text = stringBuilder.ToString();

        }
        public bool IsClosed { get; set; } = false;
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            triggerEditor.CloseTrigEditPlus();
            IsClosed = true;
            //mapEditor.EnableWindow();
        }

        private Lua.TrigEditPlus.Main teplua = Global.WindowTool.lua.tepMain;

        public void SaveCode()
        {
            List<CTrigger> cTriggers = teplua.exec(CodeEditor.Text, mapEditor, IsTrigger);
            if (cTriggers == null)
            {
                return;
            }


            mapEditor.mapdata.Triggers.Clear();
            foreach (var item in cTriggers)
            {
                mapEditor.mapdata.Triggers.Add(item);
            }
            mapEditor.SetDirty();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveCode();
        }
    }
}
