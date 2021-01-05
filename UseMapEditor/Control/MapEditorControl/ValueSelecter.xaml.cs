using System;
using System.Collections.Generic;
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
using UseMapEditor.FileData;
using static Data.Map.MapData;

namespace UseMapEditor.Control.MapEditorControl
{
    /// <summary>
    /// ValueSelecter.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ValueSelecter : UserControl
    {
        public event RoutedEventHandler SelectEvent;
        public event RoutedEventHandler CloseEvent;

        public MapEditor mapEditor;

        public void SetMapEditor(MapEditor mapEditor)
        {
            this.mapEditor = mapEditor;
            CodeSelecter.SetCodeType(Data.Map.MapData.Codetype.Unit, mapEditor, true);
        }

        public ValueSelecter()
        {
            InitializeComponent();

            CodeSelecter.SelectionChanged += CodeSelecter_SelectionChanged;
        }

        private void CodeSelecter_SelectionChanged(object sender, EventArgs e)
        {
            if (IsLoadCmp)
            {
                int rval = (int)sender;
                if(rval == -1)
                {
                    return;
                }

                CurrentArg.VALUE = rval;
                CurrentArg.IsInit = false;
                CloseEvent.Invoke(sender, new RoutedEventArgs());
            }
        }

        private Arg CurrentArg;
        private bool IsLoadCmp;
        public void OpenValueSelecter(Arg arg)
        {
            CurrentArg = arg;
            IsLoadCmp = false;
            DefaultPanel.Visibility = Visibility.Collapsed;
            CodeSelecterPanel.Visibility = Visibility.Collapsed;
            StringTextBox.Visibility = Visibility.Collapsed;
            ValueInput.Visibility = Visibility.Collapsed;
            UPUS.Visibility = Visibility.Collapsed;
            SearchBox.Text = "";


            TriggerManger tm = Global.WindowTool.triggerManger;
            if (tm.IsArgParseable(arg.ARGTYPE))
            {
                DefaultPanel.Visibility = Visibility.Visible;

                DefaultArgSelecter.Items.Clear();

                Dictionary<uint, string> args = tm.GetArgList(arg.ARGTYPE, true);

                uint[] keys = args.Keys.ToArray();
                for (int i = 0; i < keys.Length; i++)
                {
                    ListBoxItem listBoxItem = new ListBoxItem();
                    listBoxItem.Tag = keys[i];
                    listBoxItem.Content = args[keys[i]];



                    DefaultArgSelecter.Items.Add(listBoxItem);
                    if (arg.IsInit)
                    {
                        DefaultArgSelecter.SelectedIndex = -1;
                    }
                    else
                    {
                        if (arg.VALUE == keys[i])
                        {
                            DefaultArgSelecter.SelectedItem = listBoxItem;
                            DefaultArgSelecter.ScrollIntoView(listBoxItem);
                        }
                    }
                }

                if(keys.Length > 10)
                {
                    SearchBoxDockPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    SearchBoxDockPanel.Visibility = Visibility.Collapsed;
                }
            }
            else
            {

                switch (arg.ARGTYPE)
                {
                    case TriggerManger.ArgType.UNIT:
                        CodeSelecterPanel.Visibility = Visibility.Visible;

                        if(arg.VALUE < 228)
                        {
                            UnitType.SelectedIndex = 0;
                            CodeSelecter.Visibility = Visibility.Visible;
                            if (arg.IsInit)
                            {
                                CodeSelecter.SetIndex(-1);
                            }
                            else
                            {
                                CodeSelecter.SetIndex((int)arg.VALUE);
                            }
                        }
                        else
                        {
                            UnitType.SelectedIndex = (int)(arg.VALUE - 228);
                            CodeSelecter.Visibility = Visibility.Collapsed;
                        }
          
                        break;
                    case TriggerManger.ArgType.LOCATION:
                        DefaultPanel.Visibility = Visibility.Visible;
                        SearchBoxDockPanel.Visibility = Visibility.Visible;
                        DefaultArgSelecter.Items.Clear();
                        for (int i = 1; i < mapEditor.mapdata.LocationDatas.Count; i++)
                        {
                            ListBoxItem listBoxItem = new ListBoxItem();
                            listBoxItem.Tag = mapEditor.mapdata.LocationDatas[i];
                            listBoxItem.Content = mapEditor.mapdata.LocationDatas[i].STRING.String;


                            DefaultArgSelecter.Items.Add(listBoxItem);
                        }
                        DefaultArgSelecter.SelectedIndex = -1;
                        break;
                    case TriggerManger.ArgType.WAV:
                        DefaultPanel.Visibility = Visibility.Visible;
                        SearchBoxDockPanel.Visibility = Visibility.Visible;
                        DefaultArgSelecter.Items.Clear();
                        for (int i = 0; i < mapEditor.mapdata.WAV.Length; i++)
                        {
                            string d = mapEditor.mapdata.WAV[i].String;
                            if (mapEditor.mapdata.WAV[i].IsLoaded)
                            {
                                ListBoxItem listBoxItem = new ListBoxItem();
                                listBoxItem.Tag = d;
                                listBoxItem.Content = d;

                                DefaultArgSelecter.Items.Add(listBoxItem);
                            }        
                        }
                        DefaultArgSelecter.SelectedIndex = -1;
                        break;
                    case TriggerManger.ArgType.SWITCH:
                        DefaultPanel.Visibility = Visibility.Visible;
                        SearchBoxDockPanel.Visibility = Visibility.Visible;
                        DefaultArgSelecter.Items.Clear();
                        for (int i = 0; i < mapEditor.mapdata.SWNM.Length; i++)
                        {
                            ListBoxItem listBoxItem = new ListBoxItem();
                            listBoxItem.Tag = (uint)i;

                            string d = mapEditor.mapdata.SWNM[i].String;
                            if (mapEditor.mapdata.SWNM[i].IsLoaded)
                            {
                                listBoxItem.Content = d;
                            }
                            else
                            {
                                listBoxItem.Content = "스위치 " + (i + 1);
                            }



                            DefaultArgSelecter.Items.Add(listBoxItem);
                        }
                        DefaultArgSelecter.SelectedIndex = -1;

                        break;
                    case TriggerManger.ArgType.STRING:
                        StringTextBox.Visibility = Visibility.Visible;
                        StringText.Text = arg.STRING.String;
                        break;
                    case TriggerManger.ArgType.VALUE:
                        ValueInput.Visibility = Visibility.Visible;
                        AllCheckBox.Visibility = Visibility.Collapsed;
                        ValueText.IsEnabled = true;
                        ValueText.Text = arg.VALUE.ToString();
                        break;
                    case TriggerManger.ArgType.COUNT:
                        ValueInput.Visibility = Visibility.Visible;
                        AllCheckBox.Visibility = Visibility.Visible;
                        if(arg.VALUE == 0)
                        {
                            AllCheckBox.IsChecked = true;
                            ValueText.IsEnabled = false;
                            ValueText.Text = "0";
                        }
                        else
                        {
                            AllCheckBox.IsChecked = false;
                            ValueText.IsEnabled = true;
                            ValueText.Text = arg.VALUE.ToString();
                        }

                        break;
                    case TriggerManger.ArgType.UPRP:
                        UPUS.Visibility = Visibility.Visible;
                        HPTextbox.Text = arg.UPRP.HITPOINT.ToString();
                        ShildTextbox.Text = arg.UPRP.SHIELDPOINT.ToString();
                        EnergyTextbox.Text = arg.UPRP.ENERGYPOINT.ToString();
                        ResourceTextbox.Text = arg.UPRP.RESOURCE.ToString();
                        HangerTextbox.Text = arg.UPRP.HANGAR.ToString();

                        ushort status = arg.UPRP.STATUSFLAG;

                        ClackeCb.IsChecked = (status & (0b1 << 0)) > 0;
                        BurrowCb.IsChecked = (status & (0b1 << 1)) > 0;
                        LiftCb.IsChecked = (status & (0b1 << 2)) > 0;
                        HallCb.IsChecked = (status & (0b1 << 3)) > 0;
                        InviCb.IsChecked = (status & (0b1 << 4)) > 0;


                        ushort pointval = arg.UPRP.POINTVALID;
                        HPDefault.IsChecked = (pointval & (0b1 << 1)) > 0;
                        ShildDefault.IsChecked = (pointval & (0b1 << 2)) > 0;
                        EnergyDefault.IsChecked = (pointval & (0b1 << 3)) > 0;
                        ResourceDefault.IsChecked = (pointval & (0b1 << 4)) > 0;
                        HangerDefault.IsChecked = (pointval & (0b1 << 5)) > 0;


                        ushort statusval = arg.UPRP.STATUSVALID;
                        Clackefault.IsChecked = (statusval & (0b1 << 0)) > 0;
                        BurrowDefault.IsChecked = (statusval & (0b1 << 1)) > 0;
                        LiftDefault.IsChecked = (statusval & (0b1 << 2)) > 0;
                        HallDefault.IsChecked = (statusval & (0b1 << 3)) > 0;
                        InviDefault.IsChecked = (statusval & (0b1 << 4)) > 0;

                        break;
                }

            }

            IsLoadCmp = true;
        }


        public void CloseValueSelecter()
        {

        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsLoadCmp)
            {
                string searchText = SearchBox.Text;

                DefaultArgSelecter.Items.Filter = delegate (object obj)
                {
                    ListBoxItem listitem = (ListBoxItem)obj;
                    string str = (string)listitem.Content;
                    if (String.IsNullOrEmpty(str)) return false;
                    int index = str.IndexOf(searchText, 0);

                    return (index > -1);
                };
            }
        }

