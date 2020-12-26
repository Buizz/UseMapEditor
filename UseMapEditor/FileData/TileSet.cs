using Microsoft.Xna.Framework;
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
        private Dictionary<FileData.TileSet.TileType, List<Texture2D>> SDTileSet;
        private Dictionary<FileData.TileSet.TileType, List<Texture2D>> HDTileSet;
        private Dictionary<FileData.TileSet.TileType, List<Texture2D>> CBTileSet;
        public Dictionary<FileData.TileSet.TileType, List<Texture2D>> GetTileDic(Control.MapEditor.DrawType drawType)
        {
            switch (drawType)
            {
                case Control.MapEditor.DrawType.SD:
                    return SDTileSet;
                case Control.MapEditor.DrawType.HD:
                    return HDTileSet;
                case Control.MapEditor.DrawType.CB:
                    return CBTileSet;
            }
            return null;
        }



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
            //0x0001 = Edge?
            //0x0004 = Cliff?
            //0x0040 = Creep
            //0x0080 = Unbuildable
            //0x0n00 = Deprecated ground height?
            //0x1000 = Sprites.dat Reference
            //0x2000 = Units.dat Reference(unit sprite)
            //0x4000 = Overlay is Flipped
            //0x8000 = Buildable for Start Location and Beacons
            public ushort Left_OverlayID;
            public ushort Top;
            public ushort Right_DoodadGroupString;
            public ushort Bottom;
            public ushort Unknown1_DoodadID;
            public ushort EdgeUp_Width;
            public ushort Unknown2_Height;
            public ushort EdgeDown;
            public ushort[] tiles;
        }
        public class DoodadPallet
        {
            public ushort dddID;
            public string tblString;
            public ushort dddGroup;
            public ushort dddWidth;
            public ushort dddHeight;

            public ushort dddOverlayID;
            public ushort dddFlags;
            //0x0001 = Edge?
            //0x0004 = Cliff?
            //0x0040 = Creep
            //0x0080 = Unbuildable
            //0x0n00 = Deprecated ground height?
            //0x1000 = Sprites.dat Reference
            //0x2000 = Units.dat Reference(unit sprite)
            //0x4000 = Overlay is Flipped
            //0x8000 = Buildable for Start Location and Beacons
        }



        public struct vf4
        {
            ushort[] tiles;
        }

        public Dictionary<TileType, cv5[]> cv5data;
        public Dictionary<TileType, Dictionary<ushort, DoodadPallet>> DoodadPallets;


        public void TextureLoad(MapDrawer mapDrawer)
        {
            SDTileSet = new Dictionary<TileType, List<Texture2D>>();
            HDTileSet = new Dictionary<TileType, List<Texture2D>>();
            CBTileSet = new Dictionary<TileType, List<Texture2D>>();

            SDTileSetMiniMap = new Dictionary<TileType, List<Color>>();
            HDTileSetMiniMap = new Dictionary<TileType, List<Color>>();
            CBTileSetMiniMap = new Dictionary<TileType, List<Color>>();
            foreach (TileType settings in Enum.GetValues(typeof(TileType)))
            {
                {
                    List<Texture2D> texture2Ds = new List<Texture2D>();
                    List<Color> colrlist = new List<Color>();

                    ReadTile(mapDrawer, settings, "SD", texture2Ds, colrlist);
                    SDTileSetMiniMap.Add(settings, colrlist);
                    SDTileSet.Add(settings, texture2Ds);
                }
                {
                    List<Texture2D> texture2Ds = new List<Texture2D>();
                    List<Color> colrlist = new List<Color>();

                    ReadTile(mapDrawer, settings, "HD", texture2Ds, colrlist);

                    HDTileSetMiniMap.Add(settings, colrlist);
                    HDTileSet.Add(settings, texture2Ds);
                }
                {
                    List<Texture2D> texture2Ds = new List<Texture2D>();
                    List<Color> colrlist = new List<Color>();

                    ReadTile(mapDrawer, settings, "CB", texture2Ds, colrlist);

                    CBTileSetMiniMap.Add(settings, colrlist);
                    CBTileSet.Add(settings, texture2Ds);
                }
            }
        }


        public TileSet()
        {
            cv5data = new Dictionary<TileType, cv5[]>();
            DoodadPallets = new Dictionary<TileType, Dictionary<ushort, DoodadPallet>>();

            foreach (TileType tileType in Enum.GetValues(typeof(TileType)))
            {
                {
                    string fname = AppDomain.CurrentDomain.BaseDirectory + $"Data\\TileSet\\{tileType.ToString()}.cv5";

                    if (Global.Setting.Vals[Global.Setting.Settings.Program_GRPLoad] == "true")
                    {
                        fname = AppDomain.CurrentDomain.BaseDirectory + $"CascData\\TileSet\\{tileType.ToString()}.cv5";
                    }


                    BinaryReader br = new BinaryReader(new MemoryStream(File.ReadAllBytes(fname)));



                    int cv5count = (int)br.BaseStream.Length / 52;
                    cv5[] _cv5data = new cv5[cv5count];
                    Dictionary<ushort, DoodadPallet> DoodadDic = new Dictionary<ushort, DoodadPallet>();
                    for (int i = 0; i < cv5count; i++)
                    {
                        cv5 _cv5 = new cv5();
                        _cv5.Index = br.ReadUInt16();
                        _cv5.Flags = br.ReadUInt16();
                        _cv5.Left_OverlayID = br.ReadUInt16();
                        _cv5.Top = br.ReadUInt16();
                        _cv5.Right_DoodadGroupString = br.ReadUInt16();
                        _cv5.Bottom = br.ReadUInt16();
                        _cv5.Unknown1_DoodadID = br.ReadUInt16();
                        _cv5.EdgeUp_Width = br.ReadUInt16();
                        _cv5.Unknown2_Height = br.ReadUInt16();
                        _cv5.EdgeDown = br.ReadUInt16();
                        _cv5.tiles = new ushort[16];

                        for (int p = 0; p < 16; p++)
                        {
                            _cv5.tiles[p] = br.ReadUInt16();
                        }
                        _cv5data[i] = _cv5;



                        if(_cv5.Index == 1)
                        {
                            //두데드
                            ushort dddID = _cv5.Unknown1_DoodadID;
                            ushort tblString = _cv5.Right_DoodadGroupString;
                            ushort dddWidth = _cv5.EdgeUp_Width;
                            ushort dddHeight = _cv5.Unknown2_Height;

                            ushort dddOverlayID = _cv5.Left_OverlayID;
                            ushort dddFlags = _cv5.Flags;


                            ushort dddGroup = (ushort)i;
                            if (!DoodadDic.ContainsKey(dddID))
                            {
                                DoodadPallet doodadPallet = new DoodadPallet();

                                doodadPallet.dddID = dddID;
                                doodadPallet.tblString = Global.WindowTool.stat_txt.Strings[tblString].val1;
                                doodadPallet.dddWidth = dddWidth;
                                doodadPallet.dddHeight = dddHeight;
                                doodadPallet.dddOverlayID = dddOverlayID;
                                doodadPallet.dddFlags = dddFlags;
                                doodadPallet.dddGroup = dddGroup;


                                DoodadDic.Add(dddID, doodadPallet);
                            }
                        }



                    }

                    cv5data.Add(tileType, _cv5data);
                    DoodadPallets.Add(tileType, DoodadDic);

                    br.Close();
                }
                {
                    string fname = AppDomain.CurrentDomain.BaseDirectory + $"Data\\TileSet\\{tileType.ToString()}.vf4";

                    if (Global.Setting.Vals[Global.Setting.Settings.Program_GRPLoad] == "true")
                    {
                        fname = AppDomain.CurrentDomain.BaseDirectory + $"CascData\\TileSet\\{tileType.ToString()}.vf4";
                    }

                    BinaryReader br = new BinaryReader(new MemoryStream(File.ReadAllBytes(fname)));

                    br.Close();
                }
            }
        }









        public Texture2D GetTile(Control.MapEditor.DrawType drawType, TileType tileType, ushort MTXM)
        {
            int group = (MTXM >> 4);
            int index = (MTXM & 0xf);

            return GetTileDic(drawType)[tileType][cv5data[tileType][group].tiles[index]];
        }

        public Texture2D GetTile(Control.MapEditor.DrawType drawType, TileType tileType, ushort group, ushort index)
        {
            return GetTileDic(drawType)[tileType][cv5data[tileType][group].tiles[index]];
        }

        public bool IsBlack(TileType tileType, ushort group, ushort index)
        {
            return (cv5data[tileType][group].tiles[index] == 0);
        }



        private Dictionary<FileData.TileSet.TileType, List<Color>> SDTileSetMiniMap;
        private Dictionary<FileData.TileSet.TileType, List<Color>> HDTileSetMiniMap;
        private Dictionary<FileData.TileSet.TileType, List<Color>> CBTileSetMiniMap;
        public Color GetTileColor(Control.MapEditor.DrawType drawType , UseMapEditor.FileData.TileSet.TileType tileType, ushort MTXM)
        {
            int group = (MTXM >> 4);
            int index = (MTXM & 0xf);
            switch (drawType)
            {
                case Control.MapEditor.DrawType.SD:
                    return SDTileSetMiniMap[tileType][cv5data[tileType][group].tiles[index]];
                case Control.MapEditor.DrawType.HD:
                    return HDTileSetMiniMap[tileType][cv5data[tileType][group].tiles[index]];
                case Control.MapEditor.DrawType.CB:
                    return CBTileSetMiniMap[tileType][cv5data[tileType][group].tiles[index]];
            }
            return Color.Black;
        }



        private void ReadTile(MapDrawer mapDrawer, TileType tileType, string _fname, List<Texture2D> texture2Ds, List<Color> MiniMapColor)
        {
            string fname = AppDomain.CurrentDomain.BaseDirectory + $"CascData\\{_fname}\\TileSet\\{tileType.ToString()}.dds.vr4";

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

                    MiniMapColor.Add(new Color(textureData[0]));
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


                    MiniMapColor.Add(Dxt1.DecompressBlock(8,8, width, textureData));
                    texture2Ds.Add(texture);
                }
            }


            br.Close();
        }
    }
}
