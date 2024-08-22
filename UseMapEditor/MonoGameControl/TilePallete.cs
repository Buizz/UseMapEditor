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
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using UseMapEditor.Control;
using UseMapEditor.FileData;
using UseMapEditor.Global;
using UseMapEditor.Task.Events;
using UseMapEditor.Tools;
using WpfTest.Components;
using static Data.Map.MapData;
using static UseMapEditor.FileData.TileSet;
using Point = System.Windows.Point;

namespace UseMapEditor.MonoGameControl
{
    public partial class MapDrawer : WpfGame
    {
        private float tilesize
        {
            get
            {
                return mapeditor.TileSize;
            }
        }


        private float drawtilesize
        {
            get
            {
                switch (mapeditor.opt_drawType)
                {
                    case Control.MapEditor.DrawType.SD:
                        return tilesize / 32;
                    case Control.MapEditor.DrawType.HD:
                    case Control.MapEditor.DrawType.CB:
                        return tilesize / 64;
                }

                return 0;
            }
        }



        //왼쪽 마우스 클릭
        private void TileLeftClickStart()
        {
            if (!GlobalVariable.key_LeftShiftDown)
            {
                mapeditor.Tile_ResetSelectedTile();
            }
            mapeditor.taskManager.TaskStart();
        }
        private void TileLeftClickEnd()
        {
            LastCreatePos = new Vector2(-100);
            mapeditor.taskManager.TaskEnd();
        }

        private void TileDragEnd()
        {
            if (!mouse_IsLeftDrag)
            {
                TileRightMouseClick();
                return;
            }

            if (mapeditor.mapDataBinding.TILE_PAINTTYPE != Control.MapEditor.TileSetPaintType.SELECTION)
            {
                //브러시 모드일 경우
                TileDragPaint();
                LastCreatePos = new Vector2(-100);
                mapeditor.taskManager.TaskEnd();
            }
            else if (mapeditor.mapDataBinding.TILE_PAINTTYPE == Control.MapEditor.TileSetPaintType.SELECTION)
            {
                //선택모드
                float minx = Math.Min(mouse_DragMapStart.X, MouseMapPos.X);
                float miny = Math.Min(mouse_DragMapStart.Y, MouseMapPos.Y);
                float maxx = Math.Max(mouse_DragMapStart.X, MouseMapPos.X);
                float maxy = Math.Max(mouse_DragMapStart.Y, MouseMapPos.Y);

                minx = (float)(Math.Floor(minx / 32));
                miny = (float)(Math.Floor(miny / 32));
                maxx = (float)(Math.Ceiling(maxx / 32));
                maxy = (float)(Math.Ceiling(maxy / 32));

       


                for (int y = 0; y < (maxy - miny ); y++)
                {
                    for (int x = 0; x < (maxx - minx ); x++)
                    {
                        int tileindex = (int)((minx + x) + (miny + y) * mapeditor.mapdata.WIDTH);

                        if (!mapeditor.mapdata.CheckTILERange((int)(minx + x), (int)(miny + y))) continue;

                        mapeditor.Tile_SetSelectedTile((int)(minx + x), (int)(miny + y), mapeditor.mapdata.TILE[tileindex]);
                    }
                }

                //mapeditor.mapDataBinding.TILE_PAINTTYPE = MapEditor.TileSetPaintType.PENCIL;
                //mapeditor.tile_BrushMode = MapEditor.TileSetBrushMode.PASTE;
            }
        }


        private void TileRightMouseClick()
        {
            if (mapeditor.mapDataBinding.TILE_PAINTTYPE != Control.MapEditor.TileSetPaintType.SELECTION)
            {
                mapeditor.mapDataBinding.TILE_PAINTTYPE = Control.MapEditor.TileSetPaintType.SELECTION;
            }
            else
            {
                //메뉴 열기
                mapeditor.OpenTileMenu((int)MousePos.X, (int)MousePos.Y);
            }
        }


        private void PalletTitleBackGroundFill()
        {
            Color Back;
            if (Global.Setting.Vals[Global.Setting.Settings.Program_IsDark] == "true")
            {
                Back = new Color(0xFF303030);
            }
            else
            {
                Back = new Color(0xFFFAFAFA);
            }

            _spriteBatch.Begin();


            {
                Point relativePoint = mapeditor.Tile_ISOM_Expander.TransformToAncestor(mapeditor.RightExpander).Transform(new Point(0, 0));

                int startX = ((int)(screenwidth));
                int startY = (int)relativePoint.Y;

                _spriteBatch.Draw(gridtexture, new Rectangle((int)(startX), startY, RightToolBarStreachValue, (int)48), Back);
            }

            {
                Point relativePoint = mapeditor.Tile_All_Label.TransformToAncestor(mapeditor.RightExpander).Transform(new Point(0, 0));

                int startX = ((int)(screenwidth));
                int startY = (int)relativePoint.Y;

                _spriteBatch.Draw(gridtexture, new Rectangle((int)(startX), startY, RightToolBarStreachValue, (int)48), Back);
            }
            

            _spriteBatch.End();
        }



        private void DrawTileSetPallet()
        {
            if (mapeditor.PalleteLayer == Control.MapEditor.Layer.Tile)
            {
                DrawAllTileSet();

                DrawISOMSet();

                //DrawRectSet();

                //DrawCustomSet();



                PalletTitleBackGroundFill();

            }
        }




