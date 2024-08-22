using Data.Map;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms.VisualStyles;
using UseMapEditor.Control;
using static UseMapEditor.FileData.TileSet;

namespace UseMapEditor.FileData
{
    public class ISOMTIle
    {
        public string name;
        public JArray flattile;
        public List<string> connectedtilenamelist;

        //바로 연결된 낮은 타일
        public ISOMTIle connectlowtile;
        //바로 연결된 높은 타일
        public ISOMTIle connecthightile;
        //같은 높이로 연결된 타일
        public List<ISOMTIle> connectedtile;

        public double elevation;

        public ISOMGroup tip_default;
        public ISOMGroup tip_top;
        public ISOMGroup tip_bottom;
        public ISOMGroup tip_double;
        public ISOMGroup tip_toplong;
        public ISOMGroup tip_bottomlong;
        public ISOMGroup tip_doublelong;
        public ISOMGroup tip_doubletoplong;
        public ISOMGroup tip_doublebottomlong;

        public ISOMGroup cliff_default;
        public ISOMGroup cliff_slimtop;
        public ISOMGroup cliff_slim;

        public ISOMGroup edge_default;
        public ISOMGroup edge_top;
        public ISOMGroup edge_toplong;
        public ISOMGroup edge_slim;
        public ISOMGroup edge_slimtop;
        public ISOMGroup edge_slimtoplong;

        public ISOMGroup corner_default;
        public ISOMGroup corner_thickktop;
        public ISOMGroup corner_thickbottom;
        public ISOMGroup corner_slimdefault;
        public ISOMGroup corner_slimtop;
        public ISOMGroup corner_slimbottom;
        public ISOMGroup corner_slimdouble;
        public List<ISOMGroup> groups = new List<ISOMGroup>();

        public ISOMPart group1list;
        public ISOMPart group2list;

        public Dictionary<ushort, bool> edge_mtxmlist;
        public Dictionary<ushort, bool> flat_mtxmlist;
        public Dictionary<ushort, bool> cliff_flat_right_mtxmlist;
        public Dictionary<ushort, bool> cliff_flat_left_mtxmlist;
        public Dictionary<ushort, bool> tip_right_mtxmlist;
        public Dictionary<ushort, bool> tip_left_mtxmlist;



        public static Random rd = new Random();
        public static int GetRdindex(ISOMGroup isomGroup)
        {
            return rd.Next(isomGroup.Length - 1);
        }
        public int lastindex = 0;


        public bool IsCustomISOM;



