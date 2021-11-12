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
using WpfTest.Components;
using static Data.Map.MapData;
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
            if (!key_LeftShiftDown)
            {
                //mapeditor.TileDoodad.Clear();
            }
        }
        private void TileLeftClickEnd()
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
                Point relativePoint = mapeditor.Tile_Rect_Expander.TransformToAncestor(mapeditor.RightExpander).Transform(new Point(0, 0));

                int startX = ((int)(screenwidth));
                int startY = (int)relativePoint.Y;

                _spriteBatch.Draw(gridtexture, new Rectangle((int)(startX), startY, ToolBaStreachValue, (int)48), Back);
            }

            {
                Point relativePoint = mapeditor.Tile_Custom_Expander.TransformToAncestor(mapeditor.RightExpander).Transform(new Point(0, 0));

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
                DrawISOMSet();

                DrawRectSet();

                DrawCustomSet();

                DrawAllTileSet();

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


            _spriteBatch.Begin(blendState: BlendState.NonPremultiplied, samplerState: SamplerState.PointClamp);
            for (int y = 0; y < columns; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if(isomindex >= iSOMs.Count)
                    {
                        break;
                    }

                    TileSet.ISOMTIle iSOM = iSOMs[isomindex];

                    Texture2D texture2D1 = tileSet.GetTile(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, iSOM.group1, 0);
                    Texture2D texture2D2 = tileSet.GetTile(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, iSOM.group2, 0);

                    if (texture2D1 != null)
                    {
                        _spriteBatch.Draw(texture2D1, new Vector2(startX + x * tilesize * 2, startY + y * tilesize), null, Color.White, 0, Vector2.Zero, drawtilesize, SpriteEffects.None, 0);
                    }
                    if (texture2D2 != null)
                    {
                        _spriteBatch.Draw(texture2D2, new Vector2(startX + (2 * x + 1) * tilesize, startY + y * tilesize), null, Color.White, 0, Vector2.Zero, drawtilesize, SpriteEffects.None, 0);
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
                DrawLine(_spriteBatch, new Vector2(startX, startY + y * tilesize), new Vector2(startX + 8 * tilesize, startY + y * tilesize), Color.Black);
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

        private void DrawRectSet()
        {
            Point relativePoint = mapeditor.Tile_Rect_Pallet.TransformToAncestor(mapeditor.RightExpander).Transform(new Point(0, 0));

            int startX = ((int)(screenwidth));
            int startY = (int)relativePoint.Y;


            List<TileSet.ISOMTIle> iSOMs = tileSet.GetISOMData(mapeditor);

            int columns = iSOMs.Count / 16 + 1;


            _spriteBatch.Begin();
            for (int y = 0; y < columns; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    _spriteBatch.Draw(gridtexture, new Vector2(startX + x * tilesize, startY + y * tilesize), null, Color.Red, 0, Vector2.Zero, tilesize, SpriteEffects.None, 0);

                }
            }
            _spriteBatch.End();
        }

        private void DrawCustomSet()
        {
            Point relativePoint = mapeditor.Tile_Custom_Pallet.TransformToAncestor(mapeditor.RightExpander).Transform(new Point(0, 0));

            int startX = ((int)(screenwidth));
            int startY = (int)relativePoint.Y;


            List<TileSet.ISOMTIle> iSOMs = tileSet.GetISOMData(mapeditor);

            int columns = iSOMs.Count / 16 + 1;


            _spriteBatch.Begin();
            for (int y = 0; y < columns; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    _spriteBatch.Draw(gridtexture, new Vector2(startX + x * tilesize, startY + y * tilesize), null, Color.Red, 0, Vector2.Zero, tilesize, SpriteEffects.None, 0);

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
            int startY = (int)relativePoint.Y - mapeditor.brush_tilescroll % 32;

            int tiley = mapeditor.brush_tilescroll / 32;


            _spriteBatch.Begin();
            for (int y = 0; y < yitemcount; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    switch (mapeditor.opt_drawType)
                    {
                        case Control.MapEditor.DrawType.SD:
                            {
                                Texture2D texture2D = tileSet.GetTile(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, (ushort)(y + tiley), (ushort)x);
                                if (texture2D != null)
                                {
                                    _spriteBatch.Draw(texture2D, new Vector2(startX + x * tilesize, startY + y * tilesize), null, Color.White, 0, Vector2.Zero, tilesize / 32, SpriteEffects.None, 0);
                                }
                            }
                            break;
                        case Control.MapEditor.DrawType.HD:
                        case Control.MapEditor.DrawType.CB:
                            {
                                Texture2D texture2D = tileSet.GetTile(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, (ushort)(y + tiley), (ushort)x);
                                if (texture2D != null)
                                {
                                    _spriteBatch.Draw(texture2D, new Vector2(startX + x * tilesize, startY + y * tilesize), null, Color.White, 0, Vector2.Zero, tilesize / 64, SpriteEffects.None, 0);
                                }
                            }
                            break;
                    }
                }
            }

            _spriteBatch.End();
        }




        private void TilePaint()
        {
            //if (mapeditor.mapDataBinding.TILE_SELECTMODE)
            //{
            //    return;
            //}


            if (mouse_LeftDown)
            {
                if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.SELECTION)
                {

                }
                else
                {

                }
            }
        }



    }
}
