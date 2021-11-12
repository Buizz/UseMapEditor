using BondTech.HotKeyManagement.WPF._4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UseMapEditor.Control;
using static UseMapEditor.Global.Setting;

namespace UseMapEditor.Tools
{
    public class ShortCutManager
    {
        MapEditor mapEditor;

        public ShortCutManager(MapEditor mapEditor)
        {
            this.mapEditor = mapEditor;
            HotkeyInit();
        }

        private void MyHotKeyManager_LocalHotKeyPressed(object sender, LocalHotKeyEventArgs e)
        {
            if(!mapEditor.MapViewer.IsEnabled)
            {
                return;
            }
            switch (e.HotKey.Name)
            {
                case "Delete":
                    {
                        mapEditor.DeleteCommand();
                        break;
                    }
                case "Enter":
                    {
                        mapEditor.EditCommand();
                        break;
                    }
                case "Cut":
                    {
                        mapEditor.CutCommand();
                        break;
                    }
                case "Copy":
                    {
                        mapEditor.CopyCommand();
                        break;
                    }
                case "Paste":
                    {
                        mapEditor.PasteCommand();
                        break;
                    }
                case "New":
                    { mapEditor.mainWindow.NewMapCommand(); break; }
                case "Open":
                    { mapEditor.mainWindow.OpenMapCommand(); break; }
                case "Save":
                    { mapEditor.mainWindow.SaveMapCommand(); break; }
                case "Undo":
                    { mapEditor.taskManager.Undo(); break; }
                case "Redo":
                    { mapEditor.taskManager.Redo(); break; }
                //case "GridUp":
                //    { mapEditor.GridSizeUp(); break; }
                //case "GridDown":
                //    { mapEditor.GridSizeDown(); break; }
                case "CopyPaste":
                    { mapEditor.TabChange(MapEditor.Layer.CopyPaste); break; }
                case "Tileset":
                    { mapEditor.TabChange(MapEditor.Layer.Tile); break; }
                case "Doodad":
                    { mapEditor.TabChange(MapEditor.Layer.Doodad); break; }
                case "Unit":
                    { mapEditor.TabChange(MapEditor.Layer.Unit); break; }
                case "Sprite":
                    { mapEditor.TabChange(MapEditor.Layer.Sprite); break; }
                case "Location":
                    { mapEditor.TabChange(MapEditor.Layer.Location); break; }
                case "FogofWar":
                    { mapEditor.TabChange(MapEditor.Layer.FogOfWar); break; }

                case "Grid1":
                    { mapEditor.GridSizeChange(0); break; }
                case "Grid2":
                    { mapEditor.GridSizeChange(1); break; }
                case "Grid3":
                    { mapEditor.GridSizeChange(2); break; }
                case "Grid4":
                    { mapEditor.GridSizeChange(3); break; }

                case "GrpChange":
                    { mapEditor.NextgrpType(); break; }
                case "SDGrp":
                    { mapEditor.SetGrpType(MapEditor.DrawType.SD); break; }
                case "HDGrp":
                    { mapEditor.SetGrpType(MapEditor.DrawType.HD); break; }
                case "CBGrp":
                    { mapEditor.SetGrpType(MapEditor.DrawType.CB); break; }
                case "SystemDraw":
                    { mapEditor.ToggleSysdraw(); break; }
                case "mapSetting":
                case "playerSetting":
                case "forceSetting":
                case "unitSetting":
                case "upgradeSetting":
                case "techSetting":
                case "soundSetting":
                case "stringSetting":
                case "classTriggerEditor":
                case "brinfingTriggerEditor":
                    { mapEditor.ScenOpenCommand(e.HotKey.Name); break; }
                case "W":
                    {
                        if(Whoykey.WhenToRaise == RaiseLocalEvent.OnKeyDown)
                        {
                            mapEditor.key_WDown = true;
                            Whoykey.WhenToRaise = RaiseLocalEvent.OnKeyUp;
                        }
                        else
                        {
                            mapEditor.key_WDown = false;
                            Whoykey.WhenToRaise = RaiseLocalEvent.OnKeyDown;
                        }
                        break;
                    }
                case "A":
                    {
                        if (Ahoykey.WhenToRaise == RaiseLocalEvent.OnKeyDown)
                        {
                            mapEditor.key_ADown = true;
                            Ahoykey.WhenToRaise = RaiseLocalEvent.OnKeyUp;
                        }
                        else
                        {
                            mapEditor.key_ADown = false;
                            Ahoykey.WhenToRaise = RaiseLocalEvent.OnKeyDown;
                        }
                        break;
                    }
                case "S":
                    {
                        if (Shoykey.WhenToRaise == RaiseLocalEvent.OnKeyDown)
                        {
                            mapEditor.key_SDown = true;
                            Shoykey.WhenToRaise = RaiseLocalEvent.OnKeyUp;
                        }
                        else
                        {
                            mapEditor.key_SDown = false;
                            Shoykey.WhenToRaise = RaiseLocalEvent.OnKeyDown;
                        }
                        break;
                    }
                case "D":
                    {
                        if (Dhoykey.WhenToRaise == RaiseLocalEvent.OnKeyDown)
                        {
                            mapEditor.key_DDown = true;
                            Dhoykey.WhenToRaise = RaiseLocalEvent.OnKeyUp;
                        }
                        else
                        {
                            mapEditor.key_DDown = false;
                            Dhoykey.WhenToRaise = RaiseLocalEvent.OnKeyDown;
                        }
                        break;
                    }
            }
        }


