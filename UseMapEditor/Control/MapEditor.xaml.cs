using Data.Map;
using Dragablz;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
using System.Windows.Threading;
using UseMapEditor.Control.MapEditorControl;
using UseMapEditor.Control.MapEditorData;
using UseMapEditor.DataBinding;
using UseMapEditor.Dialog;
using UseMapEditor.FileData;
using UseMapEditor.Global;
using UseMapEditor.MonoGameControl;
using UseMapEditor.Task;
using UseMapEditor.Tools;
using UseMapEditor.Windows;
using static Data.Map.MapData;
using static UseMapEditor.FileData.TileSet;

namespace UseMapEditor.Control
{
    /// <summary>
    /// MapEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MapEditor : UserControl
    {
        public MapDataBinding mapDataBinding;
        public TaskManager taskManager;
        public ShortCutManager shortCutManager;
        public MainWindow mainWindow;



        public int ForceSelectID = -1;
        public int ForceStartID = -1;
        public List<int> ForceSelectPlayer = new List<int>();



        public Layer PalleteLayer;
        public enum Layer
        {
            Tile,
            Doodad,
            Unit,
            Sprite,
            Location,
            FogOfWar,
            CustomPallete,
            CopyPaste
        }

        public bool view_Unit;
        public bool view_Unit_StartLoc;
        public bool view_Unit_Maprevealer;


        public bool view_Doodad;
        public bool view_DoodadColor;
        public Microsoft.Xna.Framework.Color DoodadOverlay;


        public bool view_Sprite;
        public bool view_SpriteColor;
        public Microsoft.Xna.Framework.Color SpriteOverlay;


        public bool view_Tile;
        public Microsoft.Xna.Framework.Color TileBack;


        public bool view_Location;


        public List<LocationData> SelectLocation = new List<LocationData>();


        public byte brush_x = 1;
        public byte brush_y = 1;

        public bool brush_fogofwarcircle;

        public int brush_tilescroll
        {
            get
            {
                return (int)TileScroll.Value;
            }
            set
            {
                TileScroll.Value = value;
            }
        }

        public bool brush_useRect
        {
            get
            {
                return (bool)TileISOMRectCheckBox.IsChecked;
            }
        }


        public int opt_FogofWarplayer;


        public double opt_palletSize
        {
            get
            {
                return PalletDock.Width;
            }
            set
            {
                PalletDock.Width = value;
            }
        }


        private int _opt_xpos = 0;
        private int _opt_ypos = 0;



        public int opt_xpos
        {
            //get
            //{
            //    return _opt_xpos + (int)(GetLeftToolBarWidth() / opt_scalepercent);
            //}
            //set
            //{
            //    _opt_xpos = value - (int)(GetLeftToolBarWidth() / opt_scalepercent);


            //    int MapLeft = -(int)((MapViewer.ActualWidth + GetLeftToolBarWidth()) / opt_scalepercent / 2);
            //    int MapRight = mapdata.WIDTH * 32 + MapLeft;

            //    Vector2 MapMin = PosMapToScreen(new Vector2(0, 0));
            //    Vector2 MapMax = PosMapToScreen(new Vector2(mapdata.WIDTH, mapdata.HEIGHT) * 32);
            //    Vector2 MapSize = MapMax - MapMin;
            //    if (!((MapSize.X < (MapViewer.ActualWidth - GetLeftToolBarWidth())) & (MapSize.Y < MapViewer.ActualHeight)))
            //    {
            //        if (_opt_xpos < MapLeft)
            //        {
            //            _opt_xpos = MapLeft;
            //        }
            //        if (_opt_xpos > MapRight)
            //        {
            //            _opt_xpos = MapRight;
            //        }
            //    }
            //}
            get
            {
                return _opt_xpos;
            }
            set
            {
                _opt_xpos = value;


                int MapLeft = -(int)((MapViewer.ActualWidth) / opt_scalepercent / 2);
                int MapRight = mapdata.WIDTH * 32 + MapLeft;

                Vector2 MapMin = PosMapToScreen(new Vector2(0, 0));
                Vector2 MapMax = PosMapToScreen(new Vector2(mapdata.WIDTH, mapdata.HEIGHT) * 32);
                Vector2 MapSize = MapMax - MapMin;
                if (!((MapSize.X < (MapViewer.ActualWidth)) & (MapSize.Y < MapViewer.ActualHeight)))
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
                if (!((MapSize.X < (MapViewer.ActualWidth - GetLeftToolBarWidth())) & (MapSize.Y < MapViewer.ActualHeight)))
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


        public bool opt_sysdraw = false;
        public void ToggleSysdraw()
        {
            opt_sysdraw = !opt_sysdraw;
        }



        int scrollspeed = 10;
        public void ScrollUp()
        {
            opt_ypos -= (int)Math.Ceiling(scrollspeed / opt_scalepercent);
        }
        public void ScrollDown()
        {
            opt_ypos += (int)Math.Ceiling(scrollspeed / opt_scalepercent);
        }
        public void ScrollLeft()
        {
            opt_xpos -= (int)Math.Ceiling(scrollspeed / opt_scalepercent);
        }
        public void ScrollRight()
        {
            opt_xpos += (int)Math.Ceiling(scrollspeed / opt_scalepercent);
        }

        public void Scroll(float x, float y)
        {
            opt_xpos += (int)Math.Ceiling(x / opt_scalepercent);
            opt_ypos += (int)Math.Ceiling(y / opt_scalepercent);
        }


        public Rect GetMapRect()
        {
            Vector2 mapend = PosScreenToMap(new Vector2((float)MapViewer.ActualWidth, (float)MapViewer.ActualHeight));
            Rect rect = new Rect(opt_xpos, opt_ypos, mapend.X, mapend.Y);

            return rect;
        }

        public Vector2 GetScreenSize()
        {
            float screenWidth = (float)(MapViewer.ActualWidth - GetRightToolBarWidth());
            float screenHeight = (float)MapViewer.ActualHeight;


            return new Vector2(screenWidth, screenHeight);
        }



        public Vector2 PosScreenToMap(Vector2 pos)
        {
            //화면의 좌표를 맵 좌표로 변형한다.
            //opt_xpos 좌표로 부터 pos.x*scale만큼 더 가면 된다.

            return new Vector2((float)(opt_xpos + pos.X / opt_scalepercent), (float)(opt_ypos + pos.Y / opt_scalepercent));
        }

        /// <summary>
        /// 맵의 좌표를 화면의 좌표로 반환한다.
        /// 현재 모서리 좌표와 맵의 좌표간의 사이의 거리를 구한다.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Vector2 PosMapToScreen(Vector2 pos, double scale = -1)
        {
            if(scale == -1)
            {
                scale = opt_scalepercent;
            }

            //맵의 좌표를 화면의 좌표로 반환한다.
            //현재 모서리 좌표와 맵의 좌표간의 사이의 거리를 구한다.
            //
            return new Vector2((float)((pos.X - opt_xpos) * scale), (float)((pos.Y - opt_ypos) * scale));
        }

        






        public DrawType opt_drawType;
        public enum DrawType
        {
            SD,
            HD,
            CB,
            NOTHING
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


        public int opt_grid
        {
            get
            {
                string d = Global.Setting.Vals[Global.Setting.Settings.Program_GridSize];
                int gsize;

                if(int.TryParse(d, out gsize))
                {
                    return gsize;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                Global.Setting.Vals[Global.Setting.Settings.Program_GridSize] = value.ToString();
            }
        }
        public bool opt_tile = true;
        public bool opt_unit = true;
        public bool opt_sprite = true;
        public bool opt_location = false;
        public bool opt_fogofwar = false;
        public bool opt_eudeditor = false;


        public bool key_WDown;
        public bool key_ADown;
        public bool key_SDown;
        public bool key_DDown;
        public void KewBoardReset()
        {
            key_WDown = false;
            key_ADown = false;
            key_SDown = false;
            key_DDown = false;
        }


        public void MouseCursorChange(Cursor cursors)
        {
            MapViewer.Cursor = cursors;
        }


        public void MosuePosChange()
        {
            ToolTipChange();
        }


        public void ToolTipChange()
        {
            var pos = Global.WindowTool.MapViewer.MouseMapPos;
            Vector2 mappos = pos;

            mappos.X /= 32;
            mappos.Y /= 32;

            pos.Ceiling();
            mappos.Ceiling();

            MouseToolTip.Content = pos.X + ", " + pos.Y + "(" + mappos.X + ", " + mappos.Y + ")";
            PaletteToolTip.Content = "";
            InformationToolTip.Content = "";
        }


        public bool IsRightToolBarOpen()
        {
            return RightExpander.IsExpanded;
        }

        public bool IsLeftToolBarOpen()
        {
            return LeftExpander.IsExpanded;
        }

        public bool IsBottomToolBarOpen()
        {
            return BottomExpander.IsExpanded;
        }

        public double GetRightToolBarWidth()
        {
            return RightExpander.ActualWidth - 48;
        }

        public double GetLeftToolBarWidth()
        {
            return Math.Floor( LeftExpander.ActualWidth);
        }
        public double GetBottomToolBarWidth()
        {
            return Math.Floor(BottomExpander.ActualWidth);
        }

        internal void DisableWindow()
        {
            DisEnablePanel.Visibility = Visibility.Visible;
            MainPanel.IsEnabled = false;
        }

        internal void EnableWindow()
        {
            mainWindow.Activate();
            DisEnablePanel.Visibility = Visibility.Collapsed;
            MainPanel.IsEnabled = true;
        }







        public MapData mapdata;
        public Microsoft.Xna.Framework.Color[] minimapcolor;
        public Microsoft.Xna.Framework.Color[] miniampUnit;

        public bool IsLoad = false;
        public bool IsMinimapLoad = false;
        public bool ChangeMiniMap = false;
        public bool IsMinimapUnitRefresh = false;

        public EditorTextureData editorTextureData;

       
        public void MinimapRefresh()
        {
            ChangeMiniMap = true;
        }
        public void MinimapUnitInitRefresh()
        {
            for (int y = 0; y < 256; y++)
            {
                for (int x = 0; x < 256; x++)
                {
                    miniampUnit[x + y * 256] = new Microsoft.Xna.Framework.Color();
                }
            }
            IsMinimapUnitRefresh = false;
        }
        public void miniTileUpdate(int x, int y)
        {
            int tileindex = x + y * mapdata.WIDTH;
            ushort MTXM = mapdata.TILE[tileindex];

            minimapcolor[x + y * 256] = Global.WindowTool.MapViewer.tileSet.GetTileColor(opt_drawType, mapdata.TILETYPE, MTXM);
            //mapeditor.miniampUnit[x + y * 256] = Color.Transparent;
        }
        public void miniUnitUpdate(CUNIT cUNIT, bool IsDelete = false)
        {
            int w = cUNIT.BoxWidth;
            int h = cUNIT.BoxHeight;

            for (int x = -w / 2; x < w / 2; x++)
            {
                for (int y = -h / 2; y < h / 2; y++)
                {
                    int mx = ((cUNIT.X + x) / 32);
                    int my = ((cUNIT.Y + y) / 32);


                    mx = Math.Max(0, mx);
                    my = Math.Max(0, my);

                    mx = Math.Min(255, mx);
                    my = Math.Min(255, my);

                    if (IsDelete)
                    {
                        miniampUnit[mx + my * 256] = new Microsoft.Xna.Framework.Color();
                    }
                    else
                    {
                        miniampUnit[mx + my * 256] = mapdata.UnitColor(cUNIT.player);

                    }
                }
            }
        }




        public void TileMapReDraw()
        {
            if (Global.Setting.Vals[Global.Setting.Settings.Program_GRPLoad] != "true") return;
  

            //if (!Global.WindowTool.IsEnabledMapEditor(this)) return;
            editorTextureData.tileMap.AllTileDraw(this);
            editorTextureData.tileMap.Apply();
        }
        public void TileUpdate(int x, int y, ushort MTXM)
        {
            //if (!Global.WindowTool.IsEnabledMapEditor(this)) return;
            editorTextureData.tileMap.SetTIleFromMTXM(mapdata.TILETYPE, x, y, MTXM);
        }
        public void TileMapRefresh()
        {
            if (!Global.WindowTool.IsEnabledMapEditor(this)) return;
            editorTextureData.tileMap.Apply();
        }



        public void SettingWindowOpen()
        {
            Scenario.AllWindowClose();
        }

        //설정한 스타일대로 변경
        public void StyleChange()
        {
            if (Global.Setting.Vals[Global.Setting.Settings.Program_FastExpander] == "true")
            {
                LeftExpander.Style = null;
                RightExpander.Style = null;
                BottomExpander.Style = null;
            }
            else
            {
                LeftExpander.Style = (Style)Application.Current.Resources["MaterialDesignExpander"];
                RightExpander.Style = (Style)Application.Current.Resources["MaterialDesignExpander"];
                BottomExpander.Style = (Style)Application.Current.Resources["MaterialDesignExpander"];
            }

     
        }


        private UIBinding uIBinding;
        public MapEditor(MainWindow mainWindow)
        {
            InitializeComponent();

            this.mainWindow = mainWindow;

            Scenario = new ScenarioControl();
            mapdata = new MapData(this);
            PalletSizeRefresh();




            taskManager = new TaskManager(this);
            shortCutManager = new ShortCutManager(this);
            
            ScenarioPanel.Child = Scenario;
            minimapcolor = new Microsoft.Xna.Framework.Color[256 * 256];
            miniampUnit = new Microsoft.Xna.Framework.Color[256 * 256];

            editorTextureData = new EditorTextureData();
            editorTextureData.Init(Global.WindowTool.MapViewer, this);
   


            TileBack = Microsoft.Xna.Framework.Color.Black;
            DoodadOverlay = new Microsoft.Xna.Framework.Color(255, 0, 0, 255);
            SpriteOverlay = new Microsoft.Xna.Framework.Color(0, 255, 0, 255);





            if (Global.Setting.Vals[Global.Setting.Settings.Program_GRPLoad] == "false")
            {
                TilePalleteBtn.IsEnabled = false;
                DoodadPalleteBtn.IsEnabled = false;
                UnitPalleteBtn.IsEnabled = false;
                SpritePalleteBtn.IsEnabled = false;

                TabChange(Layer.Location);
            }
            else
            {
                TabChange(Layer.Tile);
            }
        }

        public ScenarioControl Scenario;
        public void InitControl()
        {
            RefreshGRPIcon();

            mapDataBinding = new MapDataBinding(this);

            StyleChange();


            LocationList.ItemsSource = mapdata.LocationDatas;
            LocationList.Items.SortDescriptions.Add(new SortDescription("INDEX", ListSortDirection.Ascending));

            UnitPlaceList.ItemsSource = IndexedUnitList;
            UnitPlaceList.Items.SortDescriptions.Add(new SortDescription("INDEX", ListSortDirection.Ascending));


            Scenario.Init(this);

            refreshLocBox();
            uIBinding = new UIBinding(this);
            uIBinding.view_Tile = true;
            uIBinding.view_Unit = true;
            uIBinding.view_Unit_StartLoc = true;
            uIBinding.view_Unit_Maprevealer = true;
            uIBinding.view_Doodad = true;
            uIBinding.view_Sprite = true;
            Toolbar.DataContext = uIBinding;

            this.DataContext = mapDataBinding;

            UnitPallete.SetCodeType(Codetype.Unit, this);
            SpritePallete.SetCodeType(Codetype.Sprite, this);
            SpritePallete_Unit.SetCodeType(Codetype.Unit, this);

            UnitPallete.SelectionChanged += UnitPallete_SelectionChanged;
            SpritePallete.SelectionChanged += SpritePallete_SelectionChanged;
            SpritePallete_Unit.SelectionChanged += SpritePallete_SelectionChanged;

            UnitPallete.SelectIndex = 0;
            SpritePallete.SelectIndex = 0;
            SpritePallete_Unit.SelectIndex = 0;
            TileSetUIRefresh();

            TileMapReDraw();
            TileMapRefresh();
        }


        public void TileSetUIRefresh()
        {
            List<TileSet.DoodadPalletGroup> t = Global.WindowTool.MapViewer.tileSet.DoodadGroups[mapdata.TILETYPE];

            TilePalletSelectReset();

            DoodadTypes.Items.Clear();
            for (int i = 0; i < t.Count; i++)
            {
                DoodadTypes.Items.Add(t[i].groupname);
            }
        }



        private void optionReset()
        {
            IsMinimapLoad = false;

            opt_scale = 10;
            ScaleTB.Text = opt_scale.ToString();
            foreach (ComboBoxItem item in GridCB.Items)
            {
                if ((string)item.Tag == opt_grid.ToString())
                {
                    item.IsSelected = true;
                    break;
                }
            }
        }

        private void PopupGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //PopupReLocatied();
        }
        private void PopupReLocatied()
        {
            PopupInnerGrid.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
            PopupInnerGrid.Arrange(new Rect(0, 0, PopupInnerGrid.DesiredSize.Width, PopupInnerGrid.DesiredSize.Height));




            //PopupInnerGrid
            double x = PopupInnerGrid.Margin.Left + PopupInnerGrid.ActualWidth;
            double y = PopupInnerGrid.Margin.Top + PopupInnerGrid.ActualHeight;

            double nx = PopupInnerGrid.Margin.Left;
            double ny = PopupInnerGrid.Margin.Top;

            if (x > (MapViewer.ActualWidth - RightExpander.ActualWidth))
            {
                nx -= PopupInnerGrid.ActualWidth;
            }

            if (y > MapViewer.ActualHeight)
            {
                ny -= PopupInnerGrid.ActualHeight;
            }

            nx = Math.Max(0, nx);
            ny = Math.Max(0, ny);


            PopupInnerGrid.Margin = new Thickness(nx,ny,0,0);
        }

        private void PopupVisbleManage(MouseButtonEventArgs e)
        {
            int visblecount = 0;

            foreach (ContentControl item in PopupInnerGrid.Children)
            {
                if (item.Visibility == Visibility.Visible)
                {
                    visblecount += 1;
                    System.Windows.Point p = e.GetPosition(item);
                    if ((p.X < 0) | (p.Y < 0) | (p.X > item.ActualWidth) | (p.Y > item.ActualHeight))
                    {
                        item.Visibility = Visibility.Collapsed;
                        visblecount -= 1;
                    }
                }
            }
            if (visblecount == 0 & PopupGrid.Visibility == Visibility.Visible)
            {
                SetDirty();

                PopupGrid.Visibility = Visibility.Collapsed;
                if (!MapViewer.IsEnabled)
                {
                    MapViewer.IsEnabled = true;
                }
                taskManager.TaskEnd();
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PopupVisbleManage(e);
        }


        public bool IsRightMouseDown;
        public bool IsLeftMouseDown;
        private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            PopupVisbleManage(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                IsLeftMouseDown = true;
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                IsRightMouseDown = true;
            }
        }
        private void UserControl_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                IsLeftMouseDown = false;
            }
            if (e.RightButton == MouseButtonState.Released)
            {
                IsRightMouseDown = false;
            }
            DoodadListBoxMouseDown = false;

            //MapViewer.IsEnabled = true;
            //Global.WindowTool.MapViewer.Focus();
        }


        public bool NewMap(int Width, int Height, int TileType, int startTile)
        {
            //이름을 아무것도 안넣으면 새 맵
            bool Loadcmp = mapdata.NewMap(Width, Height, TileType, startTile);
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
                IsLoad = LoadSucess;
                IsDirty = false;
                if (LoadSucess == true)
                {
                    optionReset();
                    //tabitem.Header = mapdata.SafeFileName;
                    //tabitem.Content = this;
                    InitControl();
                    mapDataBinding.PropertyChangeAll();
                }

                return LoadSucess;
            }
            catch (Exception e)
            {
                Dialog.MsgDialog msgDialog = new MsgDialog(System.IO.Path.GetFileName(_filepath) + "는 열 수 없는 맵입니다.\n" + e.Message, MessageBoxButton.OK, MessageBoxImage.Error);
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
        public void SetDirty()
        {
            IsDirty = true;
        }
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

            LocationList.ItemsSource = null;
            UnitPlaceList.ItemsSource = null;
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

        private void Scen_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.ScenOpenCommand();
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

        public void GridSizeChange(int index)
        {
            GridCB.SelectedIndex = index;
        }

        public void GridSizeUp()
        {
            if(GridCB.SelectedIndex < GridCB.Items.Count - 1)
            {
                GridCB.SelectedIndex += 1;
            }
        }
        public void GridSizeDown()
        {
            if (GridCB.SelectedIndex > 0)
            {
                GridCB.SelectedIndex -= 1;
            }
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
            if (IsRightToolBarOpen())
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


        public Vector2 GetOuterMouse()
        {
            return OuterMouse;
        }


        bool IsLayerChange;
        public void TabChange(Layer PalleteLayer, bool Isonoff = true)
        {
            IsLayerChange = true;

            if (Isonoff)
            {
                if (RightExpander.IsExpanded)
                {
                    //열려있을 경우
                    if (this.PalleteLayer == PalleteLayer)
                    {
                        RightExpander.IsExpanded = false;
                    }
                }
                else
                {
                    //닫혀있을 경우
                    RightExpander.IsExpanded = true;
                }
            }


            this.PalleteLayer = PalleteLayer;
            TilePallet.Visibility = Visibility.Collapsed;
            DoodadPallet.Visibility = Visibility.Collapsed;
            UnitPallet.Visibility = Visibility.Collapsed;
            SpritePallet.Visibility = Visibility.Collapsed;
            LocationPallet.Visibility = Visibility.Collapsed;
            FogofWarPallet.Visibility = Visibility.Collapsed;
            CutPastePallet.Visibility = Visibility.Collapsed;

            switch (PalleteLayer)
            {
                case Layer.Tile:
                    TilePallet.Visibility = Visibility.Visible;
                    break;
                case Layer.Doodad:
                    DoodadPallet.Visibility = Visibility.Visible;
                    break;
                case Layer.Unit:
                    UnitPallet.Visibility = Visibility.Visible;
                    break;
                case Layer.Sprite:
                    SpritePallet.Visibility = Visibility.Visible;
                    break;
                case Layer.Location:
                    LocationPallet.Visibility = Visibility.Visible;
                    break;
                case Layer.FogOfWar:
                    FogofWarPallet.Visibility = Visibility.Visible;
                    break;
                case Layer.CopyPaste:
                    CutPastePallet.Visibility = Visibility.Collapsed;
                    break;
            }
            LayerCB.SelectedIndex = (int)PalleteLayer;


            IsLayerChange = false;
        }

        private void LayerCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLayerChange)
            {
                if (LayerCB.SelectedIndex != -1)
                {
                    TabChange((Layer)LayerCB.SelectedIndex, false);
                }
            }
        }
        private void TileButton_Click(object sender, RoutedEventArgs e)
        {
            TabChange(Layer.Tile, false);
        }

        private void DoodadButton_Click(object sender, RoutedEventArgs e)
        {
            TabChange(Layer.Doodad, false);
        }

        private void UnitButton_Click(object sender, RoutedEventArgs e)
        {
            TabChange(Layer.Unit, false);
        }

        private void SpriteButton_Click(object sender, RoutedEventArgs e)
        {
            TabChange(Layer.Sprite, false);
        }

        private void LocationButton_Click(object sender, RoutedEventArgs e)
        {
            TabChange(Layer.Location, false);
        }

        private void FogButton_Click(object sender, RoutedEventArgs e)
        {
            TabChange(Layer.FogOfWar, false);
        }

        private void CutPasteButton_Click(object sender, RoutedEventArgs e)
        {
            TabChange(Layer.CopyPaste, false);
        }





        private void UndoBtn_Click(object sender, RoutedEventArgs e)
        {
            taskManager.Undo();
        }

        private void RedoBtn_Click(object sender, RoutedEventArgs e)
        {
            taskManager.Redo();
        }



        private void FogUseCircle(object sender, RoutedEventArgs e)
        {
            brush_fogofwarcircle = true;
        }

        private void FogUseRect(object sender, RoutedEventArgs e)
        {
            brush_fogofwarcircle = false;
        }




        public void PasteCommand()
        {
            switch (PalleteLayer)
            {
                case Layer.Tile:
                    tile_PasteStart();
                    break;
                case Layer.Unit:
                    unit_PasteStart();
                    break;
                case Layer.Sprite:
                    sprite_PasteStart();
                    break;
                case Layer.Doodad:
                    doodad_PasteStart();
                    break;
            }
        }
        public void CopyCommand()
        {
            switch (PalleteLayer)
            {
                case Layer.Tile:
                    tile_Copy();
                    break;
                case Layer.Unit:
                    unit_Copy();
                    break;
                case Layer.Sprite:
                    sprite_Copy();
                    break;
                case Layer.Doodad:
                    doodad_Copy();
                    break;
            }
        }
        public void CutCommand()
        {
            switch (PalleteLayer)
            {
                case Layer.Unit:
                    unit_Cut();
                    break;
                case Layer.Sprite:
                    sprite_Cut();
                    break;
                case Layer.Doodad:
                    doodad_Cut();
                    break;
            }
        }
        public void DeleteCommand()
        {
            switch (PalleteLayer)
            {
                case Layer.Unit:
                    unit_Delete();
                    break;
                case Layer.Sprite:
                    sprite_Delete();
                    break;
                case Layer.Doodad:
                    doodad_Delete();
                    break;
            }
        }
        public void EditCommand()
        {
            switch (PalleteLayer)
            {
                case Layer.Unit:
                    unit_Edit();
                    break;
                case Layer.Sprite:
                    sprite_Edit();
                    break;
            }
        }

        private Vector2 OuterMouse = new Vector2();
        private void UserControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            OuterMouse.X = (float)e.GetPosition(MapViewer).X;
            OuterMouse.Y = (float)e.GetPosition(MapViewer).Y;
        }

        private void MenuScenBtn_Click(object sender, RoutedEventArgs e)
        {
            MenuItem button = (MenuItem)sender;
            string tag = (string)button.Tag;


            ScenOpenCommand(tag);
        }


        public void ScenOpenCommand(string tag)
        {
            string[] scennames = {"mapSetting","playerSetting","forceSetting",
                "unitSetting","upgradeSetting","techSetting","soundSetting",
                "stringSetting","classTriggerEditor","brinfingTriggerEditor" };

            int index = scennames.ToList().IndexOf(tag);

            if (index == -1)
                return;

            //열리지 않았을 경우 열게하기
            if (!LeftExpander.IsExpanded)
            {
                LeftExpander.IsExpanded = true;
            }
            else
            {
                if(Scenario.SelectedIndex == index)
                {
                    //열린 상태에선 윈도우 열기
                    Scenario.OpenPopupWindow(index);

                    LeftExpander.IsExpanded = false;
                    return;
                }
            }
            Scenario.SwitchTab(index);
        }

        private void PalletSizeChange_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            PalletSizeBorder.Visibility = Visibility.Visible;
        }

