using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UseMapEditor.Control;
using UseMapEditor.FileData;
using UseMapEditor.Task.Events;
using static UseMapEditor.FileData.ISOMTIle;
using static UseMapEditor.Tools.ISOMTool;

namespace UseMapEditor.Tools
{
    public class ISOMTool
    {

        public static Vector2 GetMtxmRectCenter(Vector2 tilepos, Vector2 mappos)
        {
            Vector2 cpos = tilepos;

            if (tilepos.X < 0)
            {
                cpos.X = ((int)(tilepos.X + 1) / 4) * 4;
                cpos.X -= 2;
            }
            else
            {
                cpos.X = ((int)tilepos.X / 4) * 4;
                cpos.X += 2;
            }
            if (tilepos.Y < 0)
            {
                cpos.Y = ((int)(tilepos.Y + 1) / 2) * 2;
                cpos.Y -= 1;
            }
            else
            {
                cpos.Y = ((int)tilepos.Y / 2) * 2;
                cpos.Y += 1;
            }

            if (mappos.Y < -0.5 * mappos.X + (cpos.Y - 1) * 32 + 0.5 * cpos.X * 32)
            {
                cpos.X -= 2;
                cpos.Y -= 1;
            }

            if (mappos.Y > 0.5 * mappos.X + (cpos.Y + 1) * 32 - 0.5 * cpos.X * 32)
            {
                cpos.X -= 2;
                cpos.Y += 1;
            }

            if (mappos.Y < 0.5 * mappos.X + (cpos.Y - 1) * 32 - 0.5 * cpos.X * 32)
            {
                cpos.X += 2;
                cpos.Y -= 1;
            }

            if (mappos.Y > -0.5 * mappos.X + (cpos.Y + 1) * 32 + 0.5 * cpos.X * 32)
            {
                cpos.X += 2;
                cpos.Y += 1;
            }

            return cpos;
        }

        public static Vector2 GetMtxmRectBrush(Vector2 tilepos, Vector2 mappos, int brush_x, int brush_y)
        {
            Vector2 cpos;

            if(brush_x % 2 == 0 || brush_y % 2 == 0)
            {
                tilepos.X += 2;
                mappos.X += 64;
            }


            if (tilepos.X < 0)
            {
                cpos.X = ((int)(tilepos.X + 1) / 4) * 4;
                cpos.X -= 2;
            }
            else
            {
                cpos.X = ((int)tilepos.X / 4) * 4;
                cpos.X += 2;
            }
            if (tilepos.Y < 0)
            {
                cpos.Y = ((int)(tilepos.Y + 1) / 2) * 2;
                cpos.Y -= 1;
            }
            else
            {
                cpos.Y = ((int)tilepos.Y / 2) * 2;
                cpos.Y += 1;
            }


            if (mappos.Y < -0.5 * mappos.X + (cpos.Y - 1) * 32 + 0.5 * cpos.X * 32)
            {
                cpos.X -= 2;
                cpos.Y -= 1;
            }

            if (mappos.Y > 0.5 * mappos.X + (cpos.Y + 1) * 32 - 0.5 * cpos.X * 32)
            {
                cpos.X -= 2;
                cpos.Y += 1;
            }

            if (mappos.Y < 0.5 * mappos.X + (cpos.Y - 1) * 32 - 0.5 * cpos.X * 32)
            {
                cpos.X += 2;
                cpos.Y -= 1;
            }

            if (mappos.Y > -0.5 * mappos.X + (cpos.Y + 1) * 32 + 0.5 * cpos.X * 32)
            {
                cpos.X += 2;
                cpos.Y += 1;
            }

            return cpos;
        }



        public static TileType ISOMCheckTile(MapEditor mapeditor, TileSet tileSet, int tilex, int tiley)
        {
            List<ISOMTIle> iSOMTIles = tileSet.GetISOMData(mapeditor);


            ushort LT = mapeditor.mapdata.TILE[tilex - 1 + (tiley - 1) * mapeditor.mapdata.WIDTH];
            ushort RT = mapeditor.mapdata.TILE[tilex + (tiley - 1) * mapeditor.mapdata.WIDTH];
            ushort LB = mapeditor.mapdata.TILE[tilex - 1 + tiley * mapeditor.mapdata.WIDTH];
            ushort RB = mapeditor.mapdata.TILE[tilex + tiley * mapeditor.mapdata.WIDTH];


            //우선 플랫타일을 돌면서 확인한다...

            foreach (var item in iSOMTIles)
            {
                TileBorder border = item.CheckTile(LT, RT, LB, RB);

                if(border == TileBorder.None)
                {
                    continue;
                }
                return new TileType(item, border);
            }

            //Dirt
            //HighDirt
            //Dirt-HighDirt
            //....

            return new TileType(null, TileBorder.None);
        }


