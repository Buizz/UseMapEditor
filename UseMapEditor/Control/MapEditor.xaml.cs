using Dragablz;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using UseMapEditor.Dialog;

namespace UseMapEditor.Control
{
    /// <summary>
    /// MapEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MapEditor : UserControl
    {
        public double opt_scale = 1;
        public int opt_grid = 0;
        public bool opt_tile = true;
        public bool opt_unit = true;
        public bool opt_sprite = true;
        public bool opt_location = false;
        public bool opt_fogofwar = false;
        public int opt_vision = -1;


        public bool opt_eudeditor = false;






        public Data.MapData mapdata;
        private TabItem tabitem;

        public bool IsLoad = false;
        public MapEditor(string _filepath, TabItem _tabitem)
        {
            InitializeComponent();

            mapdata = new Data.MapData();
            tabitem = _tabitem;


            IsLoad = LoadMap(_filepath);
        }


        public bool NewMap()
        {
            //이름을 아무것도 안넣으면 새 맵
            bool Loadcmp = LoadMap("");

            return Loadcmp;
        }
        public bool OpenMap()
        {
            string mapname = UseMapEditor.Global.WindowTool.OpenMap();

            if (mapname != "")
            {
                return LoadMap(mapname);
            }

            return false;
        }



        public bool LoadMap(string _filepath)
        {
            bool LoadSucess = mapdata.LoadMap(_filepath);
            if (LoadSucess == true)
            {
                tabitem.Header = mapdata.SafeFileName;
                tabitem.Content = this;
            }

            return LoadSucess;
        }
        public bool SaveMap(string _filepath = "")
        {
            bool SaveSucess = mapdata.SaveMap(_filepath);

            if (SaveSucess)
            {
                IsDirty = false;
                tabitem.Header = mapdata.SafeFileName;
            }

            return SaveSucess;
        }



        public bool IsDirty = true;
        public bool CloseMap()
        {
            if (IsDirty == true)
            {
                SaveAskDailog saveDialog = new SaveAskDailog(this);

                saveDialog.Left = this.PointToScreen(new System.Windows.Point(0, 0)).X + this.ActualWidth / 2 - saveDialog.Width / 2;
                saveDialog.Top = this.PointToScreen(new System.Windows.Point(0, 0)).Y + this.ActualHeight / 2 - saveDialog.Height / 2;


                saveDialog.ShowDialog();




                //MessageBoxResult result = MessageBox.Show("파일이 수정되었습니다. 파일 종료합니다. 근데 저장 안됨. 저장 하시겠습니까?", "", MessageBoxButton.YesNoCancel);
                MessageBoxResult result = saveDialog.dialogResult;
                if (result == MessageBoxResult.Yes)
                {
                    //만약 파일 경로가 없을 경우



                    //저장 후 종료
                  if(!SaveMap())
                    {
                        return false;
                    }
                }
                else if (result == MessageBoxResult.No)
                {
                    //Global.WindowTool.ChangeView(this, true);
                    return true;
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return false;
                }
            }


            //Global.WindowTool.ChangeView(this,true);
            return true;
        }








        private void ChangeView()
        {
            Global.WindowTool.ChangeView(this,false);
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            ChangeView();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ChangeView();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            ChangeView();
        }





        private void NewBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CloseMap())
            {
                NewMap();
            }
        }

        private void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CloseMap())
            {
                OpenMap();
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveMap();
        }

        private void SaveAsBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveMap("SaveAs");
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CloseMap())
            {

                TabablzControl tabablz = (TabablzControl)tabitem.Parent;
                tabablz.RemoveFromSource(tabitem);
                //Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                //{
                //    System.Threading.Thread.Sleep(1000);
                //    TabablzControl tabablz = (TabablzControl)tabitem.Parent;
                //    tabablz.RemoveFromSource(tabitem);
                //}));
                          
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeView();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            //Global.WindowTool.ChangeView(this, true);
        }








        //private void MainGrid_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    if(TestGlobal.pcontrol == null)
        //    {
        //        TestGlobal.pcontrol = MainGrid;
        //        MainGrid.Children.Add(TestGlobal.DrawCtrl);
        //    }
        //    else
        //    {
        //        SaveControlImage(TestGlobal.DrawCtrl, "test.png");
        //        TestGlobal.pcontrol.Children.Clear();
        //        TestGlobal.pcontrol = MainGrid;
        //        MainGrid.Children.Add(TestGlobal.DrawCtrl);
        //    }
        //}

        //private void MainGrid_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    if (TestGlobal.pcontrol == MainGrid)
        //    {
        //        TestGlobal.pcontrol.Children.Clear();
        //    }
        //}


    }
}
