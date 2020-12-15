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

        int CloseTimer = 0;
        private void DrawPallet()
        {
            bool IsOpen = false;



            if (mapeditor.IsToolBarOpen())
            {
                //열려 있을 경우
                if(CloseTimer < 256)
                {
                    CloseTimer +=20;
                }
                else
                {
                    //열림 상태
                    IsOpen = true;
                }
            }
            else
            {
                //닫혀 있을 경우
                if (CloseTimer > 0)
                {
                    CloseTimer -= 20;
                }
            }
            CloseTimer = Math.Min(256, CloseTimer);
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
            _spriteBatch.Draw(gridtexture, new Rectangle((int)(screenwidth - (CloseTimer / 256d) * 384), 0, CloseTimer / 2, 128), Color.Black);
            _spriteBatch.End();


            if (!IsOpen)
            {
                return;
            }

            if (!mapeditor.IsMinimapLoad)
            {
                DrawMiniMap();
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




            _spriteBatch.Begin();
            _spriteBatch.Draw(minimap, new Vector2((int)(screenwidth - (CloseTimer / 256d) * 384), 0), Color.White);
            _spriteBatch.End();
        }

        private Texture2D minimap;





        private void DrawMiniMap()
        {

            _spriteBatch.Begin();



            for (int y = 0; y < mapeditor.mapdata.HEIGHT; y++)
            {
                for (int x = 0; x < mapeditor.mapdata.WIDTH; x++)
                {
                    int tileindex = x + y * mapeditor.mapdata.WIDTH;
                    ushort MTXM = mapeditor.mapdata.MTXM[tileindex];

                    Random random = new Random();
                    mapeditor.minimapcolor[x + y * 128] = new Color((uint)random.Next());
                }
            }
            
            _spriteBatch.End();

        }
    }
}
