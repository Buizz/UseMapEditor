using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseMapEditor;

namespace Data.Map
{
    public partial class MapData
    {


        private string filepath;
        public string FilePath
        {
            get
            {
                return filepath;
            }
            set
            {
                filepath = value;
            }
        }
        public string SafeFileName
        {
            get
            {
                return filepath.Split('\\').Last();
            }
        }



        public UseMapEditor.FileData.TileSet.TileType TILETYPE;

        public ushort WIDTH;
        public ushort HEIGHT;


        public ushort[] MTXM;







    }
}