        //그룹으로 부터 시작
        public ISOMTIle(JObject jobject, TileSet tileSet, TileType tileType)
        {
            name = jobject["name"].ToString();

            cliff_flat_right_mtxmlist = new Dictionary<ushort, bool>();
            cliff_flat_left_mtxmlist = new Dictionary<ushort, bool>();
            edge_mtxmlist = new Dictionary<ushort, bool>();
            flat_mtxmlist = new Dictionary<ushort, bool>();
            tip_right_mtxmlist = new Dictionary<ushort, bool>();
            tip_left_mtxmlist = new Dictionary<ushort, bool>();

            connectedtile = new List<ISOMTIle>();
            connectedtilenamelist = new List<string>();
            foreach (var item in (JArray)jobject["connectedtile"])
            {
                connectedtilenamelist.Add(item.ToString());
            }

            elevation = (double)jobject["elevation"];

            List<ushort> flattilelist = new List<ushort>();
            foreach (var item in (JArray)jobject["flattile"])
            {
                flattilelist.Add((ushort)item);

                if (flattilelist.Count == 16 && group1list == null)
                {
                    group1list = new ISOMPart(flattilelist, tileType, tileSet);
                    flattilelist.Clear();
                }else if (flattilelist.Count == 16 && group1list != null)
                {
                    group2list = new ISOMPart(flattilelist, tileType, tileSet);
                }
            }
            if(flattilelist.Count() == 1)
            {
                //하나만 들어왔을 경우
                group1list = new ISOMPart(flattilelist[0], tileType, tileSet);
                group2list = new ISOMPart(flattilelist[0] + 1, tileType, tileSet);
            }
            group1list.AddToDict(flat_mtxmlist);
            group2list.AddToDict(flat_mtxmlist);

            tip_default = new ISOMGroup("tip_default", jobject, tileType, tileSet);
            tip_top = new ISOMGroup("tip_top", jobject, tileType, tileSet);
            tip_bottom = new ISOMGroup("tip_bottom", jobject, tileType, tileSet);
            tip_double = new ISOMGroup("tip_double", jobject, tileType, tileSet);
            tip_toplong = new ISOMGroup("tip_toplong", jobject, tileType, tileSet);
            tip_bottomlong = new ISOMGroup("tip_bottomlong", jobject, tileType, tileSet);
            tip_doublelong = new ISOMGroup("tip_doublelong", jobject, tileType, tileSet);
            tip_doubletoplong = new ISOMGroup("tip_doubletoplong", jobject, tileType, tileSet);
            tip_doublebottomlong = new ISOMGroup("tip_doublebottomlong", jobject, tileType, tileSet);

            cliff_default = new ISOMGroup("cliff_default", jobject, tileType, tileSet);
            cliff_slimtop = new ISOMGroup("cliff_slimtop", jobject, tileType, tileSet);
            cliff_slim = new ISOMGroup("cliff_slim", jobject, tileType, tileSet);

            edge_default = new ISOMGroup("edge_default", jobject, tileType, tileSet);
            edge_top = new ISOMGroup("edge_top", jobject, tileType, tileSet);
            edge_toplong = new ISOMGroup("edge_toplong", jobject, tileType, tileSet);
            edge_slim = new ISOMGroup("edge_slim", jobject, tileType, tileSet);
            edge_slimtop = new ISOMGroup("edge_slimtop", jobject, tileType, tileSet);
            edge_slimtoplong = new ISOMGroup("edge_slimtoplong", jobject, tileType, tileSet);

            corner_default = new ISOMGroup("corner_default", jobject, tileType, tileSet);
            corner_thickktop = new ISOMGroup("corner_thickktop", jobject, tileType, tileSet);
            corner_thickbottom = new ISOMGroup("corner_thickbottom", jobject, tileType, tileSet);
            corner_slimdefault = new ISOMGroup("corner_slimdefault", jobject, tileType, tileSet);
            corner_slimtop = new ISOMGroup("corner_slimtop", jobject, tileType, tileSet);
            corner_slimbottom = new ISOMGroup("corner_slimbottom", jobject, tileType, tileSet);
            corner_slimdouble = new ISOMGroup("corner_slimdouble", jobject, tileType, tileSet);



            groups.Add(cliff_default);
            groups.Add(cliff_slimtop);
            groups.Add(cliff_slim);
            groups.Add(edge_default);
            groups.Add(edge_top);
            groups.Add(edge_toplong);
            groups.Add(edge_slim);
            groups.Add(edge_slimtop);
            groups.Add(edge_slimtoplong);
            groups.Add(corner_default);
            groups.Add(corner_thickktop);
            groups.Add(corner_thickbottom);
            groups.Add(corner_slimdefault);
            groups.Add(corner_slimtop);
            groups.Add(corner_slimbottom);
            groups.Add(corner_slimdouble);
            groups.Add(tip_default);
            groups.Add(tip_top);
            groups.Add(tip_bottom);
            groups.Add(tip_double);
            groups.Add(tip_toplong);
            groups.Add(tip_bottomlong);
            groups.Add(tip_doublelong);
            groups.Add(tip_doubletoplong);
            groups.Add(tip_doublebottomlong);



            tip_default.AddToDict(tip_right_mtxmlist, false);
            tip_top.AddToDict(tip_right_mtxmlist, false);
            tip_bottom.AddToDict(tip_right_mtxmlist, false);
            tip_double.AddToDict(tip_right_mtxmlist, false);
            tip_toplong.AddToDict(tip_right_mtxmlist, false);
            tip_bottomlong.AddToDict(tip_right_mtxmlist, false);
            tip_doublelong.AddToDict(tip_right_mtxmlist, false);
            tip_doubletoplong.AddToDict(tip_right_mtxmlist, false);
            tip_doublebottomlong.AddToDict(tip_right_mtxmlist, false);

            tip_default.AddToDict(tip_left_mtxmlist, true);
            tip_top.AddToDict(tip_left_mtxmlist, true);
            tip_bottom.AddToDict(tip_left_mtxmlist, true);
            tip_double.AddToDict(tip_left_mtxmlist, true);
            tip_toplong.AddToDict(tip_left_mtxmlist, true);
            tip_bottomlong.AddToDict(tip_left_mtxmlist, true);
            tip_doublelong.AddToDict(tip_left_mtxmlist, true);
            tip_doubletoplong.AddToDict(tip_left_mtxmlist, true);
            tip_doublebottomlong.AddToDict(tip_left_mtxmlist, true);


            cliff_default.AddToDict(cliff_flat_right_mtxmlist, false);
            cliff_slim.AddToDict(cliff_flat_right_mtxmlist, false);
            cliff_slimtop.AddToDict(cliff_flat_right_mtxmlist, false);

            cliff_default.AddToDict(cliff_flat_left_mtxmlist, true);
            cliff_slim.AddToDict(cliff_flat_left_mtxmlist, true);
            cliff_slimtop.AddToDict(cliff_flat_left_mtxmlist, true);


            foreach (var _group in groups)
            {
                if(_group.left_tiles != null)
                {
                    foreach (var _left in _group.left_tiles)
                    {
                        _left.AddToDict(edge_mtxmlist);
                    }
                }

                if (_group.right_tiles != null)
                {
                    foreach (var _right in _group.right_tiles)
                    {
                        _right.AddToDict(edge_mtxmlist);
                    }
                }
            }


        }


