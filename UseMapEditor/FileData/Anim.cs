using Microsoft.Xna.Framework.Graphics;
using Pfim;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        public Anim()
        {
        }


        public static bool ReadAnim(byte[] bytes, string filename, int img = -1)
        {
            BinaryReader mr =  new BinaryReader(new MemoryStream(bytes));
            if (mr.BaseStream.Length == 0)
            {
                return false;
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


            for (int entryindex = 0; entryindex < entries; entryindex++)
            {
                bool IsSaveIcon = false;
                System.Drawing.Bitmap iconbitmap = null;
                if (version == 0x101)
                {
                    mr.BaseStream.Position = images[entryindex];
                    img = entryindex;
                }


                string savepath;
                string savefolder = "";
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

                        if(!(layerstrs[layindex] == "diffuse" || layerstrs[layindex] == "teamcolor"))
                        {
                            continue;
                        }



                        long lastptr = mr.BaseStream.Position;


                        if (ptr != 0xFFFFFFFF)
                        {
                            mr.BaseStream.Position = ptr;
                            //실제 이미지


                            if (version == 0x101 && layerstrs[layindex] == "teamcolor")
                            {
                                mr.ReadUInt32();
                                byte[] td = mr.ReadBytes((int)size - 4);

                                BinaryWriter binaryWriter = new BinaryWriter(new MemoryStream());
                                binaryWriter.Write(FileData.BMP.bmpheader);
                                binaryWriter.Write(td);

                                binaryWriter.BaseStream.Position = 0x2;
                                binaryWriter.Write((uint)binaryWriter.BaseStream.Length);

                                binaryWriter.BaseStream.Position = 0x12;
                                binaryWriter.Write((int)ddswidth);
                                binaryWriter.Write((int)ddsheight);

                                //bmp
                                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(binaryWriter.BaseStream);
                                bitmap.MakeTransparent(System.Drawing.Color.Black);
                                savepath = filename + entryindex;

                                if (!System.IO.Directory.Exists(savepath))
                                {
                                    System.IO.Directory.CreateDirectory(savepath);
                                }

                                savefolder = savepath + "\\";
                                savepath = savepath + "\\" + layerstrs[layindex] + ".png";
                                bitmap.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
                                bitmap.Save(savepath, System.Drawing.Imaging.ImageFormat.Png);
                                binaryWriter.Close();
                            }
                            else
                            {
                                byte[] td = mr.ReadBytes((int)size);
                                using (var image = Pfim.Pfim.FromStream(new MemoryStream(td)))
                                {
                                    System.Drawing.Imaging.PixelFormat format;

                                    // Convert from Pfim's backend agnostic image format into GDI+'s image format
                                    switch (image.Format)
                                    {
                                        case ImageFormat.Rgba32:
                                            format = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
                                            break;
                                        case ImageFormat.Rgb24:
                                            format = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
                                            break;
                                        default:
                                            // see the sample for more details
                                            throw new NotImplementedException();
                                    }

                                    // Pin pfim's data array so that it doesn't get reaped by GC, unnecessary
                                    // in this snippet but useful technique if the data was going to be used in
                                    // control like a picture box
                                    var handle = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
                                    try
                                    {
                                        var data = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);
                                        var bitmap = new System.Drawing.Bitmap(image.Width, image.Height, image.Stride, format, data);

                                        if (version == 0x101)
                                        {

                                            bitmap.MakeTransparent(System.Drawing.Color.Black);
                                            savepath = filename + entryindex;

                                            if (!System.IO.Directory.Exists(savepath))
                                            {
                                                System.IO.Directory.CreateDirectory(savepath);
                                            }

                                            savefolder = savepath + "\\";
                                            savepath = savepath + "\\" + layerstrs[layindex] + ".png";
                                            IsSaveIcon = true;
                                            iconbitmap = bitmap;
                                        }
                                        else
                                        {
                                            savefolder = filename;
                                            savepath = filename + layerstrs[layindex] + ".png";
                                        }
                                        if(layerstrs[layindex] == "teamcolor")
                                        {
                                            bitmap.MakeTransparent(System.Drawing.Color.Black);
                                        }


                                        bitmap.Save(savepath, System.Drawing.Imaging.ImageFormat.Png);
                                    }
                                    finally
                                    {
                                        handle.Free();
                                    }
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

                    //frames
                    //width
                    //height   

                    BinaryWriter bw = new BinaryWriter(new FileStream(savefolder + "framedata", FileMode.Create));
                    bw.Write(frames);
                    bw.Write(width);
                    bw.Write(height);

                    ushort[] iconframe = new ushort[4];
                    {
                        //==================frame==================
                        mr.BaseStream.Position = frameptr;
                        for (int findex = 0; findex < frames; findex++)
                        {
                            ushort x = mr.ReadUInt16(); // Coordinates of the top-left pixel of the frame
                            ushort y = mr.ReadUInt16();
                            ushort xoff = mr.ReadUInt16(); // X,Y offsets from the top left of the GRP frame -- value seems directly copied from each GRP
                            ushort yoff = mr.ReadUInt16();
                            ushort fwidth = mr.ReadUInt16(); // Dimensions, relative to the top-left pixel, of the frame
                            ushort fheight = mr.ReadUInt16();
                            ushort funk1 = mr.ReadUInt16(); // always 0? or 1?
                            ushort funk2 = mr.ReadUInt16(); // always 0?

                            bw.Write(x);
                            bw.Write(y);
                            bw.Write(xoff);
                            bw.Write(yoff);
                            bw.Write(fwidth);
                            bw.Write(fheight);
                            bw.Write(funk1);
                            bw.Write(funk2);


                            if (IsSaveIcon & findex == 0)
                            {
                                iconframe[0] = x;
                                iconframe[1] = y;
                                iconframe[2] = fwidth;
                                iconframe[3] = fheight;
                            }
                        }
                        //==================frame==================
                    }

                    bw.Close();



                    //아이콘 만들기
                    if (IsSaveIcon)
                    {
                        string tsavepath = AppDomain.CurrentDomain.BaseDirectory + @"CascData\icon";
                        if (!System.IO.Directory.Exists(tsavepath))
                        {
                            System.IO.Directory.CreateDirectory(tsavepath);
                        }

                        Rectangle cropRect = new Rectangle(iconframe[0], iconframe[1], iconframe[2], iconframe[3]);
                        Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

                        using (Graphics g = Graphics.FromImage(target))
                        {
                            g.DrawImage(iconbitmap, new Rectangle(0, 0, target.Width, target.Height),
                                             cropRect,
                                             GraphicsUnit.Pixel);
                        }

                        int grp = (ushort)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.images, "GRP File", entryindex).Data;


                        tsavepath += "\\" + grp + ".png";


                        target.Save(tsavepath, System.Drawing.Imaging.ImageFormat.Png);
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
            }
            mr.Close();
            return true;
        }


    }
}
