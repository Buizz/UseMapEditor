using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UseMapEditor.Task;
using UseMapEditor.Tools;
using static Data.Map.MapData;

namespace UseMapEditor.MonoGameControl
{
    public partial class MapDrawer : WpfGame
    {
        double Thick
        {
            get
            {
                return 8 / mapeditor.opt_scalepercent;
            }
        }
        private void RenderLocation()
        {
            int gridsize = mapeditor.opt_grid;
            hoverLoc.Clear();
            lastsize = long.MaxValue;
            _spriteBatch.Begin( blendState: BlendState.NonPremultiplied);


            temp_dragscreenstartPos = mapeditor.PosMapToScreen(mouse_DragMapStart);
            temp_dragscreenendPos = MousePos;


            temp_dragmapgab = mapeditor.PosScreenToMap(MousePos) - mouse_DragMapStart;
            if(gridsize != 0)
            {
                temp_dragmapgab = new Vector2((float)(Math.Round(temp_dragmapgab.X / gridsize) * gridsize), (float)(Math.Round(temp_dragmapgab.Y / gridsize) * gridsize));
            }



            temp_dragmapmin = new Vector2(Math.Min(temp_dragscreenstartPos.X, temp_dragscreenendPos.X), Math.Min(temp_dragscreenstartPos.Y, temp_dragscreenendPos.Y));
            temp_dragmapmax = new Vector2(Math.Max(temp_dragscreenstartPos.X, temp_dragscreenendPos.X), Math.Max(temp_dragscreenstartPos.Y, temp_dragscreenendPos.Y));



            for (int i = 1; i < mapeditor.mapdata.GetLocationCount(); i++)
            {
                DrawLocation(mapeditor.mapdata.GetLocationFromListIndex(i));
            }

            if (mapeditor.PalleteLayer == Control.MapEditor.Layer.Location)
            {
                if (locIsmulitSelect)
                {
                    for (int i = 0; i < hoverLoc.Count; i++)
                    {
                        DrawLocation(hoverLoc[i]);
                    }
                }
                else
                {
                    if (hoverLoc.Count > 0)
                    {
                        DrawLocation(hoverLoc.First());
                    }
                }

                if (mouse_IsDrag)
                {

                    if (locIsmulitSelect)
                    {
                        //선택
                        Vector2 min = new Vector2(Math.Min(temp_dragscreenstartPos.X, MousePos.X), Math.Min(temp_dragscreenstartPos.Y, MousePos.Y));
                        Vector2 max = new Vector2(Math.Max(temp_dragscreenstartPos.X, MousePos.X), Math.Max(temp_dragscreenstartPos.Y, MousePos.Y));
                        Vector2 size = max - min;

                        DrawRect(_spriteBatch, temp_dragscreenstartPos, MousePos, Color.LimeGreen, 2);
                        _spriteBatch.Draw(gridtexture, new Rectangle((int)min.X, (int)min.Y, (int)size.X, (int)size.Y), null, new Color(128, 255, 128, 64), 0, new Vector2(), SpriteEffects.None, 0);
                    }
                    else if (mapeditor.SelectLocation.Count == 0)
                    {
                        //생성

                        if (gridsize == 0)
                        {
                            gridsize = 1;
                        }

                        Vector2 StartPos = new Vector2((float)(Math.Round(mouse_DragMapStart.X / gridsize) * gridsize), (float)(Math.Round(mouse_DragMapStart.Y / gridsize) * gridsize));
                        Vector2 EndPos = mapeditor.PosScreenToMap(MousePos);
                        EndPos = new Vector2((float)(Math.Round(EndPos.X / gridsize) * gridsize), (float)(Math.Round(EndPos.Y / gridsize) * gridsize));



                        Vector2 min = new Vector2(Math.Min(StartPos.X, EndPos.X), Math.Min(StartPos.Y, EndPos.Y));
                        Vector2 max = new Vector2(Math.Max(StartPos.X, EndPos.X), Math.Max(StartPos.Y, EndPos.Y));




                        temp_locCreatemin = new Vector2(0, 0);
                        temp_locCreatemax = new Vector2(mapeditor.mapdata.WIDTH * 32, mapeditor.mapdata.HEIGHT * 32);


                        temp_locCreatemin = VectorTool.Max(temp_locCreatemin, min);
                        temp_locCreatemax = VectorTool.Min(temp_locCreatemax, max);



                        Vector2 LocSize = temp_locCreatemax - temp_locCreatemin;




                        temp_locCreatemin = mapeditor.PosMapToScreen(temp_locCreatemin);
                        temp_locCreatemax = mapeditor.PosMapToScreen(temp_locCreatemax);






                        temp_locCreatesize = temp_locCreatemax - temp_locCreatemin;
                        if (temp_locCreatesize.X > 0 & temp_locCreatesize.Y > 0)
                        {
                            DrawLocationRect(_spriteBatch, string.Format("{0:0.0} ", (LocSize.X / 32)) + " X " + string.Format("{0:0.0} ", (LocSize.Y / 32)), temp_locCreatemin, temp_locCreatemax, Color.White, (float)(2 / mapeditor.opt_scalepercent), (float)(2 * mapeditor.opt_scalepercent));
                            DrawRect(_spriteBatch, temp_locCreatemin, temp_locCreatemax, Color.White, 2);
                            _spriteBatch.Draw(gridtexture, new Rectangle((int)temp_locCreatemin.X, (int)temp_locCreatemin.Y, (int)temp_locCreatesize.X, (int)temp_locCreatesize.Y), null, new Color(128, 128, 255, 64), 0, new Vector2(), SpriteEffects.None, 0);
                        }


                    }
                }
            }
            
            _spriteBatch.End();



            if (mapeditor.PalleteLayer == Control.MapEditor.Layer.Location)
            {
             
                if (hoverLoc.Count >= 1)
                {
                    for (int i = 0; i < hoverLoc.Count; i++)
                    {
                        if (mapeditor.SelectLocation.IndexOf(hoverLoc[0]) == -1)
                        {
                            //하나라도 포함되지 않은게 있을 경우 탈출
                            return;
                        }
                    }



                    //사이즈 체인지인지 확인
                    for (int i = 0; i < hoverLoc.Count; i++)
                    {
                        Vector2 min = new Vector2(Math.Min(hoverLoc[i].L, hoverLoc[i].R), Math.Min(hoverLoc[i].T, hoverLoc[i].B));
                        Vector2 max = new Vector2(Math.Max(hoverLoc[i].L, hoverLoc[i].R), Math.Max(hoverLoc[i].T, hoverLoc[i].B));
                        Vector2 size = max - min;

                        IsL = false;
                        IsR = false;
                        IsT = false;
                        IsB = false;
                        if ((size.X < Thick * 2.5) | (size.Y < Thick * 2.5))
                        {
                            mapeditor.MouseCursorChange(Cursors.SizeAll);
                            return;
                        }

                        Vector2 mousepos = mapeditor.PosScreenToMap(MousePos);



                        if (min.X - Thick < mousepos.X & mousepos.X < min.X + Thick)
                        {
                            IsL = true;
                        }
                        if (max.X - Thick < mousepos.X & mousepos.X < max.X + Thick)
                        {
                            IsR = true;
                        }
                        if (min.Y - Thick < mousepos.Y & mousepos.Y < min.Y + Thick)
                        {
                            IsT = true;
                        }
                        if (max.Y - Thick < mousepos.Y & mousepos.Y < max.Y + Thick)
                        {
                            IsB = true;
                        }

                        if (IsL)
                        {
                            if (IsT)
                            {
                                mapeditor.MouseCursorChange(Cursors.SizeNWSE);
                                return;
                            }
                            else if (IsB)
                            {
                                mapeditor.MouseCursorChange(Cursors.SizeNESW);
                                return;
                            }
                            else
                            {
                                mapeditor.MouseCursorChange(Cursors.SizeWE);
                                return;
                            }
                        }
                        if (IsR)
                        {
                            if (IsT)
                            {
                                mapeditor.MouseCursorChange(Cursors.SizeNESW);
                                return;
                            }
                            else if (IsB)
                            {
                                mapeditor.MouseCursorChange(Cursors.SizeNWSE);
                                return;
                            }
                            else
                            {
                                mapeditor.MouseCursorChange(Cursors.SizeWE);
                                return;
                            }
                        }
                        if (IsB)
                        {
                            mapeditor.MouseCursorChange(Cursors.SizeNS);
                            return;
                        }
                        if (IsT)
                        {
                            mapeditor.MouseCursorChange(Cursors.SizeNS);
                            return;
                        }
                    }
                    
                    mapeditor.MouseCursorChange(Cursors.SizeAll);
                }
            }
            
        }