        public void AddTipToFlat(ISOMTIle groupISOM)
        {
            tip_default.AddToDictOneLine(groupISOM.flat_mtxmlist, true);
            tip_top.AddToDictOneLine(groupISOM.flat_mtxmlist, true);
            tip_bottom.AddToDictOneLine(groupISOM.flat_mtxmlist, true);
            tip_double.AddToDictOneLine(groupISOM.flat_mtxmlist, true);
            tip_toplong.AddToDictOneLine(groupISOM.flat_mtxmlist, true);
            tip_bottomlong.AddToDictOneLine(groupISOM.flat_mtxmlist, true);
            tip_doublelong.AddToDictOneLine(groupISOM.flat_mtxmlist, true);
            tip_doubletoplong.AddToDictOneLine(groupISOM.flat_mtxmlist, true);
            tip_doublebottomlong.AddToDictOneLine(groupISOM.flat_mtxmlist, true);

            edge_default.AddToDictLeftTop(groupISOM.flat_mtxmlist);
            edge_top.AddToDictLeftTop(groupISOM.flat_mtxmlist);
            edge_toplong.AddToDictLeftTop(groupISOM.flat_mtxmlist);
            edge_slim.AddToDictLeftTop(groupISOM.flat_mtxmlist);
            edge_slimtop.AddToDictLeftTop(groupISOM.flat_mtxmlist);
            edge_slimtoplong.AddToDictLeftTop(groupISOM.flat_mtxmlist);
        }


