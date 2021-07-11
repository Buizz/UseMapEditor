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
        public List<CDD2> SelectDoodad = new List<CDD2>();
        public List<CDD2> CopyedDoodad = new List<CDD2>();


        public int doodad_group = 0;
        public int doodad_index = 0;

        public bool doodad_PasteMode = false;


        public bool doodad_BrushMode = false;
        public bool doodad_SelectMode = true;

        public bool DoodadPalleteStackAllow;
        public bool DoodadPalleteToTile;
        public bool DoodadPalleteSizeUp = true;

        public int DoodadScroll;

        private void DoodadTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(DoodadTypes.SelectedIndex == -1)
            {
                DoodadTypes.SelectedIndex = 0;
            }
            doodad_group = DoodadTypes.SelectedIndex;
            DoodadScroll = 0;
        }


        private int _DoodadScrollStart;
        private int DoodadDragStartY;
        public bool DoodadListBoxMouseDown;
        private void DoodadListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            int xitemcount = 6;
            int yitemcount = 6;
            int itemsize = 64;

            if (DoodadPalleteSizeUp)
            {
                xitemcount /= 2;
                itemsize *= 2;
            }


            ushort doodadgroup = (ushort)doodad_group;

            List<int> tlist = Global.WindowTool.MapViewer.tileSet.DoodadGroups[mapdata.TILETYPE][doodadgroup].dddids;

            yitemcount = (tlist.Count / xitemcount) + 1;

            int YLimit = (int)(yitemcount * itemsize - DoodadListBox.ActualHeight);


            DoodadScroll -= e.Delta;
            if (DoodadScroll < 0)
            {
                DoodadScroll = 0;
            }
            if (yitemcount * itemsize < DoodadListBox.ActualHeight)
            {
                DoodadScroll = 0;
            }
            else
            {
                if (DoodadScroll > YLimit)
                {
                    DoodadScroll = YLimit;
                }
            }
        }

        private void DoodadListBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!DoodadListBoxMouseDown)
            {
                DoodadDragStartY = (int)e.GetPosition(DoodadListBox).Y;
                _DoodadScrollStart = DoodadScroll;
            }
            DoodadListBoxMouseDown = true;
        }


        private void DoodadListBox_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            mapDataBinding.DOODAD_BRUSHMODE = true;
            doodad_PasteMode = false;

            DoodadListBoxMouseDown = false;
        }

        private void DoodadListBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (DoodadListBoxMouseDown)
            {
                DoodadScroll = _DoodadScrollStart + (int)(DoodadDragStartY - e.GetPosition(DoodadListBox).Y);
            }
            if (DoodadScroll < 0)
            {
                DoodadScroll = 0;
            }


            int xitemcount = 6;
            int yitemcount = 6;
            int itemsize = 64;

            if (DoodadPalleteSizeUp)
            {
                xitemcount /= 2;
                itemsize *= 2;
            }


            ushort doodadgroup = (ushort)doodad_group;

            List<int> tlist = Global.WindowTool.MapViewer.tileSet.DoodadGroups[mapdata.TILETYPE][doodadgroup].dddids;

            yitemcount = (tlist.Count / xitemcount) + 1;

            int YLimit = (int)(yitemcount * itemsize - DoodadListBox.ActualHeight);
            if(yitemcount * itemsize < DoodadListBox.ActualHeight)
            {
                DoodadScroll = 0;
            }
            else
            {
                if (DoodadScroll > YLimit)
                {
                    DoodadScroll = YLimit;
                }
            }
        }




        public void OpenDoodadMenu(int x, int y)
        {
            if (SelectDoodad.Count == 0)
            {
                DoodadDeselectMenuItem.IsEnabled = false;
                DoodadCutMenuItem.IsEnabled = false;
                DoodadCopyMenuItem.IsEnabled = false;
                DoodadDeleteMenuItem.IsEnabled = false;
            }
            else
            {
                DoodadDeselectMenuItem.IsEnabled = true;
                DoodadCutMenuItem.IsEnabled = true;
                DoodadCopyMenuItem.IsEnabled = true;
                DoodadDeleteMenuItem.IsEnabled = true;
            }



            PopupGrid.Visibility = Visibility.Visible;
            DoodadContextMenu.Visibility = Visibility.Visible;
            PopupReLocatied();
            PopupInnerGrid.Margin = new Thickness(x, y, 0, 0);
            MapViewer.IsEnabled = false;
            PopupReLocatied();
        }

        public void CloseDoodadMenu()
        {
            PopupGrid.Visibility = Visibility.Collapsed;
            DoodadContextMenu.Visibility = Visibility.Collapsed;
            MapViewer.IsEnabled = true;
        }




        private void doodadDeselect_Click(object sender, RoutedEventArgs e)
        {
            doodad_Deselect();
        }

        private void doodadCut_Click(object sender, RoutedEventArgs e)
        {
            doodad_Cut();
        }

        private void doodadCopy_Click(object sender, RoutedEventArgs e)
        {
            doodad_Copy();
        }

        private void doodadPaste_Click(object sender, RoutedEventArgs e)
        {
            doodad_PasteStart();
        }

        private void doodadDelete_Click(object sender, RoutedEventArgs e)
        {
            doodad_Delete();
        }



        public void doodad_Delete()
        {
            taskManager.TaskStart();


            List<CDD2> templist = new List<CDD2>();
            templist.AddRange(SelectDoodad);


            for (int i = 0; i < templist.Count; i++)
            {
                taskManager.TaskAdd(new DoodadEvent(this, templist[i], false));
                mapdata.DD2DeleteMTXM(templist[i]);
                mapdata.DD2.Remove(templist[i]);
            }
            taskManager.TaskEnd();
            SelectDoodad.Clear();
            CloseDoodadMenu();
        }

        public void doodad_Deselect()
        {
            SelectDoodad.Clear();
            CloseDoodadMenu();
        }




        public void doodad_Cut()
        {
            CloseDoodadMenu();
            if (SelectDoodad.Count == 0)
            {
                return;
            }
            doodad_Copy();
            doodad_Delete();
            doodad_PasteStart();

        }
        public void doodad_Copy()
        {
            CloseDoodadMenu();
            if (SelectDoodad.Count == 0)
            {
                return;
            }

            string jsonString = JsonConvert.SerializeObject(SelectDoodad);
            List<CDD2> templist = JsonConvert.DeserializeObject<List<CDD2>>(jsonString);



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

            //if (SpritePalleteCopyTileFix)
            //{
            //    center.X = (float)(Math.Floor(center.X / 32) * 32);
            //    center.Y = (float)(Math.Floor(center.Y / 32) * 32);
            //}

            for (int i = 0; i < templist.Count; i++)
            {
                templist[i].X -= (ushort)center.X;
                templist[i].Y -= (ushort)center.Y;
            }




            jsonString = JsonConvert.SerializeObject(templist);

            Clipboard.SetText(jsonString);
        }


        public void doodad_PasteStart()
        {
            CloseDoodadMenu();
            List<CDD2> templist;
            try
            {
                string jsonString = Clipboard.GetText();

                templist = JsonConvert.DeserializeObject<List<CDD2>>(jsonString);
            }
            catch (Exception)
            {
                return;
            }

            if (templist == null)
            {
                return;
            }


            for (int i = 0; i < templist.Count; i++)
            {
                templist[i].mapData = mapdata;
            }


            CopyedDoodad.Clear();
            CopyedDoodad.AddRange(templist);

            //복사 팔레트 On
            mapDataBinding.DOODAD_BRUSHMODE = true;
            doodad_PasteMode = true;
        }

    }
}
