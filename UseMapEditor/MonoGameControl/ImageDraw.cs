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
        private Dictionary<int, int> grpdic;
        private int GetImageIndex(int index)
        {
            int grpindex = (int)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.images, "GRP File", index).Data;
            


            return grpdic[grpindex];
        }




        public Dictionary<Control.MapEditor.DrawType, GRP[]> GRPDATA;
        public class GRP
        {
            public string GRPFile;


            public Texture2D MainGRP;
            public Texture2D Color;
            public bool IsLoad;

            public FrameData[] frameDatas;

            public ushort frameCount;
            public ushort grpwidth;
            public ushort grpheight;

            public class FrameData
            {
                public short x;  // Coordinates of the top-left pixel of the frame
                public short y;
                public short xoff; // X,Y offsets from the top left of the GRP frame -- value seems directly copied from each GRP
                public short yoff;
                public short fwidth; // Dimensions, relative to the top-left pixel, of the frame
                public short fheight;
                public short funk1; // always 0? or 1?
                public short funk2; // always 0?
            }
        }



        private void DrawImage(Control.MapEditor.DrawType drawType, CImage cImage, float scale, float grpscale)
        {
            Vector2 pos = cImage.screen;







            GRP gRP = GetImageTexture(drawType, cImage.imageID);
            if(gRP.frameCount == 0)
            {
                return;
            }
            int frame = cImage.Frame % gRP.frameCount;

            if (cImage.Turnable)
            {
                frame += cImage.turnFrame;
            }


            GRP.FrameData fd = gRP.frameDatas[frame];

            Vector2 p = pos;


            int x = 0, y = 0;
            switch (cImage.imageID)
            {
                case 235:
                    break;
                case 251:
                    if(drawType != Control.MapEditor.DrawType.CB)
                    {
                        x = -8;
                        y = -16;
                    }
                    break;
                case 254:
                    if (drawType != Control.MapEditor.DrawType.CB)
                    {
                        x = -8;
                        y = -16;
                    }
                    break;
            }


            Vector2 lol = new Vector2(x, y);
            Vector2 grppos = new Vector2(fd.xoff, fd.yoff) / grpscale;
            Vector2 grpsize = new Vector2(gRP.grpwidth, gRP.grpheight) / grpscale / grpscale;
            Vector2 imgpospos = new Vector2(cImage.XOffset, cImage.YOffset);
            Vector2 framesize = new Vector2(fd.fwidth, fd.fheight) / grpscale;


            Vector2 LastPos = new Vector2();

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (cImage.IsLeft)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;

                LastPos.X = grpsize.X / 2 - grppos.X - framesize.X;
                LastPos.Y = grppos.Y - grpsize.Y / 2;

                LastPos += imgpospos + lol;
            }
            else
            {
                LastPos = grppos - grpsize / 2 + imgpospos + lol;
            }




            LastPos *= scale;

            p += LastPos;



            _spriteBatch.Begin(SpriteSortMode.FrontToBack, blendState: BlendState.NonPremultiplied, samplerState: SamplerState.PointClamp);

            Color color;

            switch (cImage.drawType)
            {
                case CImage.DrawType.Shadow:
                    color = new Color(0, 0, 0, 128);
                    //_spriteBatch.Draw(gRP.MainGRP, p, new Rectangle(fd.x, fd.y, fd.fwidth, fd.fheight), new Color(0, 0, 0, 128), 0, Vector2.Zero, scale / grpscale, SpriteEffects.None, 1 - cImage.Level / 30f);
                    _spriteBatch.Draw(gRP.MainGRP, p, new Rectangle(fd.x, fd.y, fd.fwidth, fd.fheight), color, 0, Vector2.Zero, scale / grpscale, spriteEffects, 1 - cImage.Level / 30f);
                    _spriteBatch.End();
                    return;
                case CImage.DrawType.Clock:
                    color = new Color(255, 255, 255, 64);
                    break;
                case CImage.DrawType.Hallaction:
                    color = new Color(64, 64, 255, 255);
                    _spriteBatch.Draw(gRP.MainGRP, p, new Rectangle(fd.x, fd.y, fd.fwidth, fd.fheight), color, 0, Vector2.Zero, scale / grpscale, spriteEffects, 1 - cImage.Level / 30f);
                    _spriteBatch.End();
                    return;
                case CImage.DrawType.UnitSprite:
                    color = new Color(255, 0, 0, 255);
                    break;
                case CImage.DrawType.PureSprite:
                    color = new Color(0, 255, 0, 255);
                    break;
                default:
                    color = Color.White;
                    break;
            }





            _spriteBatch.Draw(gRP.MainGRP, p, new Rectangle(fd.x, fd.y, fd.fwidth, fd.fheight), color, 0, Vector2.Zero, scale / grpscale, spriteEffects, 1 - cImage.Level / 30f);

            _spriteBatch.End();

            if (cImage.color >= 0)
            {
                if (gRP.Color != null)
                {
                    //BlendState blendState = GraphicsDevice.BlendState;
                    //GraphicsDevice.BlendState = ColorBlend;

                    _colorBatch.Begin(SpriteSortMode.FrontToBack, ColorBlend, samplerState: SamplerState.PointClamp);
                    _colorBatch.Draw(gRP.Color, p, new Rectangle(fd.x, fd.y, fd.fwidth, fd.fheight), mapeditor.mapdata.UnitColor(cImage.color), 0, Vector2.Zero, scale / grpscale, spriteEffects, 1- cImage.Level / 30f);

                    //GraphicsDevice.BlendState = blendState;

                    _colorBatch.End();
                }
            }
        }


        public GRP GetImageTexture(Control.MapEditor.DrawType drawType, int imageindex)
        {
            imageindex = GetImageIndex(imageindex);

            GRP rgrp = GRPDATA[drawType][imageindex];

            float scale;
            if(drawType == Control.MapEditor.DrawType.SD)
            {
                scale = 1;
            }
            else
            {
                scale = 2;
            }

            if (!rgrp.IsLoad)
            {
                //GRP로드가 되지 않았을 경우.
                string fname = AppDomain.CurrentDomain.BaseDirectory + $"CascData\\{drawType.ToString()}\\anim\\" + imageindex.ToString() + "\\diffuse.png";
                string fcolorname = AppDomain.CurrentDomain.BaseDirectory + $"CascData\\{drawType.ToString()}\\anim\\" + imageindex.ToString() + "\\teamcolor.png";
                string framedataname = AppDomain.CurrentDomain.BaseDirectory + $"CascData\\{drawType.ToString()}\\anim\\" + imageindex.ToString() + "\\framedata";
                rgrp.GRPFile = fname;
                if (File.Exists(fname))
                {
                    rgrp.MainGRP = LoadFromFile(fname);
                }
                if (File.Exists(fcolorname))
                {
                    rgrp.Color = LoadFromFile(fcolorname);
                }

                if (File.Exists(framedataname))
                {
                    BinaryReader br = new BinaryReader(new MemoryStream(File.ReadAllBytes(framedataname)));

                    rgrp.frameDatas = new GRP.FrameData[br.BaseStream.Length / 16];
                    rgrp.frameCount = br.ReadUInt16();
                    rgrp.grpwidth = br.ReadUInt16();
                    rgrp.grpheight = br.ReadUInt16();
                    for (int i = 0; i < rgrp.frameDatas.Length; i++)
                    {
                        GRP.FrameData frameData = new GRP.FrameData();

                        frameData.x = (short)(br.ReadInt16() / scale);
                        frameData.y = (short)(br.ReadInt16() / scale);
                        frameData.xoff = (short)(br.ReadInt16() / scale);
                        frameData.yoff = (short)(br.ReadInt16() / scale);
                        frameData.fwidth = (short)(br.ReadInt16() / scale);
                        frameData.fheight = (short)(br.ReadInt16() / scale);
                        frameData.funk1 = (short)(br.ReadInt16() / scale);
                        frameData.funk2 = (short)(br.ReadInt16() / scale);

                        rgrp.frameDatas[i] = frameData;
                    }
                    if (drawType == Control.MapEditor.DrawType.SD)
                    {
                        rgrp.grpwidth = (ushort)SDGRPSIZE[imageindex].X;
                        rgrp.grpheight = (ushort)SDGRPSIZE[imageindex].Y;
                    }



                     br.Close();
                }


                rgrp.IsLoad = true;
            }



            return rgrp;
        }

    }
}
