using Microsoft.Xna.Framework.Graphics;
using Pfim;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UseMapEditor.MonoGameControl;

namespace UseMapEditor.FileData
{
    public class Anim
    {
        MapDrawer MAPDRAWER;

        public Anim(MapDrawer mapDrawer)
        {
            MAPDRAWER = mapDrawer;
        }




        public void ReadUnitData()
        {
            ReadAnim(@"CascData\SD\mainSD.anim", MAPDRAWER.SD_GRP, MAPDRAWER.SD_Color);


            for (int i = 0; i < 999; i++)
            {
                string num = String.Format("{0:000}", i);
                ReadAnim($"CascData\\HD\\anim\\main_{num}.anim", MAPDRAWER.HD_GRP, MAPDRAWER.HD_Color);
                ReadAnim($"CascData\\Carbot\\anim\\main_{num}.anim", MAPDRAWER.CB_GRP, MAPDRAWER.CB_Color);
            }
        }














        private void ReadAnim(string animName, List<Texture2D> MainGRP, List<Texture2D> ColorGRP)
        {
            BinaryReader mr =  new BinaryReader(new MemoryStream(File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + animName)));
            if (mr.BaseStream.Length == 0)
            {
                MainGRP.Add(null);
                ColorGRP.Add(null);

                return;
            }


            //==================header==================
            uint magic = mr.ReadUInt32(); // "ANIM"
            ushort version = mr.ReadUInt16(); // Version? 0x0101 for SD, 0x0202 for HD2, 0x0204 for HD
            ushort unk2_ = mr.ReadUInt16(); // 0 -- more bytes for version?
            ushort layers = mr.ReadUInt16();
            ushort entries = mr.ReadUInt16();


            string[] layerstrs = new string[10];
            for (int i = 0; i < 10; i++)
            {
                layerstrs[i] = System.Text.Encoding.ASCII.GetString(mr.ReadBytes(32)).Replace("\0","");
            }

            uint[] images = new uint[entries];
            if(version == 0x101)
            {
                for (int i = 0; i < entries; i++)
                {
                    images[i] = mr.ReadUInt32();
                }
            }
            //==================header==================


