using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UseMapEditor.FileData;
using UseMapEditor.Windows;

namespace UseMapEditor.Control.MapEditorControl
{
    /// <summary>
    /// TriggerEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TriggerEditor : UserControl
    {
        public MapEditor mapEditor;

        private TriggerManger tm = Global.WindowTool.triggerManger;
        ObservableCollection<CTrigger> triggerlist;

        private bool IsTrigger;
        public void SetMapEditor(MapEditor mapEditor, bool IsTrigger)
        {
            this.mapEditor = mapEditor;

            this.IsTrigger = IsTrigger;
            ValueSelecterControl.SetMapEditor(mapEditor);


            if (IsTrigger)
            {
                TrigEditPlusBtn.Visibility = Visibility.Visible;
                triggerlist = mapEditor.mapdata.Triggers;
                TrigConditionBtn.Visibility = Visibility.Visible;
            }
            else
            {
                TrigEditPlusBtn.Visibility = Visibility.Collapsed;
                triggerlist = mapEditor.mapdata.Brifings;
                TrigConditionBtn.Visibility = Visibility.Collapsed;
            }

            MainListBox.ItemsSource = triggerlist;


            TrigEditWindowOpen = false;
            EditWindow.Visibility = Visibility.Hidden;

            SearchType.SelectedIndex = 0;
            ToolBoxRefresh();
        }


        MaterialDesignThemes.Wpf.SnackbarMessageQueue SnackbarMessage = new MaterialDesignThemes.Wpf.SnackbarMessageQueue();
        public TriggerEditor()
        {
            InitializeComponent();

            SnackbarOne.MessageQueue = SnackbarMessage;
            AnimationInit();
            ValueSelecterControl.SelectEvent += ValueSelecterControl_SelectEvent;
            ValueSelecterControl.CloseEvent += ValueSelecterControl_CloseEvent;
            //for (int i = 0; i < 10000; i++)
            //{
            //    MainListBox.Items.Add("asd");
            //}
        }



        private string searchText = "";
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchText = SearchBox.Text;
            ListFliter();
        }
        private void ListFliter()
        {
            byte[] playercheck = new byte[27];
            int iii = 0;



            foreach (ListBoxItem item in PlayerFliter.SelectedItems)
            {
                int index = int.Parse((string)item.Tag);
                
                if(index == -1)
                {
                    continue;
                }

                playercheck[iii++] = (byte)index;
            }

            MainListBox.Items.Filter = delegate (object obj)
            {
                CTrigger trig = (CTrigger)obj;

                bool rval = true;
                for (int i = 0; i < iii; i++)
                {
                    if(trig.playerlist[playercheck[i]] == 0)
                    {
                        return false;
                    }
                }


                string str = "";
                switch (SearchType.SelectedIndex)
                {
                    case 0://주석
                        str = trig.CommentString;
                        break;
                    case 1://조건
                        str = trig.ConditionString;
                        break;
                    case 2://액션
                        str = trig.ActionsString;
                        break;
                    case 3://모두
                        str = trig.ConditionString + trig.ActionsString;
                        break;
                }


                if (String.IsNullOrEmpty(str)) return false;
                int index = str.IndexOf(searchText, 0);
                if (index == -1)
                {
                    return false;
                }


                return rval;
            };
        }