        bool IsL = false;
        bool IsR = false;
        bool IsT = false;
        bool IsB = false;

        private void LocationDelete()
        {
            mapeditor.taskManager.TaskStart();
            for (int i = 0; i < mapeditor.SelectLocation.Count; i++)
            {
                mapeditor.taskManager.TaskAdd(new LocationEvent(mapeditor ,mapeditor.SelectLocation[i], false));
                mapeditor.mapdata.RemoveLocation(mapeditor.SelectLocation[i]);
            }
            mapeditor.taskManager.TaskEnd();
            mapeditor.SelectLocation.Clear();

        }



        //선택모드 false일 경우 생산모드임
        bool locIsmulitSelect;
        bool locmovemode;

        

        private void LocationRightMouseClick()
        {
            if(mapeditor.SelectLocation.Count == 0)
            {
                //여기서 단일 선택됨.
                if (hoverLoc.Count == 1)
                {
                    mapeditor.SelectLocation.Clear();
                    mapeditor.SelectLocation.Add(hoverLoc[0]);
                    if (mapeditor.SelectLocation.Count == 1 && hoverLoc[0] == mapeditor.SelectLocation[0])
                    {
                        mapeditor.OpenLocEdit();
                    }
                }
            }
            else if(mapeditor.SelectLocation.Count == 1)
            {
                if (hoverLoc.Count == 1)
                {
                    mapeditor.SelectLocation.Clear();
                    mapeditor.SelectLocation.Add(hoverLoc[0]);
                    if (mapeditor.SelectLocation.Count == 1 && hoverLoc[0] == mapeditor.SelectLocation[0])
                    {
                        mapeditor.OpenLocEdit();
                    }
                }
            }
            else
            {
                mapeditor.OpenLocEdit();
            }

        }
        private void LocationLeftMouseClick()
        {
            //여기서 단일 선택됨.
            if(hoverLoc.Count == 1)
            {
                mapeditor.SelectLocation.Clear();
                mapeditor.SelectLocation.Add(hoverLoc[0]);
            }
            else if(hoverLoc.Count > 1)
            {
                //로케이션이 겹쳐있을 경우
                mapeditor.OpenLocSelecter((int)MousePos.X, (int)MousePos.Y, hoverLoc);
            }
            else if(hoverLoc.Count == 0)
            {
                //로케이션이 아무것도 없을 경우
                mapeditor.SelectLocation.Clear();
            }
        }

