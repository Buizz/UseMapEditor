using Microsoft.Office.Interop.Excel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NAudio.SoundFont;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pfim;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Shapes;
using UseMapEditor.Casc;
using UseMapEditor.Control;
using UseMapEditor.MonoGameControl;
using static UseMapEditor.FileData.DatFile.CDatFile.CParamater;
using static UseMapEditor.FileData.TileSet;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace UseMapEditor.FileData
{
    public class TileSet
    {
        public static double tinyScale = 0.20;
        public static double smallScale = 0.5;
        public static double miniBlockSize = 0.125;




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
        public Dictionary<FileData.TileSet.TileType, AtlasTileSet> GetTileAltasDic(Control.MapEditor.DrawType drawType)
        {
            switch (drawType)
            {
                case Control.MapEditor.DrawType.SD:
                    return SDAtlasTileSet;
                case Control.MapEditor.DrawType.HD:
                    return HDAtlasTileSet;
                case Control.MapEditor.DrawType.CB:
                    return CBAtlasTileSet;
            }
            return null;
        }

        public TileAtlasPainter tileAtlasPainter;

        private Dictionary<FileData.TileSet.TileType, AtlasTileSet> SDAtlasTileSet;
        private Dictionary<FileData.TileSet.TileType, AtlasTileSet> HDAtlasTileSet;
        private Dictionary<FileData.TileSet.TileType, AtlasTileSet> CBAtlasTileSet;
        public class AtlasTileSet
        {
   
            public Texture2D texture2D;
            public Texture2D smalltexture2D;

    
            public Texture2D GetTexture()
            {
                return texture2D;
            }

        


            public int framesize;
            public int length;

            public void SetSize(int area)
            {
                length = (int) Math.Ceiling(Math.Sqrt(area));
            }

            public Rectangle GetRect(int c, double scale = 1)
            {
                int x = c % length;
                int y = c / length;
                int tframesize = framesize;

                if (smallScale > scale)
                {
                    tframesize = (int)(tframesize * miniBlockSize);
                }

                return new Rectangle(x * tframesize, y * tframesize, tframesize, tframesize);
            }
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


        public class DoodadPalletGroup
        {
            public string groupname;
            public List<int> dddids = new List<int>();
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
            public ushort[] flags;
            public bool IsWall;
            public bool IsGround;

            //16 Words - MiniTile Flags:
            //0x0001 - Walkable
            //0x0002 - Mid
            //0x0004 - High(Mid and High unchecked = Low)
            //0x0008 - Blocks View
            //0x0010 - Ramp - Appears on the middle minitiles of most ramps/stairs.
            //Rest unknown/unused.;
        }

        public Dictionary<TileType, Dictionary<ushort, ushort>> TileFlipList;

        public Dictionary<TileType, List<ISOMTile>> ISOMdata;
        public Dictionary<TileType, List<ISOMTile>> CustomISOMdata;


        public Dictionary<TileType, cv5[]> cv5data;
        public Dictionary<TileType, vf4[]> vf4data;
        public Dictionary<TileType, Dictionary<ushort, DoodadPallet>> DoodadPallets;
        public Dictionary<TileType, List<DoodadPalletGroup>> DoodadGroups;

        private Dictionary<TileType, Dictionary<ushort, ushort>>  MagaTileToMTXM;

        public void TextureLoad(MapDrawer mapDrawer)
        {
            SDTileSet = new Dictionary<TileType, List<Texture2D>>();
            HDTileSet = new Dictionary<TileType, List<Texture2D>>();
            CBTileSet = new Dictionary<TileType, List<Texture2D>>();

            SDAtlasTileSet = new Dictionary<TileType, AtlasTileSet>();
            HDAtlasTileSet = new Dictionary<TileType, AtlasTileSet>();
            CBAtlasTileSet = new Dictionary<TileType, AtlasTileSet>();

            SDTileSetMiniMap = new Dictionary<TileType, List<Color>>();
            HDTileSetMiniMap = new Dictionary<TileType, List<Color>>();
            CBTileSetMiniMap = new Dictionary<TileType, List<Color>>();

            SDTileSetMiniMapPicker = new Dictionary<TileType, Dictionary<Color, ushort>>();
            HDTileSetMiniMapPicker = new Dictionary<TileType, Dictionary<Color, ushort>>();
            CBTileSetMiniMapPicker = new Dictionary<TileType, Dictionary<Color, ushort>>();


            tileAtlasPainter = new TileAtlasPainter(this, mapDrawer.GraphicsDevice);

            foreach (TileType settings in Enum.GetValues(typeof(TileType)))
            {
                {
                    List<Texture2D> texture2Ds = new List<Texture2D>();
                    List<Color> colrlist = new List<Color>();
                    Dictionary<Color, ushort> colordic = new Dictionary<Color, ushort>();
                    //ReadTile(mapDrawer, settings, "SD", texture2Ds, colrlist);

                    SDAtlasTileSet.Add(settings, ReadTileAltas(mapDrawer, settings, "SD", colrlist));
                    for (ushort i = 0; i < colrlist.Count; i++)
                    {
                        if (!colordic.ContainsKey(colrlist[i]))
                        {
                            //사용가능한 메가타일이면
                            if (MagaTileToMTXM[settings].ContainsKey(i))
                            {
                                colordic.Add(colrlist[i], MagaTileToMTXM[settings][i]);
                            }
                        }
                    }

                    SDTileSetMiniMap.Add(settings, colrlist);
                    SDTileSetMiniMapPicker.Add(settings, colordic);
                    SDTileSet.Add(settings, texture2Ds);
                }
                {
                    List<Texture2D> texture2Ds = new List<Texture2D>();
                    List<Color> colrlist = new List<Color>();
                    Dictionary<Color, ushort> colordic = new Dictionary<Color, ushort>();
                    //ReadTile(mapDrawer, settings, "HD", texture2Ds, colrlist);

                    HDAtlasTileSet.Add(settings, ReadTileAltas(mapDrawer, settings, "HD", colrlist));
                    for (ushort i = 0; i < colrlist.Count; i++)
                    {
                        if (!colordic.ContainsKey(colrlist[i]))
                        {
                            //사용가능한 메가타일이면
                            if (MagaTileToMTXM[settings].ContainsKey(i))
                            {
                                //colordic.Add(colrlist[i], MagaTileToMTXM[settings][i]);
                            }
                        }
                    }

                    HDTileSetMiniMap.Add(settings, colrlist);
                    HDTileSetMiniMapPicker.Add(settings, colordic);
                    HDTileSet.Add(settings, texture2Ds);
                }
                {
                    List<Texture2D> texture2Ds = new List<Texture2D>();
                    List<Color> colrlist = new List<Color>();
                    Dictionary<Color, ushort> colordic = new Dictionary<Color, ushort>();
                    //ReadTile(mapDrawer, settings, "CB", texture2Ds, colrlist);

                    CBAtlasTileSet.Add(settings, ReadTileAltas(mapDrawer, settings, "CB", colrlist));
                    for (ushort i = 0; i < colrlist.Count; i++)
                    {
                        if (!colordic.ContainsKey(colrlist[i]))
                        {
                            //사용가능한 메가타일이면
                            if (MagaTileToMTXM[settings].ContainsKey(i))
                            {
                                //colordic.Add(colrlist[i], MagaTileToMTXM[settings][i]);
                            }
                        }
                    }

                    CBTileSetMiniMap.Add(settings, colrlist);
                    CBTileSetMiniMapPicker.Add(settings, colordic);
                    CBTileSet.Add(settings, texture2Ds);
                }
            }
        }

        public TileSet()
        {
            MagaTileToMTXM = new Dictionary<TileType, Dictionary<ushort, ushort>>();

            cv5data = new Dictionary<TileType, cv5[]>();
            vf4data = new Dictionary<TileType, vf4[]>();
            DoodadPallets = new Dictionary<TileType, Dictionary<ushort, DoodadPallet>>();
            DoodadGroups = new Dictionary<TileType, List<DoodadPalletGroup>>();

            foreach (TileType tileType in Enum.GetValues(typeof(TileType)))
            {
                MagaTileToMTXM[tileType] = new Dictionary<ushort, ushort>();

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
                            ushort magaindex = br.ReadUInt16();

                            if (!MagaTileToMTXM[tileType].ContainsKey(magaindex))  MagaTileToMTXM[tileType].Add(magaindex, (ushort)(i * 16 + p));
                         

                            _cv5.tiles[p] = magaindex;
                        }



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
                                if(tblString != 0)
                                {
                                    doodadPallet.tblString = Global.WindowTool.stat_txt.Strings[tblString - 1].val1;
                                    //doodadPallet.tblString = Global.WindowTool.stat_txt.Strings[tblString + 1].val1;
                                    doodadPallet.dddWidth = dddWidth;
                                    doodadPallet.dddHeight = dddHeight;
                                    doodadPallet.dddOverlayID = dddOverlayID;
                                    doodadPallet.dddFlags = dddFlags;
                                    doodadPallet.dddGroup = dddGroup;


                                    DoodadDic.Add(dddID, doodadPallet);
                                }
                            }
                        }



                        _cv5data[i] = _cv5;
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
                    int vf4count = (int)br.BaseStream.Length / 32;
                    vf4[] _vf4data = new vf4[vf4count];

                    for (int i = 0; i < vf4count; i++)
                    {
                        int walkflag = 0;
                        vf4 _vf4 = new vf4();
                        _vf4.flags = new ushort[16];

                        for (int x = 0; x < 16; x++)
                        {
                            ushort flag = br.ReadUInt16();

                            _vf4.flags[x] = flag;
                            if((flag & 0b1) > 0)
                            {
                                walkflag += 1;
                            }
                        }
                        if (walkflag == 16)
                        {
                            //전체가 뚫린것
                            _vf4.IsGround = true;
                        }
                        if (walkflag == 0)
                        {
                            //전체가 막힌것
                            _vf4.IsWall = true;
                        }

                        _vf4data[i] = _vf4;
                    }



                    vf4data.Add(tileType, _vf4data);
                    br.Close();
                }
                {
                    List<DoodadPalletGroup> doodadPalletGroups = new List<DoodadPalletGroup>();
                    List<DoodadPallet> doodad = DoodadPallets[tileType].Values.ToList();
                    for (int i = 0; i < doodad.Count; i++)
                    {
                        string groupname = doodad[i].tblString;

                        DoodadPalletGroup doodadPalletGroup = doodadPalletGroups.Find((x) => x.groupname == groupname);
                        if(doodadPalletGroup == null)
                        {
                            doodadPalletGroup = new DoodadPalletGroup();
                            doodadPalletGroup.groupname = groupname;
                            doodadPalletGroups.Add(doodadPalletGroup);
                        }


                        doodadPalletGroup.dddids.Add(doodad[i].dddID);

                    }
                    DoodadGroups.Add(tileType, doodadPalletGroups);
                }
            }
            LoadPalletData();
        }

        private void ReleaseExcelObject(object obj)
        {
            try
            {
                if (obj != null)
                {
                    Marshal.ReleaseComObject(obj);
                    obj = null;
                }
            }
            catch (Exception ex)
            {
                obj = null;
                throw ex;
            }
            finally
            {
                GC.Collect();
            }
        }


        private bool IsTestDataCreate = false;
        private void LoadPalletData()
        {
            ISOMdata = new Dictionary<TileType, List<ISOMTile>>();
            TileFlipList = new Dictionary<TileType, Dictionary<ushort, ushort>>();


            foreach (var cv in cv5data)
            {
                cv5[] cv5s = cv.Value;

                if (IsTestDataCreate)
                {

                    string filepath = AppDomain.CurrentDomain.BaseDirectory + "TEST\\" + cv.Key.ToString() + "_TerrainInfor.xlsx";
                    if (File.Exists(filepath))
                    {
                        File.Delete(filepath);
                    }

                    Microsoft.Office.Interop.Excel.Application application;
                    application = new Microsoft.Office.Interop.Excel.Application();
                    application.Visible = false;
                    Workbook workBook = null;



                    Worksheet workSheet = null;


                    workBook = application.Workbooks.Add();
                    //마지막 시트 뒤에 새로운 시트 추가
                    //workSheet = workBook.Worksheets.Add(After: workBook.Worksheets.Item[workBook.Worksheets.Count]);
                    workSheet = workBook.Worksheets.Item["Sheet1"];
                    //시트 이름 변경
                    workSheet.Name = "Terrain";



                    string[] header = {"NO","Index","Bottom","EdgeDown","EdgeUp_Width","Flags","Left_OverlayID","Right_DoodadGroupString","Top","Unknown1_DoodadID","Unknown2_Height"};
                    {
                        int _i = 1;
                        foreach (var h in header)
                        {
                            //헤더 설정
                            Range cell = workSheet.Cells[1, _i++];
                            cell.Value = h;
                        }
                    }
                    for (int i = 0; i < cv5s.Length; i++)
                    {
                        {
                            Range cell = workSheet.Cells[i + 2, 1];
                            cell.Value = i;
                        }

                        {
                            Range cell = workSheet.Cells[i + 2, 2];
                            cell.Value = cv5s[i].Index;
                        }
                        {
                            Range cell = workSheet.Cells[i + 2, 3];
                            cell.Value = cv5s[i].Bottom;
                        }
                        {
                            Range cell = workSheet.Cells[i + 2, 4];
                            cell.Value = cv5s[i].EdgeDown;
                        }
                        {
                            Range cell = workSheet.Cells[i + 2, 5];
                            cell.Value = cv5s[i].EdgeUp_Width;
                        }
                        {
                            Range cell = workSheet.Cells[i + 2, 6];
                            cell.Value = "b" + Convert.ToString(cv5s[i].Flags, 2).ToString().PadLeft(16, '0');
                        }
                        {
                            Range cell = workSheet.Cells[i + 2, 7];
                            cell.Value = cv5s[i].Left_OverlayID;
                        }
                        {
                            Range cell = workSheet.Cells[i + 2, 8];
                            cell.Value = cv5s[i].Right_DoodadGroupString;
                        }
                        {
                            Range cell = workSheet.Cells[i + 2, 9];
                            cell.Value = cv5s[i].Top;
                        }
                        {
                            Range cell = workSheet.Cells[i + 2, 10];
                            cell.Value = cv5s[i].Unknown1_DoodadID;
                        }
                        {
                            Range cell = workSheet.Cells[i + 2, 11];
                            cell.Value = cv5s[i].Unknown2_Height;
                        }

                    }

                    workBook.SaveAs(filepath);

                    //변경점 저장하면서 닫기
                    workBook.Close(true);
                    //Excel 프로그램 종료
                    application.Quit();
                    //오브젝트 해제
                    ReleaseExcelObject(workBook);
                    ReleaseExcelObject(application);
                }

                string fname = AppDomain.CurrentDomain.BaseDirectory + $"Data\\TileSet\\ISOM\\{cv.Key.ToString()}.json";

                // Json 파일 읽기
                if (File.Exists(fname))
                {
                    using (StreamReader file = File.OpenText(fname))
                    {
                        using (JsonTextReader reader = new JsonTextReader(file))
                        {
                            JArray json = (JArray)JToken.ReadFrom(reader);


                            List<ISOMTile> isoms = new List<ISOMTile>();

                            foreach (JObject jobject in json)
                            {
                                ISOMTile isom = new ISOMTile(jobject, this, cv.Key);
                                isoms.Add(isom);

                            }

                            //ISOM 돌면서 타일 관계 다시 정리하기.
                            foreach (var isomitem in isoms)
                            {
                                foreach (var ctile in isomitem.connectedtilenamelist)
                                {
                                    ISOMTile cisom = isoms.Find(x => x.name == ctile);
                                    isomitem.ConnectedAllTile.Add(cisom);
                                    if (Math.Floor(isomitem.elevation) == Math.Floor(cisom.elevation + 1))
                                    {
                                        //더 낮은 지형
                                        isomitem.ConnectLowTile = cisom;
                                    }
                                    else if (Math.Floor(isomitem.elevation) == Math.Floor(cisom.elevation - 1))
                                    {
                                        //더 낮은 지형
                                        isomitem.ConnectHighTile = cisom;

                                        if(isomitem.cliff_down.PartLength > 0)
                                        {
                                            isomitem.cliff_down.AddToDict(cisom.cliff_donwmtxmlist);
                                            isomitem.cliff_downedge.AddToDict(cisom.cliff_donwmtxmlist);
                                            isomitem.tip_down.AddToDict(cisom.cliff_donwmtxmlist);
                                            isomitem.tip_downedge.AddToDict(cisom.cliff_donwmtxmlist);
                                        }
                                      
                                    }
                                    else if (Math.Floor(isomitem.elevation) == Math.Floor(cisom.elevation))
                                    {
                                        isomitem.ConnectedEqualTile.Add(cisom);
                                        if(isomitem.elevation > cisom.elevation)
                                        {
                                            if(isomitem.ConnectLowTile != null)
                                            {

                                            }
                                            isomitem.ConnectLowTile = cisom;
                                        }
                                    }
                                }
                            }

                            foreach (var isomitem in isoms)
                            {
                                if(isomitem.ConnectLowTile != null)
                                {
                                    isomitem.AddTipToFlat(isomitem.ConnectLowTile);
                                }

                                foreach (var item in isomitem.ConnectedEqualTile)
                                {
                                    if(isomitem.elevation > item.elevation)
                                    {
                                        //현재 타일의 로우타일에다가 넣어준다..
                                        isomitem.AddTipToFlat(item);
                                    }
                                }
                            }
                            Dictionary<ushort, ushort> fliplist = new Dictionary<ushort, ushort>();
                            foreach (var item in isoms)
                            {
                                item.AddFlipList(fliplist);
                            }
                            this.TileFlipList.Add(cv.Key, fliplist);


                            ISOMdata.Add(cv.Key, isoms);
                        }
                    }
                }
            }
        }



        public List<ISOMTile> GetISOMData(MapEditor mapEditor)
        {
            if(mapEditor.mapdata == null)
            {
                return new List<ISOMTile>();
            }

            MapEditor.DrawType drawType = mapEditor.opt_drawType;
            TileType tileType = mapEditor.mapdata.TILETYPE;

            return ISOMdata[tileType];
        }



        public vf4 GetVf4(TileType tileType, ushort megatileindex)
        {
            return vf4data[tileType][megatileindex];
        }
        public ushort GetMegaTileIndex(TileType tileType, ushort MTXM)
        {
            int group = (MTXM >> 4);
            int index = (MTXM & 0xf);

            var t = cv5data[tileType];
            if (t.Length <= group)
            {
                return 0;
            }
            if (t[group].Index >= 256)
            {
                return 0;
            }

            return t[group].tiles[index];
        }


        public int GetMegaTileIndex(TileType tileType, ushort group, ushort index)
        {
            var t = cv5data[tileType];
            if (t.Length <= group)
            {
                return 0;
            }
            if (t[group].Index >= 256)
            {
                return 0;
            }

            return t[group].tiles[index];
        }


        public Texture2D GetMegaTileGrp(Control.MapEditor.DrawType drawType, TileType tileType, ushort megatileindex)
        {
            return GetTileDic(drawType)[tileType][megatileindex];
        }
        public AtlasTileSet GetAtlasTileSetTexture(Control.MapEditor.DrawType drawType, TileType tileType)
        {
            return GetTileAltasDic(drawType)[tileType];
        }


        public cv5 GetCV5(TileType tileType, ushort MTXM)
        {
            int group = (MTXM >> 4);
            int index = (MTXM & 0xf);
            return cv5data[tileType][group];
        }


        public ushort TileFlip(TileType tileType, ushort MTXM)
        {
            if (TileFlipList[tileType].ContainsKey(MTXM))
            {
                return TileFlipList[tileType][MTXM];
            }
            else
            {
                return MTXM;
            }
        }


        public Texture2D GetTile(Control.MapEditor.DrawType drawType, TileType tileType, ushort MTXM)
        {
            int group = (MTXM >> 4);
            int index = (MTXM & 0xf);

            var t = cv5data[tileType];
            if (t.Length <= group)
            {
                return null;
            }
            if (t[group].Index >= 256)
            {
                return null;
            }


            return GetTileDic(drawType)[tileType][t[group].tiles[index]];
        }

        public Texture2D GetTile(Control.MapEditor.DrawType drawType, TileType tileType, ushort group, ushort index)
        {
            var t = cv5data[tileType];
            if(t.Length <= group)
            {
                return null;
            }
            if (t[group].Index >= 256)
            {
                return null;
            }

            return GetTileDic(drawType)[tileType][t[group].tiles[index]];
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
            if(cv5data[tileType].Length <= group){
                return Color.Black;
            }

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



        private Dictionary<FileData.TileSet.TileType, Dictionary<Color,ushort>> SDTileSetMiniMapPicker;
        private Dictionary<FileData.TileSet.TileType, Dictionary<Color,ushort>> HDTileSetMiniMapPicker;
        private Dictionary<FileData.TileSet.TileType, Dictionary<Color,ushort>> CBTileSetMiniMapPicker;


        public ushort GetTileSetColorToMTXM(Control.MapEditor.DrawType drawType, UseMapEditor.FileData.TileSet.TileType tileType, System.Drawing.Color color )
        {
            Dictionary<Color, ushort> coldic = null;

            Color c = new Color(color.R, color.G, color.B);

            switch (drawType)
            {
                case Control.MapEditor.DrawType.SD:
                    coldic = SDTileSetMiniMapPicker[tileType];
                    break;
                case Control.MapEditor.DrawType.HD:
                    coldic = HDTileSetMiniMapPicker[tileType];
                    break;
                case Control.MapEditor.DrawType.CB:
                    coldic = CBTileSetMiniMapPicker[tileType];
                    break;
            }

            if (coldic.ContainsKey(c))
            {
                ushort mtxm = coldic[c];

                return mtxm;
            }

            return 0;
        }


        public List<System.Drawing.Color> GetTileSetMiniMapColorList(Control.MapEditor.DrawType drawType, UseMapEditor.FileData.TileSet.TileType tileType)
        {
            Dictionary<Color, ushort> coldic = null;
            List<System.Drawing.Color> rlist = new List<System.Drawing.Color>();


            switch (drawType)
            {
                case Control.MapEditor.DrawType.SD:
                    coldic = SDTileSetMiniMapPicker[tileType];
                    break;
                case Control.MapEditor.DrawType.HD:
                    coldic = HDTileSetMiniMapPicker[tileType];
                    break;
                case Control.MapEditor.DrawType.CB:
                    coldic = CBTileSetMiniMapPicker[tileType];
                    break;
            }
            foreach (var item in coldic.Keys)
            {
                rlist.Add(System.Drawing.Color.FromArgb(item.R, item.G, item.B));
            }

            return rlist;
        }

        /// <summary>
        /// 타일을 읽어서 png파일로 만듭니다.
        /// </summary>
        /// <returns></returns>
        public static bool ReadTile(byte[] bytes, string filename)
        {
            System.Drawing.Bitmap bitmap = null;

            BinaryReader br = new BinaryReader(new MemoryStream(bytes));

            uint filesize = br.ReadUInt32();
            ushort frame = br.ReadUInt16(); //count
            ushort unknown = br.ReadUInt16();// (file version ?); -- value appears to always be 0x1001 in the files I've seen.


            AtlasTileSet atlasTileSet = new AtlasTileSet();

            atlasTileSet.SetSize(frame);
            if (unknown == 0x1011)
            {
                atlasTileSet.framesize = 32;

                int texturewidth = atlasTileSet.framesize * atlasTileSet.length;
                bitmap = new System.Drawing.Bitmap(texturewidth, texturewidth);
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);

                //SD
                ushort width = br.ReadUInt16();
                ushort height = br.ReadUInt16();


                //uint[] pallet = new uint[400];
                byte[] pallet = new byte[1024];


                for (int i = 0; i < 256; i++)
                {
                    byte R, G, B, A;

                    B = br.ReadByte();
                    G = br.ReadByte();
                    R = br.ReadByte();
                    br.ReadByte();
                    A = 255;


                    pallet[i * 4 + 0] = R;
                    pallet[i * 4 + 1] = G;
                    pallet[i * 4 + 2] = B;
                    pallet[i * 4 + 3] = A;
                }



                for (int i = 0; i < frame; i++)
                {
                    byte[] textureData = new byte[1024];


                    for (int y = 31; y >= 0; y--)
                    {
                        for (int x = 0; x < 32; x++)
                        {
                            textureData[x + y * 32] = br.ReadByte();
                        }
                    }


                    //Texture2D texture = new Texture2D(mapDrawer.GraphicsDevice, 32, 32, false, SurfaceFormat.Color);
                    //texture.SetData(0, atlasTileSet.GetRect(i), textureData, 0, 1024);


                    BinaryWriter bitmapstream = new BinaryWriter(new MemoryStream());
                    bitmapstream.Write(FileData.BMP.bmpheader);
                    bitmapstream.Write(textureData);

                    bitmapstream.BaseStream.Position = 0x2;
                    bitmapstream.Write((uint)bitmapstream.BaseStream.Length);

                    bitmapstream.BaseStream.Position = 0x12;
                    bitmapstream.Write((int)32);
                    bitmapstream.Write((int)32);

                    bitmapstream.BaseStream.Position = FileData.BMP.palletstart;
                    bitmapstream.Write(pallet);



                    var tbitmap = new System.Drawing.Bitmap(bitmapstream.BaseStream);


                    bitmapstream.Close();



                    g.DrawImage(tbitmap, atlasTileSet.GetRect(i).X, atlasTileSet.GetRect(i).Y);
                }
            }
            else if (unknown == 0x1004) 
            {
                atlasTileSet.framesize = 128;

                int texturewidth = atlasTileSet.framesize * atlasTileSet.length;
                bitmap = new System.Drawing.Bitmap(texturewidth, texturewidth);
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
                //HD2
                for (int i = 0; i < frame; i++)
                {
                  

                    //File Entry:

                    uint unk = br.ReadUInt32(); // --always zero ?
                    ushort width = br.ReadUInt16();
                    ushort height = br.ReadUInt16();
                    uint size = br.ReadUInt32();



                    byte[] textureData = br.ReadBytes((int)size);

                    int dxtHeaderOffset = 0x80;
                    //Texture2D texture = new Texture2D(mapDrawer.GraphicsDevice, width, height, false, SurfaceFormat.Dxt1);
                    //texture.SetData(0, atlasTileSet.GetRect(i), textureData, dxtHeaderOffset, textureData.Length - dxtHeaderOffset);


                    using (var image = Pfim.Pfim.FromStream(new MemoryStream(textureData)))
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
                            var tbitmap = new System.Drawing.Bitmap(image.Width, image.Height, image.Stride, format, data);

                            g.DrawImage(tbitmap, atlasTileSet.GetRect(i).X, atlasTileSet.GetRect(i).Y);
                        }
                        finally
                        {
                            handle.Free();
                        }
                    }

                g.FillRectangle(System.Drawing.Brushes.Black, 0, 0, 128, 128);



                }
            }
            br.Close();
            
            bitmap.Save(filename, System.Drawing.Imaging.ImageFormat.Png);

            return true;
        }




        private AtlasTileSet ReadTileAltas(MapDrawer mapDrawer, TileType tileType, string _fname, List<Color> MiniMapColor)
        {
            AtlasTileSet atlasTileSet = new AtlasTileSet();

            {
                string fname = AppDomain.CurrentDomain.BaseDirectory + $"CascData\\{_fname}\\TileSet\\{tileType.ToString()}.dds.vr4.png";

                Texture2D texture = mapDrawer.LoadFromFile(fname);

                if (_fname == "SD")
                {
                    atlasTileSet.framesize = 32;
                }
                else
                {
                    atlasTileSet.framesize = 64;
                }
                atlasTileSet.texture2D = texture;

                atlasTileSet.length = texture.Width / atlasTileSet.framesize;

                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(fname);

                for (int y = 0; y < atlasTileSet.length; y++)
                {
                    for (int x = 0; x < atlasTileSet.length; x++)
                    {
                        int r = 0;
                        int g = 0;
                        int b = 0;

                        System.Drawing.Color c;
                        if (_fname == "SD")
                        {
                            c = bmp.GetPixel(x * atlasTileSet.framesize + 7, y * atlasTileSet.framesize + 6);
                        }
                        else
                        {
                            c = bmp.GetPixel(x * atlasTileSet.framesize + 32 , y * atlasTileSet.framesize + 30 );
                        }

                        r += c.R;
                        g += c.G;
                        b += c.B;


                        MiniMapColor.Add(new Color(r, g, b));
                    }
                }
               

                bmp.Dispose();
            }
            {
                string fname = AppDomain.CurrentDomain.BaseDirectory + $"CascData\\{_fname}\\TileSet\\{tileType.ToString()}.dds.vr4s.png";

                Texture2D texture = mapDrawer.LoadFromFile(fname);
                atlasTileSet.smalltexture2D = texture;
            }




            return atlasTileSet;
        }
    }
}
