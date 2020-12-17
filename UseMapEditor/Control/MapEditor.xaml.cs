using Data.Map;
using Dragablz;
using Microsoft.Xna.Framework;
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
using UseMapEditor.MonoGameControl;

namespace UseMapEditor.Control
{
    /// <summary>
    /// MapEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MapEditor : UserControl
    {

        private int _opt_xpos = 0;
        private int _opt_ypos = 0;




        public int opt_xpos
        {
            get
            {
                return _opt_xpos;
            }
            set
            {

                _opt_xpos = value;
                int MapLeft = -(int)(MapViewer.ActualWidth / opt_scalepercent / 2);
                int MapRight = mapdata.WIDTH * 32 + MapLeft;

                Vector2 MapMin = PosMapToScreen(new Vector2(0, 0));
                Vector2 MapMax = PosMapToScreen(new Vector2(mapdata.WIDTH, mapdata.HEIGHT) * 32);
                Vector2 MapSize = MapMax - MapMin;
                if (!((MapSize.X < MapViewer.ActualWidth) & (MapSize.Y < MapViewer.ActualHeight)))
                {
                    if (_opt_xpos < MapLeft)
                    {
                        _opt_xpos = MapLeft;
                    }
                    if (_opt_xpos > MapRight)
                    {
                        _opt_xpos = MapRight;
                    }
                }
            }
        }
        public int opt_ypos
        {

            get
            {
                return _opt_ypos;
            }
            set
            {
                _opt_ypos = value;
                int MapUp = -(int)(MapViewer.ActualHeight / opt_scalepercent / 2);
                int MapDown = mapdata.HEIGHT * 32 + MapUp;

                Vector2 MapMin = PosMapToScreen(new Vector2(0, 0));
                Vector2 MapMax = PosMapToScreen(new Vector2(mapdata.WIDTH, mapdata.HEIGHT) * 32);
                Vector2 MapSize = MapMax - MapMin;
                if (!((MapSize.X < MapViewer.ActualWidth) & (MapSize.Y < MapViewer.ActualHeight)))
                {
                    if (_opt_ypos < MapUp)
                    {
                        _opt_ypos = MapUp;
                    }
                    if (_opt_ypos > MapDown)
                    {
                        _opt_ypos = MapDown;
                    }
                }
            }
        }




        public Vector2 PosScreenToMap(Vector2 pos)
        {
            //화면의 좌표를 맵 좌표로 변형한다.
            //opt_xpos 좌표로 부터 pos.x*scale만큼 더 가면 된다.

            return new Vector2((float)(opt_xpos + pos.X / opt_scalepercent), (float)(opt_ypos + pos.Y / opt_scalepercent));
        }

        public Vector2 PosMapToScreen(Vector2 pos)
        {
            //맵의 좌표를 화면의 좌표로 반환한다.
            //현재 모서리 좌표와 맵의 좌표간의 사이의 거리를 구한다.
            //
            return new Vector2((float)((pos.X - opt_xpos) * opt_scalepercent), (float)((pos.Y - opt_ypos) * opt_scalepercent));
        }








        public DrawType opt_drawType;
        public enum DrawType
        {
            SD,
            HD,
            CB
        }

        private int _opt_scale = 100;
        public int opt_scale
        {
            get
            {
                return _opt_scale;
            }
            set
            {
                _opt_scale = value;

                int MinScale = 20;
                if (mapdata != null)
                {
                    int MaxMap = Math.Max(mapdata.WIDTH, mapdata.HEIGHT);

                    MinScale = (int)(128d / MaxMap * 20);
                }
                //MaxMap = 128일때 스케일 = 20
                //MaxMap = 64일때 스케일 = 40

                //128/MaxMap * 20
                if(MinScale == 0)
                {
                    MinScale = 20;
                }

                if (_opt_scale < MinScale)
                {
                    _opt_scale = MinScale;
                }


                if(_opt_scale > 800)
                {
                    _opt_scale = 800;
                }

                //_opt_scale = (int)(Math.Ceiling((_opt_scale / 2d)) * 2);


                if ((90 < _opt_scale) & (_opt_scale < 110))
                {
                    _opt_scale = 100;
                }
            }
        }
        public double opt_scalepercent
        {
            get
            {
                return opt_scale / 100d;
            }
        }


        public int opt_grid = 0;
        public bool opt_tile = true;
        public bool opt_unit = true;
        public bool opt_sprite = true;
        public bool opt_location = false;
        public bool opt_fogofwar = false;
        public bool opt_eudeditor = false;


        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            //Global.WindowTool.ChangeView(this, true);
        }





