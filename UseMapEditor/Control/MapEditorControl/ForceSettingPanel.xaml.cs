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
    /// ForceSettingPanel.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ForceSettingPanel : UserControl
    {
        private MapEditor mapEditor;
        public void SetMapEditor(MapEditor _mapEditor)
        {
            mapEditor = _mapEditor;

            MainGrid.Children.Clear();
            for (int i = 0; i < 4; i++)
            {
                MainGrid.Children.Add(new ForceSetting(mapEditor, i));
            }
        }
        public void MainListRefresh()
        {
            for (int i = 0; i < 4; i++)
            {
                ((ForceSetting)MainGrid.Children[i]).Refresh();
            }
        }



        public ForceSettingPanel()
        {
            InitializeComponent();
        }
    }
}
