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
using UseMapEditor.Task.Events;
using WpfTest.Components;
using static Data.Map.MapData;
using Point = System.Windows.Point;

namespace UseMapEditor.MonoGameControl
{
    public partial class MapDrawer : WpfGame
    {
        private Vector2 LastCreatePos = new Vector2(-100);


        private List<CImage> SpritePalleteCursor;
        private CUNIT UnitPalleteCursor;


        private void DrawPalleteCursor()
        {
            //우클릭하면 선택모드로 변경됨

            bool IsDrawSelectRect = false;
            bool IsRetrun = false;
            switch (mapeditor.PalleteLayer)
            {
                case Control.MapEditor.Layer.Sprite:
                    if (mapeditor.mapDataBinding.SPRITE_SELECTMODE)
                    {
                        if (mouse_IsDrag)
                        {
                            IsDrawSelectRect = true;
                        }
                        IsRetrun = true;
                    }
                    break;
                case Control.MapEditor.Layer.Unit:
                    if (mapeditor.mapDataBinding.UNIT_SELECTMODE)
                    {
                        if (mouse_IsDrag)
                        {
                            IsDrawSelectRect = true;
                        }
                        IsRetrun = true;
                    }
                    break;
                case Control.MapEditor.Layer.Doodad:

                    break;
            }
            if (IsDrawSelectRect)
            {
                _spriteBatch.Begin(blendState: BlendState.NonPremultiplied);
                //Q하면 유닛 선택

                Vector2 dragpos = mapeditor.PosMapToScreen(mouse_DragMapStart);

                Vector2 min = new Vector2(Math.Min(dragpos.X, MousePos.X), Math.Min(dragpos.Y, MousePos.Y));
                Vector2 max = new Vector2(Math.Max(dragpos.X, MousePos.X), Math.Max(dragpos.Y, MousePos.Y));
                Vector2 size = max - min;

                DrawRect(_spriteBatch, dragpos, MousePos, Color.LimeGreen, 2);
                _spriteBatch.Draw(gridtexture, new Rectangle((int)min.X, (int)min.Y, (int)size.X, (int)size.Y), null, new Color(128, 255, 128, 64), 0, new Vector2(), SpriteEffects.None, 0);
                _spriteBatch.End();
            }
            if (IsRetrun)
            {
                return;
            }



            List<CImage> templist = new List<CImage>();
            int gridsize = mapeditor.opt_grid;

            switch (mapeditor.PalleteLayer)
            {
                case Control.MapEditor.Layer.Unit:
                    {
                        if (mapeditor.unit_PasteMode)
                        {
                            for (int i = 0; i < mapeditor.CopyedUnit.Count; i++)
                            {
                                CUNIT cUNIT = mapeditor.CopyedUnit[i];
                                if (cUNIT.Images.Count == 0)
                                {
                                    cUNIT.ImageReset();
                                }
                                Vector2 mappos = MouseMapPos;
                                byte sflag = (byte)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Special Ability Flags", cUNIT.unitID).Data;
                                ushort bwidth = (byte)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "StarEdit Placement Box Width", cUNIT.unitID).Data;
                                ushort bheight = (byte)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "StarEdit Placement Box Height", cUNIT.unitID).Data;

                                ushort uleft = (byte)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Unit Size Left", cUNIT.unitID).Data;
                                ushort uup = (byte)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Unit Size Up", cUNIT.unitID).Data;
                                ushort uright = (byte)(Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Unit Size Right", cUNIT.unitID).Data + 1);
                                ushort udown = (byte)(Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Unit Size Down", cUNIT.unitID).Data + 1);

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

                                mappos.X = (float)Math.Floor(mappos.X);
                                mappos.Y = (float)Math.Floor(mappos.Y);

                                Vector2 mousepos = mapeditor.PosMapToScreen(mappos);

                                DrawUnit(cUNIT, templist, (int)mappos.X, (int)mappos.Y);
                                DrawImageList(templist);
                                if (IsBuilding)
                                {
                                    double _w = bwidth * mapeditor.opt_scalepercent;
                                    double _h = bheight * mapeditor.opt_scalepercent;
                                    _spriteBatch.Begin(blendState: BlendState.NonPremultiplied);
                                    _spriteBatch.Draw(gridtexture, new Rectangle((int)(mousepos.X - _w / 2), (int)(mousepos.Y - _h / 2), (int)_w, (int)_h), null, new Color(128, 255, 128, 64), 0, new Vector2(), SpriteEffects.None, 0);
                                    _spriteBatch.End();
                                }




                                double _l = uleft * mapeditor.opt_scalepercent;
                                double _u = uup * mapeditor.opt_scalepercent;
                                double _r = uright * mapeditor.opt_scalepercent;
                                double _d = udown * mapeditor.opt_scalepercent;

                                _spriteBatch.Begin();
                                if (CollsionCheck(mappos, cUNIT.unitID, IsBuilding, true))
                                {
                                    //유닛 배치 가능
                                    DrawRect(_spriteBatch, new Vector2(mousepos.X - (float)_l, mousepos.Y - (float)_u), new Vector2(mousepos.X + (float)_r, mousepos.Y + (float)_d), Color.Lime, 1);
                                }
                                else
                                {
                                    DrawRect(_spriteBatch, new Vector2(mousepos.X - (float)_l, mousepos.Y - (float)_u), new Vector2(mousepos.X + (float)_r, mousepos.Y + (float)_d), Color.Red, 1);
                                }
                                _spriteBatch.End();
                                templist.Clear();
                            }

                        }
                        else
                        {
                            int unitid = (ushort)mapeditor.UnitPallete.SelectIndex;
                            int playerid = (ushort)mapeditor.unit_player;
                            if (unitid == -1)
                            {
                                return;
                            }
                            if (UnitPalleteCursor == null)
                            {
                                UnitPalleteCursor = new CUNIT();
                                //UnitPalleteCursor.stateFlag = 0b1;
                                UnitPalleteCursor.unitID = (ushort)unitid;
                            }

                            if (UnitPalleteCursor.unitID != unitid)
                            {
                                UnitPalleteCursor.unitID = (ushort)unitid;
                                UnitPalleteCursor.ImageReset();
                            }

                            if (UnitPalleteCursor.player != playerid)
                            {
                                UnitPalleteCursor.player = (byte)playerid;
                                UnitPalleteCursor.ImageReset();
                            }

                            if (UnitPalleteCursor.Images.Count == 0)
                            {
                                UnitPalleteCursor.ImageReset();
                            }

                            Vector2 mappos = MouseMapPos;
                            byte sflag = (byte)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Special Ability Flags", unitid).Data;
                            ushort bwidth = (byte)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "StarEdit Placement Box Width", unitid).Data;
                            ushort bheight = (byte)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "StarEdit Placement Box Height", unitid).Data;

                            ushort uleft = (byte)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Unit Size Left", unitid).Data;
                            ushort uup = (byte)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Unit Size Up", unitid).Data;
                            ushort uright = (byte)(Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Unit Size Right", unitid).Data + 1);
                            ushort udown = (byte)(Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Unit Size Down", unitid).Data + 1);

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

                            UnitPalleteCursor.X = (ushort)mappos.X;
                            UnitPalleteCursor.Y = (ushort)mappos.Y;

                            Vector2 mousepos = mapeditor.PosMapToScreen(mappos);

                            DrawUnit(UnitPalleteCursor, templist, AlwaysDraw: true);
                            DrawImageList(templist);
                            if(MousePos.X > screenwidth)
                            {
                                //밖으로 나갔을 경우 미리보기 그리기
                                DrawImageListPreview(templist, new Vector2(screenwidth - 128, 256));
                            }
                            if (IsBuilding)
                            {
                                double _w = bwidth * mapeditor.opt_scalepercent;
                                double _h = bheight * mapeditor.opt_scalepercent;
                                _spriteBatch.Begin(blendState: BlendState.NonPremultiplied);
                                _spriteBatch.Draw(gridtexture, new Rectangle((int)(mousepos.X - _w/2), (int)(mousepos.Y - _h / 2), (int)_w, (int)_h), null, new Color(128, 255, 128, 64), 0, new Vector2(), SpriteEffects.None, 0);
                                _spriteBatch.End();
                            }




                            double _l = uleft * mapeditor.opt_scalepercent;
                            double _u = uup * mapeditor.opt_scalepercent;
                            double _r = uright * mapeditor.opt_scalepercent;
                            double _d = udown * mapeditor.opt_scalepercent;

                            _spriteBatch.Begin();
                            if (CollsionCheck(mappos, unitid, IsBuilding, true))
                            {
                                //유닛 배치 가능
                                DrawRect(_spriteBatch, new Vector2(mousepos.X - (float)_l, mousepos.Y - (float)_u), new Vector2(mousepos.X + (float)_r, mousepos.Y + (float)_d), Color.Lime, 1);
                            }
                            else
                            {
                                DrawRect(_spriteBatch, new Vector2(mousepos.X - (float)_l, mousepos.Y - (float)_u), new Vector2(mousepos.X + (float)_r, mousepos.Y + (float)_d), Color.Red, 1);
                            }
                            _spriteBatch.End();
                        }
                    }
                    
                    break;

                case Control.MapEditor.Layer.Sprite:
                    {
                        if (mapeditor.sprite_PasteMode)
                        {
                            //복사모드
                            for (int i = 0; i < mapeditor.CopyedSprite.Count; i++)
                            {
                                CTHG2 cTHG2 = mapeditor.CopyedSprite[i];
                                Vector2 mappos = MouseMapPos;
                                if (mapeditor.SpritePalleteGridFix)
                                {
                                    //그리드 픽스
                                    if (gridsize != 0)
                                    {
                                        mappos.X = (float)(Math.Round(mappos.X / gridsize) * gridsize);
                                        mappos.Y = (float)(Math.Round(mappos.Y / gridsize) * gridsize);
                                    }
                                }


                                mappos += new Vector2((short)cTHG2.X, (short)cTHG2.Y);

                                mappos = Tools.VectorTool.Max(mappos, new Vector2(0));
                                mappos = Tools.VectorTool.Min(mappos, new Vector2(mapeditor.mapdata.WIDTH * 32, mapeditor.mapdata.HEIGHT * 32));




                                if (cTHG2.Images.Count == 0)
                                {
                                    cTHG2.ImageReset();
                                }

                                for (int c = 0; c < cTHG2.Images.Count; c++)
                                {
                                    Vector2 mp = mapeditor.PosMapToScreen(mappos);


                                    cTHG2.Images[c].IsHover = true;
                                    cTHG2.Images[c].screen = mp;
                                    templist.Add(cTHG2.Images[c]);
                                    cTHG2.Images[c].PlayScript();
                                }
                            }

                            DrawImageList(templist);
                        }
                        else
                        {
                            int spriteid;

                            if (mapeditor.sprite_SpritBrush)
                            {
                                spriteid = mapeditor.SpritePallete.SelectIndex;
                            }
                            else
                            {
                                int unitID = mapeditor.SpritePallete_Unit.SelectIndex;
                                int fligyID = (int)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Graphics", unitID).Data;
                                spriteid = (int)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.flingy, "Sprite", fligyID).Data;
                            }


                            Vector2 mappos = MouseMapPos;
                            mappos = Tools.VectorTool.Max(mappos, new Vector2(0));
                            mappos = Tools.VectorTool.Min(mappos, new Vector2(mapeditor.mapdata.WIDTH * 32, mapeditor.mapdata.HEIGHT * 32));

                            if (mapeditor.SpritePalleteGridFix)
                            {
                                //그리드 픽스
                                if (gridsize != 0)
                                {
                                    mappos.X = (float)(Math.Round(mappos.X / gridsize) * gridsize);
                                    mappos.Y = (float)(Math.Round(mappos.Y / gridsize) * gridsize);
                                }
                            }


                            if (SpritePalleteCursor == null)
                            {
                                SpritePalleteCursor = new List<CImage>();
                                SpritePalleteCursor.Add(new CImage(int.MaxValue, SpritePalleteCursor, 0, 0, 0));
                            }

                            if (SpritePalleteCursor.Count != 0)
                            {
                                if (spriteid != -1)
                                {
                                    int imageid = (int)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.sprites, "Image File", spriteid).Data;
                                    if (SpritePalleteCursor[0].imageID != imageid)
                                    {
                                        SpritePalleteCursor.Clear();
                                        SpritePalleteCursor.Add(new CImage(0, SpritePalleteCursor, imageid, 0, 0, level: 30));
                                    }
                                }
                                else
                                {
                                    SpritePalleteCursor.Clear();
                                }
                            }
                            else
                            {
                                if (spriteid != -1)
                                {
                                    int imageid = (int)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.sprites, "Image File", spriteid).Data;
                                    SpritePalleteCursor.Add(new CImage(0, SpritePalleteCursor, imageid, 0, 0, level: 30));
                                }
                                else
                                {
                                    SpritePalleteCursor.Clear();
                                }

                            }

                            if (spriteid == -1)
                            {
                                return;
                            }



                            for (int i = 0; i < SpritePalleteCursor.Count; i++)
                            {
                                SpritePalleteCursor[i].color = mapeditor.sprite_player;


                                Vector2 mp = mapeditor.PosMapToScreen(mappos);

                                SpritePalleteCursor[i].screen = mp;
                                templist.Add(SpritePalleteCursor[i]);
                                SpritePalleteCursor[i].PlayScript();
                            }
                            DrawImageList(templist);
                            if (MousePos.X > screenwidth)
                            {
                                //밖으로 나갔을 경우 미리보기 그리기
                                DrawImageListPreview(templist, new Vector2(screenwidth - 128, 256));
                            }
                        }
                    }
                    break;
            }

        }



    }
}
