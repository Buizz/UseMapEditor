using Dragablz;
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
using static UseMapEditor.FileData.ISOMTile;
using static UseMapEditor.Tools.ISOMTool;

namespace UseMapEditor.Tools
{
    public partial class ISOMTool
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


        public static ushort GetMapTile(MapEditor mapeditor, int tilex, int tiley)
        {
            ushort rval = 0;

            if (tilex < 0)
            {
                tilex = -(tilex + 1);
            }
            else if (tilex >= mapeditor.mapdata.WIDTH)
            {
                tilex = (mapeditor.mapdata.WIDTH) - (tilex - (mapeditor.mapdata.WIDTH - 1));
            }
            if (tiley < 0)
            {
                tiley = -(tiley + 1);
            }
            else if (tiley >= mapeditor.mapdata.HEIGHT)
            {
                tiley = (mapeditor.mapdata.HEIGHT) - (tiley - (mapeditor.mapdata.HEIGHT - 1));
            }


            rval = mapeditor.mapdata.TILE[tilex + (tiley) * mapeditor.mapdata.WIDTH];

            return rval;
        }
        public static TileType ISOMCheckTile(MapEditor mapeditor, TileSet tileSet, int tilex, int tiley)
        {
            List<ISOMTile> iSOMTIles = tileSet.GetISOMData(mapeditor);

            ushort LT = GetMapTile(mapeditor, tilex - 1, tiley - 1);
            ushort RT = GetMapTile(mapeditor, tilex, tiley - 1);
            ushort LB = GetMapTile(mapeditor, tilex - 1, tiley);
            ushort RB = GetMapTile(mapeditor, tilex, tiley);


            //if (mapeditor.mapdata.CheckTILERange(tilex - 1, tiley - 1)) LT = mapeditor.mapdata.TILE[tilex - 1 + (tiley - 1) * mapeditor.mapdata.WIDTH];
            //if (mapeditor.mapdata.CheckTILERange(tilex, tiley - 1)) RT = mapeditor.mapdata.TILE[tilex + (tiley - 1) * mapeditor.mapdata.WIDTH];
            //if (mapeditor.mapdata.CheckTILERange(tilex - 1, tiley)) LB = mapeditor.mapdata.TILE[tilex - 1 + tiley * mapeditor.mapdata.WIDTH];
            //if (mapeditor.mapdata.CheckTILERange(tilex, tiley)) RB = mapeditor.mapdata.TILE[tilex + tiley * mapeditor.mapdata.WIDTH];

            //우선 플랫타일을 돌면서 확인한다...

            foreach (var item in iSOMTIles)
            {
                TileBorder border = item.CheckTile(LT, RT, LB, RB);

                if(border == TileBorder.None)
                {
                    continue;
                }
                return new TileType(item, border, new Vector2(tilex, tiley));
            }

            //Dirt
            //HighDirt
            //Dirt-HighDirt
            //....

            //Null을 내보내진말고 가장 첫번쨰타일로 인식시키자
            return new TileType(null, TileBorder.None, new Vector2(tilex, tiley));
            //return new TileType(iSOMTIles.First(), TileBorder.Flat, new Vector2(tilex, tiley));
        }


        public class TileType
        {
            public ISOMTile Tile;
            public ISOMTile.TileBorder Border;
            public Vector2 Point;

            public override string ToString()
            {
                return Tile.name + " : " + Border.ToString() + "(" + Point.ToString() + ")";
            }

            public TileType(ISOMTile tile, ISOMTile.TileBorder border, Vector2 point)
            {
                this.Tile = tile;
                this.Border = border;
                this.Point = point;
            }
            public TileType(TileType tileType)
            {
                Tile = tileType.Tile;
                Border = tileType.Border;
                Point = tileType.Point;
            }

            public bool IsNull()
            {
                if (Tile == null) return true;
                if (Border == TileBorder.None) return true;

                return false;
            }


            public TileType Copy()
            {
               return new TileType(Tile, Border, Point);
            }



            public bool Check(ISOMTile tile, ISOMTile.TileBorder border)
            {
                if (Tile == null) return true;
                if (tile == null) return true;

                if (border == TileBorder.FlatDownBorder)
                {
                    if ((tile.name == Tile.name) && (Border == TileBorder.Flat))
                    {
                        return true;
                    }
                    if ((tile.name == Tile.name) && (Border == TileBorder.DownBorder))
                    {
                        return true;
                    }

                    return false;
                }


                return (tile.name == Tile.name) && (Border == border);
            }
        }
        public class ISOMChecker
        {
            private MapEditor mapeditor;
            private TileSet tileSet;

            private Dictionary<Vector2, TileType> keys = new Dictionary<Vector2, TileType>();

            public Dictionary<string, bool> drawlist = new Dictionary<string, bool>();

