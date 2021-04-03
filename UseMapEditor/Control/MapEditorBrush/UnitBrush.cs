using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UseMapEditor.FileData;
using UseMapEditor.Task;
using UseMapEditor.Task.Events;
using static Data.Map.MapData;

namespace UseMapEditor.Control
{
    public partial class MapEditor : UserControl
    {
        public ObservableCollection<CUNIT> IndexedUnitList = new ObservableCollection<CUNIT>();



        public List<CUNIT> SelectUnit = new List<CUNIT>();
        public List<CUNIT> CopyedUnit = new List<CUNIT>();


        public int unit_player = 0;
        public bool unit_BrushMode = false;
        public bool unit_SelectMode = true;


        public bool unit_PasteMode = false;


        public bool UnitPalleteGridFix = true;
        public bool UnitPalleteBuildingFix = true;
        public bool UnitPalleteStackAllow;
        public bool UnitPalleteCopyTileFix;

        private List<CUNIT> getsellist()
        {
            List<CUNIT> select = new List<CUNIT>();

            foreach (CUNIT item in UnitPlaceList.SelectedItems)
            {
                select.Add(item);
            }

            select.Sort((x, y) => mapdata.UNIT.IndexOf(x).CompareTo(mapdata.UNIT.IndexOf(y)));

            return select;
        }

        private void UnitUp_Click(object sender, RoutedEventArgs e)
        {
            List<CUNIT> select = getsellist();

            for (int i = 0; i < select.Count; i++)
            {
                int index = mapdata.UNIT.IndexOf(select[i]);
                if(index == 0)
                {
                    continue;
                }
                mapdata.UNIT.Remove(select[i]);
                mapdata.UNIT.Insert(index - 1, select[i]);
            }
            UnitPlaceList.Items.SortDescriptions.Clear();
            UnitPlaceList.Items.SortDescriptions.Add(new SortDescription("indexof", ListSortDirection.Ascending));


            IndexedUnitIndexChange();
        }

        private void UnitDown_Click(object sender, RoutedEventArgs e)
        {
            List<CUNIT> select = getsellist();

            for (int i = select.Count - 1; i >= 0; i--)
            {
                int index = mapdata.UNIT.IndexOf(select[i]);
                if (mapdata.UNIT.Count <= index + 1)
                {
                    continue;
                }
                mapdata.UNIT.Remove(select[i]);
                mapdata.UNIT.Insert(index + 1, select[i]);
            }
            UnitPlaceList.Items.SortDescriptions.Clear();
            UnitPlaceList.Items.SortDescriptions.Add(new SortDescription("indexof", ListSortDirection.Ascending));


            IndexedUnitIndexChange();
        }


        private void _unitindex_Checked(object sender, RoutedEventArgs e)
        {
            IndexedUnitRefresh();
        }


        public void IndexedUnitIndexChange()
        {
            List<CUNIT> select = IndexedUnitList.ToList();
            select.Sort((x, y) => mapdata.UNIT.IndexOf(x).CompareTo(mapdata.UNIT.IndexOf(y)));


            int index = 0;


            for (int i = 0; i < select.Count; i++)
            {
                select[i].INDEX = index++;

                switch (select[i].unitID)
                {
                    case 3:
                    case 5:
                    case 17:
                    case 23:
                        index++;
                        break;
                }
            }
        }
        public void IndexedUnitSelectionChange()
        {
            for (int i = 0; i < IndexedUnitList.Count; i++)
            {
                if (SelectUnit.Contains(IndexedUnitList[i]))
                {
                    IndexedUnitList[i].BackGround = System.Windows.Media.Brushes.Red;
                }
                else
                {
                    IndexedUnitList[i].BackGround = null;
                }
            }
        }

        public void IndexedUnitCancel()
        {
            _unitbrush.IsChecked = true;
        }

        public void IndexedUnitRefresh()
        {
            if (!(bool)_unitindex.IsChecked)
            {
                return;
            }

            IndexedUnitList.Clear();

            int index = 0;
            //스프라이트 계산


            for (int i = 0; i < mapdata.UNIT.Count; i++)
            {
                if(mapdata.UNIT[i].unitID == 214)
                {
                    //스타트로케이션
                    continue;
                }

                if (SelectUnit.Contains(mapdata.UNIT[i]))
                {
                    mapdata.UNIT[i].BackGround = System.Windows.Media.Brushes.Red;
                }
                else
                {
                    mapdata.UNIT[i].BackGround = null;
                }


                mapdata.UNIT[i].INDEX = index++;

                switch (mapdata.UNIT[i].unitID)
                {
                    case 3:
                    case 5:
                    case 17:
                    case 23:
                        index++;
                        break;
                }


                IndexedUnitList.Add(mapdata.UNIT[i]);
            }
        }






        private void UnitPallete_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mapDataBinding.UNIT_BRUSHMODE = true;
            unit_PasteMode = false;
        }

        private void UnitPallete_SelectionChanged(object sender, EventArgs e)
        {
            mapDataBinding.UNIT_BRUSHMODE = true;
            unit_PasteMode = false;
        }

        public void OpenUnitMenu(int x, int y)
        {
            if (SelectUnit.Count == 0)
            {
                UnitEditMenuItem.IsEnabled = false;
                UnitDeselectMenuItem.IsEnabled = false;
                UnitCutMenuItem.IsEnabled = false;
                UnitCopyMenuItem.IsEnabled = false;
                UnitDeleteMenuItem.IsEnabled = false;
            }
            else
            {
                UnitEditMenuItem.IsEnabled = true;
                UnitDeselectMenuItem.IsEnabled = true;
                UnitCutMenuItem.IsEnabled = true;
                UnitCopyMenuItem.IsEnabled = true;
                UnitDeleteMenuItem.IsEnabled = true;
            }



            PopupGrid.Visibility = Visibility.Visible;
            UnitContextMenu.Visibility = Visibility.Visible;
            PopupInnerGrid.Margin = new Thickness(x, y, 0, 0);
            MapViewer.IsEnabled = false;
            PopupReLocatied();
        }