        bool locdragIsL = false;
        bool locdragIsR = false;
        bool locdragIsT = false;
        bool locdragIsB = false;


        private void LocationDragStart()
        {
            locIsmulitSelect = !key_QDown;
            if (locIsmulitSelect)
            {
                if (hoverLoc.Count >= 1)
                {
                    bool DragFlag = false;

                    for (int i = 0; i < mapeditor.SelectLocation.Count; i++)
                    {
                        if (hoverLoc.IndexOf(mapeditor.SelectLocation[i]) != -1)
                        {
                            //포함된게 하나있으면 트루
                            DragFlag = true;
                            break;
                        }
                    }
                    if (DragFlag)
                    {
                        //사이즈 체인지인지 확인
                        locdragIsL = IsL;
                        locdragIsR = IsR;
                        locdragIsT = IsT;
                        locdragIsB = IsB;

                        //드래그 시작
                        locmovemode = true;
                        locIsmulitSelect = false;
                    }
                }
            }
            else
            {
                //호버링이 없을 경우 밖이므로
                mapeditor.SelectLocation.Clear();
            }
        }


        private void LocationDragEnd()
        {
            if (locmovemode)
            {
                //드래그 성공

                mapeditor.taskManager.TaskStart();
                for (int i = 0; i < mapeditor.SelectLocation.Count; i++)
                {
                    int oX, oY, oW, oH;
                    LocationData locationData = mapeditor.SelectLocation[i];

                    GetDragLocSize(locationData, out oX, out oY, out oW, out oH);

                    //이동 중이고 선택된 항목일 경우


                    locationData.X = (uint)oX;
                    locationData.Y = (uint)oY;
                    locationData.WIDTH = oW;
                    locationData.HEIGHT = oH;
                }
                mapeditor.taskManager.TaskEnd();
            }


            if (locIsmulitSelect)
            {
                //선택모드
                mapeditor.SelectLocation.Clear();
                for (int i = 0; i < hoverLoc.Count; i++)
                {
                    mapeditor.SelectLocation.Add(hoverLoc[i]);
                }
            }
            else
            {
                if(mapeditor.SelectLocation.Count == 0)
                {
                    //생산모드
                    LocationCreate();
                }
                if (!locmovemode)
                {
                    mapeditor.SelectLocation.Clear();
                }
            }
            locmovemode = false;
            locIsmulitSelect = false;
        }