        private void DrawISOMSet()
        {
            Point relativePoint = mapeditor.Tile_ISOM_Pallet.TransformToAncestor(mapeditor.RightExpander).Transform(new Point(0, 0));

            int startX = ((int)(screenwidth));
            int startY = (int)relativePoint.Y;


            List<ISOMTIle> iSOMs = tileSet.GetISOMData(mapeditor);

            int columns = iSOMs.Count / 8 + 1;

            int isomindex = 0;

            AtlasTileSet atlasTileSet = tileSet.GetAtlasTileSetTexture(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE);


            _spriteBatch.Begin(blendState: BlendState.NonPremultiplied, samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(gridtexture, new Vector2(startX , startY ), null, Color.Black, 0, Vector2.Zero, new Vector2(32 * 16, 32), SpriteEffects.None, 0);
            for (int y = 0; y < columns; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if(isomindex >= iSOMs.Count)
                    {
                        break;
                    }

                    ISOMTIle iSOM = iSOMs[isomindex];

                    int megaindex1 = tileSet.GetMegaTileIndex(mapeditor.mapdata.TILETYPE, (ushort)(iSOM.group1list[0] / 16), (ushort)(iSOM.group1list[0] % 16));
                    int megaindex2 = tileSet.GetMegaTileIndex(mapeditor.mapdata.TILETYPE, (ushort)(iSOM.group2list[0] / 16), (ushort)(iSOM.group2list[0] % 16));

                    if (atlasTileSet != null && atlasTileSet.texture2D != null)
                    {
                        _spriteBatch.Draw(atlasTileSet.texture2D, new Vector2(startX + x * tilesize * 2, startY + y * tilesize), atlasTileSet.GetRect(megaindex1), Color.White, 0, Vector2.Zero, drawtilesize, SpriteEffects.None, 0);
                    }
                    if (atlasTileSet != null && atlasTileSet.texture2D != null)
                    {
                        _spriteBatch.Draw(atlasTileSet.texture2D, new Vector2(startX + (2 * x + 1) * tilesize, startY + y * tilesize), atlasTileSet.GetRect(megaindex2), Color.White, 0, Vector2.Zero, drawtilesize, SpriteEffects.None, 0);
                    }

                    if((startX + x * tilesize * 2 < MousePos.X && MousePos.X < startX + (2 * x + 2) * tilesize)
                        && (startY + y * tilesize < MousePos.Y && MousePos.Y < startY + (y + 1) * tilesize))
                    {
                        _spriteBatch.Draw(gridtexture, new Vector2(startX + x * tilesize * 2, startY + y * tilesize), null, new Color(128,128,128,32), 0, Vector2.Zero, new Vector2(tilesize * 2, tilesize), SpriteEffects.None, 0);
                        //호버링
                        if (mapeditor.TilePalleteISOMMouseDown)
                        {
                            //선택
                            mapeditor.SelectISOMIndex = isomindex;
                        }
                    }





                    isomindex++;
                }
            }

            for (int x = 1; x < 8; x++)
            {
                DrawLine(_spriteBatch, new Vector2(startX + (2 * x) * tilesize, startY), new Vector2(startX + (2 * x) * tilesize, startY + columns * tilesize), Color.Black);
            }
            for (int y = 1; y < columns; y++)
            {
                DrawLine(_spriteBatch, new Vector2(startX, startY + y * tilesize), new Vector2(startX + 16 * tilesize, startY + y * tilesize), Color.Black);
            }

            if(mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.ISOM)
            {
                if (mapeditor.SelectISOMIndex != -1 && mapeditor.SelectISOMIndex < iSOMs.Count)
                {
                    int x, y;

                    x = mapeditor.SelectISOMIndex % 8;
                    y = mapeditor.SelectISOMIndex / 8;

                    _spriteBatch.Draw(gridtexture, new Vector2(startX + x * tilesize * 2, startY + y * tilesize), null, new Color(128, 128, 192, 64), 0, Vector2.Zero, new Vector2(tilesize * 2, tilesize), SpriteEffects.None, 0);
                    DrawRect(_spriteBatch, new Vector2(startX + x * tilesize * 2, startY + y * tilesize), new Vector2(startX + (x + 1) * tilesize * 2, startY + (y + 1) * tilesize), Color.White);
                }
            }





            _spriteBatch.End();
        }


        private void DrawAllTileSet()
        {
            int yitemcount = (int)(mapeditor.Tile_All_Pallet.ActualHeight / tilesize) + 3;

            double maxvalue = tileSet.cv5data[mapeditor.mapdata.TILETYPE].Length * 32 - mapeditor.Tile_All_Pallet.ActualHeight;
            if (maxvalue != mapeditor.TileScroll.Maximum)
            {
                mapeditor.TileScroll.Maximum = maxvalue;
            }


            Point relativePoint = mapeditor.Tile_All_Pallet.TransformToAncestor(mapeditor.RightExpander).Transform(new Point(0, 0));


            int startX = ((int)(screenwidth));
            int startY = (int)relativePoint.Y - mapeditor.brush_tilescroll % 30;

            int tiley = mapeditor.brush_tilescroll / 30;

            AtlasTileSet atlasTileSet = tileSet.GetAtlasTileSetTexture(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE);

            _spriteBatch.Begin();
            for (int y = 0; y < yitemcount; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    switch (mapeditor.opt_drawType)
                    {
                        case Control.MapEditor.DrawType.SD:
                            {
                                //Texture2D texture2D = tileSet.GetTile(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, (ushort)(y + tiley), (ushort)x);
                                int megaindex = tileSet.GetMegaTileIndex(mapeditor.mapdata.TILETYPE, (ushort)(y + tiley), (ushort)x);
                                if (atlasTileSet != null && atlasTileSet.texture2D != null)
                                {
                                    _spriteBatch.Draw(atlasTileSet.texture2D, new Vector2(startX + x * tilesize, startY + y * tilesize), atlasTileSet.GetRect(megaindex), Color.White, 0, Vector2.Zero, tilesize / 32, SpriteEffects.None, 0);
                                }
                            }
                            break;
                        case Control.MapEditor.DrawType.HD:
                        case Control.MapEditor.DrawType.CB:
                            {
                                //Texture2D texture2D = tileSet.GetTile(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, (ushort)(y + tiley), (ushort)x);
                                int megaindex = tileSet.GetMegaTileIndex(mapeditor.mapdata.TILETYPE, (ushort)(y + tiley), (ushort)x);
                                if (atlasTileSet != null && atlasTileSet.texture2D != null)
                                {
                                    _spriteBatch.Draw(atlasTileSet.texture2D, new Vector2(startX + x * tilesize, startY + y * tilesize), atlasTileSet.GetRect(megaindex), Color.White, 0, Vector2.Zero, tilesize / 64, SpriteEffects.None, 0);
                                }
                            }
                            break;
                    }



                    //if ((startX + x * tilesize < MousePos.X && MousePos.X < startX + (x + 1) * tilesize)
                    //    && (startY + y * tilesize < MousePos.Y && MousePos.Y < startY + (y + 1) * tilesize))
                    //{
                    //    _spriteBatch.Draw(gridtexture, new Vector2(startX + x * tilesize, startY + y * tilesize), null, new Color(128, 128, 128, 32), 0, Vector2.Zero, new Vector2(tilesize, tilesize), SpriteEffects.None, 0);
                    //    //호버링
                    //    if (mapeditor.TileAllMouseDown)
                    //    {
                    //        //선택
                    //        mapeditor.SelectALLTILEIndex.Add(10);
                    //    }
                    //}

                }
            }

            
            for (int x = 1; x < 16; x++)
            {
                DrawLine(_spriteBatch, new Vector2(startX + x * tilesize, startY), new Vector2(startX + x * tilesize, startY + yitemcount * tilesize), Color.Black);
            }
            for (int y = 1; y < yitemcount; y++)
            {
                DrawLine(_spriteBatch, new Vector2(startX, startY + y * tilesize), new Vector2(startX + 16 * tilesize, startY + y * tilesize), Color.Black);
            }




