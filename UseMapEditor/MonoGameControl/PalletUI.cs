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

namespace UseMapEditor.MonoGameControl
{
    public partial class MapDrawer : WpfGame
    {

        int CloseTimer = 0;


        private void DrawPallet()
        {
            bool IsOpen = false;



            CloseTimer = (int)(mapeditor.GetToolBarWidth());
            if (mapeditor.IsToolBarOpen())
            {
                //열려 있을 경우
                if(CloseTimer >= 512)
                {
                    //열림 상태
                    IsOpen = true;
                }
            }





            CloseTimer = Math.Min(512, CloseTimer);
            CloseTimer = Math.Max(0, CloseTimer);


            Color Back;
            //베이스 깔기
            if (Global.Setting.Vals[Global.Setting.Settings.Program_IsDark] == "true")
            {
                Back = new Color(0xFF303030);
            }
            else
            {
                Back = new Color(0xFFFAFAFA);
            }

            _spriteBatch.Begin();
            _spriteBatch.Draw(gridtexture, new Rectangle((int)(screenwidth - CloseTimer), 0, CloseTimer, (int)screenheight), Back);
            //_spriteBatch.Draw(gridtexture, new Rectangle((int)(screenwidth - CloseTimer - 128), 0, 128, 128), Color.DimGray);
            _spriteBatch.End();


            DrawMiniMap();


            if (!IsOpen)
            {
                return;
            }

            DrawTileSetPallet();
        }


        private bool IsMiniMapDrag = false;
        private void MiniMapClick(MouseState wpfMouse)
        {
            if(wpfMouse.LeftButton == ButtonState.Pressed)
            {
                if (0 < MousePos.Y & MousePos.Y < 128)
                {
                    if ((screenwidth - CloseTimer - 128) < MousePos.X)
                    {
                        IsMiniMapDrag = true;
                    }
                }
            }
            else
            {
                IsMiniMapDrag = false;
            }



            if (IsMiniMapDrag)
            {
                int ClickX = (int)(MousePos.X - (screenwidth - CloseTimer - 128));
                int ClickY = (int)MousePos.Y;


                int MapW = mapeditor.mapdata.WIDTH;
                int MapH = mapeditor.mapdata.HEIGHT;

                int MaxWidth = Math.Max(MapW, MapH);
                double mapScale = 128d / MaxWidth;

                MapW = (int)(MapW * mapScale);
                MapH = (int)(MapH * mapScale);

                ClickX -= (128 - MapW) / 2;
                ClickY -= (128 - MapH) / 2;

                ClickX -= (int)(screenwidth / mapeditor.opt_scalepercent / 64d * mapScale);
                ClickY -= (int)(screenheight / mapeditor.opt_scalepercent / 64d * mapScale);


                mapeditor.opt_xpos = (int)(ClickX * 32 / mapScale);
                mapeditor.opt_ypos = (int)(ClickY * 32 / mapScale);
            }
        }




        private Texture2D minimap;

        private void DrawMiniMap()
        {
            if (!mapeditor.IsMinimapLoad)
            {
                CreateMiniMap();
                minimap.SetData(mapeditor.minimapcolor);
                mapeditor.IsMinimapLoad = true;
            }
            else
            {
                if (mapeditor.ChangeMiniMap)
                {
                    minimap.SetData(mapeditor.minimapcolor);
                    mapeditor.ChangeMiniMap = false;
                }
            }

            //drawMinimap
            _spriteBatch.Begin();
            int StartX = (int)(screenwidth - CloseTimer) - 128;
            int StartY = 0;
            int MapW = mapeditor.mapdata.WIDTH;
            int MapH = mapeditor.mapdata.HEIGHT;

            int MaxWidth = Math.Max(MapW, MapH);
            double mapScale = 128d / MaxWidth;

            MapW = (int)(MapW * mapScale);
            MapH = (int)(MapH * mapScale);

            StartX += (128 - MapW) / 2;
            StartY += (128 - MapH) / 2;
            _spriteBatch.Draw(minimap, new Rectangle(StartX, StartY, MapW, MapH), new Rectangle(0, 0, mapeditor.mapdata.WIDTH, mapeditor.mapdata.HEIGHT), Color.White);



            Vector2 MiniMin = new Vector2(mapeditor.opt_xpos, mapeditor.opt_ypos) / 32;
            Vector2 Size = new Vector2((float)(screenwidth / mapeditor.opt_scalepercent), (float)(screenheight / mapeditor.opt_scalepercent));
            Size /= 32;


            MiniMin *= (float)mapScale;
            Size *= (float)mapScale;

            MiniMin += new Vector2(StartX, StartY);

            DrawRect(_spriteBatch, MiniMin, MiniMin + Size, Color.White);
            _spriteBatch.End();
        }



        private void CreateMiniMap()
        {


            for (int y = 0; y < mapeditor.mapdata.HEIGHT; y++)
            {
                for (int x = 0; x < mapeditor.mapdata.WIDTH; x++)
                {
                    int tileindex = x + y * mapeditor.mapdata.WIDTH;
                    ushort MTXM = mapeditor.mapdata.TILE[tileindex];

                    mapeditor.minimapcolor[x + y * 256] = tileSet.GetTileColor(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, MTXM);
                }
            }

        }

        private void DrawTileSetPallet()
        {
            if(mapeditor.TilePallet.Visibility == Visibility.Visible)
            {
                int startX = ((int)(screenwidth - CloseTimer));
                int startY = 128;

                _spriteBatch.Begin();
                for (int y = 0; y < 30; y++)
                {
                    for (int x = 0; x < 16; x++)
                    {

                        switch (mapeditor.opt_drawType)
                        {
                            case Control.MapEditor.DrawType.SD:
                                {
                                    Texture2D texture2D = tileSet.GetTile(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, (ushort)y, (ushort)x);
                                    _spriteBatch.Draw(texture2D, new Vector2(startX + x * 32, startY + y * 32), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                                }
                                break;
                            case Control.MapEditor.DrawType.HD:
                            case Control.MapEditor.DrawType.CB:
                                {
                                    Texture2D texture2D = tileSet.GetTile(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, (ushort)y, (ushort)x);
                                    _spriteBatch.Draw(texture2D, new Vector2(startX + x * 32, startY + y * 32), null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
                                }
                                break;
                        }
                    }
                }

                _spriteBatch.End();
            }
        }
    }
}
