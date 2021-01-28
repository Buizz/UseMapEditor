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

namespace UseMapEditor.Control.MapEditorControl
{
    public partial class TriggerEditor : UserControl
    {

        private void ActionSelectBtn_Click(object sender, RoutedEventArgs e)
        {
            if (IsOpen)
            {
                if(SelectTrigitem == null)
                {
                    return;
                }

                IsOpen = false;
                ItemTypeSelecter.Visibility = Visibility.Collapsed;
            }
            else
            {
                OpenTypeSelecter(SelectTrigitem);
            }
        }


        private bool IsOpen;
        private void OpenTypeSelecter(TrigItem trigItem, bool IsAction = false)
        {
            if (trigItem != null)
            {
                ActionName.Text = trigItem.name;
                IsAction = trigItem.IsAction;
            }
            else
            {
                ActionName.Text = "";
            }

            CloseValueSelecter();



            TrigItemTypeListBox.Items.Clear();

            List<TriggerManger.TriggerDefine> tlist;
            if (IsTrigger)
            {
                if (IsAction)
                {
                    tlist = tm.Actions;
                }
                else
                {
                    tlist = tm.Conditions;
                }
            }
            else
            {
                tlist = tm.BrifngActions;
            }

            foreach (TriggerManger.TriggerDefine item in tlist)
            {
                if(item == null)
                {
                    continue;
                }

                ListBoxItem listBoxItem = new ListBoxItem();

                listBoxItem.Tag = item;                
                listBoxItem.Content = item.NAME + "\n" + item.SUMMARY;

                TrigItemTypeListBox.Items.Add(listBoxItem);
            }
            TrigItemTypeListBox.Items.SortDescriptions.Add(new SortDescription("Content", ListSortDirection.Ascending));




            SelectTrigitem = trigItem;

            IsOpen = true;
            ItemTypeSelecter.Visibility = Visibility.Visible;

            TrigItemTypeListBox.Focus();
        }


        private void ItemTypeOkay()
        {
            if (SelectTrigitem == null)
            {
                if(TrigItemTypeListBox.SelectedIndex == -1)
                {
                    SnackbarMessage.Enqueue("타입을 선택하세요.");
                    return;
                }
            }

            if (TrigItemTypeListBox.SelectedIndex != -1)
            {
                if (CopyedSelectTrigitem == null)
                {
                    CopyedSelectTrigitem = new TrigItem(mapEditor.mapdata);

                    if (!IsTrigger)
                    {
                        CopyedSelectTrigitem.IsTrigger = false;
                        CopyedSelectTrigitem.IsAction = true;
                    }
                }


                ListBoxItem listitem = (ListBoxItem)TrigItemTypeListBox.SelectedItem;
                CopyedSelectTrigitem.Init((TriggerManger.TriggerDefine)listitem.Tag);
                RefreshItem(CopyedSelectTrigitem);
            }


            ActionName.Text = CopyedSelectTrigitem.name;


            IsOpen = false;
            ItemTypeSelecter.Visibility = Visibility.Collapsed;
        }

        private void TrigItemTypeListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ItemTypeOkay();
        }
        private void ItemTypeOkayBtn_Click(object sender, RoutedEventArgs e)
        {
            ItemTypeOkay();
        }
    }
}
