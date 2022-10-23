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
        private void DoodadTaskStart()
        {
            mapeditor.taskManager.TaskStart();
            if (!key_LeftShiftDown)
            {
                mapeditor.SelectDoodad.Clear();
            }
        }
        private void DoodadDragEnd()
        {
            if (!mouse_IsLeftDrag)
            {
                DoodadRightMouseClick();
                return;
            }

            if (mapeditor.mapDataBinding.DOODAD_BRUSHMODE)
            {
                LastCreatePos = new Vector2(-100);
                mapeditor.taskManager.TaskEnd();
            }
            else
            {
                //선택모드
                if (!key_LeftShiftDown)
                {
                    mapeditor.SelectDoodad.Clear();
                }
                mapeditor.SelectDoodad.AddRange(hoverDoodad);
            }
        }


        private void DoodadClickEnd()
        {
            if (mapeditor.mapDataBinding.DOODAD_BRUSHMODE)
            {
                LastCreatePos = new Vector2(-100);
                mapeditor.taskManager.TaskEnd();
            }
            else
            {
                mapeditor.SelectDoodad.Clear();
            }
        }



        private void DoodadRightMouseClick()
        {
            if (mapeditor.mapDataBinding.DOODAD_BRUSHMODE)
            {
                mapeditor.mapDataBinding.DOODAD_SELECTMODE = true;
            }
            else
            {
                //메뉴 열기
                mapeditor.OpenDoodadMenu((int)MousePos.X, (int)MousePos.Y);
            }
        }





        private void DrawTile(Vector2 start, int g, int i, double doodadindex)
        {
            AtlasTileSet atlasTileSet = tileSet.GetAtlasTileSetTexture(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE);
            switch (mapeditor.opt_drawType)
            {
                case Control.MapEditor.DrawType.SD:
                    {
                        int magaindex = tileSet.GetMegaTileIndex(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, (ushort)g, (ushort)i);
                        if (atlasTileSet != null && atlasTileSet.texture2D != null)
                        {
                            _spriteBatch.Draw(atlasTileSet.texture2D, start, atlasTileSet.GetRect(magaindex), Color.White, 0, Vector2.Zero, (float)(doodadindex / 32), SpriteEffects.None, 0);
                        }
                    }
                    break;
                case Control.MapEditor.DrawType.HD:
                case Control.MapEditor.DrawType.CB:
                    {
                        int magaindex = tileSet.GetMegaTileIndex(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, (ushort)g, (ushort)i);
                        if (atlasTileSet != null && atlasTileSet.texture2D != null)
                        {
                            _spriteBatch.Draw(atlasTileSet.texture2D, start, atlasTileSet.GetRect(magaindex), Color.White, 0, Vector2.Zero, (float)(doodadindex / 64), SpriteEffects.None, 0);
                        }
                    }
                    break;
            }
        }



        private int DoodadPallectSelectGroup;
        //private int DoodadPallectSelectIndex;
        private void DrawDoodadItem(Vector2 start, Vector2 size, ushort id)
        {
            TileSet.DoodadPallet doodadPallet = tileSet.DoodadPallets[mapeditor.mapdata.TILETYPE][id];

            //_spriteBatch.Draw(gridtexture, new Rectangle((int)start.X, (int)start.Y, (int)size.X, (int)size.Y), Color.LightBlue);
            _spriteBatch.Draw(gridtexture, new Rectangle((int)start.X - 1, (int)start.Y - 1, (int)size.X - 1, (int)size.Y - 1), Color.Black);

            int group = doodadPallet.dddGroup;

            double blocksize = size.X / Math.Max(doodadPallet.dddWidth, doodadPallet.dddHeight);
            //blocksize = Math.Min(32, blocksize);

            Vector2 DrawPos = start;
            DrawPos += new Vector2((float)(size.X - blocksize * doodadPallet.dddWidth) / 2, (float)(size.Y - blocksize * doodadPallet.dddHeight) / 2);
            for (int y = 0; y < doodadPallet.dddHeight; y++)
            {
                for (int x = 0; x < doodadPallet.dddWidth; x++)
                {
                    Vector2 ipos = new Vector2((float)(x * blocksize), (float)(y * blocksize));

                    DrawTile(DrawPos + ipos, group + y, x, blocksize);
                }
            }

            if (start.X < MousePos.X & MousePos.X < start.X + size.X)
            {
                if (start.Y < MousePos.Y & MousePos.Y < start.Y + size.Y)
                {
                    _spriteBatch.Draw(gridtexture, new Rectangle((int)start.X, (int)start.Y, (int)size.X, (int)size.Y), new Color(128, 128, 128, 32));
                    if (mapeditor.DoodadListBoxMouseDown)
                    {
                        DoodadPallectSelectGroup = (ushort)mapeditor.doodad_group;
                        mapeditor.doodad_index = id;
                    }
                }
            }
            if (DoodadPallectSelectGroup == mapeditor.doodad_group & mapeditor.doodad_index == id)
            {
                _spriteBatch.Draw(gridtexture, new Rectangle((int)start.X, (int)start.Y, (int)size.X, (int)size.Y), new Color(128, 128, 128, 64));
            }


            //_spriteBatch.DrawString(_font, doodadPallet.tblString, start, Color.White);
        }


        private void DrawDoodadPallet()
        {
            if (mapeditor.PalleteLayer != Control.MapEditor.Layer.Doodad)
            {
                return;
            }

            int startxpos = 128;
            int startypos = 100 - mapeditor.DoodadScroll;



            int xitemcount = 6;
            int yitemcount = 6;
            int itemsize = (int)((mapeditor.opt_palletSize - 128) / 6);

            



            if (mapeditor.DoodadPalleteSizeUp)
            {
                xitemcount /= 2;
                itemsize *= 2;
            }



            _spriteBatch.Begin(blendState: BlendState.NonPremultiplied, samplerState: SamplerState.PointClamp);

            ushort doodadgroup = (ushort)mapeditor.doodad_group;

            ushort doodadindex = 0;
            List<int> tlist = tileSet.DoodadGroups[mapeditor.mapdata.TILETYPE][doodadgroup].dddids;

            yitemcount = (tlist.Count / xitemcount) + 1;






            for (ushort y = 0; y < yitemcount; y++)
            {
                for (ushort x = 0; x < xitemcount; x++)
                {
                    ushort index = (ushort)(x + y * xitemcount + doodadindex);
                    if (tlist.Count > index)
                    {
                        int _tindex = tlist[index];

                        DrawDoodadItem(new Vector2(screenwidth + startxpos + x * itemsize, startypos + y * itemsize), new Vector2(itemsize, itemsize), (ushort)_tindex);
                    }
                }
            }

            _spriteBatch.End();
        }




        private void DoodadPaint()
        {
            if (mapeditor.mapDataBinding.DOODAD_SELECTMODE)
            {
                return;
            }

            if (mouse_LeftDown)
            {
                if (mapeditor.doodad_PasteMode)
                {
                    int gridsize = 32;

                    Vector2 mappos = MouseMapPos;

                    //생성모드
                    if ((LastCreatePos - mappos).Length() >= Math.Max(gridsize, 4))
                    {
                        LastCreatePos = mappos;

                        for (int i = 0; i < mapeditor.CopyedDoodad.Count; i++)
                        {
                            mappos = MouseMapPos;

                            mappos.X = (float)(Math.Round(mappos.X / 32) * 32);
                            mappos.Y = (float)(Math.Round(mappos.Y / 32) * 32);


                            int doodadid = mapeditor.CopyedDoodad[i].ID;
                            var t = tileSet.DoodadPallets[mapeditor.mapdata.TILETYPE];
                            DoodadPallet pallete = t[(ushort)doodadid];


                            CDD2 _cDD2 = mapeditor.CopyedDoodad[i];

                            //if (pallete.dddHeight % 2 == 1)
                            //{
                            //    mappos.Y -= 16;
                            //}

                            mappos = Tools.VectorTool.Max(mappos, new Vector2(0));
                            mappos = Tools.VectorTool.Min(mappos, new Vector2(mapeditor.mapdata.WIDTH * 32, mapeditor.mapdata.HEIGHT * 32));

                            mappos.X = (float)Math.Floor(mappos.X);
                            mappos.Y = (float)Math.Floor(mappos.Y);


                            mappos.X += (float)_cDD2.X;
                            mappos.Y += (float)_cDD2.Y;




                            if (DoodadCollsionCheck(mappos, pallete))
                            {
                                LastCreatePos = mappos;

                                CDD2 cDD2 = new CDD2(mapeditor.mapdata);
                                cDD2.X = (ushort)mappos.X;
                                cDD2.Y = (ushort)mappos.Y;
                                cDD2.ID = (ushort)doodadid;
                                cDD2.PLAYER = (byte)_cDD2.PLAYER;

                                mapeditor.mapdata.DD2.Add(cDD2);
                                cDD2.ImageReset();

                                mapeditor.mapdata.DD2ToMTXM(cDD2);
                                mapeditor.taskManager.TaskAdd(new DoodadEvent(mapeditor, cDD2, true));
                            }
                        }

                    }

                    
                }
                else
                {
                    int gridsize = 32;
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


                    Vector2 mappos = MouseMapPos;


                    mappos.X = (float)(Math.Round(mappos.X / 32) * 32);
                    mappos.Y = (float)(Math.Round(mappos.Y / 32) * 32);
                    if (pallete.dddHeight % 2 == 1)
                    {
                        mappos.Y -= 16;
                    }



                    mappos = Tools.VectorTool.Max(mappos, new Vector2(0));
                    mappos = Tools.VectorTool.Min(mappos, new Vector2(mapeditor.mapdata.WIDTH * 32, mapeditor.mapdata.HEIGHT * 32));

                    mappos.X = (float)Math.Floor(mappos.X);
                    mappos.Y = (float)Math.Floor(mappos.Y);


                    //생성모드
                    if ((LastCreatePos - mappos).Length() >= Math.Max(gridsize, 4))
                    {

                        if (DoodadCollsionCheck(mappos, pallete))
                        {
                            LastCreatePos = mappos;

                            CDD2 cDD2 = new CDD2(mapeditor.mapdata);
                            cDD2.X = (ushort)mappos.X;
                            cDD2.Y = (ushort)mappos.Y;
                            cDD2.ID = (ushort)doodadid;
                            cDD2.PLAYER = (byte)playerid;

                            mapeditor.mapdata.DD2.Add(cDD2);
                            cDD2.ImageReset();

                            mapeditor.mapdata.DD2ToMTXM(cDD2);
                            mapeditor.taskManager.TaskAdd(new DoodadEvent(mapeditor, cDD2, true));
                        }
                    }
                }
            }
        }






        private bool DoodadCollsionCheck(Vector2 pos, DoodadPallet paldoodad)
        {
            if (mapeditor.mapDataBinding.DOODAD_STACKALLOW)
            {
                return true;
            }

            int x = (ushort)pos.X / 32 * 32 - (paldoodad.dddWidth / 2) * 32;
            int y = (ushort)pos.Y / 32 * 32 - (paldoodad.dddHeight / 2) * 32;

            Rect rect = new Rect(new Point(x, y), new Point(x + paldoodad.dddWidth * 32, y + paldoodad.dddHeight * 32));

            for (int i = 0; i < mapeditor.mapdata.DD2.Count; i++)
            {
                CDD2 mcDD2 = mapeditor.mapdata.DD2[i];

                int doodadid = mcDD2.ID;
                var t = tileSet.DoodadPallets[mapeditor.mapdata.TILETYPE];
                DoodadPallet mapdoodad = t[(ushort)doodadid];

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
                            int my = (int)(interRect.Y - ty ) / 32;
                            int px = (int)(interRect.X - x) / 32;
                            int py = (int)(interRect.Y - y) / 32;


                            ushort mg = (ushort)(mapdoodad.dddGroup + my + iy);
                            ushort mi = (ushort)(mx + ix);
                            ushort pg = (ushort)(paldoodad.dddGroup + py + iy);
                            ushort pi = (ushort)(px + ix);



                            if (!tileSet.IsBlack(mapeditor.mapdata.TILETYPE, mg, mi) & !tileSet.IsBlack(mapeditor.mapdata.TILETYPE, pg, pi))
                            {
                                return false;
                            }
                        }
                    }

                }

            }





            return true;
        }



    }
}
