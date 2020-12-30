using Data.Map;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UseMapEditor.DataBinding;
using static UseMapEditor.Control.MapEditor;

namespace UseMapEditor.Control.MapEditorControl
{
    /// <summary>
    /// CodeSelecter.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CodeSelecter : UserControl
    {
        private MapEditor mapEditor;



        private Codetype codetype;





        public CodeSelecter()
        {
            InitializeComponent();
        }



        private ViewType viewType;
        public enum ViewType
        {
            Num,
            Alpha,
            Tree
        }


        public void SetCodeType(Codetype _codetype, MapEditor _mapEditor)
        {
            mapEditor = _mapEditor;
            codetype = _codetype;


            ListCreate();
        }



        List<TreeViewItem> treeViews = new List<TreeViewItem>();
        public void ListCreate()
        {
            MainTreeview.Items.Clear();
            treeViews.Clear();
            switch (codetype)
            {
                case Codetype.Unit:
                    MainListbox.ItemsSource = mapEditor.mapDataBinding.unitdataBindings;
                    for (int i = 0; i < 228; i++)
                    {
                        CreateTreeviewItem(Global.WindowTool.unitgroup[i], mapEditor.mapDataBinding.unitdataBindings[i], i);
                    }
                    break;
                case Codetype.Upgrade:
                    MainListbox.ItemsSource = mapEditor.mapDataBinding.upgradeDataBindings;
                    for (int i = 0; i < 61; i++)
                    {
                        CreateTreeviewItem(Global.WindowTool.upgradegroup[i], mapEditor.mapDataBinding.upgradeDataBindings[i], i);
                    }
                    break;
                case Codetype.Tech:
                    MainListbox.ItemsSource = mapEditor.mapDataBinding.techDataBindings;
                    for (int i = 0; i < 44; i++)
                    {
                        CreateTreeviewItem(Global.WindowTool.techgroup[i], mapEditor.mapDataBinding.techDataBindings[i], i);
                    }
                    break;
            }

            

            ListRefresh();
        }




        private void CreateTreeviewItem(string path, object databinding, int objid)
        {
            string[] paths = path.Split('\\');

            ItemCollection itemCollection = MainTreeview.Items;
            for (int i = 0; i < paths.Length; i++)
            {
                if (paths[i] == "")
                    continue;

                bool IsExist = false;
                foreach (TreeViewItem item in itemCollection)
                {
                    string header = (string)item.Tag;
                    if(header == paths[i])
                    {
                        IsExist = true;
                        itemCollection = item.Items;
                        break;
                    }
                }
                if (!IsExist)
                {
                    //존재하지 않을 경우 새로 만들기
                    TreeViewItem treeViewItem = new TreeViewItem();
                    treeViewItem.Header = paths[i];
                    treeViewItem.Tag = paths[i];

                    itemCollection.Add(treeViewItem);


                    itemCollection = treeViewItem.Items;
                }
            }


            {
                TreeViewItem treeViewItem = new TreeViewItem();
                treeViewItem.DataContext = databinding;
                treeViewItem.Header = GetDockPanel();
                treeViewItem.Tag = objid.ToString();
                itemCollection.Add(treeViewItem);

                treeViews.Add(treeViewItem);
            }
        }


        private Grid GetDockPanel()
        {
            //<DataTemplate x:Key="ListItem">
            //    <Grid x:Name="itemGrid" Margin="-12,-8,0,-8" MinHeight="32">
            //        <DockPanel Margin="12,8">
            //            <Border DockPanel.Dock="Left" Height="{Binding ElementName=itemGrid, Path=ActualHeight}" Width="32" Margin="-6,-8,0,-8" Background="Black">
            //                <Image Source="{Binding ImageIcon}"/>
            //            </Border>
            //            <StackPanel Margin="8,0,0,0">
            //                <TextBlock Foreground="{DynamicResource PrimaryHueMidBrush}" Text="{Binding MainName, UpdateSourceTrigger=PropertyChanged, Mode=TwoWay}" FontSize="{Binding MainNameSize, UpdateSourceTrigger=PropertyChanged, Mode=TwoWay}"/>
            //                <TextBlock Text="{Binding SecondName, UpdateSourceTrigger=PropertyChanged, Mode=TwoWay}" Visibility="{Binding SecondNameVisble, UpdateSourceTrigger=PropertyChanged, Mode=TwoWay}" FontSize="15"/>
            //            </StackPanel>
            //        </DockPanel>
            //    </Grid>
            //</DataTemplate>

            Grid grid = new Grid();

            grid.Name = "itemGrid";
            grid.Margin = new Thickness(-12, -8, 0, -8);
            grid.MinHeight = 32;
              

            DockPanel dockPanel = new DockPanel();
            dockPanel.Margin = new Thickness(12, 8, 12, 8);
            {
                Border border = new Border();
                border.Margin = new Thickness(-8, -8, 0, -8);
                border.Width = 32;
                border.Background = Brushes.Black;
                DockPanel.SetDock(border, Dock.Left);
                {
                    Binding binding = new Binding();
                    binding.Path = new PropertyPath("ActualHeight");
                    binding.ElementName = "itemGrid";
                    border.SetBinding(Border.HeightProperty, binding);
                }

                Image image = new Image();
                {
                    Binding binding = new Binding();
                    binding.Path = new PropertyPath("ImageIcon");
                    image.SetBinding(Image.SourceProperty, binding);
                }
                border.Child = image;


                dockPanel.Children.Add(border);
            }

            StackPanel stackPanel = new StackPanel();
            stackPanel.Margin = new Thickness(8, 0, 0, 0);
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Foreground = (Brush)Application.Current.Resources["PrimaryHueMidBrush"];
                {
                    Binding binding = new Binding();
                    binding.Path = new PropertyPath("MainName");
                    binding.Mode = BindingMode.TwoWay;
                    binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    textBlock.SetBinding(TextBlock.TextProperty, binding);
                }
                {
                    Binding binding = new Binding();
                    binding.Path = new PropertyPath("MainNameSize");
                    binding.Mode = BindingMode.TwoWay;
                    binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    textBlock.SetBinding(TextBlock.FontSizeProperty, binding);
                }


                stackPanel.Children.Add(textBlock);
            }
            {
                TextBlock textBlock = new TextBlock();
                textBlock.FontSize = 15;
                {
                    Binding binding = new Binding();
                    binding.Path = new PropertyPath("SecondName");
                    binding.Mode = BindingMode.TwoWay;
                    binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    textBlock.SetBinding(TextBlock.TextProperty, binding);
                }
                {
                    Binding binding = new Binding();
                    binding.Path = new PropertyPath("SecondNameVisble");
                    binding.Mode = BindingMode.TwoWay;
                    binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    textBlock.SetBinding(TextBlock.VisibilityProperty, binding);
                }


                stackPanel.Children.Add(textBlock);
            }
            dockPanel.Children.Add(stackPanel);
            grid.Children.Add(dockPanel);


            return grid;
        }