        private void DefaultArgSelecter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoadCmp)
            {
                ListBoxItem listBoxItem = (ListBoxItem)DefaultArgSelecter.SelectedItem;

                if(listBoxItem == null)
                {
                    return;
                }

                if (CurrentArg.ARGTYPE == TriggerManger.ArgType.LOCATION)
                {
                    LocationData locdata = (LocationData)listBoxItem.Tag;

                    CurrentArg.LOCATION = locdata;
                    CurrentArg.IsInit = false;

                    CloseEvent.Invoke(sender, e);
                }
                else if(CurrentArg.ARGTYPE == TriggerManger.ArgType.WAV)
                {
                    string v = (string)listBoxItem.Tag;

                    CurrentArg.STRING.String = v;
                    CurrentArg.IsInit = false;

                    CloseEvent.Invoke(sender, e);
                }
                else
                {
                    uint v = (uint)listBoxItem.Tag;

                    CurrentArg.VALUE = v;
                    CurrentArg.IsInit = false;

                    CloseEvent.Invoke(sender, e);
                }

            }
        }

        private void StringText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsLoadCmp)
            {
                CurrentArg.STRING.String = StringText.Text;
                CurrentArg.IsInit = false;
                SelectEvent.Invoke(sender, e);
            }
        }

        private void ValueText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsLoadCmp)
            {
                uint v;

                if(uint.TryParse(ValueText.Text, out v))
                {
                    CurrentArg.VALUE = v;
                    CurrentArg.IsInit = false;
                    SelectEvent.Invoke(sender, e);
                }
            }
        }

        private void AllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (IsLoadCmp)
            {
                CurrentArg.VALUE = 0;
                CurrentArg.IsInit = false;
                ValueText.IsEnabled = false;
                SelectEvent.Invoke(sender, e);
            }
        }

        private void AllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (IsLoadCmp)
            {
                ValueText.IsEnabled = true;
                ValueText.Text = "1";
                CurrentArg.VALUE = 1;
                CurrentArg.IsInit = false;
                SelectEvent.Invoke(sender, e);
            }
        }

        private void Default_Checked(object sender, RoutedEventArgs e)
        {
            StatusFlagChange();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            StatusFlagChange();
        }

        private void StatusFlagChange()
        {
            if (IsLoadCmp)
            {
                CurrentArg.IsInit = false;
                {
                    byte v;
                    if (byte.TryParse(HPTextbox.Text, out v))
                    {
                        CurrentArg.UPRP.HITPOINT = v;
                    }
                    if (byte.TryParse(ShildTextbox.Text, out v))
                    {
                        CurrentArg.UPRP.SHIELDPOINT = v;
                    }
                    if (byte.TryParse(EnergyTextbox.Text, out v))
                    {
                        CurrentArg.UPRP.ENERGYPOINT = v;
                    }
                }

                {
                    uint v;
                    if (uint.TryParse(ResourceTextbox.Text, out v))
                    {
                        CurrentArg.UPRP.RESOURCE = v;
                    }
                }

                {
                    ushort v;
                    if (ushort.TryParse(HangerTextbox.Text, out v))
                    {
                        CurrentArg.UPRP.HANGAR = v;
                    }
                }



                ushort status = 0;
                status = FlagCalc((bool)ClackeCb.IsChecked, status, 0);
                status = FlagCalc((bool)BurrowCb.IsChecked, status, 1);
                status = FlagCalc((bool)LiftCb.IsChecked, status, 2);
                status = FlagCalc((bool)HallCb.IsChecked, status, 3);
                status = FlagCalc((bool)InviCb.IsChecked, status, 4);
                CurrentArg.UPRP.STATUSFLAG = status;



                ushort pointval = 0;
                pointval = FlagCalc((bool)HPDefault.IsChecked, pointval, 1);
                pointval = FlagCalc((bool)ShildDefault.IsChecked, pointval, 2);
                pointval = FlagCalc((bool)EnergyDefault.IsChecked, pointval, 3);
                pointval = FlagCalc((bool)ResourceDefault.IsChecked, pointval, 4);
                pointval = FlagCalc((bool)HangerDefault.IsChecked, pointval, 5);
                CurrentArg.UPRP.POINTVALID = pointval;


                ushort statusval = 0;
                statusval = FlagCalc((bool)Clackefault.IsChecked, statusval, 0);
                statusval = FlagCalc((bool)BurrowDefault.IsChecked, statusval, 1);
                statusval = FlagCalc((bool)LiftDefault.IsChecked, statusval, 2);
                statusval = FlagCalc((bool)HallDefault.IsChecked, statusval, 3);
                statusval = FlagCalc((bool)InviDefault.IsChecked, statusval, 4);
                CurrentArg.UPRP.STATUSVALID = statusval;
            }
        }

        private ushort FlagCalc(bool val, ushort input, int bit)
        {
            if (val)
            {
                return ((ushort)(input | (0b1 << bit)));
            }
            else
            {
                input  &= unchecked((ushort)~(0b1 << bit));
            }
            return input;
        }





        private void UnitType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoadCmp)
            {
                int selectindex = UnitType.SelectedIndex;

                if (selectindex > 0)
                {
                    CodeSelecter.Visibility = Visibility.Collapsed;
                    uint v = (uint)(selectindex + 228);

                    CurrentArg.VALUE = v;
                    CurrentArg.IsInit = false;

                    CloseEvent.Invoke(sender, e);
                }
                else if (selectindex == 0)
                {
                    CodeSelecter.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
