using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UseMapEditor.FileData;

namespace Data.Map
{
    public partial class MapData
    {

        public bool IsFileLocked(string filePath)
        {
            try
            {
                using (File.Open(filePath, FileMode.Open)) { }
            }
            catch (IOException e)
            {
                var errorCode = Marshal.GetHRForException(e) & ((1 << 16) - 1);

                return errorCode == 32 || errorCode == 33;
            }

            return false;
        }


        public bool NewMap(int Width, int Height, int TileType, int startTile)
        {
            filepath = "제목없음";
            MapDataReset();

            TILETYPE = (TileSet.TileType)TileType;
            ENCODING = System.Text.Encoding.UTF8;


            BYTESTR = new List<byte[]>();
            BYTESTRx = new List<byte[]>();
            LOADSTR = new string[0];
            LOADSTRx = new string[0];

            for (int i = 0; i < 8; i++)
            {
                CRGB[i] = new Microsoft.Xna.Framework.Color(0, 0, i);

                CRGBIND[i] = (byte)CRGBINDTYPE.UseCOLRselection;
                COLR[i] = (byte)i;
            }
            SIDE = new byte[12];
            FORCEFLAG = new byte[4];
            WIDTH = (ushort)Width;
            HEIGHT = (ushort)Height;
            MASK = new byte[Width * Height];

            //TODO:스타트타일에 맞게금 시작 타일 까는 로직 필요
            TILE = new ushort[Width * Height];
            MTXM = new ushort[Width * Height];


            IOWN = new byte[12];
            for (int i = 0; i < 12; i++)
            {
                IOWN[i] = 6;
            }

            for (int i = 0; i < 228; i++)
            {
                UNIx.USEDEFAULT[i] = 1;
                UNIx.HIT[i] = (uint)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Hit Points", i).Data;
                UNIx.BUILDTIME[i] = (ushort)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Build Time", i).Data;
                UNIx.SHIELD[i] = (ushort)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Shield Amount", i).Data;
                UNIx.MIN[i] = (ushort)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Mineral Cost", i).Data;
                UNIx.ARMOR[i] = (byte)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Armor Upgrade", i).Data;
                UNIx.GAS[i] = (ushort)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Vespene Cost", i).Data;

                PUNI.DEFAULT[i] = 1;
                for (int p = 0; p < 12; p++)
                {
                    PUNI.UNITENABLED[p][i] = 1;
                    PUNI.USEDEFAULT[p][i] = 1;
                }
            }
            for (int i = 0; i < 130; i++)
            {
                UNIx.DMG[i] = (ushort)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.weapons, "Damage Amount", i).Data;
                UNIx.BONUSDMG[i] = (ushort)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.weapons, "Damage Bonus", i).Data;
            }


            for (int i = 0; i < 61; i++)
            {
                UPGx.USEDEFAULT[i] = 1;
                UPGx.BASEMIN[i] = (ushort)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.upgrades, "Mineral Cost Base", i).Data;
                UPGx.BONUSMIN[i] = (ushort)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.upgrades, "Mineral Cost Factor", i).Data;
                UPGx.BASEGAS[i] = (ushort)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.upgrades, "Vespene Cost Base", i).Data;
                UPGx.BONUSGAS[i] = (ushort)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.upgrades, "Vespene Cost Factor", i).Data;
                UPGx.BASETIME[i] = (byte)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.upgrades, "Research Time Base", i).Data;
                UPGx.BONUSTIME[i] = (ushort)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.upgrades, "Research Time Factor", i).Data;


                PUPx.DEFAULTSTARTLEVEL[i] = 0;
                PUPx.DEFAULTMAXLEVEL[i] = (byte)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.upgrades, "Max. Repeats", i).Data;

                for (int p = 0; p < 12; p++)
                {
                    PUPx.USEDEFAULT[p][i] = 1;
                    PUPx.STARTLEVEL[p][i] = 1;
                    PUPx.MAXLEVEL[p][i] = 1;
                }
            }
            for (int i = 0; i < 44; i++)
            {
                TECx.USEDEFAULT[i] = 1;
                TECx.MIN[i] = (ushort)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.techdata, "Mineral Cost", i).Data;
                TECx.GAS[i] = (ushort)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.techdata, "Vespene Cost", i).Data;
                TECx.BASETIME[i] = (ushort)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.techdata, "Resarch Time", i).Data;
                TECx.ENERGY[i] = (ushort)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.techdata, "Energy Required", i).Data;


                PTEx.DEFAULTSTARTLEVEL[i] = (byte)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.techdata, "Unused", i).Data;
                PTEx.DEFAULTMAXLEVEL[i] = 1;

                for (int p = 0; p < 12; p++)
                {
                    PTEx.USEDEFAULT[p][i] = 1;
                    PTEx.STARTLEVEL[p][i] = (byte)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.techdata, "Unused", i).Data; ;
                    PTEx.MAXLEVEL[p][i] = 1;
                }
            }




            return true;
        }

        public bool LoadMap(string _filepath)
        {

            filepath = _filepath;
            //맵 파일의 이름이 ""일 경우 새맵
            if (filepath == "")
            {
                filepath = "제목없음";
                return true;
            }
            if (IsFileLocked(_filepath))
            {
                throw new Exception("파일을 다른 프로세스에서 사용중입니다.");
            }

            if (!File.Exists(_filepath))
            {
                throw new Exception("파일이 존재하지 않습니다.");
            }


            MapDataReset();

            //맵 파일을 연다.
            //chk를 추출한다.
           


            byte[] buffer = ReadMPQFile(@"staredit\scenario.chk");
            
            if(buffer.Length == 0)
            {
                throw new Exception("scenario.chk를 열지 못했습니다.");
            }
           

            //음원파일을 추출한다.
            using (BinaryReader br = new BinaryReader(new MemoryStream(buffer)))
            {
                if (!ApplychkAll(br))
                {
                    return false;
                }
            }

            return true;
        }



    }
}
