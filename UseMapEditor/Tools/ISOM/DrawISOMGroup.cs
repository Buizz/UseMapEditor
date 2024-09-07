using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UseMapEditor.FileData.ISOMTile;
using UseMapEditor.Control;
using UseMapEditor.FileData;
using UseMapEditor.Task.Events;

namespace UseMapEditor.Tools
{
    public partial class ISOMTool
    {
        public static void DrawISOMGroup(MapEditor mapeditor, ISOMTile isomtile, ISOMTile lowtile, ISOMChecker checker, ISOMTile.ISOMGroupType grouptype, DrawDirection direction, int tx, int ty, int gindex = -1)
        {
            if (!checker.AddDrawList(isomtile, grouptype, direction, tx, ty)) return;


            TileType R = checker.TileCheck(IWay.R, tx, ty);
            TileType L = checker.TileCheck(IWay.L, tx, ty);
            TileType T = checker.TileCheck(IWay.T, tx, ty);
            TileType B = checker.TileCheck(IWay.B, tx, ty);
            TileType LT = checker.TileCheck(IWay.LT, tx, ty);
            TileType RT = checker.TileCheck(IWay.RT, tx, ty);
            TileType LB = checker.TileCheck(IWay.LB, tx, ty);
            TileType RB = checker.TileCheck(IWay.RB, tx, ty);

            TileType RBB = checker.TileCheck(IWay.RBB, tx, ty);
            TileType LBB = checker.TileCheck(IWay.LBB, tx, ty);
            TileType RTT = checker.TileCheck(IWay.RTT, tx, ty);
            TileType LTT = checker.TileCheck(IWay.LTT, tx, ty);

            TileType LLBB = checker.TileCheck(IWay.LLBB, tx, ty);
            TileType RRBB = checker.TileCheck(IWay.RRBB, tx, ty);

            TileType BB = checker.TileCheck(IWay.BB, tx, ty);
            TileType TT = checker.TileCheck(IWay.TT, tx, ty);


            if (R.IsNull() || L.IsNull() || T.IsNull() || B.IsNull()
             || LT.IsNull() || RT.IsNull() || LB.IsNull() || RB.IsNull()
             || RBB.IsNull() || LBB.IsNull() || RTT.IsNull() || LTT.IsNull()
             || BB.IsNull() || TT.IsNull() || LLBB.IsNull() || RRBB.IsNull()
             ) return;

            if (R.Tile != isomtile && R.Tile != lowtile && (isomtile.ConnectedEqualTile.Contains(R.Tile) || (isomtile.ConnectHighTile == R.Tile)) && isomtile.elevation < R.Tile.elevation) R.Tile = isomtile;
            if (L.Tile != isomtile && L.Tile != lowtile && (isomtile.ConnectedEqualTile.Contains(L.Tile) || (isomtile.ConnectHighTile == L.Tile)) && isomtile.elevation < L.Tile.elevation) L.Tile = isomtile;
            if (T.Tile != isomtile && T.Tile != lowtile && (isomtile.ConnectedEqualTile.Contains(T.Tile) || (isomtile.ConnectHighTile == T.Tile)) && isomtile.elevation < T.Tile.elevation) T.Tile = isomtile;
            if (B.Tile != isomtile && B.Tile != lowtile && (isomtile.ConnectedEqualTile.Contains(B.Tile) || (isomtile.ConnectHighTile == B.Tile)) && isomtile.elevation < B.Tile.elevation) B.Tile = isomtile;
            if (LT.Tile != isomtile && LT.Tile != lowtile && (isomtile.ConnectedEqualTile.Contains(LT.Tile) || (isomtile.ConnectHighTile == LT.Tile)) && isomtile.elevation < LT.Tile.elevation) LT.Tile = isomtile;
            if (RT.Tile != isomtile && RT.Tile != lowtile && (isomtile.ConnectedEqualTile.Contains(RT.Tile) || (isomtile.ConnectHighTile == RT.Tile)) && isomtile.elevation < RT.Tile.elevation) RT.Tile = isomtile;
            if (LB.Tile != isomtile && LB.Tile != lowtile && (isomtile.ConnectedEqualTile.Contains(LB.Tile) || (isomtile.ConnectHighTile == LB.Tile)) && isomtile.elevation < LB.Tile.elevation) LB.Tile = isomtile;
            if (RB.Tile != isomtile && RB.Tile != lowtile && (isomtile.ConnectedEqualTile.Contains(RB.Tile) || (isomtile.ConnectHighTile == RB.Tile)) && isomtile.elevation < RB.Tile.elevation) RB.Tile = isomtile;
            if (RBB.Tile != isomtile && RBB.Tile != lowtile && (isomtile.ConnectedEqualTile.Contains(RBB.Tile) || (isomtile.ConnectHighTile == RBB.Tile)) && isomtile.elevation < RBB.Tile.elevation) RBB.Tile = isomtile;
            if (LBB.Tile != isomtile && LBB.Tile != lowtile && (isomtile.ConnectedEqualTile.Contains(LBB.Tile) || (isomtile.ConnectHighTile == LBB.Tile)) && isomtile.elevation < LBB.Tile.elevation) LBB.Tile = isomtile;
            if (RTT.Tile != isomtile && RTT.Tile != lowtile && (isomtile.ConnectedEqualTile.Contains(RTT.Tile) || (isomtile.ConnectHighTile == RTT.Tile)) && isomtile.elevation < RTT.Tile.elevation) RTT.Tile = isomtile;
            if (LTT.Tile != isomtile && LTT.Tile != lowtile && (isomtile.ConnectedEqualTile.Contains(LTT.Tile) || (isomtile.ConnectHighTile == LTT.Tile)) && isomtile.elevation < LTT.Tile.elevation) LTT.Tile = isomtile;
            if (TT.Tile != isomtile && TT.Tile != lowtile && (isomtile.ConnectedEqualTile.Contains(TT.Tile) || (isomtile.ConnectHighTile == TT.Tile)) && isomtile.elevation < TT.Tile.elevation) TT.Tile = isomtile;

            ISOMGroup group = null;
            int drawx = tx, drawy = ty;
            switch (grouptype)
            {
                case ISOMGroupType.flat:
                    for (int y = 0; y < 2; y++)
                    {
                        if (direction == DrawDirection.All)
                        {
                            for (int x = 0; x < 4; x++)
                            {
                                int gtx = tx + x - 2;
                                int gty = ty + y - 1;


                                ushort newMTXM = isomtile.GetFlatTile(gtx, gty, mapeditor.mapdata);

                                int tileindex = gtx + gty * mapeditor.mapdata.WIDTH;

                                if (mapeditor.mapdata.CheckTILERange(gtx, gty))
                                {
                                    ushort oldMTXM = mapeditor.mapdata.TILE[tileindex];

                                    mapeditor.mapdata.TILEChange(gtx, gty, newMTXM);
                                    mapeditor.taskManager.TaskAdd(new TileEvent(mapeditor, newMTXM, oldMTXM, gtx, gty));
                                }
                            }
                        }
                        else
                        {
                            for (int x = 0; x < 2; x++)
                            {
                                int gtx = tx + x;
                                int gty = ty + y - 1;

                                if (direction == DrawDirection.Left)
                                {
                                    gtx -= 2;
                                }


                                ushort newMTXM = isomtile.GetFlatTile(gtx, gty, mapeditor.mapdata);

                                int tileindex = gtx + gty * mapeditor.mapdata.WIDTH;

                                if (mapeditor.mapdata.CheckTILERange(gtx, gty))
                                {
                                    ushort oldMTXM = mapeditor.mapdata.TILE[tileindex];

                                    mapeditor.mapdata.TILEChange(gtx, gty, newMTXM);
                                    mapeditor.taskManager.TaskAdd(new TileEvent(mapeditor, newMTXM, oldMTXM, gtx, gty));
                                }
                            }
                        }


                    }
                    break;
                case ISOMGroupType.tip:

                    if (!isomtile.IsMiniISOM)
                    {
                        if (T.Check(isomtile, TileBorder.DownBorder) && !B.Check(isomtile, TileBorder.DownBorder))
                        {
                            if (direction == DrawDirection.Left)
                            {
                                if (LTT.Check(isomtile, TileBorder.DownBorder))
                                {
                                    group = isomtile.tip_toplong;
                                    if (gindex == -1) gindex = GetRdindex(group);
                                    DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty - 2, gindex);
                                }
                            }
                            else
                            {
                                if (RTT.Check(isomtile, TileBorder.DownBorder))
                                {
                                    group = isomtile.tip_toplong;
                                    if (gindex == -1) gindex = GetRdindex(group);
                                    DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty - 2, gindex);
                                }
                            }

                            if (group == null)
                            {
                                group = isomtile.tip_top;
                                if (gindex == -1) gindex = GetRdindex(group);
                                DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.tip, direction, tx, ty - 2, gindex);
                            }
                        }
                        else if (B.Check(isomtile, TileBorder.DownBorder) && !T.Check(isomtile, TileBorder.DownBorder))
                        {
                            if (direction == DrawDirection.Left)
                            {
                                if (LBB.Check(isomtile, TileBorder.DownBorder))
                                {
                                    group = isomtile.tip_bottomlong;
                                    if (gindex == -1) gindex = GetRdindex(group);
                                    DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty + 2, gindex);
                                }
                            }
                            else
                            {
                                if (RBB.Check(isomtile, TileBorder.DownBorder))
                                {
                                    group = isomtile.tip_bottomlong;
                                    if (gindex == -1) gindex = GetRdindex(group);
                                    DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty + 2, gindex);
                                }
                            }