        private void PlayerFliter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListFliter();
        }

        private void SearchType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListFliter();
        }



        private void UpItem(CTrigger cTrigger)
        {
            int LastIndex = triggerlist.IndexOf(cTrigger);

            triggerlist.RemoveAt(LastIndex);
            triggerlist.Insert(LastIndex - 1, cTrigger);
        }
        private void DownItem(CTrigger cTrigger)
        {
            int LastIndex = triggerlist.IndexOf(cTrigger);

            triggerlist.RemoveAt(LastIndex);
            triggerlist.Insert(LastIndex + 1, cTrigger);
        }


        private void NewFunc()
        {
            OpenTriggerWindow(null);
        }
        private void UpFunc()
        {
            if(MainListBox.SelectedItems.Count == 0)
            {
                return;
            }

            List<CTrigger> cTriggers = new List<CTrigger>();
            foreach (CTrigger item in MainListBox.SelectedItems)
            {
                cTriggers.Add(item);
            }

            cTriggers.Sort((x, y) =>
            {
                int xpos = triggerlist.IndexOf(x);
                int ypos = triggerlist.IndexOf(y);

                return xpos.CompareTo(ypos);
            });

            if(MainListBox.Items.IndexOf(cTriggers.First()) == 0)
            {
                return;
            }


            foreach (CTrigger item in cTriggers)
            {
                UpItem(item);
            }


            foreach (CTrigger item in cTriggers)
            {
                MainListBox.SelectedItems.Add(item);
            }
            MainListBox.ScrollIntoView(cTriggers.First());
            mapEditor.SetDirty();
        }
        private void DownFunc()
        {
            if (MainListBox.SelectedItems.Count == 0)
            {
                return;
            }

            List<CTrigger> cTriggers = new List<CTrigger>();
            foreach (CTrigger item in MainListBox.SelectedItems)
            {
                cTriggers.Add(item);
            }

            cTriggers.Sort((x, y) =>
            {
                int xpos = triggerlist.IndexOf(x);
                int ypos = triggerlist.IndexOf(y);

                return ypos.CompareTo(xpos);
            });

            if (MainListBox.Items.IndexOf(cTriggers.First()) == MainListBox.Items.Count - 1)
            {
                return;
            }

            foreach (CTrigger item in cTriggers)
            {
                DownItem(item);
            }


            foreach (CTrigger item in cTriggers)
            {
                MainListBox.SelectedItems.Add(item);
            }
            MainListBox.ScrollIntoView(cTriggers.Last());
            mapEditor.SetDirty();
        }
        private void EditFunc()
        {
            if(MainListBox.SelectedItems.Count > 0)
            {
                OpenTriggerWindow((CTrigger)MainListBox.SelectedItems[0]);
            }
        }
        private void CutFunc()
        {
            CopyFunc();
            DeleteFunc();
        }
        private void CopyFunc()
        {
            StringBuilder sb = new StringBuilder();


            List<CTrigger> ctrigs = new List<CTrigger>();
            ctrigs.AddRange(MainListBox.SelectedItems.Cast<CTrigger>().ToList());

            ctrigs.Sort((x, y) => triggerlist.IndexOf(x).CompareTo(triggerlist.IndexOf(y)));

            foreach (CTrigger item in ctrigs)
            {
                item.GetTEPText(sb);
            }

            Clipboard.SetDataObject(sb.ToString());
        }


        private Lua.TrigEditPlus.Main teplua = Global.WindowTool.lua.tepMain;
        private void PasteFunc()
        {
            string pastetext = Clipboard.GetText();
            List<CTrigger> ctrig = teplua.exec(pastetext, mapEditor, IsTrigger);
            if(ctrig != null)
            {
                int sindex = MainListBox.SelectedIndex;

                int i = 1;
                foreach (var item in ctrig)
                {
                    if(sindex == -1)
                    {
                        triggerlist.Add(item);
                    }
                    else
                    {
                        triggerlist.Insert(sindex + i++, item);
                    }
                }
                mapEditor.SetDirty();
            }
        }









        private void DeleteFunc()
        {
            List<CTrigger> cTriggers = new List<CTrigger>();
            foreach (CTrigger item in MainListBox.SelectedItems)
            {
                cTriggers.Add(item);
            }
            foreach (CTrigger item in cTriggers)
            {
                triggerlist.Remove(item);
            }
            mapEditor.SetDirty();
        }








        private void ToolBoxRefresh()
        {
            if(MainListBox.SelectedItems.Count == 0)
            {
                //선택한게 없을 경우
                UpBtn.IsEnabled = false;
                DownBtn.IsEnabled = false;
                CutBtn.IsEnabled = false;
                EditBtn.IsEnabled = false;
                CutBtn.IsEnabled = false;
                CopyBtn.IsEnabled = false;
                PasteBtn.IsEnabled = true;
                DeleteBtn.IsEnabled = false;
            }
            else
            {
                UpBtn.IsEnabled = true;
                DownBtn.IsEnabled = true;
                EditBtn.IsEnabled = true;
                CutBtn.IsEnabled = true;
                CopyBtn.IsEnabled = true;
                PasteBtn.IsEnabled = true;
                DeleteBtn.IsEnabled = true;
            }
        }





        private void New_Click(object sender, RoutedEventArgs e)
        {
            NewFunc();
        }

        private void Up_Click(object sender, RoutedEventArgs e)
        {
            UpFunc();
        }

        private void Down_Click(object sender, RoutedEventArgs e)
        {
            DownFunc();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            EditFunc();
        }

        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            CutFunc();
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            CopyFunc();
        }

        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            PasteFunc();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            DeleteFunc();
        }

        private void MainListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ToolBoxRefresh();
        }

        private void MainListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditFunc();
        }



        private void EditWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if((e.GetPosition(InputDialog).Y < 0) | (e.GetPosition(InputDialog).Y > 400))
            {
                if (TrigEditWindowOpen)
                {
                    TrigEditOkayBtn();
                }
                else
                {
                    TriggerOkay();
                }
            }
            else
            {
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    if (TrigEditWindowOpen)
                    {
                        TrigEditOkayBtn();
                    }
                    else
                    {
                        if (IsOpenTriggerEidtWindow)
                        {
                            if (IsTrigger)
                            {
                                LastPage++;
                                if (LastPage > 2)
                                {
                                    LastPage = 0;
                                }
                            }
                            else
                            {
                                if (LastPage == 0)
                                {
                                    LastPage = 2;
                                }
                                else
                                {
                                    LastPage = 0;
                                }
                            }
                
                            TrigPageSetting(LastPage);
                        }
                    }
                }
            }

            if(ValueSelecter.Visibility == Visibility.Visible)
            {
                Point vp = e.GetPosition(ValueSelecter);

                if ((vp.X < 0 | vp.Y < 0) | (vp.X > ValueSelecter.ActualWidth | vp.Y > ValueSelecter.ActualHeight))
                {
                    CloseValueSelecter();
                }
            }
        }


        Storyboard OpenStroyBoard;
        Storyboard CloseStroyBoard;
        private void AnimationInit()
        {
            EditAnimationInit();
            {
                //ScaleTransform scale1 = new ScaleTransform(1, 1);

                //InputDialog.RenderTransformOrigin = new Point(0.5, 1);
                //InputDialog.RenderTransform = scale1;



                DoubleAnimation myHeightAnimation = new DoubleAnimation();
                myHeightAnimation.From = 200.0;
                myHeightAnimation.To = 400.0;
                myHeightAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(150));

                //DoubleAnimation myWidthAnimation = new DoubleAnimation();
                //myWidthAnimation.From = 0.5;
                //myWidthAnimation.To = 1.0;
                //myWidthAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(150));

                DoubleAnimation myOpacityAnimation = new DoubleAnimation();
                myOpacityAnimation.From = 0.0;
                myOpacityAnimation.To = 1.0;
                myOpacityAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(150));

                OpenStroyBoard = new Storyboard();
                OpenStroyBoard.Children.Add(myOpacityAnimation);
                //OpenStroyBoard.Children.Add(myWidthAnimation);
                OpenStroyBoard.Children.Add(myHeightAnimation);
                Storyboard.SetTargetName(myOpacityAnimation, EditWindow.Name);
                //Storyboard.SetTargetName(myWidthAnimation, InputDialog.Name);
                Storyboard.SetTargetName(myHeightAnimation, InputDialog.Name);
                Storyboard.SetTargetProperty(myOpacityAnimation, new PropertyPath(Border.OpacityProperty));
                //Storyboard.SetTargetProperty(myHeightAnimation, new PropertyPath("RenderTransform.ScaleY"));
                //Storyboard.SetTargetProperty(myWidthAnimation, new PropertyPath("RenderTransform.ScaleX"));
                Storyboard.SetTargetProperty(myHeightAnimation, new PropertyPath(Border.HeightProperty));
            }
            {
                //ScaleTransform scale1 = new ScaleTransform(1, 1);

                //InputDialog.RenderTransformOrigin = new Point(0.5, 1);
                //InputDialog.RenderTransform = scale1;



                DoubleAnimation myHeightAnimation = new DoubleAnimation();
                myHeightAnimation.From = 400.0;
                myHeightAnimation.To = 200.0;
                myHeightAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(150));

                //DoubleAnimation myWidthAnimation = new DoubleAnimation();
                //myWidthAnimation.From = 1.0;
                //myWidthAnimation.To = 0.5;
                //myWidthAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(150));

                DoubleAnimation myOpacityAnimation = new DoubleAnimation();
                myOpacityAnimation.From = 1.0;
                myOpacityAnimation.To = 0.0;
                myOpacityAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(150));

                CloseStroyBoard = new Storyboard();
                CloseStroyBoard.Children.Add(myOpacityAnimation);
                //CloseStroyBoard.Children.Add(myWidthAnimation);
                CloseStroyBoard.Children.Add(myHeightAnimation);
                Storyboard.SetTargetName(myOpacityAnimation, EditWindow.Name);
                //Storyboard.SetTargetName(myWidthAnimation, InputDialog.Name);
                Storyboard.SetTargetName(myHeightAnimation, InputDialog.Name);
                Storyboard.SetTargetProperty(myOpacityAnimation, new PropertyPath(Border.OpacityProperty));
                //Storyboard.SetTargetProperty(myHeightAnimation, new PropertyPath("RenderTransform.ScaleY"));
                //Storyboard.SetTargetProperty(myWidthAnimation, new PropertyPath("RenderTransform.ScaleX"));
                Storyboard.SetTargetProperty(myHeightAnimation, new PropertyPath(Border.HeightProperty));

                CloseStroyBoard.Completed += (object sender, EventArgs e) =>
                {
                    EditWindow.Visibility = Visibility.Hidden;
                };
            }
        }

        private void MainListBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //switch (e.Key)
            //{
            //    case Key.U:
            //        UpFunc();
            //        break;
            //    case Key.O:
            //        DownFunc();
            //        break;
            //}

        }

        public bool IsOpenTEP = false;

        public Rect GetRect()
        {
            Rect rect = new Rect(trigEditPlus.Left, trigEditPlus.Top, trigEditPlus.ActualWidth, trigEditPlus.ActualHeight);

            return rect;
        }

        public TrigEditPlus trigEditPlus;
        private void OpenTrigEditPlus_Click(object sender, RoutedEventArgs e)
        {
            //mapEditor.DisableWindow();
            trigEditPlus = new TrigEditPlus(mapEditor, this);
            trigEditPlus.Show();

            PopupBackground.Visibility = Visibility.Visible;
            IsOpenTEP = true;
        }

        public void CloseTrigEditPlus()
        {
            PopupBackground.Visibility = Visibility.Collapsed;
            IsOpenTEP = false;
            if(trigEditPlus != null)
                try
                {
                    trigEditPlus.Close();
                }
                catch (Exception)
                {
                }
            
        }


        private void PopupClose_BtnClick(object sender, RoutedEventArgs e)
        {
            trigEditPlus.Close();
            PopupBackground.Visibility = Visibility.Collapsed;
            IsOpenTEP = false;
        }
    }
}
