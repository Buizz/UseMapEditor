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
using UseMapEditor.FileData;
using UseMapEditor.Lua.TrigEditPlus;
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


        //public bool TilePalletePencil = true;
        //public bool TilePalleteRect = false;



        public TileSetPaintType tile_PaintType;
        public TileSetBrushMode tile_BrushMode;

        public enum TileSetPaintType
        {
            SELECTION,
            PENCIL,
            RECT,
            CIRCLE
        }

        public enum TileSetBrushMode
        {
            PASTE,
            ISOM,
            ALLTILE
        }


        public int SelectISOMIndex;
        //public int SelectRECTIndex;
        //public int SelectCUSTOMISOMIndex;


        public Vector2 tile_PalleteSelectStart
        {
            get
            {
                return new Vector2(Math.Min(tile_SelectPalleteALLTILEStartXIndex, tile_SelectPalleteALLTILEEndXIndex), Math.Min(tile_SelectPalleteALLTILEStartYIndex, tile_SelectPalleteALLTILEEndYIndex));
            }
        }
        public Vector2 tile_PalleteSelectEnd
        {
            get
            {
                return new Vector2(Math.Max(tile_SelectPalleteALLTILEStartXIndex, tile_SelectPalleteALLTILEEndXIndex), Math.Max(tile_SelectPalleteALLTILEStartYIndex, tile_SelectPalleteALLTILEEndYIndex));
            }
        }


        public Vector2 tile_CopyedTileSize
        {
            get
            {
                Vector2 rvec = new Vector2();


                foreach (var item in tile_CopyedTile)
                {
                    if (rvec.X < item.Key.X)
                    {
                        rvec.X = item.Key.X;
                    }
                    else if (rvec.Y < item.Key.Y)
                    {
                        rvec.Y = item.Key.Y;
                    }
                }
                rvec.X += 1;
                rvec.Y += 1;


                return rvec;
            }
        }
        public Dictionary<Vector2, ushort> tile_SelectedTile = new Dictionary<Vector2, ushort>();
        public Dictionary<Vector2, ushort> tile_CopyedTile = new Dictionary<Vector2, ushort>();
        public void Tile_ResetCopyedTile()
        {
            tile_CopyedTile.Clear();
        }
        public void Tile_SetCopyedTile(int x, int y, ushort mtxt)
        {
            Vector2 tmp = new Vector2(x, y);

            if (mtxt == 0) return;

            if (tile_CopyedTile.ContainsKey(tmp))
            {
                tile_CopyedTile[tmp] = mtxt;
            }
            else
            {
                tile_CopyedTile.Add(tmp, mtxt);
            }
            editorTextureData.TilePaletteRefresh();
        }

        public ushort Tile_GetCopyedTile(int x, int y)
        {
            Vector2 tmp = new Vector2(x, y);

            if (tile_CopyedTile.ContainsKey(tmp))
            {
                return tile_CopyedTile[tmp];
            }

            return ushort.MaxValue;
        }

        public void Tile_ResetSelectedTile()
        {
            tile_SelectedTile.Clear();
            editorTextureData.TilePaletteRefresh();
        }
        public void Tile_SetSelectedTile(int x, int y, ushort mtxt)
        {
            Vector2 tmp = new Vector2(x, y);

            if (tile_SelectedTile.ContainsKey(tmp))
            {
                tile_SelectedTile[tmp] = mtxt;
            }
            else
            {
                tile_SelectedTile.Add(tmp, mtxt);
            }
        }

        public ushort Tile_GetSelectedTile(int x, int y)
        {
            Vector2 tmp = new Vector2(x, y);

            if (tile_SelectedTile.ContainsKey(tmp))
            {
                return tile_SelectedTile[tmp];
            }

            return 0;
        }

        public void Tile_SetPalletFromMtxm(ushort mtxm, bool isscroll)
        {
            tile_SelectPalleteALLTILEStartXIndex = mtxm % 16;
            tile_SelectPalleteALLTILEStartYIndex = mtxm / 16;
            tile_SelectPalleteALLTILEEndXIndex = mtxm % 16;
            tile_SelectPalleteALLTILEEndYIndex = mtxm / 16;

            brush_tilescroll = (mtxm / 16) * 30;
        }




        public int tile_SelectPalleteALLTILEStartXIndex;
        public int tile_SelectPalleteALLTILEStartYIndex;
        public int tile_SelectPalleteALLTILEEndXIndex;
        public int tile_SelectPalleteALLTILEEndYIndex;


        public bool TilePalleteTransparentBlack = false;


        public void TilePalletSelectReset()
        {
            SelectISOMIndex = -1;
            //SelectRECTIndex = -1;
            //SelectCUSTOMISOMIndex = -1;
            tile_SelectPalleteALLTILEStartXIndex = -1;
            tile_SelectPalleteALLTILEStartYIndex = -1;
            tile_SelectPalleteALLTILEEndXIndex = -1;
            tile_SelectPalleteALLTILEEndYIndex = -1;
            editorTextureData.TilePaletteRefresh();
        }

        public bool TilePalleteISOMMouseDown;
        public bool TilePalleteAllMouseDown;


        public bool TilePalletMouseBonder = false;


        public int TilePalleteMouseStartXIndex;
        public int TilePalleteMouseStartYIndex;
        public int TilePalleteMouseStartY;

        public int TilePalleteMouseMoveXIndex;
        public int TilePalleteMouseMoveYIndex;

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
                if(tile_PaintType == TileSetPaintType.SELECTION)
                {
                    mapDataBinding.TILE_PAINTTYPE = TileSetPaintType.PENCIL;
                }
                tile_BrushMode = TileSetBrushMode.ALLTILE;


                if (!TilePalleteAllMouseDown)
                {
                    TilePalleteMouseStartXIndex = (int)(e.GetPosition(Tile_All_Pallet).X / viewTileSize);
                    TilePalleteMouseStartYIndex = (int)(e.GetPosition(Tile_All_Pallet).Y / viewTileSize + TileScroll.Value / 30);
                    TilePalleteMouseMoveXIndex = (int)(e.GetPosition(Tile_All_Pallet).X / viewTileSize);
                    TilePalleteMouseMoveYIndex = (int)(e.GetPosition(Tile_All_Pallet).Y / viewTileSize + TileScroll.Value / 30);

                    if ((int)e.GetPosition(Tile_All_Pallet).X >= (viewTileSize * 16 - 16))
                        return;

                    TilePalleteMouseStartY = (int)e.GetPosition(Tile_All_Pallet).Y;

                }
                TilePalleteAllMouseDown = true;
            }
        }

        public void Tile_All_Pallet_MouseUp()
        {
            if (TilePalleteAllMouseDown)
            {
                tile_SelectPalleteALLTILEStartXIndex = TilePalleteMouseStartXIndex;
                tile_SelectPalleteALLTILEStartYIndex = TilePalleteMouseStartYIndex;
                tile_SelectPalleteALLTILEEndXIndex = TilePalleteMouseMoveXIndex;
                tile_SelectPalleteALLTILEEndYIndex = TilePalleteMouseMoveYIndex;
            }
            //mapDataBinding.TILE_BRUSHMODE = true;
            //tile_PasteMode = false;

            editorTextureData.TilePaletteRefresh();
            TilePalleteAllMouseDown = false;
        }
        private void Tile_All_Pallet_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                Tile_All_Pallet_MouseUp();
            }
        }

        public void Tile_All_Pallet_MouseMove(int x, int y)
        {
            if (TilePalleteAllMouseDown)
            {
               //TileScroll.Value = _TileScrollStart + (TileMouseStartY - (int)e.GetPosition(TilePalletPanel).Y);
               TilePalleteMouseMoveXIndex = ((int)(x / viewTileSize));
                TilePalleteMouseMoveYIndex = ((int)(y / viewTileSize + TileScroll.Value / 30));
            }
        }
        private void Tile_All_Pallet_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Tile_All_Pallet_MouseMove((int)e.GetPosition(Tile_All_Pallet).X, (int)e.GetPosition(Tile_All_Pallet).Y);
        }





        private void Tile_ISOM_Pallet_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (tile_PaintType == TileSetPaintType.SELECTION)
                {
                    mapDataBinding.TILE_PAINTTYPE = TileSetPaintType.PENCIL;
                }
                tile_BrushMode = TileSetBrushMode.ISOM;
                TilePalleteISOMMouseDown = true;
            }
        }

        private void Tile_ISOM_Pallet_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                TilePalleteISOMMouseDown = false;
            }
        }

        private void Tile_ISOM_Pallet_PreviewMouseMove(object sender, MouseEventArgs e)
        {
        }



        public void OpenTileMenu(int x, int y)
        {
            if (tile_SelectedTile.Count == 0)
            {
                DoodadDeselectMenuItem.IsEnabled = false;
                DoodadCopyMenuItem.IsEnabled = false;
            }
            else
            {
                DoodadDeselectMenuItem.IsEnabled = true;
                DoodadCopyMenuItem.IsEnabled = true;
            }



            PopupGrid.Visibility = Visibility.Visible;
            TileContextMenu.Visibility = Visibility.Visible;
            PopupReLocatied();
            PopupInnerGrid.Margin = new Thickness(x, y, 0, 0);
            MapViewer.IsEnabled = false;
            KewBoardReset();
            PopupReLocatied();
        }

        public void CloseTileMenu()
        {
            PopupGrid.Visibility = Visibility.Collapsed;
            TileContextMenu.Visibility = Visibility.Collapsed;
            MapViewer.IsEnabled = true;
        }


        private void tileDeselect_Click(object sender, RoutedEventArgs e)
        {
            tile_Deselect();
        }

        private void tileCopy_Click(object sender, RoutedEventArgs e)
        {
            tile_Copy();
        }

        private void tilePaste_Click(object sender, RoutedEventArgs e)
        {
            tile_PasteStart();
        }


        public void tile_Deselect()
        {
            Tile_ResetSelectedTile();
            CloseTileMenu();
        }


        public void tile_Copy()
        {
            CloseTileMenu();
            if (tile_SelectedTile.Count == 0)
            {
                return;
            }

            List<Vector2> keys = tile_SelectedTile.Keys.ToList();

            for (int i = 0; i < tile_SelectedTile.Count; i++)
            {
                Vector2 key = keys[i];

                int tileindex = (int)(key.X + key.Y * mapdata.WIDTH);

                if (!mapdata.CheckTILERange((int)key.X, (int)key.Y)) continue;

                tile_SelectedTile[key] = mapdata.TILE[tileindex];
            }


            string jsonString = JsonConvert.SerializeObject(tile_SelectedTile);
            //Dictionary<Vector2, ushort> templist = JsonConvert.DeserializeObject<Dictionary<Vector2, ushort>>(jsonString);


            //jsonString = JsonConvert.SerializeObject(templist);
            try
            {
                Clipboard.SetDataObject(jsonString);
            }
            catch (Exception)
            {
                
            }
            
        }


        public void tile_PasteStart()
        {
            CloseTileMenu();
            Dictionary<Vector2, ushort> templist;
            try
            {
                string jsonString = Clipboard.GetText();

                templist = JsonConvert.DeserializeObject<Dictionary<Vector2, ushort>>(jsonString);
            }
            catch (Exception)
            {
                return;
            }

            if (templist == null)
            {
                return;
            }


            Vector2 min = new Vector2(256, 256);
            foreach (var item in templist)
            {
                if (min.X > item.Key.X)
                {
                    min.X = item.Key.X;
                }
                if (min.Y > item.Key.Y)
                {
                    min.Y = item.Key.Y;
                }
            }

            Tile_ResetSelectedTile();
            tile_CopyedTile.Clear();

            foreach (var item in templist)
            {
                tile_CopyedTile.Add(item.Key - min, item.Value);
            }


            //복사 팔레트 On
            tile_BrushMode = TileSetBrushMode.PASTE;
            if(tile_PaintType == TileSetPaintType.SELECTION)
            {
                mapDataBinding.TILE_PAINTTYPE= TileSetPaintType.PENCIL;
            }

            editorTextureData.TilePaletteRefresh();
        }


        public void tile_SetCopyedTileFromList(Dictionary<Vector2, ushort> copyedlist)
        {
            CloseTileMenu();
            Tile_ResetSelectedTile();
            tile_CopyedTile.Clear();

            foreach (var item in copyedlist.Keys)
            {
                tile_CopyedTile.Add(item, copyedlist[item]);
            }


            //복사 팔레트 On
            tile_BrushMode = TileSetBrushMode.PASTE;
            if (tile_PaintType == TileSetPaintType.SELECTION)
            {
                mapDataBinding.TILE_PAINTTYPE = TileSetPaintType.PENCIL;
            }
        }


    }
}