        private void GetDragLocSize(LocationData loc, out int X, out int Y, out int W, out int H)
        {

            int x = (int)loc.X;
            int y = (int)loc.Y;
            int w = (int)loc.WIDTH;
            int h = (int)loc.HEIGHT;

            bool wnegative = (w < 0);
            bool hnegative = (h < 0);


            w = Math.Abs(w);
            h = Math.Abs(h);




            if (!locdragIsL & !locdragIsR & !locdragIsT & !locdragIsB)
            {
                x += (int)temp_dragmapgab.X;
                y += (int)temp_dragmapgab.Y;
            }
            else
            {
                if (locdragIsL)
                {
                    x += (int)temp_dragmapgab.X;
                    w -= (int)temp_dragmapgab.X;
                }
                if (locdragIsR)
                {
                    w += (int)temp_dragmapgab.X;
                }
                if (locdragIsT)
                {
                    y += (int)temp_dragmapgab.Y;
                    h -= (int)temp_dragmapgab.Y;
                }
                if (locdragIsB)
                {
                    h += (int)temp_dragmapgab.Y;
                }
            }


            ////끝선 처리
            //int minx = Math.Min(x, x + w);
            //int miny = Math.Min(y, y + h);
            ////Vector2 min = new Vector2(Math.Min(x, x + w), Math.Min(y, y + h));
            ////Vector2 max = new Vector2(Math.Max(x, x + w), Math.Max(y, y + h));


            //w = Math.Abs(w);
            //h = Math.Abs(h);


            //minx = Math.Max(0, minx);
            //miny = Math.Max(0, miny);

            //if ((minx + w) > mapeditor.mapdata.WIDTH * 32)
            //{
            //    minx = mapeditor.mapdata.WIDTH * 32 - w;
            //}
            //if ((miny + h) > mapeditor.mapdata.HEIGHT * 32)
            //{
            //    miny = mapeditor.mapdata.HEIGHT * 32 - h;
            //}


            //X = minx;
            //Y = miny;

            int minx = Math.Min(x, x + w);
            int miny = Math.Min(y, y + h);

            w = Math.Abs(w);
            h = Math.Abs(h);


            minx = Math.Max(0, minx);
            miny = Math.Max(0, miny);

            if ((minx + w) > mapeditor.mapdata.WIDTH * 32)
            {
                minx = mapeditor.mapdata.WIDTH * 32 - w;
            }
            if ((miny + h) > mapeditor.mapdata.HEIGHT * 32)
            {
                miny = mapeditor.mapdata.HEIGHT * 32 - h;
            }

            if(w >= mapeditor.mapdata.WIDTH * 32)
            {
                w = mapeditor.mapdata.WIDTH * 32;
                minx = 0;
            }

            if (h >= mapeditor.mapdata.HEIGHT * 32)
            {
                h = mapeditor.mapdata.HEIGHT * 32;
                miny = 0;
            }




            x = minx;
            y = miny;

            if (wnegative)
            {
                w = -w;
            }

            if (hnegative)
            {
                h = -h;
            }


            double grid = mapeditor.opt_grid;
            if(grid != 0)
            {
                x = (int)(Math.Round(x / grid) * grid);
                y = (int)(Math.Round(y / grid) * grid);
                w = (int)(Math.Round(w / grid) * grid);
                h = (int)(Math.Round(h / grid) * grid);
            }

            X = x;
            Y = y;
            W = w;
            H = h;
        }




