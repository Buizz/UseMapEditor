using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Map
{
    public partial class MapData
    {

        private List<CHKToken> cHKTokens;
        public struct CHKToken
        {
            public string code;
            public TOKENTYPE tokentype;
            public long start;
            public long end;

            public long size;
            public byte[] bytes;
        }





        public void MapDataReset()
        {
            for (int i = 0; i < 8; i++)
            {
                CRGB[i] = new Microsoft.Xna.Framework.Color(0, 0, 0);

                CRGBIND[i] = (byte)CRGBINDTYPE.UseCOLRselection;
                COLR[i] = (byte)i;
            }
        }



        public enum TOKENTYPE
        {
            NULL,
            VER,
            TYPE,
            IVE2,
            VCOD,
            IOWN,
            OWNR,
            SIDE,

            COLR,
            CRGB,

            ERA,
            DIM,
            TILE
        }


        private void GetCHKAll(BinaryWriter bw)
        {
            //에디터로부터 CHK데이터를 가져오는 함수
            for (int i = 0; i < cHKTokens.Count; i++)
            {
                if(cHKTokens[i].tokentype == TOKENTYPE.NULL)
                {
                    GetCHK(bw, cHKTokens[i], TOKENTYPE.NULL);
                }
            }


            foreach (TOKENTYPE cHKTYPE in Enum.GetValues(typeof(TOKENTYPE)))
            {

                if (cHKTYPE != TOKENTYPE.NULL)
                {
                    GetCHK(bw, new CHKToken(), cHKTYPE);
                }
            }
        }


        private void GetCHK(BinaryWriter bw, CHKToken cHKToken, TOKENTYPE tOKENTYPE)
        {
            byte[] tokenbyte;

            if (tOKENTYPE == TOKENTYPE.NULL)
            {
                tokenbyte = System.Text.Encoding.ASCII.GetBytes(cHKToken.code.PadRight(4, ' '));
            }
            else
            {
                tokenbyte = System.Text.Encoding.ASCII.GetBytes(tOKENTYPE.ToString().PadRight(4, ' '));
            }


            //에디터로부터 CHK데이터를 가져오는 함수
            switch (tOKENTYPE)
            {
                case TOKENTYPE.NULL:
                    bw.Write(tokenbyte);
                    bw.Write((int)cHKToken.size);
                    bw.Write(cHKToken.bytes);

                    break;
                case TOKENTYPE.VER:
                    bw.Write(tokenbyte);
                    bw.Write((int)2);
                    bw.Write(VER);

                    break;
                case TOKENTYPE.TYPE:
                    bw.Write(tokenbyte);
                    bw.Write((int)4);
                    bw.Write(TYPE);

                    break;
                case TOKENTYPE.IVE2:
                    bw.Write(tokenbyte);
                    bw.Write((int)2);
                    bw.Write(IVE2);

                    break;
                case TOKENTYPE.VCOD:
                    bw.Write(tokenbyte);
                    bw.Write(VCOD.Length);
                    bw.Write(VCOD);

                    break;
                case TOKENTYPE.IOWN:
                    bw.Write(tokenbyte);
                    bw.Write(IOWN.Length);
                    bw.Write(IOWN);

                    break;
                case TOKENTYPE.OWNR:
                    //TODO:스타트로케이션 확인 후 IOWN수정해야됨.
                    bw.Write(tokenbyte);
                    bw.Write(IOWN.Length);
                    bw.Write(IOWN);

                    break;
                case TOKENTYPE.SIDE:
                    bw.Write(tokenbyte);
                    bw.Write(SIDE.Length);
                    bw.Write(SIDE);

                    break;
                case TOKENTYPE.COLR:
                    bw.Write(tokenbyte);
                    bw.Write(COLR.Length);
                    bw.Write(COLR);

                    break;
                case TOKENTYPE.CRGB:
                    bw.Write(tokenbyte);
                    bw.Write(32);
                    for (int i = 0; i < 8; i++)
                    {
                        bw.Write(CRGB[i].R);
                        bw.Write(CRGB[i].G);
                        bw.Write(CRGB[i].B);
                    }
                    bw.Write(CRGBIND);

                    break;
                case TOKENTYPE.ERA:
                    bw.Write(tokenbyte);
                    bw.Write((int)2);
                    bw.Write((ushort)TILETYPE);
                    
                    break;
                case TOKENTYPE.DIM:
                    bw.Write(tokenbyte);
                    bw.Write((int)4);
                    bw.Write((ushort)WIDTH);
                    bw.Write((ushort)HEIGHT);

                    break;
                case TOKENTYPE.TILE:
                    bw.Write(tokenbyte);
                    bw.Write((int)WIDTH * HEIGHT * 2);
                    for (int i = 0; i < WIDTH * HEIGHT; i++)
                    {
                        bw.Write(TILE[i]);
                    }

                    break;
            }
        }

        private bool ApplychkAll(BinaryReader br)
        {
            cHKTokens = new List<CHKToken>();

            br.BaseStream.Position = 0;

            while (br.BaseStream.Position < br.BaseStream.Length)
            {
               if (!Applychk(br))
                {
                    return false;
                }
        
            }


            ////Byte로부터 에디터로 넣는 함수
            //foreach (TOKENTYPE cHKTYPE in Enum.GetValues(typeof(TOKENTYPE)))
            //{
            //    Applychk(br, cHKTYPE);
            //}


            return true;
        }
        private bool Applychk(BinaryReader br)
        {
            CHKToken cHKToken = GetNextCHK(br);
            br.BaseStream.Position = cHKToken.start;


            //Byte로부터 에디터로 넣는 함수
            switch (cHKToken.tokentype)
            {
                case TOKENTYPE.IOWN:
                    IOWN = br.ReadBytes(12);

                    break;
                case TOKENTYPE.OWNR:
                    //넘기기

                    break;
                case TOKENTYPE.SIDE:
                    SIDE = br.ReadBytes(12);

                    break;
                case TOKENTYPE.ERA:
                    TILETYPE = (UseMapEditor.FileData.TileSet.TileType)br.ReadUInt16();

                    break;
                case TOKENTYPE.COLR:
                    COLR = br.ReadBytes(8);

                    break;
                case TOKENTYPE.CRGB:
                    for (int i = 0; i < 8; i++)
                    {
                        byte[] colors = br.ReadBytes(3);
                        CRGB[i] = new Microsoft.Xna.Framework.Color(colors[0], colors[1], colors[2]);
                    }
                    CRGBIND = br.ReadBytes(8);

                    break;
                case TOKENTYPE.DIM:
                    WIDTH = br.ReadUInt16();
                    HEIGHT = br.ReadUInt16();
                    break;
                case TOKENTYPE.TILE:
                    TILE = new ushort[WIDTH * HEIGHT];
                    for (int i = 0; i < WIDTH * HEIGHT; i++)
                    {
                        TILE[i] = br.ReadUInt16();
                    }

                    break;
            }






            for (int i = 0; i < cHKTokens.Count; i++)
            {
                if(cHKTokens[i].code == cHKToken.code)
                {
                    cHKTokens.RemoveAt(i);
                    break;
                }
            }
            cHKTokens.Add(cHKToken);
            br.BaseStream.Position = cHKToken.end;
            return true;
        }


        private CHKToken GetNextCHK(BinaryReader br)
        {
            CHKToken cHKDATA = new CHKToken();

            cHKDATA.tokentype = TOKENTYPE.NULL;

            cHKDATA.code = System.Text.Encoding.ASCII.GetString(br.ReadBytes(4)).Replace(" ", "");

            cHKDATA.size = br.ReadInt32();


            cHKDATA.start = br.BaseStream.Position;
            br.BaseStream.Seek(cHKDATA.size, SeekOrigin.Current);
            cHKDATA.end = br.BaseStream.Position;


            foreach (TOKENTYPE cHKTYPE in Enum.GetValues(typeof(TOKENTYPE)))
            {
                if(cHKDATA.code == cHKTYPE.ToString())
                {
                    cHKDATA.tokentype = cHKTYPE;
                    break;
                }
            }
            if(cHKDATA.tokentype == TOKENTYPE.NULL)
            {
                br.BaseStream.Position = cHKDATA.start;
                cHKDATA.bytes = br.ReadBytes((int)cHKDATA.size);

                br.BaseStream.Position = cHKDATA.end;
            }


            return cHKDATA;
        }
    }
}
