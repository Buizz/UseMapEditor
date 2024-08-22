using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UseMapEditor.Control;
using UseMapEditor.FileData;
using UseMapEditor.Task.Events;
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



        public static string ISOMCheckTile(MapEditor mapeditor, TileSet tileSet, int tilex, int tiley)
        {
            List<ISOMTIle> iSOMTIles = tileSet.GetISOMData(mapeditor);


            ushort LT = mapeditor.mapdata.TILE[tilex - 1 + (tiley - 1) * mapeditor.mapdata.WIDTH];
            ushort RT = mapeditor.mapdata.TILE[tilex + (tiley - 1) * mapeditor.mapdata.WIDTH];
            ushort LB = mapeditor.mapdata.TILE[tilex - 1 + tiley * mapeditor.mapdata.WIDTH];
            ushort RB = mapeditor.mapdata.TILE[tilex + tiley * mapeditor.mapdata.WIDTH];


            //우선 플랫타일을 돌면서 확인한다...

            foreach (var item in iSOMTIles)
            {
                ISOMTIle.TileBorder border = item.CheckTile(LT, RT, LB, RB);

                if (border == ISOMTIle.TileBorder.Flat)
                {
                    return item.name;
                }
                else if (border == ISOMTIle.TileBorder.DownBorder)
                {
                    return item.name + "-DownBorder";
                }
                else if (border == ISOMTIle.TileBorder.UpBorder)
                {
                    return item.name + "-UpBorder";
                }

            }

            //Dirt
            //HighDirt
            //Dirt-HighDirt
            //....

            return "";
        }

        public static void ISOMExecute(MapEditor mapeditor, TileSet tileSet, ISOMTIle isomtile, int tilex, int tiley)
        {
            Dictionary<string, string> terrain = new Dictionary<string, string>();

            terrain.Add("L", ISOMCheckTile(mapeditor, tileSet, tilex - 4, tiley));
            terrain.Add("LT", ISOMCheckTile(mapeditor, tileSet, tilex - 2, tiley - 1));
            terrain.Add("T", ISOMCheckTile(mapeditor, tileSet, tilex, tiley - 2));
            terrain.Add("RT", ISOMCheckTile(mapeditor, tileSet, tilex + 2, tiley - 1));
            terrain.Add("R", ISOMCheckTile(mapeditor, tileSet, tilex + 4, tiley));
            terrain.Add("RB", ISOMCheckTile(mapeditor, tileSet, tilex + 2, tiley + 1));
            terrain.Add("LB", ISOMCheckTile(mapeditor, tileSet, tilex - 2, tiley + 1));
            terrain.Add("B", ISOMCheckTile(mapeditor, tileSet, tilex, tiley + 2));


            terrain.Add("LTT", ISOMCheckTile(mapeditor, tileSet, tilex - 2, tiley - 3));
            terrain.Add("RTT", ISOMCheckTile(mapeditor, tileSet, tilex + 2, tiley - 3));
            terrain.Add("RBB", ISOMCheckTile(mapeditor, tileSet, tilex + 2, tiley + 3));
            terrain.Add("LBB", ISOMCheckTile(mapeditor, tileSet, tilex - 2, tiley + 3));


            #region 기둥 세우기
            if (terrain["L"] == isomtile.connectlowtile.name
                && terrain["LT"] == isomtile.connectlowtile.name
                && terrain["T"] == isomtile.connectlowtile.name
                && terrain["RT"] == isomtile.connectlowtile.name
                && terrain["R"] == isomtile.connectlowtile.name
                && terrain["RB"] == isomtile.connectlowtile.name
                && terrain["LB"] == isomtile.connectlowtile.name
                && terrain["B"] == isomtile.connectlowtile.name)
            {
                ISOMTool.DrawISOMTile(mapeditor, isomtile.tip_default, DrawDirection.Left, tilex - 2, tiley - 1);
                ISOMTool.DrawISOMTile(mapeditor, isomtile.tip_default, DrawDirection.Right, tilex, tiley - 1);
            }


            if (terrain["L"] == isomtile.name + "-DownBorder"
                && terrain["LT"] == isomtile.connectlowtile.name
                && terrain["T"] == isomtile.connectlowtile.name
                && terrain["RT"] == isomtile.connectlowtile.name
                && terrain["R"] == isomtile.connectlowtile.name
                && terrain["RB"] == isomtile.connectlowtile.name
                && terrain["LB"] == isomtile.connectlowtile.name
                && terrain["B"] == isomtile.connectlowtile.name)
            {
                ISOMTool.DrawISOMTile(mapeditor, isomtile.tip_default, DrawDirection.Left, tilex - 2, tiley - 1);
                ISOMTool.DrawISOMTile(mapeditor, isomtile.tip_default, DrawDirection.Right, tilex, tiley - 1);
            }


            if (terrain["L"] == isomtile.connectlowtile.name
                && terrain["LT"] == isomtile.connectlowtile.name
                && terrain["T"] == isomtile.connectlowtile.name
                && terrain["RT"] == isomtile.connectlowtile.name
                && terrain["R"] == isomtile.name + "-DownBorder"
                && terrain["RB"] == isomtile.connectlowtile.name
                && terrain["LB"] == isomtile.connectlowtile.name
                && terrain["B"] == isomtile.connectlowtile.name)
            {
                ISOMTool.DrawISOMTile(mapeditor, isomtile.tip_default, DrawDirection.Left, tilex - 2, tiley - 1);
                ISOMTool.DrawISOMTile(mapeditor, isomtile.tip_default, DrawDirection.Right, tilex, tiley - 1);
            }

            if ((terrain["L"] == isomtile.connectlowtile.name || terrain["L"] == isomtile.name + "-DownBorder")
               && terrain["LT"] == isomtile.connectlowtile.name
               && terrain["T"] == isomtile.name + "-DownBorder"
               && terrain["RT"] == isomtile.connectlowtile.name
               && (terrain["R"] == isomtile.connectlowtile.name || terrain["R"] == isomtile.name + "-DownBorder")
               && terrain["RB"] == isomtile.connectlowtile.name
               && terrain["LB"] == isomtile.connectlowtile.name
               && terrain["B"] == isomtile.connectlowtile.name)
            {
                if (terrain["LTT"] == isomtile.name + "-DownBorder")
                    ISOMTool.DrawISOMTile(mapeditor, isomtile.tip_toplong, DrawDirection.Left, tilex - 2, tiley - 1);
                else
                    ISOMTool.DrawISOMTile(mapeditor, isomtile.tip_top, DrawDirection.Left, tilex - 2, tiley - 1);

                if (terrain["RTT"] == isomtile.name + "-DownBorder")
                    ISOMTool.DrawISOMTile(mapeditor, isomtile.tip_toplong, DrawDirection.Right, tilex, tiley - 1);
                else
                    ISOMTool.DrawISOMTile(mapeditor, isomtile.tip_top, DrawDirection.Right, tilex, tiley - 1);
            }

            if ((terrain["L"] == isomtile.connectlowtile.name || terrain["L"] == isomtile.name + "-DownBorder")
                && terrain["LT"] == isomtile.connectlowtile.name
                && terrain["T"] == isomtile.connectlowtile.name
                && terrain["RT"] == isomtile.connectlowtile.name
                && (terrain["R"] == isomtile.connectlowtile.name || terrain["R"] == isomtile.name + "-DownBorder")
                && terrain["RB"] == isomtile.connectlowtile.name
                && terrain["LB"] == isomtile.connectlowtile.name
                && terrain["B"] == isomtile.name + "-DownBorder")
            {
                if (terrain["LBB"] == isomtile.name + "-DownBorder")
                    ISOMTool.DrawISOMTile(mapeditor, isomtile.tip_bottomlong, DrawDirection.Left, tilex - 2, tiley - 1);
                else
                    ISOMTool.DrawISOMTile(mapeditor, isomtile.tip_bottom, DrawDirection.Left, tilex - 2, tiley - 1);

                if (terrain["RBB"] == isomtile.name + "-DownBorder")
                    ISOMTool.DrawISOMTile(mapeditor, isomtile.tip_bottomlong, DrawDirection.Right, tilex, tiley - 1);
                else
                    ISOMTool.DrawISOMTile(mapeditor, isomtile.tip_bottom, DrawDirection.Right, tilex, tiley - 1);


            }

            if ((terrain["L"] == isomtile.connectlowtile.name || terrain["L"] == isomtile.name + "-DownBorder")
                && terrain["LT"] == isomtile.connectlowtile.name
                && terrain["T"] == isomtile.name + "-DownBorder"
                && terrain["RT"] == isomtile.connectlowtile.name
                && (terrain["R"] == isomtile.connectlowtile.name || terrain["R"] == isomtile.name + "-DownBorder")
                && terrain["RB"] == isomtile.connectlowtile.name
                && terrain["LB"] == isomtile.connectlowtile.name
                && terrain["B"] == isomtile.name + "-DownBorder")
            {
                if (terrain["LTT"] == isomtile.name + "-DownBorder" && terrain["LBB"] == isomtile.name + "-DownBorder")
                    ISOMTool.DrawISOMTile(mapeditor, isomtile.tip_doublelong, DrawDirection.Left, tilex - 2, tiley - 1);
                else if (terrain["LTT"] == isomtile.name + "-DownBorder")
                    ISOMTool.DrawISOMTile(mapeditor, isomtile.tip_doubletoplong, DrawDirection.Left, tilex - 2, tiley - 1);
                else if (terrain["LBB"] == isomtile.name + "-DownBorder")
                    ISOMTool.DrawISOMTile(mapeditor, isomtile.tip_doublebottomlong, DrawDirection.Left, tilex - 2, tiley - 1);
                else
                    ISOMTool.DrawISOMTile(mapeditor, isomtile.tip_double, DrawDirection.Left, tilex - 2, tiley - 1);


                if (terrain["RTT"] == isomtile.name + "-DownBorder" && terrain["RBB"] == isomtile.name + "-DownBorder")
                    ISOMTool.DrawISOMTile(mapeditor, isomtile.tip_doublelong, DrawDirection.Right, tilex, tiley - 1);
                else if (terrain["RTT"] == isomtile.name + "-DownBorder")
                    ISOMTool.DrawISOMTile(mapeditor, isomtile.tip_doubletoplong, DrawDirection.Right, tilex, tiley - 1);
                else if (terrain["RBB"] == isomtile.name + "-DownBorder")
                    ISOMTool.DrawISOMTile(mapeditor, isomtile.tip_doublebottomlong, DrawDirection.Right, tilex, tiley - 1);
                else
                    ISOMTool.DrawISOMTile(mapeditor, isomtile.tip_double, DrawDirection.Right, tilex, tiley - 1);

            }
            #endregion




        }

        public enum DrawDirection
        {
            Left,
            Right
        }

        public static void DrawISOMTile(MapEditor mapeditor, ISOMTIle.ISOMGroup group, DrawDirection direction, int tilex, int tiley)
        {
            int gindex = ISOMTIle.GetRdindex(group);
            for (int i = 0; i < group.PartLength; i++)
            {
                int lmx = tilex + i % 2;
                int lmy = tiley + i / 2;
                if (mapeditor.mapdata.CheckTILERange(lmx, lmy))
                {
                    int tileindex = lmx + lmy * mapeditor.mapdata.WIDTH;
                    ushort oldMTXM = mapeditor.mapdata.TILE[tileindex];


                    //Group먼저 찾아본다...
                    //색칠될 타일들을 조사해서 어떤타일인지 알아내야하나..?


                    //byte oldgroupindex = (byte)(oldMTXM % 16);

                    ushort newMTXM = 0;
                    if (direction == DrawDirection.Left)
                    {
                        //newMTXM = group.left_tiles[i].GetFromGroup(oldgroupindex);
                        newMTXM = group.left_tiles[i][gindex];
                    }
                    else if (direction == DrawDirection.Right)
                    {
                        //newMTXM = group.right_tiles[i].GetFromGroup(oldgroupindex);
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