        private HotKeyManager MyHotKeyManager;


        LocalHotKey Whoykey;
        LocalHotKey Ahoykey;
        LocalHotKey Shoykey;
        LocalHotKey Dhoykey;


        public void ResetShortCut()
        {
            foreach (var item in localHotKeys)
            {
                MyHotKeyManager.RemoveLocalHotKey(item);
            }

            foreach (var item in Global.Setting.ShortCutKeys)
            {
                string key = item.Key;
                ShortCutKey shortCutKey = item.Value;


                LocalHotKey hotkey = new LocalHotKey(key, shortCutKey.modifierKeys, shortCutKey.keys);
                MyHotKeyManager.AddLocalHotKey(hotkey);
                localHotKeys.Add(hotkey);
            }
        }


        List<LocalHotKey> localHotKeys = new List<LocalHotKey>();
        private void HotkeyInit()
        {
            MyHotKeyManager = new HotKeyManager(mapEditor.mainWindow);



            foreach (var item in Global.Setting.ShortCutKeys)
            {
                string key = item.Key;
                ShortCutKey shortCutKey = item.Value;


                LocalHotKey hotkey = new LocalHotKey(key, shortCutKey.modifierKeys, shortCutKey.keys);
                MyHotKeyManager.AddLocalHotKey(hotkey);
                localHotKeys.Add(hotkey);
            }


            {
                LocalHotKey hoykey = new LocalHotKey("Delete", Keys.Delete);
                MyHotKeyManager.AddLocalHotKey(hoykey);
            }
            {
                LocalHotKey hoykey = new LocalHotKey("Enter", Keys.Enter);
                MyHotKeyManager.AddLocalHotKey(hoykey);
            }
            {
                LocalHotKey hoykey = new LocalHotKey("Copy", ModifierKeys.Control, Keys.C);
                MyHotKeyManager.AddLocalHotKey(hoykey);
            }
            {
                LocalHotKey hoykey = new LocalHotKey("Paste", ModifierKeys.Control, Keys.V);
                MyHotKeyManager.AddLocalHotKey(hoykey);
            }
            {
                LocalHotKey hoykey = new LocalHotKey("Cut", ModifierKeys.Control, Keys.X);
                MyHotKeyManager.AddLocalHotKey(hoykey);
            }



            {
                Whoykey = new LocalHotKey("W", ModifierKeys.None, Keys.W, RaiseLocalEvent.OnKeyDown);
                MyHotKeyManager.AddLocalHotKey(Whoykey);
            }
            {
                Ahoykey = new LocalHotKey("A", ModifierKeys.None, Keys.A, RaiseLocalEvent.OnKeyDown);
                MyHotKeyManager.AddLocalHotKey(Ahoykey);
            }
            {
                Shoykey = new LocalHotKey("S", ModifierKeys.None, Keys.S, RaiseLocalEvent.OnKeyDown);
                MyHotKeyManager.AddLocalHotKey(Shoykey);
            }
            {
                Dhoykey = new LocalHotKey("D", ModifierKeys.None, Keys.D, RaiseLocalEvent.OnKeyDown);
                MyHotKeyManager.AddLocalHotKey(Dhoykey);
            }






            MyHotKeyManager.LocalHotKeyPressed += MyHotKeyManager_LocalHotKeyPressed;
        }
    }
}