        private void PalletSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(e.NewValue < 302)
            {
                PalletSize.Value = 302;
            }

            PalletSizeRefresh();
        }
        private void PalletSizeRefresh()
        {
            List<ISOMTIle> iSOMs = Global.WindowTool.MapViewer.tileSet.GetISOMData(this);

            int columns = iSOMs.Count / 8 + 1;


            Tile_ISOM_Pallet.Height = columns * TileSize;
            //Tile_Rect_Pallet.Height = columns * TileSize;
            //Tile_Custom_Pallet.Height = columns * TileSize;
        }

        private void PalletSize_LostMouseCapture(object sender, MouseEventArgs e)
        {
            PalletSizeBorder.Visibility = Visibility.Collapsed;
        }

        private void Tile_Pallet_Expanded(object sender, RoutedEventArgs e)
        {
            PalletSizeRefresh();
        }

        private void TilePencil_Click(object sender, RoutedEventArgs e)
        {
            mapDataBinding.TILE_PAINTTYPE = TileSetPaintType.PENCIL;
        }

        private void TileCircle_Click(object sender, RoutedEventArgs e)
        {
            mapDataBinding.TILE_PAINTTYPE = TileSetPaintType.CIRCLE;
        }
        private void TileSquare_Click(object sender, RoutedEventArgs e)
        {
            mapDataBinding.TILE_PAINTTYPE = TileSetPaintType.RECT;
        }

        private void TileHand_Click(object sender, RoutedEventArgs e)
        {
            mapDataBinding.TILE_PAINTTYPE = TileSetPaintType.SELECTION;
        }

        private void TilePaste_Click(object sender, RoutedEventArgs e)
        {
            tile_BrushMode = TileSetBrushMode.PASTE;
        }

        private void tileMinimap_Click(object sender, RoutedEventArgs e)
        {
            CloseTileMenu();
            Windows.MinimapImageWindow minimapImageWindow = new MinimapImageWindow(this);
            minimapImageWindow.ShowDialog();
            editorTextureData.TilePaletteRefresh();
            //tile_PasteStart();
        }

        private void EmptyButton_Click(object sender, RoutedEventArgs e)
        {
            EditCommand();
        }

    }
}
