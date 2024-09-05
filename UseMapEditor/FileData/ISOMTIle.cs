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
    public class ISOMTile
    {
        public override string ToString()
        {
            return name;
        }



        public string name;
        public JArray flattile;
        public List<string> connectedtilenamelist;

        //바로 연결된 낮은 타일
        public ISOMTile ConnectLowTile;
        //바로 연결된 높은 타일
        public ISOMTile ConnectHighTile;
        //같은 높이로 연결된 타일
        public List<ISOMTile> ConnectedEqualTile;


        //같은 높이로 연결된 타일
        public List<ISOMTile> ConnectedAllTile;


        public double elevation;

        public enum ISOMGroupType
        {
            tip,
            cliff,
            edge,
            flat
        }

        public bool IsMiniISOM = false;


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


        public ISOMGroup edgetop_default;
        public ISOMGroup edgetop_top;
        public ISOMGroup edgetop_toplong;
        public ISOMGroup edgetop_corner;
        public ISOMGroup edgetop_cornerslim;
        public ISOMGroup edgetop_cornerslimtop;

        public ISOMGroup edgebottom_default;
        public ISOMGroup edgebottom_slim;
        public ISOMGroup edgebottom_slimbottom;
        public ISOMGroup edgebottom_corner;
        public ISOMGroup edgebottom_cornerslim;
        public ISOMGroup edgebottom_cornerslimbottom;

        public ISOMGroup cliff_down;
        public ISOMGroup tip_down;
        public ISOMGroup tip_downedge;
        public ISOMGroup cliff_downedge;

        public List<ISOMGroup> groups = new List<ISOMGroup>();

        public ISOMPart group1list;
        public ISOMPart group2list;

        public Dictionary<ushort, bool> edge_mtxmlist;
        public Dictionary<ushort, bool> flat_mtxmlist;
        public Dictionary<ushort, bool> cliff_flat_right_mtxmlist;
        public Dictionary<ushort, bool> cliff_flat_left_mtxmlist;
        public Dictionary<ushort, bool> tip_right_mtxmlist;
        public Dictionary<ushort, bool> tip_left_mtxmlist;


        public Dictionary<ushort, bool> topdonwlist;

        public static Random rd = new Random();
        public static int GetRdindex(ISOMGroup isomGroup)
        {
            return rd.Next(isomGroup.Length - 1);
        }
        public int lastindex = 0;


        public bool IsCustom;
        public bool IsNoEdge;
        public bool IsHasCliffDown;



        //그룹으로 부터 시작
        public ISOMTile(JObject jobject, TileSet tileSet, TileType tileType)
        {
            name = jobject["name"].ToString();

            IsCustom = (bool)jobject["is_custom"];
            IsNoEdge = (bool)jobject["is_no_edge"];

            cliff_flat_right_mtxmlist = new Dictionary<ushort, bool>();
            cliff_flat_left_mtxmlist = new Dictionary<ushort, bool>();
            edge_mtxmlist = new Dictionary<ushort, bool>();
            flat_mtxmlist = new Dictionary<ushort, bool>();
            tip_right_mtxmlist = new Dictionary<ushort, bool>();
            tip_left_mtxmlist = new Dictionary<ushort, bool>();

            topdonwlist = new Dictionary<ushort, bool>();

            ConnectedEqualTile = new List<ISOMTile>();
            connectedtilenamelist = new List<string>();
            ConnectedAllTile = new List<ISOMTile>();
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

            edgetop_default = new ISOMGroup("edgetop_default", jobject, tileType, tileSet);
            edgetop_top = new ISOMGroup("edgetop_top", jobject, tileType, tileSet);
            edgetop_toplong = new ISOMGroup("edgetop_toplong", jobject, tileType, tileSet);
            edgetop_corner = new ISOMGroup("edgetop_corner", jobject, tileType, tileSet);
            edgetop_cornerslim = new ISOMGroup("edgetop_cornerslim", jobject, tileType, tileSet);
            edgetop_cornerslimtop = new ISOMGroup("edgetop_cornerslimtop", jobject, tileType, tileSet);

            edgebottom_default = new ISOMGroup("edgebottom_default", jobject, tileType, tileSet);
            edgebottom_slim = new ISOMGroup("edgebottom_slim", jobject, tileType, tileSet);
            edgebottom_slimbottom = new ISOMGroup("edgebottom_slimbottom", jobject, tileType, tileSet);
            edgebottom_corner = new ISOMGroup("edgebottom_corner", jobject, tileType, tileSet);
            edgebottom_cornerslim = new ISOMGroup("edgebottom_cornerslim", jobject, tileType, tileSet);
            edgebottom_cornerslimbottom = new ISOMGroup("edgebottom_cornerslimbottom", jobject, tileType, tileSet);

            cliff_down = new ISOMGroup("cliff_down", jobject, tileType, tileSet);
            tip_down = new ISOMGroup("tip_down", jobject, tileType, tileSet);
            tip_downedge = new ISOMGroup("tip_downedge", jobject, tileType, tileSet);
            cliff_downedge = new ISOMGroup("cliff_downedge", jobject, tileType, tileSet);

            cliff_down.AddToDict(topdonwlist);
            tip_down.AddToDict(topdonwlist);
            tip_downedge.AddToDict(topdonwlist);
            cliff_downedge.AddToDict(topdonwlist);

            IsHasCliffDown = (topdonwlist.Count != 0);


            if (tip_default.PartLength == 4) IsMiniISOM = true;


            groups.Add(cliff_default);
            groups.Add(cliff_slimtop);
            groups.Add(cliff_slim);

            groups.Add(edgetop_default);
            groups.Add(edgetop_top);
            groups.Add(edgetop_toplong);
            groups.Add(edgetop_corner);
            groups.Add(edgetop_cornerslim);
            groups.Add(edgetop_cornerslimtop);

            groups.Add(edgebottom_default);
            groups.Add(edgebottom_slim);
            groups.Add(edgebottom_slimbottom);
            groups.Add(edgebottom_corner);
            groups.Add(edgebottom_cornerslim);
            groups.Add(edgebottom_cornerslimbottom);

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


        public void AddTipToFlat(ISOMTile groupISOM)
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

            edgetop_default.AddToDictLeftTop(groupISOM.flat_mtxmlist);
            edgetop_top.AddToDictLeftTop(groupISOM.flat_mtxmlist);
            edgetop_toplong.AddToDictLeftTop(groupISOM.flat_mtxmlist);

            cliff_default.AddToDictLeftBottom(groupISOM.flat_mtxmlist);
            cliff_slim.AddToDictLeftBottom(groupISOM.flat_mtxmlist);
            cliff_slimtop.AddToDictLeftBottom(groupISOM.flat_mtxmlist);
        }


        public enum TileBorder
        {
            Flat,
            DownBorder,
            FlatDownBorder,
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



            if (topdonwlist.ContainsKey(LT)) return TileBorder.DownBorder;
            else if (topdonwlist.ContainsKey(RT)) return TileBorder.DownBorder;
            else if (topdonwlist.ContainsKey(LB)) return TileBorder.DownBorder;
            else if (topdonwlist.ContainsKey(RB)) return TileBorder.DownBorder;

            if (ConnectHighTile != null && !ConnectHighTile.IsMiniISOM)
            {
                if (isEdgeLB && isEdgeRB
                    && (ConnectHighTile.tip_right_mtxmlist.ContainsKey(RT) || ConnectHighTile.cliff_flat_right_mtxmlist.ContainsKey(RT))
                    && (ConnectHighTile.tip_left_mtxmlist.ContainsKey(LT) || ConnectHighTile.cliff_flat_left_mtxmlist.ContainsKey(LT)))
                {
                    return TileBorder.DownBorder;
                }
            }



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
                if(ConnectHighTile != null)
                {
                    alledge = true;
                    if (!isFlatLT) alledge &= ConnectHighTile.edge_mtxmlist.ContainsKey(LT);
                    if (!isFlatRT) alledge &= ConnectHighTile.edge_mtxmlist.ContainsKey(RT);
                    if (!isFlatLB) alledge &= ConnectHighTile.edge_mtxmlist.ContainsKey(LB);
                    if (!isFlatRB) alledge &= ConnectHighTile.edge_mtxmlist.ContainsKey(RB);

                    if (alledge)
                    {
                        return TileBorder.Flat;
                    }
                
                    if(ConnectHighTile.edge_mtxmlist.ContainsKey(LT) && ConnectHighTile.edge_mtxmlist.ContainsKey(RT))
                    {
                        if (isFlatLB && isEdgeRB)
                        {
                            return TileBorder.DownBorder;
                        }else if (isFlatRB && isEdgeLB)
                        {
                            return TileBorder.DownBorder;
                        }
                    }
                
                }
            }



            //하나라도 플랫 타일이 없을 경우
            //1. 4개가 LT,LB는 HighTile의 Cliff, RT,RB가 HighTile의 Cliff일 경우 플랫타일
            if (ConnectHighTile != null)
            {
                if(ConnectHighTile.cliff_flat_left_mtxmlist.ContainsKey(RT)
                    && ConnectHighTile.cliff_flat_right_mtxmlist.ContainsKey(LT)
                    && ConnectHighTile.edge_mtxmlist.ContainsKey(RB)
                    && ConnectHighTile.edge_mtxmlist.ContainsKey(LB)
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
                if (left_tiles.Count > 0)
                {
                    left_tiles[0].AddToDict(dic);
                }
                if (right_tiles.Count > 1)
                {
                    right_tiles[1].AddToDict(dic);
                }
            }

            public void AddToDictLeftBottom(Dictionary<ushort, bool> dic)
            {
                if (left_tiles.Count > 0)
                {
                    left_tiles[left_tiles.Count - 2].AddToDict(dic);
                }
                if (right_tiles.Count > 1)
                {
                    right_tiles.Last().AddToDict(dic);
                }
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

                if (left.Count <= 3)
                {
                    //1개 or 2개 or 3개 그보다 많은 
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


                if (right.Count <= 3)
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