        public static void DrawISOMGroup(MapEditor mapeditor, ISOMTIle isomtile, ISOMChecker checker, ISOMTIle.ISOMGroupType grouptype, DrawDirection direction, int tx, int ty, int gindex = -1, bool recall = true)
        {
            ISOMTIle.ISOMGroup group = null;
            int drawx = tx, drawy = ty;
            switch (grouptype)
            {
                case ISOMGroupType.tip:
                    if (checker.TileCheck(IWay.T, tx, ty).Check(isomtile, TileBorder.DownBorder) && !checker.TileCheck(IWay.B, tx, ty).Check(isomtile, TileBorder.DownBorder))
                    {
                        if (direction == DrawDirection.Left)
                        {
                            if (checker.TileCheck(IWay.LTT, tx, ty).Check(isomtile, TileBorder.DownBorder))
                            {
                                group = isomtile.tip_toplong;
                                if (gindex == -1) gindex = GetRdindex(group);
                                if (recall) DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty - 2, gindex, false);
                            }
                        }
                        else
                        {
                            if (checker.TileCheck(IWay.RTT, tx, ty).Check(isomtile, TileBorder.DownBorder))
                            {
                                group = isomtile.tip_toplong;
                                if (gindex == -1) gindex = GetRdindex(group);
                                if (recall) DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty - 2, gindex, false);
                            }
                        }

                        if (group == null)
                        {
                            group = isomtile.tip_top;
                        }
                    }
                    else if (checker.TileCheck(IWay.B, tx, ty).Check(isomtile, TileBorder.DownBorder) && !checker.TileCheck(IWay.T, tx, ty).Check(isomtile, TileBorder.DownBorder))
                    {
                        if (direction == DrawDirection.Left)
                        {
                            if (checker.TileCheck(IWay.LBB, tx, ty).Check(isomtile, TileBorder.DownBorder))
                            {
                                group = isomtile.tip_bottomlong;
                                if (gindex == -1) gindex = GetRdindex(group);
                                if (recall) DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty + 2, gindex);
                            }
                        }
                        else
                        {
                            if (checker.TileCheck(IWay.RBB, tx, ty).Check(isomtile, TileBorder.DownBorder))
                            {
                                group = isomtile.tip_bottomlong;
                                if (gindex == -1) gindex = GetRdindex(group);
                                if (recall) DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty + 2, gindex);
                            }
                        }

