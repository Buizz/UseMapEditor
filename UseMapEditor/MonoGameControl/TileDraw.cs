﻿using Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using SpriteFontPlus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using UseMapEditor.FileData;
using WpfTest.Components;
using static Data.Map.MapData;
using static UseMapEditor.FileData.TileSet;

namespace UseMapEditor.MonoGameControl
{
    public partial class MapDrawer : WpfGame
    {






        public void DrawRect(SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness = 1f)
        {
            DrawLine(spriteBatch, new Vector2(point1.X, point1.Y), new Vector2(point2.X, point1.Y), color, thickness);
            DrawLine(spriteBatch, new Vector2(point1.X, point1.Y), new Vector2(point1.X, point2.Y), color, thickness);
            DrawLine(spriteBatch, new Vector2(point2.X, point1.Y), new Vector2(point2.X, point2.Y), color, thickness);
            DrawLine(spriteBatch, new Vector2(point2.X, point2.Y), new Vector2(point1.X, point2.Y), color, thickness);
        }

        public void DrawLine(SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness = 1f)
        {
            var distance = Vector2.Distance(point1, point2);
            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            DrawLine(spriteBatch, point1, distance, angle, color, thickness);
        }

        public void DrawLine(SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness = 1f)
        {
            var origin = new Vector2(0f, 0.5f);
            var scale = new Vector2(length, thickness);
            spriteBatch.Draw(gridtexture, point, null, color, angle, origin, scale, SpriteEffects.None, 0);
        }


        private void GridDraw()
        {
            if (mapeditor.opt_grid == 0)
            {
                return;
            }


            float screenwidth = (float)this.ActualWidth;
            float screenheight = (float)this.ActualHeight;


            Vector2 MapMin = mapeditor.PosMapToScreen(new Vector2(0, 0));
            Vector2 MapMax = mapeditor.PosMapToScreen(new Vector2(mapeditor.mapdata.WIDTH, mapeditor.mapdata.HEIGHT) * 32);


            float startx = (float)-((mapeditor.opt_xpos % mapeditor.opt_grid) * mapeditor.opt_scalepercent);
            float starty = (float)-((mapeditor.opt_ypos % mapeditor.opt_grid) * mapeditor.opt_scalepercent);


            float mag = (float)(mapeditor.opt_grid * mapeditor.opt_scalepercent);



            Vector2 StartPoint = new Vector2(Math.Max(startx, MapMin.X), Math.Max(starty, MapMin.Y)); ;
            Vector2 EndPoint = new Vector2(Math.Min(screenwidth, MapMax.X), Math.Min(screenheight, MapMax.Y));




            _spriteBatch.Begin();
            for (float xi = StartPoint.X; xi < EndPoint.X; xi += mag)
            {
                DrawLine(_spriteBatch, new Vector2(xi, StartPoint.Y), new Vector2(xi, EndPoint.Y), GridColor, 1);
            }
            for (float yi = StartPoint.Y; yi < EndPoint.Y; yi += mag)
            {
                DrawLine(_spriteBatch, new Vector2(StartPoint.X, yi), new Vector2(EndPoint.X, yi), GridColor, 1);
            }

            _spriteBatch.End();
            return;
        }


