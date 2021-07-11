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
        private void TileSetListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            TileScroll.Value -= e.Delta;
        }


        public bool tile_PasteMode = false;


        public bool tile_BrushMode = false;
        public bool tile_SelectMode = true;



        public bool TileListBoxMouseDown;
        private int _TileScrollStart;
        private int TileMouseStartY;

        private void TileSetListBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.RightButton == MouseButtonState.Pressed)
            {
                if (!TileListBoxMouseDown)
                {
                    TileMouseStartY = (int)e.GetPosition(TilePalletPanel).Y;
                    _TileScrollStart = (int)TileScroll.Value;
                }
                TileListBoxMouseDown = true;
            }
       
        }


        private void TileSetListBox_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Released)
            {
                mapDataBinding.TILE_BRUSHMODE = true;
                tile_PasteMode = false;

                TileListBoxMouseDown = false;
            }

  
        }

        private void TileSetListBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (TileListBoxMouseDown)
            {
                //TileScroll.Value = _TileScrollStart + (TileMouseStartY - (int)e.GetPosition(TilePalletPanel).Y);
            }

           
        }
    }
}
