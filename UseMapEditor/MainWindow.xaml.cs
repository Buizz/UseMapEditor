using Dragablz;
using Dragablz.Dockablz;
using MahApps.Metro.Controls;
using Microsoft.Win32;
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
using UseMapEditor.Control;
using MaterialDesignThemes.Wpf;
using System.Windows.Threading;

namespace UseMapEditor
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        bool noTabItemClose;
        private void ClosingTabItemHandlerImpl(ItemActionCallbackArgs<TabablzControl> args)
        {
            bool isclose = CloseTabItem((TabItem)args.DragablzItem.Content);

            if (!isclose)
            {
                args.Cancel();
            }
            else
            {
                noTabItemClose = true;
            }
        }

        public bool CloseTabItem(TabItem tabItem)
        {
            MapEditor map = (MapEditor)(tabItem).Content;

            bool Isclosing = map.CloseMap();

            if (Isclosing)
            {

                MainTab.RemoveFromSource(tabItem);




                //Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                //{
                //    System.Threading.Thread.Sleep(1000);
                //    MainTab.RemoveFromSource(tabItem);
                //}));


            }

            return Isclosing;
        }



        /// <summary>
        /// 윈도우 맵을 연다. 사실상 탭을 새로 생성하는 곳
        /// </summary>
        /// <returns></returns>
        private bool OpenTab(string _filepath)
        {
            TabItem titem = new TabItem();
            MapEditor mapEditor = new MapEditor(_filepath, titem);


            if (mapEditor.IsLoad)
            {
                MainTab.AddToSource(titem);
                if (MainTab.SelectedItem == null)
                {
                    MainTab.SelectedItem = titem;
                }
            }

            return true;
        }






        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (noTabItemClose == true)
            {
                noTabItemClose = false;
                e.Cancel = true;
                return;
            }


            //텝이 비어있을 경우 종료해도 됩니다.
           if(MainTab.Items.Count != 0)
            {
                List<TabItem> tabItems = new List<TabItem>();


                for (int i = 0; i < MainTab.Items.Count; i++)
                {
                    tabItems.Add((TabItem)MainTab.Items[i]);
                }


                    for (int i = 0; i < tabItems.Count; i++)
                {
                    bool isclose = CloseTabItem(tabItems[i]);

                    //닫지 않았을 경우
                    if (!isclose)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }
        }


        public void OpenMap()
        {
            string mapname = UseMapEditor.Global.WindowTool.OpenMap();

            if (mapname != "")
            {
                OpenTab(mapname);
            }
        }


        public void NewMap()
        {
            OpenTab("");
        }













        private void refreshStartPage()
        {
            if (MainTab.Items.Count != 0)
            {
                //아이템이 있으면 숨기기
                StartPage.Visibility = Visibility.Collapsed;
            }
            else
            {
                //아이템이 없으면 나타나기
                StartPage.Visibility = Visibility.Visible;
            }
        }





        public MainWindow()
        {
            InitializeComponent();



            MainTab.ClosingItemCallback += ClosingTabItemHandlerImpl;

            StartPage.mainWindow = this;
            PaletteHelper paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();

            theme.SetBaseTheme(Theme.Dark);
            paletteHelper.SetTheme(theme);
            //OpenTab("D:\\User\\Desktop\\chk분석맵.scx");

            //string[] maps = {"Asuna","Marin","Medic"};

            //for (int i = 0; i < 3; i++)
            //{
            //    TabItem titem = new TabItem();
            //    titem.Header = maps[i];
            //    titem.Content = new MapEditor(maps[i]);

            //    MainTab.AddToSource(titem);
            //    //MainTab.Items.Add(titem);
            //}

            Global.WindowTool.LoadGrp();
        }



        private void MainTab_IsEmptyChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            refreshStartPage();
        }

        private void NewMapBtn_Click(object sender, RoutedEventArgs e)
        {
            NewMap();
        }

        private void OpenMapBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenMap();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Global.WindowTool.LoadGrp();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            int a = 0;
            a += 1;



        }
    }
}
