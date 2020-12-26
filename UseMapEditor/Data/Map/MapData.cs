using Microsoft.Win32;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseMapEditor;
using UseMapEditor.FileData;
using static UseMapEditor.FileData.TileSet;

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


        public Encoding ENCODING;
        public void SetEncoding(int codepage)
        {
            ENCODING = System.Text.Encoding.GetEncoding(codepage);
        }

        public void MapDataReset()
        {
            for (int i = 0; i < 8; i++)
            {
                CRGB[i] = new Microsoft.Xna.Framework.Color(0, 0, 0);

                CRGBIND[i] = (byte)CRGBINDTYPE.UseCOLRselection;
                COLR[i] = (byte)i;
            }
            UNIT.Clear();

            MTXM = null;
            TILE = null;
            LOADSTR = null;
            LOADSTRx = null;
            IOWN = new byte[12];
            SIDE = new byte[12];
            FORCE = new byte[8];
            FORCENAME = new StringData[4];
            FORCENAME[0] = new StringData(this, "세력 1");
            FORCENAME[1] = new StringData(this, "세력 2");
            FORCENAME[2] = new StringData(this, "세력 3");
            FORCENAME[3] = new StringData(this, "세력 4");


            FORCEFLAG = new byte[4];
            WIDTH = 0;
            HEIGHT = 0;

            SCEARIONAME = new StringData(this, "제목 없음");
            SCEARIODES = new StringData(this, "시나리오 설명이 없습니다.");

            ENCODING = null;// System.Text.Encoding.

            BYTESTR = null;
            BYTESTRx = null;

            DD2.Clear();
            THG2.Clear();

            MASK = null;

            MRGN = new CMRGN[255];
            DDDTHG2.Clear();
        }


        public UseMapEditor.FileData.TileSet.TileType TILETYPE;



        public ushort VER = 0xCE;
        public uint TYPE = 0x42574152;
        public ushort IVE2 = 0x0B;
        public byte[] VCOD = { 0x34, 0x19, 0xCA, 0x77, 0x99, 0xDC, 0x68, 0x71, 0x0A, 0x60, 0xBF, 0xC3, 0xA7, 0xE7, 0x75, 0xA7,
            0x1F, 0x29, 0x7D, 0xA6, 0xD7, 0xB0, 0x3A, 0xBB, 0xCC, 0x31, 0x24, 0xED, 0x17, 0x4C, 0x13, 0x0B, 0x65, 0x20, 0xA2,
            0xB7, 0x91, 0xBD, 0x18, 0x6B, 0x8D, 0xC3, 0x5D, 0xDD, 0xE2, 0x7A, 0xD5, 0x37, 0xF6, 0x59, 0x64, 0xD4, 0x63, 0x9A,
            0x12, 0x0F, 0x43, 0x5C, 0x2E, 0x46, 0xE3, 0x74, 0xF8, 0x2A, 0x08, 0x6A, 0x37, 0x06, 0x37, 0xF6, 0xD6, 0x3B, 0x0E,
            0x94, 0x63, 0x16, 0x45, 0x67, 0x5C, 0xEC, 0xD7, 0x7B, 0xF7, 0xB7, 0x1A, 0xFC, 0xD4, 0x9E, 0x73, 0xFA, 0x3F, 0x8C,
            0x2E, 0xC0, 0xE1, 0x0F, 0xD1, 0x74, 0x09, 0x07, 0x95, 0xE3, 0x64, 0xD7, 0x75, 0x16, 0x68, 0x74, 0x99, 0xA7, 0x4F,
            0xDA, 0xD5, 0x20, 0x18, 0x1F, 0xE7, 0xE6, 0xA0, 0xBE, 0xA6, 0xB6, 0xE3, 0x1F, 0xCA, 0x0C, 0xEF, 0x70, 0x31, 0xD5,
            0x1A, 0x31, 0x4D, 0xB8, 0x24, 0x35, 0xE3, 0xF8, 0xC7, 0x7D, 0xE1, 0x1A, 0x58, 0xDE, 0xF4, 0x05, 0x27, 0x43, 0xBA,
            0xAC, 0xDB, 0x07, 0xDC, 0x69, 0xBE, 0x0A, 0xA8, 0x8F, 0xEC, 0x49, 0xD7, 0x58, 0x16, 0x3F, 0xE5, 0xDB, 0xC1, 0x8A,
            0x41, 0xCF, 0xC0, 0x05, 0x9D, 0xCA, 0x1C, 0x72, 0xA2, 0xB1, 0x5F, 0xA5, 0xC4, 0x23, 0x70, 0x9B, 0x84, 0x04, 0xE1,
            0x14, 0x80, 0x7B, 0x90, 0xDA, 0xFA, 0xDB, 0x69, 0x06, 0xA3, 0xF3, 0x0F, 0x40, 0xBE, 0xF3, 0xCE, 0xD4, 0xE3, 0xC9,
            0xCB, 0xD7, 0x5A, 0x40, 0x01, 0x34, 0xF2, 0x68, 0x14, 0xF8, 0x38, 0x8E, 0xC5, 0x1A, 0xFE, 0xD6, 0x3D, 0x4B, 0x53,
            0x05, 0x05, 0xFA, 0x34, 0x10, 0x45, 0x8E, 0xDD, 0x91, 0x69, 0xFE, 0xAF, 0xE0, 0xEE, 0xF0, 0xF3, 0x48, 0x7E, 0xDD,
            0x9F, 0xAD, 0xDC, 0x75, 0x62, 0x7A, 0xAC, 0xE5, 0x31, 0x1B, 0x62, 0x67, 0x20, 0xCD, 0x36, 0x4D, 0xE0, 0x98, 0x21,
            0x74, 0xFB, 0x09, 0x79, 0x71, 0x36, 0x67, 0xCD, 0x7F, 0x77, 0x5F, 0xD6, 0x3C, 0xA2, 0xA2, 0xA6, 0xC6, 0x1A, 0xE3,
            0xCE, 0x6A, 0x4E, 0xCD, 0xA9, 0x6C, 0x86, 0xBA, 0x9D, 0x3B, 0xB5, 0xF4, 0x76, 0xFD, 0xF8, 0x44, 0xF0, 0xBC, 0x2E,
            0xE9, 0x6E, 0x29, 0x23, 0x25, 0x2F, 0x6B, 0x08, 0xAB, 0x27, 0x44, 0x7A, 0x12, 0xCC, 0x99, 0xED, 0xDC, 0xF2, 0x75,
            0xC5, 0x3C, 0x38, 0x7E, 0xF7, 0x1C, 0x1B, 0xC5, 0xD1, 0x2D, 0x94, 0x65, 0x06, 0xC9, 0x48, 0xDD, 0xBE, 0x32, 0x2D,
            0xAC, 0xB5, 0xC9, 0x32, 0x81, 0x66, 0x4A, 0xD8, 0x34, 0x35, 0x3F, 0x15, 0xDF, 0xB2, 0xEE, 0xEB, 0xB6, 0x04, 0xF6,
            0x4D, 0x96, 0x35, 0x42, 0x94, 0x9C, 0x62, 0x8A, 0xD3, 0x61, 0x52, 0xA8, 0x7B, 0x6F, 0xDC, 0x61, 0xFC, 0xF4, 0x6C,
            0x14, 0x2D, 0xFE, 0x99, 0xEA, 0xA4, 0x0A, 0xE8, 0xD9, 0xFE, 0x13, 0xD0, 0x48, 0x44, 0x59, 0x80, 0x66, 0xF3, 0xE3,
            0x34, 0xD9, 0x8D, 0x19, 0x16, 0xD7, 0x63, 0xFE, 0x30, 0x18, 0x7E, 0x3A, 0x9B, 0x8D, 0x0F, 0xB1, 0x12, 0xF0, 0xF5,
            0x8C, 0x0A, 0x78, 0x58, 0xDB, 0x3E, 0x63, 0xB8, 0x8C, 0x3A, 0xAA, 0xF3, 0x8E, 0x37, 0x8A, 0x1A, 0x2E, 0x5C, 0x31,
            0xF9, 0xEF, 0xE3, 0x6D, 0xE3, 0x7E, 0x9B, 0xBD, 0x3E, 0x13, 0xC6, 0x44, 0xC0, 0xB9, 0xBC, 0x3A, 0xDA, 0x90, 0xA4,
            0xAD, 0xB0, 0x74, 0xF8, 0x57, 0x27, 0x89, 0x47, 0xE6, 0x3F, 0x37, 0xE4, 0x42, 0x79, 0x5A, 0xDF, 0x43, 0x8D, 0xEE,
            0xB4, 0x0A, 0x49, 0xE8, 0x3C, 0xC3, 0x88, 0x1A, 0x88, 0x01, 0x6B, 0x76, 0x8A, 0xC3, 0xFD, 0xA3, 0x16, 0x7A, 0x4E,
            0x56, 0xA7, 0x7F, 0xCB, 0xBA, 0x02, 0x5E, 0x1C, 0xEC, 0xB0, 0xB9, 0xC9, 0x76, 0x1E, 0x82, 0xB1, 0x39, 0x3E, 0xC9,
            0x57, 0xC5, 0x19, 0x24, 0x38, 0x4C, 0x5D, 0x2F, 0x54, 0xB8, 0x6F, 0x5D, 0x57, 0x8E, 0x30, 0xA1, 0x0A, 0x52, 0x6D,
            0x18, 0x71, 0x5E, 0x13, 0x06, 0xC3, 0x59, 0x1F, 0xDC, 0x3E, 0x62, 0xDC, 0xDA, 0xB5, 0xEB, 0x1B, 0x91, 0x95, 0xF9,
            0xA7, 0x91, 0xD5, 0xDA, 0x33, 0x53, 0xCE, 0x6B, 0xF5, 0x00, 0x70, 0x01, 0x7F, 0xD8, 0xEE, 0xE8, 0xC0, 0x0A, 0xF1,
            0xCE, 0x63, 0xEB, 0xB6, 0xD3, 0x78, 0xEF, 0xCC, 0xA5, 0xAA, 0x5D, 0xBC, 0xA4, 0x96, 0xAB, 0xF2, 0xD2, 0x61, 0xFF,
            0xEA, 0x9A, 0xA8, 0x6A, 0xED, 0xA2, 0xBD, 0x3E, 0xED, 0x61, 0x39, 0xC1, 0x82, 0x92, 0x16, 0x36, 0x23, 0xB1, 0xB0,
            0xA0, 0x24, 0xE5, 0x05, 0x9B, 0xA7, 0xAA, 0x0D, 0x12, 0x9B, 0x33, 0x83, 0x92, 0x20, 0xDA, 0x25, 0xB0, 0xEC, 0xFC,
            0x24, 0xD0, 0x38, 0x23, 0xFC, 0x95, 0xF2, 0x74, 0x80, 0x73, 0xE5, 0x19, 0x97, 0x50, 0x7D, 0x44, 0x45, 0x93, 0x44,
            0xDB, 0xA2, 0xAD, 0x1D, 0x69, 0x44, 0x14, 0xEE, 0xE7, 0x2C, 0x7F, 0x87, 0xFF, 0x38, 0x9E, 0x32, 0xF1, 0x4D, 0xBC,
            0x29, 0xDA, 0x42, 0x27, 0x26, 0xFE, 0xC1, 0xD2, 0x2B, 0xA9, 0xF6, 0x42, 0x7A, 0x0E, 0xCB, 0xE8, 0x7C, 0xD1, 0x0F,
            0x5B, 0xEC, 0x56, 0x69, 0xB7, 0x61, 0x31, 0xB4, 0x6D, 0xF9, 0x25, 0x40, 0x34, 0x79, 0x6D, 0xFA, 0x53, 0xA7, 0x0B,
            0xFA, 0xA4, 0x82, 0xCE, 0xC3, 0x45, 0x49, 0x61, 0x0D, 0x45, 0x2C, 0x8F, 0x28, 0x49, 0x60, 0xF7, 0xF3, 0x7D, 0xC9,
            0x1E, 0x0F, 0xD0, 0x89, 0xC1, 0x26, 0x52, 0xF8, 0xD3, 0x4D, 0x8F, 0x35, 0x14, 0xBA, 0x9D, 0x5F, 0x0B, 0x07, 0xA9,
            0x4A, 0x00, 0xF7, 0x22, 0x26, 0x2F, 0x3E, 0x67, 0xFB, 0x1F, 0xA1, 0x9C, 0x11, 0xC6, 0x69, 0x4F, 0x5D, 0x66, 0x58,
            0x34, 0x15, 0x90, 0x6C, 0xE5, 0x54, 0x46, 0xAF, 0x5F, 0x63, 0xD6, 0x8A, 0x0C, 0x95, 0xDF, 0xBD, 0x0D, 0xE4, 0xAF,
            0xBF, 0x40, 0x40, 0x4C, 0xA3, 0xF6, 0x51, 0x71, 0x29, 0xED, 0x26, 0xF8, 0x85, 0x28, 0x22, 0xD5, 0xBF, 0xBE, 0xCF,
            0xFA, 0x28, 0xC5, 0x7F, 0x51, 0xB8, 0x06, 0x63, 0x07, 0xEC, 0xBD, 0x8F, 0x29, 0xFA, 0x55, 0x7E, 0x71, 0x1A, 0x40,
            0x32, 0x66, 0xE8, 0xD4, 0xDE, 0x9D, 0xD4, 0x5E, 0xFC, 0x93, 0x7A, 0x3D, 0xD5, 0x3B, 0xCD, 0x75, 0x2E, 0x80, 0x0A,
            0x4F, 0x74, 0x87, 0x1B, 0xCC, 0x8F, 0xEA, 0x9A, 0xA9, 0xDB, 0x7C, 0x16, 0x53, 0xE5, 0xEF, 0xAB, 0x78, 0xC1, 0x6E,
            0xA4, 0x72, 0x89, 0x5A, 0x98, 0x2C, 0x70, 0x50, 0xFB, 0xA1, 0xDF, 0x1F, 0x6B, 0xB7, 0xD9, 0x44, 0x07, 0x80, 0x82,
            0x56, 0xFD, 0xBF, 0xC0, 0x83, 0x0E, 0x49, 0xD0, 0x5B, 0x1E, 0x68, 0x6A, 0x0E, 0x9A, 0xC2, 0x0B, 0x2F, 0x8E, 0x43,
            0xA0, 0xE1, 0x99, 0x0C, 0xF6, 0xB2, 0xE0, 0x7A, 0x1C, 0x5E, 0x2C, 0xC8, 0xA0, 0x45, 0x3C, 0x0B, 0xE9, 0x88, 0xAC,
            0xB9, 0x96, 0xC6, 0x74, 0xAE, 0x83, 0x2A, 0xBB, 0x13, 0xFA, 0x65, 0xEB, 0x4F, 0x1F, 0xA6, 0xB0, 0x8A, 0x8A, 0xE1,
            0x81, 0xE9, 0xB8, 0xB9, 0xD5, 0x55, 0x15, 0x4E, 0x45, 0xF2, 0xAD, 0x9B, 0x3E, 0xC2, 0x35, 0x7E, 0x5F, 0x92, 0x2E,
            0x72, 0xB6, 0x5B, 0x68, 0x23, 0x6E, 0xC6, 0x45, 0x0E, 0xE9, 0x3B, 0x87, 0xD4, 0xF4, 0x41, 0xC0, 0xE3, 0xA8, 0x05,
            0x44, 0xBE, 0xE4, 0x0F, 0x8A, 0x13, 0x1A, 0xC4, 0x37, 0xF4, 0x5A, 0x40, 0x55, 0xEF, 0x9D, 0x79, 0x1D, 0x4B, 0x4A,
            0x79, 0x3A, 0x9C, 0x76, 0x85, 0x37, 0xCC, 0x82, 0x3D, 0x0F, 0xB6, 0x60, 0xA6, 0x93, 0x7E, 0xBD, 0x5C, 0xC2, 0xC4,
            0x72, 0xC7, 0x7F, 0x90, 0x4D, 0x1B, 0x96, 0x10, 0x13, 0x05, 0x68, 0x68, 0x35, 0xC0, 0x7B, 0xFF, 0x46, 0x85, 0x43,
            0x2A, 0x01, 0x04, 0x05, 0x06, 0x02, 0x01, 0x05, 0x02, 0x00, 0x03, 0x07, 0x07, 0x05, 0x04, 0x06, 0x03};

        public StringData SCEARIONAME = new StringData(null, "");
        public StringData SCEARIODES = new StringData(null, "");




        public byte[] IOWN = new byte[12];
        public enum IOWNTYPE
        {
            NOPLAYER = 0,
            RESCUABLE = 3,
            COMPUTER = 5,
            HUMAN = 6,
            NEUTRAL = 7
        }
        public string GetIOWNTYPEName(IOWNTYPE index)
        {
            switch (index)
            {
                case IOWNTYPE.NOPLAYER:
                    return "사용안함";
                case IOWNTYPE.RESCUABLE:
                    return "구조가능";
                case IOWNTYPE.COMPUTER:
                    return "컴퓨터";
                case IOWNTYPE.HUMAN:
                    return "사람";
                case IOWNTYPE.NEUTRAL:
                    return "중립";
            }
            return "알수없음";
        }



        public byte[] SIDE = new byte[12];
        public enum SIDETYPE
        {
            Zerg = 0,
            Terran = 1,
            Protoss = 2,
            Userselectable = 5,
            Inactive = 7
        }
        public string GetSIDETYPEName(SIDETYPE index)
        {
            switch (index)
            {
                case SIDETYPE.Zerg:
                    return "저그";
                case SIDETYPE.Terran:
                    return "테란";
                case SIDETYPE.Protoss:
                    return "프로토스";
                case SIDETYPE.Userselectable:
                    return "유저선택";
                case SIDETYPE.Inactive:
                    return "비활성";
            }
            return "알수없음";
        }

        public byte[] FORCE = new byte[8];
        public StringData[] FORCENAME = new StringData[4];
        public byte[] FORCEFLAG = new byte[4];
        //Bit 0 - Random start location
        //Bit 1 - Allies
        //Bit 2 - Allied victory
        //Bit 3 - Shared vision
        //Bit 4-7 - Unused


        public byte[] COLR = new byte[8];
        public byte[] CRGBIND = new byte[8];
        public enum CRGBINDTYPE
        {
            RandomPredefined = 0,
            PlayerChoice = 1,
            CustomRGBColor = 2,
            UseCOLRselection = 3
        }
        public Color[] CRGB = new Color[8];


        public static string[] ColorName =
        {
            "빨강",
            "파랑",
            "청록",
            "보라",
            "주황",
            "갈색",
            "흰색",
            "노랑",

            "초록",
            "옅은노랑",
            "황갈색",
            "중립",
            "담록",
            "푸른회색",
            "시안",
            "분홍",

            "황록",
            "라임",
            "남색",
            "자홍",
            "회색",
            "검정"
    };
        public static Color[] PlayerColors = {
            new Color(0xd6, 0x03, 0x03), new Color(0x09, 0x37, 0x9b), new Color(0x25, 0x85, 0x6f), new Color(0x77, 0x38, 0x89),
            new Color(0xd9, 0x7b, 0x12), new Color(0x62, 0x2a, 0x12), new Color(0xb3, 0xc5, 0xb6), new Color(0xdd, 0xde, 0x31),

            new Color(0x0b, 0x60, 0x0b), new Color(0xdd, 0xde, 0x6d), new Color(0xcf, 0xad, 0x9a), new Color(0x38, 0x5b, 0xba),
            new Color(0x66, 0x90, 0x6d), new Color(0x64, 0x7f, 0xa1), new Color(0x00, 0xc9, 0xdd), new Color(0xe0, 0xad, 0xc8),
            new Color(0x70, 0x71, 0x00), new Color(0xa0, 0xba, 0x2e), new Color(0x00, 0x00, 0x70), new Color(0xd2, 0x2c, 0xca),
            new Color(0x61, 0x61, 0x61), new Color(0x34, 0x35, 0x34)};



        public Microsoft.Xna.Framework.Color UnitColor(int PlayerID)
        {
            if(PlayerID < 8)
            {
                if ((CRGBINDTYPE)CRGBIND[PlayerID] == CRGBINDTYPE.UseCOLRselection)
                {
                    return MapData.PlayerColors[CRGB[PlayerID].B];
                }
                else
                {
                    if ((CRGBINDTYPE)CRGBIND[PlayerID] == CRGBINDTYPE.CustomRGBColor)
                    {

                        return CRGB[PlayerID];

                    }
                    else
                    {
                        return new Color();
                    }
                }
            }
            else
            {
                return PlayerColors[PlayerID];
            }



        }





        public ushort WIDTH;
        public ushort HEIGHT;


        public ushort[] TILE;
        public ushort[] MTXM;


        public List<CDD2> DD2 = new List<CDD2>();
        public class CDD2
        {
            public int alpha;

            public ushort ID;//u16: Number of the doodad.Size of the doodad is dependent on this. Doodads are different for each tileset.
            public ushort X;//u16: X coordinate of the doodad unit
            public ushort Y;//u16: Y coordinate of the doodad unit
            public byte PLAYER;//u8: Player number that owns the doodad
            public byte FLAG;//u8: Enabled flag
                             //00 - Doodad is enabled (trap can attack, door is closed, etc)
                             //01 - Doodad is disabled


            MapData mapData;


            public void ImageReset()
            {
                DoodadPallet pallete = UseMapEditor.Global.WindowTool.MapViewer.tileSet.DoodadPallets[mapData.TILETYPE][ID];
                if ((pallete.dddFlags & 0x1000) > 0)
                {
                    //0x1000 = Sprites.dat Reference
                    int ImageID = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.sprites, "Image File", pallete.dddOverlayID).Data;

                    CImage p = new CImage(alpha, Images, ImageID, 0, 0, _drawType: CImage.DrawType.Normal, level: 8);
                    Images.Add(p);
                }
                if ((pallete.dddFlags & 0x2000) > 0)
                {
                    //0x2000 = Units.dat Reference(unit sprite)
                    int Graphics = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Graphics", pallete.dddOverlayID).Data;
                    int Sprite = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.flingy, "Sprite", Graphics).Data;
                    int ImageID = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.sprites, "Image File", Sprite).Data;

                    CImage p = new CImage(alpha, Images, ImageID, 0, 0, _drawType: CImage.DrawType.Normal, level: 8);
                    Images.Add(p);
                }
            }
            public CDD2(BinaryReader br, MapData _mapData)
            {
                alpha = UseMapEditor.Global.WindowTool.random.Next();
                mapData = _mapData;

                ID = br.ReadUInt16();
                X = br.ReadUInt16();
                Y = br.ReadUInt16();
                PLAYER = br.ReadByte();
                FLAG = br.ReadByte();


                Images = new List<CImage>();

                ImageReset();
            }



            public List<CImage> Images;
        }

        public List<CTHG2> DDDTHG2 = new List<CTHG2>();
        public List<CTHG2> THG2 = new List<CTHG2>();
        public class CTHG2
        {
            public int alpha;


            public ushort ID;//u16: Unit/Sprite number of the sprite
            public ushort X;//u16: X coordinate of the doodad unit
            public ushort Y;//u16: Y coordinate of the doodad unit
            public byte PLAYER;//u8: Player number that owns the doodad
            public byte UNUSED;//u8: Unused
            public ushort FLAG;//u16: Flags
                               //Bit 0-11 - Unused
                               //Bit 12 - Pure Sprite
                               //Bit 13 - Unit Sprite
                               //Bit 14 - Flipped
                               //Bit 15 - Disabled(Only valid if Draw as sprite is unchecked, disables the unit)

            public CTHG2()
            {
                alpha = UseMapEditor.Global.WindowTool.random.Next();
            }

            public CTHG2(BinaryReader br)
            {
                alpha = UseMapEditor.Global.WindowTool.random.Next();

                ID = br.ReadUInt16();
                X = br.ReadUInt16();
                Y = br.ReadUInt16();
                PLAYER = br.ReadByte();
                UNUSED = br.ReadByte();
                FLAG = br.ReadUInt16();

                Images = new List<CImage>();

                ImageReset();
            }


            public int BoxWidth;
            public int BoxHeight;


            public void ImageReset()
            {
                Images.Clear();
                int ImageID = 0;

                if ((FLAG & (1 << 12)) > 0)
                {
                    //Pure
                    ImageID = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.sprites, "Image File", ID).Data;

                    BoxWidth = 256;
                    BoxHeight = 256;

                    CImage p = new CImage(alpha, Images, ImageID, 0, PLAYER, _drawType: CImage.DrawType.Normal, level: 8);
                    Images.Add(p);
                }

                if ((FLAG & (1 << 13)) > 0)
                {
                    //Unit
                    BoxWidth = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "StarEdit Placement Box Width", ID).Data;
                    BoxHeight = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "StarEdit Placement Box Height", ID).Data;

                    int Graphics = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Graphics", ID).Data;
                    int Level = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Elevation Level", ID).Data;
                    int Sprite = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.flingy, "Sprite", Graphics).Data;
                    ImageID = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.sprites, "Image File", Sprite).Data;

                    int Dir = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Unit Direction", ID).Data;

                    if (Dir == 32)
                    {
                        Dir = -1;
                    }

                    CImage p = new CImage(alpha, Images, ImageID, Dir, PLAYER, _drawType: CImage.DrawType.Normal, level: Level);
                    Images.Add(p);

                    int Subunit = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Subunit 1", ID).Data;
                    if (Subunit != 228)
                    {
                        Graphics = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Graphics", Subunit).Data;
                        Level = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Elevation Level", Subunit).Data;
                        Sprite = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.flingy, "Sprite", Graphics).Data;
                        ImageID = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.sprites, "Image File", Sprite).Data;

                        if (ImageID == 254)
                        {
                            Dir += 16;
                        }
                        Images.Add(new CImage(alpha, Images, ImageID, Dir, PLAYER, _parentImage: p, level: Level + 1));
                    }
                }
            }


            public List<CImage> Images;
        }

        public byte[] MASK;




        public CMRGN[] MRGN;
        public class CMRGN
        {
            public uint Left;//u32: Left(X1) coordinate of location, in pixels(usually 32 pt grid aligned)
            public uint Top;//u32: Top(Y1) coordinate of location, in pixels
            public uint Right;//u32: Right(X2) coordinate of location, in pixels
            public uint Bottom;//u32: Bottom(Y2) coordinate of location, in pixels
            public StringData String;//u16: String number of the name of this location
            public ushort Flag;//u16: Location elevation flags.If an elevation is disabled in the location, it's bit will be on (1)
            //Bit 0 - Low elevation
            //Bit 1 - Medium elevation
            //Bit 2 - High elevation
            //Bit 3 - Low air
            //Bit 4 - Medium air
            //Bit 5 - High air
            //Bit 6-15 - Unused
        }










        public List<CUNIT> UNIT = new List<CUNIT>();
        public class CUNIT
        {
            public CUNIT(BinaryReader br)
            {
                unitclass = br.ReadUInt32();
                x = br.ReadUInt16();
                y = br.ReadUInt16();
                unitID = br.ReadUInt16();
                linkFlag = br.ReadUInt16();
                validstatusFlag = br.ReadUInt16();
                validunitFlag = br.ReadUInt16();
                player = br.ReadByte();
                hitPoints = br.ReadByte();
                shieldPoints = br.ReadByte();
                energyPoints = br.ReadByte();
                resoruceAmount = br.ReadUInt32();
                hangar = br.ReadUInt16();
                stateFlag = br.ReadUInt16();
                unused = br.ReadUInt32();
                linkedUnit = br.ReadUInt32();

                Images = new List<CImage>();



                ImageReset();
            }
            public void ImageReset()
            {
                Images.Clear();


                int Graphics = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Graphics", unitID).Data;
                int Level = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Elevation Level", unitID).Data;
                int Sprite = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.flingy, "Sprite", Graphics).Data;
                int ImageID = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.sprites, "Image File", Sprite).Data;


                BoxWidth = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "StarEdit Placement Box Width", unitID).Data;
                BoxHeight = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "StarEdit Placement Box Height", unitID).Data;


                int Dir = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Unit Direction", unitID).Data;

                if (Dir == 32)
                {
                    Dir = -1;
                }

                CImage.DrawType drawType = CImage.DrawType.Normal;

                int startanim = 0;
                if ((stateFlag & 0b1) > 0)
                {
                    //Unit is cloaked
                    drawType = CImage.DrawType.Clock;
                }
                if ((stateFlag & 0b10) > 0)
                {
                    //Unit is burrowed
                    startanim = 25;
                }
                if ((stateFlag & 0b100) > 0)
                {
                    //Building is in transit
                    startanim = 18;
                }
                if ((stateFlag & 0b1000) > 0)
                {
                    //Unit is hallucinated
                    drawType = CImage.DrawType.Hallaction;
                }


                int StatusFlag = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Special Ability Flags", unitID).Data;
                if((StatusFlag & 0b10) > 0)
                {
                    if ((linkFlag & 0b1000000000) > 0)
                    {
                        //Bit 10 - Addon Link
                        startanim = 17;
                    }
                    else
                    {
                        startanim = 18;
                    }
                }



                CImage p = new CImage(unitclass, Images, ImageID, Dir, player, _drawType: drawType, level: Level, _StartAnim: startanim);
                Images.Add(p);
                int Subunit = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Subunit 1", unitID).Data;
                if(Subunit != 228)
                {
                    Graphics = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Graphics", Subunit).Data;
                    Level = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Elevation Level", Subunit).Data;
                    Sprite = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.flingy, "Sprite", Graphics).Data;
                    ImageID = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.sprites, "Image File", Sprite).Data;

                    if(ImageID == 254)
                    {
                        Dir += 16;
                    }
                    Images.Add(new CImage(unitclass, Images, ImageID, Dir, player, _parentImage: p, _drawType: drawType, level: Level + 1, _StartAnim: startanim));
                }
            }



            public int BoxWidth;
            public int BoxHeight;


            public uint unitclass;//u32: The unit's class instance (sort of a "serial number")
            public ushort x;//U16: X coordinate of unit
            public ushort y;//U16: Y coordinate of unit
            public ushort unitID;//u16: Unit ID
            public ushort linkFlag;//u16: Type of relation to another building (i.e. add-on, nydus link)
                                   //Bit 9 - Nydus Link
                                   //Bit 10 - Addon Link
            public ushort validstatusFlag;//u16: Flags of special properties which can be applied to the unit and are valid:
                                          //Bit 0 - Cloak is valid
                                          //Bit 1 - Burrow is valid
                                          //Bit 2 - In transit is valid
                                          //Bit 3 - Hallucinated is valid
                                          //Bit 4 - Invincible is valid
                                          //Bit 5-15 - Unused
            public ushort validunitFlag;//u16: Out of the elements of the unit data, the properties which can be changed by the map maker:
                                        //Bit 0 - Owner player is valid (the unit is not a critter, start location, etc.; not a neutral unit)
                                        //Bit 1 - HP is valid
                                        //Bit 2 - Shields is valid
                                        //Bit 3 - Energy is valid (unit is a wraith, etc.)
                                        //Bit 4 - Resource amount is valid (unit is a mineral patch, vespene geyser, etc.)
                                        //Bit 5 - Amount in hangar is valid (unit is a reaver, carrier, etc.)
                                        //Bit 6-15 - Unused
            public byte player;//u8: Player number of owner (0-based)
            public byte hitPoints;//u8: Hit points % (1-100)
            public byte shieldPoints;//u8: Shield points % (1-100)
            public byte energyPoints;//u8: Energy points % (1-100)
            public uint resoruceAmount;//u32: Resource amount
            public ushort hangar;//u16: Number of units in hangar
            public ushort stateFlag;//u16: Unit state flags
                                    //Bit 0 - Unit is cloaked
                                    //Bit 1 - Unit is burrowed
                                    //Bit 2 - Building is in transit
                                    //Bit 3 - Unit is hallucinated
                                    //Bit 4 - Unit is invincible
                                    //Bit 5-15 - Unused
            public uint unused;//u32: Unused
            public uint linkedUnit;//u32: Class instance of the unit to which this unit is related to (i.e. via an add-on, nydus link, etc.). It is "0" if the unit is not linked to any other unit.


            public List<CImage> Images;

        }



        public string[] LOADSTR;
        public string[] LOADSTRx;


        public List<byte[]> BYTESTR;
        public List<byte[]> BYTESTRx;
    }
}
