using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UseMapEditor.Task;
using UseMapEditor.Task.Events;
using static Data.Map.MapData;

namespace UseMapEditor.Control
{
    public partial class MapEditor : UserControl
    {
        private void Tile_All_Pallet_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            TileScroll.Value -= e.Delta;
        }



        public float TileSize
        {
            get
            {
                return (float)(opt_palletSize / 16f);
            }
        }



        public enum TileSetBrushMode
        {
            SELECTION,
            PASTE,
            ISOM,
            //RECT,
            //CUSTOMISOM,
            ALLTILE
        }

        public int SelectISOMIndex;
        //public int SelectRECTIndex;
        //public int SelectCUSTOMISOMIndex;


        public Vector2 tile_PalleteSelectStart
        {
            get
            {
                return new Vector2(Math.Min(tile_SelectALLTILEStartXIndex, tile_SelectALLTILEEndXIndex), Math.Min(tile_SelectALLTILEStartYIndex, tile_SelectALLTILEEndYIndex));
            }
        }
        public Vector2 tile_PalleteSelectEnd
        {
            get
            {
                return new Vector2(Math.Max(tile_SelectALLTILEStartXIndex, tile_SelectALLTILEEndXIndex), Math.Max(tile_SelectALLTILEStartYIndex, tile_SelectALLTILEEndYIndex));
            }
        }


        public int tile_SelectALLTILEStartXIndex;
        public int tile_SelectALLTILEStartYIndex;
        public int tile_SelectALLTILEEndXIndex;
        public int tile_SelectALLTILEEndYIndex;


        public bool TilePalletePencil = true;
        public bool TilePalleteRect = false;
        public bool TilePalleteTransparentBlack = false;


        public void TilePalletSelectReset()
        {
            SelectISOMIndex = -1;
            //SelectRECTIndex = -1;
            //SelectCUSTOMISOMIndex = -1;
            tile_SelectALLTILEStartXIndex = -1;
            tile_SelectALLTILEStartYIndex = -1;
            tile_SelectALLTILEEndXIndex = -1;
            tile_SelectALLTILEEndYIndex = -1;
        }

        public bool TileISOMMouseDown;
        public bool TileRECTMouseDown;
        public bool TileCUSTOMISOMMouseDown;
        public bool TileAllMouseDown;


        public TileSetBrushMode tile_BrushMode;


        public int TileMouseStartXIndex;
        public int TileMouseStartYIndex;
        public int TileMouseStartY;

        public int TileMouseMoveXIndex;
        public int TileMouseMoveYIndex;

        public float viewTileSize
        {
            get
            {
                return (float)(opt_palletSize) / 16;
            }
        }
        


        private void Tile_All_Pallet_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                tile_BrushMode = TileSetBrushMode.ALLTILE;


