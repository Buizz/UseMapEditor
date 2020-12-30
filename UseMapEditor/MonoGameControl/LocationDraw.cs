using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Data.Map.MapData;

namespace UseMapEditor.MonoGameControl
{
    public partial class MapDrawer : WpfGame
    {
        private void RenderLocation()
        {
            _spriteBatch.Begin( blendState: BlendState.NonPremultiplied);
            //_spriteBatch.Begin(blendState: BlendState.NonPremultiplied);
            for (int i = 1; i < mapeditor.mapdata.LocationDatas.Count; i++)
            {
                DrawLocation(mapeditor.mapdata.LocationDatas[i]);
            }
            _spriteBatch.End();
        }


        private void DrawLocation(LocationData location)
        {
            uint L = location.L;
            uint R = location.R;
            uint T = location.T;
            uint B = location.B;

            uint minX = Math.Min(L, R);
            uint maxX = Math.Max(L, R);
            uint minY = Math.Min(T, B);
            uint maxY = Math.Max(T, B);
            uint locwidth = maxX - minX;
            uint locheight = maxY - minY;

            if((locwidth == mapeditor.mapdata.WIDTH * 32) & (locheight == mapeditor.mapdata.HEIGHT * 32))
            {
                return;
            }


            float MinSize = Math.Min(locwidth, locheight);

            float mag = MinSize / 150;

            mag = Math.Max(0.5f, mag);
            mag = Math.Min(4, mag);



            Vector2 min = mapeditor.PosMapToScreen(new Vector2(minX, minY));
            Vector2 max = mapeditor.PosMapToScreen(new Vector2(maxX, maxY));
            Vector2 size = max - min;


            float screenminX = 0;
            float screenminY = 0;
            float screenmaxX = screenwidth;
            float screenmaxY = screenheight;




            _spriteBatch.Draw(gridtexture, new Rectangle((int)min.X, (int)min.Y, (int)size.X, (int)size.Y), null, location.RnColor, 0, new Vector2(), SpriteEffects.None, 0);
            DrawLocationRect(_spriteBatch, location.STRING.String, min, max, Color.Black, mag, (float)(2 * mapeditor.opt_scalepercent));
        }

        public void DrawLocationRect(SpriteBatch spriteBatch, string str, Vector2 point1, Vector2 point2, Color color, float loSize, float thickness = 1f)
        {
            DrawLine(spriteBatch, new Vector2(point1.X, point1.Y), new Vector2(point2.X, point1.Y), color, thickness);
            DrawLine(spriteBatch, new Vector2(point1.X, point1.Y), new Vector2(point1.X, point2.Y), color, thickness);
            DrawLine(spriteBatch, new Vector2(point2.X, point1.Y), new Vector2(point2.X, point2.Y), color, thickness);
            DrawLine(spriteBatch, new Vector2(point2.X, point2.Y), new Vector2(point1.X, point2.Y), color, thickness);

            color.A = 64;
            DrawLine(spriteBatch, new Vector2(point1.X, point1.Y), new Vector2(point2.X, point2.Y), color, thickness);
            DrawLine(spriteBatch, new Vector2(point2.X, point1.Y), new Vector2(point1.X, point2.Y), color, thickness);


            Vector2 drawSize = point2 - point1;



            loSize *= (float)mapeditor.opt_scalepercent;

            int stra = 255;
            stra -= (int)loSize * 30;
            stra = Math.Max(stra, 0);
            stra = Math.Min(stra, 255);


            Vector2 strsize = _locationfont.MeasureString(str);
            strsize *= loSize;


            if (strsize.X > drawSize.X)
            {
                str = str.Substring(0, str.Length / 2).Trim() + "\n" + str.Substring(str.Length / 2).Trim();
                strsize = _locationfont.MeasureString(str);
                strsize *= loSize;
            }




            spriteBatch.DrawString(_locationfont, str, (point1 + point2) / 2 - strsize / 2, new Color(255,255,255,stra), 0, new Vector2(), loSize, SpriteEffects.None, 0);
        }

    }
}