        Vector2 temp_dragscreenstartPos;
        Vector2 temp_dragscreenendPos;

        Vector2 temp_dragmapgab;


        Vector2 temp_dragmapmin;
        Vector2 temp_dragmapmax;


        Vector2 temp_locCreatemin;
        Vector2 temp_locCreatemax;
        Vector2 temp_locCreatesize;



        private void LocationCreate()
        {
            if (mouse_IsDrag & !key_LeftShiftDown & mapeditor.SelectLocation.Count == 0)
            {

                if (temp_locCreatesize.X <= 0 | temp_locCreatesize.Y <= 0)
                {
                    return;
                }

                //로케이션 만들기
                int index = 0;
                for (int i = 1; i < 256; i++)
                {
                    LocationData tloc = mapeditor.mapdata.GetLocationFromLocIndex(i);

                    if (!tloc.IsEnabled)
                    {
                        index = i;
                        break;
                    }
                }
                if (index == 0)
                {
                    //에러발생
                    return;
                }


                LocationData locationData = new LocationData(mapeditor);

                locationData.INDEX = index;
                locationData.NAME = mapeditor.mapdata.GetNextLocationName(index);


                Vector2 mapmin = mapeditor.PosScreenToMap(temp_locCreatemin);
                Vector2 mapmax = mapeditor.PosScreenToMap(temp_locCreatemax);



                locationData.L = (uint)mapmin.X;
                locationData.R = (uint)mapmax.X;
                locationData.T = (uint)mapmin.Y;
                locationData.B = (uint)mapmax.Y;


                mapeditor.mapdata.AddLocation(locationData);
                mapeditor.taskManager.TaskStart();
                mapeditor.taskManager.TaskAdd(new LocationEvent(mapeditor ,locationData, true));
                mapeditor.taskManager.TaskEnd();
                return;
            }
        }



