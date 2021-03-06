﻿using System;
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
    /// StringSetting.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class StringSetting : UserControl
    {
        private MapEditor mapEditor;
        public void SetMapEditor(MapEditor _mapEditor)
        {
            mapEditor = _mapEditor;

            MainList.Items.Clear();

        }


        public void MainListRefresh()
        {
            mapEditor.mapdata.LoadString();

            MainList.Items.Clear();
            for (int i = 0; i < mapEditor.mapdata.stringDatas.Count; i++)
            {
                MainList.Items.Add(mapEditor.mapdata.stringDatas[i].String);
            }
        }


        public StringSetting()
        {
            InitializeComponent();
        }
    }
}
