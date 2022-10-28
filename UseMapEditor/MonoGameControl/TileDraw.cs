using Data;
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
using Point = System.Windows.Point;

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




            _spriteBatch.Begin();


            if (mapeditor.PalleteLayer == Control.MapEditor.Layer.Tile && ((mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.ISOM && !mapeditor.brush_useRect)))
            {

                // 128, 64

                float mag = (float)(32 * mapeditor.opt_scalepercent);

                float startx = (float)-((mapeditor.opt_xpos % 128)  * mapeditor.opt_scalepercent);
                float starty = (float)-((mapeditor.opt_ypos % 64) * mapeditor.opt_scalepercent);


                Vector2 StartPoint = new Vector2(Math.Max(startx, MapMin.X), Math.Max(starty, MapMin.Y));
                Vector2 EndPoint = new Vector2(Math.Min(screenwidth, MapMax.X), Math.Min(screenheight, MapMax.Y));

                Vector2 Gab = EndPoint - StartPoint;


                int width = (int)(Gab.X / (mag * 4));
                int height = (int)(Gab.Y / (mag * 2));

                float grdcount = width + height;

                for (float i = 0; i < grdcount + 2; i++)
                {
                    float x1 = StartPoint.X;
                    float y1 = StartPoint.Y + mag * 2 * i + mag;
                    float x2 = StartPoint.X + mag * 4 * i + 2 * mag;
                    float y2 = StartPoint.Y;

                    if(y1 > EndPoint.Y)
                    {
                        x1 += (y1 - EndPoint.Y) * 2;
                        y1 = EndPoint.Y;
                    }


                    if (x2 > EndPoint.X)
                    {
                        y2 += (x2 - EndPoint.X) / 2;
                        x2 = EndPoint.X;
                    }


                    if (y1 < y2)
                    {
                        continue;
                    }


                    DrawLine(_spriteBatch, new Vector2(x1, y1), new Vector2(x2, y2), GridColor, 1);
                }

                for (float i = 0; i < grdcount + 2; i++)
                {
                    float x1 = StartPoint.X;
                    float y1 = StartPoint.Y + ( height + 1 ) * (mag * 2) - (mag * 2 * i) - mag;
                    float y2 = EndPoint.Y;

                    float x2 = StartPoint.X + (y2 - y1) * 2;


                    if (y1 < StartPoint.Y)
                    {
                        x1 += (StartPoint.Y - y1) * 2;
                        y1 = StartPoint.Y;
                    }


                    if (x2 > EndPoint.X)
                    {
                        y2 -= (x2 - EndPoint.X) / 2;
                        x2 = EndPoint.X;
                    }


                    if (y1 > y2)
                    {
                        continue;
                    }
                    if (x1 > x2)
                    {
                        continue;
                    }


                    DrawLine(_spriteBatch, new Vector2(x1, y1), new Vector2(x2, y2), GridColor, 1);
                }


            }
            else
            {

                float startx = (float)-((mapeditor.opt_xpos % mapeditor.opt_grid) * mapeditor.opt_scalepercent);
                float starty = (float)-((mapeditor.opt_ypos % mapeditor.opt_grid) * mapeditor.opt_scalepercent);

                float mag = (float)(mapeditor.opt_grid * mapeditor.opt_scalepercent);

                Vector2 StartPoint = new Vector2(Math.Max(startx, MapMin.X), Math.Max(starty, MapMin.Y)); ;
                Vector2 EndPoint = new Vector2(Math.Min(screenwidth, MapMax.X), Math.Min(screenheight, MapMax.Y));
                for (float xi = StartPoint.X; xi < EndPoint.X; xi += mag)
                {
                    DrawLine(_spriteBatch, new Vector2(xi, StartPoint.Y), new Vector2(xi, EndPoint.Y), GridColor, 1);
                }
                for (float yi = StartPoint.Y; yi < EndPoint.Y; yi += mag)
                {
                    DrawLine(_spriteBatch, new Vector2(StartPoint.X, yi), new Vector2(EndPoint.X, yi), GridColor, 1);
                }
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

                        AtlasTileSet atlasTileSet = tileSet.GetAtlasTileSetTexture(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE);

                        //Texture2D texture2D = tileSet.GetMegaTileGrp(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, megaindex);
                        


                        switch (mapeditor.opt_drawType)
                        {
                            case Control.MapEditor.DrawType.SD:
                                {
                                    //_spriteBatch.Draw(texture2D, new Vector2(xi, yi), null, Color.White, 0, Vector2.Zero, (float)mapeditor.opt_scalepercent, SpriteEffects.None, 0);
                                    _spriteBatch.Draw(atlasTileSet.GetTexture(mapeditor.opt_scalepercent), new Vector2(xi, yi), atlasTileSet.GetRect(megaindex, mapeditor.opt_scalepercent), Color.White, 0, Vector2.Zero, (float)(mapeditor.opt_scalepercent * atlasTileSet.GetCompScale(mapeditor.opt_scalepercent)), SpriteEffects.None, 0);
                                }
                                break;
                            case Control.MapEditor.DrawType.HD: case Control.MapEditor.DrawType.CB:
                                {
                                    //_spriteBatch.Draw(texture2D, new Vector2(xi, yi), null, Color.White, 0, Vector2.Zero, (float)mapeditor.opt_scalepercent / 2, SpriteEffects.None, 0);
                                    _spriteBatch.Draw(atlasTileSet.GetTexture(mapeditor.opt_scalepercent), new Vector2(xi, yi), atlasTileSet.GetRect(megaindex, mapeditor.opt_scalepercent), Color.White, 0, Vector2.Zero, (float)(mapeditor.opt_scalepercent / 2 * atlasTileSet.GetCompScale(mapeditor.opt_scalepercent)), SpriteEffects.None, 0);
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


        private void DrawTilePreview(AtlasTileSet atlasTileSet, float x, float y, int megaindex)
        {
            Vector2 screen = mapeditor.PosMapToScreen(new Vector2(x, y));


            switch (mapeditor.opt_drawType)
            {
                case Control.MapEditor.DrawType.SD:
                    {
                        //_spriteBatch.Draw(texture2D, new Vector2(xi, yi), null, Color.White, 0, Vector2.Zero, (float)mapeditor.opt_scalepercent, SpriteEffects.None, 0);
                        _spriteBatch.Draw(atlasTileSet.GetTexture(mapeditor.opt_scalepercent), screen, atlasTileSet.GetRect(megaindex, mapeditor.opt_scalepercent), new Color(Color.Wheat, 0.7f), 0, Vector2.Zero, (float)(mapeditor.opt_scalepercent * atlasTileSet.GetCompScale(mapeditor.opt_scalepercent)), SpriteEffects.None, 0);
                    }
                    break;
                case Control.MapEditor.DrawType.HD:
                case Control.MapEditor.DrawType.CB:
                    {
                        //_spriteBatch.Draw(texture2D, new Vector2(xi, yi), null, Color.White, 0, Vector2.Zero, (float)mapeditor.opt_scalepercent / 2, SpriteEffects.None, 0);
                        _spriteBatch.Draw(atlasTileSet.GetTexture(mapeditor.opt_scalepercent), screen, atlasTileSet.GetRect(megaindex, mapeditor.opt_scalepercent), new Color(Color.Wheat, 0.7f), 0, Vector2.Zero, (float)(mapeditor.opt_scalepercent / 2 * atlasTileSet.GetCompScale(mapeditor.opt_scalepercent)), SpriteEffects.None, 0);
                    }
                    break;
            }
        }


        private List<CDD2> hoverDoodad = new List<CDD2>();
        private void RenderDoodad(bool IsDrawGrp)
        {
            if (!IsDrawGrp)
            {
                return;
            }



            hoverDoodad.Clear();
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.NonPremultiplied);
            for (int i = 0; i < mapeditor.mapdata.DD2.Count; i++)
            {
                DrawDooDad(mapeditor.mapdata.DD2[i]);
            }
            _spriteBatch.End();
        }





        private void DrawDooDad(CDD2 cDD2, List<CImage> templist = null, bool IsPallete = false)
        {
            var t = tileSet.DoodadPallets[mapeditor.mapdata.TILETYPE];
            
            if (!t.ContainsKey(cDD2.ID))
            {
                return;
            }

            DoodadPallet pallete = t[cDD2.ID];

            ushort X;
            ushort Y;

            if (IsPallete)
            {
                X = cDD2.PalleteX;
                Y = cDD2.PalleteY;
            }
            else
            {
                X = cDD2.X;
                Y = cDD2.Y;
            }



            bool IsSelect = false;
            bool IsHover = false;
            if (mapeditor.PalleteLayer == Control.MapEditor.Layer.Doodad)
            {
                if (mapeditor.mapDataBinding.DOODAD_SELECTMODE)
                {
                    if (mouse_IsDrag)
                    {
                        //선택모드
                        Vector2 min = new Vector2(Math.Min(mouse_DragMapStart.X, MouseMapPos.X), Math.Min(mouse_DragMapStart.Y, MouseMapPos.Y));
                        Vector2 max = new Vector2(Math.Max(mouse_DragMapStart.X, MouseMapPos.X), Math.Max(mouse_DragMapStart.Y, MouseMapPos.Y));



                        if (((min.X - 8 < X & X < max.X + 8) & (min.Y - 8 < Y & Y < max.Y + 8)))
                        {
                            hoverDoodad.Add(cDD2);
                        }
                    }
                    if (mapeditor.SelectDoodad.Contains(cDD2))
                    {
                        IsSelect = true;
                    }
                    else if (hoverDoodad.Contains(cDD2))
                    {
                        IsHover = true;
                    }
                }
            }


            int _x = X / 32 * 32 - (pallete.dddWidth / 2) * 32;
            int _y = Y / 32 * 32 - (pallete.dddHeight / 2) * 32;


            Vector2 screen = mapeditor.PosMapToScreen(new Vector2(_x, _y));
            Vector2 spritescreen = mapeditor.PosMapToScreen(new Vector2(X, Y));



            int grpwidth = (int)(pallete.dddWidth * 32 * mapeditor.opt_scalepercent);
            int grpheight = (int)(pallete.dddHeight * 32 * mapeditor.opt_scalepercent);


            float mag = (float)(32 * mapeditor.opt_scalepercent);


            float minX = 0 - grpwidth;
            float minY = 0 - grpheight;
            float maxX = screenwidth + grpwidth * 2;
            float maxY = screenheight + grpheight * 2;

            AtlasTileSet atlasTileSet = tileSet.GetAtlasTileSetTexture(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE);


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

                        if (templist != null)
                        {
                            templist.Add(cDD2.Images[i]);
                        }
                        else
                        {
                            ImageList.Add(cDD2.Images[i]);
                        }
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
                            Vector2 StartPoint = screen + new Vector2(x, y) * mag;

                            switch (mapeditor.opt_drawType)
                            {
                                case Control.MapEditor.DrawType.SD:
                                    {
                                        int magaindex = tileSet.GetMegaTileIndex(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, group, index);
                                        _spriteBatch.Draw(atlasTileSet.texture2D, StartPoint, atlasTileSet.GetRect(magaindex), color, 0, Vector2.Zero, (float)mapeditor.opt_scalepercent, SpriteEffects.None, 0);
                                    }
                                    break;
                                case Control.MapEditor.DrawType.HD:
                                case Control.MapEditor.DrawType.CB:
                                    {
                                        int magaindex = tileSet.GetMegaTileIndex(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, group, index);
                                        _spriteBatch.Draw(atlasTileSet.texture2D, StartPoint, atlasTileSet.GetRect(magaindex), color, 0, Vector2.Zero, (float)mapeditor.opt_scalepercent / 2, SpriteEffects.None, 0);
                                    }
                                    break;
                            }

                            if (IsSelect)
                            {
                                _spriteBatch.Draw(gridtexture, StartPoint, null, new Color(128, 255, 128, 128), 0, Vector2.Zero, (float)mapeditor.opt_scalepercent * 32, SpriteEffects.None, 0);
                                DrawRect(_spriteBatch, StartPoint, StartPoint + new Vector2((float)(mapeditor.opt_scalepercent * 32)), Color.Yellow, 3);
                            }
                            if (IsHover)
                            {
                                _spriteBatch.Draw(gridtexture, StartPoint, null, new Color(128, 128, 255, 128), 0, Vector2.Zero, (float)mapeditor.opt_scalepercent * 32, SpriteEffects.None, 0);
                                DrawRect(_spriteBatch, StartPoint, StartPoint + new Vector2((float)(mapeditor.opt_scalepercent * 32)), Color.Yellow, 3);
                            }


                            if (IsPallete)
                            {
                                DrawRect(_spriteBatch, StartPoint, StartPoint + new Vector2((float)(mapeditor.opt_scalepercent * 32)), Color.Lime, 3);
                            }
                        }
                    }


                    if (IsPallete)
                    {
                        Vector2 pos = new Vector2(cDD2.PalleteX, cDD2.PalleteY);
                        DoodadPallet paldoodad;

                        paldoodad = tileSet.DoodadPallets[mapeditor.mapdata.TILETYPE][cDD2.ID];



                        int x = (ushort)pos.X / 32 * 32 - (paldoodad.dddWidth / 2) * 32;
                        int y = (ushort)pos.Y / 32 * 32 - (paldoodad.dddHeight / 2) * 32;

                        Rect rect = new Rect(new Point(x, y), new Point(x + paldoodad.dddWidth * 32, y + paldoodad.dddHeight * 32));

                        for (int i = 0; i < mapeditor.mapdata.DD2.Count; i++)
                        {
                            CDD2 mcDD2 = mapeditor.mapdata.DD2[i];

                            DoodadPallet mapdoodad = tileSet.DoodadPallets[mapeditor.mapdata.TILETYPE][mcDD2.ID];

                            int tx = (ushort)mcDD2.X / 32 * 32 - (mapdoodad.dddWidth / 2) * 32;
                            int ty = (ushort)mcDD2.Y / 32 * 32 - (mapdoodad.dddHeight / 2) * 32;


                            Rect _rect = new Rect(new Point(tx, ty), new Point(tx + mapdoodad.dddWidth * 32, ty + mapdoodad.dddHeight * 32));

                            Rect interRect = Rect.Intersect(rect, _rect);
                            if (interRect != Rect.Empty)
                            {
                                //if (tileSet.IsBlack(mapeditor.mapdata.TILETYPE, group, index))
                                //{
                                //    continue;
                                //}
                                //충돌상황
                                //하나하나 비교시작
                                for (int iy = 0; iy < (int)(interRect.Height / 32); iy++)
                                {
                                    for (int ix = 0; ix < (int)(interRect.Width / 32); ix++)
                                    {
                                        int mx = (int)(interRect.X - tx) / 32;
                                        int my = (int)(interRect.Y - ty) / 32;
                                        int px = (int)(interRect.X - x) / 32;
                                        int py = (int)(interRect.Y - y) / 32;


                                        ushort mg = (ushort)(mapdoodad.dddGroup + my + iy);
                                        ushort mi = (ushort)(mx + ix);
                                        ushort pg = (ushort)(paldoodad.dddGroup + py + iy);
                                        ushort pi = (ushort)(px + ix);



                                        if (!tileSet.IsBlack(mapeditor.mapdata.TILETYPE, mg, mi) & !tileSet.IsBlack(mapeditor.mapdata.TILETYPE, pg, pi))
                                        {
                                            Vector2 _spos = mapeditor.PosMapToScreen(new Vector2((float)interRect.X + ix * 32, (float)interRect.Y + iy * 32));
                                            DrawRect(_spriteBatch, _spos, _spos + new Vector2((float)(mapeditor.opt_scalepercent * 32)), Color.Red, 3);
                                        }
                                    }
                                }

                            }

                        }



                        //if (DoodadCollsionCheckTile(new Vector2(_mapx + x, _mapy + y)))
                        //{
                        //    DrawRect(_spriteBatch, StartPoint, StartPoint + new Vector2((float)(mapeditor.opt_scalepercent * 32)), Color.Lime, 3);
                        //}
                        //else
                        //{
                        //    DrawRect(_spriteBatch, StartPoint, StartPoint + new Vector2((float)(mapeditor.opt_scalepercent * 32)), Color.Red, 3);
                        //}

                    }







                }
            }
        }
    }
}
