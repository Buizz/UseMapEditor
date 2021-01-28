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



        private void DrawImageListPreview(List<CImage> templist, Vector2 center)
        {
            float scale = 1;
            float grpscale = 1;
            switch (mapeditor.opt_drawType)
            {
                case Control.MapEditor.DrawType.SD:
                    //scale = (float)mapeditor.opt_scalepercent;
                    grpscale = 1;

                    break;
                case Control.MapEditor.DrawType.HD:
                case Control.MapEditor.DrawType.CB:
                    //scale = (float)mapeditor.opt_scalepercent;
                    grpscale = 2;
                    break;
            }

            _spriteBatch.Begin(blendState: BlendState.NonPremultiplied);
            _spriteBatch.Draw(gridtexture, new Rectangle((int)(center.X - 128), (int)(center.Y - 128), (int)256, (int)256), null, new Color(0, 0, 0, 128), 0, new Vector2(), SpriteEffects.None, 0);
            _spriteBatch.End();


            templist.Sort((x1, x2) => x1.drawsort.CompareTo(x2.drawsort));
            for (int i = 0; i < templist.Count; i++)
            {
                templist[i].screen = center;

                DrawImage(mapeditor.opt_drawType, templist[i], scale, grpscale);
            }
        }


        private void DrawImageList(List<CImage> templist)
        {
            float scale = 1;
            float grpscale = 1;
            switch (mapeditor.opt_drawType)
            {
                case Control.MapEditor.DrawType.SD:
                    scale = (float)mapeditor.opt_scalepercent;
                    grpscale = 1;

                    break;
                case Control.MapEditor.DrawType.HD:
                case Control.MapEditor.DrawType.CB:
                    scale = (float)mapeditor.opt_scalepercent;
                    grpscale = 2;
                    break;
            }

            templist.Sort((x1, x2) => x1.drawsort.CompareTo(x2.drawsort));
            for (int i = 0; i < templist.Count; i++)
            {
                DrawImage(mapeditor.opt_drawType, templist[i], scale, grpscale);
            }
        }


        private void DrawImage(Control.MapEditor.DrawType drawType, CImage cImage, float scale, float grpscale)
        {
            if (!mapeditor.view_Unit_StartLoc & cImage.imageID == 588)
            {
                return;
            }
            if (!mapeditor.view_Unit_Maprevealer & cImage.imageID == 582)
            {
                return;
            }




            Vector2 pos = cImage.screen;

            GRP gRP = null;
            bool setgrp = false;
            if (cImage.imageID == 588)
            {
                //스타트로케이션
                if(drawType == Control.MapEditor.DrawType.CB)
                {
                    gRP = GetImageTexture(Control.MapEditor.DrawType.HD, cImage.imageID);
                    setgrp = true;
                }
            }

            if (!setgrp)
            {
                gRP = GetImageTexture(drawType, cImage.imageID);
            }


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
                case 588:
                    //스타트로케이션
                    if (drawType != Control.MapEditor.DrawType.SD)
                    {
                        x = -61;
                        y = -32;
                    }
                    break;
                case 582:
                    //맵리빌러
                    if (drawType == Control.MapEditor.DrawType.HD)
                    {
                        x = -16;
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




            Color color = Color.White;

            bool drawcmp = false;
            bool nonecolor = false;
            switch (cImage.drawType)
            {
                case CImage.DrawType.Shadow:
                    color = new Color(0, 0, 0, 128);
                    _spriteBatch.Begin(SpriteSortMode.FrontToBack, blendState: BlendState.NonPremultiplied, samplerState: SamplerState.PointClamp);
                    _spriteBatch.Draw(gRP.MainGRP, p, new Rectangle(fd.x, fd.y, fd.fwidth, fd.fheight), color, 0, Vector2.Zero, scale / grpscale, spriteEffects, 1 - cImage.Level / 30f);
                    _spriteBatch.End();
                    return;
                case CImage.DrawType.Clock:
                    color = new Color(255, 255, 255, 64);
                    break;
                case CImage.DrawType.Hallaction:
                    color = new Color(64, 64, 255, 255);
                    nonecolor = true;
                    break;
                case CImage.DrawType.UnitSprite:
                    color = Color.White;
                    if (mapeditor.view_SpriteColor)
                    {
                        color = mapeditor.SpriteOverlay;
                    }
                    //color = new Color(255, 0, 0, 255);
                    break;
                case CImage.DrawType.PureSprite:
                    if (mapeditor.view_SpriteColor)
                    {
                        color = mapeditor.SpriteOverlay;
                    }
                    //color = new Color(0, 255, 0, 255);
                    break;
                case CImage.DrawType.Doodad:
                    if (mapeditor.view_DoodadColor)
                    {
                        color = mapeditor.DoodadOverlay;
                    }
                    //color = new Color(0, 255, 0, 255);
                    break;
            }
            if (!drawcmp)
            {
                _spriteBatch.Begin(SpriteSortMode.FrontToBack, blendState: BlendState.NonPremultiplied, samplerState: SamplerState.PointClamp);
                _spriteBatch.Draw(gRP.MainGRP, p, new Rectangle(fd.x, fd.y, fd.fwidth, fd.fheight), color, 0, Vector2.Zero, scale / grpscale, spriteEffects, 1 - cImage.Level / 30f);
            }


            _spriteBatch.End();
            if (!nonecolor)
            {
                if (cImage.color >= 0)
                {
                    if (gRP.Color != null)
                    {
                        Color unitColor = mapeditor.mapdata.UnitColor(cImage.color);

                        if (cImage.drawType == CImage.DrawType.Clock)
                        {
                            unitColor.A = 64;
                        }

                        //BlendState blendState = GraphicsDevice.BlendState;
                        //GraphicsDevice.BlendState = ColorBlend;

                        _colorBatch.Begin(SpriteSortMode.FrontToBack, ColorBlend, samplerState: SamplerState.PointClamp);
                        _colorBatch.Draw(gRP.Color, p, new Rectangle(fd.x, fd.y, fd.fwidth, fd.fheight), unitColor, 0, Vector2.Zero, scale / grpscale, spriteEffects, 1 - cImage.Level / 30f);

                        //GraphicsDevice.BlendState = blendState;

                        _colorBatch.End();
                    }
                }
            }






            _spriteBatch.Begin(blendState: BlendState.NonPremultiplied);

            if(cImage.IsHover | cImage.IsSelect)
            {
                Vector2 rectpos;
                Vector2 rectsize;
                if (cImage.IsUnitRect)
                {
                    rectpos = pos - new Vector2(cImage.Left, cImage.Up) * scale;
                    rectsize = new Vector2(cImage.Left + cImage.Right, cImage.Up + cImage.Down) * scale;
                }
                else
                {
                    rectpos = p;
                    rectsize = new Vector2(fd.fwidth, fd.fheight);

                    if(rectsize.LengthSquared() < 8)
                    {
                        rectpos -= new Vector2(4 * scale);
                        rectsize += new Vector2(8);
                    }


                    rectsize *= scale / grpscale;
                }


                if (cImage.IsHover)
                {
                    _spriteBatch.Draw(gridtexture, new Rectangle((int)rectpos.X, (int)rectpos.Y, (int)rectsize.X, (int)rectsize.Y), null, new Color(128, 128, 255, 48), 0, new Vector2(), SpriteEffects.None, 1);
                    DrawRect(_spriteBatch, rectpos, rectpos + rectsize, Color.Red);
                }
                else if (cImage.IsSelect)
                {
                    _spriteBatch.Draw(gridtexture, new Rectangle((int)rectpos.X, (int)rectpos.Y, (int)rectsize.X, (int)rectsize.Y), null, new Color(128, 255, 128, 48), 0, new Vector2(), SpriteEffects.None, 1);
                    DrawRect(_spriteBatch, rectpos, rectpos + rectsize, Color.Red);
                }
            }
            _spriteBatch.End();



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
