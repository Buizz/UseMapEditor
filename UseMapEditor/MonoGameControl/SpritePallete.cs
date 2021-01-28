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
        private void SpriteTaskStart()
        {
            mapeditor.taskManager.TaskStart();
            if (!key_LeftShiftDown)
            {
                mapeditor.SelectSprite.Clear();
            }
        }
        private void SpriteDragEnd()
        {
            if (!mouse_IsLeftDrag)
            {
                SpriteRightMouseClick();
                return;
            }

            if (mapeditor.mapDataBinding.SPRITE_BRUSHMODE)
            {
                LastCreatePos = new Vector2(-100);
                mapeditor.taskManager.TaskEnd();
            }
            else
            {
                //선택모드
                if (!key_LeftShiftDown)
                {
                    mapeditor.SelectSprite.Clear();
                }
                mapeditor.SelectSprite.AddRange(hoverSprite);
            }
        }


        private void SpriteClickEnd()
        {
            if (mapeditor.mapDataBinding.SPRITE_BRUSHMODE)
            {
                LastCreatePos = new Vector2(-100);
                mapeditor.taskManager.TaskEnd();
            }
            else
            {
                mapeditor.SelectSprite.Clear();
            }
        }



        private void SpriteRightMouseClick()
        {
            if(mapeditor.mapDataBinding.SPRITE_BRUSHMODE)
            {
                mapeditor.mapDataBinding.SPRITE_SELECTMODE = true;
            }
            else
            {
                //메뉴 열기
                mapeditor.OpenSpriteMenu((int)MousePos.X, (int)MousePos.Y);
            }
        }



        private void SpritePalleteDraw()
        {
            int gridsize = mapeditor.opt_grid;

            if (mapeditor.mapDataBinding.SPRITE_SELECTMODE)
            {
                return;
            }

            if (mouse_LeftDown)
            {
                if (mapeditor.sprite_PasteMode)
                {
                    Vector2 mappos = mapeditor.PosScreenToMap(MousePos);
                    if (mapeditor.SpritePalleteGridFix)
                    {
                        //그리드 픽스
                        if (gridsize != 0)
                        {
                            mappos.X = (float)(Math.Round(mappos.X / gridsize) * gridsize);
                            mappos.Y = (float)(Math.Round(mappos.Y / gridsize) * gridsize);
                        }
                    }
                    if ((LastCreatePos - mappos).Length() >= Math.Max(gridsize, 4))
                    {
                        LastCreatePos = mappos;

                        for (int i = 0; i < mapeditor.CopyedSprite.Count; i++)
                        {
                            CTHG2 csTHG2 = mapeditor.CopyedSprite[i];
                            mappos = MouseMapPos;
                            if (mapeditor.SpritePalleteGridFix)
                            {
                                //그리드 픽스
                                if (gridsize != 0)
                                {
                                    mappos.X = (float)(Math.Round(mappos.X / gridsize) * gridsize);
                                    mappos.Y = (float)(Math.Round(mappos.Y / gridsize) * gridsize);
                                }
                            }

                            mappos += new Vector2((short)csTHG2.X, (short)csTHG2.Y);

                            mappos = Tools.VectorTool.Max(mappos, new Vector2(0));
                            mappos = Tools.VectorTool.Min(mappos, new Vector2(mapeditor.mapdata.WIDTH * 32, mapeditor.mapdata.HEIGHT * 32));


                            CTHG2 cTHG2 = new CTHG2();
                            cTHG2.ID = (ushort)mapeditor.CopyedSprite[i].ID;
                            cTHG2.FLAG = mapeditor.CopyedSprite[i].FLAG;
                            cTHG2.PLAYER = (byte)mapeditor.CopyedSprite[i].PLAYER;
                            cTHG2.X = (ushort)(mappos.X);
                            cTHG2.Y = (ushort)(mappos.Y);


                            mapeditor.mapdata.THG2.Add(cTHG2);
                            cTHG2.ImageReset();

                            mapeditor.taskManager.TaskAdd(new SpriteEvent(mapeditor, cTHG2, true));
                        }
                    }
                }
                else
                {
                    Vector2 mappos = mapeditor.PosScreenToMap(MousePos);
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
                    //생성모드
                    if ((LastCreatePos - mappos).Length() >= Math.Max(gridsize, 4))
                    {
                        LastCreatePos = mappos;

                        CTHG2 cTHG2 = new CTHG2();

                        cTHG2.FLAG = (0b1 << 7) + (0b1 << 9);
                        if (mapeditor.sprite_SpritBrush)
                        {
                            cTHG2.ID = (ushort)mapeditor.SpritePallete.SelectIndex;
                            cTHG2.FLAG += (0b1 << 12);
                        }
                        else
                        {
                            int unitID = mapeditor.SpritePallete_Unit.SelectIndex;
                            cTHG2.ID = (ushort)unitID;
                            cTHG2.FLAG += (0b1 << 13);
                        }
                        cTHG2.PLAYER = (byte)mapeditor.sprite_player;
                        cTHG2.X = (ushort)mappos.X;
                        cTHG2.Y = (ushort)mappos.Y;


                        mapeditor.mapdata.THG2.Add(cTHG2);
                        cTHG2.ImageReset();

                        mapeditor.taskManager.TaskAdd(new SpriteEvent(mapeditor, cTHG2, true));
                    }
                }
            }
            
        }


    }
}