        private void ListRefresh()
        {
            if (viewType == ViewType.Tree)
            {
                MainListbox.Visibility = Visibility.Collapsed;
                MainTreeview.Visibility = Visibility.Visible;
            }
            else
            {
                MainListbox.Visibility = Visibility.Visible;
                MainTreeview.Visibility = Visibility.Collapsed;

                MainListbox.Items.SortDescriptions.Clear();

                if (viewType == ViewType.Alpha)
                {
                    MainListbox.Items.SortDescriptions.Add(new SortDescription("AlphaName",
                              ListSortDirection.Ascending));
                }
                else
                {
                    MainListbox.Items.SortDescriptions.Add(new SortDescription("ObjectID",
                              ListSortDirection.Ascending));
                }
            }




            ListItemRefresh();
        }

        public void ListItemRefresh()
        {
            SearchBox.Text = "";
            string fliter = SearchBox.Text.ToLower();

            if (viewType == ViewType.Tree)
            {

            }
        }

        private bool TreeviewFliter(ItemCollection items, string searchText)
        {
            bool returnbool = false;
            for (int i = 0; i < items.Count; i++)
            {
                TreeViewItem titem = (TreeViewItem)items[i];

                if(titem.Items.Count == 0)
                {
                    //마지막 아이템일 경우
                    if(titem.Visibility == Visibility.Visible)
                    {
                        //아이템이 보일 경우
                        return true;
                    }
                }
                else
                {
                    bool rbool = TreeviewFliter(titem.Items, searchText);

                    if (rbool)
                    {
                        //안에 보이는 아이템이 하나 이상 있다는 뜻
                        titem.Visibility = Visibility.Visible;
                        if(searchText == "")
                        {
                            titem.IsExpanded = false;
                        }
                        else
                        {
                            titem.IsExpanded = true;
                        }
                        returnbool = true;
                    }
                    else
                    {
                        titem.Visibility = Visibility.Collapsed;
                        titem.IsExpanded = false;
                    }


                }
            }
            return returnbool;
        }
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text;
            if (viewType == ViewType.Tree)
            {
                for (int i = 0; i < treeViews.Count; i++)
                {
                    TreeViewItem list = treeViews[i];
                    string str = "";
                    switch (codetype)
                    {
                        case Codetype.Unit:
                            {
                                UnitDataBinding binding = (UnitDataBinding)list.DataContext;
                                if (binding != null)
                                {
                                    str = binding.AlphaName;
                                }
                            }
                            break;
                        case Codetype.Upgrade:
                            {
                                UpgradeDataBinding binding = (UpgradeDataBinding)list.DataContext;
                                if (binding != null)
                                {
                                    str = binding.AlphaName;
                                }
                            }
                            break;
                        case Codetype.Tech:
                            {
                                TechDataBinding binding = (TechDataBinding)list.DataContext;
                                if (binding != null)
                                {
                                    str = binding.AlphaName;
                                }
                            }
                            break;
                    }


                    if (!String.IsNullOrEmpty(str))
                    {
                        int index = str.IndexOf(searchText, 0);

                        if(index > -1)
                        {
                            //보이게하기
                            list.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            //안보이게하기
                            list.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        //안보이게하기
                        list.Visibility = Visibility.Collapsed;
                    }
                }


                TreeviewFliter(MainTreeview.Items, searchText);
            }
            else
            {
                MainListbox.Items.Filter = delegate (object obj)
                {
                    UnitDataBinding unitdata = (UnitDataBinding)obj;
                    string str = unitdata.AlphaName;
                    if (String.IsNullOrEmpty(str)) return false;
                    int index = str.IndexOf(searchText, 0);

                    return (index > -1);
                };
            }

            //ListItemRefresh();
        }

