﻿using System;
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

namespace UseMapEditor.Control.MapEditorControl
{
    /// <summary>
    /// ScenarioControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ScenarioControl : UserControl
    {
        public int SelectedIndex
        {
            get
            {
                return scenTabControl.SelectedIndex;
            }
        }


        public ScenarioControl()
        {
            InitializeComponent();
        }

        private MapEditor mapEditor;


        public MapSetting mapSetting = new MapSetting();
        public PlayerSettingPanel playerSetting = new PlayerSettingPanel();
        public ForceSettingPanel forceSetting = new ForceSettingPanel();
        public UnitSetting unitSetting = new UnitSetting();
        public UpgradeSetting upgradeSetting = new UpgradeSetting();

        public TechSetting techSetting = new TechSetting();
        public SoundSetting soundSetting = new SoundSetting();
        public StringSetting stringSetting = new StringSetting();
        public TriggerEditor classTriggerEditor = new TriggerEditor();
        public TriggerEditor brinfingTriggerEditor = new TriggerEditor();

        public ChkSetting chkSetting = new ChkSetting();

        List<DialogableControlPanel> dialogableControlPanels = new List<DialogableControlPanel>();

        public void SwitchTab(int index)
        {
            scenTabControl.SelectedIndex = index;
        }

        public void OpenPopupWindow(int index)
        {
            dialogableControlPanels[index].PopupWindowOpen();
        }

        public bool CheckWindowPos(Point p)
        {
            if(mapEditor == null)
            {
                return false;
            }
            else
            {
                if(!mapEditor.IsLoad)
                {
                    return false;
                }
            }

            p = mapEditor.PointToScreen(p);
            p.Y += 34;
            //
            string tt = "(" + p.ToString() + ")";

            foreach (var item in dialogableControlPanels)
            {
                if (item.IsPopup)
                {
                    Rect tp = item.GetRect();
                    if(tp.Left <= p.X && p.X <= tp.Right &&
                        tp.Top <= p.Y && p.Y <= tp.Bottom)
                    {
                        return true;
                    }
                }
            }



            if (classTriggerEditor.IsOpenTEP)
            {
                Rect tp = classTriggerEditor.GetRect();
                if (tp.Left <= p.X && p.X <= tp.Right &&
                    tp.Top <= p.Y && p.Y <= tp.Bottom)
                {
                    return true;
                }
            }



            //mapEditor.BottomExpander.Header = tt;

            return false;
        }



        public void AllWindowClose()
        {
            classTriggerEditor.CloseTrigEditPlus();

            foreach (var item in dialogableControlPanels)
            {
                item.PopupWindowClose();
            }
        }


        private void TabItemAdd(UserControl userControl, string header)
        {
            DialogableControlPanel dialogableControlPanel = new DialogableControlPanel(userControl);

            dialogableControlPanels.Add(dialogableControlPanel);

            TabItem tabItem = new TabItem();
            tabItem.Content = dialogableControlPanel;
            tabItem.Header = header;
            scenTabControl.Items.Add(tabItem);
        }
        public void Init(MapEditor mapEditor)
        {
            dialogableControlPanels.Clear();
            this.mapEditor = mapEditor;

            mapSetting.SetMapEditor(mapEditor);
            playerSetting.SetMapEditor(mapEditor);
            forceSetting.SetMapEditor(mapEditor);
            unitSetting.SetMapEditor(mapEditor);
            upgradeSetting.SetMapEditor(mapEditor);
            techSetting.SetMapEditor(mapEditor);
            soundSetting.SetMapEditor(mapEditor);
            stringSetting.SetMapEditor(mapEditor);
            classTriggerEditor.SetMapEditor(mapEditor, true);
            brinfingTriggerEditor.SetMapEditor(mapEditor, false);
            chkSetting.SetMapEditor(mapEditor);


            TabItemAdd(mapSetting, "맵");
            TabItemAdd(playerSetting, "플레이어");
            TabItemAdd(forceSetting, "세력");
            TabItemAdd(unitSetting, "유닛");
            TabItemAdd(upgradeSetting, "업그레이드");
            TabItemAdd(techSetting, "테크");
            TabItemAdd(soundSetting, "사운드");
            TabItemAdd(stringSetting, "스트링");
            TabItemAdd(classTriggerEditor, "트리거");
            TabItemAdd(brinfingTriggerEditor, "브리핑");
            TabItemAdd(chkSetting, "데이터");
        }


        private int lasttabindex;
        private void TabablzControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int sindex = SelectedIndex;
            if (lasttabindex != sindex)
            {
                lasttabindex = sindex;
                if (sindex == 2)
                {
                    forceSetting.MainListRefresh();
                }
                if (sindex == 7)
                {
                    stringSetting.MainListRefresh();
                }
            }
        }
    }
}
