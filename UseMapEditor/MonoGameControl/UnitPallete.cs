using Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using SpriteFontPlus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using UseMapEditor.FileData;
using UseMapEditor.Global;
using UseMapEditor.Task.Events;
using WpfTest.Components;
using static Data.Map.MapData;
using static UseMapEditor.FileData.TileSet;
using Point = System.Windows.Point;

namespace UseMapEditor.MonoGameControl
{
    public partial class MapDrawer : WpfGame
    {
        private void UnitTaskStart()
        {
            mapeditor.taskManager.TaskStart();
            if (!GlobalVariable.key_LeftShiftDown)
            {
                mapeditor.SelectUnit.Clear();
            }
        }
        private void UnitDragEnd()
        {
            if (!mouse_IsLeftDrag)
            {
                UnitRightMouseClick();
                return;
            }

            if (mapeditor.mapDataBinding.UNIT_BRUSHMODE)
            {
                LastCreatePos = new Vector2(-100);
                mapeditor.taskManager.TaskEnd();
            }
            else
            {
                //선택모드
                if (!GlobalVariable.key_LeftShiftDown)
                {
                    mapeditor.SelectUnit.Clear();
                }
                mapeditor.SelectUnit.AddRange(hoverUnit);
                mapeditor.IndexedUnitSelectionChange();
            }
        }


        private void UnitClickEnd()
        {
            if (mapeditor.mapDataBinding.UNIT_BRUSHMODE)
            {
                LastCreatePos = new Vector2(-100);
                mapeditor.taskManager.TaskEnd();
            }
            else
            {
                mapeditor.SelectUnit.Clear();
                mapeditor.IndexedUnitSelectionChange();
            }
        }



        private void UnitRightMouseClick()
        {
            if (mapeditor.mapDataBinding.UNIT_BRUSHMODE)
            {
                mapeditor.mapDataBinding.UNIT_SELECTMODE = true;
            }
            else
            {
                //메뉴 열기
                mapeditor.OpenUnitMenu((int)MousePos.X, (int)MousePos.Y);
            }
        }



