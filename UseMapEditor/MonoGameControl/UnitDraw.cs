using Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using SpriteFontPlus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using UseMapEditor.FileData;
using WpfTest.Components;
using static Data.Map.MapData;

namespace UseMapEditor.MonoGameControl
{
    public partial class MapDrawer : WpfGame
    {
        private List<CUNIT> hoverUnit = new List<CUNIT>();
        private void RenderUnit(bool IsDrawGrp)
        {
            hoverUnit.Clear();
            for (int i = 0; i < mapeditor.mapdata.UNIT.Count; i++)
            {
                DrawUnit(mapeditor.mapdata.UNIT[i]);
            }
        }

        private List<CTHG2> hoverSprite = new List<CTHG2>();
        private void RenderSprite(bool IsDrawGrp)
        {
            hoverSprite.Clear();
            for (int i = 0; i < mapeditor.mapdata.THG2.Count; i++)
            {
                DrawSprite(mapeditor.mapdata.THG2[i]);
            }
        }

        private void DrawSprite(CTHG2 cTHG2)
        {
            int _x = cTHG2.X;
            int _y = cTHG2.Y;
            Vector2 screen = mapeditor.PosMapToScreen(new Vector2(_x, _y));

            bool IsSelect = false;
            bool IsHover = false;
            if(mapeditor.PalleteLayer == Control.MapEditor.Layer.Sprite)
            {
                if (mapeditor.mapDataBinding.SPRITE_SELECTMODE)
                {
                    if (mouse_IsDrag)
                    {
                        //선택모드
                        Vector2 min = new Vector2(Math.Min(mouse_DragMapStart.X, MouseMapPos.X), Math.Min(mouse_DragMapStart.Y, MouseMapPos.Y));
                        Vector2 max = new Vector2(Math.Max(mouse_DragMapStart.X, MouseMapPos.X), Math.Max(mouse_DragMapStart.Y, MouseMapPos.Y));

                        if (min.X - 8 < _x & _x < max.X + 8)
                        {
                            if (min.Y - 8 < _y & _y < max.Y + 8)
                            {
                                hoverSprite.Add(cTHG2);
                            }
                        }
                    }
                    if (mapeditor.SelectSprite.Contains(cTHG2))
                    {
                        IsSelect = true;
                    }
                    else if (hoverSprite.Contains(cTHG2))
                    {
                        IsHover = true;
                    }
                }
            }

            int objID = cTHG2.ID;

            int objwidth = (int)(cTHG2.BoxWidth * mapeditor.opt_scalepercent);
            int objheight = (int)(cTHG2.BoxHeight * mapeditor.opt_scalepercent);

            float minX = 0 - objwidth;
            float minY = 0 - objheight;
            float maxX = screenwidth + objwidth;
            float maxY = screenheight + objheight;

            if ((minX < screen.X) & (screen.X < maxX))
            {
                if ((minY < screen.Y) & (screen.Y < maxY))
                {
                    if (cTHG2.Images.Count == 0)
                    {
                        cTHG2.ImageReset();
                    }

                    for (int i = 0; i < cTHG2.Images.Count; i++)
                    {
                        cTHG2.Images[i].IsSelect = false;
                        cTHG2.Images[i].IsHover = false;
                        if(i == 0)
                        {
                            if (IsSelect)
                            {
                                cTHG2.Images[i].IsSelect = true;
                            }
                            else if (IsHover)
                            {
                                cTHG2.Images[i].IsHover = true;
                            }
                        }
                        cTHG2.Images[i].color = cTHG2.PLAYER;
                        cTHG2.Images[i].screen = screen;
                        ImageList.Add(cTHG2.Images[i]);
                        cTHG2.Images[i].PlayScript();
                    }
                }
            }
        }


        List<Vector4> Lines = new List<Vector4>();
        private void DrawConnect()
        {
            _spriteBatch.Begin();
            for (int i = 0; i < Lines.Count; i++)
            {
                Vector2 s = new Vector2(Lines[i].X, Lines[i].Y);
                Vector2 e = new Vector2(Lines[i].Z, Lines[i].W);

                DrawLine(_spriteBatch, s, e, Color.Lime, 2);
            }
            _spriteBatch.End();


            Lines.Clear();
        }


