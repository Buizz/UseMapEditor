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
        public Dictionary<Control.MapEditor.DrawType, GRP[]> GRPDATA;
        public struct GRP
        {
            public Texture2D MainGRP;
            public Texture2D Color;
            public bool IsLoad;

            public FrameData[] frameDatas;
            public struct FrameData
            {
                public ushort x;  // Coordinates of the top-left pixel of the frame
                public ushort y;
                public ushort xoff; // X,Y offsets from the top left of the GRP frame -- value seems directly copied from each GRP
                public ushort yoff;
                public ushort fwidth; // Dimensions, relative to the top-left pixel, of the frame
                public ushort fheight;
                public ushort funk1; // always 0? or 1?
                public ushort funk2; // always 0?
            }
        }





        public GRP GetImageTexture(Control.MapEditor.DrawType drawType, int imageindex)
        {
            GRP rgrp = GRPDATA[drawType][imageindex];

            if (!rgrp.IsLoad)
            {
                //GRP로드가 되지 않았을 경우.
                string fname = AppDomain.CurrentDomain.BaseDirectory + $"CascData\\{drawType.ToString()}\\anim\\" + imageindex.ToString() + "\\diffuse.png";
                string fcolorname = AppDomain.CurrentDomain.BaseDirectory + $"CascData\\{drawType.ToString()}\\anim\\" + imageindex.ToString() + "\\teamcolor.png";

                if (File.Exists(fname))
                {
                    rgrp.MainGRP = LoadFromFile(fname);
                }
                if (File.Exists(fcolorname))
                {
                    rgrp.Color = LoadFromFile(fcolorname);
                }


                rgrp.IsLoad = true;
            }



            return rgrp;
        }

    }
}