        private void UnitPaint()
        {
            int gridsize = mapeditor.opt_grid;

            if (mapeditor.mapDataBinding.UNIT_SELECTMODE)
            {
                return;
            }

            if (mouse_LeftDown)
            {
                if (mapeditor.unit_PasteMode)
                {
                    Vector2 mappos = MouseMapPos;
                    if ((LastCreatePos - mappos).Length() >= Math.Max(gridsize, 4))
                    {
                        LastCreatePos = mappos;
                        for (int i = 0; i < mapeditor.CopyedUnit.Count; i++)
                        {
                            mappos = MouseMapPos;
                            CUNIT cUNIT = mapeditor.CopyedUnit[i];
                            if (cUNIT.Images.Count == 0)
                            {
                                cUNIT.ImageReset();
                            }
                            byte sflag = (byte)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Special Ability Flags", cUNIT.unitID).Data;

                            bool IsBuilding = false;
                            if ((sflag & 0x1) > 0)
                            {
                                //건물
                                IsBuilding = true;
                            }
                            if (mapeditor.UnitPalleteGridFix)
                            {
                                //그리드 픽스
                                if (gridsize != 0)
                                {
                                    mappos.X = (float)(Math.Round(mappos.X / gridsize) * gridsize);
                                    mappos.Y = (float)(Math.Round(mappos.Y / gridsize) * gridsize);
                                }
                            }

                            mappos += new Vector2((short)cUNIT.X, (short)cUNIT.Y);

                            mappos = Tools.VectorTool.Max(mappos, new Vector2(0));
                            mappos = Tools.VectorTool.Min(mappos, new Vector2(mapeditor.mapdata.WIDTH * 32, mapeditor.mapdata.HEIGHT * 32));



                            //생성모드
                            if (UnitCollsionCheck(mappos, cUNIT.unitID, IsBuilding, false))
                            {
                                CUNIT _cUNIT = new CUNIT(cUNIT);
                                _cUNIT.SetMapEditor(mapeditor);
                                _cUNIT.X = (ushort)mappos.X;
                                _cUNIT.Y = (ushort)mappos.Y;

                                cUNIT.ImageReset();
                                if (mapeditor.mapdata.UNITListAdd(_cUNIT))
                                {
                                    mapeditor.taskManager.TaskAdd(new UnitEvent(mapeditor, _cUNIT, true));
                                }
                                //mapeditor.mapdata.UNIT.Add(_cUNIT);
                            }
                        }
                    }
                }
                else
                {
                    int unitid = (ushort)mapeditor.UnitPallete.SelectIndex;
                    int playerid = (ushort)mapeditor.unit_player;
                    Vector2 mappos = mapeditor.PosScreenToMap(MousePos);
                    byte sflag = (byte)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Special Ability Flags", unitid).Data;
                    ushort bwidth = (byte)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "StarEdit Placement Box Width", unitid).Data;
                    ushort bheight = (byte)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "StarEdit Placement Box Height", unitid).Data;

                    if (mapeditor.UnitPalleteGridFix)
                    {
                        //그리드 픽스
                        if (gridsize != 0)
                        {
                            mappos.X = (float)(Math.Round(mappos.X / gridsize) * gridsize);
                            mappos.Y = (float)(Math.Round(mappos.Y / gridsize) * gridsize);
                        }
                    }

                    bool IsBuilding = false;
                    if ((sflag & 0x1) > 0)
                    {
                        //건물
                        IsBuilding = true;
                    }

                    if (mapeditor.UnitPalleteBuildingFix & IsBuilding)
                    {
                        //빌딩 그리드 픽스
                        mappos.X = (float)(Math.Round(mappos.X / 32) * 32);
                        mappos.Y = (float)(Math.Round(mappos.Y / 32) * 32);

                        mappos.X += (bwidth / 2) % 32;
                        mappos.Y += (bheight / 2) % 32;
                    }
                    else if (mapeditor.UnitPalleteGridFix)
                    {
                        //그리드 픽스
                        if (gridsize != 0)
                        {
                            mappos.X = (float)(Math.Round(mappos.X / gridsize) * gridsize);
                            mappos.Y = (float)(Math.Round(mappos.Y / gridsize) * gridsize);
                        }
                    }

                    mappos = Tools.VectorTool.Max(mappos, new Vector2(0));
                    mappos = Tools.VectorTool.Min(mappos, new Vector2(mapeditor.mapdata.WIDTH * 32, mapeditor.mapdata.HEIGHT * 32));

                    mappos.X = (float)Math.Floor(mappos.X);
                    mappos.Y = (float)Math.Floor(mappos.Y);



                    //생성모드
                    if ((LastCreatePos - mappos).Length() >= Math.Max(gridsize, 4))
                    {
                        if (UnitCollsionCheck(mappos, unitid, IsBuilding, false))
                        {
                            LastCreatePos = mappos;

                            CUNIT cUNIT = new CUNIT();
                            cUNIT.SetMapEditor(mapeditor);
                            cUNIT.unitID = (ushort)unitid;
                            cUNIT.X = (ushort)mappos.X;
                            cUNIT.Y = (ushort)mappos.Y;

                            cUNIT.player = (byte)playerid;


                            cUNIT.ImageReset();
                            if(mapeditor.mapdata.UNITListAdd(cUNIT))
                            {
                                mapeditor.taskManager.TaskAdd(new UnitEvent(mapeditor, cUNIT, true));
                            }
                            //mapeditor.mapdata.UNIT.Add(cUNIT);

                            
                        }
                    }
                }
            }
        }


        private bool UnitCollsionCheck(Vector2 pos, int unitid, bool IsBuilding, bool IsDrawRedLine)
        {
            if (mapeditor.UnitPalleteStackAllow)
            {
                return true;
            }

            byte height = (byte)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Elevation Level", unitid).Data;
            bool IsAir = false;
            if(height >= 12)
            {
                IsAir = true;
            }

            ushort uleft = (byte)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Unit Size Left", unitid).Data;
            ushort uup = (byte)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Unit Size Up", unitid).Data;
            ushort uright = (byte)(Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Unit Size Right", unitid).Data + 1);
            ushort udown = (byte)(Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Unit Size Down", unitid).Data + 1);

            Vector2 min = pos - new Vector2(uleft, uup);
            Vector2 max = pos + new Vector2(uright, udown);
            Rect rect = new Rect(new Point(min.X, min.Y), new Point(max.X, max.Y));


            //====지형====
            if (!IsAir)
            {
                if (IsBuilding)
                {
                    ushort bwidth = (byte)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "StarEdit Placement Box Width", unitid).Data;
                    ushort bheight = (byte)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "StarEdit Placement Box Height", unitid).Data;


                    min = pos - new Vector2(bwidth, bheight) / 2;
                    max = pos + new Vector2(bwidth, bheight) / 2;
                }


                Vector2 mapmin = new Vector2((float)Math.Floor(min.X / 32), (float)Math.Floor(min.Y / 32));
                Vector2 mapmax = new Vector2((float)Math.Floor((max.X - 1) / 32), (float)Math.Floor((max.Y - 1) / 32));

                Vector2 minimin = new Vector2((float)Math.Floor(min.X / 8), (float)Math.Floor(min.Y / 8));
                Vector2 minimax = new Vector2((float)Math.Floor((max.X - 1) / 8), (float)Math.Floor((max.Y - 1) / 8));

                Vector2 gab = mapmax - mapmin;

                for (int Y = 0; Y <= gab.Y; Y++)
                {
                    for (int X = 0; X <= gab.X; X++)
                    {
                        int cxti = (int)(mapmin.X + X);
                        int cyti = (int)(mapmin.Y + Y);

                        if (cxti < 0 || cyti < 0)
                        {
                            continue;
                        }
                        if (cxti >= mapeditor.mapdata.WIDTH || cyti >= mapeditor.mapdata.HEIGHT)
                        {
                            continue;
                        }
                        int tileindex = (int)((mapmin.X + X) + (mapmin.Y + Y) * mapeditor.mapdata.WIDTH);


                        ushort MTXM = mapeditor.mapdata.MTXM[tileindex];


                        ushort megaindex = tileSet.GetMegaTileIndex(mapeditor.mapdata.TILETYPE, MTXM);
                        vf4 vf4 = tileSet.GetVf4(mapeditor.mapdata.TILETYPE, megaindex);
                        cv5 cv5 = tileSet.GetCV5(mapeditor.mapdata.TILETYPE, MTXM);

                        if (mapeditor.UnitPalleteBuildingFix & IsBuilding)
                        {
                            //빌딩

                            if (((cv5.Flags & 0x0040) > 0) | ((cv5.Flags & 0x0080) > 0))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            //일반 판정
                            if (vf4.IsWall)
                            {
                                return false;
                            }

                            if(X == 0 | X == gab.X | Y == 0 | Y == gab.Y)
                            {
                                //가장자리
                                for (int my = 0; my < 4; my++)
                                {
                                    for (int mx = 0; mx < 4; mx++)
                                    {
                                        Vector2 minitilepos = new Vector2((mapmin.X + X) * 4 + mx, (mapmin.Y + Y) * 4 + my);

                                        if ((minimin.X <= minitilepos.X) & (minitilepos.X <= minimax.X) &
                                            (minimin.Y <= minitilepos.Y) & (minitilepos.Y <= minimax.Y))
                                        {
                                            int index = mx + my * 4;

                                            if ((vf4.flags[index] & 0b1) == 0)
                                            {
                                                return false;
                                            }
                                        }                                        
                                    }
                                }
                            }
                            else
                            {
                                //안쪽
                                if (!vf4.IsGround)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }


            //====유닛====
            for (int i = 0; i < mapeditor.mapdata.UNIT.Count; i++)
            {
                CUNIT cUNIT = mapeditor.mapdata.UNIT[i];

                int _unitid = cUNIT.unitID;

                byte _height = (byte)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Elevation Level", _unitid).Data;
                bool _IsAir = false;
                if (_height >= 12)
                {
                    _IsAir = true;
                }
                if(_IsAir != IsAir)
                {
                    continue;
                }

                ushort _uleft = (byte)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Unit Size Left", _unitid).Data;
                ushort _uup = (byte)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Unit Size Up", _unitid).Data;
                ushort _uright = (byte)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Unit Size Right", _unitid).Data;
                ushort _udown = (byte)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Unit Size Down", _unitid).Data;

                _uleft -= 1;
                _uup -= 1;


                Vector2 _pos = new Vector2(cUNIT.X, cUNIT.Y);
                Vector2 _min = _pos - new Vector2(_uleft, _uup);
                Vector2 _max = _pos + new Vector2(_uright, _udown);
                Rect _rect = new Rect(new Point(_min.X, _min.Y), new Point(_max.X, _max.Y));

                
                if (Rect.Intersect(rect, _rect) != Rect.Empty)
                {
                    //충돌상황
                    if (IsDrawRedLine)
                    {
                        Vector2 mousepos = mapeditor.PosMapToScreen(_pos);

                        double _l = (_uleft + 1) * mapeditor.opt_scalepercent;
                        double _u = (_uup + 1) * mapeditor.opt_scalepercent;
                        double _r = (_uright + 1) * mapeditor.opt_scalepercent;
                        double _d = (_udown + 1) * mapeditor.opt_scalepercent;
                        DrawRect(_spriteBatch, new Vector2(mousepos.X - (float)_l, mousepos.Y - (float)_u), new Vector2(mousepos.X + (float)_r, mousepos.Y + (float)_d), Color.Red, 1);
                    }
                    return false;
                }
            }


            return true;
        }


        //END
    }
}
