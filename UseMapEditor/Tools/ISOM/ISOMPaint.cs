using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseMapEditor.Control;
using UseMapEditor.FileData;
using static BondTech.HotKeyManagement.WPF._4.HotKeyManager;
using static UseMapEditor.FileData.ISOMTile;
using static UseMapEditor.Tools.ISOMTool;

namespace UseMapEditor.Tools
{
    public partial class ISOMTool
    {
        public static void ISOMLeftFill(MapEditor mapeditor, ISOMTile uptile, ISOMTile lowtile, ISOMChecker checker, TileType L, TileType LLT, TileType LLTT, TileType LLB, TileType LLBB, int tx, int ty)
        {
            if(!L.Check(uptile, TileBorder.FlatDownBorder))
            {
                return;
            }
            //else
            //{
            //    if (tx == 2)
            //    {
            //        //왼쪽이나 끝쪽일경우
            //    }
            //    else
            //    {
            //        return;
            //    }
            //}

            if (LLT.Check(uptile, TileBorder.FlatDownBorder) && LLTT.Check(uptile, TileBorder.FlatDownBorder))
            {
                DrawISOMFlatTile(mapeditor, uptile, DrawDirection.Left, tx - 2, ty - 1);
            }
            else
            {
                DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx - 2, ty - 1);
            }

