using Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using SharpDX.Direct2D1.Effects;
using SharpDX.Direct2D1;
using SpriteFontPlus;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Windows;
using System.Windows.Forms;
using UseMapEditor.Control;
using UseMapEditor.FileData;
using UseMapEditor.Lua.TrigEditPlus;
using UseMapEditor.Task.Events;
using WpfTest.Components;
using static Data.Map.MapData;
using static UseMapEditor.FileData.TileSet;
using Point = System.Windows.Point;

namespace UseMapEditor.MonoGameControl
{
    public partial class MapDrawer : WpfGame
    {
        private Vector2 LastCreatePos = new Vector2(-100);


        private List<CImage> SpritePalleteCursor;
        private CUNIT UnitPalleteCursor;
        private CDD2 DoodadPalleteCursor;


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
                    if (mapeditor.mapDataBinding.DOODAD_SELECTMODE)
                    {
                        if (mouse_IsDrag)
                        {
                            IsDrawSelectRect = true;
                        }
                        IsRetrun = true;
                    }
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
                                if (UnitCollsionCheck(mappos, cUNIT.unitID, IsBuilding, true))
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
                            if (UnitCollsionCheck(mappos, unitid, IsBuilding, true))
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

                case Control.MapEditor.Layer.Tile:
                    if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.ALLTILE || mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.PASTE)
                    {
                        if (mapeditor.tile_PaintType == MapEditor.TileSetPaintType.CIRCLE)
                        {
                            if (mouse_IsDrag)
                            {
                                //드래그 하면 미리보기 그려짐
                                //TODO:드래그시 미리보기 그리기


                                int width = 0;
                                int height = 0;

                                if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.ALLTILE)
                                {
                                    width = (int)(mapeditor.tile_PalleteSelectEnd.X - mapeditor.tile_PalleteSelectStart.X) + 1;
                                    height = (int)(mapeditor.tile_PalleteSelectEnd.Y - mapeditor.tile_PalleteSelectStart.Y) + 1;
                                }
                                else if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.PASTE)
                                {
                                    if (mapeditor.tile_CopyedTile.Count != 0)
                                    {
                                        width = (int)mapeditor.tile_CopyedTileSize.X;
                                        height = (int)mapeditor.tile_CopyedTileSize.Y;
                                    }
                                }

                                int sx = (int)Math.Floor(width / 2.0);
                                int sy = (int)Math.Floor(height / 2.0);

                                bool IsOneTile = false;
                                if (width == 1 && height == 1)
                                {
                                    IsOneTile = true;
                                    width = mapeditor.brush_x;
                                    height = mapeditor.brush_y;
                                }

                                if (width == 0 || height == 0)
                                {
                                    return;
                                }

                                Vector2 DragStart = mouse_DragMapStart;
                                Vector2 DragEndtemp = MouseTilePos;


                                DragStart.X = (float)Math.Floor(DragStart.X / 32);
                                DragStart.Y = (float)Math.Floor(DragStart.Y / 32);


                                if (DragStart.X > DragEndtemp.X)
                                {
                                    DragStart.X -= 1;
                                }
                                if (DragStart.Y > DragEndtemp.Y)
                                {
                                    DragStart.Y -= 1;
                                }

                                Vector2 DragEnd;
                                if (key_LeftShiftDown)
                                {
                                    Vector2 differ = DragEndtemp - DragStart;
                                    double DragLen = Math.Max(Math.Abs(differ.X), Math.Abs(differ.Y));

                                    if (differ.X >= 0)
                                    {
                                        //양수 일 경우
                                        DragEnd.X = (float)(DragStart.X + DragLen);
                                    }
                                    else
                                    {
                                        //음수 일 경우
                                        DragEnd.X = (float)(DragStart.X - DragLen);
                                    }

                                    if (differ.Y >= 0)
                                    {
                                        //양수 일 경우
                                        DragEnd.Y = (float)(DragStart.Y + DragLen);
                                    }
                                    else
                                    {
                                        //음수 일 경우
                                        DragEnd.Y = (float)(DragStart.Y - DragLen);
                                    }
                                }
                                else
                                {
                                    DragEnd = DragEndtemp;
                                }


                    


                                Vector2 Size = DragEnd - DragStart;

                                int Dragwidth = (int)Math.Abs(Size.X);
                                int Dragheight = (int)Math.Abs(Size.Y);

                                Vector2 center = new Vector2(Dragwidth, Dragheight) / 2;

                                bool[,] flag = new bool[Dragwidth + 1, Dragheight + 1];

                                double radius = Math.Max(Dragwidth / 2, Dragheight / 2);

                                double widthscale = radius / (Dragheight / 2);
                                double heightscale = radius / (Dragwidth / 2);


                                Vector2 DragMin = new Vector2(Math.Min(DragStart.X, DragEnd.X), Math.Min(DragStart.Y, DragEnd.Y));


                                _spriteBatch.Begin(blendState:BlendState.NonPremultiplied);
                                for (int y = 0; y < Dragheight + 1; y++)
                                {
                                    int minx = -1;
                                    int maxx = -1;

                                    for (int x = 0; x < Dragwidth + 1; x++)
                                    {
                                        double tx = (center.X - x) * heightscale ;
                                        double ty = (center.Y - y) * widthscale ;

                                        double d = Math.Sqrt(Math.Pow(tx, 2) + Math.Pow(ty, 2));
                                        if(Math.Max(Dragwidth, Dragheight) % 2 == 1)
                                        {
                                            d -= 1;
                                        }
                                        else
                                        {
                                            d -= 0.4;
                                        }

                                        if (maxx == -1)
                                        {
                                            minx = x;
                                        }

                                        if (radius >= d)
                                        {
                                            flag[x, y] = true;

                                            //DrawRect(_spriteBatch, mapeditor.PosMapToScreen(32 * (DragMin + new Vector2(x,y))), mapeditor.PosMapToScreen(32 * (DragMin + new Vector2(x + 1, y + 1))), new Color(138, 43, 226, 128), 3, true);

                                            maxx = x;                                        
                                        }
                                        
                                    }
                                    if((minx != -1 || maxx != -1) && maxx >= minx)
                                    {
                                        DrawRect(_spriteBatch, mapeditor.PosMapToScreen(32 * (DragMin + new Vector2(minx, y))), mapeditor.PosMapToScreen(32 * (DragMin + new Vector2(maxx + 1, y + 1))), new Color(138, 43, 226, 128), 1, true);
                                    }


                                }



                                DragEnd.X += 1;
                                DragEnd.Y += 1;

                                if (DragStart.X >= DragEnd.X)
                                {
                                    DragStart.X += 1;
                                    DragEnd.X -= 1;
                                }
                                if (DragStart.Y >= DragEnd.Y)
                                {
                                    DragStart.Y += 1;
                                    DragEnd.Y -= 1;
                                }

                                DragStart *= 32;
                                DragStart = mapeditor.PosMapToScreen(DragStart);
                                DragEnd = mapeditor.PosMapToScreen(DragEnd * 32);

                                //_spriteBatch.Begin();

                                Vector2 stringCenter = (DragStart + DragEnd) / 2;

                                stringCenter.X -= 30;
                                stringCenter.Y -= 10;


                                //DrawRect(_spriteBatch, DragStart, DragEnd, Color.BlueViolet, 4);
                                DrawRect(_spriteBatch, DragStart, DragEnd, Color.BlueViolet, 4);
                                //DrawRect(_spriteBatch, DragStart, DragEnd, new Color(138, 43, 226, 64), 3, true);

                                _spriteBatch.DrawString(_font, (Dragwidth + 1) + " X " + (Dragheight + 1), stringCenter, Color.White, 0, new Vector2(), 1.5f, SpriteEffects.None, 0);

                                _spriteBatch.End();
                            }
                        }
                        else if (mapeditor.tile_PaintType == MapEditor.TileSetPaintType.RECT)
                        {
                            if (mouse_IsDrag)
                            {
                                //드래그 하면 미리보기 그려짐
                                //TODO:드래그시 미리보기 그리기


                                int width = 0;
                                int height = 0;

                                if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.ALLTILE)
                                {
                                    width = (int)(mapeditor.tile_PalleteSelectEnd.X - mapeditor.tile_PalleteSelectStart.X) + 1;
                                    height = (int)(mapeditor.tile_PalleteSelectEnd.Y - mapeditor.tile_PalleteSelectStart.Y) + 1;
                                }
                                else if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.PASTE)
                                {
                                    if(mapeditor.tile_CopyedTile.Count != 0)
                                    {
                                        width = (int)mapeditor.tile_CopyedTileSize.X;
                                        height = (int)mapeditor.tile_CopyedTileSize.Y;
                                    }
                                }

                                int sx = (int)Math.Floor(width / 2.0);
                                int sy = (int)Math.Floor(height / 2.0);

                                bool IsOneTile = false;
                                if (width == 1 && height == 1)
                                {
                                    IsOneTile = true;
                                    width = mapeditor.brush_x;
                                    height = mapeditor.brush_y;
                                }

                                if (width == 0 || height == 0)
                                {
                                    return;
                                }




                                Vector2 DragStart = mouse_DragMapStart;
                                Vector2 DragEndtemp = MouseTilePos;
                               
                                DragStart.X = (float)Math.Round(DragStart.X / 32);
                                DragStart.Y = (float)Math.Round(DragStart.Y / 32);


                                if (DragStart.X <= DragEndtemp.X)
                                {
                                    DragEndtemp.X += 1;
                                }
                                if (DragStart.Y <= DragEndtemp.Y)
                                {
                                    DragEndtemp.Y += 1;
                                }

                                Vector2 DragEnd;
                                if (key_LeftShiftDown)
                                {
                                    Vector2 differ = DragEndtemp - DragStart;
                                    double DragLen = Math.Max(Math.Abs(differ.X), Math.Abs(differ.Y));

                                    if (differ.X >= 0)
                                    {
                                        //양수 일 경우
                                        DragEnd.X = (float)(DragStart.X + DragLen);
                                    }
                                    else
                                    {
                                        //음수 일 경우
                                        DragEnd.X = (float)(DragStart.X - DragLen);
                                    }

                                    if (differ.Y >= 0)
                                    {
                                        //양수 일 경우
                                        DragEnd.Y = (float)(DragStart.Y + DragLen);
                                    }
                                    else
                                    {
                                        //음수 일 경우
                                        DragEnd.Y = (float)(DragStart.Y - DragLen);
                                    }
                                }
                                else
                                {
                                    DragEnd = DragEndtemp;
                                }



                                Vector2 DragSize = DragEnd - DragStart;

                                DragStart *= 32;

                                DragStart = mapeditor.PosMapToScreen(DragStart);
                                DragEnd = mapeditor.PosMapToScreen(DragEnd * 32);

                                _spriteBatch.Begin(blendState: BlendState.NonPremultiplied);
                                DrawRect(_spriteBatch, DragStart, DragEnd, Color.BlueViolet, 4);
                                DrawRect(_spriteBatch, DragStart, DragEnd, new Color(138,43,226,128), 3, true);

                                Vector2 Center = (DragStart + DragEnd) / 2;
                                Center.X -= 30;
                                Center.Y -= 10;


                                _spriteBatch.DrawString(_font, Math.Abs(DragSize.X) + " X " + Math.Abs(DragSize.Y), Center, Color.White, 0, new Vector2(), 1.5f, SpriteEffects.None, 0);

                                _spriteBatch.End();
                            }
                        }
                        else if (mapeditor.tile_PaintType == MapEditor.TileSetPaintType.PENCIL)
                        {

                            AtlasTileSet atlasTileSet = tileSet.GetAtlasTileSetTexture(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE);
                            Vector2 mappos = MouseTilePos;

                            //mappos.X = (float)(mappos.X * 32);
                            //mappos.Y = (float)(mappos.Y * 32);

                            int width = 0;
                            int height = 0;

                            if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.ALLTILE)
                            {
                                width = (int)(mapeditor.tile_PalleteSelectEnd.X - mapeditor.tile_PalleteSelectStart.X) + 1;
                                height = (int)(mapeditor.tile_PalleteSelectEnd.Y - mapeditor.tile_PalleteSelectStart.Y) + 1;
                            }
                            else if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.PASTE)
                            {
                                if (mapeditor.tile_CopyedTile.Count != 0)
                                {
                                    width = (int)mapeditor.tile_CopyedTileSize.X;
                                    height = (int)mapeditor.tile_CopyedTileSize.Y;
                                }
                            }

                            int sx = (int)Math.Floor(width / 2.0);
                            int sy = (int)Math.Floor(height / 2.0);

                            bool IsOneTile = false;
                            if (width == 1 && height == 1)
                            {
                                IsOneTile = true;
                                width = mapeditor.brush_x;
                                height = mapeditor.brush_y;
                            }
                            else
                            {
                                mappos -= new Vector2(width / 2, height / 2);
                            }




                            mappos = mapeditor.PosMapToScreen(mappos * 32, 2);
                            tileSet.tileAtlasPainter.Draw(mappos + new Vector2(-2, -2), mapeditor, mapeditor.editorTextureData.tilePaletteMap, true);
                            tileSet.tileAtlasPainter.Draw(mappos + new Vector2(2, 2), mapeditor, mapeditor.editorTextureData.tilePaletteMap, true);
                            tileSet.tileAtlasPainter.Draw(mappos + new Vector2(2, -2), mapeditor, mapeditor.editorTextureData.tilePaletteMap, true);
                            tileSet.tileAtlasPainter.Draw(mappos + new Vector2(-2, 2), mapeditor, mapeditor.editorTextureData.tilePaletteMap, true);
                            tileSet.tileAtlasPainter.Draw(mappos, mapeditor, mapeditor.editorTextureData.tilePaletteMap, false, 0.8f);

                        }


                        if (mapeditor.mapDataBinding.TILE_PAINTTYPE == Control.MapEditor.TileSetPaintType.SELECTION)
                        {
                            if (mouse_IsDrag)
                            {

                                float minx = Math.Min(mouse_DragMapStart.X, MouseMapPos.X);
                                float miny = Math.Min(mouse_DragMapStart.Y, MouseMapPos.Y);
                                float maxx = Math.Max(mouse_DragMapStart.X, MouseMapPos.X);
                                float maxy = Math.Max(mouse_DragMapStart.Y, MouseMapPos.Y);

                                minx = (float)(Math.Floor(minx / 32) * 32);
                                miny = (float)(Math.Floor(miny / 32) * 32);
                                maxx = (float)(Math.Ceiling(maxx / 32) * 32);
                                maxy = (float)(Math.Ceiling(maxy / 32) * 32);

                                //선택모드
                                Vector2 min = new Vector2(minx, miny);
                                Vector2 max = new Vector2(maxx, maxy);



                                _spriteBatch.Begin();
                                DrawRect(_spriteBatch, mapeditor.PosMapToScreen(min), mapeditor.PosMapToScreen(max), Color.Lime, 3);

                                _spriteBatch.End();

                                //_spriteBatch.Draw(gridtexture, new Rectangle((int)MapMin.X, (int)MapMin.Y, (int)MapSize.X, (int)MapSize.Y), null, mapeditor.TileBack, 0, new Vector2(), SpriteEffects.None, 0);
                            }

                            #region 타일 선택시 그리기
                            //_spriteBatch.Begin();
                            //AtlasTileSet atlasTileSet = tileSet.GetAtlasTileSetTexture(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE);
                            //foreach (var item in mapeditor.tile_SelectedTile)
                            //{
                            //    int mapx = (int)(item.Key.X);
                            //    int mapy = (int)(item.Key.Y);
                            //    Vector2 screen = mapeditor.PosMapToScreen(new Vector2(mapx * 32, mapy * 32));
                            //    Vector2 screenm = mapeditor.PosMapToScreen(new Vector2((mapx + 1) * 32, (mapy + 1) * 32));


                            //    int tileindex = (int)(mapx + mapy * mapeditor.mapdata.WIDTH);

                            //    if (!mapeditor.mapdata.CheckTILERange(mapx, mapy)) continue;
                            //    int megaindex = tileSet.GetMegaTileIndex(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, mapeditor.mapdata.TILE[tileindex]);

                            //    if (mapeditor.TilePalleteTransparentBlack && megaindex == 0) continue;

                            //    DrawRect(_spriteBatch, screen, screenm, Color.Lime, 3);
                            //}
                            //_spriteBatch.End();


                            _spriteBatch.Begin(blendState: BlendState.NonPremultiplied, samplerState: SamplerState.PointClamp);

                            foreach (var item in mapeditor.tile_SelectedTile)
                            {
                                int mapx = (int)(item.Key.X);
                                int mapy = (int)(item.Key.Y);


                                int tileindex = (int)(mapx + mapy * mapeditor.mapdata.WIDTH);

                                if (!mapeditor.mapdata.CheckTILERange(mapx, mapy)) continue;


                                int megaindex = tileSet.GetMegaTileIndex(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, mapeditor.mapdata.TILE[tileindex]);

                                if (mapeditor.TilePalleteTransparentBlack && megaindex == 0) continue;

                                Vector2 screen = mapeditor.PosMapToScreen(new Vector2(mapx * 32, mapy * 32));
                                if (0 <= mapx && mapx < mapeditor.mapdata.WIDTH &&
                                0 <= mapy && mapy < mapeditor.mapdata.HEIGHT)
                                {

                                    _spriteBatch.Draw(gridtexture, screen, null, new Color(0.4f, 0.4f, 7f, 0.35f), 0, new Vector2(), (float)(32 * mapeditor.opt_scalepercent), SpriteEffects.None, 0);

                                  

                                    //DrawTilePreview(atlasTileSet, mapx * 32, mapy * 32, megaindex);

                                    //DrawRect(_spriteBatch, new Vector2(mapx * 32, mapy * 32), new Vector2(32, 32), Color.Red, 3);
                                }

                            }
                            _spriteBatch.End();
                            #endregion



                        }

                    }
                    else if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.ISOM)
                    {
                        //TODO:ISOM
                    }
                    //else if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.PASTE)
                    //{
                    //    if (mapeditor.tile_PaintType == MapEditor.TileSetPaintType.RECT)
                    //    {

                    //    }
                    //    else if (mapeditor.tile_PaintType == MapEditor.TileSetPaintType.PENCIL)
                    //    {
                    //        _spriteBatch.Begin(blendState: BlendState.NonPremultiplied, samplerState: SamplerState.PointClamp);
                    //        AtlasTileSet atlasTileSet = tileSet.GetAtlasTileSetTexture(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE);
                    //        Vector2 mappos = MouseTilePos;

                    //        //mappos.X = (float)(mappos.X * 32);
                    //        //mappos.Y = (float)(mappos.Y * 32);


                    //        int width = (int)mapeditor.tile_CopyedTileSize.X;
                    //        int height = (int)mapeditor.tile_CopyedTileSize.Y;

                    //        int sx = (int)Math.Floor(width / 2.0);
                    //        int sy = (int)Math.Floor(height / 2.0);

                    //        bool IsOneTile = false;
                    //        if (width == 1 && height == 1)
                    //        {
                    //            IsOneTile = true;
                    //            width = mapeditor.brush_x;
                    //            height = mapeditor.brush_y;
                    //        }

                    //        for (int y = 0; y < height; y++)
                    //        {
                    //            for (int x = 0; x < width; x++)
                    //            {
                    //                int megaindex;

                    //                if (IsOneTile)
                    //                {
                    //                    megaindex = tileSet.GetMegaTileIndex(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, mapeditor.Tile_GetCopyedTile(0, 0));
                    //                }
                    //                else
                    //                {
                    //                    megaindex = tileSet.GetMegaTileIndex(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, mapeditor.Tile_GetCopyedTile(x, y));
                    //                }


                    //                if (mapeditor.TilePalleteTransparentBlack && megaindex == 0) continue;

                    //                int mapx = (int)(mappos.X + (x - sx));
                    //                int mapy = (int)(mappos.Y + (y - sy));


                    //                if (0 <= mapx && mapx < mapeditor.mapdata.WIDTH &&
                    //                    0 <= mapy && mapy < mapeditor.mapdata.HEIGHT)
                    //                {

                    //                    DrawTilePreview(atlasTileSet, mapx * 32, mapy * 32, megaindex);
                    //                }

                    //            }
                    //        }


                    //        _spriteBatch.End();
                    //    }
                        
                    //}
                    break;
                case Control.MapEditor.Layer.Doodad:
                    {
                        if (mapeditor.doodad_PasteMode)
                        {

                            for (int i = 0; i < mapeditor.CopyedDoodad.Count; i++)
                            {
                                Vector2 mappos = MouseMapPos;


                                mappos.X = (float)(Math.Round(mappos.X / 32) * 32);
                                mappos.Y = (float)(Math.Round(mappos.Y / 32) * 32);


                                int doodadid = mapeditor.CopyedDoodad[i].ID;
                                var t = tileSet.DoodadPallets[mapeditor.mapdata.TILETYPE];


                                CDD2 cDD2 = mapeditor.CopyedDoodad[i];
                                if (cDD2.Images.Count == 0)
                                {
                                    cDD2.ImageReset();
                                }


                                Vector2 DoodadPos = mappos + new Vector2(cDD2.X, cDD2.Y);


                                //if (pallete.dddHeight % 2 == 1)
                                //{
                                //    mappos.Y -= 16;
                                //}

                                mappos = Tools.VectorTool.Max(mappos, new Vector2(0));
                                mappos = Tools.VectorTool.Min(mappos, new Vector2(mapeditor.mapdata.WIDTH * 32, mapeditor.mapdata.HEIGHT * 32));

                                mappos.X = (float)Math.Floor(mappos.X);
                                mappos.Y = (float)Math.Floor(mappos.Y);


                                mappos.X += (float)cDD2.X;
                                mappos.Y += (float)cDD2.Y;


                                cDD2.PalleteX = (ushort)mappos.X;
                                cDD2.PalleteY = (ushort)mappos.Y;

                                

                                //Vector2 mousepos = mapeditor.PosMapToScreen(mappos);

                                _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                                DrawDooDad(cDD2, templist, IsPallete: true);
                                //_spriteBatch.DrawString(_font, doodadid.ToString(), mousepos, Color.Red);
                                _spriteBatch.End();
                                DrawImageList(templist);
                            }
                        }
                        else
                        {
                            int doodadid = mapeditor.doodad_index;
                            var t = tileSet.DoodadPallets[mapeditor.mapdata.TILETYPE];

                            if (!t.ContainsKey((ushort)doodadid))
                            {
                                return;
                            }

                            DoodadPallet pallete = t[(ushort)doodadid];


                            int playerid = 12;
                            if (doodadid == -1)
                            {
                                return;
                            }
                            if (DoodadPalleteCursor == null)
                            {
                                DoodadPalleteCursor = new CDD2(mapeditor.mapdata);
                                DoodadPalleteCursor.ID = (ushort)doodadid;
                                DoodadPalleteCursor.PLAYER = (byte)playerid;
                            }

                            if (DoodadPalleteCursor.ID != doodadid)
                            {
                                DoodadPalleteCursor.ID = (ushort)doodadid;
                                DoodadPalleteCursor.ImageReset();
                            }




                            if (DoodadPalleteCursor.Images.Count == 0)
                            {
                                DoodadPalleteCursor.ImageReset();
                            }

                            Vector2 mappos = MouseMapPos;


                            mappos.X = (float)(Math.Round(mappos.X / 32) * 32);
                            mappos.Y = (float)(Math.Round(mappos.Y / 32) * 32);
                            if(pallete.dddHeight % 2 == 1)
                            {
                                mappos.Y -= 16;
                            }



                            mappos = Tools.VectorTool.Max(mappos, new Vector2(0));
                            mappos = Tools.VectorTool.Min(mappos, new Vector2(mapeditor.mapdata.WIDTH * 32, mapeditor.mapdata.HEIGHT * 32));

                            mappos.X = (float)Math.Floor(mappos.X);
                            mappos.Y = (float)Math.Floor(mappos.Y);

                            DoodadPalleteCursor.PalleteX = (ushort)mappos.X;
                            DoodadPalleteCursor.PalleteY = (ushort)mappos.Y;

                            //Vector2 mousepos = mapeditor.PosMapToScreen(mappos);

                            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                            DrawDooDad(DoodadPalleteCursor, templist, IsPallete: true);
                            //_spriteBatch.DrawString(_font, doodadid.ToString(), mousepos, Color.Red);
                            _spriteBatch.End();
                            DrawImageList(templist);
                        }
                    }
                    break;
            }

        }


    }
}