        public void CloseUnitMenu()
        {
            PopupGrid.Visibility = Visibility.Collapsed;
            UnitContextMenu.Visibility = Visibility.Collapsed;
            MapViewer.IsEnabled = true;
        }




        private void unitEdit_Click(object sender, RoutedEventArgs e)
        {
            unit_Edit();
        }

        private void unitDeselect_Click(object sender, RoutedEventArgs e)
        {
            unit_Deselect();
        }

        private void unitCut_Click(object sender, RoutedEventArgs e)
        {
            unit_Cut();
        }

        private void unitCopy_Click(object sender, RoutedEventArgs e)
        {
            unit_Copy();
        }

        private void unitPaste_Click(object sender, RoutedEventArgs e)
        {
            unit_PasteStart();
        }

        private void unitDelete_Click(object sender, RoutedEventArgs e)
        {
            unit_Delete();
        }





        public void unit_Delete()
        {
            taskManager.TaskStart();


            List<CUNIT> templist = new List<CUNIT>();
            templist.AddRange(SelectUnit);


            for (int i = 0; i < templist.Count; i++)
            {
                taskManager.TaskAdd(new UnitEvent(this, templist[i], false));
                mapdata.UNITListRemove(templist[i]);
                //mapdata.UNIT.Remove(templist[i]);
            }
            taskManager.TaskEnd();
            SelectUnit.Clear();
            CloseUnitMenu();
        }

        public void unit_Deselect()
        {
            SelectUnit.Clear();
            CloseUnitMenu();
        }

        public void unit_Edit()
        {
            if (SelectUnit.Count == 0)
            {
                return;
            }
            PopupGrid.Visibility = Visibility.Visible;
            UnitEditPanel.Visibility = Visibility.Visible;
            UnitContextMenu.Visibility = Visibility.Collapsed;
            MapViewer.IsEnabled = false;

            Vector2 m = PosMapToScreen(new Vector2(SelectUnit.First().X, SelectUnit.First().Y));
            PopupInnerGrid.Margin = new Thickness(m.X, m.Y, 0, 0);


            UnitEditList.Items.Clear();
            foreach (var item in SelectUnit)
            {
                ListBoxItem boxItem = new ListBoxItem();
                boxItem.Content = mapdata.GetCodeName(Codetype.Unit, item.unitID);
                boxItem.Tag = item;
                UnitEditList.Items.Add(boxItem);
            }
            UnitEditList.SelectedIndex = 0;
            PopupReLocatied();

            //LocEditPanel.DataContext = locdata;
        }


        private void UnitEditList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UnitEditList.SelectedIndex == -1)
            {
                return;
            }

            ListBoxItem listBoxItem = (ListBoxItem)UnitEditList.SelectedItem;
            CUNIT spdata = (CUNIT)listBoxItem.Tag;
            UnitName.Text = mapdata.GetCodeName(Codetype.Unit, spdata.unitID);


            UnitEditPanel.DataContext = spdata;
        }


        private void UnitPlaceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UnitPlaceList.SelectedIndex == -1)
            {
                return;
            }

            CUNIT spdata = (CUNIT) UnitPlaceList.SelectedItem;

            int width = (int)MapViewer.ActualWidth;
            int height = (int)MapViewer.ActualHeight;

            int x = (int)spdata.X;
            int y = (int)spdata.Y;



            opt_xpos = (int)(x - width / opt_scalepercent / 2);
            opt_ypos = (int)(y - height / opt_scalepercent / 2);
        }

        public void unit_Cut()
        {
            CloseUnitMenu();
            if (SelectUnit.Count == 0)
            {
                return;
            }
            unit_Copy();
            unit_Delete();
            unit_PasteStart();

        }
        public void unit_Copy()
        {
            CloseUnitMenu();
            if (SelectUnit.Count == 0)
            {
                return;
            }

            string jsonString = JsonConvert.SerializeObject(SelectUnit);
            List<CUNIT> templist = JsonConvert.DeserializeObject<List<CUNIT>>(jsonString);



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

            bool fidextile = UnitPalleteCopyTileFix;
            bool buildFix = UnitPalleteBuildingFix;


            bool IsBuilding = false;
            for (int i = 0; i < templist.Count; i++)
            {
                int unitid = templist[i].unitID;

                byte sflag = (byte)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Special Ability Flags", unitid).Data;
                if ((sflag & 0x1) > 0)
                {
                    //건물
                    IsBuilding = true;
                    break;
                }
            }





            if (fidextile | (IsBuilding & buildFix))
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


        public void unit_PasteStart()
        {
            CloseUnitMenu();
            List<CUNIT> templist;
            try
            {
                string jsonString = Clipboard.GetText();

                templist = JsonConvert.DeserializeObject<List<CUNIT>>(jsonString);
            }
            catch (Exception)
            {
                return;
            }

            if (templist == null)
            {
                return;
            }

            CopyedUnit.Clear();
            CopyedUnit.AddRange(templist);

            //복사 팔레트 On
            mapDataBinding.UNIT_BRUSHMODE = true;
            unit_PasteMode = true;
        }
    }
}
