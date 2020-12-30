using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Map
{
    public partial class MapData
    {
        public List<LocationData> LocationDatas = new List<LocationData>();

        public class LocationData
        {
            public int INDEX;

            public uint L, R, T, B;
            public StringData STRING;
            public ushort FLAG;

            public Color RnColor;

            public LocationData()
            {
                RnColor = new Color((uint)UseMapEditor.Global.WindowTool.random.Next());

                RnColor.A = 32;
            }

            //Bit 0 - Low elevation
            //Bit 1 - Medium elevation
            //Bit 2 - High elevation
            //Bit 3 - Low air
            //Bit 4 - Medium air
            //Bit 5 - High air
            //Bit 6 - 15 - Unused
        }
    }
}
