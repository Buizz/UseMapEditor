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
        int ToolBaStreachValue = 0;

        private void DrawPallet(bool IsDrawGrp)
        {
            bool IsOpen = false;



            if (mapeditor.IsToolBarOpen())
            {
                //열려 있을 경우
                if(ToolBaStreachValue >= mapeditor.opt_palletSize)
                {
                    //열림 상태
                    IsOpen = true;
                }
            }





            ToolBaStreachValue = Math.Min(512, ToolBaStreachValue);
            ToolBaStreachValue = Math.Max(0, ToolBaStreachValue);


            Color Back = Color.Black;
            //베이스 깔기
            //if (Global.Setting.Vals[Global.Setting.Settings.Program_IsDark] == "true")
            //{
            //    Back = new Color(0xFF303030);
            //}
            //else
            //{
            //    Back = new Color(0xFFFAFAFA);
            //}

            _spriteBatch.Begin();
            _spriteBatch.Draw(gridtexture, new Rectangle((int)(screenwidth), 0, ToolBaStreachValue, (int)screenheight), Back);
            _spriteBatch.End();


            if (IsDrawGrp)
            {
                DrawMiniMap();
            }
            DrawMiniMapRect();
            //if (!IsOpen)
            //{
            //    return;
            //}

            if (!IsDrawGrp)
            {
                return;
            }
            if (ToolBaStreachValue == 0)
            {
                return;
            }
            switch(mapeditor.PalleteLayer)
            {
                case Control.MapEditor.Layer.Tile:
                    DrawTileSetPallet();
                    break;
                case Control.MapEditor.Layer.Doodad:
                    DrawDoodadPallet();
                    break;
            }
        }


        private bool IsMiniMapDrag = false;
        private void MiniMapClick(MouseState wpfMouse)
        {
            if(wpfMouse.LeftButton == ButtonState.Pressed)
            {
                if (0 < MousePos.Y & MousePos.Y < 128)
                {
                    if ((screenwidth - 128) < MousePos.X)
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
                int ClickX = (int)(MousePos.X - (screenwidth - 128));
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





        private void DrawMiniMap()
        {
            if (!mapeditor.IsMinimapLoad)
            {
                CreateMiniMap();
                mapeditor.editorTextureData.minimapUnit.SetData(mapeditor.miniampUnit);
                mapeditor.editorTextureData.minimap.SetData(mapeditor.minimapcolor);
                mapeditor.IsMinimapLoad = true;
                mapeditor.IsMinimapUnitRefresh = true;
            }
            else
            {
                if (mapeditor.ChangeMiniMap)
                {
                    mapeditor.editorTextureData.minimapUnit.SetData(mapeditor.miniampUnit);
                    mapeditor.editorTextureData.minimap.SetData(mapeditor.minimapcolor);
                    mapeditor.ChangeMiniMap = false;
                }
            }

            if (!mapeditor.IsMinimapUnitRefresh)
            {

                CreateMiniMapUnit();

                mapeditor.editorTextureData.minimapUnit.SetData(mapeditor.miniampUnit);
                mapeditor.IsMinimapUnitRefresh = true;
            }




            //drawMinimap
            _spriteBatch.Begin();
            int StartX = (int)(screenwidth) - 128;
            int StartY = 0;
            int MapW = mapeditor.mapdata.WIDTH;
            int MapH = mapeditor.mapdata.HEIGHT;

            int MaxWidth = Math.Max(MapW, MapH);
            double mapScale = 128d / MaxWidth;

            MapW = (int)(MapW * mapScale);
            MapH = (int)(MapH * mapScale);

            StartX += (128 - MapW) / 2;
            StartY += (128 - MapH) / 2;
            _spriteBatch.Draw(mapeditor.editorTextureData.minimap, new Rectangle(StartX, StartY, MapW, MapH), new Rectangle(0, 0, mapeditor.mapdata.WIDTH, mapeditor.mapdata.HEIGHT), Color.White);
            _spriteBatch.Draw(mapeditor.editorTextureData.minimapUnit, new Rectangle(StartX, StartY, MapW, MapH), new Rectangle(0, 0, mapeditor.mapdata.WIDTH, mapeditor.mapdata.HEIGHT), Color.White);
            _spriteBatch.End();



        }

        private void DrawMiniMapRect()
        {
            int StartX = (int)(screenwidth) - 128;
            int StartY = 0;
            int MapW = mapeditor.mapdata.WIDTH;
            int MapH = mapeditor.mapdata.HEIGHT;

            int MaxWidth = Math.Max(MapW, MapH);
            double mapScale = 128d / MaxWidth;

            MapW = (int)(MapW * mapScale);
            MapH = (int)(MapH * mapScale);

            StartX += (128 - MapW) / 2;
            StartY += (128 - MapH) / 2;



            //int MapW = mapeditor.mapdata.WIDTH;
            //int MapH = mapeditor.mapdata.HEIGHT;
            //int MaxWidth = Math.Max(MapW, MapH);
            //double mapScale = 128d / MaxWidth;
            //int StartX = (int)(screenwidth - CloseTimer) - 128;
            //int StartY = 0;



            _spriteBatch.Begin();
            Vector2 MiniMin = new Vector2(mapeditor.opt_xpos, mapeditor.opt_ypos) / 32;
            Vector2 Size = new Vector2((float)(screenwidth / mapeditor.opt_scalepercent), (float)(screenheight / mapeditor.opt_scalepercent));
            Size /= 32;


            MiniMin *= (float)mapScale;
            Size *= (float)mapScale;

            MiniMin += new Vector2(StartX, StartY);



            Vector2 MapMax = MiniMin + Size;

            MapMax.X = Math.Min(MapMax.X, screenwidth);




            DrawRect(_spriteBatch, MiniMin, MapMax, Color.White);
            _spriteBatch.End();
        }

      
        private void CreateMiniMap()
        {
            CreateMiniMapTile();
            CreateMiniMapUnit();
        }
        private void CreateMiniMapTile()
        {
            for (int y = 0; y < mapeditor.mapdata.HEIGHT; y++)
            {
                for (int x = 0; x < mapeditor.mapdata.WIDTH; x++)
                {
                    mapeditor.miniTileUpdate(x, y);
                }
            }
        }
        private void CreateMiniMapUnit()
        {
            for (int i = 0; i < mapeditor.mapdata.UNIT.Count; i++)
            {
                if (mapeditor.mapdata.UNIT[i].unitID == 214)
                {
                    //스타트로케이션
                    if (mapeditor.view_Unit_StartLoc)
                    {
                        mapeditor.miniUnitUpdate(mapeditor.mapdata.UNIT[i]);
                    }
                }
                else if (mapeditor.mapdata.UNIT[i].unitID == 101)
                {
                    //맵리빌러
                    if (mapeditor.view_Unit_Maprevealer)
                    {
                        mapeditor.miniUnitUpdate(mapeditor.mapdata.UNIT[i]);
                    }
                }
                else
                {
                    mapeditor.miniUnitUpdate(mapeditor.mapdata.UNIT[i]);
                }
            }
        }





    }
}