        private void RenderTileOverlay(bool IsDrawGrp)
        {
            bool WalkableOverlay = false;
            bool BulidingOverlay = false;

            if (mapeditor.PalleteLayer == Control.MapEditor.Layer.Unit)
            {
                if (mapeditor.mapDataBinding.UNIT_BRUSHMODE)
                {
                    if (mapeditor.unit_PasteMode)
                    {

                    }
                    else
                    {
                        int unitid = (ushort)mapeditor.UnitPallete.SelectIndex;
                        byte gflag = (byte)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Staredit Group Flags", unitid).Data;

                        if ((gflag & 0x10) > 0)
                        {
                            //건물
                            BulidingOverlay = true;
                        }
                        else
                        {
                            WalkableOverlay = true;
                        }
                    }
                }
            }



            float width = (float)this.ActualWidth;
            float height = (float)this.ActualHeight;




            Vector2 MapMin = mapeditor.PosMapToScreen(new Vector2(0, 0));
            Vector2 MapMax = mapeditor.PosMapToScreen(new Vector2(mapeditor.mapdata.WIDTH, mapeditor.mapdata.HEIGHT) * 32);
            Vector2 MapSize = MapMax - MapMin;


            if (!mapeditor.view_Tile)
            {
                return;
            }


            int startxti = (int)(mapeditor.opt_xpos / 32d);
            int startyti = (int)(mapeditor.opt_ypos / 32d);

            float mag = (float)(32 * mapeditor.opt_scalepercent);




            float startx = (float)-((mapeditor.opt_xpos % 32) * mapeditor.opt_scalepercent);
            float starty = (float)-((mapeditor.opt_ypos % 32) * mapeditor.opt_scalepercent);



            int tileindex = 0;

            int cxti = startxti;
            int cyti = startyti;


            _spriteBatch.Begin(blendState: BlendState.NonPremultiplied, samplerState: SamplerState.PointClamp);
            for (float yi = starty; yi < height; yi += mag)
            {
                cxti = startxti;
                for (float xi = startx; xi < width; xi += mag)
                {
                    if (cxti < 0 || cyti < 0)
                    {
                        cxti++;
                        continue;
                    }
                    if (cxti >= mapeditor.mapdata.WIDTH || cyti >= mapeditor.mapdata.HEIGHT)
                    {
                        cxti++;
                        continue;
                    }


                    tileindex = cxti + cyti * mapeditor.mapdata.WIDTH;
                    ushort MTXM = mapeditor.mapdata.TILE[tileindex];
                    if (IsDrawGrp)
                    {
                        ushort megaindex = tileSet.GetMegaTileIndex(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, MTXM);

                        if (WalkableOverlay)
                        {
                            vf4 vf4 = tileSet.GetVf4(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, megaindex);
                            if (!vf4.IsGround)
                            {
                                if (vf4.IsWall)
                                {
                                    _spriteBatch.Draw(gridtexture, new Vector2(xi, yi), null, new Color(255, 128, 0, 64), 0, Vector2.Zero, (float)mapeditor.opt_scalepercent * 32, SpriteEffects.None, 0);
                                }
                                else
                                {
                                    for (int ym = 0; ym < 4; ym++)
                                    {
                                        for (int xm = 0; xm < 4; xm++)
                                        {
                                            int index = xm + ym * 4;

                                            if ((vf4.flags[index] & 0b1) == 0)
                                            {
                                                float xmp = (float)(xi + xm * 8 * mapeditor.opt_scalepercent);
                                                float ymp = (float)(yi + ym * 8 * mapeditor.opt_scalepercent);

                                                _spriteBatch.Draw(gridtexture, new Vector2(xmp, ymp), null, new Color(255, 128, 0, 64), 0, Vector2.Zero, (float)mapeditor.opt_scalepercent * 8, SpriteEffects.None, 0);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (BulidingOverlay)
                        {
                            cv5 cv5 = tileSet.GetCV5(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, MTXM);
                            //_spriteBatch.DrawString(_font, cv5.Flags.ToString("X"), new Vector2(xi, yi), Color.Red);
                            if (((cv5.Flags & 0x0040) > 0) | ((cv5.Flags & 0x0080) > 0))
                            {
                                //빌딩 건설불가능
                                _spriteBatch.Draw(gridtexture, new Vector2(xi, yi), null, new Color(255, 0, 0, 64), 0, Vector2.Zero, (float)mapeditor.opt_scalepercent * 32, SpriteEffects.None, 0);

                            }
                        }



                    }
                    else
                    {
                        _spriteBatch.DrawString(_font, MTXM.ToString(), new Vector2(xi, yi), Color.Red);
                    }
                    cxti++;
                }
                cyti++;
            }
            _spriteBatch.End();
        }



        private void RenderTile(bool IsDrawGrp)
        {
            float width = (float)this.ActualWidth;
            float height = (float)this.ActualHeight;




            Vector2 MapMin = mapeditor.PosMapToScreen(new Vector2(0, 0));
            Vector2 MapMax = mapeditor.PosMapToScreen(new Vector2(mapeditor.mapdata.WIDTH, mapeditor.mapdata.HEIGHT) * 32);
            Vector2 MapSize = MapMax - MapMin;


            _spriteBatch.Begin();
            _spriteBatch.Draw(gridtexture, new Rectangle((int)MapMin.X, (int)MapMin.Y, (int)MapSize.X, (int)MapSize.Y), null, mapeditor.TileBack, 0, new Vector2(), SpriteEffects.None,0);
            _spriteBatch.End();

            if (!mapeditor.view_Tile)
            {
                return;
            }


            int startxti = (int)(mapeditor.opt_xpos / 32d);
            int startyti = (int)(mapeditor.opt_ypos / 32d);

            float mag = (float)(32 * mapeditor.opt_scalepercent);


            

            float startx = (float)-((mapeditor.opt_xpos % 32) * mapeditor.opt_scalepercent);
            float starty = (float)-((mapeditor.opt_ypos % 32) * mapeditor.opt_scalepercent);



            int tileindex = 0;

            int cxti = startxti;
            int cyti = startyti;


            _spriteBatch.Begin(blendState: BlendState.NonPremultiplied, samplerState: SamplerState.PointClamp);
            for (float yi = starty; yi < height; yi += mag)
            {
                cxti = startxti;
                for (float xi = startx; xi < width; xi += mag)
                {
                    if (cxti < 0 || cyti < 0)
                    {
                        cxti++;
                        continue;
                    }
                    if (cxti >= mapeditor.mapdata.WIDTH || cyti >= mapeditor.mapdata.HEIGHT)
                    {
                        cxti++;
                        continue;
                    }


                    tileindex = cxti + cyti * mapeditor.mapdata.WIDTH;
                    ushort MTXM = mapeditor.mapdata.TILE[tileindex];
                    if (IsDrawGrp)
                    {
                        ushort megaindex = tileSet.GetMegaTileIndex(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, MTXM);
                        Texture2D texture2D = tileSet.GetMegaTileGrp(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, megaindex);
                        //Texture2D texture2D = tileSet.GetTile(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, MTXM);
                        switch (mapeditor.opt_drawType)
                        {
                            case Control.MapEditor.DrawType.SD:
                                {
                                    _spriteBatch.Draw(texture2D, new Vector2(xi, yi), null, Color.White, 0, Vector2.Zero, (float)mapeditor.opt_scalepercent, SpriteEffects.None, 0);
                                }
                                break;
                            case Control.MapEditor.DrawType.HD: case Control.MapEditor.DrawType.CB:
                                {
                                    _spriteBatch.Draw(texture2D, new Vector2(xi, yi), null, Color.White, 0, Vector2.Zero, (float)mapeditor.opt_scalepercent / 2, SpriteEffects.None, 0);
                                }
                                break;
                        }
                    }
                    else
                    {
                        _spriteBatch.DrawString(_font, MTXM.ToString(), new Vector2(xi, yi), Color.Red);
                    }
                    cxti++;
                }
                cyti++;
            }
            _spriteBatch.End();
        }
        private void RenderDoodad(bool IsDrawGrp)
        {
            if (!IsDrawGrp)
            {
                return;
            }



            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            for (int i = 0; i < mapeditor.mapdata.DD2.Count; i++)
            {
                DrawDooDad(mapeditor.mapdata.DD2[i]);
            }
            _spriteBatch.End();
        }





        private void DrawDooDad(CDD2 cDD2)
        {
            var t = tileSet.DoodadPallets[mapeditor.mapdata.TILETYPE];
            if (t.Count <= cDD2.ID)
            {
                return;
            }

            DoodadPallet pallete = t[cDD2.ID];




            int _x = cDD2.X / 32 * 32 - (pallete.dddWidth / 2) * 32;
            int _y = cDD2.Y / 32 * 32 - (pallete.dddHeight / 2) * 32;


            Vector2 screen = mapeditor.PosMapToScreen(new Vector2(_x, _y));
            Vector2 spritescreen = mapeditor.PosMapToScreen(new Vector2(cDD2.X, cDD2.Y));



            int grpwidth = (int)(pallete.dddWidth * 32 * mapeditor.opt_scalepercent);
            int grpheight = (int)(pallete.dddHeight * 32 * mapeditor.opt_scalepercent);


            float mag = (float)(32 * mapeditor.opt_scalepercent);


            float minX = 0 - grpwidth;
            float minY = 0 - grpheight;
            float maxX = screenwidth + grpwidth * 2;
            float maxY = screenheight + grpheight * 2;

            if ((minX < screen.X) & (screen.X < maxX))
            {
                if ((minY < screen.Y) & (screen.Y < maxY))
                {
                    if (cDD2.Images.Count == 0)
                    {
                        cDD2.ImageReset();
                    }
                    for (int i = 0; i < cDD2.Images.Count; i++)
                    {
                        cDD2.Images[i].screen = spritescreen;
                        ImageList.Add(cDD2.Images[i]);
                        cDD2.Images[i].PlayScript();
                    }
                    for (int y = 0; y < pallete.dddHeight; y++)
                    {
                        for (int x = 0; x < pallete.dddWidth; x++)
                        {
                            ushort group = (ushort)(pallete.dddGroup + y);
                            ushort index = (ushort)x;


                            if(tileSet.IsBlack(mapeditor.mapdata.TILETYPE, group, index))
                            {
                                continue;
                            }
                            Color color = Color.White;
                            if (mapeditor.view_DoodadColor)
                            {
                                color = mapeditor.DoodadOverlay;
                            }
                            switch (mapeditor.opt_drawType)
                            {
                                case Control.MapEditor.DrawType.SD:
                                    {
                                        Texture2D texture2D = tileSet.GetTile(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, group, index);
                                        _spriteBatch.Draw(texture2D, screen + new Vector2(x,y) * mag, null, color, 0, Vector2.Zero, (float)mapeditor.opt_scalepercent, SpriteEffects.None, 0);
                                    }
                                    break;
                                case Control.MapEditor.DrawType.HD:
                                case Control.MapEditor.DrawType.CB:
                                    {
                                        Texture2D texture2D = tileSet.GetTile(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, group, index);
                                        _spriteBatch.Draw(texture2D, screen + new Vector2(x, y) * mag, null, color, 0, Vector2.Zero, (float)mapeditor.opt_scalepercent / 2, SpriteEffects.None, 0);
                                    }
                                    break;
                            }

                        }
                    }
                }
            }
        }
    }
}
