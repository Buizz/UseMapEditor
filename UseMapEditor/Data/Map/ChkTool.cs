using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseMapEditor.Dialog;
using static UseMapEditor.FileData.TileSet;

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






        public enum TOKENTYPE
        {
            NULL,
            ENCD,
            VER,
            TYPE,
            //IVER,//Map Version
            IVE2,
            VCOD,
            IOWN,
            OWNR,
            SIDE,
            UNIT,
            ERA,
            DIM,
            //지형
            TILE,
            MTXM,
            COLR,
            CRGB,
            STR,//String Data
            STRx,//String Data
            SPRP,//Scenario Properties              STR사용
            FORC,//Force Settings                   STR사용


            //ISOM,//Isometric Terrain
            //TODO:여기서부터 만들어야됨
            DD2,//StarEdit Sprites (Doodads)
            THG2,//StarCraft Sprites
            MASK,//Fog of War Layer
                 //지형


            //MRGN,//Locations                     STR사용
            
            //UPRP,//CUWP Slots
            //UPUS,//CUWP Slots Used
            //TRIG,//Triggers                      STR사용

            //MBRF,//Mission Briefings             STR사용
            //WAV,//WAV String Indexes             STR사용
            //SWNM,//Switch Names                  STR사용



            //PUNI,//Player Unit Restrictions

            //UPGR,//Upgrade Restrictions
            //PTEC,//Tech Restrictions

            //UNIS,//Unit Settings                 STR사용
            //UPGS,//Upgrade Settings
            //TECS,//Tech Settings
            //브르드워
            //PUPx,//BW Upgrade Restrictions
            //PTEx,//BW Tech Restrictions

            //UNIx,//BW Unit Settings              STR사용
            //UPGx,//BW Upgrade Settings
            //TECx,//BW Tech Settings
        }


        private void GetCHKAll(BinaryWriter bw)
        {
            LoadString();

            DDDTHG2.Clear();
            MTXM = (ushort[])TILE.Clone();
            //Doodad풀기
            for (int i = 0; i < DD2.Count; i++)
            {
                CDD2 cDD2 = DD2[i];

                DoodadPallet pallete = UseMapEditor.Global.WindowTool.MapViewer.tileSet.DoodadPallets[TILETYPE][cDD2.ID];

                int _x = cDD2.X / 32 - (pallete.dddWidth / 2);
                int _y = cDD2.Y / 32 - (pallete.dddHeight / 2);


                for (int y = 0; y < pallete.dddHeight; y++)
                {
                    for (int x = 0; x < pallete.dddWidth; x++)
                    {
                        ushort group = (ushort)(pallete.dddGroup + y);
                        ushort index = (ushort)x;


                        if (UseMapEditor.Global.WindowTool.MapViewer.tileSet.IsBlack(TILETYPE, group, index))
                        {
                            continue;
                        }

                        MTXM[_x + x + (_y + y) * WIDTH] = (ushort)((group << 4) + index);
                    }
                }

                CTHG2 cTHG2 = new CTHG2();
                cTHG2.FLAG = pallete.dddFlags;
                cTHG2.X = cDD2.X;
                cTHG2.Y = cDD2.Y;
                cTHG2.ID = pallete.dddOverlayID;

                DDDTHG2.Add(cTHG2);
            }


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
                    bw.Write(cHKToken.bytes);

                    break;
                case TOKENTYPE.ENCD:
                    bw.Write(tokenbyte);
                    bw.Write((int)4);
                    bw.Write(ENCODING.CodePage);

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
                case TOKENTYPE.UNIT:
                    bw.Write(tokenbyte);
                    bw.Write(UNIT.Count * 36);
                    for (int i = 0; i < UNIT.Count; i++)
                    {
                        bw.Write(UNIT[i].unitclass);
                        bw.Write(UNIT[i].x);
                        bw.Write(UNIT[i].y);
                        bw.Write(UNIT[i].unitID);
                        bw.Write(UNIT[i].linkFlag);
                        bw.Write(UNIT[i].validstatusFlag);
                        bw.Write(UNIT[i].validunitFlag);
                        bw.Write(UNIT[i].player);
                        bw.Write(UNIT[i].hitPoints);
                        bw.Write(UNIT[i].shieldPoints);
                        bw.Write(UNIT[i].energyPoints);
                        bw.Write(UNIT[i].resoruceAmount);
                        bw.Write(UNIT[i].hangar);
                        bw.Write(UNIT[i].stateFlag);
                        bw.Write(UNIT[i].unused);
                        bw.Write(UNIT[i].linkedUnit);
                    }

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
                case TOKENTYPE.MTXM:
                    //TODO:TILE단락에다가 DD2등 합성해야됨
                    bw.Write(tokenbyte);
                    bw.Write((int)WIDTH * HEIGHT * 2);
                    for (int i = 0; i < WIDTH * HEIGHT; i++)
                    {
                        bw.Write(MTXM[i]);
                    }

                    break;
                case TOKENTYPE.DD2:
                    //TODO:TILE단락에다가 DD2등 합성해야됨
                    bw.Write(tokenbyte);
                    bw.Write((int)DD2.Count * 8);
                    for (int i = 0; i < DD2.Count; i++)
                    {
                        bw.Write(DD2[i].ID);
                        bw.Write(DD2[i].X);
                        bw.Write(DD2[i].Y);
                        bw.Write(DD2[i].PLAYER);
                        bw.Write(DD2[i].FLAG);
                    }

                    break;
                case TOKENTYPE.THG2:
                    //TODO:TILE단락에다가 DD2등 합성해야됨
                    bw.Write(tokenbyte);

                    DDDTHG2.AddRange(THG2);

                    bw.Write((int)DDDTHG2.Count * 10);
                    for (int i = 0; i < DDDTHG2.Count; i++)
                    {
                        bw.Write(DDDTHG2[i].ID);
                        bw.Write(DDDTHG2[i].X);
                        bw.Write(DDDTHG2[i].Y);
                        bw.Write(DDDTHG2[i].PLAYER);
                        bw.Write(DDDTHG2[i].UNUSED);
                        bw.Write(DDDTHG2[i].FLAG);
                    }

                    break;
                case TOKENTYPE.MASK:
                    //TODO:TILE단락에다가 DD2등 합성해야됨
                    bw.Write(tokenbyte);
                    bw.Write((int)MASK.Length);
                    bw.Write(MASK);

                    break;
                case TOKENTYPE.STRx:
                    bw.Write(tokenbyte);

                    {
                        int strptrlen = stringDatas.Count * 4 + 4;

                        MemoryStream memory = new MemoryStream();
                        BinaryWriter tbw = new BinaryWriter(memory);

                        uint[] strptr = new uint[stringDatas.Count];
                        for (int i = 0; i < stringDatas.Count; i++)
                        {
                            byte[] bytes = ENCODING.GetBytes(stringDatas[i].String);
                            strptr[i] = (uint)((uint)tbw.BaseStream.Position + strptrlen);
                            tbw.Write(bytes);
                            tbw.Write((byte)0);
                        }


                        bw.Write((uint)(tbw.BaseStream.Length + strptrlen));
                        bw.Write((uint)stringDatas.Count);
                        for (int i = 0; i < stringDatas.Count; i++)
                        {
                            bw.Write(strptr[i]);
                        }
                        bw.Write(memory.ToArray());
                        tbw.Close();
                        memory.Close();
                    }





                    //long strptrstart = bw.BaseStream.Position;


                    //bw.Write((uint)0);


                    //long ptrpos = bw.BaseStream.Position;


                    //for (int i = 0; i < stringDatas.Count; i++)
                    //{
                    //    bw.Write((uint)0);
                    //}


                    //long endpos = bw.BaseStream.Position;
                    //bw.BaseStream.Position = strptrstart;
                    //bw.Write((uint)endpos - strptrstart);


                    //for (int i = 0; i < stringDatas.Count; i++)
                    //{
                    //    bw.Write(strptr[i]);
                    //}


                    //bw.BaseStream.Position = endpos;
                    break;
                case TOKENTYPE.SPRP:
                    bw.Write(tokenbyte);
                    bw.Write((int)4);
                    bw.Write((ushort)SCEARIONAME.ResultIndex);
                    bw.Write((ushort)SCEARIODES.ResultIndex);

                    break;
                case TOKENTYPE.FORC:
                    bw.Write(tokenbyte);
                    bw.Write((int)20);
                    bw.Write(FORCE);
                    bw.Write((ushort)FORCENAME[0].ResultIndex);
                    bw.Write((ushort)FORCENAME[1].ResultIndex);
                    bw.Write((ushort)FORCENAME[2].ResultIndex);
                    bw.Write((ushort)FORCENAME[3].ResultIndex);
                    bw.Write(FORCEFLAG);
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


            if(TILE == null)
            {
                TILE = (ushort[])MTXM.Clone();
            }


            if (BYTESTRx == null)
            {
                BYTESTRx = BYTESTR;
            }

            if (LOADSTRx == null)
            {
                LOADSTRx = (string[])LOADSTR.Clone();
            }


            //만약 인코딩이 없을 경우
            if(ENCODING == null)
            {
                ENCODING = System.Text.Encoding.UTF8;

                EncodingSelectDialog encodingSelectDialog = new EncodingSelectDialog(this);
                encodingSelectDialog.ShowDialog();
            }

            for (int i = 0; i < BYTESTRx.Count; i++)
            {
                LOADSTRx[i] = ENCODING.GetString(BYTESTRx[i]);
            }



            for (int i = 0; i < DD2.Count; i++)
            {
                DoodadPallet pallete = UseMapEditor.Global.WindowTool.MapViewer.tileSet.DoodadPallets[TILETYPE][DD2[i].ID]; ;




                if (THG2.Exists(x => (x.X == DD2[i].X) & (x.Y == DD2[i].Y)))
                {
                    CTHG2 s = THG2.Find(x => (x.X == DD2[i].X) & (x.Y == DD2[i].Y));

                    if (s.ID == pallete.dddOverlayID)
                    {
                        THG2.Remove(s);
                    }
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
                case TOKENTYPE.ENCD:
                    SetEncoding(br.ReadInt32());

                    break;
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
                    for (int i = 0; i < 8; i++)
                    {
                        CRGB[i] = new Microsoft.Xna.Framework.Color(0, 0, COLR[i]);
                    }


                    break;
                case TOKENTYPE.CRGB:
                    for (int i = 0; i < 8; i++)
                    {
                        byte[] colors = br.ReadBytes(3);
                        CRGB[i] = new Microsoft.Xna.Framework.Color(colors[0], colors[1], colors[2]);
                    }
                    CRGBIND = br.ReadBytes(8);

                    break;
                case TOKENTYPE.UNIT:
                    UNIT.Clear();
                    for (int i = 0; i < cHKToken.size / 36; i++)
                    {
                        UNIT.Add(new CUNIT(br));
                    }

                    break;
                case TOKENTYPE.DIM:
                    WIDTH = br.ReadUInt16();
                    HEIGHT = br.ReadUInt16();
                    break;
                case TOKENTYPE.TILE:
                    TILE = new ushort[cHKToken.size / 2];
                    for (int i = 0; i < cHKToken.size / 2; i++)
                    {
                        TILE[i] = br.ReadUInt16();
                    }

                    break;
                case TOKENTYPE.MTXM:
                    MTXM = new ushort[cHKToken.size / 2];
                    for (int i = 0; i < cHKToken.size / 2; i++)
                    {
                        MTXM[i] = br.ReadUInt16();
                    }

                    break;
                case TOKENTYPE.DD2:
                    for (int i = 0; i < cHKToken.size / 8; i++)
                    {
                        CDD2 cDD2 = new CDD2(br,this);

                        DD2.Add(cDD2);
                    }
                    break;
                case TOKENTYPE.THG2:
                    for (int i = 0; i < cHKToken.size / 10; i++)
                    {
                        CTHG2 cTHG2 = new CTHG2(br);

                        THG2.Add(cTHG2);
                    }

                    break;
                case TOKENTYPE.MASK:
                    MASK = br.ReadBytes((int)cHKToken.size);

                    break;
                case TOKENTYPE.STR:
                    {
                        long startpoint = br.BaseStream.Position;

                        LOADSTR = new string[br.ReadUInt16()];
                        BYTESTR = new List<byte[]>();

                        ushort[] ptrs = new ushort[LOADSTR.Length];
                        for (int i = 0; i < ptrs.Length; i++)
                        {
                            ptrs[i] = br.ReadUInt16();
                        }


                        for (int i = 0; i < ptrs.Length; i++)
                        {
                            br.BaseStream.Position = startpoint + ptrs[i];

                            List<byte> strs = new List<byte>();
                            byte readbyte = br.ReadByte();


                            if (readbyte != 0)
                            {
                                strs.Add(readbyte);
                                while (true)
                                {
                                    readbyte = br.ReadByte();
                                    if (readbyte == 0)
                                    {
                                        break;
                                    }
                                    strs.Add(readbyte);
                                }
                            }


                            BYTESTR.Add(strs.ToArray());

                            //LOADSTR[i] = System.Text.Encoding.GetEncoding(949).GetString(strs.ToArray());
                        }
                    }
                    break;
                case TOKENTYPE.STRx:
                    {
                        long startpoint = br.BaseStream.Position;

                        LOADSTRx = new string[br.ReadUInt32()];
                        BYTESTRx = new List<byte[]>();

                        uint[] ptrs = new uint[LOADSTRx.Length];
                        for (int i = 0; i < ptrs.Length; i++)
                        {
                            ptrs[i] = br.ReadUInt32();
                        }


                        for (int i = 0; i < ptrs.Length; i++)
                        {
                            br.BaseStream.Position = startpoint + ptrs[i];

                            List<byte> strs = new List<byte>();
                            byte readbyte = br.ReadByte();

                            if (readbyte != 0)
                            {
                                strs.Add(readbyte);
                                while (true)
                                {
                                    readbyte = br.ReadByte();
                                    if (readbyte == 0)
                                    {
                                        break;
                                    }
                                    strs.Add(readbyte);
                                }
                            }


                            BYTESTRx.Add(strs.ToArray());

                            //LOADSTRx[i] = System.Text.Encoding.UTF8.GetString(strs.ToArray());
                        }
                    }
                    break;
                case TOKENTYPE.SPRP:
                    SCEARIONAME = new StringData(this, br.ReadUInt16());
                    SCEARIODES = new StringData(this, br.ReadUInt16());


                    break;
                case TOKENTYPE.FORC:
                    FORCE = br.ReadBytes(8);

                    FORCENAME = new StringData[4];
                    FORCENAME[0] = new StringData(this, br.ReadUInt16());
                    FORCENAME[1] = new StringData(this, br.ReadUInt16());
                    FORCENAME[2] = new StringData(this, br.ReadUInt16());
                    FORCENAME[3] = new StringData(this, br.ReadUInt16());

                    FORCEFLAG = br.ReadBytes(4);
                    break;


                    /*
                    DD2,//StarEdit Sprites (Doodads)
                    THG2,//StarCraft Sprites
                    MASK,//Fog of War Layer


                    UPRP,//CUWP Slots
                    UPUS,//CUWP Slots Used
                    TRIG,//Triggers                      STR사용

                    MBRF,//Mission Briefings             STR사용
                    WAV,//WAV String Indexes             STR사용
                    SWNM,//Switch Names                  STR사용
                    MRGN,//Locations                     STR사용



                    PUNI,//Player Unit Restrictions

                    UPGR,//Upgrade Restrictions
                    PTEC,//Tech Restrictions
                    UNIS,//Unit Settings                 STR사용
                    UPGS,//Upgrade Settings
                    TECS,//Tech Settings
                    //브르드워
                    PUPx,//BW Upgrade Restrictions
                    PTEx,//BW Tech Restrictions
                    UNIx,//BW Unit Settings              STR사용
                    UPGx,//BW Upgrade Settings
                    TECx,//BW Tech Settings
                    */

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
                br.BaseStream.Position = cHKDATA.start - 8;
                cHKDATA.bytes = br.ReadBytes((int)cHKDATA.size + 8);

                br.BaseStream.Position = cHKDATA.end;
            }


            return cHKDATA;
        }
    }
}