            for (int frameindex = 0; frameindex < entries; frameindex++)
            {
                Texture2D maingrp = null;
                Texture2D colorgrp = null;






                if (version == 0x101)
                {
                    mr.BaseStream.Position = images[frameindex];
                }


                ushort frames = mr.ReadUInt16(); // if frames == 0, it's an entryref and not this struct
                if (frames != 0)
                {
                    //==================entry==================
                    ushort unk2 = mr.ReadUInt16(); // always 0xFFFF?
                    ushort width = mr.ReadUInt16(); // width and height are 0 in SD images, and should be retrieved from the appropriate GRP file.
                    ushort height = mr.ReadUInt16();

                    uint frameptr = mr.ReadUInt32(); // pointer to an array of size [frames]


                    for (int layindex = 0; layindex < layers; layindex++)
                    {
                        //==================entryimg==================
                        uint ptr = mr.ReadUInt32(); // NULL if this layer does not exist
                        uint size = mr.ReadUInt32();
                        ushort ddswidth = mr.ReadUInt16();
                        ushort ddsheight = mr.ReadUInt16();
                        if(ptr == 0)
                        {
                            continue;
                        }

                        long lastptr = mr.BaseStream.Position;


                        if (ptr != 0xFFFFFFFF)
                        {
                            mr.BaseStream.Position = ptr;
                            //실제 이미지

                            byte[] td = mr.ReadBytes((int)size);

                            //if(version != 0x101)
                            //{
                            //    using (var image = Pfim.Pfim.FromStream(new MemoryStream(td)))
                            //    {
                            //        System.Drawing.Imaging.PixelFormat format;

                            //        // Convert from Pfim's backend agnostic image format into GDI+'s image format
                            //        switch (image.Format)
                            //        {
                            //            case ImageFormat.Rgba32:
                            //                format = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
                            //                break;
                            //            case ImageFormat.Rgb24:
                            //                format = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
                            //                break;
                            //            default:
                            //                // see the sample for more details
                            //                throw new NotImplementedException();
                            //        }

                            //        // Pin pfim's data array so that it doesn't get reaped by GC, unnecessary
                            //        // in this snippet but useful technique if the data was going to be used in
                            //        // control like a picture box
                            //        var handle = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
                            //        try
                            //        {
                            //            var data = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);
                            //            var bitmap = new System.Drawing.Bitmap(image.Width, image.Height, image.Stride, format, data);
                            //            bitmap.Save(@"D:\User\Desktop\새 폴더\" + animName.Replace("\\","_") + frameindex.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                            //        }
                            //        finally
                            //        {
                            //            handle.Free();
                            //        }
                            //    }
                            //}





                            if (version == 0x101 && layerstrs[layindex] == "teamcolor")
                            {
                                Texture2D _t;


                                int dxtHeaderOffset = 0x4;
                                _t = new Texture2D(MAPDRAWER.GraphicsDevice, ddswidth, ddsheight, false, SurfaceFormat.Alpha8);
                                _t.SetData(td, dxtHeaderOffset, td.Length - dxtHeaderOffset);

                                colorgrp = _t;
                            }
                            else
                            {
                                mr.BaseStream.Position = ptr + 0x54;
                                uint ddsType = mr.ReadUInt32();

                                SurfaceFormat surfaceFormat = SurfaceFormat.Dxt1;
                                switch (ddsType)
                                {
                                    case 0x35545844:
                                        surfaceFormat = SurfaceFormat.Dxt5;
                                        break;
                                    case 0x31545844:
                                        surfaceFormat = SurfaceFormat.Dxt1;
                                        break;
                                }

                                //DDS
                                if (layerstrs[layindex] == "diffuse")
                                {
                                    Texture2D _t;


                                    int dxtHeaderOffset = 0x80;
                                    _t = new Texture2D(MAPDRAWER.GraphicsDevice, ddswidth, ddsheight, false, surfaceFormat);
                                    _t.SetData(td, dxtHeaderOffset, td.Length - dxtHeaderOffset);

                                    maingrp = _t;
                                }
                                else if(layerstrs[layindex] == "teamcolor")
                                {
                                    Texture2D _t;


                                    int dxtHeaderOffset = 0x80;
                                    _t = new Texture2D(MAPDRAWER.GraphicsDevice, ddswidth, ddsheight, false, surfaceFormat);
                                    _t.SetData(td, dxtHeaderOffset, td.Length - dxtHeaderOffset);
                                    colorgrp = _t;
                                }
                            }

                        }
                        mr.BaseStream.Position = lastptr; 
                        //==================entryimg==================
                        //In version 0x0101, the player color mask is in a bitmap format,
                        //which is just "BMP " followed by width*height bytes,
                        //either 0x00 or 0xFF in a top-to-bottom row order. version 0x0202 uses only DDS files.
                    }
                    //==================entry==================

                    {
                        //==================frame==================
                        mr.BaseStream.Position = frameptr;
                        ushort x = mr.ReadUInt16(); // Coordinates of the top-left pixel of the frame
                        ushort y = mr.ReadUInt16();
                        ushort xoff = mr.ReadUInt16(); // X,Y offsets from the top left of the GRP frame -- value seems directly copied from each GRP
                        ushort yoff = mr.ReadUInt16();
                        ushort fwidth = mr.ReadUInt16(); // Dimensions, relative to the top-left pixel, of the frame
                        ushort fheight = mr.ReadUInt16();
                        ushort funk1 = mr.ReadUInt16(); // always 0? or 1?
                        ushort funk2 = mr.ReadUInt16(); // always 0?
                        //==================frame==================
                    }
                }
                else
                {
                    //==================entryref==================
                    uint refid = mr.ReadUInt32(); // image ID to refer to
                    uint unk1 = mr.ReadUInt32(); // always 0?
                    uint unk2 = mr.ReadUInt32(); // unknown values -- who knows
                    //==================entryref==================
                }






                MainGRP.Add(maingrp);
                ColorGRP.Add(colorgrp);
            }



            mr.Close();
        }




        /*
        struct header
        {
            uint magic; // "ANIM"
            ushort version; // Version? 0x0101 for SD, 0x0202 for HD2, 0x0204 for HD
            ushort unk2; // 0 -- more bytes for version?
            ushort layers;
            ushort entries;
            char layerstrs[10][32];

            // The following value is only present in Version 0x0101
            entry* images[entries]; // one pointer per entry
        };

        struct entry
        {
            ushort frames; // if frames == 0, it's an entryref and not this struct

            ushort unk2; // always 0xFFFF?
            ushort width; // width and height are 0 in SD images, and should be retrieved from the appropriate GRP file.
            ushort height;

            frame* frameptr; // pointer to an array of size [frames]

            entryimg img[header.layers];
        };

        struct entryimg
        {
            DDS* ptr; // NULL if this layer does not exist
            unsigned int size;
            ushort width;
            ushort height;
        };
        // In version 0x0101, the player color mask is in a bitmap format, which is just "BMP " followed by width*height bytes, either 0x00 or 0xFF in a top-to-bottom row order. version 0x0202 uses only DDS files.


        struct entryref
        {
            ushort frames; // necessarily 0 for this struct
                                   // These probably aren't ints, but w/e
            uint refid; // image ID to refer to
            uint unk1; // always 0?
            uint unk2; // unknown values -- who knows
        };

        struct frame
        {
            ushort x; // Coordinates of the top-left pixel of the frame
            ushort y;
            ushort xoff; // X,Y offsets from the top left of the GRP frame -- value seems directly copied from each GRP
            ushort yoff;
            ushort width; // Dimensions, relative to the top-left pixel, of the frame
            ushort height;
            ushort unk1; // always 0? or 1?
            ushort unk2; // always 0?
        };
        */
    }
}