        public enum TileBorder
        {
            Flat,
            DownBorder,
            UpBorder,
            None
        }
        public TileBorder CheckTile(ushort LT, ushort RT, ushort LB, ushort RB)
        {
            bool isFlatLT = flat_mtxmlist.ContainsKey(LT);
            bool isFlatRT = flat_mtxmlist.ContainsKey(RT);
            bool isFlatLB = flat_mtxmlist.ContainsKey(LB);
            bool isFlatRB = flat_mtxmlist.ContainsKey(RB);

            bool isEdgeLT = edge_mtxmlist.ContainsKey(LT);
            bool isEdgeRT = edge_mtxmlist.ContainsKey(RT);
            bool isEdgeLB = edge_mtxmlist.ContainsKey(LB);
            bool isEdgeRB = edge_mtxmlist.ContainsKey(RB);

            //4개가 전부 플랫 타일일 경우 (무조건 플랫타일)
            if (isFlatLT && isFlatRT && isFlatLB && isFlatRB) return TileBorder.Flat;


            //하나라도 플랫 타일이 있을 경우
            if (isFlatLT || isFlatRT || isFlatLB || isFlatRB)
            {
                bool alledge = true;
                if (!isFlatLT) alledge &= isEdgeLT;
                if (!isFlatRT) alledge &= isEdgeRT;
                if (!isFlatLB) alledge &= isEdgeLB;
                if (!isFlatRB) alledge &= isEdgeRB;

                //1. 나머지 타일이 전부 해당 타일의 Edge일 경우 -> 경계타일
                if (alledge)
                {
                    return TileBorder.DownBorder;
                }

                //2. 나머지 타일이 전부 HighTile의 Edge일 경우 -> 플랫타일
                if(connecthightile != null)
                {
                    alledge = true;
                    if (!isFlatLT) alledge &= connecthightile.edge_mtxmlist.ContainsKey(LT);
                    if (!isFlatRT) alledge &= connecthightile.edge_mtxmlist.ContainsKey(RT);
                    if (!isFlatLB) alledge &= connecthightile.edge_mtxmlist.ContainsKey(LB);
                    if (!isFlatRB) alledge &= connecthightile.edge_mtxmlist.ContainsKey(RB);

                    if (alledge)
                    {
                        return TileBorder.Flat;
                    }
                }
            }



            //하나라도 플랫 타일이 없을 경우
            //1. 4개가 LT,LB는 HighTile의 Cliff, RT,RB가 HighTile의 Cliff일 경우 플랫타일
            if (connecthightile != null)
            {
                if(connecthightile.cliff_flat_left_mtxmlist.ContainsKey(RT)
                    && connecthightile.cliff_flat_right_mtxmlist.ContainsKey(LT)
                    && connecthightile.edge_mtxmlist.ContainsKey(RB)
                    && connecthightile.edge_mtxmlist.ContainsKey(LB)
                    //&& connecthightile.cliff_flat_left_mtxmlist.ContainsKey(RB)
                    //&& connecthightile.cliff_flat_right_mtxmlist.ContainsKey(LB)
                    )
                {
                    return TileBorder.Flat;
                }


                //if (connecthightile.tip_left_mtxmlist.ContainsKey(RT)
                //    && connecthightile.tip_left_mtxmlist.ContainsKey(RB)
                //    && connecthightile.tip_right_mtxmlist.ContainsKey(LT)
                //    && connecthightile.tip_right_mtxmlist.ContainsKey(LB))
                //{
                //    return TileBorder.Flat;
                //}
            }


            //2. 모두 Edge일 경우 -> 경계타일
            if (isEdgeLT && isEdgeRT && isEdgeLB && isEdgeRB)
            {
                //모두 Tip일 경우 아무것도 아님.
                if (!tip_left_mtxmlist.ContainsKey(RT) && !tip_left_mtxmlist.ContainsKey(RB) && !tip_right_mtxmlist.ContainsKey(LT) && !tip_right_mtxmlist.ContainsKey(LB))
                {
                    return TileBorder.DownBorder;
                }
            }




            //그 외는 None처리
            return TileBorder.None;
        }


        public ushort GetFlatTile(int x, int y, MapData refmapdata)
        {
            if(x % 2 == 0)
            {
                int rv = rd.Next(0, group1list.Length - 1);

                lastindex = rv;


                return group1list.mtxmlist[rv];
            }
            else
            {
                int rv = lastindex;

                if(lastindex == -1)
                {
                    rv = rd.Next(0, group2list.Length - 1);
                }
                else if (group2list.Length <= rv)
                {
                    rv = 0;
                }

                lastindex = -1;

                return group2list.mtxmlist[rv];
            }
        }

        #region 기본 Flat ISOM
        public class ISOMPart
        {
            public ushort[] mtxmlist;
            public ushort[] index;

            public void AddToDict(Dictionary<ushort, bool> dic)
            {
                foreach (var item in mtxmlist)
                {
                    if (!dic.ContainsKey(item))
                    {
                        dic.Add(item, true);
                    }
                }
            }

            public ushort this[int index]
            {
                get
                {
                    if (mtxmlist.Length <= index)
                    {
                        return mtxmlist[0];
                    }

                    return mtxmlist[index];
                }
            }


            public ushort GetFromGroup(int group)
            {
                if (index[group] == 0)
                {
                    return mtxmlist[0];
                }

                return index[group];
            }



            public int Length
            {
                get
                {
                    return mtxmlist.Length;
                }
            }


            public ISOMPart(int mtxmgroup, TileType tileType, TileSet tileSet)
            {
                index = new ushort[16];

                List<ushort> list = new List<ushort>();
                
                for (int i = 0; i < 16; i++)
                {
                    ushort mtxm = ((ushort)(mtxmgroup * 16 + i));

                    if (tileSet.GetMegaTileIndex(tileType, mtxm) != 0)
                    {
                        list.Add(mtxm);
                        index[i] = mtxm;
                    }
                }

                mtxmlist = list.ToArray();
            }