        public bool IsToolBarOpen()
        {
            return ToolBarExpander.IsExpanded;
        }


        public double GetToolBarWidth()
        {
            return ToolBarExpander.ActualWidth - 48;
        }







        public MainWindow mainWindow;



        public MapData mapdata;
        public Microsoft.Xna.Framework.Color[] minimapcolor;

        public bool IsLoad = false;
        public bool IsMinimapLoad = false;
        public bool ChangeMiniMap = false;
        public MapEditor()
        {
            InitializeComponent();

            mapdata = new MapData();
            minimapcolor = new Microsoft.Xna.Framework.Color[256 * 256];
            RefreshGRPIcon();
        }


        private void optionReset()
        {
            IsMinimapLoad = false;
            opt_scale = 10;
            ScaleTB.Text = opt_scale.ToString();

        }

        public bool NewMap()
        {
            //이름을 아무것도 안넣으면 새 맵
            bool Loadcmp = LoadMap("");
            IsDirty = false;
            IsLoad = true;
            
            return Loadcmp;
        }
        public bool OpenMap()
        {
            string mapname = UseMapEditor.Global.WindowTool.OpenMap();

            if (mapname != "")
            {
                bool result = LoadMap(mapname);
                IsDirty = result;

                return result;
            }
            IsLoad = false;
            return false;
        }



