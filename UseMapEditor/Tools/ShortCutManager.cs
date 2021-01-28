using BondTech.HotKeyManagement.WPF._4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UseMapEditor.Control;

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
                case "GridUp":
                    { mapEditor.GridSizeUp(); break; }
                case "GridDown":
                    { mapEditor.GridSizeDown(); break; }
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
            }
        }


        private HotKeyManager MyHotKeyManager;

        private void HotkeyInit()
        {
            MyHotKeyManager = new HotKeyManager(mapEditor.mainWindow);

            {
                LocalHotKey hoykey = new LocalHotKey("New", ModifierKeys.Control, Keys.N);
                MyHotKeyManager.AddLocalHotKey(hoykey);
            }
            {
                LocalHotKey hoykey = new LocalHotKey("Open", ModifierKeys.Control, Keys.O);
                MyHotKeyManager.AddLocalHotKey(hoykey);
            }
            {
                LocalHotKey hoykey = new LocalHotKey("Save", ModifierKeys.Control, Keys.S);
                MyHotKeyManager.AddLocalHotKey(hoykey);
            }
            {
                LocalHotKey hoykey = new LocalHotKey("Undo", ModifierKeys.Control, Keys.Z);
                MyHotKeyManager.AddLocalHotKey(hoykey);
            }
            {
                LocalHotKey hoykey = new LocalHotKey("Redo", ModifierKeys.Control, Keys.U);
                MyHotKeyManager.AddLocalHotKey(hoykey);
            }
            {
                LocalHotKey hoykey = new LocalHotKey("GridUp", ModifierKeys.Control, Keys.Q);
                MyHotKeyManager.AddLocalHotKey(hoykey);
            }
            {
                LocalHotKey hoykey = new LocalHotKey("GridDown", ModifierKeys.Control, Keys.E);
                MyHotKeyManager.AddLocalHotKey(hoykey);
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
                LocalHotKey hoykey = new LocalHotKey("CopyPaste", ModifierKeys.Control, Keys.Oemtilde);
                MyHotKeyManager.AddLocalHotKey(hoykey);
            }
            {
                LocalHotKey hoykey = new LocalHotKey("Tileset", ModifierKeys.Control, Keys.D1);
                MyHotKeyManager.AddLocalHotKey(hoykey);
            }
            {
                LocalHotKey hoykey = new LocalHotKey("Doodad", ModifierKeys.Control, Keys.D2);
                MyHotKeyManager.AddLocalHotKey(hoykey);
            }
            {
                LocalHotKey hoykey = new LocalHotKey("Unit", ModifierKeys.Control, Keys.D3);
                MyHotKeyManager.AddLocalHotKey(hoykey);
            }
            {
                LocalHotKey hoykey = new LocalHotKey("Sprite", ModifierKeys.Control, Keys.D4);
                MyHotKeyManager.AddLocalHotKey(hoykey);
            }
            {
                LocalHotKey hoykey = new LocalHotKey("Location", ModifierKeys.Control, Keys.D5);
                MyHotKeyManager.AddLocalHotKey(hoykey);
            }
            {
                LocalHotKey hoykey = new LocalHotKey("FogofWar", ModifierKeys.Control, Keys.D6);
                MyHotKeyManager.AddLocalHotKey(hoykey);
            }


            MyHotKeyManager.LocalHotKeyPressed += MyHotKeyManager_LocalHotKeyPressed;
        }
    }
}
