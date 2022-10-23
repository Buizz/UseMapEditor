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
            ISOM,
            RECT,
            CUSTOMISOM,
            ALLTILE
        }

        public int SelectISOMIndex;
        public int SelectRECTIndex;
        public int SelectCUSTOMISOMIndex;
        public List<int> SelectALLTILEIndex = new List<int>();



        public void TilePalletSelectReset()
        {
            SelectISOMIndex = -1;
            SelectRECTIndex = -1;
            SelectCUSTOMISOMIndex = -1;
            SelectALLTILEIndex.Clear();
        }

        public bool TileISOMMouseDown;
        public bool TileRECTMouseDown;
        public bool TileCUSTOMISOMMouseDown;
        public bool TileAllMouseDown;


        public TileSetBrushMode tile_BrushMode;


        public int TileMouseStartXIndex;
        public int TileMouseStartYIndex;
        public int TileMouseStartY;


        
        private void Tile_All_Pallet_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                tile_BrushMode = TileSetBrushMode.ALLTILE;

                if (!TileAllMouseDown)
                {
                    TileMouseStartXIndex = (int)e.GetPosition(Tile_All_Pallet).X / 25;
                    TileMouseStartYIndex = (int)e.GetPosition(Tile_All_Pallet).Y / 25 + (int)TileScroll.Value / 30;


                    TileMouseStartY = (int)e.GetPosition(Tile_All_Pallet).Y;

                }
                TileAllMouseDown = true;
            }
        }
        private void Tile_All_Pallet_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
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
            }
        }



        private void Tile_Custom_Pallet_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                tile_BrushMode = TileSetBrushMode.CUSTOMISOM;
                TileCUSTOMISOMMouseDown = true;
            }
        }
        private void Tile_Custom_Pallet_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                TileCUSTOMISOMMouseDown = false;
            }
        }
        private void Tile_Custom_Pallet_PreviewMouseMove(object sender, MouseEventArgs e)
        {
        }



        private void Tile_Rect_Pallet_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                tile_BrushMode = TileSetBrushMode.RECT;
                TileRECTMouseDown = true;
            }
        }
        private void Tile_Rect_Pallet_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                TileRECTMouseDown = false;
            }
        }
        private void Tile_Rect_Pallet_PreviewMouseMove(object sender, MouseEventArgs e)
        {
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
    }
}
