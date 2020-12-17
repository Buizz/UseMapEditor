using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UseMapEditor.Global
{
    public static class Setting
    {
        readonly static string iniFile = AppDomain.CurrentDomain.BaseDirectory + @"\Setting.ini";



        public static Dictionary<Settings, string> Vals;

        public static void LoadSetting()
        {
            Vals = new Dictionary<Settings, string>();

            foreach (Settings settings in Enum.GetValues(typeof(Settings)))
            {
                Vals.Add(settings, ReadSetting(settings));
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
            Properties.Settings.Default.Save();
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
                        rv = "60";
                        break;
                    case Settings.Render_UseVFR:
                        rv = "true";
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



        public enum Settings
        {
            Program_StarCraftPath,
            Program_GRPLoad,
            Program_IsDark,
            Render_MaxFrame,
            Render_UseVFR
        }









        [DllImport("kernel32")]
        public static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        public static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
    }
}
