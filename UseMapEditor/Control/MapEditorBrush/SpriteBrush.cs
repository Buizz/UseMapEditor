using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        public List<CTHG2> SelectSprite = new List<CTHG2>();
        public List<CTHG2> CopyedSprite = new List<CTHG2>();


        public int sprite_player = 11;

        public bool sprite_PasteMode = false;


        public bool sprite_BrushMode = false;
        public bool sprite_SelectMode = true;

        public bool sprite_UnitBrush = false;
        public bool sprite_SpritBrush = true;



        public bool SpritePalleteGridFix = true;
        public bool SpritePalleteCopyTileFix = false;



        private void SpritePallete_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mapDataBinding.SPRITE_BRUSHMODE = true;
            sprite_PasteMode = false;
        }

        private void SpritePallete_SelectionChanged(object sender, EventArgs e)
        {
            mapDataBinding.SPRITE_BRUSHMODE = true;
            sprite_PasteMode = false;
        }


        public void OpenSpriteMenu(int x, int y)
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

        public void CloseSpriteMenu()
        {
            PopupGrid.Visibility = Visibility.Collapsed;
            SpriteContextMenu.Visibility = Visibility.Collapsed;
            MapViewer.IsEnabled = true;
        }

        private void spriteEdit_Click(object sender, RoutedEventArgs e)
        {
            sprite_Edit();
        }

        private void spriteCut_Click(object sender, RoutedEventArgs e)
        {
            sprite_Cut();
        }

        private void spriteCopy_Click(object sender, RoutedEventArgs e)
        {
            sprite_Copy();
        }

        private void spritePaste_Click(object sender, RoutedEventArgs e)
        {
            sprite_PasteStart();
        }
        private void spriteDelete_Click(object sender, RoutedEventArgs e)
        {
            sprite_Delete();
        }

        private void spriteDeselect_Click(object sender, RoutedEventArgs e)
        {
            sprite_Deselect();
        }







        public void sprite_Delete()
        {
            taskManager.TaskStart();


            List<CTHG2> templist = new List<CTHG2>();
            templist.AddRange(SelectSprite);


            for (int i = 0; i < templist.Count; i++)
            {
                taskManager.TaskAdd(new SpriteEvent(this, templist[i], false));
                mapdata.THG2.Remove(templist[i]);
            }
            taskManager.TaskEnd();
            SelectSprite.Clear();
            CloseSpriteMenu();
        }

        public void sprite_Deselect()
        {
            SelectSprite.Clear();
            CloseSpriteMenu();
        }

        public void sprite_Edit()
        {
            if(SelectSprite.Count == 0)
            {
                return;
            }
            PopupGrid.Visibility = Visibility.Visible;
            UnitEditPanel.Visibility = Visibility.Collapsed;
            SpriteEditPanel.Visibility = Visibility.Visible;
            LocEditPanel.Visibility = Visibility.Collapsed;
            SpriteContextMenu.Visibility = Visibility.Collapsed;
            PopupReLocatied();
            MapViewer.IsEnabled = false;



            Vector2 center = new Vector2();
            foreach (var item in SelectSprite)
            {
                center.X += item.X;
                center.Y += item.Y;
            }

            center.X /= SelectSprite.Count;
            center.Y /= SelectSprite.Count;

            Vector2 m = PosMapToScreen(center);
            if (SelectSprite.Count > 1)
            {
                m.X -= (float)SpriteEditPanel.Width / 2;
                m.Y -= (float)SpriteEditPanel.Height / 2;
            }

            m.X = Math.Max(0, m.X);
            m.Y = Math.Max(0, m.Y);

            Vector2 screenSize = GetScreenSize();
            if (screenSize.X < (m.X + SpriteEditPanel.Width))
            {
                m.X = (float)(screenSize.X - SpriteEditPanel.Width);
            }

            if (screenSize.Y < (m.Y + SpriteEditPanel.Height))
            {
                m.Y = (float)(screenSize.Y - SpriteEditPanel.Height);
            }



            PopupInnerGrid.Margin = new Thickness(m.X, m.Y, 0, 0);


            SpriteEditList.Items.Clear();
            foreach (var item in SelectSprite)
            {
                ListBoxItem boxItem = new ListBoxItem();
                boxItem.Content = item.NAME(this);
                boxItem.Tag = item;
                SpriteEditList.Items.Add(boxItem);
            }
            SpriteEditList.SelectedIndex = 0;
            //PopupReLocatied();

            //LocEditPanel.DataContext = locdata;
        }


        private void SpriteEditList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(SpriteEditList.SelectedIndex == -1)
            {
                return;
            }

            ListBoxItem listBoxItem = (ListBoxItem)SpriteEditList.SelectedItem;
            CTHG2 spdata = (CTHG2)listBoxItem.Tag;
            SpriteName.Text = spdata.NAME(this);


            SpriteEditPanel.DataContext = spdata;
        }




        public void sprite_Cut()
        {
            CloseSpriteMenu();
            if (SelectSprite.Count == 0)
            {
                return;
            }
            sprite_Copy();
            sprite_Delete();
            sprite_PasteStart();

        }
        public void sprite_Copy()
        {
            CloseSpriteMenu();
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


        public void sprite_PasteStart()
        {
            CloseSpriteMenu();
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

            if(templist == null)
            {
                return;
            }

            CopyedSprite.Clear();
            CopyedSprite.AddRange(templist);

            //복사 팔레트 On
            mapDataBinding.SPRITE_BRUSHMODE = true;
            sprite_PasteMode = true;
        }

    }
}
