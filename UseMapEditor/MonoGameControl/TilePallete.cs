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
using UseMapEditor.Control;
using UseMapEditor.FileData;
using UseMapEditor.Task.Events;
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

            if (mapeditor.mapDataBinding.TILE_BRUSHMODE != Control.MapEditor.TileSetBrushMode.SELECTION)
            {
                //브러시 모드일 경우
                LastCreatePos = new Vector2(-100);
                mapeditor.taskManager.TaskEnd();
            }
            else
            {
                //선택모드
                if (!key_LeftShiftDown)
                {
                    //mapeditor.SelectDoodad.Clear();
                }
                //mapeditor.SelectDoodad.AddRange(hoverDoodad);
            }
        }


        private void TileRightMouseClick()
        {
            if (mapeditor.mapDataBinding.TILE_BRUSHMODE != Control.MapEditor.TileSetBrushMode.SELECTION)
            {
                mapeditor.mapDataBinding.TILE_BRUSHMODE = Control.MapEditor.TileSetBrushMode.SELECTION;
            }
            else
            {
                //메뉴 열기
                //mapeditor.OpenDoodadMenu((int)MousePos.X, (int)MousePos.Y);
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

                _spriteBatch.Draw(gridtexture, new Rectangle((int)(startX), startY, ToolBaStreachValue, (int)48), Back);
            }

            {
                Point relativePoint = mapeditor.Tile_All_Label.TransformToAncestor(mapeditor.RightExpander).Transform(new Point(0, 0));

                int startX = ((int)(screenwidth));
                int startY = (int)relativePoint.Y;

                _spriteBatch.Draw(gridtexture, new Rectangle((int)(startX), startY, ToolBaStreachValue, (int)48), Back);
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


            List<TileSet.ISOMTIle> iSOMs = tileSet.GetISOMData(mapeditor);

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

                    TileSet.ISOMTIle iSOM = iSOMs[isomindex];

                    int megaindex1 = tileSet.GetMegaTileIndex(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, iSOM.group1, 0);
                    int megaindex2 = tileSet.GetMegaTileIndex(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, iSOM.group2, 0);

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
                        if (mapeditor.TileISOMMouseDown)
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

        //private void DrawRectSet()
        //{
        //    Point relativePoint = mapeditor.Tile_Rect_Pallet.TransformToAncestor(mapeditor.RightExpander).Transform(new Point(0, 0));

        //    int startX = ((int)(screenwidth));
        //    int startY = (int)relativePoint.Y;


        //    List<TileSet.ISOMTIle> iSOMs = tileSet.GetISOMData(mapeditor);

        //    int columns = iSOMs.Count / 16 + 1;


        //    _spriteBatch.Begin();
        //    _spriteBatch.Draw(gridtexture, new Vector2(startX, startY), null, Color.Black, 0, Vector2.Zero, new Vector2(32 * 16, 32), SpriteEffects.None, 0);
        //    for (int y = 0; y < columns; y++)
        //    {
        //        for (int x = 0; x < 16; x++)
        //        {
        //            _spriteBatch.Draw(gridtexture, new Vector2(startX + x * tilesize, startY + y * tilesize), null, Color.Red, 0, Vector2.Zero, tilesize, SpriteEffects.None, 0);
        //        }
        //    }
        //    _spriteBatch.End();
        //}

        //private void DrawCustomSet()
        //{
        //    Point relativePoint = mapeditor.Tile_Custom_Pallet.TransformToAncestor(mapeditor.RightExpander).Transform(new Point(0, 0));

        //    int startX = ((int)(screenwidth));
        //    int startY = (int)relativePoint.Y;


        //    List<TileSet.ISOMTIle> iSOMs = tileSet.GetISOMData(mapeditor);

        //    int columns = iSOMs.Count / 16 + 1;


        //    _spriteBatch.Begin();
        //    _spriteBatch.Draw(gridtexture, new Vector2(startX, startY), null, Color.Black, 0, Vector2.Zero, new Vector2(32 * 16, 32), SpriteEffects.None, 0);
        //    for (int y = 0; y < columns; y++)
        //    {
        //        for (int x = 0; x < 16; x++)
        //        {
        //            _spriteBatch.Draw(gridtexture, new Vector2(startX + x * tilesize, startY + y * tilesize), null, Color.Red, 0, Vector2.Zero, tilesize, SpriteEffects.None, 0);

        //        }
        //    }
        //    _spriteBatch.End();
        //}




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
                                int megaindex = tileSet.GetMegaTileIndex(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, (ushort)(y + tiley), (ushort)x);
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
                                int megaindex = tileSet.GetMegaTileIndex(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, (ushort)(y + tiley), (ushort)x);
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
            if (mapeditor.tile_SelectALLTILEStartXIndex != -1 && mapeditor.tile_SelectALLTILEStartYIndex != -1 && mapeditor.tile_SelectALLTILEEndXIndex != -1 && mapeditor.tile_SelectALLTILEEndYIndex != -1)
            {
                int xstart = Math.Min(mapeditor.tile_SelectALLTILEStartXIndex, mapeditor.tile_SelectALLTILEEndXIndex);
                int ystart = Math.Min(mapeditor.tile_SelectALLTILEStartYIndex, mapeditor.tile_SelectALLTILEEndYIndex);
                int xend = Math.Max(mapeditor.tile_SelectALLTILEStartXIndex, mapeditor.tile_SelectALLTILEEndXIndex);
                int yend = Math.Max(mapeditor.tile_SelectALLTILEStartYIndex, mapeditor.tile_SelectALLTILEEndYIndex);


                int w = xend - xstart + 1;
                int h = yend - ystart + 1;


                float drawX = startX + xstart * tilesize;
                float drawY = startY + ystart * tilesize - tiley * tilesize;


                _spriteBatch.Draw(gridtexture, new Vector2(drawX, drawY), null, new Color(128, 128, 192, 64), 0, Vector2.Zero, new Vector2(tilesize * w, tilesize * h), SpriteEffects.None, 0);
            }

            //호버링
            if (mapeditor.TileAllMouseDown)
            {
                int xstart = Math.Min(mapeditor.TileMouseStartXIndex, mapeditor.TileMouseMoveXIndex);
                int ystart = Math.Min(mapeditor.TileMouseStartYIndex, mapeditor.TileMouseMoveYIndex);
                int xend = Math.Max(mapeditor.TileMouseStartXIndex, mapeditor.TileMouseMoveXIndex);
                int yend = Math.Max(mapeditor.TileMouseStartYIndex, mapeditor.TileMouseMoveYIndex);

                int w = xend - xstart + 1;
                int h = yend - ystart + 1;

                _spriteBatch.Draw(gridtexture, new Vector2(startX + xstart * tilesize, startY + ystart * tilesize - tiley * tilesize), null, new Color(128, 128, 128, 64), 0, Vector2.Zero, new Vector2(tilesize * w, tilesize * h), SpriteEffects.None, 0);
            }


            _spriteBatch.End();
        }




        private void TilePaint()
        {

            //TODO:타일 넣기
            //if (mapeditor.mapDataBinding.TILE_SELECTMODE)
            //{
            //    return;
            //}


            if (mouse_LeftDown)
            {
                if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.PASTE)
                {
                    //붙여넣기
                }
                else if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.ALLTILE)
                {
                    if (mapeditor.TilePalletePencil)
                    {
                        Vector2 mappos = MouseTilePos;


                        int width = (int)(mapeditor.tile_PalleteSelectEnd.X - mapeditor.tile_PalleteSelectStart.X) + 1;
                        int height = (int)(mapeditor.tile_PalleteSelectEnd.Y - mapeditor.tile_PalleteSelectStart.Y) + 1;

                        int sx = (int)Math.Floor(width / 2.0);
                        int sy = (int)Math.Floor(height / 2.0);


                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                ushort group = (ushort)(mapeditor.tile_PalleteSelectStart.Y + y);
                                ushort index = (ushort)(mapeditor.tile_PalleteSelectStart.X + x);


                                int megaindex = tileSet.GetMegaTileIndex(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, group, index);

                                if (mapeditor.TilePalleteTransparentBlack && megaindex == 0) continue;


                                int mapx = (ushort)mappos.X + (x - sx);
                                int mapy = (ushort)mappos.Y + (y - sy);


                                int tileindex = mapx + mapy * mapeditor.mapdata.WIDTH;

                                ushort newMTXM = (ushort)(index + (group << 4));
                                ushort oldMTXM = mapeditor.mapdata.TILE[tileindex];

                                mapeditor.mapdata.TILE[tileindex] = newMTXM;

                                mapeditor.taskManager.TaskAdd(new TileEvent(mapeditor, newMTXM, oldMTXM, mapx, mapy));
                            }
                        }
                    }
                    else if (mapeditor.TilePalleteRect)
                    {

                    }
                    
                }
                else if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.ISOM)
                {

                }
            }
        }



    }
}
