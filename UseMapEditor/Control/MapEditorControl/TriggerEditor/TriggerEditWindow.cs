using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using UseMapEditor.FileData;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace UseMapEditor.Control.MapEditorControl
{
    public partial class TriggerEditor : UserControl
    {


        private CTrigger OpenedTrigger;
        private bool IsOpenNewTrigger;
        private ObservableCollection<TrigItem> currentList;
        private bool IsOpenTriggerEidtWindow;


        private ObservableCollection<TrigItem> Copyedactions = new ObservableCollection<TrigItem>();
        private ObservableCollection<TrigItem> Copyedconditions = new ObservableCollection<TrigItem>();

        private int LastPage = 0;
        private void OpenTriggerWindow(CTrigger cTrigger)
        {
            IsOpenTriggerEidtWindow = true;
            if (cTrigger == null)
            {
                IsOpenNewTrigger = true;
                cTrigger = new CTrigger(mapEditor.mapdata);
            }
            else
            {
                IsOpenNewTrigger = false;
            }
            OpenedTrigger = cTrigger;

            Copyedconditions.Clear();
            Copyedactions.Clear();

            foreach (TrigItem item in OpenedTrigger.conditions)
            {
                Copyedconditions.Add(item.Clone());
            }

            foreach (TrigItem item in OpenedTrigger.actions)
            {
                Copyedactions.Add(item.Clone());
            }







            InputDialog.DataContext = OpenedTrigger;

            FlagListbox.SelectedIndex = -1;
            foreach (ListBoxItem item in FlagListbox.Items)
            {
                int tagv = int.Parse((string)item.Tag);

                if((OpenedTrigger.exeflag & (0b1 << tagv)) > 0)
                {
                    FlagListbox.SelectedItems.Add(item);
                }
            }




            PlayerListbox.SelectedIndex = -1;
            for (int i = 0; i < OpenedTrigger.playerlist.Length; i++)
            {
                if(OpenedTrigger.playerlist[i] == 1)
                {
                    foreach (ListBoxItem item in PlayerListbox.Items)
                    {
                        int tagv = int.Parse((string)item.Tag);
                        if(tagv == i)
                        {
                            PlayerListbox.SelectedItems.Add(item);
                            break;
                        }
                    }
                }
            }



            ItemEditPage.Visibility = Visibility.Collapsed;
            TrigEditPage.Visibility = Visibility.Visible;

            EditWindow.Visibility = Visibility.Visible;
            OpenStroyBoard.Begin(this);
            ItemToolBoxRefresh();
            TrigPageSetting(LastPage);
        }


        private void TrigPageSetting(int page)
        {
            LastPage = page;
            switch (page)
            {
                case 0:
                    TrigSettingBtn.IsEnabled = false;
                    TrigConditionBtn.IsEnabled = true;
                    TrigActionBtn.IsEnabled = true;

                    TriggerToolBar.IsEnabled = false;
                    TrigSetting.Visibility = Visibility.Visible;
                    TrigItems.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    TrigSettingBtn.IsEnabled = true;
                    TrigConditionBtn.IsEnabled = false;
                    TrigActionBtn.IsEnabled = true;

                    TriggerToolBar.IsEnabled = true;
                    TrigSetting.Visibility = Visibility.Collapsed;
                    TrigItems.Visibility = Visibility.Visible;

                    currentList = Copyedconditions;
                    TriggerItemListbox.ItemsSource = Copyedconditions;
                    break;
                case 2:
                    TrigSettingBtn.IsEnabled = true;
                    TrigConditionBtn.IsEnabled = true;
                    TrigActionBtn.IsEnabled = false;

                    TriggerToolBar.IsEnabled = true;
                    TrigSetting.Visibility = Visibility.Collapsed;
                    TrigItems.Visibility = Visibility.Visible;

                    currentList = Copyedactions;
                    TriggerItemListbox.ItemsSource = Copyedactions;
                    break;
            }
        }




        private void TrigSettingBtn_Click(object sender, RoutedEventArgs e)
        {
            TrigPageSetting(0);
        }

        private void TrigConditionBtn_Click(object sender, RoutedEventArgs e)
        {
            TrigPageSetting(1);
        }

        private void TrigActionBtn_Click(object sender, RoutedEventArgs e)
        {
            TrigPageSetting(2);
        }




        private void OkayBtn_Click(object sender, RoutedEventArgs e)
        {
            TriggerOkay();
        }
        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            TriggerCancel();
        }

        private void TriggerOkay()
        {
            if(PlayerListbox.SelectedItems.Count == 0)
            {
                SnackbarMessage.Enqueue("하나 이상의 플레이어가 설정되야 합니다.");
                return;
            }

            uint flagv = 0;
            foreach (ListBoxItem item in FlagListbox.SelectedItems)
            {
                int tagv = int.Parse((string)item.Tag);

                flagv += (uint)(0b1 << tagv);
            }
            OpenedTrigger.exeflag = flagv;






            for (int i = 0; i < OpenedTrigger.playerlist.Length; i++)
            {
                OpenedTrigger.playerlist[i] = 0;
            }


            foreach (ListBoxItem item in PlayerListbox.SelectedItems)
            {
                int tagv = int.Parse((string)item.Tag);
                OpenedTrigger.playerlist[tagv] = 1;
            }



            if (IsOpenNewTrigger)
            {
                //새 트리거
                if(MainListBox.SelectedIndex == -1)
                {
                    //선택 안했을 경우 맨 아래에 넣기
                    mapEditor.mapdata.Triggers.Add(OpenedTrigger);
                    MainListBox.ScrollIntoView(OpenedTrigger);
                }
                else
                {
                    //아닐 경우 인서트
                    mapEditor.mapdata.Triggers.Insert(MainListBox.SelectedIndex, OpenedTrigger);
                    MainListBox.ScrollIntoView(OpenedTrigger);
                }
            }

            OpenedTrigger.conditions.Clear();
            foreach (TrigItem item in Copyedconditions)
            {
                OpenedTrigger.conditions.Add(item);
            }

            OpenedTrigger.actions.Clear();
            foreach (TrigItem item in Copyedactions)
            {
                OpenedTrigger.actions.Add(item);
            }





            mapEditor.SetDirty();
            OpenedTrigger.PropertyChangeAll();
            IsOpenTriggerEidtWindow = false;
            CloseStroyBoard.Begin(this);
            ListFliter();
        }
        private void TriggerCancel()
        {
            IsOpenTriggerEidtWindow = false;
            CloseStroyBoard.Begin(this);
        }

        private void UpItemtrig(TrigItem item)
        {
            int LastIndex = currentList.IndexOf(item);

            currentList.RemoveAt(LastIndex);
            currentList.Insert(LastIndex - 1, item);
        }
        private void DownItemtrig(TrigItem item)
        {
            int LastIndex = currentList.IndexOf(item);

            currentList.RemoveAt(LastIndex);
            currentList.Insert(LastIndex + 1, item);
        }



        private void ItemNew()
        {
            if (!IsOpenTriggerEidtWindow)
            {
                return;
            }

            
            OpenTrigItemEditWindow(null, (LastPage == 2));
        }

        private void ItemUp()
        {
            if (TriggerItemListbox.SelectedItems.Count == 0)
            {
                return;
            }

            List<TrigItem> trigitems = new List<TrigItem>();
            foreach (TrigItem item in TriggerItemListbox.SelectedItems)
            {
                trigitems.Add(item);
            }

            trigitems.Sort((x, y) =>
            {
                int xpos = currentList.IndexOf(x);
                int ypos = currentList.IndexOf(y);

                return xpos.CompareTo(ypos);
            });

            if (TriggerItemListbox.Items.IndexOf(trigitems.First()) == 0)
            {
                return;
            }


            foreach (TrigItem item in trigitems)
            {
                UpItemtrig(item);
            }


            foreach (TrigItem item in trigitems)
            {
                TriggerItemListbox.SelectedItems.Add(item);
            }
            TriggerItemListbox.ScrollIntoView(trigitems.First());
        }

        private void ItemDown()
        {
            if (TriggerItemListbox.SelectedItems.Count == 0)
            {
                return;
            }

            List<TrigItem> trigitems = new List<TrigItem>();
            foreach (TrigItem item in TriggerItemListbox.SelectedItems)
            {
                trigitems.Add(item);
            }

            trigitems.Sort((x, y) =>
            {
                int xpos = currentList.IndexOf(x);
                int ypos = currentList.IndexOf(y);

                return ypos.CompareTo(xpos);
            });

            if (TriggerItemListbox.Items.IndexOf(trigitems.First()) == TriggerItemListbox.Items.Count - 1)
            {
                return;
            }

            foreach (TrigItem item in trigitems)
            {
                DownItemtrig(item);
            }


            foreach (TrigItem item in trigitems)
            {
                TriggerItemListbox.SelectedItems.Add(item);
            }
            TriggerItemListbox.ScrollIntoView(trigitems.Last());
        }


        private void ItemEdit()
        {
            if (TriggerItemListbox.SelectedItems.Count == 1)
            {
                OpenTrigItemEditWindow((TrigItem)TriggerItemListbox.SelectedItems[0]);
            }
        }

        private void ItemCut()
        {
            ItemCopy();
            ItemDelete();
        }

        private void ItemCopy()
        {
            StringBuilder sb = new StringBuilder();

            List<TrigItem> trigitems = new List<TrigItem>();
            trigitems.AddRange(TriggerItemListbox.SelectedItems.Cast< TrigItem>().ToList());
            trigitems.Sort((x, y) => TriggerItemListbox.Items.IndexOf(x).CompareTo(TriggerItemListbox.Items.IndexOf(y)));

            foreach (TrigItem item in trigitems)
            {
                item.CodeText(sb);
                sb.AppendLine(";");
            }
            Clipboard.SetText(sb.ToString());
        }

        private void ItemPaste()
        {
            

            string pastetext = Clipboard.GetText();

            if(LastPage == 1)
            {
                pastetext = "Trigger {conditions = {" + pastetext  + "},actions = {},}";
            }
            else
            {
                pastetext = "Trigger {conditions = {},actions = {" + pastetext + "},}";
            }

            

            List<CTrigger> ctrig = teplua.exec(pastetext, mapEditor);
            if (ctrig != null)
            {

                int sindex = TriggerItemListbox.SelectedIndex;

                int i = 1;


                if (LastPage == 1)
                {
                    foreach (var item in ctrig[0].conditions)
                    {
                        if (sindex == -1)
                        {
                            Copyedconditions.Add(item);
                        }
                        else
                        {
                            Copyedconditions.Insert(sindex + i++, item);
                        }
                    }
                }
                else
                {
                    foreach (var item in ctrig[0].actions)
                    {
                        if (sindex == -1)
                        {
                            Copyedactions.Add(item);
                        }
                        else
                        {
                            Copyedactions.Insert(sindex + i++, item);
                        }
                    }
                }



                mapEditor.SetDirty();
            }


        }

        private void ItemDelete()
        {
            if (!IsOpenTriggerEidtWindow)
            {
                return;
            }


            List<TrigItem> items = new List<TrigItem>();
            foreach (TrigItem item in TriggerItemListbox.SelectedItems)
            {
                items.Add(item);
            }
            foreach (TrigItem item in items)
            {
                currentList.Remove(item);
            }
        }


        private void TriggerItemListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ItemToolBoxRefresh();
        }


        private void ItemToolBoxRefresh()
        {
            if (TriggerItemListbox.SelectedItems.Count == 0)
            {
                //선택한게 없을 경우
                ItemUpBtn.IsEnabled = false;
                ItemDownBtn.IsEnabled = false;
                ItemCutBtn.IsEnabled = false;
                ItemEditBtn.IsEnabled = false;
                ItemCutBtn.IsEnabled = false;
                ItemCopyBtn.IsEnabled = false;
                ItemPasteBtn.IsEnabled = false;
                ItemDeleteBtn.IsEnabled = false;
            }
            else
            {
                ItemUpBtn.IsEnabled = true;
                ItemDownBtn.IsEnabled = true;
                ItemEditBtn.IsEnabled = true;
                ItemCutBtn.IsEnabled = true;
                ItemCopyBtn.IsEnabled = true;

                ItemPasteBtn.IsEnabled = true;

                ItemDeleteBtn.IsEnabled = true;
            }
        }




        private void TriggerItemListbox_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void TriggerItemListbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ItemEdit();
        }



        private void ItemNew_Click(object sender, RoutedEventArgs e)
        {
            ItemNew();
        }

        private void ItemUp_Click(object sender, RoutedEventArgs e)
        {
            ItemUp();
        }

        private void ItemDown_Click(object sender, RoutedEventArgs e)
        {
            ItemDown();
        }

        private void ItemEdit_Click(object sender, RoutedEventArgs e)
        {
            ItemEdit();
        }

        private void ItemCut_Click(object sender, RoutedEventArgs e)
        {
            ItemCut();
        }

        private void ItemCopy_Click(object sender, RoutedEventArgs e)
        {
            ItemCopy();
        }

        private void ItemPaste_Click(object sender, RoutedEventArgs e)
        {
            ItemPaste();
        }

        private void ItemDelete_Click(object sender, RoutedEventArgs e)
        {
            ItemDelete();
        }
    }
}
