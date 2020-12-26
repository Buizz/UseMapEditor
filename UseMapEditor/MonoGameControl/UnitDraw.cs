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
using static Data.Map.MapData;

namespace UseMapEditor.MonoGameControl
{
    public partial class MapDrawer : WpfGame
    {
        private void RenderUnit(bool IsDrawGrp)
        {
            for (int i = 0; i < mapeditor.mapdata.UNIT.Count; i++)
            {
                DrawUnit(mapeditor.mapdata.UNIT[i]);
            }
        }
        private void RenderSprite(bool IsDrawGrp)
        {
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
                        cTHG2.Images[i].screen = screen;
                        ImageList.Add(cTHG2.Images[i]);
                        cTHG2.Images[i].PlayScript();
                    }
                    
                }
            }
        }






        private void DrawUnit(CUNIT cUNIT)
        {
            int _x = cUNIT.x;
            int _y = cUNIT.y;
            Vector2 screen = mapeditor.PosMapToScreen(new Vector2(_x, _y));


            int grpwidth = (int)(cUNIT.BoxWidth * mapeditor.opt_scalepercent);
            int grpheight = (int)(cUNIT.BoxHeight * mapeditor.opt_scalepercent);



            float minX = 0 - grpwidth;
            float minY = 0 - grpheight;
            float maxX = screenwidth + grpwidth;
            float maxY = screenheight + grpheight;



            if ((minX < screen.X) & (screen.X < maxX))
            {
                if ((minY < screen.Y) & (screen.Y < maxY))
                {



                    if (cUNIT.Images.Count == 0)
                    {
                        cUNIT.ImageReset();
                    }
                    for (int i = 0; i < cUNIT.Images.Count; i++)
                    {
                        cUNIT.Images[i].screen = screen;
                        ImageList.Add(cUNIT.Images[i]);
                        cUNIT.Images[i].PlayScript();
                    }


                    ////UnitZPos
                    //_spriteBatch.Begin();
                    //DrawLine(_spriteBatch, new Vector2(screen.X - 20, screen.Y), new Vector2(screen.X + 20, screen.Y), Color.Red);
                    //DrawLine(_spriteBatch, new Vector2(screen.X, screen.Y - 20), new Vector2(screen.X, screen.Y + 20), Color.Red);

                    //_spriteBatch.End();


                }
            }
        }


        private List<CImage> ImageList = new List<CImage>();

    }
}