            public ISOMPart(List<ushort> mtxmlist, TileType tileType, TileSet tileSet)
            {
                index = new ushort[16];

                List<ushort> list = new List<ushort>();

                int i = 0;
                foreach (var mtxm in mtxmlist)
                {
                    if (tileSet.GetMegaTileIndex(tileType, mtxm) != 0)
                    {
                        list.Add(mtxm);
                        index[i] = mtxm;
                    }
                    i += 1;
                }

                this.mtxmlist = list.ToArray();
            }
        }


        public class ISOMGroup
        {
            public string groupname;

            public List<ISOMPart> left_tiles;
            public List<ISOMPart> right_tiles;

            public int Length = int.MaxValue;

            public int PartLength
            {
                get { return left_tiles.Count; }
            }

            public void AddToDict(Dictionary<ushort, bool> dic, bool IsLeft)
            {
                if (IsLeft)
                {
                    foreach (var item in left_tiles)
                    {
                        item.AddToDict(dic);
                    }
                }
                else
                {
                    foreach (var item in right_tiles)
                    {
                        item.AddToDict(dic);
                    }
                }
            }


            public void AddToDictOneLine(Dictionary<ushort, bool> dic, bool IsLeft)
            {
                if (IsLeft)
                {
                    for (int i = 0; i < left_tiles.Count; i += 2)
                    {
                        left_tiles[i].AddToDict(dic);
                    }
                    for (int i = 1; i < right_tiles.Count; i += 2)
                    {
                        right_tiles[i].AddToDict(dic);
                    }
                }
                else
                {
                    for (int i = 1; i < left_tiles.Count; i += 2)
                    {
                        left_tiles[i].AddToDict(dic);
                    }
                    for (int i = 0; i < right_tiles.Count; i += 2)
                    {
                        right_tiles[i].AddToDict(dic);
                    }
                }
            }


            public void AddToDictLeftTop(Dictionary<ushort, bool> dic)
            {
                left_tiles[0].AddToDict(dic);
                right_tiles[1].AddToDict(dic);
            
            }

            public void AddToDict(Dictionary<ushort, bool> dic)
            {
                foreach (var item in left_tiles)
                {
                    item.AddToDict(dic);
                }
                foreach (var item in right_tiles)
                {
                    item.AddToDict(dic);
                }
            }


            public ISOMGroup(string name, JObject jobject, TileType tileType, TileSet tileSet)
            {
                groupname = name;

                left_tiles = new List<ISOMPart>();
                right_tiles = new List<ISOMPart>();

                JArray array = (JArray)jobject[name];

                if (array == null) return;

                JArray left = (JArray)array.First();
                JArray right = (JArray)array.Last();

                if (left.Count == 2 || left.Count == 3)
                {
                    //2개 or 3개 그보다 많은 
                    foreach (var item in left)
                    {
                        ushort lmtxm = (ushort)item;
                        ushort rmtxm = (ushort)((ushort)item + 1);

                        left_tiles.Add(new ISOMPart(lmtxm, tileType, tileSet));
                        left_tiles.Add(new ISOMPart(rmtxm, tileType, tileSet));
                    }
                }
                else
                {
                    List<ushort> list = new List<ushort>();
                    foreach (var item in left)
                    {
                        list.Add((ushort)item);

                        if (list.Count == 16)
                        {
                            left_tiles.Add(new ISOMPart(list, tileType, tileSet));

                            list.Clear();
                        }
                    }
                }


                if (right.Count == 2 || right.Count == 3)
                {
                    //2개 or 3개 그보다 많은 
                    foreach (var item in right)
                    {
                        ushort lmtxm = (ushort)item;
                        ushort rmtxm = (ushort)((ushort)item + 1);

                        right_tiles.Add(new ISOMPart(lmtxm, tileType, tileSet));
                        right_tiles.Add(new ISOMPart(rmtxm, tileType, tileSet));
                    }
                }
                else
                {
                    List<ushort> list = new List<ushort>();
                    foreach (var item in right)
                    {
                        list.Add((ushort)item);

                        if (list.Count == 16)
                        {
                            right_tiles.Add(new ISOMPart(list, tileType, tileSet));

                            list.Clear();
                        }
                    }
                }

                foreach (var item in left_tiles)
                {
                    if (Length > item.Length)
                    {
                        Length = item.Length;
                    }
                }
                foreach (var item in right_tiles)
                {
                    if (Length > item.Length)
                    {
                        Length = item.Length;
                    }
                }

            }
        }

        #endregion
    }
}