                if (!TileAllMouseDown)
                {
                    TileMouseStartXIndex = (int)(e.GetPosition(Tile_All_Pallet).X / viewTileSize);
                    TileMouseStartYIndex = (int)(e.GetPosition(Tile_All_Pallet).Y / viewTileSize + TileScroll.Value / 30);
                    TileMouseMoveXIndex = (int)(e.GetPosition(Tile_All_Pallet).X / viewTileSize);
                    TileMouseMoveYIndex = (int)(e.GetPosition(Tile_All_Pallet).Y / viewTileSize + TileScroll.Value / 30);

                    if ((int)e.GetPosition(Tile_All_Pallet).X >= (viewTileSize * 16 - 16))
                        return;

                    TileMouseStartY = (int)e.GetPosition(Tile_All_Pallet).Y;

                }
                TileAllMouseDown = true;
            }
        }
        private void Tile_All_Pallet_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                if (TileAllMouseDown)
                {
                    tile_SelectALLTILEStartXIndex = TileMouseStartXIndex;
                    tile_SelectALLTILEStartYIndex = TileMouseStartYIndex;
                    tile_SelectALLTILEEndXIndex = TileMouseMoveXIndex;
                    tile_SelectALLTILEEndYIndex = TileMouseMoveYIndex;
                }
                //mapDataBinding.TILE_BRUSHMODE = true;
                //tile_PasteMode = false;

                TileAllMouseDown = false;
            }
        }
        private void Tile_All_Pallet_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (TileAllMouseDown)
            {
                //TileScroll.Value = _TileScrollStart + (TileMouseStartY - (int)e.GetPosition(TilePalletPanel).Y);

                TileMouseMoveXIndex = (int)(e.GetPosition(Tile_All_Pallet).X / viewTileSize);
                TileMouseMoveYIndex = (int)(e.GetPosition(Tile_All_Pallet).Y / viewTileSize + TileScroll.Value / 30);
            }
        }


        private void Tile_ISOM_Pallet_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                tile_BrushMode = TileSetBrushMode.ISOM;
                TileISOMMouseDown = true;
            }
        }
        private void Tile_ISOM_Pallet_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                TileISOMMouseDown = false;
            }
        }
        private void Tile_ISOM_Pallet_PreviewMouseMove(object sender, MouseEventArgs e)
        {
        }





        public void OpenTileMenu(int x, int y)
        {
            if (SelectSprite.Count == 0)
            {
                SpriteEditMenuItem.IsEnabled = false;
                SpriteDeselectMenuItem.IsEnabled = false;
                SpriteCutMenuItem.IsEnabled = false;
                SpriteCopyMenuItem.IsEnabled = false;
                SpriteDeleteMenuItem.IsEnabled = false;
            }
            else
            {
                SpriteEditMenuItem.IsEnabled = true;
                SpriteDeselectMenuItem.IsEnabled = true;
                SpriteCutMenuItem.IsEnabled = true;
                SpriteCopyMenuItem.IsEnabled = true;
                SpriteDeleteMenuItem.IsEnabled = true;
            }



            PopupGrid.Visibility = Visibility.Visible;
            SpriteContextMenu.Visibility = Visibility.Visible;
            PopupReLocatied();
            PopupInnerGrid.Margin = new Thickness(x, y, 0, 0);
            MapViewer.IsEnabled = false;
            PopupReLocatied();
        }

        public void CloseTileMenu()
        {
            PopupGrid.Visibility = Visibility.Collapsed;
            SpriteContextMenu.Visibility = Visibility.Collapsed;
            MapViewer.IsEnabled = true;
        }

        private void tileCopy_Click(object sender, RoutedEventArgs e)
        {
            sprite_Copy();
        }

        private void tilePaste_Click(object sender, RoutedEventArgs e)
        {
            sprite_PasteStart();
        }


        public void tile_Copy()
        {
            CloseTileMenu();
            if (SelectSprite.Count == 0)
            {
                return;
            }

            string jsonString = JsonConvert.SerializeObject(SelectSprite);
            List<CTHG2> templist = JsonConvert.DeserializeObject<List<CTHG2>>(jsonString);



            Vector2 min = new Vector2(ushort.MaxValue);
            Vector2 max = new Vector2(0);
            for (int i = 0; i < templist.Count; i++)
            {
                if (min.X > templist[i].X)
                {
                    min.X = templist[i].X;
                }
                if (min.Y > templist[i].Y)
                {
                    min.Y = templist[i].Y;
                }

                if (max.X < templist[i].X)
                {
                    max.X = templist[i].X;
                }
                if (max.Y < templist[i].Y)
                {
                    max.Y = templist[i].Y;
                }
            }

            Vector2 center = (min + max) / 2;

            if (SpritePalleteCopyTileFix)
            {
                center.X = (float)(Math.Floor(center.X / 32) * 32);
                center.Y = (float)(Math.Floor(center.Y / 32) * 32);
            }

            for (int i = 0; i < templist.Count; i++)
            {
                templist[i].X -= (ushort)center.X;
                templist[i].Y -= (ushort)center.Y;
            }




            jsonString = JsonConvert.SerializeObject(templist);

            Clipboard.SetText(jsonString);

        }


        public void tile_PasteStart()
        {
            CloseTileMenu();
            List<CTHG2> templist;
            try
            {
                string jsonString = Clipboard.GetText();

                templist = JsonConvert.DeserializeObject<List<CTHG2>>(jsonString);
            }
            catch (Exception)
            {
                return;
            }

            if (templist == null)
            {
                return;
            }

            CopyedSprite.Clear();
            CopyedSprite.AddRange(templist);

            //복사 팔레트 On
            tile_BrushMode = TileSetBrushMode.PASTE;
        }
    }
}
