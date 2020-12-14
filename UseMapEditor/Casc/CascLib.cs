using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UseMapEditor.Casc
{
    public static class CascLib
    {
        [DllImport("CascLib.dll")]
        public static extern bool CascOpenStorage(string szDataPath, uint dwLocaleMask, ref IntPtr phStorage);

        [DllImport("CascLib.dll")]
        public static extern bool CascOpenFile(IntPtr hStorage, string szFileName, uint dwLocale, uint dwFlags, ref IntPtr phFile);

        [DllImport("CascLib.dll")]
        public static extern bool CascReadFile(IntPtr hFile, byte[] lpBuffer, uint dwToRead, ref IntPtr pdwRead);

        [DllImport("CascLib.dll")]
        public static extern bool CascCloseFile(IntPtr hFile);

        [DllImport("CascLib.dll")]
        public static extern bool CascCloseStorage(IntPtr hStorage);
    }
}