            //선택 강조
            if (mapeditor.tile_SelectPalleteALLTILEStartXIndex != -1 && mapeditor.tile_SelectPalleteALLTILEStartYIndex != -1 && mapeditor.tile_SelectPalleteALLTILEEndXIndex != -1 && mapeditor.tile_SelectPalleteALLTILEEndYIndex != -1)
            {
                int xstart = Math.Min(mapeditor.tile_SelectPalleteALLTILEStartXIndex, mapeditor.tile_SelectPalleteALLTILEEndXIndex);
                int ystart = Math.Min(mapeditor.tile_SelectPalleteALLTILEStartYIndex, mapeditor.tile_SelectPalleteALLTILEEndYIndex);
                int xend = Math.Max(mapeditor.tile_SelectPalleteALLTILEStartXIndex, mapeditor.tile_SelectPalleteALLTILEEndXIndex);
                int yend = Math.Max(mapeditor.tile_SelectPalleteALLTILEStartYIndex, mapeditor.tile_SelectPalleteALLTILEEndYIndex);


                int w = xend - xstart + 1;
                int h = yend - ystart + 1;


                float drawX = startX + xstart * tilesize;
                float drawY = startY + ystart * tilesize - tiley * tilesize;


                _spriteBatch.Draw(gridtexture, new Vector2(drawX, drawY), null, new Color(128, 128, 192, 64), 0, Vector2.Zero, new Vector2(tilesize * w, tilesize * h), SpriteEffects.None, 0);
            }

            //호버링
            if (mapeditor.TilePalleteAllMouseDown)
            {
                int xstart = Math.Min(mapeditor.TilePalleteMouseStartXIndex, mapeditor.TilePalleteMouseMoveXIndex);
                int ystart = Math.Min(mapeditor.TilePalleteMouseStartYIndex, mapeditor.TilePalleteMouseMoveYIndex);
                int xend = Math.Max(mapeditor.TilePalleteMouseStartXIndex, mapeditor.TilePalleteMouseMoveXIndex);
                int yend = Math.Max(mapeditor.TilePalleteMouseStartYIndex, mapeditor.TilePalleteMouseMoveYIndex);

                int w = xend - xstart + 1;
                int h = yend - ystart + 1;

                _spriteBatch.Draw(gridtexture, new Vector2(startX + xstart * tilesize, startY + ystart * tilesize - tiley * tilesize), null, new Color(128, 128, 128, 64), 0, Vector2.Zero, new Vector2(tilesize * w, tilesize * h), SpriteEffects.None, 0);
            }


            _spriteBatch.End();
        }

        private void DD2ToTile(CDD2 cDD2)
        {
            DoodadPallet pallete = UseMapEditor.Global.WindowTool.MapViewer.tileSet.DoodadPallets[mapeditor.mapdata.TILETYPE][cDD2.ID];

            int _x = cDD2.X / 32 - (pallete.dddWidth / 2);
            int _y = cDD2.Y / 32 - (pallete.dddHeight / 2);


            for (int y = 0; y < pallete.dddHeight; y++)
            {

                for (int x = 0; x < pallete.dddWidth; x++)
                {
                    if (!((0 <= _x + x && _x + x < mapeditor.mapdata.WIDTH) && (0 <= _y + y && _y + y < mapeditor.mapdata.HEIGHT)))
                    {
                        continue;
                    }


                    ushort group = (ushort)(pallete.dddGroup + y);
                    ushort index = (ushort)x;


                    if (UseMapEditor.Global.WindowTool.MapViewer.tileSet.IsBlack(mapeditor.mapdata.TILETYPE, group, index))
                    {
                        continue;
                    }

                    int mapx = _x + x;
                    int mapy = _y + y;

                    int tileindex = mapx + mapy * mapeditor.mapdata.WIDTH;

                    ushort newMTXM = (ushort)(index + (group << 4));
                    ushort oldMTXM = mapeditor.mapdata.TILE[tileindex];


                    mapeditor.mapdata.TILEChange(mapx, mapy, newMTXM);
                    mapeditor.taskManager.TaskAdd(new TileEvent(mapeditor, newMTXM, oldMTXM, mapx, mapy));
                }
            }
            mapeditor.mapdata.TILEChangeComplete();
        }


