using Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using SpriteFontPlus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using UseMapEditor.FileData;
using WpfTest.Components;

namespace UseMapEditor.MonoGameControl
{
    public partial class MapDrawer : WpfGame
    {
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
                DrawLine(_spriteBatch, new Vector2(xi, StartPoint.Y), new Vector2(xi, EndPoint.Y), new Color(0xAAF361A6));
            }
            for (float yi = StartPoint.Y; yi < EndPoint.Y; yi += mag)
            {
                DrawLine(_spriteBatch, new Vector2(StartPoint.X, yi), new Vector2(EndPoint.X, yi), new Color(0xAAF361A6));
            }

            _spriteBatch.End();
            return;
        }


        private void TileDraw(bool IsDrawGrp)
        {
            float width = (float)this.ActualWidth;
            float height = (float)this.ActualHeight;




            Vector2 MapMin = mapeditor.PosMapToScreen(new Vector2(0, 0));
            Vector2 MapMax = mapeditor.PosMapToScreen(new Vector2(mapeditor.mapdata.WIDTH, mapeditor.mapdata.HEIGHT) * 32);
            Vector2 MapSize = MapMax - MapMin;


            _spriteBatch.Begin();
            _spriteBatch.Draw(gridtexture, new Rectangle((int)MapMin.X, (int)MapMin.Y, (int)MapSize.X, (int)MapSize.Y), Color.Black);
            _spriteBatch.End();




            int startxti = (int)(mapeditor.opt_xpos / 32d);
            int startyti = (int)(mapeditor.opt_ypos / 32d);

            float mag = (float)(32 * mapeditor.opt_scalepercent);


            

            float startx = (float)-((mapeditor.opt_xpos % 32) * mapeditor.opt_scalepercent);
            float starty = (float)-((mapeditor.opt_ypos % 32) * mapeditor.opt_scalepercent);



            int tileindex = 0;

            int cxti = startxti;
            int cyti = startyti;


            _spriteBatch.Begin( samplerState: SamplerState.PointClamp);
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
                    ushort MTXM = mapeditor.mapdata.MTXM[tileindex];
                    if (IsDrawGrp)
                    {

                        switch (mapeditor.opt_drawType)
                        {
                            case Control.MapEditor.DrawType.SD:
                                {
                                    Texture2D texture2D = tileSet.GetTile(mapeditor.mapdata.TILETYPE, SDTileSet, MTXM);
                                    _spriteBatch.Draw(texture2D, new Vector2(xi, yi), null, Color.White, 0, Vector2.Zero, (float)mapeditor.opt_scalepercent, SpriteEffects.None, 1);
                                }
                                break;
                            case Control.MapEditor.DrawType.HD:
                                {
                                    Texture2D texture2D = tileSet.GetTile(mapeditor.mapdata.TILETYPE, HDTileSet, MTXM);
                                    _spriteBatch.Draw(texture2D, new Vector2(xi, yi), null, Color.White, 0, Vector2.Zero, (float)mapeditor.opt_scalepercent / 2, SpriteEffects.None, 1);
                                }
                                break;
                            case Control.MapEditor.DrawType.CB:
                                {
                                    Texture2D texture2D = tileSet.GetTile(mapeditor.mapdata.TILETYPE, CBTileSet, MTXM);
                                    _spriteBatch.Draw(texture2D, new Vector2(xi, yi), null, Color.White, 0, Vector2.Zero, (float)mapeditor.opt_scalepercent / 2, SpriteEffects.None, 1);
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
    }
}