        private List<LocationData> hoverLoc = new List<LocationData>();
        private long lastsize;
        private void DrawLocation(LocationData location)
        {
            if (!location.IsEnabled) return;

            uint L = location.L;
            uint R = location.R;
            uint T = location.T;
            uint B = location.B;

            if(locmovemode & mapeditor.SelectLocation.IndexOf(location) != -1)
            {
                int oX, oY, oW, oH;
                GetDragLocSize(location, out oX, out oY, out oW, out oH);


                L = (uint)oX;
                R = (uint)(oX + Math.Abs(oW));

                T = (uint)oY;
                B = (uint)(oY + Math.Abs(oH));
            }



            uint minX = Math.Min(L, R);
            uint maxX = Math.Max(L, R);
            uint minY = Math.Min(T, B);
            uint maxY = Math.Max(T, B);
            uint locwidth = maxX - minX;
            uint locheight = maxY - minY;

            long locsize = locwidth * locheight;

            if ((locwidth == mapeditor.mapdata.WIDTH * 32) & (locheight == mapeditor.mapdata.HEIGHT * 32))
            {
                return;
            }


            float MinSize = Math.Min(locwidth, locheight);

            float mag = MinSize / 150;

            mag = Math.Max(0.5f, mag);
            mag = Math.Min(4, mag);



            Vector2 min = mapeditor.PosMapToScreen(new Vector2(minX, minY));
            Vector2 max = mapeditor.PosMapToScreen(new Vector2(maxX, maxY));
            Vector2 size = max - min;


            //float screenminX = 0;
            //float screenminY = 0;
            //float screenmaxX = screenwidth;
            //float screenmaxY = screenheight;





            if (mapeditor.PalleteLayer == Control.MapEditor.Layer.Location)
            {
                //호버드로우
                if (hoverLoc.IndexOf(location) != -1)
                {
                    DrawLocationRect(_spriteBatch, location.STRING.String, min, max, Color.White, mag, (float)(2 * mapeditor.opt_scalepercent));
                    return;
                }

                //호버링
                if (locIsmulitSelect)
                {
                    if (temp_dragmapmin.X < min.X & max.X < temp_dragmapmax.X)
                    {
                        if (temp_dragmapmin.Y < min.Y & max.Y < temp_dragmapmax.Y)
                        {
                            hoverLoc.Add(location);
                        }
                    }
                }
                else
                {
                    if (min.X - Thick < MousePos.X & MousePos.X < max.X + Thick)
                    {
                        if (min.Y - Thick < MousePos.Y & MousePos.Y < max.Y + Thick)
                        {
                            if (locsize < lastsize)
                            {
                                hoverLoc.Clear();
                                lastsize = locsize;
                                hoverLoc.Add(location);
                            }
                            else if (lastsize == locsize)
                            {
                                if((hoverLoc[0].X == location.X) & (hoverLoc[0].Y == location.Y))
                                {
                                    hoverLoc.Add(location);
                                }
                            }
                        }
                    }
                }
            }



            //로케이션 배경 그리기
            if (mapeditor.SelectLocation.IndexOf(location) != -1)
            {
                //선택된거 하이라이트
                _spriteBatch.Draw(gridtexture, new Rectangle((int)min.X, (int)min.Y, (int)size.X, (int)size.Y), null, new Color(128, 255, 128, 64), 0, new Vector2(), SpriteEffects.None, 0);
            }
            else
            {
                //일반
                _spriteBatch.Draw(gridtexture, new Rectangle((int)min.X, (int)min.Y, (int)size.X, (int)size.Y), null, location.RnColor, 0, new Vector2(), SpriteEffects.None, 0);
            }


            //윤곽선과 로케이션 이름 그리기
            if (location.WIDTH < 0 | location.HEIGHT < 0)
            {
                //인버스 로케이션
                DrawLocationRect(_spriteBatch, location.STRING.String, min, max, Color.Red, mag, (float)(2 * mapeditor.opt_scalepercent));
            }
            else
            {
                if (mapeditor.SelectLocation.IndexOf(location) != -1)
                {
                    //선택
                    DrawLocationRect(_spriteBatch, location.STRING.String, min, max, Color.White, mag, (float)(2 * mapeditor.opt_scalepercent));
                }
                else
                {
                    //일반
                    DrawLocationRect(_spriteBatch, location.STRING.String, min, max, Color.Black, mag, (float)(2 * mapeditor.opt_scalepercent));
                }
            }
        }




        public void DrawLocationRect(SpriteBatch spriteBatch, string str, Vector2 point1, Vector2 point2, Color color, float loSize, float thickness = 1f)
        {
            DrawLine(spriteBatch, new Vector2(point1.X, point1.Y), new Vector2(point2.X, point1.Y), color, thickness);
            DrawLine(spriteBatch, new Vector2(point1.X, point1.Y), new Vector2(point1.X, point2.Y), color, thickness);
            DrawLine(spriteBatch, new Vector2(point2.X, point1.Y), new Vector2(point2.X, point2.Y), color, thickness);
            DrawLine(spriteBatch, new Vector2(point2.X, point2.Y), new Vector2(point1.X, point2.Y), color, thickness);

            color.A = 64;
            DrawLine(spriteBatch, new Vector2(point1.X, point1.Y), new Vector2(point2.X, point2.Y), color, thickness);
            DrawLine(spriteBatch, new Vector2(point2.X, point1.Y), new Vector2(point1.X, point2.Y), color, thickness);


            Vector2 drawSize = point2 - point1;



            loSize *= (float)mapeditor.opt_scalepercent;

            int stra = 255;
            stra -= (int)loSize * 30;
            stra = Math.Max(stra, 0);
            stra = Math.Min(stra, 255);

            if(str == null)
            {
                str = "???";
            }

            Vector2 strsize = _locationfont.MeasureString(str);
            strsize *= loSize;


            if (strsize.X > drawSize.X)
            {
                str = str.Substring(0, str.Length / 2).Trim() + "\n" + str.Substring(str.Length / 2).Trim();
                strsize = _locationfont.MeasureString(str);
                strsize *= loSize;
            }




            spriteBatch.DrawString(_locationfont, str, (point1 + point2) / 2 - strsize / 2, new Color(255,255,255,stra), 0, new Vector2(), loSize, SpriteEffects.None, 0);
        }

    }
}