        public bool LoadMap(string _filepath)
        {
            try
            {
                bool LoadSucess = mapdata.LoadMap(_filepath);
                if (LoadSucess == true)
                {
                    optionReset();
                    //tabitem.Header = mapdata.SafeFileName;
                    //tabitem.Content = this;
                }
                IsLoad = LoadSucess;
                IsDirty = false;
                return LoadSucess;
            }
            catch (Exception e)
            {
                Dialog.MsgDialog msgDialog = new MsgDialog("열 수 없는 맵입니다.\n" + e.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                msgDialog.ShowDialog();
                return false;
            }

        }
        public bool SaveMap(string _filepath = "")
        {
            bool SaveSucess = mapdata.SaveMap(_filepath);

            if (SaveSucess)
            {
                IsDirty = false;
                //tabitem.Header = mapdata.SafeFileName;
            }

            return SaveSucess;
        }


        private bool isdirty;
        public bool IsDirty
        {
            get
            {
                return isdirty;
            }
            set
            {
                isdirty = value;

                mainWindow.SetWindowName();
            }

        }
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
                        IsLoad = true;
                        return false;
                    }
                }
                else if (result == MessageBoxResult.No)
                {
                    IsLoad = false;
                    Global.WindowTool.ChangeView(this, true);
                    return true;
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    IsLoad = true;
                    return false;
                }
            }


            Global.WindowTool.ChangeView(this,true);
            IsLoad = false;
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
            mainWindow.NewMapCommand();
        }

        private void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.OpenMapCommand();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.SaveMapCommand();
        }

        private void SaveAsBtn_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.SaveAsMapCommand();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.CloseMapCommand();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeView();
        }


        private void GridCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int v = 0;
            string str = (string)((ComboBoxItem)GridCB.SelectedItem).Tag;

            if (int.TryParse(str, out v))
            {
                opt_grid = v;
            }
        }
        private void grpTypeClick(object sender, RoutedEventArgs e)
        {
            NextgrpType();
        }


        public void NextgrpType()
        {
            opt_drawType += 1;
            if ((int)opt_drawType >= 3)
            {
                opt_drawType = 0;
            }
            RefreshGRPIcon();

        }
        public void SetGrpType(DrawType drawType)
        {
            opt_drawType = drawType;
            RefreshGRPIcon();
        }


        private void RefreshGRPIcon()
        {
            switch (opt_drawType)
            {
                case DrawType.SD:
                    grpImg.Source = new BitmapImage(new Uri("/Resources/SD.png", UriKind.RelativeOrAbsolute));
                    break;
                case DrawType.HD:
                    grpImg.Source = new BitmapImage(new Uri("/Resources/HD.png", UriKind.RelativeOrAbsolute));
                    break;
                case DrawType.CB:
                    grpImg.Source = new BitmapImage(new Uri("/Resources/CB.png", UriKind.RelativeOrAbsolute));
                    break;
            }
            IsMinimapLoad = false;
        }





        private void ScaleTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            int v;
            if(int.TryParse(ScaleTB.Text, out v))
            {
                opt_scale = v;
            }
        }
        public void ScaleUp(Vector2 MousePos)
        {
            Vector2 oldMouseMap = PosScreenToMap(MousePos);
            double LastScale = opt_scalepercent;

            opt_scale = (int)(opt_scale * 1.1);
            ScaleTB.Text = opt_scale.ToString();

            if(LastScale == opt_scalepercent)
            {
                return;
            }

            Vector2 newMouseMap = PosScreenToMap(MousePos);

            opt_xpos += (int)(oldMouseMap.X - newMouseMap.X);
            opt_ypos += (int)(oldMouseMap.Y - newMouseMap.Y);
        }
        public void ScaleDown(Vector2 MousePos)
        {
            Vector2 oldMouseMap = PosScreenToMap(MousePos);
            double LastScale = opt_scalepercent;

            opt_scale = (int)(opt_scale * 0.9);
            ScaleTB.Text = opt_scale.ToString();

            if (LastScale == opt_scalepercent)
            {
                return;
            }

            Vector2 newMouseMap = PosScreenToMap(MousePos);

            opt_xpos += (int)(oldMouseMap.X - newMouseMap.X);
            opt_ypos += (int)(oldMouseMap.Y - newMouseMap.Y);
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {

            if (IsToolBarOpen())
            {
                if (e.GetPosition(MapViewer).X < (MapViewer.ActualWidth - 256))
                {
                    if(MapViewer.Children.Count != 0)
                    {
                        MapDrawer mapDrawer = (MapDrawer)MapViewer.Children[0];
                        mapDrawer.IsEnabled = true;
                    }
                }
            }
            else
            {
                if (MapViewer.Children.Count != 0)
                {
                    MapDrawer mapDrawer = (MapDrawer)MapViewer.Children[0];
                    mapDrawer.IsEnabled = true;
                }
            }
        }

        private void TileButton_Click(object sender, RoutedEventArgs e)
        {
            TilePallet.Visibility = Visibility.Visible;
            DoodadPallet.Visibility = Visibility.Collapsed;
            UnitPallet.Visibility = Visibility.Collapsed;
            SpritePallet.Visibility = Visibility.Collapsed;
            LocationPallet.Visibility = Visibility.Collapsed;
            FogofWarPallet.Visibility = Visibility.Collapsed;
        }

        private void DoodadButton_Click(object sender, RoutedEventArgs e)
        {
            TilePallet.Visibility = Visibility.Collapsed;
            DoodadPallet.Visibility = Visibility.Visible;
            UnitPallet.Visibility = Visibility.Collapsed;
            SpritePallet.Visibility = Visibility.Collapsed;
            LocationPallet.Visibility = Visibility.Collapsed;
            FogofWarPallet.Visibility = Visibility.Collapsed;
        }

        private void UnitButton_Click(object sender, RoutedEventArgs e)
        {
            TilePallet.Visibility = Visibility.Collapsed;
            DoodadPallet.Visibility = Visibility.Collapsed;
            UnitPallet.Visibility = Visibility.Visible;
            SpritePallet.Visibility = Visibility.Collapsed;
            LocationPallet.Visibility = Visibility.Collapsed;
            FogofWarPallet.Visibility = Visibility.Collapsed;
        }

        private void SpriteButton_Click(object sender, RoutedEventArgs e)
        {
            TilePallet.Visibility = Visibility.Collapsed;
            DoodadPallet.Visibility = Visibility.Collapsed;
            UnitPallet.Visibility = Visibility.Collapsed;
            SpritePallet.Visibility = Visibility.Visible;
            LocationPallet.Visibility = Visibility.Collapsed;
            FogofWarPallet.Visibility = Visibility.Collapsed;
        }

        private void LocationButton_Click(object sender, RoutedEventArgs e)
        {
            TilePallet.Visibility = Visibility.Collapsed;
            DoodadPallet.Visibility = Visibility.Collapsed;
            UnitPallet.Visibility = Visibility.Collapsed;
            SpritePallet.Visibility = Visibility.Collapsed;
            LocationPallet.Visibility = Visibility.Visible;
            FogofWarPallet.Visibility = Visibility.Collapsed;
        }

        private void FogButton_Click(object sender, RoutedEventArgs e)
        {
            TilePallet.Visibility = Visibility.Collapsed;
            DoodadPallet.Visibility = Visibility.Collapsed;
            UnitPallet.Visibility = Visibility.Collapsed;
            SpritePallet.Visibility = Visibility.Collapsed;
            LocationPallet.Visibility = Visibility.Collapsed;
            FogofWarPallet.Visibility = Visibility.Visible;
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
