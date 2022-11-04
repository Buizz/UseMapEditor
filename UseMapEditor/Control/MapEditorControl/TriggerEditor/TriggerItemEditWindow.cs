using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using UseMapEditor.FileData;

namespace UseMapEditor.Control.MapEditorControl
{
    public partial class TriggerEditor : UserControl
    {

        private TrigItem SelectTrigitem;
        private TrigItem CopyedSelectTrigitem;
        private bool TrigEditWindowOpen;
        private bool IsNewTrigEdit;
        private void OpenTrigItemEditWindow(TrigItem trigItem, bool IsAction = false)
        {
            ItemEditPage.Visibility = Visibility.Visible;
            EditOpenStroyBoard.Begin(this);

            ItemTypeSelecter.Visibility = Visibility.Collapsed;

            SelectTrigitem = trigItem;
            if (SelectTrigitem == null)
            {
                IsNewTrigEdit = true;
                CopyedSelectTrigitem = null;
                OpenTypeSelecter(SelectTrigitem, IsAction);
            }
            else
            {
                ActionName.Text = SelectTrigitem.triggerDefine.NAME;
                IsNewTrigEdit = false;
                CopyedSelectTrigitem = SelectTrigitem.Clone();
                RefreshItem(CopyedSelectTrigitem);
            }


            TrigEditWindowOpen = true;
        }



        private void RefreshItem(TrigItem trigItem)
        {
            TrigArgs.Children.Clear();

            TriggerManger.TriggerDefine td = trigItem.triggerDefine;
            string summary = td.SUMMARY;

            for (int i = 0; i < td.argDefines.Count; i++)
            {
                string argname = "[" + td.argDefines[i].argname + "]";



                summary = summary.Replace(argname, "☻$" + i + "☻");
            }

            string[] block = summary.Split('☻');

            for (int i = 0; i < block.Length; i++)
            {
                if(block[i].Length == 0)
                {
                    continue;
                }

                if(block[i][0] == '$')
                {
                    string argcheck = block[i].Substring(1);

                    int argnum;

                    if(int.TryParse(argcheck, out argnum))
                    {
                        Button button = new Button();
                        button.Style = (Style)Application.Current.Resources["MaterialDesignOutlinedButton"];
                        button.Content = trigItem.args[argnum].GetValue;
                        button.Tag = trigItem.args[argnum];

                        button.Click += ArgEditBtn;


                        TrigArgs.Children.Add(button);
                        continue;
                    }
                }


                TextBlock textBlock = new TextBlock();
                textBlock.Text = block[i];
                textBlock.VerticalAlignment = VerticalAlignment.Center;


                TrigArgs.Children.Add(textBlock);
            }
        }


        private void ArgEditBtn(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            Point point = button.TransformToAncestor(MainGrid).Transform(new Point(0, 0));



            int xpos = (int)Math.Min(MainGrid.ActualWidth, point.X + ValueSelecter.Width) - (int)ValueSelecter.Width;

            Arg arg = (Arg)button.Tag;




            ValueSelecter.Margin = new Thickness(xpos, point.Y + button.ActualHeight, 0, 0);
            OpenValueSelecter(arg, button);
        }


        private Button OpenedButton;
        private void ValueSelecterControl_SelectEvent(object sender, RoutedEventArgs e)
        {
            Button button = OpenedButton;
            Arg arg = (Arg)button.Tag;
            button.Content = arg.GetValue;
        }
        private void ValueSelecterControl_CloseEvent(object sender, RoutedEventArgs e)
        {
            Button button = OpenedButton;
            Arg arg = (Arg)button.Tag;
            button.Content = arg.GetValue;

            CloseValueSelecter();
        }
        private void OpenValueSelecter(Arg arg, Button button)
        {
            OpenedButton = button;
            ValueSelecter.Visibility = Visibility.Visible;
            ValueSelecterControl.OpenValueSelecter(arg);
        }
        private void CloseValueSelecter()
        {
            ValueSelecter.Visibility = Visibility.Collapsed;
            ValueSelecterControl.CloseValueSelecter();
        }



        private void CloseTrigItemEditWindow()
        {
            CloseValueSelecter();
            EditCloseStroyBoard.Begin(this);
            TrigEditWindowOpen = false;
        }