                        if (group == null)
                        {
                            group = isomtile.tip_bottom;
                        }
                    }
                    else if (checker.TileCheck(IWay.T, tx, ty).Check(isomtile, TileBorder.DownBorder) && checker.TileCheck(IWay.B, tx, ty).Check(isomtile, TileBorder.DownBorder))
                    {
                        if (direction == DrawDirection.Left)
                        {
                            if (checker.TileCheck(IWay.LTT, tx, ty).Check(isomtile, TileBorder.DownBorder)
                                && checker.TileCheck(IWay.LBB, tx, ty).Check(isomtile, TileBorder.DownBorder))
                            {
                                group = isomtile.tip_doublelong;
                            }
                            else if (checker.TileCheck(IWay.LTT, tx, ty).Check(isomtile, TileBorder.DownBorder))
                            {
                                group = isomtile.tip_doubletoplong;
                            }
                            else if (checker.TileCheck(IWay.LBB, tx, ty).Check(isomtile, TileBorder.DownBorder))
                            {
                                group = isomtile.tip_doublebottomlong;
                            }
                        }
                        else
                        {
                            if (checker.TileCheck(IWay.RTT, tx, ty).Check(isomtile, TileBorder.DownBorder)
                                && checker.TileCheck(IWay.RBB, tx, ty).Check(isomtile, TileBorder.DownBorder))
                            {
                                group = isomtile.tip_doublelong;
                            }
                            else if (checker.TileCheck(IWay.RTT, tx, ty).Check(isomtile, TileBorder.DownBorder))
                            {
                                group = isomtile.tip_doubletoplong;
                            }
                            else if (checker.TileCheck(IWay.RBB, tx, ty).Check(isomtile, TileBorder.DownBorder))
                            {
                                group = isomtile.tip_doublebottomlong;
                            }
                        }

                        if (group == null)
                        {
                            group = isomtile.tip_double;
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
                    break;
                case ISOMGroupType.corner:
                    if (group == null)
                    {
                        group = isomtile.corner_default;
                    }
                    GetRdindex(group);
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
                    break;
                case ISOMGroupType.edge:
                    if (direction == DrawDirection.Left)
                    {
                        if (checker.TileCheck(IWay.T, tx, ty).Check(isomtile, TileBorder.DownBorder)
                            && !checker.TileCheck(IWay.B, tx, ty).Check(isomtile, TileBorder.DownBorder))
                        {
                            if (checker.TileCheck(IWay.LTT, tx, ty).Check(isomtile, TileBorder.DownBorder))
                            {
                                group = isomtile.edge_slimtoplong;
                                if (gindex == -1) gindex = GetRdindex(group);
                                if (recall) DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty - 2, gindex, false);
                            }
                            else
                            {
                                group = isomtile.edge_slimtop;
                            }
                        }else if (checker.TileCheck(IWay.B, tx, ty).Check(isomtile, TileBorder.DownBorder)
                            && !checker.TileCheck(IWay.T, tx, ty).Check(isomtile, TileBorder.DownBorder))
                        {
                            group = isomtile.edge_slim;
                        }
                        else if (checker.TileCheck(IWay.B, tx, ty).Check(isomtile, TileBorder.DownBorder)
                            && checker.TileCheck(IWay.T, tx, ty).Check(isomtile, TileBorder.DownBorder))
                        {

                            if (checker.TileCheck(IWay.LTT, tx, ty).Check(isomtile, TileBorder.DownBorder))
                            {
                                group = isomtile.edge_toplong;
                                if (gindex == -1) gindex = GetRdindex(group);
                                if (recall) DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty - 2, gindex, false);
                            }
                            else
                            {
                                group = isomtile.edge_top;
                            }
                        }
                    }
                    else
                    {
                        if (checker.TileCheck(IWay.T, tx, ty).Check(isomtile, TileBorder.DownBorder)
                            && !checker.TileCheck(IWay.B, tx, ty).Check(isomtile, TileBorder.DownBorder))
                        {
                            if (checker.TileCheck(IWay.RTT, tx, ty).Check(isomtile, TileBorder.DownBorder))
                            {
                                group = isomtile.edge_slimtoplong;
                                if (gindex == -1) gindex = GetRdindex(group);
                                if (recall) DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty - 2, gindex, false);
                            }
                            else
                            {
                                group = isomtile.edge_slimtop;
                            }
                        }
                        else if (checker.TileCheck(IWay.B, tx, ty).Check(isomtile, TileBorder.DownBorder)
                            && !checker.TileCheck(IWay.T, tx, ty).Check(isomtile, TileBorder.DownBorder))
                        {
                            group = isomtile.edge_slim;
                        }
                        else if (checker.TileCheck(IWay.B, tx, ty).Check(isomtile, TileBorder.DownBorder)
                            && checker.TileCheck(IWay.T, tx, ty).Check(isomtile, TileBorder.DownBorder))
                        {

                            if (checker.TileCheck(IWay.RTT, tx, ty).Check(isomtile, TileBorder.DownBorder))
                            {
                                group = isomtile.edge_toplong;
                                if (gindex == -1) gindex = GetRdindex(group);
                                if (recall) DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty - 2, gindex, false);
                            }
                            else
                            {
                                group = isomtile.edge_top;
                            }
                        }
                    }


                    if (group == null)
                    {
                        group = isomtile.edge_default;
                    }
                    if (gindex == -1) gindex = GetRdindex(group);

                    if (direction == DrawDirection.Left)
                    {
                        if (checker.TileCheck(IWay.LT, tx, ty).Check(isomtile, TileBorder.DownBorder))
                        {
                            if (recall) DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.corner, DrawDirection.Left, tx, ty, gindex);
                            return;
                        }
                    }
                    else
                    {
                        if (checker.TileCheck(IWay.RT, tx, ty).Check(isomtile, TileBorder.DownBorder))
                        {
                            if (recall) DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.corner, DrawDirection.Right, tx, ty, gindex);
                            return;
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
                    DrawISOMTile(mapeditor, group, direction, drawx, drawy, gindex);
                    break;
                case ISOMGroupType.cliff:
                    if (direction == DrawDirection.Left)
                    {
                        if (checker.TileCheck(IWay.T, tx, ty).Check(isomtile.connectlowtile, TileBorder.Flat))
                        {
                            group = isomtile.cliff_slim;
                            if (gindex == -1) gindex = GetRdindex(group);
                            if (recall) DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty - 1, gindex);
                        }
                        else if (checker.TileCheck(IWay.T, tx, ty).Check(isomtile, TileBorder.DownBorder))
                        {
                            group = isomtile.cliff_slimtop;
                            if (gindex == -1) gindex = GetRdindex(group);
                        }
                    }
                    else
                    {
                        if (checker.TileCheck(IWay.T, tx, ty).Check(isomtile.connectlowtile, TileBorder.Flat))
                        {
                            group = isomtile.cliff_slim;
                            if (gindex == -1) gindex = GetRdindex(group);
                            if (recall) DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty - 1, gindex);
                        }
                        else if (checker.TileCheck(IWay.T, tx, ty).Check(isomtile, TileBorder.DownBorder))
                        {
                            group = isomtile.cliff_slimtop;
                            if (gindex == -1) gindex = GetRdindex(group);
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


                    if (direction == DrawDirection.Left)
                    {
                        if (checker.TileCheck(IWay.LB, tx, ty).Check(isomtile, TileBorder.DownBorder))
                        {
                            if (recall) DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.corner, DrawDirection.Left, tx, ty, gindex);
                            return;
                        }
                    }
                    else
                    {
                        if (checker.TileCheck(IWay.RB, tx, ty).Check(isomtile, TileBorder.DownBorder))
                        {
                            if (recall) DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.corner, DrawDirection.Right, tx, ty, gindex);
                            return;
                        }
                    }


                    DrawISOMTile(mapeditor, group, direction, drawx, drawy, gindex);


                    if (direction == DrawDirection.Left)
                    {
                        if (checker.TileCheck(IWay.B, tx, ty).Check(isomtile, TileBorder.DownBorder))
                        {
                            if (checker.TileCheck(IWay.LBB, tx, ty).Check(isomtile, TileBorder.DownBorder))
                            {
                                if (recall) DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty + 2, gindex);
                            }
                            else
                            {
                                if (recall) DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty + 2, gindex);
                            }
                        }
                    }
                    else
                    {
                        if (checker.TileCheck(IWay.B, tx, ty).Check(isomtile, TileBorder.DownBorder))
                        {
                            if (checker.TileCheck(IWay.RBB, tx, ty).Check(isomtile, TileBorder.DownBorder))
                            {
                                if (recall) DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty + 2, gindex);
                            }
                            else
                            {
                                if (recall) DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty + 2, gindex);
                            }
                        }
                    }

                    break;
            }



        }

        public class TileType
        {
            public ISOMTIle Tile;
            public ISOMTIle.TileBorder Border;

            public TileType(ISOMTIle tile, ISOMTIle.TileBorder border)
            {
                Tile = tile;
                Border = border;
            }

            public bool Check(ISOMTIle tile, ISOMTIle.TileBorder border)
            {
                return (tile.name == Tile.name) && (Border == border);
            }
        }
        public class ISOMChecker
        {
            private MapEditor mapeditor;
            private TileSet tileSet;

            private Dictionary<Vector2, TileType> keys = new Dictionary<Vector2, TileType>();

            public ISOMChecker(MapEditor mapeditor, TileSet tileSet)
            {
                this.mapeditor = mapeditor;
                this.tileSet = tileSet;
            }

            public TileType TileCheck(IWay direction, int tilex, int tiley)
            {
                int ctilex = tilex;
                int ctiley = tiley;

                switch (direction)
                {
                    case IWay.L:
                        ctilex -= 4;
                        ctiley += 0;
                        break;
                    case IWay.LT:
                        ctilex -= 2;
                        ctiley -= 1;
                        break;
                    case IWay.T:
                        ctilex += 0;
                        ctiley -= 2;
                        break;
                    case IWay.RT:
                        ctilex += 2;
                        ctiley -= 1;
                        break;
                    case IWay.R:
                        ctilex += 4;
                        ctiley += 0;
                        break;
                    case IWay.RB:
                        ctilex += 2;
                        ctiley += 1;
                        break;
                    case IWay.LB:
                        ctilex -= 2;
                        ctiley += 1;
                        break;
                    case IWay.B:
                        ctilex += 0;
                        ctiley += 2;
                        break;
                    case IWay.LTT:
                        ctilex -= 2;
                        ctiley -= 3;
                        break;
                    case IWay.RTT:
                        ctilex += 2;
                        ctiley -= 3;
                        break;
                    case IWay.RBB:
                        ctilex += 2;
                        ctiley += 3;
                        break;
                    case IWay.LBB:
                        ctilex -= 2;
                        ctiley += 3;
                        break;
                    case IWay.BB:
                        ctilex += 0;
                        ctiley += 4;
                        break;
                    case IWay.TT:
                        ctilex += 0;
                        ctiley -= 4;
                        break;
                }

                Vector2 vec = new Vector2(ctilex, ctiley);

                if (!keys.ContainsKey(vec))
                {
                    keys.Add(vec, ISOMCheckTile(mapeditor, tileSet, ctilex, ctiley));
                }
                return keys[vec];
            }
        }

        public enum IWay
        {
            L,
            LT,
            T,
            RT,
            R,
            RB,
            LB,
            B,
            LTT,
            RTT,
            RBB,
            LBB,
            TT,
            BB
        }

        public static void ISOMExecute(MapEditor mapeditor, TileSet tileSet, ISOMTIle isomtile, int tx, int ty)
        {
            ISOMChecker checker = new ISOMChecker(mapeditor, tileSet);

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

            #region 기둥 세우기
            if (LT.Check(isomtile.connectlowtile, TileBorder.Flat)
                && (T.Check(isomtile.connectlowtile, TileBorder.Flat) || T.Check(isomtile, TileBorder.DownBorder))
                && RT.Check(isomtile.connectlowtile, TileBorder.Flat)
                && RB.Check(isomtile.connectlowtile, TileBorder.Flat)
                && LB.Check(isomtile.connectlowtile, TileBorder.Flat)
                && (B.Check(isomtile.connectlowtile, TileBorder.Flat) || B.Check(isomtile, TileBorder.DownBorder))
                )
            {
                if (T.Check(isomtile, TileBorder.DownBorder) && LTT.Check(isomtile, TileBorder.DownBorder))
                {
                    int gindex = GetRdindex(isomtile.tip_default);
                    DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty - 2, gindex);
                    DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty, gindex);
                }
                else
                {
                    DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty);
                }


                if (T.Check(isomtile, TileBorder.DownBorder) && RTT.Check(isomtile, TileBorder.DownBorder))
                {
                    int gindex = GetRdindex(isomtile.tip_default);
                    DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty - 2, gindex);
                    DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty, gindex);
                }
                else
                {
                    DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty);
                }
            }
            #endregion

            #region 두개 절벽 세우기
            if (LT.Check(isomtile, TileBorder.DownBorder)
                && T.Check(isomtile.connectlowtile, TileBorder.Flat)
                && RT.Check(isomtile.connectlowtile, TileBorder.Flat)
                && RB.Check(isomtile.connectlowtile, TileBorder.Flat)
                && LB.Check(isomtile.connectlowtile, TileBorder.Flat)
                && B.Check(isomtile.connectlowtile, TileBorder.Flat)
                )
            {
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty);
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty);
            }

            if (LT.Check(isomtile.connectlowtile, TileBorder.Flat)
                && T.Check(isomtile.connectlowtile, TileBorder.Flat)
                && RT.Check(isomtile, TileBorder.DownBorder)
                && RB.Check(isomtile.connectlowtile, TileBorder.Flat)
                && LB.Check(isomtile.connectlowtile, TileBorder.Flat)
                && B.Check(isomtile.connectlowtile, TileBorder.Flat)
                )
            {
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty);
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty);
            }

            if (LT.Check(isomtile.connectlowtile, TileBorder.Flat)
                && T.Check(isomtile.connectlowtile, TileBorder.Flat)
                && RT.Check(isomtile.connectlowtile, TileBorder.Flat)
                && RB.Check(isomtile, TileBorder.DownBorder)
                && LB.Check(isomtile.connectlowtile, TileBorder.Flat)
                && B.Check(isomtile.connectlowtile, TileBorder.Flat)
                )
            {
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty);
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty + 1);
            }

            if (LT.Check(isomtile.connectlowtile, TileBorder.Flat)
                && T.Check(isomtile.connectlowtile, TileBorder.Flat)
                && RT.Check(isomtile.connectlowtile, TileBorder.Flat)
                && RB.Check(isomtile.connectlowtile, TileBorder.Flat)
                && LB.Check(isomtile, TileBorder.DownBorder)
                && B.Check(isomtile.connectlowtile, TileBorder.Flat)
                )
            {
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty);
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty + 1);
            }
            #endregion

            #region 3개 절벽 세우기
            //=====================================왼쪽 오른쪽 절벽=====================================
            if (LT.Check(isomtile, TileBorder.DownBorder)
                && T.Check(isomtile.connectlowtile, TileBorder.Flat)
                && RT.Check(isomtile, TileBorder.DownBorder)
                && RB.Check(isomtile.connectlowtile, TileBorder.Flat)
                && LB.Check(isomtile.connectlowtile, TileBorder.Flat)
                && B.Check(isomtile.connectlowtile, TileBorder.Flat)
                )
            {
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty);
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty);
            }

            if (LT.Check(isomtile.connectlowtile, TileBorder.Flat)
                && T.Check(isomtile.connectlowtile, TileBorder.Flat)
                && RT.Check(isomtile.connectlowtile, TileBorder.Flat)
                && RB.Check(isomtile, TileBorder.DownBorder)
                && LB.Check(isomtile, TileBorder.DownBorder)
                && B.Check(isomtile.connectlowtile, TileBorder.Flat)
                )
            {
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty + 1);
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty + 1);
            }
            //=====================================왼쪽 오른쪽 절벽=====================================

            //=====================================위 아래 절벽=====================================
            if (LT.Check(isomtile, TileBorder.DownBorder)
                && T.Check(isomtile.connectlowtile, TileBorder.Flat)
                && RT.Check(isomtile.connectlowtile, TileBorder.Flat)
                && RB.Check(isomtile.connectlowtile, TileBorder.Flat)
                && LB.Check(isomtile, TileBorder.DownBorder)
                && B.Check(isomtile.connectlowtile, TileBorder.Flat)
                )
            {
                int gindex = GetRdindex(isomtile.cliff_default);
                if(L.Check(isomtile, TileBorder.DownBorder))
                {

                }

                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx - 2, ty - 1, gindex);
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty + 1, gindex);
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty, gindex);
            }

            if (LT.Check(isomtile.connectlowtile, TileBorder.Flat)
                && T.Check(isomtile.connectlowtile, TileBorder.Flat)
                && RT.Check(isomtile, TileBorder.DownBorder)
                && RB.Check(isomtile, TileBorder.DownBorder)
                && LB.Check(isomtile.connectlowtile, TileBorder.Flat)
                && B.Check(isomtile.connectlowtile, TileBorder.Flat)
                )
            {
                int gindex = GetRdindex(isomtile.cliff_default);
                if (R.Check(isomtile, TileBorder.DownBorder))
                {

                }

                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx + 2, ty - 1, gindex);
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty + 1, gindex);
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty, gindex);
            }
            //=====================================위 아래 절벽=====================================


            //=====================================오른쪽 아래 붙은 절벽=====================================
            if (LT.Check(isomtile.connectlowtile, TileBorder.Flat)
                && T.Check(isomtile.connectlowtile, TileBorder.Flat)
                && RT.Check(isomtile.connectlowtile, TileBorder.Flat)
                && RB.Check(isomtile, TileBorder.DownBorder)
                && LB.Check(isomtile.connectlowtile, TileBorder.Flat)
                && B.Check(isomtile, TileBorder.DownBorder)
                )
            {
                int gindex = GetRdindex(isomtile.cliff_default);
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty, gindex);
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty + 2, gindex);
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.corner, DrawDirection.Left, tx + 2, ty + 1, gindex);
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty, gindex);
            }


            if (LT.Check(isomtile, TileBorder.DownBorder)
                && T.Check(isomtile, TileBorder.DownBorder)
                && RT.Check(isomtile.connectlowtile, TileBorder.Flat)
                && RB.Check(isomtile.connectlowtile, TileBorder.Flat)
                && LB.Check(isomtile.connectlowtile, TileBorder.Flat)
                && B.Check(isomtile.connectlowtile, TileBorder.Flat)
                )
            {
                int gindex = GetRdindex(isomtile.cliff_default);
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty - 2, gindex);
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty, gindex);
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.corner, DrawDirection.Right, tx - 2, ty - 1, gindex);
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty, gindex);
            }
            //=====================================오른쪽 아래 붙은 절벽=====================================


            //=====================================왼쪽 아래 붙은 절벽=====================================
            if (LT.Check(isomtile.connectlowtile, TileBorder.Flat)
                && T.Check(isomtile.connectlowtile, TileBorder.Flat)
                && RT.Check(isomtile.connectlowtile, TileBorder.Flat)
                && RB.Check(isomtile.connectlowtile, TileBorder.Flat)
                && LB.Check(isomtile, TileBorder.DownBorder)
                && B.Check(isomtile, TileBorder.DownBorder)
                )
            {
                int gindex = GetRdindex(isomtile.cliff_default);
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.edge, DrawDirection.Left, tx, ty, gindex);
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty + 2, gindex);
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.corner, DrawDirection.Right, tx - 2, ty + 1, gindex);
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty, gindex);
            }


            if (LT.Check(isomtile.connectlowtile, TileBorder.Flat)
                && T.Check(isomtile, TileBorder.DownBorder)
                && RT.Check(isomtile, TileBorder.DownBorder)
                && RB.Check(isomtile.connectlowtile, TileBorder.Flat)
                && LB.Check(isomtile.connectlowtile, TileBorder.Flat)
                && B.Check(isomtile.connectlowtile, TileBorder.Flat)
                )
            {
                int gindex = GetRdindex(isomtile.cliff_default);
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.edge, DrawDirection.Right, tx, ty - 2, gindex);
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty, gindex);
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.corner, DrawDirection.Left, tx + 2, ty - 1, gindex);
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty, gindex);
            }
            //=====================================왼쪽 아래 붙은 절벽=====================================

            //=====================================오른쪽 위 왼쪽 아래 절벽=====================================
            if (LT.Check(isomtile.connectlowtile, TileBorder.Flat)
                && T.Check(isomtile.connectlowtile, TileBorder.Flat)
                && RT.Check(isomtile, TileBorder.DownBorder)
                && RB.Check(isomtile.connectlowtile, TileBorder.Flat)
                && LB.Check(isomtile, TileBorder.DownBorder)
                && B.Check(isomtile.connectlowtile, TileBorder.Flat)
                )
            {
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty);
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty + 1);
            }
            //=====================================오른쪽 위 왼쪽 아래 절벽=====================================

            //=====================================왼쪽 위 오른쪽 아래 절벽=====================================
            if (LT.Check(isomtile, TileBorder.DownBorder)
                && T.Check(isomtile.connectlowtile, TileBorder.Flat)
                && RT.Check(isomtile.connectlowtile, TileBorder.Flat)
                && RB.Check(isomtile, TileBorder.DownBorder)
                && LB.Check(isomtile.connectlowtile, TileBorder.Flat)
                && B.Check(isomtile.connectlowtile, TileBorder.Flat)
                )
            {
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty);
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty + 1);
            }
            //=====================================왼쪽 위 오른쪽 아래 절벽=====================================

            //=====================================위 왼쪽 아래 절벽=====================================
            if (LT.Check(isomtile.connectlowtile, TileBorder.Flat)
                && T.Check(isomtile, TileBorder.DownBorder)
                && RT.Check(isomtile.connectlowtile, TileBorder.Flat)
                && RB.Check(isomtile.connectlowtile, TileBorder.Flat)
                && LB.Check(isomtile, TileBorder.DownBorder)
                && B.Check(isomtile.connectlowtile, TileBorder.Flat)
                )
            {
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty);
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx - 2, ty + 1);
            }
            //=====================================위 왼쪽 아래 절벽=====================================

            //=====================================위 오른쪽 아래 절벽=====================================
            if (LT.Check(isomtile.connectlowtile, TileBorder.Flat)
                && T.Check(isomtile, TileBorder.DownBorder)
                && RT.Check(isomtile.connectlowtile, TileBorder.Flat)
                && RB.Check(isomtile, TileBorder.DownBorder)
                && LB.Check(isomtile.connectlowtile, TileBorder.Flat)
                && B.Check(isomtile.connectlowtile, TileBorder.Flat)
                )
            {
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty);
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx + 2, ty + 1);
            }
            //=====================================위 오른쪽 아래 절벽=====================================

            //=====================================왼쪽 위 아래 절벽=====================================
            if (LT.Check(isomtile, TileBorder.DownBorder)
                && T.Check(isomtile.connectlowtile, TileBorder.Flat)
                && RT.Check(isomtile.connectlowtile, TileBorder.Flat)
                && RB.Check(isomtile.connectlowtile, TileBorder.Flat)
                && LB.Check(isomtile.connectlowtile, TileBorder.Flat)
                && B.Check(isomtile, TileBorder.DownBorder)
                )
            {
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.tip, DrawDirection.Right, tx, ty);
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.cliff, DrawDirection.Left, tx, ty);
            }
            //=====================================왼쪽 위 아래 절벽=====================================

            //=====================================오른쪽 위 아래 절벽=====================================
            if (LT.Check(isomtile.connectlowtile, TileBorder.Flat)
                && T.Check(isomtile.connectlowtile, TileBorder.Flat)
                && RT.Check(isomtile, TileBorder.DownBorder)
                && RB.Check(isomtile.connectlowtile, TileBorder.Flat)
                && LB.Check(isomtile.connectlowtile, TileBorder.Flat)
                && B.Check(isomtile, TileBorder.DownBorder)
                )
            {
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.tip, DrawDirection.Left, tx, ty);
                DrawISOMGroup(mapeditor, isomtile, checker, ISOMGroupType.cliff, DrawDirection.Right, tx, ty);
            }
            //=====================================오른쪽 위 아래 절벽=====================================

            #endregion
        }


        public enum DrawDirection
        {
            Left,
            Right
        }

        public static void DrawISOMTile(MapEditor mapeditor, ISOMTIle.ISOMGroup group, DrawDirection direction, int tilex, int tiley, int gindex = -1)
        {
            if(gindex == -1)
            {
                gindex = ISOMTIle.GetRdindex(group);
            }
            for (int i = 0; i < group.PartLength; i++)
            {
                int lmx = tilex + i % 2;
                int lmy = tiley + i / 2;
                if (mapeditor.mapdata.CheckTILERange(lmx, lmy))
                {
                    int tileindex = lmx + lmy * mapeditor.mapdata.WIDTH;
                    ushort oldMTXM = mapeditor.mapdata.TILE[tileindex];

                    ushort newMTXM = 0;
                    if (direction == DrawDirection.Left)
                    {
                        newMTXM = group.left_tiles[i][gindex];
                    }
                    else if (direction == DrawDirection.Right)
                    {
                        newMTXM = group.right_tiles[i][gindex];
                    }

                    mapeditor.mapdata.TILEChange(lmx, lmy, newMTXM);
                    mapeditor.taskManager.TaskAdd(new TileEvent(mapeditor, newMTXM, oldMTXM, lmx, lmy));
                }
            }
        }

        public static void GetIntersectionPoint(Vector2 p1, Vector2 p2, out Vector2 rp1, out Vector2 rp2)
        {
            rp1 = new Vector2();
            rp2 = new Vector2();

            double a1 = p1.Y + 0.5 * p1.X;
            double a2 = p1.Y - 0.5 * p1.X;

            double b1 = p2.Y + 0.5 * p2.X;
            double b2 = p2.Y - 0.5 * p2.X;


            //a1 -0.5 - b2 +0.5
            //a2 +0.5 - b1 -0.5

            rp1.X = (float)((b2 - a1) / (-0.5 - 0.5));
            rp1.Y = (float)(-0.5 * (b2 - a1) / (-0.5 - 0.5) + a1);


            rp2.X = (float)((b1 - a2) / (0.5 + 0.5));
            rp2.Y = (float)(0.5 * (b1 - a2) / (0.5 + 0.5) + a2);
        }
    }
}
