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
using System.Windows.Navigation;
using System.Windows.Shapes;
using UseMapEditor.FileData;

namespace UseMapEditor.Control
{
    /// <summary>
    /// MapCreate.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MapCreate : UserControl
    {
        public MainWindow mainWindow;
        public MapCreate()
        {
            InitializeComponent();
        }

        public void OpenInit()
        {
            TileTypeListbox.SelectedIndex = 0;
            StartTileListbox.SelectedIndex = 1;
        }

        private void Cancle_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.CancleNewMapCommand();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            int Width;
            int Height;

            int TileType = TileTypeListbox.SelectedIndex;

            int startTile = 0;

            if (!int.TryParse(MapWidth.Text, out Width))
            {
                return;
            }
            if (!int.TryParse(MapHeight.Text, out Height))
            {
                return;
            }
            if(TileType == -1)
            {
                return;
            }



            MapEditor mapeditor = new MapEditor(mainWindow);

            mapeditor.NewMap(Width, Height, TileType, (ISOMTile)((ListBoxItem)StartTileListbox.SelectedItem).Tag);
            mapeditor.InitControl();


            mainWindow.mapcreate.Visibility = Visibility.Collapsed;
            mainWindow.mapEditorGrid.Children.Add(mapeditor);
            mainWindow.startpage.Visibility = Visibility.Collapsed;
            mainWindow.WindowState = WindowState.Maximized;
            mainWindow.mapeditor = mapeditor;
            mainWindow.SetWindowName();
        }

        private void TileTypeListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            TileSet.TileType tiletype = (TileSet.TileType)TileTypeListbox.SelectedIndex;


            StartTileListbox.Items.Clear();
            {
                ListBoxItem listBoxItem = new ListBoxItem();
                listBoxItem.Content = "Null";
                listBoxItem.Tag = null;
                StartTileListbox.Items.Add(listBoxItem);
            }



            foreach (var item in Global.WindowTool.MapViewer.tileSet.ISOMdata[tiletype])
            {

                ListBoxItem listBoxItem = new ListBoxItem();
                listBoxItem.Content = item.name;
                listBoxItem.Tag = item;
                StartTileListbox.Items.Add(listBoxItem);
            }

            StartTileListbox.SelectedIndex = 1;
        }
    }
}
