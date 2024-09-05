using BondTech.HotKeyManagement.WPF._4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UseMapEditor.Global
{
    public static class Setting
    {
        public readonly static string iniFile = AppDomain.CurrentDomain.BaseDirectory + @"\Setting.ini";
        public readonly static string backFolder = AppDomain.CurrentDomain.BaseDirectory + @"\Data\BackUp";



        public static Dictionary<string, ShortCutKey> ShortCutKeys;
        public class ShortCutKey
        {
            public ModifierKeys modifierKeys;
            public Keys keys;

            public ShortCutKey(ModifierKeys modifierKeys, Keys keys)
            {
                this.modifierKeys = modifierKeys;
                this.keys = keys;
            }

            public string ToValue()
            {
                int m = (int)modifierKeys;
                int k = (int)keys;

                return m.ToString() + "," + k.ToString();
            }
        }
        public static void ResetShortCut()
        {
            ShortCutKeys = new Dictionary<string, ShortCutKey>();

            ShortCutKeys.Add("New", new ShortCutKey(ModifierKeys.Control, Keys.N));
            ShortCutKeys.Add("Open", new ShortCutKey(ModifierKeys.Control, Keys.O));
            ShortCutKeys.Add("Save", new ShortCutKey(ModifierKeys.Control, Keys.S));
            ShortCutKeys.Add("Undo", new ShortCutKey(ModifierKeys.Control, Keys.Z));
            ShortCutKeys.Add("Redo", new ShortCutKey(ModifierKeys.Control, Keys.Y));

            ShortCutKeys.Add("SystemDraw", new ShortCutKey(ModifierKeys.Control, Keys.F));

            ShortCutKeys.Add("Grid1", new ShortCutKey(ModifierKeys.None, Keys.F1));
            ShortCutKeys.Add("Grid2", new ShortCutKey(ModifierKeys.None, Keys.F2));
            ShortCutKeys.Add("Grid3", new ShortCutKey(ModifierKeys.None, Keys.F3));
            ShortCutKeys.Add("Grid4", new ShortCutKey(ModifierKeys.None, Keys.F4));

            ShortCutKeys.Add("GrpChange", new ShortCutKey(ModifierKeys.None, Keys.F5));
            ShortCutKeys.Add("SDGrp", new ShortCutKey(ModifierKeys.None, Keys.F6));
            ShortCutKeys.Add("HDGrp", new ShortCutKey(ModifierKeys.None, Keys.F7));
            ShortCutKeys.Add("CBGrp", new ShortCutKey(ModifierKeys.None, Keys.F8));

            ShortCutKeys.Add("CopyPaste", new ShortCutKey(ModifierKeys.Control, Keys.Oemtilde));
            ShortCutKeys.Add("mapSetting", new ShortCutKey(ModifierKeys.Control | ModifierKeys.Shift, Keys.D1));
            ShortCutKeys.Add("playerSetting", new ShortCutKey(ModifierKeys.Control | ModifierKeys.Shift, Keys.D2));
            ShortCutKeys.Add("forceSetting", new ShortCutKey(ModifierKeys.Control | ModifierKeys.Shift, Keys.D3));
            ShortCutKeys.Add("unitSetting", new ShortCutKey(ModifierKeys.Control, Keys.Q));
            ShortCutKeys.Add("upgradeSetting", new ShortCutKey(ModifierKeys.Control, Keys.W));
            ShortCutKeys.Add("techSetting", new ShortCutKey(ModifierKeys.Control, Keys.E));
            ShortCutKeys.Add("soundSetting", new ShortCutKey(ModifierKeys.Control, Keys.R));
            ShortCutKeys.Add("stringSetting", new ShortCutKey(ModifierKeys.Control | ModifierKeys.Shift, Keys.D4));
            ShortCutKeys.Add("classTriggerEditor", new ShortCutKey(ModifierKeys.Control, Keys.T));
            ShortCutKeys.Add("brinfingTriggerEditor", new ShortCutKey(ModifierKeys.Control | ModifierKeys.Shift, Keys.D5));

            ShortCutKeys.Add("Tileset", new ShortCutKey(ModifierKeys.Control, Keys.D1));
            ShortCutKeys.Add("Doodad", new ShortCutKey(ModifierKeys.Control, Keys.D2));
            ShortCutKeys.Add("Unit", new ShortCutKey(ModifierKeys.Control, Keys.D3));
            ShortCutKeys.Add("Sprite", new ShortCutKey(ModifierKeys.Control, Keys.D4));
            ShortCutKeys.Add("Location", new ShortCutKey(ModifierKeys.Control, Keys.D5));
            ShortCutKeys.Add("FogofWar", new ShortCutKey(ModifierKeys.Control, Keys.D6));
        }



        public static Dictionary<Settings, string> Vals;
        


        public static void LoadSetting()
        {
            ResetShortCut();
            Vals = new Dictionary<Settings, string>();

            foreach (Settings settings in Enum.GetValues(typeof(Settings)))
            {
                Vals.Add(settings, ReadSetting(settings));
            }

            foreach (var item in ShortCutKeys)
            {
                ReadSettingShortCut(item.Key);
            }
        }

        public static void SaveSetting()
        {
            if(Vals != null)
            {
                foreach (Settings settings in Enum.GetValues(typeof(Settings)))
                {
                    WriteSetting(settings, Vals[settings].ToString());
                }
            }

            if (ShortCutKeys != null)
            {
                foreach (var item in ShortCutKeys)
                {
                    WriteSettingShortCut(item);
                }
            }


            Properties.Settings.Default.Save();
        }


        public static void ReadSettingShortCut(string name)
        {
            StringBuilder sb = new StringBuilder(1024);
            GetPrivateProfileString("ShortCut", name, "", sb, sb.Capacity, iniFile);

            string rv = sb.ToString();

            if (rv == "") return;


            string[] vals = rv.Split(',');





            int modifierKeys, keys;
            if(!int.TryParse(vals[0], out modifierKeys)) return;
            if(!int.TryParse(vals[1], out keys)) return;


            ShortCutKeys[name].modifierKeys = (ModifierKeys)modifierKeys;
            ShortCutKeys[name].keys = (Keys)keys;


        }

        public static string ReadSetting(Settings settings)
        {
            string group;
            string key;

            group = settings.ToString().Split('_').First();
            key = settings.ToString().Split('_').Last();




            StringBuilder sb = new StringBuilder(1024);
            GetPrivateProfileString(group, key, "", sb, sb.Capacity, iniFile);

            string rv = sb.ToString();

            if(rv == "")
            {
                switch (settings)
                {
                    case Settings.Program_IsDark:
                        rv = "false";
                        break;
                    case Settings.Program_GRPLoad:
                        rv = "false";
                        break;
                    case Settings.Program_StarCraftPath:
                        rv = "";
                        break;
                    case Settings.Render_MaxFrame:
                        rv = "0";
                        break;
                    case Settings.Render_UseVFR:
                        rv = "true";
                        break;
                    case Settings.language_StatLan:
                        rv = "stat_txt_kor_kor";
                        break;
                    case Settings.Program_GridColor:
                        rv = "4278190080";
                        break;
                    case Settings.Program_GridSize:
                        rv = "32";
                        break;
                    case Settings.Program_FastExpander:
                        rv = "false";
                        break;
                    case Settings.TIlePreviewOpacity:
                        rv = "100";
                        break;
                }
            }


            return rv;
        }
        public static void WriteSetting(Settings settings, string value)
        {
            string group;
            string key;

            group = settings.ToString().Split('_').First();
            key = settings.ToString().Split('_').Last();

            WritePrivateProfileString(group, key, value, iniFile);
        }
        public static void WriteSettingShortCut(KeyValuePair<string, ShortCutKey> shortCutKey)
        {
            string group = "ShortCut";
            string key = shortCutKey.Key;


            string value = shortCutKey.Value.ToValue();




            WritePrivateProfileString(group, key, value, iniFile);
        }


        public enum Settings
        {
            Program_StarCraftPath,
            Program_GRPLoad,
            Program_IsDark,
            Program_GridColor,
            Program_GridSize,
            Render_MaxFrame,
            Render_UseVFR,
            language_StatLan,
            Program_FastExpander,
            TIlePreviewOpacity
        }









        [DllImport("kernel32")]
        public static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        public static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
    }
}