        private void TrigEditOkayBtn()
        {
            if (CopyedSelectTrigitem == null)
            {
                SnackbarMessage.Enqueue("취소할 수 없습니다.");
                return;
            }

            for (int i = 0; i < CopyedSelectTrigitem.args.Count; i++)
            {
                if (CopyedSelectTrigitem.args[i].IsInit)
                {
                    SnackbarMessage.Enqueue("모든 인자를 선택하세요.");
                    return;
                }
            }

            if (IsNewTrigEdit)
            {
                //새로 연 에딧
                //TriggerItemListbox
                //currentList

                if (TriggerItemListbox.SelectedItems.Count == 0)
                {
                    currentList.Add(CopyedSelectTrigitem);
                }
                else
                {
                    currentList.Insert(TriggerItemListbox.SelectedIndex, CopyedSelectTrigitem);
                }
            }
            else
            {
                //이미 있던 것
                SelectTrigitem.args.Clear();
                for (int i = 0; i < CopyedSelectTrigitem.args.Count; i++)
                {
                    SelectTrigitem.args.Add(CopyedSelectTrigitem.args[i].Clone());
                }
                SelectTrigitem.type = CopyedSelectTrigitem.type;
                SelectTrigitem.triggerDefine = CopyedSelectTrigitem.triggerDefine;

                SelectTrigitem.PropertyChangeAll();
            }


            CloseTrigItemEditWindow();
        }
        private void TrigEditOkayBtn_Click(object sender, RoutedEventArgs e)
        {
            TrigEditOkayBtn();
        }

        private void TrigEditCancelBtn_Click(object sender, RoutedEventArgs e)
        {
            CloseTrigItemEditWindow();
        }




        Storyboard EditOpenStroyBoard;
        Storyboard EditCloseStroyBoard;
        private void EditAnimationInit()
        {
            {
                ScaleTransform scale1 = new ScaleTransform(1, 1);

                ItemEditPage.RenderTransformOrigin = new Point(0.5, 0.5);
                ItemEditPage.RenderTransform = scale1;


                DoubleAnimation myHeightAnimation = new DoubleAnimation();
                myHeightAnimation.From = 0.5;
                myHeightAnimation.To = 1.0;
                myHeightAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(150));

                DoubleAnimation myWidthAnimation = new DoubleAnimation();
                myWidthAnimation.From = 0.5;
                myWidthAnimation.To = 1.0;
                myWidthAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(150));

                DoubleAnimation myOpacityAnimation = new DoubleAnimation();
                myOpacityAnimation.From = 0.0;
                myOpacityAnimation.To = 1.0;
                myOpacityAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(150));

                EditOpenStroyBoard = new Storyboard();
                EditOpenStroyBoard.Children.Add(myOpacityAnimation);
                EditOpenStroyBoard.Children.Add(myWidthAnimation);
                EditOpenStroyBoard.Children.Add(myHeightAnimation);
                Storyboard.SetTargetName(myOpacityAnimation, ItemEditPage.Name);
                Storyboard.SetTargetName(myHeightAnimation, ItemEditPage.Name);
                Storyboard.SetTargetName(myWidthAnimation, ItemEditPage.Name);
                Storyboard.SetTargetProperty(myOpacityAnimation, new PropertyPath(OpacityProperty));
                Storyboard.SetTargetProperty(myHeightAnimation, new PropertyPath("RenderTransform.ScaleY"));
                Storyboard.SetTargetProperty(myWidthAnimation, new PropertyPath("RenderTransform.ScaleX"));
            }
            {
                ScaleTransform scale1 = new ScaleTransform(1, 1);

                ItemEditPage.RenderTransformOrigin = new Point(0.5, 0.5);
                ItemEditPage.RenderTransform = scale1;



                DoubleAnimation myHeightAnimation = new DoubleAnimation();
                myHeightAnimation.From = 1.0;
                myHeightAnimation.To = 0.5;
                myHeightAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(150));

                DoubleAnimation myWidthAnimation = new DoubleAnimation();
                myWidthAnimation.From = 1.0;
                myWidthAnimation.To = 0.5;
                myWidthAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(150));

                DoubleAnimation myOpacityAnimation = new DoubleAnimation();
                myOpacityAnimation.From = 1.0;
                myOpacityAnimation.To = 0.0;
                myOpacityAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(150));

                EditCloseStroyBoard = new Storyboard();
                EditCloseStroyBoard.Children.Add(myOpacityAnimation);
                EditCloseStroyBoard.Children.Add(myWidthAnimation);
                EditCloseStroyBoard.Children.Add(myHeightAnimation);
                Storyboard.SetTargetName(myOpacityAnimation, ItemEditPage.Name);
                Storyboard.SetTargetName(myHeightAnimation, ItemEditPage.Name);
                Storyboard.SetTargetName(myWidthAnimation, ItemEditPage.Name);
                Storyboard.SetTargetProperty(myOpacityAnimation, new PropertyPath(OpacityProperty));
                Storyboard.SetTargetProperty(myHeightAnimation, new PropertyPath("RenderTransform.ScaleY"));
                Storyboard.SetTargetProperty(myWidthAnimation, new PropertyPath("RenderTransform.ScaleX"));

                EditCloseStroyBoard.Completed += (object sender, EventArgs e) =>
                {
                    ItemEditPage.Visibility = Visibility.Hidden;
                };
            }
        }
    }
}
