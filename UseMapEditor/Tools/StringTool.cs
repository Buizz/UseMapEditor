using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UseMapEditor.Tools
{
    public class StringTool
    {
        public static string RemoveCtrlChar(string str)
        {
            for (byte i = 0; i < 32; i++)
            {
                string o = "\\x" + i.ToString("X2");

                str = str.Replace(o, "");
            }

            return str;
        }



        public static string ReadRawString(string str)
        {
            for (byte i = 0; i < 32; i++)
            {
                string o = "\\x" + i.ToString("X2");
                if (i == 10)
                {
                    o = "\\n";
                }
                else if (i == 13)
                {
                    o = "\\r";
                }
                str = str.Replace(((char)i).ToString(), o);

            }

            return str;
        }

        public static string WriteRawString(string str)
        {
            for (int i = 0; i < 32; i++)
            {
                string o = "\\x" + i.ToString("X2");
                if (i == 10)
                {
                    o = "\\n";
                }
                else if (i == 13)
                {
                    o = "\\r";
                }

                str = str.Replace(o, ((char)i).ToString());
            }

            return str;
        }

        public static void SafeCopy(string text)
        {
            try
            {
                Clipboard.Clear();
                Clipboard.SetText(text);
            }
            catch (COMException e1)
            {
                try
                {
                    Clipboard.SetDataObject(text);
                }
                catch (COMException e2)
                {

                }
            }

        }
    }
}
