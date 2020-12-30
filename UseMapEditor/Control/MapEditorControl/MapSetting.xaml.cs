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

namespace UseMapEditor.Control.MapEditorControl
{
    /// <summary>
    /// MapSetting.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MapSetting : UserControl
    {
        private MapEditor mapEditor;
        public void SetMapEditor(MapEditor _mapEditor)
        {
            mapEditor = _mapEditor;
        }



        public MapSetting()
        {
            InitializeComponent();
        }

        private void PopupBox_Opened(object sender, RoutedEventArgs e)
        {
            MapWidth.Text = mapEditor.mapdata.WIDTH.ToString();
            MapHeight.Text = mapEditor.mapdata.HEIGHT.ToString();
        }

        private void SizeChange_Click(object sender, RoutedEventArgs e)
        {
            int ChangeWidth;
            int ChangeHeight;
            int Width = mapEditor.mapdata.WIDTH;
            int Height = mapEditor.mapdata.HEIGHT;


            if (!int.TryParse(MapWidth.Text, out ChangeWidth))
            {
                return;
            }
            if (!int.TryParse(MapHeight.Text, out ChangeHeight))
            {
                return;
            }

            int index = 0;
            foreach (Button item in BtnPanel.Children)
            {
                if (!item.IsEnabled)
                {
                    break;
                }
                index++;
            }

            int xi = index % 3;
            int yi = index / 3;


            int xdiffer = ChangeWidth - Width;
            int ydiffer = ChangeHeight - Height;
            //이 만큼 맵의 모든 엔티티를 이동시킨다.


            switch (xi)
            {
                case 0:
                    //왼쪽으로 확장
                    break;
                case 1:
                    //양쪽으로 확장 
                    xdiffer /= 2;
                    break;
                case 2:
                    //오른쪽으로 확장 변화없음
                    xdiffer = 0;
                    break;
            }

            switch (yi)
            {
                case 0:
                    //왼쪽으로 확장
                    break;
                case 1:
                    //양쪽으로 확장 
                    ydiffer /= 2;
                    break;
                case 2:
                    //오른쪽으로 확장 변화없음
                    ydiffer = 0;
                    break;
            }


            ushort[] TILE = (ushort[])mapEditor.mapdata.TILE.Clone();

            ushort[] NEWTILE = new ushort[ChangeWidth * ChangeHeight];


            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    ushort tilevalue = TILE[x + y * Width];

                    int nx = x + xdiffer;
                    int ny = y + ydiffer;

                    if (0 <= nx & nx < ChangeWidth)
                    {
                        if (0 <= ny & ny < ChangeHeight)
                        {
                            NEWTILE[nx + ny * ChangeWidth] = tilevalue;
                        }
                    }
                }
            }

            mapEditor.mapdata.TILE = NEWTILE;
            mapEditor.mapdata.WIDTH = (ushort)ChangeWidth;
            mapEditor.mapdata.HEIGHT = (ushort)ChangeHeight;




            for (int i = 0; i < mapEditor.mapdata.LocationDatas.Count; i++)
            {
                int ysize = (int)(Math.Abs((int)mapEditor.mapdata.LocationDatas[i].B - (int)mapEditor.mapdata.LocationDatas[i].T) / 32);
                int xsize = (int)(Math.Abs((int)mapEditor.mapdata.LocationDatas[i].R - (int)mapEditor.mapdata.LocationDatas[i].L) / 32);

                if((xsize == Width) & (ysize == Height))
                {
                    mapEditor.mapdata.LocationDatas[i].T = 0;
                    mapEditor.mapdata.LocationDatas[i].L = 0;


                    mapEditor.mapdata.LocationDatas[i].B = (uint)(ChangeHeight * 32);
                    mapEditor.mapdata.LocationDatas[i].R = (uint)(ChangeWidth * 32);

                    break;
                }

                mapEditor.mapdata.LocationDatas[i].B += (uint)ydiffer * 32;
                mapEditor.mapdata.LocationDatas[i].T += (uint)ydiffer * 32;
                mapEditor.mapdata.LocationDatas[i].R += (uint)xdiffer * 32;
                mapEditor.mapdata.LocationDatas[i].L += (uint)xdiffer * 32;
            }

            for (int i = 0; i < mapEditor.mapdata.UNIT.Count; i++)
            {
                mapEditor.mapdata.UNIT[i].x += (ushort)(xdiffer * 32);
                mapEditor.mapdata.UNIT[i].y += (ushort)(ydiffer * 32);
            }

            for (int i = 0; i < mapEditor.mapdata.DD2.Count; i++)
            {
                mapEditor.mapdata.DD2[i].X += (ushort)(xdiffer * 32);
                mapEditor.mapdata.DD2[i].Y += (ushort)(ydiffer * 32);
            }

            for (int i = 0; i < mapEditor.mapdata.THG2.Count; i++)
            {
                mapEditor.mapdata.THG2[i].X += (ushort)(xdiffer * 32);
                mapEditor.mapdata.THG2[i].Y += (ushort)(ydiffer * 32);
            }








            mapEditor.mapDataBinding.PropertyChangeAll();
            mapEditor.IsMinimapLoad = false;


            SizeChangePopupBox.IsPopupOpen = false;
        }


        private void SizePosBtn_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            
            foreach (Button item in BtnPanel.Children)
            {
                item.IsEnabled = true;
            }

            button.IsEnabled = false;
        }
    }
}