            public bool AddDrawList(ISOMTile isomtile, ISOMGroupType grouptype, DrawDirection direction, int tx, int ty)
            {
                string key = isomtile.name + "_" + grouptype.ToString() + "_" + direction.ToString() + "_" + tx.ToString() + "_" + ty.ToString();

                if (drawlist.ContainsKey(key)) return false;

                drawlist.Add(key, true);
                return true;
            }


            public ISOMChecker(MapEditor mapeditor, TileSet tileSet)
            {
                this.mapeditor = mapeditor;
                this.tileSet = tileSet;
            }


            public TileType TileCheck(IWay direction, int tilex, int tiley, bool IsCopy = true)
            {
                int ctilex = tilex;
                int ctiley = tiley;

                switch (direction)
                {
                    case IWay.C:
                        ctilex += 0;
                        ctiley += 0;
                        break;
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
                    case IWay.RRBB:
                        ctilex += 4;
                        ctiley += 2;
                        break;
                    case IWay.LLBB:
                        ctilex -= 4;
                        ctiley += 2;
                        break;
                    case IWay.RRTT:
                        ctilex += 4;
                        ctiley -= 2;
                        break;
                    case IWay.LLTT:
                        ctilex -= 4;
                        ctiley -= 2;
                        break;
                    case IWay.RRT:
                        ctilex += 6;
                        ctiley -= 1;
                        break;
                    case IWay.LLT:
                        ctilex -= 6;
                        ctiley -= 1;
                        break;
                    case IWay.RRB:
                        ctilex += 6;
                        ctiley += 1;
                        break;
                    case IWay.LLB:
                        ctilex -= 6;
                        ctiley += 1;
                        break;
                }

                Vector2 vec = new Vector2(ctilex, ctiley);

                if (!keys.ContainsKey(vec))
                {
                    TileType tileType = ISOMCheckTile(mapeditor, tileSet, ctilex, ctiley);

                    keys.Add(vec, tileType);
                }
                if (IsCopy)
                {
                    return new TileType(keys[vec]);

                }
                else
                {
                    return keys[vec];
                }
            }
        }

        public enum IWay
        {
            C,
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
            BB,
            RRBB,
            LLBB,
            RRTT,
            LLTT,
            RRT,
            LLT,
            RRB,
            LLB
        }

        public static bool GetConnectedList(ISOMTile toisom, ISOMTile fromisom, List<ISOMTile> connectedlist, List<ISOMTile> duplist)
        {
            foreach (var item in fromisom.ConnectedAllTile)
            {
                if (duplist.Contains(item)) continue;
                duplist.Add(item);
                connectedlist.Add(item);

                if (item != toisom)
                {
                    //목표가 아닐 경우 한번 더 들어간다.
                    bool findison = GetConnectedList(toisom, item, connectedlist, duplist);
                    if (findison == true) return true;
                }
                else
                {
                    //목표일 경우
                    return true;
                }


            }

            connectedlist.Remove(fromisom);

            return false;
        }


