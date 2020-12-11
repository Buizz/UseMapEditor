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

namespace UseMapEditor.Control
{
    /// <summary>
    /// StartPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class StartPage : UserControl
    {
        public MainWindow mainWindow;

        public StartPage()
        {
            InitializeComponent();
        }

        private void NewMapBtn_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.NewMap();
        }

        private void OpenMapBtn_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.OpenMap();
        }
    }
}
