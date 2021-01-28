using Microsoft.Xna.Framework;
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
using static Data.Map.MapData;

namespace UseMapEditor.Control
{
    public partial class MapEditor : UserControl
    {
        public void OpenLocSelecter(int x, int y, List<LocationData> locationDatas)
        {
            MulitLocSelecter.Visibility = Visibility.Visible;
            PopupGrid.Visibility = Visibility.Visible;
            PopupInnerGrid.Margin = new Thickness(x, y, 0, 0);
            MapViewer.IsEnabled = false;
            PopupReLocatied();

            LocList.Items.Clear();
            for (int i = 0; i < locationDatas.Count; i++)
            {
                ListBoxItem listBoxItem = new ListBoxItem();

                listBoxItem.Content = locationDatas[i].NAME;
                listBoxItem.Tag = locationDatas[i];


                LocList.Items.Add(listBoxItem);
            }
            LocList.Focus();
        }


        private void LocList_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (LocList.SelectedIndex != -1)
            {
                ListBoxItem listBoxItem = (ListBoxItem)LocList.SelectedItem;
                LocationData locdata = (LocationData)listBoxItem.Tag;

                SelectLocation.Clear();
                SelectLocation.Add(locdata);

                MulitLocSelecter.Visibility = Visibility.Collapsed;
                PopupGrid.Visibility = Visibility.Collapsed;
                MapViewer.IsEnabled = true;
            }
        }


        public void OpenLocEdit()
        {
            if(SelectLocation.Count == 0)
            {
                return;
            }
            if (SelectLocation.Count == 1)
            {
                LocEditList.Visibility = Visibility.Collapsed;

                LocationData selloc = SelectLocation.First();

                Vector2 m = PosMapToScreen(new Vector2(selloc.X, selloc.Y));
                LocEditPanel.DataContext = selloc;
                PopupInnerGrid.Margin = new Thickness(m.X, m.Y, 0, 0);
            }
            else
            {
                LocEditList.Visibility = Visibility.Visible;

                //long xsum = 0;
                //long ysum = 0;

                LocEditList.Items.Clear();
                for (int i = 0; i < SelectLocation.Count; i++)
                {
                    ListBoxItem listBoxItem = new ListBoxItem();

                    Vector2 m = PosMapToScreen(new Vector2(SelectLocation[i].X, SelectLocation[i].Y));
                    //xsum += (long)m.X;
                    //ysum += (long)m.Y;

                    listBoxItem.Content = SelectLocation[i].NAME;
                    listBoxItem.Tag = SelectLocation[i];

                    LocEditList.Items.Add(listBoxItem);
                }
                LocEditList.SelectedIndex = 0;
                ListRefresh();

                //xsum = xsum / SelectLocation.Count;
                //ysum = ysum / SelectLocation.Count;


                //LocEditPanel.Margin = new Thickness(xsum, ysum, 0, 0);
            }


            LocEditPanel.Visibility = Visibility.Visible;
            PopupGrid.Visibility = Visibility.Visible;
            PopupReLocatied();
            MapViewer.IsEnabled = false;
            taskManager.TaskStart();
        }


        private void ListRefresh()
        {
            if (LocEditList.SelectedIndex != -1)
            {
                ListBoxItem listBoxItem = (ListBoxItem)LocEditList.SelectedItem;
                LocationData locdata = (LocationData)listBoxItem.Tag;

                Vector2 m = PosMapToScreen(new Vector2(locdata.X, locdata.Y));
                //LocEditPanel.Margin = new Thickness(m.X, m.Y, 0, 0);

                LocEditPanel.DataContext = locdata;
                //locdata.PropertyChangeAll();
            }
        }

        private void LocEditList_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            ListRefresh();
        }



        private void LocEditList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void LocEditBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenLocEdit();
        }

        private void LocationList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenLocEdit();
        }

        private void LocUp_Click(object sender, RoutedEventArgs e)
        {
            taskManager.TaskStart();
            LocationData loc = (LocationData)LocationList.SelectedItem;
            if(loc == null)
            {
                return;
            }

            int index = loc.INDEX - 1;
            if (index == 0)
            {
                return;
            }

            LocationData uloc = mapdata.LocationDatas.SingleOrDefault((x) => x.INDEX == index);
            if (uloc == null)
            {
                loc.INDEX = index;
            }
            else
            {
                loc.INDEX = index;
                uloc.INDEX = index + 1;
            }
            LocationListSort();
            taskManager.TaskEnd();
        }
        public void LocationListSort()
        {
            LocationList.Items.SortDescriptions.Clear();
            LocationList.Items.SortDescriptions.Add(new SortDescription("INDEX", ListSortDirection.Ascending));
        }

        private void LocDown_Click(object sender, RoutedEventArgs e)
        {
            taskManager.TaskStart();
            LocationData loc = (LocationData)LocationList.SelectedItem;
            if (loc == null)
            {
                return;
            }

            int index = loc.INDEX + 1;
            if (index == 256)
            {
                return;
            }

            LocationData uloc = mapdata.LocationDatas.SingleOrDefault((x) => x.INDEX == index);
            if (uloc == null)
            {
                loc.INDEX = index;
            }
            else
            {
                loc.INDEX = index;
                uloc.INDEX = index - 1;
            }
            LocationListSort();
            taskManager.TaskEnd();
        }

        private void LocDelete_Click(object sender, RoutedEventArgs e)
        {
            SetDirty();
            LocationData locationData = (LocationData)LocationList.SelectedItem;
            taskManager.TaskStart();
            taskManager.TaskAdd(new LocationEvent(this, locationData, false));
            taskManager.TaskEnd();
            mapdata.LocationDatas.Remove(locationData);

        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            refreshLocBox();
        }
        private void refreshLocBox()
        {
            string searchText = SearchBox.Text;

            LocationList.Items.Filter = delegate (object obj)
            {
                LocationData loc = (LocationData)obj;
                if (loc.INDEX == 0)
                {
                    return false;
                }

                string str = loc.NAME;
                if (String.IsNullOrEmpty(str)) return false;
                int index = str.IndexOf(searchText, 0);

                return (index > -1);
            };
        }

        private void LocationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LocationData loc = (LocationData)LocationList.SelectedItem;
            if (loc != null)
            {
                SelectLocation.Clear();
                SelectLocation.Add(loc);
                int width = (int)MapViewer.ActualWidth;
                int height = (int)MapViewer.ActualHeight;

                int x = (int)((loc.R + loc.L) / 2);
                int y = (int)((loc.T + loc.B) / 2);



                opt_xpos = (int)(x - width / opt_scalepercent / 2);
                opt_ypos = (int)(y - height / opt_scalepercent / 2);
            }
        }


    }
}