        private void DrawUnit(CUNIT cUNIT, List<CImage> templist = null, int x = -1, int y = -1, bool AlwaysDraw = false)
        {
            int _x = cUNIT.X;
            int _y = cUNIT.Y;

            if (x != -1)
            {
                _x = x;
            }
            if (y != -1)
            {
                _y = y;
            }


            Vector2 screen = mapeditor.PosMapToScreen(new Vector2(_x, _y));
            if (cUNIT.linkFlag != 0 & cUNIT.linkedUnit != 0)
            {
                CUNIT c = mapeditor.mapdata.UNIT.SingleOrDefault((tx) => tx.unitclass == cUNIT.linkedUnit);
                if(c != null)
                {
                    Vector2 _ = mapeditor.PosMapToScreen(new Vector2(c.X, c.Y));
                    Lines.Add(new Vector4(screen.X, screen.Y, _.X, _.Y));
                }
            }


            bool HoverDisenable = false;
            if (cUNIT.unitID == 214 & !mapeditor.view_Unit_StartLoc)
            {
                //스타트로케이션
                HoverDisenable = true;
            }
            else if (cUNIT.unitID == 101 & !mapeditor.view_Unit_Maprevealer)
            {
                //맵리빌러
                HoverDisenable = true;
            }


            bool IsSelect = false;
            bool IsHover = false;
            if (!HoverDisenable)
            {
                if (mapeditor.PalleteLayer == Control.MapEditor.Layer.Unit)
                {
                    if (mapeditor.mapDataBinding.UNIT_SELECTMODE)
                    {
                        if (mouse_IsDrag)
                        {
                            //선택모드
                            Vector2 min = new Vector2(Math.Min(mouse_DragMapStart.X, MouseMapPos.X), Math.Min(mouse_DragMapStart.Y, MouseMapPos.Y));
                            Vector2 max = new Vector2(Math.Max(mouse_DragMapStart.X, MouseMapPos.X), Math.Max(mouse_DragMapStart.Y, MouseMapPos.Y));



                            if (((min.X - 8 < _x & _x < max.X + 8) & (min.Y - 8 < _y & _y < max.Y + 8)) )
                            {
                                hoverUnit.Add(cUNIT);
                            }
                        }
                        if (mapeditor.SelectUnit.Contains(cUNIT))
                        {
                            IsSelect = true;
                        }
                        else if (hoverUnit.Contains(cUNIT))
                        {
                            IsHover = true;
                        }
                    }
                }
            }


            int grpwidth = (int)(cUNIT.BoxWidth * mapeditor.opt_scalepercent);
            int grpheight = (int)(cUNIT.BoxHeight * mapeditor.opt_scalepercent);



            float minX = 0 - grpwidth;
            float minY = 0 - grpheight;
            float maxX = screenwidth + grpwidth;
            float maxY = screenheight + grpheight;


            
            if (((minX < screen.X) & (screen.X < maxX) & (minY < screen.Y) & (screen.Y < maxY)) | AlwaysDraw)
            {
                if (cUNIT.Images.Count == 0)
                {
                    cUNIT.ImageReset();
                }
                for (int i = 0; i < cUNIT.Images.Count; i++)
                {
                    cUNIT.Images[i].sortvalue = (int)cUNIT.unitclass;

                    cUNIT.Images[i].IsSelect = false;
                    cUNIT.Images[i].IsHover = false;
                    cUNIT.Images[i].IsUnitRect = false;
                    if (i == 0)
                    {
                        if (IsSelect)
                        {
                            cUNIT.Images[i].IsUnitRect = true;
                            cUNIT.Images[i].IsSelect = true;
                        }
                        else if (IsHover)
                        {
                            cUNIT.Images[i].IsUnitRect = true;
                            cUNIT.Images[i].IsHover = true;
                        }
                    }
                    cUNIT.Images[i].Left = (byte)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Unit Size Left", cUNIT.unitID).Data;
                    cUNIT.Images[i].Up = (byte)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Unit Size Up", cUNIT.unitID).Data;
                    cUNIT.Images[i].Right = (byte)(Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Unit Size Right", cUNIT.unitID).Data + 1);
                    cUNIT.Images[i].Down = (byte)(Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Unit Size Down", cUNIT.unitID).Data + 1);



                    cUNIT.Images[i].color = cUNIT.player;
                    cUNIT.Images[i].screen = screen;
                    if (templist != null)
                    {
                        templist.Add(cUNIT.Images[i]);
                    }
                    else
                    {
                        ImageList.Add(cUNIT.Images[i]);
                    }
                    cUNIT.Images[i].PlayScript();
                }



                ////UnitZPos
                //_spriteBatch.Begin();
                //DrawLine(_spriteBatch, new Vector2(screen.X - 20, screen.Y), new Vector2(screen.X + 20, screen.Y), Color.Red);
                //DrawLine(_spriteBatch, new Vector2(screen.X, screen.Y - 20), new Vector2(screen.X, screen.Y + 20), Color.Red);

                //_spriteBatch.End();


            }
        }


        private List<CImage> ImageList = new List<CImage>();

    }
}