        private void TileDragPaint()
        {
            if (mapeditor.tile_PaintType == MapEditor.TileSetPaintType.RECT)
            {
                Vector2 mappos = MouseTilePos;


                int width = 0;
                int height = 0;

                bool IsOneTile = false;
                if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.ISOM)
                {
                    if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.ISOM && mapeditor.brush_useRect)
                    {
                        width = mapeditor.brush_x;
                        height = mapeditor.brush_y;
                    }
                    else if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.ISOM && !mapeditor.brush_useRect)
                    {
                        //ISOM RECT로직
                        Vector2 DragStartMap = mouse_DragMapStart;
                        Vector2 DragStartTile;

                        DragStartTile.X = (float)Math.Floor(DragStartMap.X / 32);
                        DragStartTile.Y = (float)Math.Floor(DragStartMap.Y / 32);

                        Vector2 spoint = ISOMTool.GetMtxmRectCenter(MouseTilePos, MouseMapPos);
                        Vector2 epoint = ISOMTool.GetMtxmRectCenter(DragStartTile, DragStartMap);

                        Vector2 lpoint1;
                        Vector2 lpoint2;

                        ISOMTool.GetIntersectionPoint(spoint, epoint, out lpoint1, out lpoint2);


                        int topx = 0, leftx = 0;
                        int bottomy = 0, lefty = 0;

                        bool sp = true, ep = true, lp1 = true, lp2 = true;
                        if (Math.Max(Math.Max(spoint.X, epoint.X), Math.Max(lpoint1.X, lpoint2.X)) == spoint.X && sp)
                        {
                            //가장 오른쪽에 있는 경우.
                            spoint.X += 2;
                            sp = false;
                        }
                        if (Math.Max(Math.Max(spoint.X, epoint.X), Math.Max(lpoint1.X, lpoint2.X)) == epoint.X && ep)
                        {
                            //가장 오른쪽에 있는 경우.
                            epoint.X += 2;
                            ep = false;
                        }
                        if (Math.Max(Math.Max(spoint.X, epoint.X), Math.Max(lpoint1.X, lpoint2.X)) == lpoint1.X && lp1)
                        {
                            //가장 오른쪽에 있는 경우.
                            lpoint1.X += 2;
                            lp1 = false;
                        }
                        if (Math.Max(Math.Max(spoint.X, epoint.X), Math.Max(lpoint1.X, lpoint2.X)) == lpoint2.X && lp2)
                        {
                            //가장 오른쪽에 있는 경우.
                            lpoint2.X += 2;
                            lp2 = false;
                        }

                        if (Math.Max(Math.Max(spoint.Y, epoint.Y), Math.Max(lpoint1.Y, lpoint2.Y)) == lpoint2.Y && lp2)
                        {
                            //가장 위에 있는 경우.
                            lpoint2.Y += 1;
                            bottomy = (int)lpoint2.Y;
                            lp2 = false;
                        }
                        if (Math.Max(Math.Max(spoint.Y, epoint.Y), Math.Max(lpoint1.Y, lpoint2.Y)) == lpoint1.Y && lp1)
                        {
                            //가장 위에 있는 경우.
                            lpoint1.Y += 1;
                            bottomy = (int)lpoint1.Y;
                            lp1 = false;
                        }
                        if (Math.Max(Math.Max(spoint.Y, epoint.Y), Math.Max(lpoint1.Y, lpoint2.Y)) == epoint.Y && ep)
                        {
                            //가장 위에 있는 경우.
                            epoint.Y += 1;
                            bottomy = (int)epoint.Y;
                            ep = false;
                        }
                        if (Math.Max(Math.Max(spoint.Y, epoint.Y), Math.Max(lpoint1.Y, lpoint2.Y)) == spoint.Y && sp)
                        {
                            //가장 위에 있는 경우.
                            spoint.Y += 1;
                            bottomy = (int)spoint.Y;
                            sp = false;
                        }

                        if (Math.Min(Math.Min(spoint.X, epoint.X), Math.Min(lpoint1.X, lpoint2.X)) == spoint.X && sp)
                        {
                            //가장 왼쪽에 있는 경우.
                            spoint.X -= 2;
                            leftx = (int)spoint.X;
                            lefty = (int)spoint.Y;
                            sp = false;
                        }
                        if (Math.Min(Math.Min(spoint.X, epoint.X), Math.Min(lpoint1.X, lpoint2.X)) == epoint.X && ep)
                        {
                            //가장 왼쪽에 있는 경우.
                            epoint.X -= 2;
                            leftx = (int)epoint.X;
                            lefty = (int)epoint.Y;
                            ep = false;
                        }
                        if (Math.Min(Math.Min(spoint.X, epoint.X), Math.Min(lpoint1.X, lpoint2.X)) == lpoint1.X && lp1)
                        {
                            //가장 왼쪽에 있는 경우.
                            lpoint1.X -= 2;
                            leftx = (int)lpoint1.X;
                            lefty = (int)lpoint1.Y;
                            lp1 = false;
                        }
                        if (Math.Min(Math.Min(spoint.X, epoint.X), Math.Min(lpoint1.X, lpoint2.X)) == lpoint2.X && lp2)
                        {
                            //가장 왼쪽에 있는 경우.
                            lpoint2.X -= 2;
                            leftx = (int)lpoint2.X;
                            lefty = (int)lpoint2.Y;
                            lp2 = false;
                        }

                        if (Math.Min(Math.Min(spoint.Y, epoint.Y), Math.Min(lpoint1.Y, lpoint2.Y)) == lpoint2.Y && lp2)
                        {
                            //가장 아래에 있는 경우.
                            lpoint2.Y -= 1;
                            topx = (int)lpoint2.X;
                            lp2 = false;
                        }
                        if (Math.Min(Math.Min(spoint.Y, epoint.Y), Math.Min(lpoint1.Y, lpoint2.Y)) == lpoint1.Y && lp1)
                        {
                            //가장 아래에 있는 경우.
                            lpoint1.Y -= 1;
                            topx = (int)lpoint1.X;
                            lp1 = false;
                        }
                        if (Math.Min(Math.Min(spoint.Y, epoint.Y), Math.Min(lpoint1.Y, lpoint2.Y)) == epoint.Y && ep)
                        {
                            //가장 아래에 있는 경우.
                            epoint.Y -= 1;
                            topx = (int)epoint.X;
                            ep = false;
                        }
                        if (Math.Min(Math.Min(spoint.Y, epoint.Y), Math.Min(lpoint1.Y, lpoint2.Y)) == spoint.Y && sp)
                        {
                            //가장 아래에 있는 경우.
                            spoint.Y -= 1;
                            topx = (int)spoint.X;
                            sp = false;
                        }

                        int xcount = (topx - leftx) / 2;
                        int ycount = bottomy - lefty;

                        Vector2 startpoint = new Vector2(leftx, lefty);



                        for (int _y = 0; _y < ycount + 1; _y++)
                        {
                            Vector2 t = startpoint;
                            t.X += _y * 2;
                            t.Y += _y * 1;
                            for (int _x = 0; _x < xcount + 1; _x++)
                            {
                                List<ISOMTIle> iSOMs = tileSet.GetISOMData(mapeditor);

                                ISOMTIle isom = iSOMs[mapeditor.SelectISOMIndex];
                                ISOMTool.ISOMExecute(mapeditor, tileSet, isom, (int)t.X, (int)t.Y);

                                t.X += 2;
                                t.Y -= 1;
                            }
                        }
                        mapeditor.mapdata.TILEChangeComplete();
                        return;
                    }
                }
                else
                {
                    if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.ALLTILE)
                    {
                        //붙여넣기
                        width = (int)(mapeditor.tile_PalleteSelectEnd.X - mapeditor.tile_PalleteSelectStart.X) + 1;
                        height = (int)(mapeditor.tile_PalleteSelectEnd.Y - mapeditor.tile_PalleteSelectStart.Y) + 1;
                    }
                    else if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.PASTE)
                    {
                        if (mapeditor.tile_CopyedTile.Count > 0)
                        {
                            width = (int)mapeditor.tile_CopyedTileSize.X;
                            height = (int)mapeditor.tile_CopyedTileSize.Y;
                        }
                    }

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

                }

                Vector2 DragStart = mouse_DragMapStart;
                Vector2 DragEndtemp = MouseTilePos;

                DragStart.X = (float)Math.Floor(DragStart.X / 32);
                DragStart.Y = (float)Math.Floor(DragStart.Y / 32);

                if (DragStart.X > DragEndtemp.X)
                {
                    DragStart.X += 1;
                }
                if (DragStart.Y > DragEndtemp.Y)
                {
                    DragStart.Y += 1;
                }

                if (DragStart.X <= DragEndtemp.X)
                {
                    DragEndtemp.X += 1;
                }
                if (DragStart.Y <= DragEndtemp.Y)
                {
                    DragEndtemp.Y += 1;
                }

                Vector2 DragEnd;
                if (GlobalVariable.key_LeftShiftDown)
                {
                    Vector2 differ = DragEndtemp - DragStart;
                    double DragLen = Math.Max(Math.Abs(differ.X), Math.Abs(differ.Y));

                    if (differ.X > 0)
                    {
                        //양수 일 경우
                        DragEnd.X = (float)(DragStart.X + DragLen);
                    }
                    else
                    {
                        //음수 일 경우
                        DragEnd.X = (float)(DragStart.X - DragLen);
                    }

                    if (differ.Y > 0)
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



                int dragsx = (int)Math.Min(DragStart.X, DragEnd.X);
                int dragsy = (int)Math.Min(DragStart.Y, DragEnd.Y);
                int dragex = (int)Math.Max(DragStart.X, DragEnd.X);
                int dragey = (int)Math.Max(DragStart.Y, DragEnd.Y);

                int x = 0;
                int y = 0;


                if (DragStart.X > DragEnd.X)
                {
                    int w = (int)(DragStart.X - DragEnd.X);

                    x = ((int)((Math.Floor((double)(w / width)) + 1) * width - w));
                }

                if (DragStart.Y > DragEnd.Y)
                {
                    int h = (int)(DragStart.Y - DragEnd.Y);

                    y = ((int)((Math.Floor((double)(h / height)) + 1) * height - h));
                }

                for (int mapy = dragsy; mapy < dragey; mapy++)
                {
                    if (DragStart.X > DragEnd.X)
                    {
                        int w = (int)(DragStart.X - DragEnd.X);

                        x = ((int)((Math.Floor((double)(w / width)) + 1) * width - w));
                    }
                    else
                    {
                        x = 0;
                    }

                    for (int mapx = dragsx; mapx < dragex; mapx++)
                    {
                        int megaindex = 0;


                        ushort newMTXM = 0;

                        if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.ALLTILE)
                        {
                            if (IsOneTile)
                            {
                                newMTXM = (ushort)((ushort)(mapeditor.tile_PalleteSelectStart.X) + ((ushort)(mapeditor.tile_PalleteSelectStart.Y) << 4));
                                megaindex = tileSet.GetMegaTileIndex(mapeditor.mapdata.TILETYPE, newMTXM);
                            }
                            else
                            {
                                newMTXM = (ushort)((ushort)(mapeditor.tile_PalleteSelectStart.X + (x % width)) + ((ushort)(mapeditor.tile_PalleteSelectStart.Y + (y % height)) << 4));
                                megaindex = tileSet.GetMegaTileIndex(mapeditor.mapdata.TILETYPE, newMTXM);
                            }
                        }
                        else if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.PASTE)
                        {
                            if (IsOneTile)
                            {
                                newMTXM = mapeditor.Tile_GetCopyedTile(0, 0);
                                if (newMTXM == ushort.MaxValue) continue;
                                megaindex = tileSet.GetMegaTileIndex(mapeditor.mapdata.TILETYPE, newMTXM);
                            }
                            else
                            {
                                newMTXM = mapeditor.Tile_GetCopyedTile(x % width, y % height);
                                if (newMTXM == ushort.MaxValue) continue;
                                megaindex = tileSet.GetMegaTileIndex(mapeditor.mapdata.TILETYPE, newMTXM);
                            }
                        }
                        else if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.ISOM)
                        {
                            List<ISOMTIle> iSOMs = tileSet.GetISOMData(mapeditor);

                            newMTXM = iSOMs[mapeditor.SelectISOMIndex].GetFlatTile(mapx, mapy, mapeditor.mapdata);
                        }

                        if (mapeditor.TilePalleteTransparentBlack && megaindex == 0) continue;

                        int tileindex = mapx + mapy * mapeditor.mapdata.WIDTH;

                        if (mapeditor.mapdata.CheckTILERange(mapx, mapy))
                        {
                            ushort oldMTXM = mapeditor.mapdata.TILE[tileindex];

                            mapeditor.mapdata.TILEChange(mapx, mapy, newMTXM);
                            mapeditor.taskManager.TaskAdd(new TileEvent(mapeditor, newMTXM, oldMTXM, mapx, mapy));
                        }

                        x++;
                    }
                    y++;
                }

                mapeditor.mapdata.TILEChangeComplete();
            }
            else if (mapeditor.tile_PaintType == MapEditor.TileSetPaintType.CIRCLE)
            {
                int width = 0;
                int height = 0;



                bool IsOneTile = false;
                if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.ISOM)
                {
                    if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.ISOM && mapeditor.brush_useRect)
                    {
                        width = mapeditor.brush_x;
                        height = mapeditor.brush_y;
                    }
                    else if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.ISOM && !mapeditor.brush_useRect)
                    {
                        //ISOM RECT로직
                        {
                            //드래그 하면 미리보기 그려짐
                            //TODO:드래그시 미리보기 그리기

                            Vector2 DragStart = mouse_DragMapStart;

                            DragStart.X = (float)Math.Floor(DragStart.X / 32);
                            DragStart.Y = (float)Math.Floor(DragStart.Y / 32);

                            DragStart = ISOMTool.GetMtxmRectCenter(DragStart, mouse_DragMapStart);
                            Vector2 DragEndtemp = ISOMTool.GetMtxmRectCenter(MouseTilePos, MouseMapPos);


                            Vector2 DragEnd;
                            if (GlobalVariable.key_LeftShiftDown)
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

                            double radius = Math.Max(Dragwidth / 2, Dragheight / 2);

                            double widthscale = radius / (Dragheight / 2);
                            double heightscale = radius / (Dragwidth / 2);


                            Dictionary<string, bool> map = new Dictionary<string, bool>();

                            Vector2 DragMin = new Vector2(Math.Min(DragStart.X, DragEnd.X), Math.Min(DragStart.Y, DragEnd.Y));
                            Vector2 centertilepos = DragMin + center;
                            List<ISOMTIle> iSOMs = tileSet.GetISOMData(mapeditor);
                            ISOMTIle isom = iSOMs[mapeditor.SelectISOMIndex];
                            for (int y = 0; y < Dragheight; y += 1)
                            {
                                if (Dragheight <= 1) break;

                                if (Size.X * Size.Y < 0)
                                {
                                    //음수일 경우 작동한다
                                    if (Dragheight % 2 == 0)
                                    {
                                        if (y == 0)
                                        {
                                            y = 1;
                                        }
                                    }
                                    else
                                    {
                                        if (y == Dragheight - 1)
                                        {
                                            continue;
                                        }
                                    }
                                }
                                else
                                {
                                    if (y == 0)
                                    {
                                        y = 1;
                                    }
                                }

                                if (Dragwidth == 4 && y % 2 == 0) continue;

                                for (int x = 2; x < Dragwidth; x += 4)
                                {
                                    if (y % 2 == 0 && x == 2 && Dragwidth != 4) x += 2;


                                    Vector2 tilepos = DragMin + new Vector2(x, y);

                                    if (Size.X * Size.Y < 0 && Dragheight % 2 == 1)
                                    {
                                        //음수일 경우 작동한다
                                        tilepos.Y += 1;
                                    }

                                    double tx = (centertilepos.X - tilepos.X) * heightscale;
                                    double ty = (centertilepos.Y - tilepos.Y) * widthscale;

                                    double d = Math.Sqrt(Math.Pow(tx, 2) + Math.Pow(ty, 2));

                                    if (radius >= d + 2)
                                    {
                                        if (!map.ContainsKey((tilepos.X - 2).ToString() + (tilepos.Y).ToString()))
                                        {
                                            ISOMTool.ISOMExecute(mapeditor, tileSet, isom, (int)tilepos.X - 2, (int)tilepos.Y);
                                            map.Add((tilepos.X - 2).ToString() + (tilepos.Y).ToString(), true);
                                        }
                                        if (!map.ContainsKey((tilepos.X + 2).ToString() + (tilepos.Y).ToString()))
                                        {
                                            ISOMTool.ISOMExecute(mapeditor, tileSet, isom, (int)tilepos.X + 2, (int)tilepos.Y);
                                            map.Add((tilepos.X + 2).ToString() + (tilepos.Y).ToString(), true);
                                        }
                                        if (!map.ContainsKey((tilepos.X).ToString() + (tilepos.Y + 1).ToString()))
                                        {
                                            ISOMTool.ISOMExecute(mapeditor, tileSet, isom, (int)tilepos.X, (int)tilepos.Y + 1);
                                            map.Add((tilepos.X).ToString() + (tilepos.Y + 1).ToString(), true);
                                        }
                                        if (!map.ContainsKey((tilepos.X).ToString() + (tilepos.Y - 1).ToString()))
                                        {
                                            ISOMTool.ISOMExecute(mapeditor, tileSet, isom, (int)tilepos.X, (int)tilepos.Y - 1);
                                            map.Add((tilepos.X).ToString() + (tilepos.Y -1).ToString(), true);
                                        }
                                    }

                                }
                            }
                      
                        }
                        mapeditor.mapdata.TILEChangeComplete();
                        return;
                    }
                }
                else
                {
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
                }


                {
                    Vector2 DragStart = mouse_DragMapStart;
                    Vector2 DragEndtemp = MouseTilePos;

                    DragStart.X = (float)Math.Floor(DragStart.X / 32);
                    DragStart.Y = (float)Math.Floor(DragStart.Y / 32);

                    if (DragStart.X > DragEndtemp.X)
                    {
                        DragStart.X += 1;
                    }
                    if (DragStart.Y > DragEndtemp.Y)
                    {
                        DragStart.Y += 1;
                    }

                    if (DragStart.X > DragEndtemp.X)
                    {
                        DragStart.X -= 1;
                    }
                    if (DragStart.Y > DragEndtemp.Y)
                    {
                        DragStart.Y -= 1;
                    }

                    Vector2 DragEnd;
                    if (GlobalVariable.key_LeftShiftDown)
                    {
                        Vector2 differ = DragEndtemp - DragStart;
                        double DragLen = Math.Max(Math.Abs(differ.X), Math.Abs(differ.Y));

                        if (differ.X > 0)
                        {
                            //양수 일 경우
                            DragEnd.X = (float)(DragStart.X + DragLen);
                        }
                        else
                        {
                            //음수 일 경우
                            DragEnd.X = (float)(DragStart.X - DragLen);
                        }

                        if (differ.Y > 0)
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

                    int cx = 0, cy = 0;

                    if (DragStart.Y > DragEnd.Y)
                    {
                        int h = (int)(DragStart.Y - DragEnd.Y);

                        cy = ((int)((Math.Floor((double)(h / height)) + 1) * height - h));
                    }

                    //_spriteBatch.Begin();
                    for (int y = 0; y < Dragheight + 1; y++)
                    {
                        if (DragStart.X > DragEnd.X)
                        {
                            int w = (int)(DragStart.X - DragEnd.X);

                            cx = ((int)((Math.Floor((double)(w / width)) + 1) * width - w));
                        }
                        else
                        {
                            cx = 0;
                        }


                        for (int x = 0; x < Dragwidth + 1; x++)
                        {
                            double tx = (center.X - x) * heightscale;
                            double ty = (center.Y - y) * widthscale;

                            double d = Math.Sqrt(Math.Pow(tx, 2) + Math.Pow(ty, 2));
                            if (Math.Max(Dragwidth, Dragheight) % 2 == 1)
                            {
                                d -= 1;
                            }
                            else
                            {
                                d -= 0.4;
                            }

                            if (radius >= d)
                            {
                                flag[x, y] = true;

                                int megaindex = 0;


                                ushort newMTXM = 0;

                                int mapx = (int)(DragMin.X + x);
                                int mapy = (int)(DragMin.Y + y);

                                if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.ALLTILE)
                                {
                                    if (IsOneTile)
                                    {
                                        newMTXM = (ushort)((ushort)(mapeditor.tile_PalleteSelectStart.X) + ((ushort)(mapeditor.tile_PalleteSelectStart.Y) << 4));
                                        megaindex = tileSet.GetMegaTileIndex(mapeditor.mapdata.TILETYPE, newMTXM);
                                    }
                                    else
                                    {
                                        newMTXM = (ushort)((ushort)(mapeditor.tile_PalleteSelectStart.X + (cx % width)) + ((ushort)(mapeditor.tile_PalleteSelectStart.Y + (cy % height)) << 4));
                                        megaindex = tileSet.GetMegaTileIndex(mapeditor.mapdata.TILETYPE, newMTXM);
                                    }
                                }
                                else if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.PASTE)
                                {
                                    if (IsOneTile)
                                    {
                                        newMTXM = mapeditor.Tile_GetCopyedTile(0, 0);
                                        if (newMTXM == ushort.MaxValue) continue;
                                        megaindex = tileSet.GetMegaTileIndex(mapeditor.mapdata.TILETYPE, newMTXM);
                                    }
                                    else
                                    {
                                        newMTXM = mapeditor.Tile_GetCopyedTile(cx % width, cy % height);
                                        if (newMTXM == ushort.MaxValue) continue;
                                        megaindex = tileSet.GetMegaTileIndex(mapeditor.mapdata.TILETYPE, newMTXM);
                                    }
                                }
                                else if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.ISOM)
                                {
                                    List<ISOMTIle> iSOMs = tileSet.GetISOMData(mapeditor);

                                    newMTXM = iSOMs[mapeditor.SelectISOMIndex].GetFlatTile(mapx, mapy, mapeditor.mapdata);
                                }



                                if (mapeditor.TilePalleteTransparentBlack && megaindex == 0) continue;


                                int tileindex = mapx + mapy * mapeditor.mapdata.WIDTH;

                                if (mapeditor.mapdata.CheckTILERange(mapx, mapy))
                                {
                                    ushort oldMTXM = mapeditor.mapdata.TILE[tileindex];

                                    mapeditor.mapdata.TILEChange(mapx, mapy, newMTXM);
                                    mapeditor.taskManager.TaskAdd(new TileEvent(mapeditor, newMTXM, oldMTXM, mapx, mapy));
                                }


                                //DrawRect(_spriteBatch, mapeditor.PosMapToScreen(32 * (DragMin + new Vector2(x, y))), mapeditor.PosMapToScreen(32 * (DragMin + new Vector2(x + 1, y + 1))), new Color(138, 43, 226, 128), 3, true);
                            }
                            cx++;
                        }
                        cy++;
                    }
                    mapeditor.mapdata.TILEChangeComplete();

                }
                //_spriteBatch.End();
            }
        }


        Vector2 lastpaint = new Vector2(-1, -1);
        private void TilePaint()
        {
            //TODO:타일 넣기
            //if (mapeditor.mapDataBinding.TILE_SELECTMODE)
            //{
            //    return;
            //}

            if (mouse_LeftDown)
            {
                if (GlobalVariable.key_QDown)
                {
                    Vector2 mappos = MouseTilePos;

                    mapeditor.tile_BrushMode = Control.MapEditor.TileSetBrushMode.ALLTILE;
                    mapeditor.tile_PaintType = MapEditor.TileSetPaintType.PENCIL;

                    int mapx = (ushort)mappos.X;
                    int mapy = (ushort)mappos.Y;
                    int tileindex = mapx + mapy * mapeditor.mapdata.WIDTH;

                    ushort oldMTXM = mapeditor.mapdata.TILE[tileindex];


                    mapeditor.Tile_SetPalletFromMtxm(oldMTXM, true);
                    mapeditor.editorTextureData.TilePaletteRefresh();
                }
                else
                {
                    if (mapeditor.tile_PaintType == MapEditor.TileSetPaintType.PENCIL)
                    {
                        Vector2 mappos = MouseTilePos;


                        int width = 0;
                        int height = 0;

                        if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.ALLTILE)
                        {
                            //붙여넣기
                            width = (int)(mapeditor.tile_PalleteSelectEnd.X - mapeditor.tile_PalleteSelectStart.X) + 1;
                            height = (int)(mapeditor.tile_PalleteSelectEnd.Y - mapeditor.tile_PalleteSelectStart.Y) + 1;
                        }
                        else if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.PASTE)
                        {
                            if (mapeditor.tile_CopyedTile.Count > 0)
                            {
                                width = (int)mapeditor.tile_CopyedTileSize.X;
                                height = (int)mapeditor.tile_CopyedTileSize.Y;
                            }
                        }
                        else if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.ISOM)
                        {
                            width = (int)mapeditor.tile_CopyedTileSize.X;
                            height = (int)mapeditor.tile_CopyedTileSize.Y;
                        }


                        if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.ISOM && !mapeditor.brush_useRect)
                        {
                            if (lastpaint.X == MouseTilePos.X && lastpaint.Y == MouseTilePos.Y)
                            {
                                mapeditor.mapdata.TILEChangeComplete();
                                return;
                            }
                            if(mapeditor.SelectISOMIndex == -1)
                            {
                                mapeditor.mapdata.TILEChangeComplete();
                                return;
                            }

                            List<ISOMTIle> iSOMs = tileSet.GetISOMData(mapeditor);
                            ////TODO:ISOM 미리그리기
                            ///
                            int brush_x = mapeditor.brush_x - 1;
                            int brush_y = mapeditor.brush_y - 1;

                            Vector2 startpoint = ISOMTool.GetMtxmRectBrush(MouseTilePos, MouseMapPos, brush_x, brush_y) + new Vector2(-2, 0);

                            startpoint.X += -brush_x / 2 * 2 - brush_y / 2 * 2;
                            startpoint.Y += brush_x / 2 - brush_y / 2;

                            for (int y = 0; y < mapeditor.brush_y; y++)
                            {
                                Vector2 t = startpoint;
                                t.X += y * 2;
                                t.Y += y * 1;
                                for (int x = 0; x < mapeditor.brush_x; x++)
                                {
                                    ISOMTIle isom = iSOMs[mapeditor.SelectISOMIndex];
                                    ISOMTool.ISOMExecute(mapeditor, tileSet, isom, (int)t.X, (int)t.Y);

                                    t.X += 2;
                                    t.Y -= 1;
                                }
                            }


                        }
                        else
                        {
                            bool IsOneTile = false;
                            if (width == 1 && height == 1)
                            {
                                IsOneTile = true;
                                width = mapeditor.brush_x;
                                height = mapeditor.brush_y;
                            }

                            int sx = (int)Math.Floor(width / 2.0);
                            int sy = (int)Math.Floor(height / 2.0);

                            for (int y = 0; y < height; y++)
                            {
                                for (int x = 0; x < width; x++)
                                {
                                    int megaindex = 0;


                                    ushort newMTXM = 0;

                                    int mapx = (ushort)mappos.X + (x - sx);
                                    int mapy = (ushort)mappos.Y + (y - sy);

                                    if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.ALLTILE)
                                    {
                                        if (IsOneTile)
                                        {
                                            newMTXM = (ushort)((ushort)(mapeditor.tile_PalleteSelectStart.X) + ((ushort)(mapeditor.tile_PalleteSelectStart.Y) << 4));
                                            megaindex = tileSet.GetMegaTileIndex(mapeditor.mapdata.TILETYPE, newMTXM);
                                        }
                                        else
                                        {
                                            newMTXM = (ushort)((ushort)(mapeditor.tile_PalleteSelectStart.X + x) + ((ushort)(mapeditor.tile_PalleteSelectStart.Y + y) << 4));
                                            megaindex = tileSet.GetMegaTileIndex(mapeditor.mapdata.TILETYPE, newMTXM);
                                        }
                                    }
                                    else if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.PASTE)
                                    {
                                        if (IsOneTile)
                                        {
                                            newMTXM = mapeditor.Tile_GetCopyedTile(0, 0);
                                            if (newMTXM == ushort.MaxValue) continue;
                                            megaindex = tileSet.GetMegaTileIndex(mapeditor.mapdata.TILETYPE, newMTXM);
                                        }
                                        else
                                        {
                                            newMTXM = mapeditor.Tile_GetCopyedTile(x, y);
                                            if (newMTXM == ushort.MaxValue) continue;
                                            megaindex = tileSet.GetMegaTileIndex(mapeditor.mapdata.TILETYPE, newMTXM);
                                        }
                                    }
                                    else if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.ISOM)
                                    {
                                        if(lastpaint.X == MouseTilePos.X && lastpaint.Y == MouseTilePos.Y)
                                        {
                                            mapeditor.mapdata.TILEChangeComplete();
                                            return;
                                        }
                                        if (mapeditor.SelectISOMIndex == -1)
                                        {
                                            mapeditor.mapdata.TILEChangeComplete();
                                            return;
                                        }

                                        List<ISOMTIle> iSOMs = tileSet.GetISOMData(mapeditor);

                                        newMTXM = iSOMs[mapeditor.SelectISOMIndex].GetFlatTile(mapx, mapy, mapeditor.mapdata);
                                        if (newMTXM == ushort.MaxValue) continue;
                                        megaindex = tileSet.GetMegaTileIndex(mapeditor.mapdata.TILETYPE, newMTXM);
                                    }

                                    if (mapeditor.TilePalleteTransparentBlack && megaindex == 0) continue;

                                    int tileindex = mapx + mapy * mapeditor.mapdata.WIDTH;

                                    if (mapeditor.mapdata.CheckTILERange(mapx, mapy))
                                    {
                                        ushort oldMTXM = mapeditor.mapdata.TILE[tileindex];

                                        mapeditor.mapdata.TILEChange(mapx, mapy, newMTXM);
                                        mapeditor.taskManager.TaskAdd(new TileEvent(mapeditor, newMTXM, oldMTXM, mapx, mapy));
                                    }

                                }
                            }


                        }


                        mapeditor.mapdata.TILEChangeComplete();
                    }

                }


                lastpaint.X = MouseTilePos.X;
                lastpaint.Y = MouseTilePos.Y;
            }
            else
            {
                lastpaint.X = -1;
                lastpaint.Y = -1;
            }
        }



    }
}