        private void NumBtn_Click(object sender, RoutedEventArgs e)
        {
            viewType = ViewType.Num;
            ListRefresh();
        }

        private void AlphaBtn_Click(object sender, RoutedEventArgs e)
        {
            viewType = ViewType.Alpha;
            ListRefresh();
        }

        private void TreeBtn_Click(object sender, RoutedEventArgs e)
        {
            viewType = ViewType.Tree;
            ListRefresh();
        }





        public event EventHandler SelectionChanged;
        private void MainListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (viewType != ViewType.Tree)
            {
                int objid = -1;

                switch (codetype)
                {
                    case Codetype.Unit:
                        {
                            UnitDataBinding list = (UnitDataBinding)MainListbox.SelectedItem;
                            if (list != null)
                            {
                                objid = list.ObjectID;
                            }
                        }
                        break;
                    case Codetype.Upgrade:
                        {
                            UpgradeDataBinding list = (UpgradeDataBinding)MainListbox.SelectedItem;
                            if (list != null)
                            {
                                objid = list.ObjectID;
                            }
                        }
                        break;
                    case Codetype.Tech:
                        {
                            TechDataBinding list = (TechDataBinding)MainListbox.SelectedItem;
                            if (list != null)
                            {
                                objid = list.ObjectID;
                            }
                        }
                        break;
                }


                SelectionChanged.Invoke(objid, e);
            }
        }

        private void MainTreeview_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (viewType == ViewType.Tree)
            {
                int objid = -1;


                TreeViewItem list = (TreeViewItem)MainTreeview.SelectedItem;
                if (list != null)
                {
                    if(list.Items.Count == 0)
                    {
                        switch (codetype)
                        {
                            case Codetype.Unit:
                                {
                                    UnitDataBinding binding = (UnitDataBinding)list.DataContext;
                                    if (binding != null)
                                    {
                                        objid = binding.ObjectID;
                                    }
                                }
                                break;
                            case Codetype.Upgrade:
                                {
                                    UpgradeDataBinding binding = (UpgradeDataBinding)list.DataContext;
                                    if (binding != null)
                                    {
                                        objid = binding.ObjectID;
                                    }
                                }
                                break;
                            case Codetype.Tech:
                                {
                                    TechDataBinding binding = (TechDataBinding)list.DataContext;
                                    if (binding != null)
                                    {
                                        objid = binding.ObjectID;
                                    }
                                }
                                break;
                        }
                    }
                }

                SelectionChanged.Invoke(objid, e);
            }
        }
    }
}