        public static bool ISOMExecute(MapEditor mapeditor, TileSet tileSet, ISOMTile isomtile, int tx, int ty)
        {
            ISOMChecker checker = new ISOMChecker(mapeditor, tileSet);

            List<ISOMTile> tileList = new List<ISOMTile>();
            List<ISOMTile> ISOMCheckList = new List<ISOMTile>();

            TileType C = checker.TileCheck(IWay.C, tx, ty, false);
            TileType L = checker.TileCheck(IWay.L, tx, ty);
            TileType LT = checker.TileCheck(IWay.LT, tx, ty);
            TileType T = checker.TileCheck(IWay.T, tx, ty);
            TileType RT = checker.TileCheck(IWay.RT, tx, ty);
            TileType R = checker.TileCheck(IWay.R, tx, ty);
            TileType RB = checker.TileCheck(IWay.RB, tx, ty);
            TileType LB = checker.TileCheck(IWay.LB, tx, ty);
            TileType B = checker.TileCheck(IWay.B, tx, ty);
            TileType BB = checker.TileCheck(IWay.BB, tx, ty);

            TileType LTT = checker.TileCheck(IWay.LTT, tx, ty);
            TileType RTT = checker.TileCheck(IWay.RTT, tx, ty);
            TileType RBB = checker.TileCheck(IWay.RBB, tx, ty);
            TileType LBB = checker.TileCheck(IWay.LBB, tx, ty);

            TileType RRBB = checker.TileCheck(IWay.RRBB, tx, ty);
            TileType LLBB = checker.TileCheck(IWay.LLBB, tx, ty);

            TileType RRTT = checker.TileCheck(IWay.RRTT, tx, ty);
            TileType LLTT = checker.TileCheck(IWay.LLTT, tx, ty);

            bool IsTileChange;

            if(C.Tile == null)
            {
                C.Tile = isomtile;
                C.Border = TileBorder.DownBorder;
            }


            Dictionary<IWay, TileType> tile9line = new Dictionary<IWay, TileType>
            {
                { IWay.C, C.Copy() },
                { IWay.L, L },
                { IWay.LT, LT },
                { IWay.T, T },
                { IWay.RT, RT },
                { IWay.R, R },
                { IWay.RB, RB },
                { IWay.LB, LB },
                { IWay.B, B }
            };

            Dictionary<IWay, TileType> tile13upline = new Dictionary<IWay, TileType>
            {
                { IWay.C, C.Copy() },
                { IWay.L, L },
                { IWay.LT, LT },
                { IWay.T, T },
                { IWay.RT, RT },
                { IWay.R, R },
                { IWay.RB, RB },
                { IWay.LB, LB },
                { IWay.B, B },
                { IWay.LBB, LBB },
                { IWay.RBB, RBB },
                { IWay.LLBB, LLBB },
                { IWay.RRBB, RRBB }
            };

            Dictionary<IWay, TileType> tile11downline = new Dictionary<IWay, TileType>
            {
                { IWay.C, C.Copy() },
                { IWay.L, L },
                { IWay.LT, LT },
                { IWay.T, T },
                { IWay.RT, RT },
                { IWay.R, R },
                { IWay.RB, RB },
                { IWay.LB, LB },
                { IWay.B, B },
                { IWay.LTT, LTT },
                { IWay.RTT, RTT },
                { IWay.LLTT, LLTT },
                { IWay.RRTT, RRTT },
            };

            //if (!tileList.Contains(isomtile)) tileList.Add(isomtile);

            if (R.IsNull() || L.IsNull() || T.IsNull() || B.IsNull()
             || LT.IsNull() || RT.IsNull() || LB.IsNull() || RB.IsNull()
             || RBB.IsNull() || LBB.IsNull() || RTT.IsNull() || LBB.IsNull()
             || LLBB.IsNull() || RRBB.IsNull() ) return false;

            if (!tileList.Contains(C.Tile)) tileList.Add(C.Tile);
            if (!tileList.Contains(LT.Tile)) tileList.Add(LT.Tile);
            if (!tileList.Contains(T.Tile)) tileList.Add(T.Tile);
            if (!tileList.Contains(RT.Tile)) tileList.Add(RT.Tile);
            if (!tileList.Contains(LB.Tile)) tileList.Add(LB.Tile);
            if (!tileList.Contains(B.Tile)) tileList.Add(B.Tile);
            if (!tileList.Contains(RB.Tile)) tileList.Add(RB.Tile);


            if (!tileList.Contains(L.Tile)) tileList.Add(L.Tile);
            if (!tileList.Contains(R.Tile)) tileList.Add(R.Tile);



            if(isomtile.ConnectLowTile != null && !isomtile.ConnectLowTile.IsHasCliffDown && !isomtile.IsMiniISOM)
            {
                //절벽일 경우
                if (LLBB.Tile != isomtile.ConnectLowTile)
                {
                    if (!tileList.Contains(LLBB.Tile)) tileList.Add(LLBB.Tile);
                }
                if (LBB.Tile != isomtile.ConnectLowTile)
                {
                    if (!tileList.Contains(LBB.Tile)) tileList.Add(LBB.Tile);
                }

                if (RRBB.Tile != isomtile.ConnectLowTile)
                {
                    if (!tileList.Contains(RRBB.Tile)) tileList.Add(RRBB.Tile);
                }
                if (RBB.Tile != isomtile.ConnectLowTile)
                {
                    if (!tileList.Contains(RBB.Tile)) tileList.Add(RBB.Tile);
                }
            }



            if (LT.Tile.ConnectHighTile != null && LT.Tile.ConnectHighTile == LTT.Tile && !LT.Tile.IsHasCliffDown && !LT.Tile.ConnectHighTile.IsMiniISOM)
            {
                if (!tileList.Contains(LTT.Tile)) tileList.Add(LTT.Tile);
            }
            if (RT.Tile.ConnectHighTile != null && RT.Tile.ConnectHighTile == RTT.Tile && !RT.Tile.IsHasCliffDown && !RT.Tile.ConnectHighTile.IsMiniISOM)
            {
                if (!tileList.Contains(RTT.Tile)) tileList.Add(RTT.Tile);
            }

            if (L.Tile.ConnectHighTile != null && L.Tile.ConnectHighTile == LLTT.Tile && !L.Tile.IsHasCliffDown && !L.Tile.ConnectHighTile.IsMiniISOM)
            {
                if (!tileList.Contains(LLTT.Tile)) tileList.Add(LLTT.Tile);
            }
            if (R.Tile.ConnectHighTile != null && R.Tile.ConnectHighTile == RRTT.Tile && !R.Tile.IsHasCliffDown && !R.Tile.ConnectHighTile.IsMiniISOM)
            {
                if (!tileList.Contains(RRTT.Tile)) tileList.Add(RRTT.Tile);
            }

            ISOMCheckList.AddRange(tileList);



            if (C.Tile.name == isomtile.name)
            {
                IsTileChange = false;
            }
            else
            {
                IsTileChange = true;
            }

            C.Tile = isomtile;
            C.Border = TileBorder.DownBorder;


            bool IsIsomRefresh = false;

            //우선 TileList중에 연결이 안되어 있는 항목이 있는지 먼저 확인하고 연결되어 있지 않은 항목을 ISOM처리한다.
            foreach (var target in ISOMCheckList)
            {
                if (!isomtile.connectedtilenamelist.Contains(target.name) && isomtile != target)
                {
                    ISOMTile connectedTile = null;

                    //서로 연결되어 있지 않은 타일일 경우 일반적인 ISOM은 불가능 하다.
                    List<ISOMTile> connectedlist = new List<ISOMTile>();
                    List<ISOMTile> duplist = new List<ISOMTile>();
                    connectedlist.Add(target);
                    duplist.Add(target);

                    GetConnectedList(isomtile, target, connectedlist, duplist);

                   
                    if (connectedlist.Count > 1) connectedTile = connectedlist[connectedlist.Count - 2];


                    if (connectedTile == null)
                    {
                        //연결 할 ISOM을 찾지 못하면 끝
                        return false;
                    }

                    Dictionary<IWay, TileType> lines = tile9line;

                    if (target.elevation > isomtile.elevation)
                    {
                        //단계를 낮춰야한다.
                        if (!connectedTile.IsHasCliffDown)
                        {
                            lines = tile11downline;
                        }
                    }
                    else
                    {
                        //단계를 높여야한다.
                        if (!connectedTile.IsHasCliffDown && !isomtile.IsMiniISOM)
                        {
                            lines = tile13upline;
                        }
                    }

                    foreach (var tileitem in lines)
                    {
                        //tiles를 돌면서 해당 isom에 연결 명령을 내린다.
                        if(tileitem.Value.Tile == target)
                        {
                            bool issucess = ISOMExecute(mapeditor, tileSet, connectedTile, (int)tileitem.Value.Point.X, (int)tileitem.Value.Point.Y);
                    
                            IsIsomRefresh = true;
                        }
                    }

                }
            }

            if(glovar > 200)
            {
                return true;
            }

            //ISOM을 다시 깔면 목록을 리프레쉬해야하므로 다시 시작
            if (IsIsomRefresh)
            {
                glovar += 1;
                return ISOMExecute(mapeditor, tileSet, isomtile, tx, ty);
            }


            foreach (var target in tileList)
            {
                if (tileList.Count > 1 && target == isomtile) continue;

                //만약에 L과 R만 따로 튀는 경우
                if (target.elevation < isomtile.elevation)
                {
                    //if (!isomtile.IsMiniISOM && target != null && !target.IsHasCliffDown)
                    //{
                    //    if(!((LB.Check(target, TileBorder.Flat) || LB.Check(isomtile, TileBorder.FlatDownBorder))
                    //        && RB.Check(target, TileBorder.Flat) || RB.Check(isomtile, TileBorder.FlatDownBorder)))
                    //    {
                    //        continue;
                    //    }
                    //}

                    ISOMPaint(mapeditor, tileSet, isomtile, target, checker, tx, ty, true, IsTileChange);
                }
                else
                {
                    ISOMPaint(mapeditor, tileSet, target, isomtile, checker, tx, ty, false, IsTileChange);
                }


                //주변 모든 타일을 target과 isom으로 평준화 해야됨.
                /*
                 * 만약 High 정글
                 * 
                 * 
                 */
            }
            return true;
        }
        public static int glovar = 0;

        public enum DrawDirection
        {
            Left,
            Right,
            All
        }

        public static void DrawISOMTile(MapEditor mapeditor, ISOMGroup group, DrawDirection direction, int tilex, int tiley, int gindex = -1)
        {
            if(gindex == -1)
            {
                gindex = GetRdindex(group);
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
        public static void DrawISOMFlatTile(MapEditor mapeditor, ISOMTile isomtile, DrawDirection direction, int tilex, int tiley)
        {
            int max = 2;
            if (direction == DrawDirection.All) max = 4;

            for (int x = 0; x < max; x++)
            {
                int gtx = tilex + x;
                int gty = tiley;

                if (direction == DrawDirection.Left || direction == DrawDirection.All) gtx -= 2;


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