            if (LLB.Check(uptile, TileBorder.FlatDownBorder) && LLBB.Check(uptile, TileBorder.FlatDownBorder))
            {
                DrawISOMFlatTile(mapeditor, uptile, DrawDirection.Left, tx - 2, ty);
            }
            else
            {
                if(LLBB.Check(uptile, TileBorder.FlatDownBorder))
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx - 2, ty + 1);
                }
                else
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx - 2, ty + 1);
                }
            }

            if (uptile.ConnectHighTile != null && checker.TileCheck(IWay.LLTT, tx, ty).Check(uptile.ConnectHighTile, TileBorder.DownBorder)
                 && checker.TileCheck(IWay.LTT, tx, ty).Check(uptile.ConnectHighTile, TileBorder.DownBorder))
            {
                DrawISOMGroup(mapeditor, uptile.ConnectHighTile, uptile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 4, ty - 2);
            }

            if (uptile.ConnectHighTile != null)
            {
                if (checker.TileCheck(IWay.LLTT, tx, ty).Check(uptile.ConnectHighTile, TileBorder.DownBorder) && checker.TileCheck(IWay.LTT, tx, ty).Check(uptile.ConnectHighTile, TileBorder.DownBorder))
                {
                    DrawISOMGroup(mapeditor, uptile.ConnectHighTile, uptile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 4, ty - 2);
                }
                else if (checker.TileCheck(IWay.LTT, tx, ty).Check(uptile.ConnectHighTile, TileBorder.DownBorder)
                    && checker.TileCheck(IWay.LTT, tx - 2, ty - 1).Check(uptile.ConnectHighTile, TileBorder.DownBorder))
                {
                    DrawISOMGroup(mapeditor, uptile.ConnectHighTile, uptile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx - 2, ty - 3);
                }
                else if (checker.TileCheck(IWay.LLTT, tx, ty).Check(uptile.ConnectHighTile, TileBorder.DownBorder))
                {
                    DrawISOMGroup(mapeditor, uptile.ConnectHighTile, uptile, checker, ISOMGroupType.tip, DrawDirection.Right, tx - 4, ty - 2);
                }
            }
        }

        public static void ISOMRightFill(MapEditor mapeditor, ISOMTile uptile, ISOMTile lowtile, ISOMChecker checker, TileType R, TileType RRT, TileType RRTT, TileType RRB, TileType RRBB, int tx, int ty)
        {
            if (!R.Check(uptile, TileBorder.FlatDownBorder))
            {
                return;
            }

            if (RRT.Check(uptile, TileBorder.FlatDownBorder) && RRTT.Check(uptile, TileBorder.FlatDownBorder))
            {
                DrawISOMFlatTile(mapeditor, uptile, DrawDirection.Right, tx + 2, ty - 1);
            }
            else
            {
                DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx + 2, ty - 1);
            }

            if (RRB.Check(uptile, TileBorder.FlatDownBorder) && RRBB.Check(uptile, TileBorder.FlatDownBorder))
            {
                DrawISOMFlatTile(mapeditor, uptile, DrawDirection.Right, tx + 2, ty);
            }
            else
            {
                if (RRBB.Check(uptile, TileBorder.FlatDownBorder))
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx + 2, ty + 1);
                }
                else
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx + 2, ty + 1);
                }
            }

            if (uptile.ConnectHighTile != null)
            {
                if (checker.TileCheck(IWay.RRTT, tx, ty).Check(uptile.ConnectHighTile, TileBorder.DownBorder) && checker.TileCheck(IWay.RTT, tx, ty).Check(uptile.ConnectHighTile, TileBorder.DownBorder))
                {
                    DrawISOMGroup(mapeditor, uptile.ConnectHighTile, uptile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 4, ty - 2);
                }
                else if (checker.TileCheck(IWay.RTT, tx, ty).Check(uptile.ConnectHighTile, TileBorder.DownBorder)
                    && checker.TileCheck(IWay.RTT, tx + 2, ty - 1).Check(uptile.ConnectHighTile, TileBorder.DownBorder))
                {
                    DrawISOMGroup(mapeditor, uptile.ConnectHighTile, uptile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx + 2, ty - 3);
                }
                else if (checker.TileCheck(IWay.RRTT, tx, ty).Check(uptile.ConnectHighTile, TileBorder.DownBorder))
                {
                    DrawISOMGroup(mapeditor, uptile.ConnectHighTile, uptile, checker, ISOMGroupType.tip, DrawDirection.Left, tx + 4, ty - 2);
                }
            }
        }

        public static void ISOMUpFill(MapEditor mapeditor, ISOMTile uptile, ISOMTile lowtile, ISOMChecker checker, TileType LTT, TileType LLTT, TileType RTT, TileType RRTT, int tx, int ty)
        {
            if (LLTT.Check(uptile, TileBorder.FlatDownBorder) && LTT.Check(uptile, TileBorder.FlatDownBorder))
            {
                DrawISOMFlatTile(mapeditor, uptile, DrawDirection.Left, tx, ty - 2);
            }
            else
            {
                DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty - 2);
            }

            if (RRTT.Check(uptile, TileBorder.FlatDownBorder) && RTT.Check(uptile, TileBorder.FlatDownBorder))
            {
                DrawISOMFlatTile(mapeditor, uptile, DrawDirection.Right, tx, ty - 2);
            }
            else
            {
                DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty - 2);
            }

            if(uptile.ConnectHighTile != null)
            {
                if(checker.TileCheck(IWay.TT, tx, ty).Check(uptile.ConnectHighTile, TileBorder.DownBorder))
                {
                    if (checker.TileCheck(IWay.LTT, tx, ty).Check(uptile.ConnectHighTile, TileBorder.DownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile.ConnectHighTile, uptile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty - 3);
                    }


                    if (checker.TileCheck(IWay.RTT, tx, ty).Check(uptile.ConnectHighTile, TileBorder.DownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile.ConnectHighTile, uptile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty - 3);
                    }
                }
                else
                {
                    if (checker.TileCheck(IWay.LTT, tx, ty).Check(uptile.ConnectHighTile, TileBorder.DownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile.ConnectHighTile, uptile, checker, ISOMGroupType.tip, DrawDirection.Right, tx - 2, ty - 3);
                    }


                    if (checker.TileCheck(IWay.RTT, tx, ty).Check(uptile.ConnectHighTile, TileBorder.DownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile.ConnectHighTile, uptile, checker, ISOMGroupType.tip, DrawDirection.Left, tx + 2, ty - 3);
                    }
                }


            }
        }

        public static void ISOMDownFill(MapEditor mapeditor, ISOMTile uptile, ISOMTile lowtile, ISOMChecker checker, TileType LBB, TileType LLBB, TileType RBB, TileType RRBB, int tx, int ty)
        {
            if (LBB.Check(uptile, TileBorder.FlatDownBorder))
            {
                if (LLBB.Check(uptile, TileBorder.FlatDownBorder))
                {
                    DrawISOMFlatTile(mapeditor, uptile, DrawDirection.Left, tx, ty + 1);
                }
                else
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty + 2);
                }
            }
            else
            {
                DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty + 2);
            }


            if (RBB.Check(uptile, TileBorder.FlatDownBorder))
            {
                if (RRBB.Check(uptile, TileBorder.FlatDownBorder))
                {
                    DrawISOMFlatTile(mapeditor, uptile, DrawDirection.Right, tx, ty + 1);
                }
                else
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty + 2);
                }
            }
            else
            {
                DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty + 2);
            }
        }


        public static void ISOMDownWaterFill(MapEditor mapeditor, ISOMTile uptile, ISOMTile lowtile, ISOMChecker checker, TileType LBB, TileType RBB, TileType LLBB, TileType RRBB, int tx, int ty)
        {
            if (lowtile.ConnectLowTile != null)
            {
                if (lowtile.ConnectLowTile == RBB.Tile)
                {
                    DrawISOMGroup(mapeditor, lowtile, lowtile.ConnectLowTile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty + 2);
                }
                else if (lowtile.ConnectLowTile == RRBB.Tile)
                {
                    DrawISOMGroup(mapeditor, lowtile, lowtile.ConnectLowTile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty + 2);
                }
                if (lowtile.ConnectLowTile == LBB.Tile)
                {
                    DrawISOMGroup(mapeditor, lowtile, lowtile.ConnectLowTile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty + 2);
                }
                else if (lowtile.ConnectLowTile == LLBB.Tile)
                {
                    DrawISOMGroup(mapeditor, lowtile, lowtile.ConnectLowTile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty + 2);
                }
            }
        }




        /// <summary>
        /// 실질적으로 ISOM작업을 수행하는 함수
        /// </summary>
        /// 
        public static bool ISOMPaint(MapEditor mapeditor, TileSet tileSet, ISOMTile uptile, ISOMTile lowtile, ISOMChecker checker, int tx, int ty, bool IsTileStack, bool IsTileChange)
        {
            TileType C = checker.TileCheck(IWay.C, tx, ty);
            TileType L = checker.TileCheck(IWay.L, tx, ty);
            TileType LT = checker.TileCheck(IWay.LT, tx, ty);
            TileType T = checker.TileCheck(IWay.T, tx, ty);
            TileType RT = checker.TileCheck(IWay.RT, tx, ty);
            TileType R = checker.TileCheck(IWay.R, tx, ty);
            TileType RB = checker.TileCheck(IWay.RB, tx, ty);
            TileType LB = checker.TileCheck(IWay.LB, tx, ty);
            TileType B = checker.TileCheck(IWay.B, tx, ty);

            TileType LTT = checker.TileCheck(IWay.LTT, tx, ty);
            TileType RTT = checker.TileCheck(IWay.RTT, tx, ty);
            TileType RBB = checker.TileCheck(IWay.RBB, tx, ty);
            TileType LBB = checker.TileCheck(IWay.LBB, tx, ty);

            TileType RRBB = checker.TileCheck(IWay.RRBB, tx, ty);
            TileType LLBB = checker.TileCheck(IWay.LLBB, tx, ty);
            TileType RRTT = checker.TileCheck(IWay.RRTT, tx, ty);
            TileType LLTT = checker.TileCheck(IWay.LLTT, tx, ty);

            TileType RRT = checker.TileCheck(IWay.RRT, tx, ty);
            TileType LLT = checker.TileCheck(IWay.LLT, tx, ty);
            TileType RRB = checker.TileCheck(IWay.RRB, tx, ty);
            TileType LLB = checker.TileCheck(IWay.LLB, tx, ty);

            //TileType LLBBW = checker.TileCheck(IWay.LLBB, tx, ty);
            //TileType LLBW = checker.TileCheck(IWay.LLB, tx, ty);
            //TileType BBW = checker.TileCheck(IWay.BB, tx, ty);
            //TileType RRBW = checker.TileCheck(IWay.RRB, tx, ty);
            //TileType RRBBW = checker.TileCheck(IWay.RRBB, tx, ty);

            List<TileType> list = new List<TileType>();
            list.Add(C);
            list.Add(L);
            list.Add(LT);
            list.Add(T);
            list.Add(RT);
            list.Add(R);
            list.Add(RB);
            list.Add(LB);
            list.Add(B);

            list.Add(LTT);
            list.Add(RTT);
            list.Add(RBB);
            list.Add(LBB);

            list.Add(RRBB);
            list.Add(LLBB);
            list.Add(RRTT);
            list.Add(LLTT);

            list.Add(RRT);
            list.Add(LLT);
            list.Add(RRB);
            list.Add(LLB);

            if (uptile != lowtile)
            {
                if (IsTileStack)
                {
                    //더 높은 타일을 낮춰야됨
                    foreach (var item in list)
                    {
                        if (item.Tile == null) continue;

                        if (item.Tile.elevation > uptile.elevation)
                        {
                            //고도가 높을 경우
                            item.Tile = uptile;
                            item.Border = TileBorder.Flat;
                        }
                    }
                }
                else
                {
                    //다른 높은 타일을 낮춰야됨
                    //foreach (var item in list)
                    //{
                    //    if (item.Tile == null) continue;
                    //    if (item.Tile == uptile) continue;
                    //    if (item.Tile == lowtile) continue;

                    //    if (item.Tile.elevation > lowtile.elevation)
                    //    {
                    //        //고도가 높을 경우
                    //        item.Tile = lowtile;
                    //        item.Border = TileBorder.Flat;
                    //    }
                    //}
                }
            }



            if (IsTileStack)
            {
                #region 기둥 세우기
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && (T.Check(lowtile, TileBorder.FlatDownBorder) || T.Check(uptile, TileBorder.DownBorder))
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && (B.Check(lowtile, TileBorder.FlatDownBorder) || B.Check(uptile, TileBorder.DownBorder))
                    )
                {
                    if (T.Check(uptile, TileBorder.DownBorder) && LTT.Check(uptile, TileBorder.DownBorder))
                    {
                        int gindex = GetRdindex(uptile.tip_default);
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty - 2, gindex);
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty, gindex);
                    }
                    else
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty);
                    }


                    if (T.Check(uptile, TileBorder.DownBorder) && RTT.Check(uptile, TileBorder.DownBorder))
                    {
                        int gindex = GetRdindex(uptile.tip_default);
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty - 2, gindex);
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty, gindex);
                    }
                    else
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty);
                    }
                    return true;
                }
                #endregion

                #region 두개 절벽 세우기
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty);
                    return true;
                }

                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty);
                    return true;
                }

                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty + 1);
                    return true;
                }

                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty + 1);
                    return true;
                }
                #endregion

                #region 3개 절벽 세우기
                //=====================================왼쪽 오른쪽 절벽=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty);
                    return true;
                }

                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty + 1);
                    return true;
                }
                //=====================================왼쪽 오른쪽 절벽=====================================

                //=====================================위 아래 절벽=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    int gindex = GetRdindex(uptile.cliff_default);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty - 1, gindex);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty + 1, gindex);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty, gindex);

                    if (IsTileChange)
                    {
                        ISOMLeftFill(mapeditor, uptile, lowtile, checker, L, LLT, LLTT, LLB, LLBB, tx, ty);
                        //ISOMExecute(mapeditor, tileSet, uptile, tx - 4, ty);
                    }


                    return true;
                }

                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    int gindex = GetRdindex(uptile.cliff_default);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty - 1, gindex);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty + 1, gindex);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty, gindex);

                    if (IsTileChange)
                    {
                        ISOMRightFill(mapeditor, uptile, lowtile, checker, R, RRT, RRTT, RRB, RRBB, tx, ty);
                    }
                    return true;
                }
                //=====================================위 아래 절벽=====================================


                //=====================================오른쪽 아래 붙은 절벽=====================================
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    int gindex = GetRdindex(uptile.cliff_default);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty, gindex);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty, gindex);
                    return true;
                }


                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    int gindex = GetRdindex(uptile.cliff_default);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty, gindex);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty, gindex);
                    return true;
                }
                //=====================================오른쪽 아래 붙은 절벽=====================================


                //=====================================왼쪽 아래 붙은 절벽=====================================
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    int gindex = GetRdindex(uptile.cliff_default);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty, gindex);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty, gindex);
                    return true;
                }


                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    int gindex = GetRdindex(uptile.cliff_default);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty, gindex);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty, gindex);
                    return true;
                }
                //=====================================왼쪽 아래 붙은 절벽=====================================

                //=====================================오른쪽 위 왼쪽 아래 절벽=====================================
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty + 1);
                    return true;
                }
                //=====================================오른쪽 위 왼쪽 아래 절벽=====================================

                //=====================================왼쪽 위 오른쪽 아래 절벽=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty + 1);
                    return true;
                }
                //=====================================왼쪽 위 오른쪽 아래 절벽=====================================

                //=====================================위 왼쪽 아래 절벽=====================================
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty + 1);
                    return true;
                }
                //=====================================위 왼쪽 아래 절벽=====================================

                //=====================================위 오른쪽 아래 절벽=====================================
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty + 1);
                    return true;
                }
                //=====================================위 오른쪽 아래 절벽=====================================

                //=====================================왼쪽 위 아래 절벽=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty);
                    return true;
                }
                //=====================================왼쪽 위 아래 절벽=====================================

                //=====================================오른쪽 위 아래 절벽=====================================
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty);
                    return true;
                }
                //=====================================오른쪽 위 아래 절벽=====================================

                #endregion

                #region 4개 절벽 세우기
                //=====================================⠁⠁⠁=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(uptile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty);
                    if (IsTileChange)
                    {
                        ISOMUpFill(mapeditor, uptile, lowtile, checker, LTT, LLTT, RTT, RRTT, tx, ty);
                    }
                    return true;
                }
                //=====================================⠁⠁⠁=====================================

                //=====================================⠄⠄⠄=====================================
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(uptile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty);
                    if (IsTileChange)
                    {
                        ISOMDownFill(mapeditor, uptile, lowtile, checker, LBB, LLBB, RBB, RRBB, tx, ty);
                    }
                    return true;
                }
                //=====================================⠄⠄⠄=====================================

                //=====================================⠁⠄⠁=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty);
                    return true;
                }
                //=====================================⠁⠄⠁=====================================

                //=====================================⠄⠁⠄=====================================
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty + 1);
                    return true;
                }
                //=====================================⠄⠁⠄=====================================

                //=====================================⠅⠀⠄=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty - 1);
                    if (IsTileChange)
                    {
                        ISOMLeftFill(mapeditor, uptile, lowtile, checker, L, LLT, LLTT, LLB, LLBB, tx, ty);
                    }
                    return true;
                }
                //=====================================⠅⠀⠄=====================================

                //=====================================⠄⠀⠅=====================================
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty - 1);
                    if (IsTileChange)
                    {
                        ISOMRightFill(mapeditor, uptile, lowtile, checker, R, RRT, RRTT, RRB, RRBB, tx, ty);
                    }
                    return true;
                }
                //=====================================⠄⠀⠅=====================================

                //=====================================⠅⠀⠁=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty - 1);
                    if (IsTileChange)
                    {
                        ISOMLeftFill(mapeditor, uptile, lowtile, checker, L, LLT, LLTT, LLB, LLBB, tx, ty);
                    }
                    return true;
                }
                //=====================================⠅⠀⠁=====================================

                //=====================================⠁⠀⠅=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty - 1);
                    if (IsTileChange)
                    {
                        ISOMRightFill(mapeditor, uptile, lowtile, checker, R, RRT, RRTT, RRB, RRBB, tx, ty);
                    }
                    return true;
                }
                //=====================================⠁⠀⠅=====================================


                //=====================================⠅⠁⠀=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty - 1);
                    if (IsTileChange)
                    {
                        ISOMLeftFill(mapeditor, uptile, lowtile, checker, L, LLT, LLTT, LLB, LLBB, tx, ty);
                    }
                    return true;
                }
                //=====================================⠅⠁⠀=====================================

                //=====================================⠁⠅⠀=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty);
                    return true;
                }
                //=====================================⠁⠅⠀=====================================

                //=====================================⠁⠁⠄=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty);
                    return true;
                }
                //=====================================⠁⠁⠄=====================================


                //=====================================⠅⠄⠀=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty + 1);
                    if (IsTileChange)
                    {
                        ISOMLeftFill(mapeditor, uptile, lowtile, checker, L, LLT, LLTT, LLB, LLBB, tx, ty);
                    }
                    return true;
                }
                //=====================================⠅⠄⠀=====================================

                //=====================================⠄⠅⠀=====================================
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty);
                    return true;
                }
                //=====================================⠄⠅⠀=====================================

                //=====================================⠄⠄⠁=====================================
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty);
                    return true;
                }
                //=====================================⠄⠄⠁=====================================


                //=====================================⠀⠁⠅=====================================
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty - 1);
                    if (IsTileChange)
                    {
                        ISOMRightFill(mapeditor, uptile, lowtile, checker, R, RRT, RRTT, RRB, RRBB, tx, ty);
                    }
                    return true;
                }
                //=====================================⠀⠁⠅=====================================

                //=====================================⠀⠅⠁=====================================
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty);
                    return true;
                }
                //=====================================⠀⠅⠁=====================================

                //=====================================⠄⠁⠁=====================================
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty);
                    return true;
                }
                //=====================================⠄⠁⠁=====================================


                //=====================================⠀⠄⠅=====================================
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty + 1);
                    if (IsTileChange)
                    {
                        ISOMRightFill(mapeditor, uptile, lowtile, checker, R, RRT, RRTT, RRB, RRBB, tx, ty);
                    }
                    return true;
                }
                //=====================================⠀⠄⠅=====================================

                //=====================================⠀⠅⠄=====================================
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty);
                    return true;
                }
                //=====================================⠀⠅⠄=====================================

                //=====================================⠁⠄⠄=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty);
                    return true;
                }
                //=====================================⠁⠄⠄=====================================
                #endregion

                #region 5개 절벽 세우기
                //=====================================⠅⠀⠅=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty + 1);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty + 1);

                    if (IsTileChange)
                    {
                        ISOMRightFill(mapeditor, uptile, lowtile, checker, R, RRT, RRTT, RRB, RRBB, tx, ty);
                        ISOMLeftFill(mapeditor, uptile, lowtile, checker, L, LLT, LLTT, LLB, LLBB, tx, ty);
                    }
                    return true;
                }
                //=====================================⠅⠀⠅=====================================

                //=====================================⠅⠁⠁=====================================
                if (LT.Check(uptile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty + 1);
                    if (L.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        if (IsTileChange)
                        {
                            DrawISOMFlatTile(mapeditor, uptile, DrawDirection.Left, tx, ty - 1);
                        }
                    }

                    if (IsTileChange)
                    {
                        ISOMLeftFill(mapeditor, uptile, lowtile, checker, L, LLT, LLTT, LLB, LLBB, tx, ty);
                        ISOMUpFill(mapeditor, uptile, lowtile, checker, LTT, LLTT, RTT, RRTT, tx, ty);
                    }
                    return true;
                }
                //=====================================⠅⠁⠁=====================================

                //=====================================⠁⠅⠁=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(uptile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty);


                    if (IsTileChange)
                    {
                        ISOMUpFill(mapeditor, uptile, lowtile, checker, LTT, LLTT, RTT, RRTT, tx, ty);
                    }
                    return true;
                }
                //=====================================⠁⠅⠁=====================================

                //=====================================⠁⠁⠅=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(uptile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    if (IsTileChange && uptile.IsMiniISOM && uptile.IsNoEdge)
                    {
                        DrawISOMFlatTile(mapeditor, uptile, DrawDirection.All, tx, ty - 1);
                    }
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty + 1);
                    if (R.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        if (IsTileChange)
                        {
                            DrawISOMFlatTile(mapeditor, uptile, DrawDirection.Right, tx, ty - 1);
                        }
                    }

                    if (IsTileChange)
                    {
                        ISOMRightFill(mapeditor, uptile, lowtile, checker, R, RRT, RRTT, RRB, RRBB, tx, ty);
                        ISOMUpFill(mapeditor, uptile, lowtile, checker, LTT, LLTT, RTT, RRTT, tx, ty);
                    }
                    return true;
                }
                //=====================================⠁⠁⠅=====================================


                //=====================================⠅⠄⠄=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty - 1);
                    if (L.Check(uptile, TileBorder.DownBorder))
                    {
                        if (IsTileChange)
                        {
                            DrawISOMFlatTile(mapeditor, uptile, DrawDirection.Left, tx, ty);
                        }
                    }
                    else
                    {
                        if (L.Check(lowtile, TileBorder.FlatDownBorder))
                        {
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty);
                        }
                    }

                    if (IsTileChange)
                    {
                        ISOMLeftFill(mapeditor, uptile, lowtile, checker, L, LLT, LLTT, LLB, LLBB, tx, ty);
                        ISOMDownFill(mapeditor, uptile, lowtile, checker, LBB, LLBB, RBB, RRBB, tx, ty);
                    }
                    if (IsTileChange && uptile.IsMiniISOM && uptile.IsNoEdge)
                    {
                        DrawISOMFlatTile(mapeditor, uptile, DrawDirection.All, tx, ty);
                    }
                    return true;
                }
                //=====================================⠅⠄⠄=====================================

                //=====================================⠄⠅⠄=====================================
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(uptile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty);


                    if (IsTileChange)
                    {
                        ISOMDownFill(mapeditor, uptile, lowtile, checker, LBB, LLBB, RBB, RRBB, tx, ty);
                    }
                    return true;
                }
                //=====================================⠄⠅⠄=====================================

                //=====================================⠄⠄⠅=====================================
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(uptile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty - 1);
                    if (R.Check(uptile, TileBorder.DownBorder))
                    {

                        if (IsTileChange)
                        {
                            DrawISOMFlatTile(mapeditor, uptile, DrawDirection.Right, tx, ty);
                        }
                    }
                    else
                    {
                        if (R.Check(lowtile, TileBorder.FlatDownBorder))
                        {
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty);
                        }
                    }

                    if (IsTileChange)
                    {
                        ISOMRightFill(mapeditor, uptile, lowtile, checker, R, RRT, RRTT, RRB, RRBB, tx, ty);
                        ISOMDownFill(mapeditor, uptile, lowtile, checker, LBB, LLBB, RBB, RRBB, tx, ty);
                    }
                    return true;
                }
                //=====================================⠄⠄⠅=====================================


                //=====================================⠅⠅⠀=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty);

                    if (IsTileChange)
                    {
                        ISOMLeftFill(mapeditor, uptile, lowtile, checker, L, LLT, LLTT, LLB, LLBB, tx, ty);
                    }
                    return true;
                }
                //=====================================⠅⠅⠀=====================================

                //=====================================⠀⠅⠅=====================================
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty);

                    if (IsTileChange)
                    {
                        ISOMRightFill(mapeditor, uptile, lowtile, checker, R, RRT, RRTT, RRB, RRBB, tx, ty);
                    }
                    return true;
                }
                //=====================================⠀⠅⠅=====================================


                //=====================================⠄⠅⠁=====================================
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty);
                    return true;
                }
                //=====================================⠄⠅⠁=====================================

                //=====================================⠁⠅⠄=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty);
                    return true;
                }
                //=====================================⠁⠅⠄=====================================


                //=====================================⠅⠁⠄=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty - 1);

                    if (IsTileChange)
                    {
                        ISOMLeftFill(mapeditor, uptile, lowtile, checker, L, LLT, LLTT, LLB, LLBB, tx, ty);
                    }
                    return true;
                }
                //=====================================⠅⠁⠄=====================================

                //=====================================⠄⠁⠅=====================================
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty - 1);

                    if (IsTileChange)
                    {
                        ISOMRightFill(mapeditor, uptile, lowtile, checker, R, RRT, RRTT, RRB, RRBB, tx, ty);
                    }
                    return true;
                }
                //=====================================⠄⠁⠅=====================================


                //=====================================⠅⠄⠁=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty - 1);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty + 1);
                    if (IsTileChange)
                    {
                        ISOMLeftFill(mapeditor, uptile, lowtile, checker, L, LLT, LLTT, LLB, LLBB, tx, ty);
                    }
                    return true;
                }
                //=====================================⠅⠄⠁=====================================

                //=====================================⠁⠄⠅=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty - 1);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty + 1);

                    if (IsTileChange)
                    {
                        ISOMRightFill(mapeditor, uptile, lowtile, checker, R, RRT, RRTT, RRB, RRBB, tx, ty);
                    }
                    return true;
                }
                //=====================================⠁⠄⠅=====================================
                #endregion

                #region 6개 절벽 세우기
                //=====================================⠄⠅⠅=====================================
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(uptile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty - 1);

                    if (IsTileChange && uptile.IsMiniISOM && uptile.IsNoEdge)
                    {
                        DrawISOMFlatTile(mapeditor, uptile, DrawDirection.All, tx, ty);
                    }

                    if (R.Check(uptile, TileBorder.DownBorder))
                    {
                        if (IsTileChange)
                        {
                            DrawISOMFlatTile(mapeditor, uptile, DrawDirection.Right, tx, ty);
                        }

                    }
                    else
                    {
                        if (RB.Check(uptile, TileBorder.DownBorder) && R.Check(lowtile, TileBorder.FlatDownBorder))
                        {
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty);
                        }
                    }

                    if (IsTileChange)
                    {
                        ISOMRightFill(mapeditor, uptile, lowtile, checker, R, RRT, RRTT, RRB, RRBB, tx, ty);
                        ISOMDownFill(mapeditor, uptile, lowtile, checker, LBB, LLBB, RBB, RRBB, tx, ty);
                    }
                    return true;
                }
                //=====================================⠄⠅⠅=====================================

                //=====================================⠅⠄⠅=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(uptile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty - 1);

                    if (IsTileChange && uptile.IsMiniISOM && uptile.IsNoEdge)
                    {
                        DrawISOMFlatTile(mapeditor, uptile, DrawDirection.All, tx, ty);
                    }

                    if (R.Check(uptile, TileBorder.DownBorder))
                    {
                        if (IsTileChange)
                        {
                            DrawISOMFlatTile(mapeditor, uptile, DrawDirection.Right, tx, ty);
                        }
                    }

                    if (L.Check(uptile, TileBorder.DownBorder))
                    {
                        if (IsTileChange)
                        {
                            DrawISOMFlatTile(mapeditor, uptile, DrawDirection.Left, tx, ty);

                        }
                    }

                    if (IsTileChange)
                    {
                        ISOMRightFill(mapeditor, uptile, lowtile, checker, R, RRT, RRTT, RRB, RRBB, tx, ty);
                        ISOMLeftFill(mapeditor, uptile, lowtile, checker, L, LLT, LLTT, LLB, LLBB, tx, ty);
                        ISOMDownFill(mapeditor, uptile, lowtile, checker, LBB, LLBB, RBB, RRBB, tx, ty);
                    }
                    return true;
                }
                //=====================================⠅⠄⠅=====================================

                //=====================================⠅⠅⠄=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty - 1);

                    if (IsTileChange && uptile.IsMiniISOM && uptile.IsNoEdge)
                    {
                        DrawISOMFlatTile(mapeditor, uptile, DrawDirection.All, tx, ty);
                    }

                    if (L.Check(uptile, TileBorder.DownBorder))
                    {
                        if (IsTileChange)
                        {
                            DrawISOMFlatTile(mapeditor, uptile, DrawDirection.Left, tx, ty);
                        }
                    }
                    else
                    {
                        if (LB.Check(uptile, TileBorder.DownBorder) && L.Check(lowtile, TileBorder.FlatDownBorder))
                        {
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty);
                        }
                    }

                    if (IsTileChange)
                    {
                        ISOMLeftFill(mapeditor, uptile, lowtile, checker, L, LLT, LLTT, LLB, LLBB, tx, ty);
                        ISOMDownFill(mapeditor, uptile, lowtile, checker, LBB, LLBB, RBB, RRBB, tx, ty);
                    }
                    return true;
                }
                //=====================================⠅⠅⠄=====================================

                //=====================================⠁⠅⠅=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(uptile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty + 1);


                    if (IsTileChange && uptile.IsMiniISOM && uptile.IsNoEdge)
                    {
                        DrawISOMFlatTile(mapeditor, uptile, DrawDirection.All, tx, ty - 1);
                    }

                    if (R.Check(uptile, TileBorder.DownBorder))
                    {
                        if (IsTileChange)
                        {
                            if (RRTT.Check(uptile, TileBorder.FlatDownBorder) && RRT.Check(uptile, TileBorder.FlatDownBorder))
                            {
                                DrawISOMFlatTile(mapeditor, uptile, DrawDirection.Right, tx + 2, ty - 1);
                            }
                            DrawISOMFlatTile(mapeditor, uptile, DrawDirection.Right, tx, ty - 1);
                        }
                    }
                    else if (R.Check(lowtile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty);
                    }

                    if (IsTileChange)
                    {
                        ISOMRightFill(mapeditor, uptile, lowtile, checker, R, RRT, RRTT, RRB, RRBB, tx, ty);
                        if (RRTT.Check(uptile, TileBorder.FlatDownBorder) && RTT.Check(uptile, TileBorder.FlatDownBorder))
                        {
                            DrawISOMFlatTile(mapeditor, uptile, DrawDirection.Right, tx, ty - 2);
                        }
                        ISOMUpFill(mapeditor, uptile, lowtile, checker, LTT, LLTT, RTT, RRTT, tx, ty);
                    }
                    return true;
                }
                //=====================================⠁⠅⠅=====================================

                //=====================================⠅⠁⠅=====================================
                if (LT.Check(uptile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty + 1);

                    if (IsTileChange && uptile.IsMiniISOM && uptile.IsNoEdge)
                    {
                        DrawISOMFlatTile(mapeditor, uptile, DrawDirection.All, tx, ty - 1);
                    }

                    if (R.Check(uptile, TileBorder.DownBorder))
                    {
                        if (IsTileChange)
                        {
                            DrawISOMFlatTile(mapeditor, uptile, DrawDirection.Right, tx, ty - 1);
                        }
                    }

                    if (L.Check(uptile, TileBorder.DownBorder))
                    {
                        if (IsTileChange)
                        {
                            DrawISOMFlatTile(mapeditor, uptile, DrawDirection.Left, tx, ty - 1);
                        }
                    }

                    if (IsTileChange)
                    {
                        ISOMRightFill(mapeditor, uptile, lowtile, checker, R, RRT, RRTT, RRB, RRBB, tx, ty);
                        ISOMLeftFill(mapeditor, uptile, lowtile, checker, L, LLT, LLTT, LLB, LLBB, tx, ty);
                        if (RRTT.Check(uptile, TileBorder.FlatDownBorder) && RTT.Check(uptile, TileBorder.FlatDownBorder))
                        {
                            DrawISOMFlatTile(mapeditor, uptile, DrawDirection.Right, tx, ty - 2);
                        }
                        if (LLTT.Check(uptile, TileBorder.FlatDownBorder) && LTT.Check(uptile, TileBorder.FlatDownBorder))
                        {
                            DrawISOMFlatTile(mapeditor, uptile, DrawDirection.Left, tx, ty - 2);
                        }
                        ISOMUpFill(mapeditor, uptile, lowtile, checker, LTT, LLTT, RTT, RRTT, tx, ty);
                    }
                    return true;
                }
                //=====================================⠅⠁⠅=====================================

                //=====================================⠅⠅⠁=====================================
                if (LT.Check(uptile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty + 1);


                    if (IsTileChange && uptile.IsMiniISOM && uptile.IsNoEdge)
                    {
                        DrawISOMFlatTile(mapeditor, uptile, DrawDirection.All, tx, ty - 1);
                    }

                    if (L.Check(uptile, TileBorder.DownBorder))
                    {
                        if (IsTileChange)
                        {
                            if (LLTT.Check(uptile, TileBorder.FlatDownBorder) && LLT.Check(uptile, TileBorder.FlatDownBorder))
                            {
                                DrawISOMFlatTile(mapeditor, uptile, DrawDirection.Left, tx - 2, ty - 1);
                            }
                            DrawISOMFlatTile(mapeditor, uptile, DrawDirection.Left, tx, ty - 1);
                        }
                    }
                    else if (L.Check(lowtile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty);
                    }

                    if (IsTileChange)
                    {
                        ISOMLeftFill(mapeditor, uptile, lowtile, checker, L, LLT, LLTT, LLB, LLBB, tx, ty);
                        if (LLTT.Check(uptile, TileBorder.FlatDownBorder) && LTT.Check(uptile, TileBorder.FlatDownBorder))
                        {
                            DrawISOMFlatTile(mapeditor, uptile, DrawDirection.Left, tx, ty - 2);
                        }

                        ISOMUpFill(mapeditor, uptile, lowtile, checker, LTT, LLTT, RTT, RRTT, tx, ty);
                    }
                    return true;
                }
                //=====================================⠅⠅⠁=====================================
                #endregion

                #region 7개 절벽 세우기
                if (LT.Check(uptile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.FlatDownBorder)
                    )
                {
                    if(uptile.IsMiniISOM && uptile.IsNoEdge)
                    {
                        DrawISOMFlatTile(mapeditor, uptile, DrawDirection.All, tx, ty - 1);
                        DrawISOMFlatTile(mapeditor, uptile, DrawDirection.All, tx, ty);
                    }


                    if (IsTileChange)
                    {
                        ISOMLeftFill(mapeditor, uptile, lowtile, checker, L, LLT, LLTT, LLB, LLBB, tx, ty);
                        ISOMRightFill(mapeditor, uptile, lowtile, checker, R, RRT, RRTT, RRB, RRBB, tx, ty);
                        ISOMUpFill(mapeditor, uptile, lowtile, checker, LTT, LLTT, RTT, RRTT, tx, ty);
                        ISOMDownFill(mapeditor, uptile, lowtile, checker, LBB, LLBB, RBB, RRBB, tx, ty);
                    }


                    if (L.Check(uptile, TileBorder.FlatDownBorder) && R.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        C.Tile = uptile;
                        C.Border = TileBorder.Flat;
                        if (IsTileChange)
                        {
                            DrawISOMFlatTile(mapeditor, uptile, DrawDirection.All, tx, ty - 1);
                            DrawISOMFlatTile(mapeditor, uptile, DrawDirection.All, tx, ty);
                        }
                    }
                    else if (L.Check(lowtile, TileBorder.FlatDownBorder) && R.Check(lowtile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty);
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty);


                    }
                    else if (L.Check(uptile, TileBorder.FlatDownBorder) && R.Check(lowtile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty);

                        if (IsTileChange)
                        {
                            DrawISOMFlatTile(mapeditor, uptile, DrawDirection.Left, tx, ty - 1);
                            DrawISOMFlatTile(mapeditor, uptile, DrawDirection.Left, tx, ty);
                        }
                    }
                    else if (L.Check(lowtile, TileBorder.FlatDownBorder) && R.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty);

                        if (IsTileChange)
                        {
                            DrawISOMFlatTile(mapeditor, uptile, DrawDirection.Right, tx, ty - 1);
                            DrawISOMFlatTile(mapeditor, uptile, DrawDirection.Right, tx, ty);
                        }
                    }

                    if (L.Check(lowtile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty);
                    }
                    if (R.Check(lowtile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty);
                    }

                    return true;
                }
                #endregion
            }
            else
            {
                #region 기둥 지우기
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    if (IsTileChange)
                    {
                        if (!uptile.IsMiniISOM)
                        {
                            DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty + 1);
                        }
                    }


                    if(lowtile.ConnectLowTile != null)
                    {
                        if(L.Tile != lowtile.ConnectLowTile)
                        {
                            DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.Left, tx, ty);
                            DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.Left, tx, ty - 1);
                        }
                    }
                    else
                    {
                        DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.Left, tx, ty);
                        DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.Left, tx, ty - 1);
                    }


                    if (lowtile.ConnectLowTile != null && R.Tile != lowtile.ConnectLowTile)
                    {
                        if (R.Tile != lowtile.ConnectLowTile)
                        {
                            DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.Right, tx, ty);
                            DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.Right, tx, ty - 1);
                        }
                    }
                    else
                    {
                        DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.Right, tx, ty);
                        DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.Right, tx, ty - 1);
                    }

        

                    ISOMDownWaterFill(mapeditor, uptile, lowtile, checker, LBB, RBB, LLBB, RRBB, tx, ty);
                    return true;
                }
                #endregion

                #region 2개 절벽 지우기
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    if (IsTileChange)
                    {
                        if (!uptile.IsMiniISOM)
                        {
                            DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty + 1);
                        }
                    }

                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx - 2, ty - 1);
                    ISOMDownWaterFill(mapeditor, uptile, lowtile, checker, LBB, RBB, LLBB, RRBB, tx, ty);
                    return true;
                }


                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    if (IsTileChange)
                    {
                        if (!uptile.IsMiniISOM)
                        {
                            DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty + 1);
                        }
                        else
                        {
                            DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);
                        }
                    }

                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);
                    if (LTT.Check(uptile, TileBorder.DownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty - 2);
                    }
                    else
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty - 2);
                    }
                    if (RTT.Check(uptile, TileBorder.DownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty - 2);
                    }
                    else
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty - 2);
                    }
                    //ISOMExecute(mapeditor, tileSet, uptile, tx, ty - 2);
                    ISOMDownWaterFill(mapeditor, uptile, lowtile, checker, LBB, RBB, LLBB, RRBB, tx, ty);
                    return true;
                }


                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    if (IsTileChange)
                    {
                        if (!uptile.IsMiniISOM)
                        {
                            DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty + 1);
                        }
                    }

                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx + 2, ty - 1);
                    ISOMDownWaterFill(mapeditor, uptile, lowtile, checker, LBB, RBB, LLBB, RRBB, tx, ty);
                    return true;
                }


                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    if (IsTileChange)
                    {
                        if (!uptile.IsMiniISOM)
                        {
                            if (!uptile.IsMiniISOM)
                            {
                                DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty + 1);
                            }
                        }
                    }

                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx + 2, ty + 1);
                    ISOMDownWaterFill(mapeditor, uptile, lowtile, checker, LBB, RBB, LLBB, RRBB, tx, ty);
                    return true;
                }


                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    if (IsTileChange)
                    {
                        if (!uptile.IsMiniISOM)
                        {
                            if (!uptile.IsMiniISOM)
                            {
                                DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty + 1);
                            }
                        }
                    }

                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx - 2, ty + 1);
                    ISOMDownWaterFill(mapeditor, uptile, lowtile, checker, LBB, RBB, LLBB, RRBB, tx, ty);
                    return true;
                }


                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);

                    if (LBB.Check(uptile, TileBorder.DownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty + 2);
                    }
                    else
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty + 2);
                    }

                    if (RBB.Check(uptile, TileBorder.DownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty + 2);
                    }
                    else
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty + 2);
                    }

                    //if (IsTileChange)
                    //{
                    //    if (!uptile.IsMiniISOM)
                    //    {
                    //        DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty + 1);
                    //    }
                    //}

            

                    //ISOMExecute(mapeditor, tileSet, uptile, tx, ty + 2);
                    return true;
                }
                #endregion

                #region 3개 절벽 지우기
                //--------------------------------------------------------------------------------------------------------------------

                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    if (IsTileChange)
                    {
                        if (!uptile.IsMiniISOM)
                        {
                            DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty + 1);
                        }
                    }

                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx - 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx - 2, ty + 1);

                    if (L.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 4, ty);
                    }
                    ISOMDownWaterFill(mapeditor, uptile, lowtile, checker, LBB, RBB, LLBB, RRBB, tx, ty);
                    return true;
                }

                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    if (IsTileChange)
                    {
                        if (!uptile.IsMiniISOM)
                        {
                            DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty + 1);
                        }
                    }

                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx + 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx + 2, ty + 1);

                    if (R.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 4, ty);
                    }
                    ISOMDownWaterFill(mapeditor, uptile, lowtile, checker, LBB, RBB, LLBB, RRBB, tx, ty);
                    return true;
                }

                //--------------------------------------------------------------------------------------------------------------------

                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    if (IsTileChange)
                    {
                        if (!uptile.IsMiniISOM)
                        {
                            DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty + 1);
                        }
                    }

                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty - 1);
                    if(RTT.Check(uptile, TileBorder.DownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty - 2);
                    }
                    else
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty - 2);
                    }

                    //ISOMExecute(mapeditor, tileSet, uptile, tx - 2, ty - 1);
                    //ISOMExecute(mapeditor, tileSet, uptile, tx, ty - 2);
                    ISOMDownWaterFill(mapeditor, uptile, lowtile, checker, LBB, RBB, LLBB, RRBB, tx, ty);
                    return true;
                }

                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    if (IsTileChange)
                    {
                        if (!uptile.IsMiniISOM)
                        {
                            DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty + 1);
                        }
                    }

                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty - 1);
                    if (LTT.Check(uptile, TileBorder.DownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty - 2);
                    }
                    else
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty - 2);
                    }

                    //ISOMExecute(mapeditor, tileSet, uptile, tx + 2, ty - 1);
                    //ISOMExecute(mapeditor, tileSet, uptile, tx, ty - 2);
                    ISOMDownWaterFill(mapeditor, uptile, lowtile, checker, LBB, RBB, LLBB, RRBB, tx, ty);
                    return true;
                }

                //--------------------------------------------------------------------------------------------------------------------

                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.Left, tx, ty);
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty + 1);

                    if (LBB.Check(uptile, TileBorder.DownBorder))
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty + 2);
                    else
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty + 2);
                    return true;
                }

                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.Right, tx, ty);
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty + 1);

                    if (RBB.Check(uptile, TileBorder.DownBorder))
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty + 2);
                    else
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty + 2);
                    return true;
                }

                //--------------------------------------------------------------------------------------------------------------------

                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {

                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx - 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx + 2, ty + 1);
                    ISOMDownWaterFill(mapeditor, uptile, lowtile, checker, LBB, RBB, LLBB, RRBB, tx, ty);
                    return true;
                }

                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    if (IsTileChange)
                    {
                        //DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty + 1);
                    }
                    if (uptile.IsMiniISOM)
                    {
                        DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);
                    }
                    else
                    {
                        DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty + 1);
                    }
                    //DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx - 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx + 2, ty - 1);
                    ISOMDownWaterFill(mapeditor, uptile, lowtile, checker, LBB, RBB, LLBB, RRBB, tx, ty);
                    return true;
                }

                //--------------------------------------------------------------------------------------------------------------------

                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);
                    if (uptile.IsMiniISOM)
                    {
                        DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);
                    }

                    if (LTT.Check(uptile, TileBorder.DownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty - 2);
                    }
                    else
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty - 2);
                    }
                    if (RTT.Check(uptile, TileBorder.DownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty - 2);
                    }
                    else
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty - 2);
                    }
                    //ISOMExecute(mapeditor, tileSet, uptile, tx, ty - 2);
                    ISOMExecute(mapeditor, tileSet, uptile, tx, ty + 2);
                    return true;
                }

                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    if (IsTileChange)
                    {
                        if (!uptile.IsMiniISOM)
                        {
                            DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.Right, tx, ty + 1);

                            if (uptile.IsMiniISOM)
                            {
                                DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);
                            }
                        }
                        DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.Right, tx, ty);
                    }

                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx - 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx + 2, ty - 1);
                    ISOMDownWaterFill(mapeditor, uptile, lowtile, checker, LBB, RBB, LLBB, RRBB, tx, ty);
                    return true;
                }

                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    if (IsTileChange)
                    {
                        DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.Left, tx, ty + 1);

                        if (uptile.IsMiniISOM)
                        {
                            DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);
                        }
                    }

                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx - 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx + 2, ty + 1);
                    ISOMDownWaterFill(mapeditor, uptile, lowtile, checker, LBB, RBB, LLBB, RRBB, tx, ty);
                    return true;
                }

                //--------------------------------------------------------------------------------------------------------------------

                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    if (uptile.IsMiniISOM)
                    {
                        DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);
                    }
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.Left, tx, ty + 1);
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.Left, tx, ty);


                    if (LTT.Check(uptile, TileBorder.DownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty - 2);
                    }
                    else
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty - 2);
                    }
                    if (RTT.Check(uptile, TileBorder.DownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty - 2);
                    }
                    else
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty - 2);
                    }
                    //ISOMExecute(mapeditor, tileSet, uptile, tx, ty - 2);


                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx + 2, ty + 1);
                    ISOMDownWaterFill(mapeditor, uptile, lowtile, checker, LBB, RBB, LLBB, RRBB, tx, ty);
                    return true;
                }

                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    if (uptile.IsMiniISOM)
                    {
                        DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);
                    }
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.Right, tx, ty + 1);
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.Right, tx, ty);

                    if (LTT.Check(uptile, TileBorder.DownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty - 2);
                    }
                    else
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty - 2);
                    }
                    if (RTT.Check(uptile, TileBorder.DownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty - 2);
                    }
                    else
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty - 2);
                    }
                    //ISOMExecute(mapeditor, tileSet, uptile, tx, ty - 2);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx - 2, ty + 1);
                    ISOMDownWaterFill(mapeditor, uptile, lowtile, checker, LBB, RBB, LLBB, RRBB, tx, ty);
                    return true;
                }

                //--------------------------------------------------------------------------------------------------------------------

                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.Right, tx, ty - 1);
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);

                    ISOMExecute(mapeditor, tileSet, uptile, tx, ty + 2);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx - 2, ty - 1);
                    return true;
                }

                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.Left, tx, ty - 1);
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);

                    ISOMExecute(mapeditor, tileSet, uptile, tx, ty + 2);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx + 2, ty - 1);
                    return true;
                }

                #endregion

                #region 4개 절벽 지우기
                //--------------------------------------------------------------------------------------------------------------------
                //=====================================⠁⠁⠁=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(uptile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    if (IsTileChange)
                    {
                        if (!uptile.IsMiniISOM)
                        {
                            DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty + 1);
                        }
                        DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);
                    }


                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty - 1);

                    ISOMDownWaterFill(mapeditor, uptile, lowtile, checker, LBB, RBB, LLBB, RRBB, tx, ty);
                    return true;
                }
                //=====================================⠁⠁⠁=====================================

                //=====================================⠄⠄⠄=====================================
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(uptile, TileBorder.FlatDownBorder)
                    )
                {
                    if (IsTileChange)
                    {
                        DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);
                    }


                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty + 1);
                    return true;
                }
                //=====================================⠄⠄⠄=====================================
                //--------------------------------------------------------------------------------------------------------------------

                //--------------------------------------------------------------------------------------------------------------------
                //=====================================⠅⠁⠀=====================================
                if (LT.Check(uptile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    if (IsTileChange)
                    {
                        if (!uptile.IsMiniISOM)
                        {
                            DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty + 1);
                        }
                    }
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);


                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx - 2, ty + 1);

                    if(IsTileChange && !uptile.IsNoEdge && L.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 4, ty);
                    }

                    if (RTT.Check(uptile, TileBorder.DownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty - 2);
                    }
                    else
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty - 2);
                    }

                    ISOMDownWaterFill(mapeditor, uptile, lowtile, checker, LBB, RBB, LLBB, RRBB, tx, ty);
                    return true;
                }
                //=====================================⠅⠁⠀=====================================

                //=====================================⠀⠁⠅=====================================
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(uptile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    if (IsTileChange)
                    {
                        if (!uptile.IsMiniISOM)
                        {
                            DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty + 1);
                        }
                    }
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);


                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx + 2, ty + 1);

                    if (IsTileChange && !uptile.IsNoEdge && R.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 4, ty);
                    }

                    if (LTT.Check(uptile, TileBorder.DownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty - 2);
                    }
                    else
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty - 2);
                    }

                    ISOMDownWaterFill(mapeditor, uptile, lowtile, checker, LBB, RBB, LLBB, RRBB, tx, ty);
                    return true;
                }
                //=====================================⠀⠁⠅=====================================
                //--------------------------------------------------------------------------------------------------------------------

                //--------------------------------------------------------------------------------------------------------------------
                //=====================================⠀⠄⠅=====================================
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(uptile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    )
                {

                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);


                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx + 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty + 1);

                    if (IsTileChange && R.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 4, ty);
                    }

                    if (LBB.Check(uptile, TileBorder.DownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty + 2);
                    }
                    else
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty + 2);
                    }
                    return true;
                }
                //=====================================⠀⠄⠅=====================================

                //=====================================⠅⠄⠀=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);


                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx - 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty + 1);

                    if (IsTileChange && L.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 4, ty);
                    }

                    if (RBB.Check(uptile, TileBorder.DownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty + 2);
                    }
                    else
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty + 2);
                    }
                    return true;
                }
                //=====================================⠅⠄⠀=====================================
                //--------------------------------------------------------------------------------------------------------------------

                //--------------------------------------------------------------------------------------------------------------------
                //=====================================⠄⠁⠁=====================================
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    if (IsTileChange)
                    {
                        if (!uptile.IsMiniISOM)
                        {
                            DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty + 1);
                        }
                    }
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);

                    if (LTT.Check(uptile, TileBorder.DownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty - 2);
                    }
                    else
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty - 2);
                    }

                    if (RTT.Check(uptile, TileBorder.DownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty - 2);
                    }
                    else
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty - 2);
                    }


                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx - 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx + 2, ty + 1);
                    ISOMDownWaterFill(mapeditor, uptile, lowtile, checker, LBB, RBB, LLBB, RRBB, tx, ty);
                    return true;
                }
                //=====================================⠄⠁⠁=====================================

                //=====================================⠁⠄⠄=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);

                    if (LBB.Check(uptile, TileBorder.DownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty + 2);
                    }
                    else
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty + 2);
                    }

                    if (RBB.Check(uptile, TileBorder.DownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty + 2);
                    }
                    else
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty + 2);
                    }


                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx - 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx + 2, ty - 1);
                    return true;
                }
                //=====================================⠁⠄⠄=====================================
                //--------------------------------------------------------------------------------------------------------------------

                //--------------------------------------------------------------------------------------------------------------------
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    if (IsTileChange)
                    {
                        if (!uptile.IsMiniISOM)
                        {
                            DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty + 1);
                        }
                    }
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty - 1);


                    if (!uptile.IsMiniISOM)
                    {
                        if (LBB.Check(uptile, TileBorder.DownBorder))
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty + 2);
                        else
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty + 2);

                        if (RBB.Check(uptile, TileBorder.DownBorder))
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty + 2);
                        else
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty + 2);
                    }

                    if (LTT.Check(uptile, TileBorder.DownBorder))
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty - 2);
                    else
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty - 2);

                    //if (RTT.Check(uptile, TileBorder.DownBorder))
                    //    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty - 2);
                    //else
                    //    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty - 2);
                    return true;
                }

                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    if (IsTileChange)
                    {
                        if (!uptile.IsMiniISOM)
                        {
                            DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty + 1);
                        }
                    }
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty - 1);


                    if (!uptile.IsMiniISOM)
                    {
                        if (LBB.Check(uptile, TileBorder.DownBorder))
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty + 2);
                        else
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty + 2);

                        if (RBB.Check(uptile, TileBorder.DownBorder))
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty + 2);
                        else
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty + 2);
                    }

                    //if (LTT.Check(uptile, TileBorder.DownBorder))
                    //    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty - 2);
                    //else
                    //    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty - 2);

                    if (RTT.Check(uptile, TileBorder.DownBorder))
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty - 2);
                    else
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty - 2);
                    return true;
                }
                //--------------------------------------------------------------------------------------------------------------------

                //--------------------------------------------------------------------------------------------------------------------
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    if (IsTileChange)
                    {
                        if (!uptile.IsMiniISOM)
                        {
                            DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty + 1);
                        }
                    }
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty + 1);


                    if (LBB.Check(uptile, TileBorder.DownBorder))
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty + 2);
                    else
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty + 2);

                    //if (RBB.Check(uptile, TileBorder.DownBorder))
                    //    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty + 2);
                    //else
                    //    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty + 2);

                    if (!uptile.IsMiniISOM)
                    {
                        if (LTT.Check(uptile, TileBorder.DownBorder))
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty - 2);
                        else
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty - 2);

                        if (RTT.Check(uptile, TileBorder.DownBorder))
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty - 2);
                        else
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty - 2);
                    }
                    return true;
                }

                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    if (IsTileChange)
                    {
                        if (!uptile.IsMiniISOM)
                        {
                            DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty + 1);
                        }
                    }
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty + 1);


                    //if (LBB.Check(uptile, TileBorder.DownBorder))
                    //    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty + 2);
                    //else
                    //    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty + 2);

                    if (RBB.Check(uptile, TileBorder.DownBorder))
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty + 2);
                    else
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty + 2);

                    if (!uptile.IsMiniISOM)
                    {
                        if (LTT.Check(uptile, TileBorder.DownBorder))
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty - 2);
                        else
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty - 2);

                        if (RTT.Check(uptile, TileBorder.DownBorder))
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty - 2);
                        else
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty - 2);
                    }
                    return true;
                }
                //--------------------------------------------------------------------------------------------------------------------

                //--------------------------------------------------------------------------------------------------------------------
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    if (IsTileChange)
                    {
                        if (!uptile.IsMiniISOM)
                        {
                            DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty + 1);
                        }
                    }
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx - 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty - 1);

                    if (!uptile.IsMiniISOM)
                    {
                        if (LTT.Check(uptile, TileBorder.DownBorder))
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty - 2);
                        else
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty - 2);
                    }
                    ISOMDownWaterFill(mapeditor, uptile, lowtile, checker, LBB, RBB, LLBB, RRBB, tx, ty);
                    return true;
                }

                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    if (IsTileChange)
                    {
                        if (!uptile.IsMiniISOM)
                        {
                            DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty + 1);
                        }
                    }
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx + 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty - 1);

                    if (!uptile.IsMiniISOM)
                    {
                        if (RTT.Check(uptile, TileBorder.DownBorder))
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty - 2);
                        else
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty - 2);
                    }
                    ISOMDownWaterFill(mapeditor, uptile, lowtile, checker, LBB, RBB, LLBB, RRBB, tx, ty);
                    return true;
                }
                //--------------------------------------------------------------------------------------------------------------------

                //--------------------------------------------------------------------------------------------------------------------
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    if (IsTileChange)
                    {
                        if (!uptile.IsMiniISOM)
                        {
                            DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty + 1);
                        }
                    }
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx + 2, ty - 1);

                    if (RBB.Check(uptile, TileBorder.DownBorder))
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty + 2);
                    else
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty + 2);
                    return true;
                }

                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    if (IsTileChange)
                    {
                        if (!uptile.IsMiniISOM)
                        {
                            DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty + 1);
                        }
                    }
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx - 2, ty - 1);

                    if (LBB.Check(uptile, TileBorder.DownBorder))
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty + 2);
                    else
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty + 2);
                    return true;
                }
                //--------------------------------------------------------------------------------------------------------------------

                //--------------------------------------------------------------------------------------------------------------------
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx - 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx - 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx + 2, ty + 1);


                    if (IsTileChange && L.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 4, ty);
                    }
                    ISOMDownWaterFill(mapeditor, uptile, lowtile, checker, LBB, RBB, LLBB, RRBB, tx, ty);
                    return true;
                }

                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx + 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx + 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx - 2, ty + 1);


                    if (IsTileChange && R.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 4, ty);
                    }
                    ISOMDownWaterFill(mapeditor, uptile, lowtile, checker, LBB, RBB, LLBB, RRBB, tx, ty);
                    return true;
                }
                //--------------------------------------------------------------------------------------------------------------------

                //--------------------------------------------------------------------------------------------------------------------
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    if (!uptile.IsMiniISOM)
                    {
                        DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty + 1);
                    }
                    else
                    {
                        DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);
                    }

                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx - 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx - 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx + 2, ty - 1);


                    if (IsTileChange && L.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 4, ty);
                    }
                    ISOMDownWaterFill(mapeditor, uptile, lowtile, checker, LBB, RBB, LLBB, RRBB, tx, ty);
                    return true;
                }

                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    if (!uptile.IsMiniISOM)
                    {
                        DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty + 1);
                    }
                    else
                    {
                        DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);
                    }

                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx + 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx + 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx - 2, ty - 1);


                    if (IsTileChange && R.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 4, ty);
                    }
                    ISOMDownWaterFill(mapeditor, uptile, lowtile, checker, LBB, RBB, LLBB, RRBB, tx, ty);
                    return true;
                }
                //--------------------------------------------------------------------------------------------------------------------
                #endregion

                #region 5개 절벽 지우기
                //=====================================⠅⠀⠅=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx + 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx + 2, ty + 1);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx - 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx - 2, ty + 1);

                    if (IsTileChange && L.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 4, ty);
                    }

                    if (IsTileChange && R.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 4, ty);
                    }

                    ISOMDownWaterFill(mapeditor, uptile, lowtile, checker, LBB, RBB, LLBB, RRBB, tx, ty);
                    return true;
                }
                //=====================================⠅⠀⠅=====================================

                //=====================================⠅⠁⠁=====================================
                if (LT.Check(uptile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    checker.TileCheck(IWay.LT, tx, ty, false).Border = TileBorder.DownBorder;
                    checker.TileCheck(IWay.T, tx, ty, false).Border = TileBorder.DownBorder;

                    if (uptile.IsMiniISOM)
                    {
                        DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);
                    }
                    else
                    {
                        DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty + 1);
                    }



                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty - 1);

                    if (IsTileChange && L.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 4, ty);
                    }
                    ISOMDownWaterFill(mapeditor, uptile, lowtile, checker, LBB, RBB, LLBB, RRBB, tx, ty);
                    return true;
                }
                //=====================================⠅⠁⠁=====================================

                //=====================================⠁⠅⠁=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(uptile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    if (uptile.IsMiniISOM)
                    {
                        DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);
                    }
                    else
                    {
                        DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty + 1);
                    }

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty - 1);

                    if (!uptile.IsMiniISOM)
                    {
                        if (LBB.Check(uptile, TileBorder.DownBorder))
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty + 2);
                        else
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty + 2);

                        if (RBB.Check(uptile, TileBorder.DownBorder))
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty + 2);
                        else
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty + 2);
                    }
                    return true;
                }
                //=====================================⠁⠅⠁=====================================

                //=====================================⠁⠁⠅=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(uptile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    checker.TileCheck(IWay.RT, tx, ty, false).Border = TileBorder.DownBorder;
                    checker.TileCheck(IWay.T, tx, ty, false).Border = TileBorder.DownBorder;

                    if (uptile.IsMiniISOM)
                    {
                        DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);
                    }
                    else
                    {
                        DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty + 1);
                    }



                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty - 1);

                    if (IsTileChange && R.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 4, ty);
                    }
                    ISOMDownWaterFill(mapeditor, uptile, lowtile, checker, LBB, RBB, LLBB, RRBB, tx, ty);
                    return true;
                }
                //=====================================⠁⠁⠅=====================================


                //=====================================⠅⠄⠄=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    checker.TileCheck(IWay.B, tx, ty, false).Border = TileBorder.DownBorder;

                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx - 2, ty - 1);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty + 1);

                    if (IsTileChange && L.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 4, ty);
                    }
                    return true;
                }
                //=====================================⠅⠄⠄=====================================

                //=====================================⠄⠅⠄=====================================
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(uptile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty + 1);

                    if (!uptile.IsMiniISOM)
                    {
                        if (LTT.Check(uptile, TileBorder.DownBorder))
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty - 2);
                        else
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty - 2);

                        if (RTT.Check(uptile, TileBorder.DownBorder))
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty - 2);
                        else
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty - 2);
                    }
                    return true;
                }
                //=====================================⠄⠅⠄=====================================

                //=====================================⠄⠄⠅=====================================
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(uptile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.FlatDownBorder)
                    )
                {
                    checker.TileCheck(IWay.B, tx, ty, false).Border = TileBorder.DownBorder;

                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx + 2, ty - 1);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty + 1);

                    if (IsTileChange && R.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 4, ty);
                    }
                    return true;
                }
                //=====================================⠄⠄⠅=====================================


                //=====================================⠅⠅⠀=====================================
                if (LT.Check(uptile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);

                    checker.TileCheck(IWay.LT, tx, ty, false).Border = TileBorder.DownBorder;
                    checker.TileCheck(IWay.LB, tx, ty, false).Border = TileBorder.DownBorder;

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty + 1);

                    if (IsTileChange && L.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 4, ty);
                    }

                    if (!uptile.IsMiniISOM)
                    {
                        if (RTT.Check(uptile, TileBorder.DownBorder))
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty - 2);
                        else
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty - 2);

                        if (RBB.Check(uptile, TileBorder.DownBorder))
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty + 2);
                        else
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty + 2);
                    }
                    return true;
                }
                //=====================================⠅⠅⠀=====================================

                //=====================================⠀⠅⠅=====================================
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(uptile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(uptile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);

                    checker.TileCheck(IWay.RT, tx, ty, false).Border = TileBorder.DownBorder;
                    checker.TileCheck(IWay.RB, tx, ty, false).Border = TileBorder.DownBorder;

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty + 1);

                    if (IsTileChange && R.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 4, ty);
                    }

                    if (!uptile.IsMiniISOM)
                    {
                        if (LTT.Check(uptile, TileBorder.DownBorder))
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty - 2);
                        else
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty - 2);

                        if (LBB.Check(uptile, TileBorder.DownBorder))
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty + 2);
                        else
                            DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty + 2);
                    }
                    return true;
                }
                //=====================================⠀⠅⠅=====================================


                //=====================================⠄⠅⠁=====================================
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);

                    if (uptile.IsMiniISOM)
                    {
                        DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);
                    }

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty + 1);

                    if (LTT.Check(uptile, TileBorder.DownBorder))
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty - 2);
                    else
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty - 2);

                    if (RBB.Check(uptile, TileBorder.DownBorder))
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty + 2);
                    else
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty + 2);
                    return true;
                }
                //=====================================⠄⠅⠁=====================================

                //=====================================⠁⠅⠄=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);

                    if (uptile.IsMiniISOM)
                    {
                        DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);
                    }

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty + 1);

                    if (RTT.Check(uptile, TileBorder.DownBorder))
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty - 2);
                    else
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty - 2);

                    if (LBB.Check(uptile, TileBorder.DownBorder))
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty + 2);
                    else
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty + 2);
                    return true;
                }
                //=====================================⠁⠅⠄=====================================


                //=====================================⠅⠁⠄=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty - 1);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx - 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx + 2, ty + 1);

                    if (IsTileChange && !uptile.IsNoEdge && L.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 4, ty);
                    }

                    if (RTT.Check(uptile, TileBorder.DownBorder))
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty - 2);
                    else
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty - 2);
                    ISOMDownWaterFill(mapeditor, uptile, lowtile, checker, LBB, RBB, LLBB, RRBB, tx, ty);
                    return true;
                }
                //=====================================⠅⠁⠄=====================================

                //=====================================⠄⠁⠅=====================================
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.DownBorder)
                    && RT.Check(uptile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty - 1);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx + 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx - 2, ty + 1);

                    if (IsTileChange && !uptile.IsNoEdge && R.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 4, ty);
                    }

                    if (LTT.Check(uptile, TileBorder.DownBorder))
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty - 2);
                    else
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty - 2);
                    ISOMDownWaterFill(mapeditor, uptile, lowtile, checker, LBB, RBB, LLBB, RRBB, tx, ty);
                    return true;
                }
                //=====================================⠄⠁⠅=====================================


                //=====================================⠅⠄⠁=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(uptile, TileBorder.DownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    if (uptile.IsMiniISOM)
                    {
                        DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);
                    }

                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty + 1);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx + 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx - 2, ty - 1);
                    if (IsTileChange && L.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 4, ty);
                    }

                    if (RBB.Check(uptile, TileBorder.DownBorder))
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty + 2);
                    else
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty + 2);
                    return true;
                }
                //=====================================⠅⠄⠁=====================================

                //=====================================⠁⠄⠅=====================================
                if (LT.Check(uptile, TileBorder.DownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.DownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.DownBorder)
                    && RB.Check(uptile, TileBorder.DownBorder)
                    )
                {
                    if (uptile.IsMiniISOM)
                    {
                        DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);
                    }

                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty + 1);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx - 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx + 2, ty - 1);
                    if (IsTileChange && R.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 4, ty);
                    }

                    if (LBB.Check(uptile, TileBorder.DownBorder))
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty + 2);
                    else
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty + 2);
                    return true;
                }
                //=====================================⠁⠄⠅=====================================
                #endregion

                #region 6개 절벽 지우기
                if (LT.Check(lowtile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.FlatDownBorder)
                    )
                {
                    checker.TileCheck(IWay.B, tx, ty, false).Border = TileBorder.DownBorder;
                    checker.TileCheck(IWay.RB, tx, ty, false).Border = TileBorder.DownBorder;
                    checker.TileCheck(IWay.RT, tx, ty, false).Border = TileBorder.DownBorder;

                    if (uptile.IsMiniISOM)
                    {
                        DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);
                    }
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty - 1);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx -2, ty + 1);

                    if (IsTileChange && R.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 4, ty);
                    }
                    if (LTT.Check(uptile, TileBorder.DownBorder))
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty - 2);
                    else
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty - 2);
                    return true;
                }

                if (LT.Check(uptile, TileBorder.FlatDownBorder)
                    && T.Check(lowtile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.FlatDownBorder)
                    )
                {
                    checker.TileCheck(IWay.B, tx, ty, false).Border = TileBorder.DownBorder;

                    if (uptile.IsMiniISOM)
                    {
                        DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);
                    }
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx - 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx + 2, ty - 1);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty + 1);

                    if (IsTileChange && R.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 4, ty);
                    }

                    if (IsTileChange && L.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 4, ty);
                    }
                    return true;
                }

                if (LT.Check(uptile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.FlatDownBorder)
                    && RT.Check(lowtile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.FlatDownBorder)
                    )
                {
                    checker.TileCheck(IWay.B, tx, ty, false).Border = TileBorder.DownBorder;
                    checker.TileCheck(IWay.LB, tx, ty, false).Border = TileBorder.DownBorder;
                    checker.TileCheck(IWay.LT, tx, ty, false).Border = TileBorder.DownBorder;

                    if (uptile.IsMiniISOM)
                    {
                        DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);
                    }
                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty - 1);

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty + 1);

                    if (IsTileChange && L.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 4, ty);
                    }
                    if (RTT.Check(uptile, TileBorder.DownBorder))
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty - 2);
                    else
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty - 2);
                    return true;
                }

                if (LT.Check(uptile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.FlatDownBorder)
                    && LB.Check(lowtile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.FlatDownBorder)
                    )
                {
                    checker.TileCheck(IWay.T, tx, ty, false).Border = TileBorder.DownBorder;
                    checker.TileCheck(IWay.RT, tx, ty, false).Border = TileBorder.DownBorder;
                    checker.TileCheck(IWay.RB, tx, ty, false).Border = TileBorder.DownBorder;

                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty + 1);
                    if (uptile.IsMiniISOM)
                    {
                        DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);
                    }

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty - 1);

                    if (IsTileChange && R.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 4, ty);
                    }
                    if (LBB.Check(uptile, TileBorder.DownBorder))
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty + 2);
                    else
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty + 2);
                    return true;
                }

                if (LT.Check(uptile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.FlatDownBorder)
                    && B.Check(lowtile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.FlatDownBorder)
                    )
                {
                    checker.TileCheck(IWay.T, tx, ty, false).Border = TileBorder.DownBorder;
                    checker.TileCheck(IWay.RT, tx, ty, false).Border = TileBorder.DownBorder;
                    checker.TileCheck(IWay.LT, tx, ty, false).Border = TileBorder.DownBorder;

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty - 1);


                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx - 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx + 2, ty + 1);


                    if (IsTileChange && R.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 4, ty);
                    }

                    if (IsTileChange && L.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 4, ty);
                    }


                    ISOMDownWaterFill(mapeditor, uptile, lowtile, checker, LBB, RBB, LLBB, RRBB, tx, ty);
                    return true;
                }

                if (LT.Check(uptile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.FlatDownBorder)
                    && RB.Check(lowtile, TileBorder.FlatDownBorder)
                    )
                {
                    checker.TileCheck(IWay.T, tx, ty, false).Border = TileBorder.DownBorder;
                    checker.TileCheck(IWay.LT, tx, ty, false).Border = TileBorder.DownBorder;
                    checker.TileCheck(IWay.LB, tx, ty, false).Border = TileBorder.DownBorder;

                    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty + 1);
                    if (uptile.IsMiniISOM)
                    {
                        DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty);
                    }

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty - 1);

                    if (IsTileChange && L.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 4, ty);
                    }
                    if (RBB.Check(uptile, TileBorder.DownBorder))
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty + 2);
                    else
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty + 2);
                    return true;
                }
                #endregion

                #region 7개 절벽 지우기
                if (LT.Check(uptile, TileBorder.FlatDownBorder)
                    && T.Check(uptile, TileBorder.FlatDownBorder)
                    && RT.Check(uptile, TileBorder.FlatDownBorder)
                    && RB.Check(uptile, TileBorder.FlatDownBorder)
                    && LB.Check(uptile, TileBorder.FlatDownBorder)
                    && B.Check(uptile, TileBorder.FlatDownBorder)
                    )
                {
                    checker.TileCheck(IWay.B, tx, ty, false).Border = TileBorder.DownBorder;
                    checker.TileCheck(IWay.T, tx, ty, false).Border = TileBorder.DownBorder;
                    checker.TileCheck(IWay.LT, tx, ty, false).Border = TileBorder.DownBorder;
                    checker.TileCheck(IWay.LB, tx, ty, false).Border = TileBorder.DownBorder;
                    checker.TileCheck(IWay.RT, tx, ty, false).Border = TileBorder.DownBorder;
                    checker.TileCheck(IWay.RB, tx, ty, false).Border = TileBorder.DownBorder;

                    //if (IsTileChange)
                    //{
                    //    DrawISOMFlatTile(mapeditor, lowtile, DrawDirection.All, tx, ty - 1);
                    //}

                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty - 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty + 1);
                    DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty + 1);

                    if (L.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        //DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 4, ty);
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 4, ty);
                    }
                    if (R.Check(uptile, TileBorder.FlatDownBorder))
                    {
                        //DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 4, ty);
                        DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 4, ty);
                    }

                    //DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty + 1);
                    //DrawISOMGroup(mapeditor, uptile, lowtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty + 1);
                    return true;
                }
                #endregion
            }

            return false;
        }
    }
}
