using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
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


            DD2,//StarEdit Sprites (Doodads)
            THG2,//StarCraft Sprites
            MASK,//Fog of War Layer
                 //지형


            MRGN,//Locations                     STR사용

            UPRP,//CUWP Slots
            UPUS,//CUWP Slots Used
            WAV,//WAV String Indexes             STR사용
            SWNM,//Switch Names                  STR사용


            TRIG,//Triggers                      STR사용
            MBRF,//Mission Briefings             STR사용



            PUNI,//Player Unit Restrictions
            PUPx,//BW Upgrade Restrictions
            PTEx,//BW Tech Restrictions
            UNIx,// - Unit Settings                 STR사용
            UPGx,//BW Upgrade Settings
            TECx,//BW Tech Settings
        }

        public void DD2DeleteMTXM(CDD2 cDD2)
        {
            DoodadPallet pallete = UseMapEditor.Global.WindowTool.MapViewer.tileSet.DoodadPallets[TILETYPE][cDD2.ID];

            int _x = cDD2.X / 32 - (pallete.dddWidth / 2);
            int _y = cDD2.Y / 32 - (pallete.dddHeight / 2);


            for (int y = 0; y < pallete.dddHeight; y++)
            {

                for (int x = 0; x < pallete.dddWidth; x++)
                {
                    if (!((0 <= _x + x && _x + x < WIDTH) && (0 <= _y + y && _y + y < HEIGHT)))
                    {
                        continue;
                    }

                    ushort group = (ushort)(pallete.dddGroup + y);
                    ushort index = (ushort)x;


                    if (UseMapEditor.Global.WindowTool.MapViewer.tileSet.IsBlack(TILETYPE, group, index))
                    {
                        continue;
                    }

                    MTXM[_x + x + (_y + y) * WIDTH] = TILE[_x + x + (_y + y) * WIDTH];
                }
            }
        }


        public void DD2ToMTXM(CDD2 cDD2)
        {
            DoodadPallet pallete = UseMapEditor.Global.WindowTool.MapViewer.tileSet.DoodadPallets[TILETYPE][cDD2.ID];

            int _x = cDD2.X / 32 - (pallete.dddWidth / 2);
            int _y = cDD2.Y / 32 - (pallete.dddHeight / 2);


            for (int y = 0; y < pallete.dddHeight; y++)
            {

                for (int x = 0; x < pallete.dddWidth; x++)
                {
                    if (!((0 <= _x + x && _x + x < WIDTH) && (0 <= _y + y && _y + y < HEIGHT)))
                    {
                        continue;
                    }


                    ushort group = (ushort)(pallete.dddGroup + y);
                    ushort index = (ushort)x;


                    if (UseMapEditor.Global.WindowTool.MapViewer.tileSet.IsBlack(TILETYPE, group, index))
                    {
                        continue;
                    }

                    MTXM[_x + x + (_y + y) * WIDTH] = (ushort)((group << 4) + index);
                }
            }
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
                DD2ToMTXM(cDD2);

                CTHG2 cTHG2 = new CTHG2();
                cTHG2.FLAG = pallete.dddFlags;
                cTHG2.X = cDD2.X;
                cTHG2.Y = cDD2.Y;
                cTHG2.ID = pallete.dddOverlayID;

                if (((cTHG2.FLAG & (0b1 << 12)) != 0) | ((cTHG2.FLAG & (0b1 << 13)) != 0))
                {
                    DDDTHG2.Add(cTHG2);
                }
            }




            TriggerSave();




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
                    OWNR = (byte[])IOWN.Clone();

                    bool[] pexist = new bool[8];
                    for (int i = 0; i < UNIT.Count; i++)
                    {
                        if(UNIT[i].unitID == 214)
                        {
                            pexist[UNIT[i].player] = true;
                        }
                    }
                    for (int i = 0; i < 8; i++)
                    {
                        if (!pexist[i])
                        {
                            OWNR[i] = 0;
                        }
                    }


                    bw.Write(tokenbyte);
                    bw.Write(OWNR.Length);
                    bw.Write(OWNR);

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
                        bw.Write(UNIT[i].X);
                        bw.Write(UNIT[i].Y);
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
                    bw.Write(tokenbyte);
                    bw.Write((int)WIDTH * HEIGHT * 2);
                    for (int i = 0; i < WIDTH * HEIGHT; i++)
                    {
                        bw.Write(MTXM[i]);
                    }

                    break;
                case TOKENTYPE.DD2:
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
                            byte[] bytes = ENCODING.GetBytes(stringDatas[i].CodeString);
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
                case TOKENTYPE.MRGN:
                    bw.Write(tokenbyte);
                    bw.Write((int)5100);

                    for (int i = 0; i < 255; i++)
                    {
                        LocationData locationData = GetLocationFromLocIndex(i + 1);
                        
                        if (locationData == null || !locationData.IsEnabled)
                        {
                            bw.Write(new byte[20]);
                        }
                        else
                        {
                            bw.Write(locationData.L);
                            bw.Write(locationData.T);
                            bw.Write(locationData.R);
                            bw.Write(locationData.B);
                            bw.Write((ushort)locationData.STRING.ResultIndex);
                            bw.Write(locationData.FLAG);
                        }
                    }

                    break;

                case TOKENTYPE.UPRP:
                    bw.Write(tokenbyte);
                    bw.Write((int)UPRP.Length * 20);
                    for (int i = 0; i < UPRP.Length; i++)
                    {
                        bw.Write(UPRP[i].STATUSVALID);
                        bw.Write(UPRP[i].POINTVALID);
                        bw.Write(UPRP[i].PLAYER);
                        bw.Write(UPRP[i].HITPOINT);
                        bw.Write(UPRP[i].SHIELDPOINT);
                        bw.Write(UPRP[i].ENERGYPOINT);
                        bw.Write(UPRP[i].RESOURCE);
                        bw.Write(UPRP[i].HANGAR);
                        bw.Write(UPRP[i].STATUSFLAG);
                        bw.Write(UPRP[i].UNUSED);
                    }
                    break;
                case TOKENTYPE.UPUS:
                    bw.Write(tokenbyte);
                    bw.Write((int)UPUS.Length);
                    bw.Write(UPUS);
                    break;
                case TOKENTYPE.WAV:
                    bw.Write(tokenbyte);
                    bw.Write((int)WAV.Count * 4);

                    for (int i = 0; i < WAV.Count; i++)
                    {
                        bw.Write(WAV[i].ResultIndex);
                    }
                    break;
                case TOKENTYPE.SWNM:
                    bw.Write(tokenbyte);
                    bw.Write((int)SWNM.Length * 4);

                    for (int i = 0; i < SWNM.Length; i++)
                    {
                        bw.Write(SWNM[i].ResultIndex);
                    }
                    break;
                case TOKENTYPE.PUNI:
                    bw.Write(tokenbyte);
                    bw.Write((int)5700);
                    for (int i = 0; i < 12; i++)
                    {
                        bw.Write(PUNI.UNITENABLED[i]);
                    }
                    bw.Write(PUNI.DEFAULT);
                    for (int i = 0; i < 12; i++)
                    {
                        bw.Write(PUNI.USEDEFAULT[i]);
                    }

                    break;
                case TOKENTYPE.PUPx:
                    bw.Write(tokenbyte);
                    bw.Write((int)2318);
                    for (int i = 0; i < 12; i++)
                    {
                        bw.Write(PUPx.MAXLEVEL[i]);
                    }
                    for (int i = 0; i < 12; i++)
                    {
                        bw.Write(PUPx.STARTLEVEL[i]);
                    }
                    bw.Write(PUPx.DEFAULTMAXLEVEL);
                    bw.Write(PUPx.DEFAULTSTARTLEVEL);
                    for (int i = 0; i < 12; i++)
                    {
                        bw.Write(PUPx.USEDEFAULT[i]);
                    }

                    break;
                case TOKENTYPE.PTEx:
                    bw.Write(tokenbyte);
                    bw.Write((int)1672);
                    for (int i = 0; i < 12; i++)
                    {
                        bw.Write(PTEx.MAXLEVEL[i]);
                    }
                    for (int i = 0; i < 12; i++)
                    {
                        bw.Write(PTEx.STARTLEVEL[i]);
                    }
                    bw.Write(PTEx.DEFAULTMAXLEVEL);
                    bw.Write(PTEx.DEFAULTSTARTLEVEL);
                    for (int i = 0; i < 12; i++)
                    {
                        bw.Write(PTEx.USEDEFAULT[i]);
                    }

                    break;
                case TOKENTYPE.UNIx:
                    bw.Write(tokenbyte);
                    bw.Write((int)4168);
                    for (int i = 0; i < 228; i++)
                        bw.Write(UNIx.USEDEFAULT[i]);

                    for (int i = 0; i < 228; i++)
                        bw.Write(UNIx.HIT[i]);
                    for (int i = 0; i < 228; i++)
                        bw.Write(UNIx.SHIELD[i]);
                    for (int i = 0; i < 228; i++)
                        bw.Write(UNIx.ARMOR[i]);
                    for (int i = 0; i < 228; i++)
                        bw.Write(UNIx.BUILDTIME[i]);
                    for (int i = 0; i < 228; i++)
                        bw.Write(UNIx.MIN[i]);
                    for (int i = 0; i < 228; i++)
                        bw.Write(UNIx.GAS[i]);
                    for (int i = 0; i < 228; i++)
                        bw.Write((ushort)UNIx.STRING[i].ResultIndex);
                    for (int i = 0; i < 130; i++)
                        bw.Write(UNIx.DMG[i]);
                    for (int i = 0; i < 130; i++)
                        bw.Write(UNIx.BONUSDMG[i]);

                    break;
                case TOKENTYPE.UPGx:
                    bw.Write(tokenbyte);
                    bw.Write((int)794);
                    for (int i = 0; i < 61; i++)
                        bw.Write(UPGx.USEDEFAULT[i]);
                    bw.Write((byte)0);
                    for (int i = 0; i < 61; i++)
                        bw.Write(UPGx.BASEMIN[i]);
                    for (int i = 0; i < 61; i++)
                        bw.Write(UPGx.BONUSMIN[i]);
                    for (int i = 0; i < 61; i++)
                        bw.Write(UPGx.BASEGAS[i]);
                    for (int i = 0; i < 61; i++)
                        bw.Write(UPGx.BONUSGAS[i]);
                    for (int i = 0; i < 61; i++)
                        bw.Write(UPGx.BASETIME[i]);
                    for (int i = 0; i < 61; i++)
                        bw.Write(UPGx.BONUSTIME[i]);

                    break;
                case TOKENTYPE.TECx:
                    bw.Write(tokenbyte);
                    bw.Write((int)396);
                    for (int i = 0; i < 44; i++)
                        bw.Write(TECx.USEDEFAULT[i]);
                    for (int i = 0; i < 44; i++)
                        bw.Write(TECx.MIN[i]);
                    for (int i = 0; i < 44; i++)
                        bw.Write(TECx.GAS[i]);
                    for (int i = 0; i < 44; i++)
                        bw.Write(TECx.BASETIME[i]);
                    for (int i = 0; i < 44; i++)
                        bw.Write(TECx.ENERGY[i]);

                    break;
                case TOKENTYPE.TRIG:
                    bw.Write(tokenbyte);
                    bw.Write((int)2400 * TRIG.Count);
                    for (int i = 0; i < TRIG.Count; i++)
                    {
                        for (int c = 0; c < 16; c++)
                        {
                            bw.Write(TRIG[i].conditions[c].locid);
                            bw.Write(TRIG[i].conditions[c].player);
                            bw.Write(TRIG[i].conditions[c].amount);
                            bw.Write(TRIG[i].conditions[c].unitid);
                            bw.Write(TRIG[i].conditions[c].comparison);
                            bw.Write(TRIG[i].conditions[c].condtype);
                            bw.Write(TRIG[i].conditions[c].restype);
                            bw.Write(TRIG[i].conditions[c].flags);
                            bw.Write(TRIG[i].conditions[c].maskflag);
                        }
                        for (int a = 0; a < 64; a++)
                        {
                            bw.Write(TRIG[i].actions[a].locid1);
                            bw.Write(TRIG[i].actions[a].strid);
                            bw.Write(TRIG[i].actions[a].wavid);
                            bw.Write(TRIG[i].actions[a].time);
                            bw.Write(TRIG[i].actions[a].player1);
                            bw.Write(TRIG[i].actions[a].player2);
                            bw.Write(TRIG[i].actions[a].unitid);
                            bw.Write(TRIG[i].actions[a].acttype);
                            bw.Write(TRIG[i].actions[a].amount);
                            bw.Write(TRIG[i].actions[a].flags);
                            bw.Write(TRIG[i].actions[a].padding);
                            bw.Write(TRIG[i].actions[a].maskflag);
                        }

                        bw.Write(TRIG[i].exeflag);
                        bw.Write(TRIG[i].playerlist);
                        bw.Write(TRIG[i].trigindex);
                    }


                    break;
                case TOKENTYPE.MBRF:
                    bw.Write(tokenbyte);
                    bw.Write((int)2400 * MBRF.Count);
                    for (int i = 0; i < MBRF.Count; i++)
                    {
                        for (int c = 0; c < 16; c++)
                        {
                            bw.Write(MBRF[i].conditions[c].locid);
                            bw.Write(MBRF[i].conditions[c].player);
                            bw.Write(MBRF[i].conditions[c].amount);
                            bw.Write(MBRF[i].conditions[c].unitid);
                            bw.Write(MBRF[i].conditions[c].comparison);
                            bw.Write(MBRF[i].conditions[c].condtype);
                            bw.Write(MBRF[i].conditions[c].restype);
                            bw.Write(MBRF[i].conditions[c].flags);
                            bw.Write(MBRF[i].conditions[c].maskflag);
                        }
                        for (int a = 0; a < 64; a++)
                        {
                            bw.Write(MBRF[i].actions[a].locid1);
                            bw.Write(MBRF[i].actions[a].strid);
                            bw.Write(MBRF[i].actions[a].wavid);
                            bw.Write(MBRF[i].actions[a].time);
                            bw.Write(MBRF[i].actions[a].player1);
                            bw.Write(MBRF[i].actions[a].player2);
                            bw.Write(MBRF[i].actions[a].unitid);
                            bw.Write(MBRF[i].actions[a].acttype);
                            bw.Write(MBRF[i].actions[a].amount);
                            bw.Write(MBRF[i].actions[a].flags);
                            bw.Write(MBRF[i].actions[a].padding);
                            bw.Write(MBRF[i].actions[a].maskflag);
                        }

                        bw.Write(MBRF[i].exeflag);
                        bw.Write(MBRF[i].playerlist);
                        bw.Write(MBRF[i].trigindex);
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

            if (IOWN == null)
            {
                //IOWN = (byte[])OWNR.Clone();
                throw new Exception("");
            }

            //만약 인코딩이 없을 경우
            if (ENCODING == null)
            {
                ENCODING = System.Text.Encoding.UTF8;

                EncodingSelectDialog encodingSelectDialog = new EncodingSelectDialog(this);
                encodingSelectDialog.ShowDialog();
            }

            for (int i = 0; i < BYTESTRx.Count; i++)
            {
                //여기서 작업해준다.
                string s = ENCODING.GetString(BYTESTRx[i]);
                
                LOADSTRx[i] = UseMapEditor.Tools.StringTool.ReadRawString(s);
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


            TriggerLoad();


            //soundDatas
            ulong hmpq = OpenArchive();
            soundDatas.Clear();
            for (int i = 0; i < WAV.Count; i++)
            {
                string d = WAV[i].String;
                if (WAV[i].IsLoaded)
                {
                    SoundData soundData = new SoundData();
                    soundData.path = d;
                    soundData.bytes = ReadMPQFileC(hmpq, d);
                    if(soundData.bytes.Length != 0)
                    {
                        soundDatas.Add(soundData);
                    }
                }
            }
            CloseArchive(hmpq);


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
                    OWNR = br.ReadBytes(12);

                    break;
                case TOKENTYPE.SIDE:
                    SIDE = br.ReadBytes(12);

                    break;
                case TOKENTYPE.ERA:
                    ushort tile = br.ReadUInt16();
                    if(tile > 7)
                    {
                        throw new Exception("");
                    }
                    TILETYPE = (TileType)tile;


                    break;
                case TOKENTYPE.COLR:
                    COLR = br.ReadBytes(8);
                    for (int i = 0; i < 8; i++)
                    {
                        if (COLR[i] >= ColorName.Count())
                        {
                            COLR[i] = 0;
                        }


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
                        CUNIT cUNIT = new CUNIT(br);
                        cUNIT.SetMapEditor(mapEditor);
                        UNIT.Add(cUNIT);
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
                case TOKENTYPE.MRGN:
                    LocationInit(mapEditor);
                    for (int i = 1; i < LocationDatas.Count; i++)
                    {
                        LocationData locationData = GetLocationFromLocIndex(i);

                        //locationData.INDEX = i + 1;

                        locationData.L = br.ReadUInt32();
                        locationData.T = br.ReadUInt32();
                        locationData.R = br.ReadUInt32();
                        locationData.B = br.ReadUInt32();
                        locationData.STRING = new StringData(this, br.ReadUInt16());
                        locationData.FLAG = br.ReadUInt16();

                        if (locationData.L == 0 & locationData.T == 0 & locationData.L == 0 & locationData.T == 0 &
                            locationData.STRING.LoadedIndex == -1 & locationData.FLAG == 0)
                        {
                            continue;
                        }
                        locationData.Enable();
                    }

                    //LocationDatas.Clear();
                    //LocationDatas.Add(new LocationData(mapEditor));
                    //for (int i = 0; i < 255; i++)
                    //{
                    //    LocationData locationData = new LocationData(mapEditor);

                    //    locationData.INDEX = i + 1;

                    //    locationData.L = br.ReadUInt32();
                    //    locationData.T = br.ReadUInt32();
                    //    locationData.R = br.ReadUInt32();
                    //    locationData.B = br.ReadUInt32();
                    //    locationData.STRING = new StringData(this, br.ReadUInt16());
                    //    locationData.FLAG = br.ReadUInt16();

                    //    if(locationData.L == 0 & locationData.T == 0 & locationData.L == 0 & locationData.T == 0 &
                    //        locationData.STRING.LoadedIndex == -1 & locationData.FLAG == 0)
                    //    {
                    //        continue;
                    //    }


                    //    LocationDatas.Add(locationData);
                    //}
                    //u32: Left(X1) coordinate of location, in pixels(usually 32 pt grid aligned)
                    //u32: Top(Y1) coordinate of location, in pixels
                    //u32: Right(X2) coordinate of location, in pixels
                    //u32: Bottom(Y2) coordinate of location, in pixels
                    //u16: String number of the name of this location
                    //u16: Location elevation flags.If an elevation is disabled in the location, it's bit will be on (1)
                    //Bit 0 - Low elevation
                    //Bit 1 - Medium elevation
                    //Bit 2 - High elevation
                    //Bit 3 - Low air
                    //Bit 4 - Medium air
                    //Bit 5 - High air
                    //Bit 6 - 15 - Unused


                    break;
                case TOKENTYPE.UPRP:
                    UPRP = new CUPRP[64];
                    for (int i = 0; i < 64; i++)
                    {
                        CUPRP cUPRP = new CUPRP();
                        cUPRP.STATUSVALID = br.ReadUInt16();
                        cUPRP.POINTVALID = br.ReadUInt16();

                        cUPRP.PLAYER = br.ReadByte();
                        cUPRP.HITPOINT = br.ReadByte();
                        cUPRP.SHIELDPOINT = br.ReadByte();
                        cUPRP.ENERGYPOINT = br.ReadByte();


                        cUPRP.RESOURCE = br.ReadUInt32();
                        cUPRP.HANGAR = br.ReadUInt16();
                        cUPRP.STATUSFLAG = br.ReadUInt16();
                        cUPRP.UNUSED = br.ReadUInt32();

                        UPRP[i] = cUPRP;
                    }
                    break;
                case TOKENTYPE.UPUS:
                    UPUS = br.ReadBytes(64);
                    break;
                case TOKENTYPE.WAV:
                    WAV.Clear();
                    
                    for (int i = 0; i < cHKToken.size / 4; i++)
                    {
                        WAV.Add(new StringData(this, br.ReadInt32()));
                    }
                    break;
                case TOKENTYPE.SWNM:
                    SWNM = new StringData[256];
                    for (int i = 0; i < 256; i++)
                    {
                        SWNM[i] = new StringData(this, br.ReadInt32());
                    }
                    break;
                case TOKENTYPE.PUNI:
                    PUNI = new CPUNI();
                    for (int i = 0; i < 12; i++)
                    {
                        PUNI.UNITENABLED[i] = br.ReadBytes(228);
                    }
                    PUNI.DEFAULT = br.ReadBytes(228);
                    for (int i = 0; i < 12; i++)
                    {
                        PUNI.USEDEFAULT[i] = br.ReadBytes(228);
                    }

                    break;
                case TOKENTYPE.PUPx:
                    PUPx = new CPUPx();
                    for (int i = 0; i < 12; i++)
                    {
                        PUPx.MAXLEVEL[i] = br.ReadBytes(61);
                    }
                    for (int i = 0; i < 12; i++)
                    {
                        PUPx.STARTLEVEL[i] = br.ReadBytes(61);
                    }
                    PUPx.DEFAULTMAXLEVEL = br.ReadBytes(61);
                    PUPx.DEFAULTSTARTLEVEL = br.ReadBytes(61);
                    for (int i = 0; i < 12; i++)
                    {
                        PUPx.USEDEFAULT[i] = br.ReadBytes(61);
                    }

                    break;
                case TOKENTYPE.PTEx:
                    PTEx = new CPTEx();
                    for (int i = 0; i < 12; i++)
                    {
                        PTEx.MAXLEVEL[i] = br.ReadBytes(44);
                    }
                    for (int i = 0; i < 12; i++)
                    {
                        PTEx.STARTLEVEL[i] = br.ReadBytes(44);
                    }
                    PTEx.DEFAULTMAXLEVEL = br.ReadBytes(44);
                    PTEx.DEFAULTSTARTLEVEL = br.ReadBytes(44);
                    for (int i = 0; i < 12; i++)
                    {
                        PTEx.USEDEFAULT[i] = br.ReadBytes(44);
                    }

                    break;
                case TOKENTYPE.UNIx:
                    UNIx = new CUNIx();
                    for (int i = 0; i < 228; i++)
                        UNIx.USEDEFAULT[i] = br.ReadByte();

                    for (int i = 0; i < 228; i++)
                        UNIx.HIT[i] = br.ReadUInt32();
                    for (int i = 0; i < 228; i++)
                        UNIx.SHIELD[i] = br.ReadUInt16();
                    for (int i = 0; i < 228; i++)
                        UNIx.ARMOR[i] = br.ReadByte();
                    for (int i = 0; i < 228; i++)
                        UNIx.BUILDTIME[i] = br.ReadUInt16();
                    for (int i = 0; i < 228; i++)
                        UNIx.MIN[i] = br.ReadUInt16();
                    for (int i = 0; i < 228; i++)
                        UNIx.GAS[i] = br.ReadUInt16();
                    for (int i = 0; i < 228; i++)
                        UNIx.STRING[i] = new StringData(this, br.ReadUInt16());
                    for (int i = 0; i < 130; i++)
                        UNIx.DMG[i] = br.ReadUInt16();
                    for (int i = 0; i < 130; i++)
                        UNIx.BONUSDMG[i] = br.ReadUInt16();

                    break;
                case TOKENTYPE.UPGx:
                    UPGx = new CUPGx();
                    for (int i = 0; i < 61; i++)
                        UPGx.USEDEFAULT[i] = br.ReadByte();
                    br.ReadByte();
                    for (int i = 0; i < 61; i++)
                        UPGx.BASEMIN[i] = br.ReadUInt16();
                    for (int i = 0; i < 61; i++)
                        UPGx.BONUSMIN[i] = br.ReadUInt16();
                    for (int i = 0; i < 61; i++)
                        UPGx.BASEGAS[i] = br.ReadUInt16();
                    for (int i = 0; i < 61; i++)
                        UPGx.BONUSGAS[i] = br.ReadUInt16();
                    for (int i = 0; i < 61; i++)
                        UPGx.BASETIME[i] = br.ReadUInt16();
                    for (int i = 0; i < 61; i++)
                        UPGx.BONUSTIME[i] = br.ReadUInt16();

                    break;
                case TOKENTYPE.TECx:
                    TECx = new CTECx();
                    for (int i = 0; i < 44; i++)
                        TECx.USEDEFAULT[i] = br.ReadByte();
                    for (int i = 0; i < 44; i++)
                        TECx.MIN[i] = br.ReadUInt16();
                    for (int i = 0; i < 44; i++)
                        TECx.GAS[i] = br.ReadUInt16();
                    for (int i = 0; i < 44; i++)
                        TECx.BASETIME[i] = br.ReadUInt16();
                    for (int i = 0; i < 44; i++)
                        TECx.ENERGY[i] = br.ReadUInt16();

                    break;
                case TOKENTYPE.TRIG:
                    TRIG.Clear();

                    for (int i = 0; i < cHKToken.size/2400; i++)
                    {
                        RAWTRIGMBRF trig = new RAWTRIGMBRF(br);

                        TRIG.Add(trig);
                    }


                    break;
                case TOKENTYPE.MBRF:
                    MBRF.Clear();

                    for (int i = 0; i < cHKToken.size / 2400; i++)
                    {
                        RAWTRIGMBRF mbrf = new RAWTRIGMBRF(br);

                        MBRF.Add(mbrf);
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
                br.BaseStream.Position = cHKDATA.start - 8;
                cHKDATA.bytes = br.ReadBytes((int)cHKDATA.size + 8);

                br.BaseStream.Position = cHKDATA.end;
            }


            return cHKDATA;
        }
    }
}
