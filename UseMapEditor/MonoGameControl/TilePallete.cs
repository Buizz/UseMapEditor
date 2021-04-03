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



        private void DrawTileSetPallet()
        {
            if (mapeditor.PalleteLayer == Control.MapEditor.Layer.Tile)
            {
                double maxvalue = tileSet.cv5data[mapeditor.mapdata.TILETYPE].Length * 32;
                if (maxvalue != mapeditor.TileScroll.Maximum)
                {
                    mapeditor.TileScroll.Maximum = maxvalue;
                }


                Point relativePoint = mapeditor.TileDrawPanel.TransformToAncestor(mapeditor.ToolBarExpander).Transform(new Point(0, 0));


                int startX = ((int)(screenwidth));
                int startY = (int)relativePoint.Y - mapeditor.brush_tilescroll % 32;

                int tiley = mapeditor.brush_tilescroll / 32;


                _spriteBatch.Begin();
                for (int y = 0; y < 30; y++)
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
                                        _spriteBatch.Draw(texture2D, new Vector2(startX + x * 32, startY + y * 32), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                                    }
                                }
                                break;
                            case Control.MapEditor.DrawType.HD:
                            case Control.MapEditor.DrawType.CB:
                                {
                                    Texture2D texture2D = tileSet.GetTile(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, (ushort)(y + tiley), (ushort)x);
                                    if (texture2D != null)
                                    {
                                        _spriteBatch.Draw(texture2D, new Vector2(startX + x * 32, startY + y * 32), null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
                                    }
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
