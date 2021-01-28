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
using UseMapEditor.Task.Events;
using WpfTest.Components;
using static Data.Map.MapData;
using static UseMapEditor.FileData.TileSet;

namespace UseMapEditor.MonoGameControl
{
    public partial class MapDrawer : WpfGame
    {
        private void RenderFogofWar()
        {

            float width = (float)this.ActualWidth;
            float height = (float)this.ActualHeight;



            Vector2 MapMin = mapeditor.PosMapToScreen(new Vector2(0, 0));
            Vector2 MapMax = mapeditor.PosMapToScreen(new Vector2(mapeditor.mapdata.WIDTH, mapeditor.mapdata.HEIGHT) * 32);
            Vector2 MapSize = MapMax - MapMin;

            int startxti = (int)(mapeditor.opt_xpos / 32d);
            int startyti = (int)(mapeditor.opt_ypos / 32d);
            float mag = (float)(32 * mapeditor.opt_scalepercent);
            float startx = (float)-((mapeditor.opt_xpos % 32) * mapeditor.opt_scalepercent);
            float starty = (float)-((mapeditor.opt_ypos % 32) * mapeditor.opt_scalepercent);


            int tileindex = 0;

            int cxti = startxti;
            int cyti = startyti;

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
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
                    byte flag = mapeditor.mapdata.MASK[tileindex];

                    int _opacity = 0;
                    if (mapeditor.opt_FogofWarplayer < 8)
                    {
                        if((flag & (0b1 << mapeditor.opt_FogofWarplayer)) > 0)
                        {
                            _opacity = 128;
                        }
                    }
                    else if(mapeditor.opt_FogofWarplayer <= 12)
                    {
                        List<byte> players = new List<byte>();

                        if(mapeditor.opt_FogofWarplayer != 12)
                        {
                            int force = mapeditor.opt_FogofWarplayer - 8;
                            for (byte i = 0; i < 8; i++)
                            {
                                if (mapeditor.mapdata.FORCE[i] == force)
                                {
                                    players.Add(i);
                                }
                            }
                        }
                        else
                        {
                            for (byte i = 0; i < 8; i++)
                            {
                                players.Add(i);
                            }
                        }



                        for (int i = 0; i < players.Count; i++)
                        {
                            if ((flag & (0b1 << players[i])) > 0)
                            {
                                _opacity += 128 / players.Count;
                            }
                        }
                    }


                    if(_opacity != 0)
                    {
                        _spriteBatch.Draw(gridtexture, new Vector2((float)xi, (float)yi), null, new Color(0, 0, 0, _opacity), 0, new Vector2(), (float)mapeditor.opt_scalepercent * 32, SpriteEffects.None, 0);
                    }
               


                    cxti++;
                }
                cyti++;
            }
            _spriteBatch.End();


