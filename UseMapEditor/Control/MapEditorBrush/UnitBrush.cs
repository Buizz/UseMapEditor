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
            SpriteEditPanel.Visibility = Visibility.Collapsed;
            LocEditPanel.Visibility = Visibility.Collapsed;
            UnitContextMenu.Visibility = Visibility.Collapsed;
            MapViewer.IsEnabled = false;


            Vector2 center = new Vector2();
            foreach (var item in SelectUnit)
            {
                center.X += item.X;
                center.Y += item.Y;
            }

            center.X /= SelectUnit.Count;
            center.Y /= SelectUnit.Count;

            Vector2 m = PosMapToScreen(center);
            if (SelectUnit.Count > 1)
            {
                m.X -= (float)UnitEditPanel.Width / 2;
                m.Y -= (float)UnitEditPanel.Height / 2;
            }

            m.X = Math.Max(0, m.X);
            m.Y = Math.Max(0, m.Y);

            Vector2 screenSize = GetScreenSize();
            if (screenSize.X < (m.X + UnitEditPanel.Width))
            {
                m.X = (float)(screenSize.X - UnitEditPanel.Width);
            }

            if (screenSize.Y < (m.Y + UnitEditPanel.Height))
            {
                m.Y = (float)(screenSize.Y - UnitEditPanel.Height);
            }




            PopupInnerGrid.Margin = new Thickness(m.X, m.Y, 0, 0);



            UnitEditList.Items.Clear();
            foreach (var item in SelectUnit)
            {
                ListBoxItem boxItem = new ListBoxItem();
                boxItem.Content = mapdata.GetCodeName(Codetype.Unit, item.unitID);
                boxItem.Tag = item;
                UnitEditList.Items.Add(boxItem);
                UnitEditList.SelectedItems.Add(boxItem);
            }
            //PopupReLocatied();
            //LocEditPanel.DataContext = locdata;
        }

        private void uEdit_Changed(object sender, RoutedEventArgs e)
        {
            if (unitEditIsLoad)
            {
                return;
            }


            uint tuint;
            ushort tushort;
            byte tbyte;

            uint? _resoruceAmount = null;

            ushort? _X = null;
            ushort? _Y = null;
            ushort? _unitID = null;
            ushort? _hangar = null;

            byte? _player = null;
            byte? _hitPoints = null;
            byte? _shieldPoints = null;
            byte? _energyPoints = null;

            bool? _hpvalid = null;
            bool? _shvalid = null;
            bool? _envalid = null;
            bool? _resvalid = null;
            bool? _hangarvalid = null;
            bool? _cloakvalid = null;
            bool? _cloakstate = null;
            bool? _burrowvalid = null;
            bool? _burrowstate = null;
            bool? _tranvalid = null;
            bool? _buildstate = null;
            bool? _hallvalid = null;
            bool? _hallstate = null;
            bool? _invinvalid = null;
            bool? _invincstate = null;

            if (uint.TryParse(uEdit_resoruceAmount.Text, out tuint)) _resoruceAmount = tuint;

            if (ushort.TryParse(uEdit_X.Text, out tushort)) _X = tushort;
            if (ushort.TryParse(uEdit_Y.Text, out tushort)) _Y = tushort;
            if (ushort.TryParse(uEdit_unitID.Text, out tushort)) _unitID = tushort;
            if (ushort.TryParse(uEdit_hangar.Text, out tushort)) _hangar = tushort;

            if (uEdit_player.SelectedIndex != -1) _player = (byte)uEdit_player.SelectedIndex;
            if (byte.TryParse(uEdit_hitPoints.Text, out tbyte)) _hitPoints = tbyte;
            if (byte.TryParse(uEdit_shieldPoints.Text, out tbyte)) _shieldPoints = tbyte;
            if (byte.TryParse(uEdit_energyPoints.Text, out tbyte)) _energyPoints = tbyte;

            if (uEdit_hpvalid.IsChecked != null) _hpvalid = uEdit_hpvalid.IsChecked;
            if (uEdit_shvalid.IsChecked != null) _shvalid = uEdit_shvalid.IsChecked;
            if (uEdit_envalid.IsChecked != null) _envalid = uEdit_envalid.IsChecked;
            if (uEdit_resvalid.IsChecked != null) _resvalid = uEdit_resvalid.IsChecked;
            if (uEdit_hangarvalid.IsChecked != null) _hangarvalid = uEdit_hangarvalid.IsChecked;
            if (uEdit_cloakvalid.IsChecked != null) _cloakvalid = uEdit_cloakvalid.IsChecked;
            if (uEdit_cloakstate.IsChecked != null) _cloakstate = uEdit_cloakstate.IsChecked;
            if (uEdit_burrowvalid.IsChecked != null) _burrowvalid = uEdit_burrowvalid.IsChecked;
            if (uEdit_burrowstate.IsChecked != null) _burrowstate = uEdit_burrowstate.IsChecked;
            if (uEdit_tranvalid.IsChecked != null) _tranvalid = uEdit_tranvalid.IsChecked;
            if (uEdit_buildstate.IsChecked != null) _buildstate = uEdit_buildstate.IsChecked;
            if (uEdit_hallvalid.IsChecked != null) _hallvalid = uEdit_hallvalid.IsChecked;
            if (uEdit_hallstate.IsChecked != null) _hallstate = uEdit_hallstate.IsChecked;
            if (uEdit_invinvalid.IsChecked != null) _invinvalid = uEdit_invinvalid.IsChecked;
            if (uEdit_invincstate.IsChecked != null) _invincstate = uEdit_invincstate.IsChecked;

            //_hpvalid
            //_shvalid
            //_envalid
            //_resvalid
            //_hangarvalid
            //_cloakvalid
            //_cloakstate
            //_burrowvalid
            //_burrowstate
            //_tranvalid
            //_buildstate
            //_hallvalid
            //_hallstate
            //_invinvalid
            //_invincstate



            foreach (ListBoxItem item in UnitEditList.SelectedItems)
            {
                CUNIT spdata = (CUNIT)item.Tag;

                UnitPropertyEvent.UnitData OLDunitData = new UnitPropertyEvent.UnitData(spdata);

                if (_X != null) spdata.X = (ushort)_X;
                if (_Y != null) spdata.Y = (ushort)_Y;
                if (_unitID != null) spdata.unitID = (ushort)_unitID;
                if (_hangar != null) spdata.hangar = (ushort)_hangar;
                if (_resoruceAmount != null) spdata.resoruceAmount = (uint)_resoruceAmount;
                if (_player != null) spdata.player = (byte)_player;
                if (_hitPoints != null) spdata.hitPoints = (byte)_hitPoints;
                if (_shieldPoints != null) spdata.shieldPoints = (byte)_shieldPoints;
                if (_energyPoints != null) spdata.energyPoints = (byte)_energyPoints;
                if (_hpvalid != null) spdata.hpvalid = (bool)_hpvalid;
                if (_shvalid != null) spdata.shvalid = (bool)_shvalid;
                if (_envalid != null) spdata.envalid = (bool)_envalid;
                if (_resvalid != null) spdata.resvalid = (bool)_resvalid;
                if (_hangarvalid != null) spdata.hangarvalid = (bool)_hangarvalid;
                if (_cloakvalid != null) spdata.cloakvalid = (bool)_cloakvalid;
                if (_cloakstate != null) spdata.cloakstate = (bool)_cloakstate;
                if (_burrowvalid != null) spdata.burrowvalid = (bool)_burrowvalid;
                if (_burrowstate != null) spdata.burrowstate = (bool)_burrowstate;
                if (_tranvalid != null) spdata.tranvalid = (bool)_tranvalid;
                if (_buildstate != null) spdata.buildstate = (bool)_buildstate;
                if (_hallvalid != null) spdata.hallvalid = (bool)_hallvalid;
                if (_hallstate != null) spdata.hallstate = (bool)_hallstate;
                if (_invinvalid != null) spdata.invinvalid = (bool)_invinvalid;
                if (_invincstate != null) spdata.invincstate = (bool)_invincstate;

                UnitPropertyEvent.UnitData NEWunitData = new UnitPropertyEvent.UnitData(spdata);

                taskManager.TaskAdd(new UnitPropertyEvent(this, spdata, NEWunitData, OLDunitData));
            }



            if (_player != null)
            {
                MinimapUnitInitRefresh();
            }
        }

        private bool unitEditIsLoad = false;
        private void UnitEditList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UnitEditList.SelectedIndex == -1)
            {
                return;
            }
            unitEditIsLoad = true;



            CUNIT fspdata = (CUNIT)((ListBoxItem)UnitEditList.SelectedItems[0]).Tag;

            string selectheaders = "[" + UnitEditList.SelectedItems.Count + "개] " + mapdata.GetCodeName(Codetype.Unit, fspdata.unitID) + " ";


            ushort _X = fspdata.X;
            ushort _Y = fspdata.Y;
            ushort _unitID = fspdata.unitID;
            byte _player = fspdata.player;
            bool _hpvalid = fspdata.hpvalid;
            byte _hitPoints = fspdata.hitPoints;
            bool _shvalid = fspdata.shvalid;
            byte _shieldPoints = fspdata.shieldPoints;
            bool _envalid = fspdata.envalid;
            byte _energyPoints = fspdata.energyPoints;
            bool _resvalid = fspdata.resvalid;
            uint _resoruceAmount = fspdata.resoruceAmount;
            bool _hangarvalid = fspdata.hangarvalid;
            ushort _hangar = fspdata.hangar;
            bool _cloakvalid = fspdata.cloakvalid;
            bool _cloakstate = fspdata.cloakstate;
            bool _burrowvalid = fspdata.burrowvalid;
            bool _burrowstate = fspdata.burrowstate;
            bool _tranvalid = fspdata.tranvalid;
            bool _buildstate = fspdata.buildstate;
            bool _hallvalid = fspdata.hallvalid;
            bool _hallstate = fspdata.hallstate;
            bool _invinvalid = fspdata.invinvalid;
            bool _invincstate = fspdata.invincstate;


            uEdit_X.Text = _X.ToString();
            uEdit_Y.Text = _Y.ToString();
            uEdit_unitID.Text = _unitID.ToString();
            uEdit_player.SelectedIndex = _player;
            uEdit_hpvalid.IsChecked = _hpvalid;
            uEdit_hitPoints.Text = _hitPoints.ToString();
            uEdit_shvalid.IsChecked = _shvalid;
            uEdit_shieldPoints.Text = _shieldPoints.ToString();
            uEdit_envalid.IsChecked = _envalid;
            uEdit_energyPoints.Text = _energyPoints.ToString();
            uEdit_resvalid.IsChecked = _resvalid;
            uEdit_resoruceAmount.Text = _resoruceAmount.ToString();
            uEdit_hangarvalid.IsChecked = _hangarvalid;
            uEdit_hangar.Text = _hangar.ToString();
            uEdit_cloakvalid.IsChecked = _cloakvalid;
            uEdit_cloakstate.IsChecked = _cloakstate;
            uEdit_burrowvalid.IsChecked = _burrowvalid;
            uEdit_burrowstate.IsChecked = _burrowstate;
            uEdit_tranvalid.IsChecked = _tranvalid;
            uEdit_buildstate.IsChecked = _buildstate;
            uEdit_hallvalid.IsChecked = _hallvalid;
            uEdit_hallstate.IsChecked = _hallstate;
            uEdit_invinvalid.IsChecked = _invinvalid;
            uEdit_invincstate.IsChecked = _invincstate;


            for (int i = 1; i < UnitEditList.SelectedItems.Count; i++)
            {
                CUNIT spdata = (CUNIT)((ListBoxItem)UnitEditList.SelectedItems[i]).Tag;
                selectheaders += mapdata.GetCodeName(Codetype.Unit, spdata.unitID) + " ";

                if (_X != spdata.X) uEdit_X.Text = "";
                if (_Y != spdata.Y) uEdit_Y.Text = "";
                if (_unitID != spdata.unitID) uEdit_unitID.Text = "";
                if (_player != spdata.player) uEdit_player.SelectedIndex = -1;
                if (_hpvalid != spdata.hpvalid) uEdit_hpvalid.IsChecked = null;
                if (_hitPoints != spdata.hitPoints) uEdit_hitPoints.Text = "";
                if (_shvalid != spdata.shvalid) uEdit_shvalid.IsChecked = null;
                if (_shieldPoints != spdata.shieldPoints) uEdit_shieldPoints.Text = "";
                if (_envalid != spdata.envalid) uEdit_envalid.IsChecked = null;
                if (_energyPoints != spdata.energyPoints) uEdit_energyPoints.Text = "";
                if (_resvalid != spdata.resvalid) uEdit_resvalid.IsChecked = null;
                if (_resoruceAmount != spdata.resoruceAmount) uEdit_resoruceAmount.Text = "";
                if (_hangarvalid != spdata.hangarvalid) uEdit_hangarvalid.IsChecked = null;
                if (_hangar != spdata.hangar) uEdit_hangar.Text = "";
                if (_cloakvalid != spdata.cloakvalid) uEdit_cloakvalid.IsChecked = null;
                if (_cloakstate != spdata.cloakstate) uEdit_cloakstate.IsChecked = null;
                if (_burrowvalid != spdata.burrowvalid) uEdit_burrowvalid.IsChecked = null;
                if (_burrowstate != spdata.burrowstate) uEdit_burrowstate.IsChecked = null;
                if (_tranvalid != spdata.tranvalid) uEdit_tranvalid.IsChecked = null;
                if (_buildstate != spdata.buildstate) uEdit_buildstate.IsChecked = null;
                if (_hallvalid != spdata.hallvalid) uEdit_hallvalid.IsChecked = null;
                if (_hallstate != spdata.hallstate) uEdit_hallstate.IsChecked = null;
                if (_invinvalid != spdata.invinvalid) uEdit_invinvalid.IsChecked = null;
                if (_invincstate != spdata.invincstate) uEdit_invincstate.IsChecked = null;
            }




            //ListBoxItem listBoxItem = (ListBoxItem)UnitEditList.SelectedItem;
            //CUNIT spdata = (CUNIT)listBoxItem.Tag;
            //UnitName.Text = mapdata.GetCodeName(Codetype.Unit, spdata.unitID);

            selectheaders = selectheaders.Replace("\n", "");
            selectheaders = selectheaders.Replace("\r", "");
            //UnitEditPanel.DataContext = spdata;
            if (selectheaders.Length > 35)
            {
                selectheaders = selectheaders.Substring(0, 33) + " ...";
            }
            UnitName.Text = selectheaders;
            unitEditIsLoad = false;
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
