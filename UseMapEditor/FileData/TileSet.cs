using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseMapEditor.MonoGameControl;

namespace UseMapEditor.FileData
{
    public class TileSet
    {
        public enum TileType{
            badlands,
            platform,
            install,
            AshWorld,
            Jungle,
            Desert,
            Ice,
            Twilight
        }

        public enum Flags
        {
            Edge,
            Cliff,
            Creep,
            Unbuildable,
            Deprecated,
            BuildableStartLocation
        }

        public struct cv5
        {
            public ushort Index;
            public ushort Flags;
            public ushort Left;
            public ushort Top;
            public ushort Right;
            public ushort Bottom;
            public ushort Unknown1;
            public ushort EdgeUp;
            public ushort Unknown2;
            public ushort EdgeDown;
            public ushort[] tiles;
        }
        public struct vf4
        {
            ushort[] tiles;
        }

        public Dictionary<TileType,cv5[]> cv5data;


        public TileSet()
        {

            cv5data = new Dictionary<TileType, cv5[]>();

            foreach (TileType tileType in Enum.GetValues(typeof(TileType)))
            {
                {
                    string fname = $"CascData\\TileSet\\{tileType.ToString()}.cv5";
                    BinaryReader br = new BinaryReader(new MemoryStream(File.ReadAllBytes(fname)));

                    int cv5count = (int)br.BaseStream.Length / 52;
                    cv5[] _cv5data = new cv5[cv5count];
                    for (int i = 0; i < cv5count; i++)
                    {
                        cv5 _cv5 = new cv5();
                        _cv5.Index = br.ReadUInt16();
                        _cv5.Flags = br.ReadUInt16();
                        _cv5.Left = br.ReadUInt16();
                        _cv5.Top = br.ReadUInt16();
                        _cv5.Right = br.ReadUInt16();
                        _cv5.Bottom = br.ReadUInt16();
                        _cv5.Unknown1 = br.ReadUInt16();
                        _cv5.EdgeUp = br.ReadUInt16();
                        _cv5.Unknown2 = br.ReadUInt16();
                        _cv5.EdgeDown = br.ReadUInt16();
                        _cv5.tiles = new ushort[16];

                        for (int p = 0; p < 16; p++)
                        {
                            _cv5.tiles[p] = br.ReadUInt16();
                        }
                        _cv5data[i] = _cv5;
                    }

                    cv5data.Add(tileType, _cv5data);

                    br.Close();
                }
                {
                    string fname = $"CascData\\TileSet\\{tileType.ToString()}.vf4";
                    BinaryReader br = new BinaryReader(new MemoryStream(File.ReadAllBytes(fname)));

                    br.Close();
                }
            }
             
        }
        public Texture2D GetTile(TileType tileType,Dictionary<TileType, List<Texture2D>> tlist, int vMTXM)
        {
            int group = (vMTXM >> 4);
            int index = (vMTXM & 0xf);

            return tlist[tileType][cv5data[tileType][group].tiles[index]];
        }




        public static void ReadTileSet(MapDrawer mapDrawer)
        {
            foreach (TileType settings in Enum.GetValues(typeof(TileType)))
            {
                {
                    List<Texture2D> texture2Ds = new List<Texture2D>();

                    ReadTile(mapDrawer, settings, "SD", texture2Ds);

                    mapDrawer.SDTileSet.Add(settings, texture2Ds);
                }
                {
                    List<Texture2D> texture2Ds = new List<Texture2D>();

                    ReadTile(mapDrawer, settings, "HD", texture2Ds);

                    mapDrawer.HDTileSet.Add(settings, texture2Ds);
                }
                {
                    List<Texture2D> texture2Ds = new List<Texture2D>();

                    ReadTile(mapDrawer, settings, "CB", texture2Ds);

                    mapDrawer.CBTileSet.Add(settings, texture2Ds);
                }
            }
        }

        private static void ReadTile(MapDrawer mapDrawer, TileType tileType, string _fname, List<Texture2D> texture2Ds)
        {
            string fname = $"CascData\\{_fname}\\TileSet\\{tileType.ToString()}.dds.vr4";

            BinaryReader br = new BinaryReader(new MemoryStream(File.ReadAllBytes(fname)));


            uint filesize = br.ReadUInt32();
            ushort frame = br.ReadUInt16(); //count
            ushort unknown = br.ReadUInt16();// (file version ?); -- value appears to always be 0x1001 in the files I've seen.

            if (unknown == 0x1011)
            {
                //SD
                ushort width = br.ReadUInt16();
                ushort height = br.ReadUInt16();

                uint[] pallet = new uint[400];
                for (int i = 0; i < 256; i++)
                {
                    pallet[i] = br.ReadUInt32() + 0xFF000000;
                }



                for (int i = 0; i < frame; i++)
                {
                    uint[] textureData = new uint[1024];

                    for (int p = 0; p < 1024; p++)
                    {
                        byte index = br.ReadByte();

                        textureData[p] = pallet[index];
                    }


                    Texture2D texture = new Texture2D(mapDrawer.GraphicsDevice, 32, 32, false, SurfaceFormat.Color);
                    texture.SetData(textureData, 0, 1024);


                    texture2Ds.Add(texture);
                }
            }
            else if(unknown == 0x1002)
            {
                //HD2
                for (int i = 0; i < frame; i++)
                {            //File Entry:
                    uint unk = br.ReadUInt32(); // --always zero ?
                    ushort width = br.ReadUInt16();
                    ushort height = br.ReadUInt16();
                    uint size = br.ReadUInt32();



                    byte[] textureData = br.ReadBytes((int)size);

                    int dxtHeaderOffset = 0x80;
                    Texture2D texture = new Texture2D(mapDrawer.GraphicsDevice, width, height, false, SurfaceFormat.Dxt1);
                    texture.SetData(textureData, dxtHeaderOffset, textureData.Length - dxtHeaderOffset);


                    texture2Ds.Add(texture);
                }
            }


            br.Close();
        }
    }
}