            FogofWarPalletePreview();
        }

        private void FogofWarPalleteDraw()
        {
            if (mapeditor.brush_fogofwarcircle)
            {
                int bx = mapeditor.brush_x + 2;
                int by = mapeditor.brush_y + 2;


                int startx = (int)(MouseTilePos.X - Math.Floor(bx / 2d));
                int starty = (int)(MouseTilePos.Y - Math.Floor(by / 2d));
                Vector2 center = new Vector2(startx, starty);
                int a = (int)Math.Floor(bx / 2d);
                int b = (int)Math.Floor(by / 2d);


                for (int y = 0; y < by; y++)
                {
                    for (int x = 0; x < bx; x++)
                    {
                        int cxti = startx + x;
                        int cyti = starty + y;

                        if (cxti < 0 || cyti < 0)
                        {
                            continue;
                        }
                        if (cxti >= mapeditor.mapdata.WIDTH || cyti >= mapeditor.mapdata.HEIGHT)
                        {
                            continue;
                        }

                        int r = Math.Max(bx, by) / 2;


                        bool check;
                        if (bx > by)
                        {
                            check = (Math.Pow(x - a, 2) + Math.Pow((y - b) * ((double)bx / by), 2)) >= Math.Pow(r, 2);
                        }
                        else if (bx < by)
                        {
                            check = (Math.Pow((x - a) * ((double)by / bx), 2) + Math.Pow(y - b, 2)) >= Math.Pow(r, 2);
                        }
                        else
                        {
                            check = (Math.Pow(x - a, 2) + Math.Pow(y - b, 2)) >= Math.Pow(r, 2);
                        }
                        if (check)
                        {
                            continue;
                        }

                        int tileindex = cxti + cyti * mapeditor.mapdata.WIDTH;
                        _PaintFogOfWar(tileindex);
                    }
                }
            }
            else
            {
                int startx = (int)(MouseTilePos.X - Math.Floor(mapeditor.brush_x / 2d));
                int starty = (int)(MouseTilePos.Y - Math.Floor(mapeditor.brush_y / 2d));
                for (int y = 0; y < mapeditor.brush_y; y++)
                {
                    for (int x = 0; x < mapeditor.brush_x; x++)
                    {
                        int cxti = startx + x;
                        int cyti = starty + y;

                        if (cxti < 0 || cyti < 0)
                        {
                            continue;
                        }
                        if (cxti >= mapeditor.mapdata.WIDTH || cyti >= mapeditor.mapdata.HEIGHT)
                        {
                            continue;
                        }

                        int tileindex = cxti + cyti * mapeditor.mapdata.WIDTH;
                        _PaintFogOfWar(tileindex);
                    }
                }
            }
        }


        bool _fogofwarmouseleftDown;
        bool _fogofwarmouserightDown;
        private void _PaintFogOfWar(int tileindex)
        {
            List<byte> players = new List<byte>();
            if (mapeditor.opt_FogofWarplayer < 8)
            {
                players.Add((byte)mapeditor.opt_FogofWarplayer);
            }
            else if (mapeditor.opt_FogofWarplayer <= 12)
            {

                if (mapeditor.opt_FogofWarplayer != 12)
                {
                    int force = mapeditor.opt_FogofWarplayer - 8;
                    for (byte i = 0; i < 8; i++)
                    {
                        if (mapeditor.mapdata.FORCE[i] == force)
                        {
                            players.Add(i);
                        }
                    }
                }
                else
                {
                    for (byte i = 0; i < 8; i++)
                    {
                        players.Add(i);
                    }
                }
            }

            byte flag = mapeditor.mapdata.MASK[tileindex];
            if (mouse_LeftDown)
            {
                for (int i = 0; i < players.Count; i++)
                {
                    flag &= (byte)~(0b1 << players[i]);
                }

                if (!_fogofwarmouseleftDown)
                {
                    _fogofwarmouseleftDown = true;
                    mapeditor.taskManager.TaskStart();
                }
                mapeditor.taskManager.TaskAdd(new MaskEvent(mapeditor, tileindex, flag, mapeditor.mapdata.MASK[tileindex]));
                mapeditor.mapdata.MASK[tileindex] = flag;
            }
            else if (mouse_RightDown)
            {
                for (int i = 0; i < players.Count; i++)
                {
                    flag |= (byte)(0b1 << players[i]);
                }

                if (!_fogofwarmouserightDown)
                {
                    _fogofwarmouserightDown = true;
                    mapeditor.taskManager.TaskStart();
                }
                mapeditor.taskManager.TaskAdd(new MaskEvent(mapeditor, tileindex, flag, mapeditor.mapdata.MASK[tileindex]));
                mapeditor.mapdata.MASK[tileindex] = flag;
            }

            if (!mouse_LeftDown)
            {
                if (_fogofwarmouseleftDown)
                {
                    _fogofwarmouseleftDown = false;
                    mapeditor.taskManager.TaskEnd();
                }
            }
            if (!mouse_RightDown)
            {
                if (_fogofwarmouserightDown)
                {
                    _fogofwarmouserightDown = false;
                    mapeditor.taskManager.TaskEnd();
                }
            }
        }




        private void FogofWarPalletePreview()
        {
            _spriteBatch.Begin();

            if (mapeditor.brush_fogofwarcircle)
            {
                int bx = mapeditor.brush_x + 2;
                int by = mapeditor.brush_y + 2;


                int startx = (int)(MouseTilePos.X - Math.Floor(bx / 2d));
                int starty = (int)(MouseTilePos.Y - Math.Floor(by / 2d));
                Vector2 center = new Vector2(startx, starty);
                int a = (int)Math.Floor(bx / 2d);
                int b = (int)Math.Floor(by / 2d);


                for (int y = 0; y < by; y++)
                {
                    for (int x = 0; x < bx; x++)
                    {
                        int cxti = startx + x;
                        int cyti = starty + y;

                        if (cxti < 0 || cyti < 0)
                        {
                            continue;
                        }
                        if (cxti >= mapeditor.mapdata.WIDTH || cyti >= mapeditor.mapdata.HEIGHT)
                        {
                            continue;
                        }

                        int r = Math.Max(bx, by) / 2;


                        bool check;
                        if(bx > by)
                        {
                            check = (Math.Pow(x - a, 2) + Math.Pow((y - b) * ((double)bx / by), 2)) >= Math.Pow(r, 2);
                        }
                        else if(bx < by)
                        {
                            check = (Math.Pow((x - a) * ((double)by / bx), 2) + Math.Pow(y - b, 2)) >= Math.Pow(r, 2);
                        }
                        else
                        {
                            check = (Math.Pow(x - a, 2) + Math.Pow(y - b, 2)) >= Math.Pow(r, 2);
                        }
                        if (check)
                        {
                            continue;
                        }



                        Vector2 v = mapeditor.PosMapToScreen(new Vector2(cxti, cyti) * 32);

                        _spriteBatch.Draw(gridtexture, v, null, new Color(0, 128, 0, 16), 0, new Vector2(), (float)mapeditor.opt_scalepercent * 32, SpriteEffects.None, 0);

                    }
                }
            }
            else
            {
                int startx = (int)(MouseTilePos.X - Math.Floor(mapeditor.brush_x / 2d));
                int starty = (int)(MouseTilePos.Y - Math.Floor(mapeditor.brush_y / 2d));
                for (int y = 0; y < mapeditor.brush_y; y++)
                {
                    for (int x = 0; x < mapeditor.brush_x; x++)
                    {
                        int cxti = startx + x;
                        int cyti = starty + y;

                        if (cxti < 0 || cyti < 0)
                        {
                            continue;
                        }
                        if (cxti >= mapeditor.mapdata.WIDTH || cyti >= mapeditor.mapdata.HEIGHT)
                        {
                            continue;
                        }


                        Vector2 v = mapeditor.PosMapToScreen(new Vector2(cxti, cyti) * 32);

                        _spriteBatch.Draw(gridtexture, v, null, new Color(0, 128, 0, 16), 0, new Vector2(), (float)mapeditor.opt_scalepercent * 32, SpriteEffects.None, 0);

                    }
                }
            }
            _spriteBatch.End();
        }
    }
}