                            if (group == null)
                            {
                                group = isomtile.tip_bottom;
                                if (direction == DrawDirection.Left)
                                {
                                    if (gindex == -1) gindex = GetRdindex(group);
                                    DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.tip, direction, tx, ty + 2, gindex);
                                }
                                else
                                {
                                    if (gindex == -1) gindex = GetRdindex(group);
                                    DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.tip, direction, tx, ty + 2, gindex);
                                }
                            }
                        }
                        else if (T.Check(isomtile, TileBorder.DownBorder) && B.Check(isomtile, TileBorder.DownBorder))
                        {
                            if (direction == DrawDirection.Left)
                            {
                                if (LTT.Check(isomtile, TileBorder.DownBorder)
                                    && LBB.Check(isomtile, TileBorder.DownBorder))
                                {
                                    group = isomtile.tip_doublelong;
                                    if (gindex == -1) gindex = GetRdindex(group);
                                    DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty - 2, gindex);
                                    DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.edge, direction, tx, ty + 2, gindex);
                                }
                                else if (LTT.Check(isomtile, TileBorder.DownBorder))
                                {
                                    group = isomtile.tip_doubletoplong;
                                    if (gindex == -1) gindex = GetRdindex(group);
                                    DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty - 2, gindex);
                                    DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.tip, direction, tx, ty + 2, gindex);
                                }
                                else if (LBB.Check(isomtile, TileBorder.DownBorder))
                                {
                                    group = isomtile.tip_doublebottomlong;
                                    if (gindex == -1) gindex = GetRdindex(group);
                                    DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.tip, direction, tx, ty - 2, gindex);
                                    DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty + 2, gindex);
                                }
                            }
                            else
                            {
                                if (RTT.Check(isomtile, TileBorder.DownBorder)
                                    && RBB.Check(isomtile, TileBorder.DownBorder))
                                {
                                    group = isomtile.tip_doublelong;
                                    if (gindex == -1) gindex = GetRdindex(group);
                                    DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty - 2, gindex);
                                    DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.edge, direction, tx, ty + 2, gindex);
                                }
                                else if (RTT.Check(isomtile, TileBorder.DownBorder))
                                {
                                    group = isomtile.tip_doubletoplong;
                                    if (gindex == -1) gindex = GetRdindex(group);
                                    DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty - 2, gindex);
                                    DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.tip, direction, tx, ty + 2, gindex);
                                }
                                else if (RBB.Check(isomtile, TileBorder.DownBorder))
                                {
                                    group = isomtile.tip_doublebottomlong;
                                    if (gindex == -1) gindex = GetRdindex(group);
                                    DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.tip, direction, tx, ty - 2, gindex);
                                    DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty + 2, gindex);
                                }
                            }

                            if (group == null)
                            {
                                group = isomtile.tip_double;
                                if (gindex == -1) gindex = GetRdindex(group);
                                DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.tip, direction, tx, ty + 2, gindex);
                                DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.tip, direction, tx, ty - 2, gindex);
                            }
                        }
                    }


                    if (group == null)
                    {
                        group = isomtile.tip_default;
                    }
                    if (gindex == -1) gindex = GetRdindex(group);
                    if (direction == DrawDirection.Left)
                    {
                        drawx -= 2;
                        drawy -= 1;
                    }
                    else
                    {
                        drawx += 0;
                        drawy -= 1;
                    }
                    DrawISOMTile(mapeditor, group, direction, drawx, drawy, gindex);
                    if (!isomtile.IsMiniISOM && lowtile != null)
                    {
                        if (direction == DrawDirection.Left)
                        {
                            if (BB.Check(lowtile.ConnectLowTile, TileBorder.Flat) && LBB.Check(lowtile.ConnectLowTile, TileBorder.Flat))
                            {
                                DrawISOMGroup(mapeditor, lowtile, lowtile.ConnectLowTile, checker, ISOMGroupType.cliff, direction, tx, ty + 2, gindex);
                                DrawISOMTile(mapeditor, lowtile.tip_down, direction, drawx, drawy + 2, gindex);
                            }
                            else if (L.Check(lowtile, TileBorder.DownBorder)
                                 && B.Check(lowtile, TileBorder.DownBorder)
                                 && LB.Check(lowtile, TileBorder.DownBorder)
                                 && LBB.Check(lowtile, TileBorder.DownBorder)
                                 && LLBB.Check(lowtile.ConnectLowTile, TileBorder.Flat))
                            {
                                DrawISOMGroup(mapeditor, lowtile, lowtile.ConnectLowTile, checker, ISOMGroupType.edge, direction, tx, ty + 2, gindex);
                                DrawISOMTile(mapeditor, lowtile.tip_downedge, direction, drawx, drawy + 2, gindex);
                            }
                            else if (B.Check(lowtile, TileBorder.DownBorder)
                                 && BB.Check(lowtile, TileBorder.DownBorder)
                                 && LB.Check(lowtile, TileBorder.DownBorder)
                                 && LBB.Check(lowtile.ConnectLowTile, TileBorder.Flat))
                            {
                                DrawISOMGroup(mapeditor, lowtile, lowtile.ConnectLowTile, checker, ISOMGroupType.cliff, direction, tx, ty + 2, gindex);
                                DrawISOMTile(mapeditor, lowtile.tip_down, direction, drawx, drawy + 2, gindex);
                            }
                        }
                        else
                        {
                            if (BB.Check(lowtile.ConnectLowTile, TileBorder.Flat) && RBB.Check(lowtile.ConnectLowTile, TileBorder.Flat))
                            {
                                DrawISOMGroup(mapeditor, lowtile, lowtile.ConnectLowTile, checker, ISOMGroupType.cliff, direction, tx, ty + 2, gindex);
                                DrawISOMTile(mapeditor, lowtile.tip_down, direction, drawx, drawy + 2, gindex);
                            }
                            else if (R.Check(lowtile, TileBorder.DownBorder)
                                 && B.Check(lowtile, TileBorder.DownBorder)
                                 && RB.Check(lowtile, TileBorder.DownBorder)
                                 && RBB.Check(lowtile, TileBorder.DownBorder)
                                 && RRBB.Check(lowtile.ConnectLowTile, TileBorder.Flat))
                            {
                                DrawISOMGroup(mapeditor, lowtile, lowtile.ConnectLowTile, checker, ISOMGroupType.edge, direction, tx, ty + 2, gindex);
                                DrawISOMTile(mapeditor, lowtile.tip_downedge, direction, drawx, drawy + 2, gindex);
                            }
                            else if (B.Check(lowtile, TileBorder.DownBorder)
                                 && BB.Check(lowtile, TileBorder.DownBorder)
                                 && RB.Check(lowtile, TileBorder.DownBorder)
                                 && RBB.Check(lowtile.ConnectLowTile, TileBorder.Flat))
                            {
                                DrawISOMGroup(mapeditor, lowtile, lowtile.ConnectLowTile, checker, ISOMGroupType.cliff, direction, tx, ty + 2, gindex);
                                DrawISOMTile(mapeditor, lowtile.tip_down, direction, drawx, drawy + 2, gindex);
                            }
                        }

                    }
                    break;
                case ISOMGroupType.edge:
                    if (isomtile.IsNoEdge)
                    {
                        #region 코너 없는 Edge
                        bool IsFlat = false;

                        if (direction == DrawDirection.Left)
                        {
                            if (LT.Check(isomtile, TileBorder.DownBorder))
                            {
                                if (T.Check(isomtile, TileBorder.FlatDownBorder))
                                {
                                    IsFlat = true;
                                }
                                else
                                {
                                    group = isomtile.edgetop_cornerslimtop;
                                }
                            }
                        }
                        else
                        {
                            if (RT.Check(isomtile, TileBorder.DownBorder))
                            {
                                //코너인 경우
                                if (T.Check(isomtile, TileBorder.FlatDownBorder))
                                {
                                    IsFlat = true;
                                }
                                else
                                {
                                    group = isomtile.edgetop_cornerslimtop;
                                }
                            }
                        }

                        if (group == null)
                        {
                            group = isomtile.edgetop_default;
                        }


                        if (gindex == -1) gindex = GetRdindex(group);

                        if (direction == DrawDirection.Left)
                        {
                            drawx -= 2;
                            drawy -= 1;
                        }
                        else
                        {
                            drawx += 0;
                            drawy -= 1;
                        }

                        if (IsFlat)
                        {
                            DrawISOMFlatTile(mapeditor, isomtile, direction, tx, ty - 1);
                        }
                        else
                        {
                            DrawISOMTile(mapeditor, group, direction, drawx, drawy, gindex);
                        }

                        IsFlat = false;
                        group = null;
                        if (direction == DrawDirection.Left)
                        {
                            if (LT.Check(isomtile, TileBorder.DownBorder))
                            {
                                //위에 바로 있으면 코너지형
                                if (B.Check(isomtile, TileBorder.DownBorder))
                                {
                                    IsFlat = true;
                                }
                                else
                                {
                                    group = isomtile.edgebottom_slimbottom;
                                }
                            }
                            else
                            {
                                if (!B.Check(isomtile, TileBorder.DownBorder)
                                    && (!RB.Check(isomtile, TileBorder.FlatDownBorder) || !B.Check(isomtile, TileBorder.FlatDownBorder)))
                                {
                                    group = isomtile.edgebottom_slim;
                                    if (gindex == -1) gindex = GetRdindex(group);
                                    DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty + 1, gindex);
                                }
                                else if (B.Check(isomtile, TileBorder.DownBorder)
                                    && (!RB.Check(isomtile, TileBorder.FlatDownBorder) || !B.Check(isomtile, TileBorder.FlatDownBorder)))
                                {
                                    group = isomtile.edgebottom_slimbottom;
                                    if (gindex == -1) gindex = GetRdindex(group);
                                    DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty + 1, gindex);
                                }
                            }
                        }
                        else
                        {
                            if (RT.Check(isomtile, TileBorder.DownBorder))
                            {
                                //위에 바로 있으면 코너지형
                                if (B.Check(isomtile, TileBorder.DownBorder))
                                {
                                    IsFlat = true;
                                }
                                else
                                {
                                    group = isomtile.edgebottom_slimbottom;
                                }
                            }
                            else
                            {
                                if (!B.Check(isomtile, TileBorder.DownBorder)
                                    && (!LB.Check(isomtile, TileBorder.FlatDownBorder) || !B.Check(isomtile, TileBorder.FlatDownBorder)))
                                {
                                    group = isomtile.edgebottom_slim;
                                    if (gindex == -1) gindex = GetRdindex(group);
                                    DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty + 1, gindex);
                                }
                                else if (B.Check(isomtile, TileBorder.DownBorder)
                                    && (!LB.Check(isomtile, TileBorder.FlatDownBorder) || !B.Check(isomtile, TileBorder.FlatDownBorder)))
                                {
                                    group = isomtile.edgebottom_slimbottom;
                                    if (gindex == -1) gindex = GetRdindex(group);
                                    DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty + 1, gindex);
                                }
                            }
                        }
                        if (group == null)
                        {
                            group = isomtile.edgebottom_default;
                        }
                        if (gindex == -1) gindex = GetRdindex(group);

                        if (direction == DrawDirection.Left)
                        {
                            drawx += 0;
                            drawy += 1;
                        }
                        else
                        {
                            drawx += 0;
                            drawy += 1;
                        }

                        if (IsFlat)
                        {
                            DrawISOMFlatTile(mapeditor, isomtile, direction, tx, ty);
                        }
                        else
                        {
                            DrawISOMTile(mapeditor, group, direction, drawx, drawy, gindex);
                        }
                        #endregion

                    }
                    else
                    {
                        #region 코너 있는 Edge
                        if (direction == DrawDirection.Left)
                        {
                            if (LT.Check(isomtile, TileBorder.DownBorder))
                            {
                                //코너인 경우
                                if (LT.Check(isomtile, TileBorder.DownBorder)
                                    && !T.Check(isomtile, TileBorder.FlatDownBorder)
                                    && !RT.Check(isomtile, TileBorder.FlatDownBorder))
                                {
                                    group = isomtile.edgetop_cornerslim;
                                    if (gindex == -1) gindex = GetRdindex(group);
                                }
                                else if (LT.Check(isomtile, TileBorder.DownBorder)
                                    && T.Check(isomtile, TileBorder.FlatDownBorder)
                                    && !RT.Check(isomtile, TileBorder.FlatDownBorder))
                                {
                                    group = isomtile.edgetop_cornerslimtop;
                                    if (gindex == -1) gindex = GetRdindex(group);
                                }
                                else if (LT.Check(isomtile, TileBorder.DownBorder)
                                    && !T.Check(isomtile, TileBorder.FlatDownBorder)
                                    && RT.Check(isomtile, TileBorder.FlatDownBorder))
                                {
                                    group = isomtile.edgetop_cornerslim;
                                    if (gindex == -1) gindex = GetRdindex(group);
                                }
                                else if (LT.Check(isomtile, TileBorder.FlatDownBorder)
                                    && T.Check(isomtile, TileBorder.FlatDownBorder)
                                    && RT.Check(isomtile, TileBorder.FlatDownBorder))
                                {
                                    group = isomtile.edgetop_corner;
                                    if (gindex == -1) gindex = GetRdindex(group);
                                }
                            }
                            else
                            {
                                if (!isomtile.IsMiniISOM)
                                {
                                    if (T.Check(isomtile, TileBorder.DownBorder) && !LTT.Check(isomtile, TileBorder.DownBorder))
                                    {
                                        group = isomtile.edgetop_top;
                                        if (gindex == -1) gindex = GetRdindex(group);
                                        DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty - 2, gindex);
                                    }
                                    else if (T.Check(isomtile, TileBorder.DownBorder) && LTT.Check(isomtile, TileBorder.DownBorder))
                                    {
                                        group = isomtile.edgetop_toplong;
                                        if (gindex == -1) gindex = GetRdindex(group);
                                        DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty - 2, gindex);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (RT.Check(isomtile, TileBorder.DownBorder))
                            {
                                //코너인 경우
                                if (RT.Check(isomtile, TileBorder.DownBorder)
                                    && !T.Check(isomtile, TileBorder.FlatDownBorder)
                                    && !LT.Check(isomtile, TileBorder.FlatDownBorder))
                                {
                                    group = isomtile.edgetop_cornerslim;
                                    if (gindex == -1) gindex = GetRdindex(group);
                                }
                                else if (RT.Check(isomtile, TileBorder.DownBorder)
                                    && T.Check(isomtile, TileBorder.FlatDownBorder)
                                    && !LT.Check(isomtile, TileBorder.FlatDownBorder))
                                {
                                    group = isomtile.edgetop_cornerslimtop;
                                    if (gindex == -1) gindex = GetRdindex(group);
                                }
                                else if (RT.Check(isomtile, TileBorder.DownBorder)
                                    && !T.Check(isomtile, TileBorder.FlatDownBorder)
                                    && LT.Check(isomtile, TileBorder.FlatDownBorder))
                                {
                                    group = isomtile.edgetop_cornerslim;
                                    if (gindex == -1) gindex = GetRdindex(group);
                                }
                                else if (RT.Check(isomtile, TileBorder.FlatDownBorder)
                                    && T.Check(isomtile, TileBorder.FlatDownBorder)
                                    && LT.Check(isomtile, TileBorder.FlatDownBorder))
                                {
                                    group = isomtile.edgetop_corner;
                                    if (gindex == -1) gindex = GetRdindex(group);
                                }
                            }
                            else
                            {
                                if (!isomtile.IsMiniISOM)
                                {
                                    if (T.Check(isomtile, TileBorder.DownBorder) && !RTT.Check(isomtile, TileBorder.DownBorder))
                                    {
                                        group = isomtile.edgetop_top;
                                        if (gindex == -1) gindex = GetRdindex(group);
                                        DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty - 2, gindex);
                                    }
                                    else if (T.Check(isomtile, TileBorder.DownBorder) && RTT.Check(isomtile, TileBorder.DownBorder))
                                    {
                                        group = isomtile.edgetop_toplong;
                                        if (gindex == -1) gindex = GetRdindex(group);
                                        DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty - 2, gindex);
                                    }
                                }
                            }
                        }


                        if (group == null)
                        {
                            group = isomtile.edgetop_default;
                        }


                        if (gindex == -1) gindex = GetRdindex(group);

                        if (direction == DrawDirection.Left)
                        {
                            drawx -= 2;
                            drawy -= 1;
                        }
                        else
                        {
                            drawx += 0;
                            drawy -= 1;
                        }


                        DrawISOMTile(mapeditor, group, direction, drawx, drawy, gindex);


                        if (isomtile.ConnectHighTile != null && lowtile != null)
                        {
                            if (direction == DrawDirection.Left)
                            {
                                if (L.Check(lowtile, TileBorder.FlatDownBorder)
                                    && LT.Check(isomtile, TileBorder.FlatDownBorder)
                                    && checker.TileCheck(IWay.T, tx, ty).Check(isomtile.ConnectHighTile, TileBorder.FlatDownBorder)
                                    && checker.TileCheck(IWay.LTT, tx, ty).Check(isomtile.ConnectHighTile, TileBorder.FlatDownBorder))
                                {
                                    DrawISOMGroup(mapeditor, isomtile.ConnectHighTile, isomtile, checker, ISOMGroupType.cliff, direction, tx, ty - 2, gindex);
                                    DrawISOMTile(mapeditor, isomtile.cliff_downedge, direction, drawx, drawy, gindex);
                                }
                                else if (L.Check(lowtile, TileBorder.FlatDownBorder)
                                    && LT.Check(isomtile, TileBorder.FlatDownBorder)
                                    && checker.TileCheck(IWay.T, tx, ty).Check(isomtile.ConnectHighTile, TileBorder.FlatDownBorder)
                                    && checker.TileCheck(IWay.LTT, tx, ty).Check(isomtile, TileBorder.FlatDownBorder))
                                {
                                    DrawISOMGroup(mapeditor, isomtile.ConnectHighTile, isomtile, checker, ISOMGroupType.tip, direction, tx, ty - 2, gindex);
                                    DrawISOMTile(mapeditor, isomtile.tip_downedge, direction, drawx, drawy, gindex);
                                }
                            }
                            else
                            {
                                if (R.Check(lowtile, TileBorder.FlatDownBorder)
                                    && RT.Check(isomtile, TileBorder.FlatDownBorder)
                                    && checker.TileCheck(IWay.T, tx, ty).Check(isomtile.ConnectHighTile, TileBorder.FlatDownBorder)
                                    && checker.TileCheck(IWay.RTT, tx, ty).Check(isomtile.ConnectHighTile, TileBorder.FlatDownBorder))
                                {
                                    DrawISOMGroup(mapeditor, isomtile.ConnectHighTile, isomtile, checker, ISOMGroupType.cliff, direction, tx, ty - 2, gindex);
                                    DrawISOMTile(mapeditor, isomtile.cliff_downedge, direction, drawx, drawy, gindex);
                                }else if (R.Check(lowtile, TileBorder.FlatDownBorder)
                                    && RT.Check(isomtile, TileBorder.FlatDownBorder)
                                    && checker.TileCheck(IWay.T, tx, ty).Check(isomtile.ConnectHighTile, TileBorder.FlatDownBorder)
                                    && checker.TileCheck(IWay.RTT, tx, ty).Check(isomtile, TileBorder.FlatDownBorder))
                                {
                                    DrawISOMGroup(mapeditor, isomtile.ConnectHighTile, isomtile, checker, ISOMGroupType.tip, direction, tx, ty - 2, gindex);
                                    DrawISOMTile(mapeditor, isomtile.tip_downedge, direction, drawx, drawy, gindex);
                                }
                            }



                        }



                        group = null;
                        if (direction == DrawDirection.Left)
                        {
                            if (LT.Check(isomtile, TileBorder.DownBorder))
                            {
                                //위에 바로 있으면 코너지형
                                if (!B.Check(isomtile, TileBorder.DownBorder)
                                    && (!RB.Check(isomtile, TileBorder.FlatDownBorder) || !B.Check(isomtile, TileBorder.FlatDownBorder)))
                                {
                                    group = isomtile.edgebottom_cornerslim;
                                    if (gindex == -1) gindex = GetRdindex(group);
                                    DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty + 1, gindex);
                                }
                                else if (B.Check(isomtile, TileBorder.DownBorder)
                                    && (!RB.Check(isomtile, TileBorder.FlatDownBorder) || !B.Check(isomtile, TileBorder.FlatDownBorder)))
                                {
                                    group = isomtile.edgebottom_cornerslimbottom;
                                    if (gindex == -1) gindex = GetRdindex(group);
                                    DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty + 1, gindex);
                                }


                                if (group == null) group = isomtile.edgebottom_corner;
                            }
                            else
                            {
                                if (!B.Check(isomtile, TileBorder.DownBorder)
                                    && (!RB.Check(isomtile, TileBorder.FlatDownBorder) || !B.Check(isomtile, TileBorder.FlatDownBorder)))
                                {
                                    group = isomtile.edgebottom_slim;
                                    if (gindex == -1) gindex = GetRdindex(group);
                                    DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty + 1, gindex);
                                }
                                else if (B.Check(isomtile, TileBorder.DownBorder)
                                    && (!RB.Check(isomtile, TileBorder.FlatDownBorder) || !B.Check(isomtile, TileBorder.FlatDownBorder)))
                                {
                                    group = isomtile.edgebottom_slimbottom;
                                    if (gindex == -1) gindex = GetRdindex(group);
                                    DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty + 1, gindex);
                                }
                            }


                        }
                        else
                        {
                            if (RT.Check(isomtile, TileBorder.DownBorder))
                            {
                                //위에 바로 있으면 코너지형
                                if (!B.Check(isomtile, TileBorder.DownBorder)
                                    && (!LB.Check(isomtile, TileBorder.FlatDownBorder) || !B.Check(isomtile, TileBorder.FlatDownBorder)))
                                {
                                    ;
                                    group = isomtile.edgebottom_cornerslim;
                                    if (gindex == -1) gindex = GetRdindex(group);
                                    DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty + 1, gindex);
                                }
                                else if (B.Check(isomtile, TileBorder.DownBorder)
                                    && (!LB.Check(isomtile, TileBorder.FlatDownBorder) || !B.Check(isomtile, TileBorder.FlatDownBorder)))
                                {
                                    group = isomtile.edgebottom_cornerslimbottom;
                                    if (gindex == -1) gindex = GetRdindex(group);
                                    DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty + 1, gindex);
                                }


                                if (group == null) group = isomtile.edgebottom_corner;
                            }
                            else
                            {
                                if (!B.Check(isomtile, TileBorder.DownBorder)
                                    && (!LB.Check(isomtile, TileBorder.FlatDownBorder) || !B.Check(isomtile, TileBorder.FlatDownBorder)))
                                {
                                    group = isomtile.edgebottom_slim;
                                    if (gindex == -1) gindex = GetRdindex(group);
                                    DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty + 1, gindex);
                                }
                                else if (B.Check(isomtile, TileBorder.DownBorder)
                                    && (!LB.Check(isomtile, TileBorder.FlatDownBorder) || !B.Check(isomtile, TileBorder.FlatDownBorder)))
                                {
                                    group = isomtile.edgebottom_slimbottom;
                                    if (gindex == -1) gindex = GetRdindex(group);
                                    DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty + 1, gindex);
                                }
                            }

                        }

                        if (group == null)
                        {
                            group = isomtile.edgebottom_default;
                        }
                        if (gindex == -1) gindex = GetRdindex(group);

                        if (direction == DrawDirection.Left)
                        {
                            drawx += 0;
                            drawy += 1;
                        }
                        else
                        {
                            drawx += 0;
                            drawy += 1;
                        }

                        DrawISOMTile(mapeditor, group, direction, drawx, drawy, gindex);
                        #endregion
                    }


                    break;
                case ISOMGroupType.cliff:
                    if (direction == DrawDirection.Left)
                    {
                        //if(!RB.Check(isomtile, TileBorder.DownBorder))
                        {
                            if (T.Check(lowtile, TileBorder.FlatDownBorder))
                            {
                                group = isomtile.cliff_slim;
                                if (gindex == -1) gindex = GetRdindex(group);
                                DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty - 1, gindex);
                            }
                            else if (T.Check(isomtile, TileBorder.DownBorder) && !RT.Check(isomtile, TileBorder.FlatDownBorder))
                            {
                                group = isomtile.cliff_slimtop;
                                if (gindex == -1) gindex = GetRdindex(group);
                                DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty - 1, gindex);
                            }
                        }
                    }
                    else
                    {
                        //if (!LB.Check(isomtile, TileBorder.DownBorder))
                        {
                            if (T.Check(lowtile, TileBorder.FlatDownBorder))
                            {
                                group = isomtile.cliff_slim;
                                if (gindex == -1) gindex = GetRdindex(group);
                                DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty - 1, gindex);
                            }
                            else if (T.Check(isomtile, TileBorder.DownBorder) && !LT.Check(isomtile, TileBorder.FlatDownBorder))
                            {
                                group = isomtile.cliff_slimtop;
                                if (gindex == -1) gindex = GetRdindex(group);
                                DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty - 1, gindex);
                            }
                        }
                    }

                    if (direction == DrawDirection.Left)
                    {
                        drawx -= 2;
                        drawy -= 1;
                    }
                    else
                    {
                        drawx += 0;
                        drawy -= 1;
                    }

                    if (group == null)
                    {
                        group = isomtile.cliff_default;
                    }
                    if (gindex == -1) gindex = GetRdindex(group);

                    DrawISOMTile(mapeditor, group, direction, drawx, drawy, gindex);

                    if (!isomtile.IsMiniISOM && lowtile != null)
                    {
                        if (lowtile.ConnectLowTile != null)
                        {
                            if (direction == DrawDirection.Left)
                            {
                                if ((BB.Check(lowtile.ConnectLowTile, TileBorder.FlatDownBorder) || BB.Check(lowtile, TileBorder.DownBorder))
                                    && LBB.Check(lowtile.ConnectLowTile, TileBorder.FlatDownBorder))
                                {
                                    DrawISOMGroup(mapeditor, lowtile, lowtile.ConnectLowTile, checker, ISOMGroupType.cliff, direction, tx, ty + 2, gindex);
                                    DrawISOMTile(mapeditor, lowtile.cliff_down, direction, drawx, drawy + 2, gindex);
                                }
                                else if (B.Check(lowtile, TileBorder.DownBorder)
                                    && LB.Check(lowtile, TileBorder.DownBorder)
                                    && LBB.Check(lowtile, TileBorder.DownBorder)
                                    && LLBB.Check(lowtile.ConnectLowTile, TileBorder.Flat))
                                {
                                    DrawISOMGroup(mapeditor, lowtile, lowtile.ConnectLowTile, checker, ISOMGroupType.edge, direction, tx, ty + 2, gindex);
                                    DrawISOMTile(mapeditor, lowtile.cliff_downedge, direction, drawx, drawy + 2, gindex);
                                }
                            }
                            else
                            {
                                if ((BB.Check(lowtile.ConnectLowTile, TileBorder.FlatDownBorder) || BB.Check(lowtile, TileBorder.DownBorder))
                                    && RBB.Check(lowtile.ConnectLowTile, TileBorder.FlatDownBorder))
                                {
                                    DrawISOMGroup(mapeditor, lowtile, lowtile.ConnectLowTile, checker, ISOMGroupType.cliff, direction, tx, ty + 2, gindex);
                                    DrawISOMTile(mapeditor, lowtile.cliff_down, direction, drawx, drawy + 2, gindex);
                                }
                                else if (B.Check(lowtile, TileBorder.DownBorder)
                                    && RB.Check(lowtile, TileBorder.DownBorder)
                                    && RBB.Check(lowtile, TileBorder.DownBorder)
                                    && RRBB.Check(lowtile.ConnectLowTile, TileBorder.Flat))
                                {
                                    DrawISOMGroup(mapeditor, lowtile, lowtile.ConnectLowTile, checker, ISOMGroupType.edge, direction, tx, ty + 2, gindex);
                                    DrawISOMTile(mapeditor, lowtile.cliff_downedge, direction, drawx, drawy + 2, gindex);
                                }
                            }
                        }
                        else if(isomtile.ConnectLowTile != null && isomtile.ConnectHighTile != null)
                        {
                            if (direction == DrawDirection.Left)
                            {
                                if ((B.Check(lowtile, TileBorder.FlatDownBorder) || B.Check(isomtile, TileBorder.DownBorder))
                                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                                    && checker.TileCheck(IWay.T, tx, ty).Check(isomtile.ConnectHighTile, TileBorder.DownBorder)
                                    //&& RB.Check(isomtile, TileBorder.DownBorder)
                                    && checker.TileCheck(IWay.LTT, tx, ty).Check(isomtile.ConnectHighTile, TileBorder.DownBorder))
                                {
                                    DrawISOMGroup(mapeditor, isomtile.ConnectHighTile, isomtile, checker, ISOMGroupType.cliff, direction, tx, ty - 2, gindex);
                                    DrawISOMTile(mapeditor, isomtile.cliff_down, direction, drawx, drawy, gindex);
                                }
                                else if ((B.Check(lowtile, TileBorder.FlatDownBorder) || B.Check(isomtile, TileBorder.DownBorder))
                                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                                    && checker.TileCheck(IWay.T, tx, ty).Check(isomtile.ConnectHighTile, TileBorder.DownBorder)
                                    && LT.Check(isomtile, TileBorder.FlatDownBorder)
                                    && checker.TileCheck(IWay.LTT, tx, ty).Check(isomtile, TileBorder.FlatDownBorder))
                                {
                                    DrawISOMGroup(mapeditor, isomtile.ConnectHighTile, isomtile, checker, ISOMGroupType.tip, direction, tx, ty - 2, gindex);
                                    DrawISOMTile(mapeditor, isomtile.tip_down, direction, drawx, drawy, gindex);
                                }
                            }
                            else
                            {
                                if ((B.Check(lowtile, TileBorder.FlatDownBorder) || B.Check(isomtile, TileBorder.DownBorder))
                                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                                    && checker.TileCheck(IWay.T, tx, ty).Check(isomtile.ConnectHighTile, TileBorder.DownBorder)
                                    //&& LB.Check(isomtile, TileBorder.DownBorder)
                                    && checker.TileCheck(IWay.RTT, tx, ty).Check(isomtile.ConnectHighTile, TileBorder.DownBorder))
                                {
                                    DrawISOMGroup(mapeditor, isomtile.ConnectHighTile, isomtile, checker, ISOMGroupType.cliff, direction, tx, ty - 2, gindex);
                                    DrawISOMTile(mapeditor, isomtile.cliff_down, direction, drawx, drawy, gindex);
                                }
                                else if ((B.Check(lowtile, TileBorder.FlatDownBorder) || B.Check(isomtile, TileBorder.DownBorder))
                                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                                    && checker.TileCheck(IWay.T, tx, ty).Check(isomtile.ConnectHighTile, TileBorder.DownBorder)
                                    && RT.Check(isomtile, TileBorder.FlatDownBorder)
                                    && checker.TileCheck(IWay.RTT, tx, ty).Check(isomtile, TileBorder.FlatDownBorder))
                                {
                                    DrawISOMGroup(mapeditor, isomtile.ConnectHighTile, isomtile, checker, ISOMGroupType.tip, direction, tx, ty - 2, gindex);
                                    DrawISOMTile(mapeditor, isomtile.tip_down, direction, drawx, drawy, gindex);
                                }
                            }
                        }
                      
                    }

                    if (direction == DrawDirection.Left)
                    {
                        if (B.Check(isomtile, TileBorder.DownBorder))
                        {
                            if (LBB.Check(isomtile, TileBorder.DownBorder))
                            {
                                DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty + 2, gindex);
                            }
                            else
                            {
                                DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty + 2, gindex);
                            }
                        }
                    }
                    else
                    {
                        if (B.Check(isomtile, TileBorder.DownBorder))
                        {
                            if (RBB.Check(isomtile, TileBorder.DownBorder))
                            {
                                DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty + 2, gindex);
                            }
                            else
                            {
                                DrawISOMGroup(mapeditor, isomtile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty + 2, gindex);
                            }
                        }


                    }

                    break;
            }
        }
    }
}
