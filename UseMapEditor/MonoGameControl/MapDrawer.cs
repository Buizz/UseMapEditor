﻿using Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using SpriteFontPlus;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using UseMapEditor.Control;
using UseMapEditor.FileData;
using UseMapEditor.Global;
using UseMapEditor.Tools;
using WpfTest.Components;

namespace UseMapEditor.MonoGameControl
{
    public partial class MapDrawer : WpfGame
    {
        private UseMapEditor.Control.MapEditor mapeditor;
        public void ChangeMap(UseMapEditor.Control.MapEditor _mapeditor)
        {
            mapeditor = _mapeditor;
        }

        public Color GridColor;


        private IGraphicsDeviceService _graphicsDeviceManager;
        private WpfKeyboard _keyboard;
        private CustomWpfMouse _mouse;

        private SpriteBatch _spriteBatch;
        private SpriteBatch _colorBatch;
        private SpriteFont _font;
        private SpriteFont _locationfont;

        BlendState ColorBlend;
        BlendState HallucinateBlend;

        private Vector2[] SDGRPSIZE;
        protected override void Initialize()
        {
            // must be initialized. required by Content loading and rendering (will add itself to the Services)
            // note that MonoGame requires this to be initialized in the constructor, while WpfInterop requires it to
            // be called inside Initialize (before base.Initialize())
            _graphicsDeviceManager = new WpfGraphicsDeviceService(this);

            // wpf and keyboard need reference to the host control in order to receive input
            // this means every WpfGame control will have it's own keyboard & mouse manager which will only react if the mouse is in the control
            _keyboard = new WpfKeyboard(this);
            _mouse = new CustomWpfMouse(this);

            //WpfMouse

            //Components.Add(new FpsComponent(this));

            // must be called after the WpfGraphicsDeviceService instance was created
            base.Initialize();

            GraphicsDevice.PresentationParameters.IsFullScreen = true;
            GraphicsDevice.PresentationParameters.HardwareModeSwitch = true;
            GraphicsDevice.PresentationParameters.BackBufferWidth = GraphicsDevice.DisplayMode.Width;
            GraphicsDevice.PresentationParameters.BackBufferHeight = GraphicsDevice.DisplayMode.Height;



            string gridcolorstr = Global.Setting.Vals[Global.Setting.Settings.Program_GridColor];
            uint gridcolorcode;
            if (uint.TryParse(gridcolorstr, out gridcolorcode))
            {
                GridColor = new Color(gridcolorcode);
            }
            else
            {
                GridColor = Color.Black;
            }

            int.TryParse(Global.Setting.Vals[Global.Setting.Settings.Render_MaxFrame], out MaxFrame);
            updateTick = 1f / MaxFrame * 10000000;





            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _colorBatch = new SpriteBatch(GraphicsDevice);

            ColorBlend = new BlendState();
            ColorBlend.ColorBlendFunction = BlendFunction.Add;
            ColorBlend.ColorSourceBlend = Blend.DestinationColor;
            ColorBlend.ColorDestinationBlend = Blend.InverseSourceAlpha;



            HallucinateBlend = new BlendState();

            HallucinateBlend.ColorBlendFunction = BlendFunction.Add;

            HallucinateBlend.ColorSourceBlend = Blend.DestinationColor;
            HallucinateBlend.ColorDestinationBlend = Blend.InverseSourceAlpha;



            DownKeys = new List<Microsoft.Xna.Framework.Input.Keys>();
            UpKeys = new List<Microsoft.Xna.Framework.Input.Keys>();

            BinaryReader br = new BinaryReader(new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\Data\\SDGRPSIZE", FileMode.Open));

            SDGRPSIZE = new Vector2[999];
            for (int i = 0; i < 999; i++)
            {
                SDGRPSIZE[i] = new Vector2(br.ReadUInt32(), br.ReadUInt32());
            }

            br.Close();


            //shader = new Effect(GraphicsDevice, File.ReadAllBytes("Content\\effect"));
        }
        public Effect shader;
        private Texture2D gridtexture;
        private Texture2D isomtexture;
        protected override void LoadContent()
        {
            base.LoadContent();


            CharacterRange Hangle = new CharacterRange((char)44032, (char)55203);
            // content loading now possible
            var fontBakeResult = TtfFontBaker.Bake(File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "\\Font\\NanumBarunGothicBold.ttf"), 25, 4096, 4096, new[]{
                 CharacterRange.BasicLatin, Hangle, new CharacterRange((char) 12593, (char) 12643), new CharacterRange((char) 8200, (char) 9900)});




            _font = fontBakeResult.CreateSpriteFont(GraphicsDevice);
            _locationfont = fontBakeResult.CreateSpriteFont(GraphicsDevice);

            gridtexture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            gridtexture.SetData(new[] { Color.White });

            isomtexture = Texture2D.FromFile(GraphicsDevice, AppDomain.CurrentDomain.BaseDirectory + "\\Data\\Image\\isomtexture.png");

            tileSet = new TileSet();


            if (Global.Setting.Vals[Global.Setting.Settings.Program_GRPLoad] == "true")
            {
                GrpLoad();
            }
        }




        public TileSet tileSet;
        private void GrpLoad()
        {
            tileSet.TextureLoad(this);

            GRPDATA = new Dictionary<Control.MapEditor.DrawType, GRP[]>();

            foreach (Control.MapEditor.DrawType drawType in Enum.GetValues(typeof(Control.MapEditor.DrawType)))
            {
                GRP[] gRPs = new GRP[999];

                for (int i = 0; i < 999; i++)
                {
                    gRPs[i] = new GRP();
                }
                GRPDATA.Add(drawType, gRPs);
            }

            byte[] grpexist = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + @"\CascData\grplist");


            grpdic = new Dictionary<int, int>();
            for (int i = 0; i < 999; i++)
            {
                int grpindex = (int)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.images, "GRP File", i).Data;


                if (grpexist[i] == 1)
                {
                    if (!grpdic.ContainsKey(grpindex))
                    {
                        grpdic.Add(grpindex, i);
                    }
                }
            }
            grpdic.Add(868, 920);
            grpdic.Add(620, 643);
        }





        public Texture2D LoadFromFile(string fname)
        {
            Texture2D texture2D = null;


            FileStream fileStream = new FileStream(fname, FileMode.Open);

            texture2D = Texture2D.FromStream(GraphicsDevice, fileStream);

            fileStream.Close();
            fileStream.Dispose();


            //using (MemoryStream memoryStream = new MemoryStream(File.ReadAllBytes(fname)))
            //{
            //    texture2D = Texture2D.FromStream(GraphicsDevice, memoryStream);
            //}



            return texture2D;
        }

        public Vector2 MouseOuter = new Vector2();
        public Vector2 MousePos;
        public Vector2 MouseMapPos;
        public Vector2 MouseTilePos;
        private int LastScroll;

        private bool refresh = false;
        protected override void Update(GameTime time)
        {
            // every update we can now query the keyboard & mouse for our WpfGame
            var mouseState = _mouse.GetState();
            var keyboardState = _keyboard.GetState();

            if (mapeditor == null)
            {
                return;
            }


            MouseOuter = mapeditor.GetOuterMouse();
            //MousePos = mouseState.Position.ToVector2();
            MousePos = MouseOuter;
            MouseMapPos = mapeditor.PosScreenToMap(MousePos);
            MouseTilePos = new Vector2((float)(Math.Floor(MouseMapPos.X / 32)), (float)(Math.Floor(MouseMapPos.Y / 32)));

            if (mouseState.ScrollWheelValue != LastScroll)
            {
                if ((LastScroll - mouseState.ScrollWheelValue) > 0)
                {
                    if (GlobalVariable.key_LeftShiftDown)
                    {
                        if(mapeditor.mapDataBinding.BRUSHX > 1)
                        {
                            mapeditor.mapDataBinding.BRUSHX -= 1;
                        }

                        if (mapeditor.mapDataBinding.BRUSHY > 1)
                        {
                            mapeditor.mapDataBinding.BRUSHY -= 1;
                        }
                    }
                    else
                    {
                        mapeditor.ScaleDown(MousePos);
                    }
                }
                else
                {
                    if (GlobalVariable.key_LeftShiftDown)
                    {
                        mapeditor.mapDataBinding.BRUSHX += 1;
                        mapeditor.mapDataBinding.BRUSHY += 1;
                    }
                    else
                    {
                        mapeditor.ScaleUp(MousePos);
                    }
                }
                LastScroll = mouseState.ScrollWheelValue;
            }

            //screenwidth = (float)this.ActualWidth - ToolBaStreachValue;
            //screenheight = (float)this.ActualHeight;



            MouseEvent(mouseState);
            KeyboardEvent(keyboardState);




            //=========================팔레트 처리는 여기서==================================
            switch (mapeditor.PalleteLayer)
            {
                case Control.MapEditor.Layer.Tile:
                    TilePaint();
                    break;
                case Control.MapEditor.Layer.Doodad:
                    DoodadPaint();
                    break;
                case Control.MapEditor.Layer.Unit:
                    UnitPaint();
                    break;
                case Control.MapEditor.Layer.Sprite:
                    SpritePaint();
                    break;
                case Control.MapEditor.Layer.FogOfWar:
                    FogofWarPaint();
                    break;
            }
            //===============================================================================



            Vector2 MapMin = mapeditor.PosMapToScreen(new Vector2(0, 0));
            Vector2 MapMax = mapeditor.PosMapToScreen(new Vector2(mapeditor.mapdata.WIDTH, mapeditor.mapdata.HEIGHT) * 32);
            Vector2 MapSize = MapMax - MapMin;




            if ((MapSize.X < screenwidth) & (MapSize.Y < screenheight))
            {
                //화면이 충분히 작을 경우
                mapeditor.opt_xpos = -(int)((screenwidth - (int)MapSize.X) / 2 / mapeditor.opt_scalepercent);
                mapeditor.opt_ypos = -(int)((screenheight - (int)MapSize.Y) / 2 / mapeditor.opt_scalepercent);
            }


            if (mapeditor.Scenario.CheckWindowPos(new System.Windows.Point(MousePos.X + mapeditor.LeftExpander.ActualWidth, MousePos.Y)))
            {
                this.IsEnabled = false;
            }

            if (mapeditor.IsRightToolBarOpen())
            {
                if (MousePos.X > screenwidth)
                {
                    if (this.IsFocused)
                    {
                        this.IsEnabled = false;
                        //mapeditor.Dispatcher.Invoke(new Action(() => {
                        //    System.Threading.Thread.Sleep(100);
                        //    this.mapeditor.Focus();
                        //    System.Threading.Thread.Sleep(100);
                        //    this.IsEnabled = true;
                        //}), System.Windows.Threading.DispatcherPriority.Normal);
                    }
                }
            }

            if (mapeditor.IsLeftToolBarOpen())
            {
                if (MousePos.X < LeftToolBarStreachValue)
                {
                    if (this.IsFocused)
                    {
                        this.IsEnabled = false;
                        //mapeditor.Dispatcher.Invoke(new Action(() => {
                        //    System.Threading.Thread.Sleep(100);
                        //    this.mapeditor.Focus();
                        //    System.Threading.Thread.Sleep(100);
                        //    this.IsEnabled = true;
                        //}), System.Windows.Threading.DispatcherPriority.Normal);
                    }
                }
            }

            if (mapeditor.IsBottomToolBarOpen())
            {
                if (MousePos.Y > screenheight - BottomToolBarStreachValue)
                {
                    if (this.IsFocused)
                    {
                        this.IsEnabled = false;
                        //mapeditor.Dispatcher.Invoke(new Action(() => {
                        //    System.Threading.Thread.Sleep(100);
                        //    this.mapeditor.Focus();
                        //    System.Threading.Thread.Sleep(100);
                        //    this.IsEnabled = true;
                        //}), System.Windows.Threading.DispatcherPriority.Normal);
                    }
                }
            }

            
            if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Y))
            {
                mapeditor.Dispatcher.Invoke(new Action(() =>
                {
                    mapeditor.Tile_All_Pallet.Focus();
                }), System.Windows.Threading.DispatcherPriority.Normal);
            }

            //마우스 들어왔을 경우
            if (MousePos.X <= screenwidth && MousePos.Y > 0 && MousePos.X > LeftToolBarStreachValue
                && !mapeditor.GridCB.IsDropDownOpen && !mapeditor.LayerCB.IsDropDownOpen && !mapeditor.visibilityPopupbox.IsPopupOpen
                && !mapeditor.Scenario.IsHaveFocusWindow()
                )
            {
                if (this.IsFocused)
                {
                    mapeditor.EmptyButton.Focus();
                }
            }


            _timeElapsed += time.ElapsedGameTime;
            if (_timeElapsed >= TimeSpan.FromSeconds(1))
            {
                _timeElapsed -= TimeSpan.FromSeconds(1);
                _frames = _liveFrames;
                _liveFrames = 0;
            }
        }
        private int _frames;
        private int _liveFrames;
        private TimeSpan _timeElapsed;
        private TimeSpan _LastDrawTime;



        float screenwidth;
        float screenheight;


        double drawtimer;


        //float VarFrame = 60;

        int MaxFrame = 60;
        double updateTick;
        double drawTick = 0;

        //private List<scaleframe> scals = new List<scaleframe>();
        //private class scaleframe
        //{
        //    public int scale;
        //    public int frame;

        //    public scaleframe(int scale, int frame)
        //    {
        //        this.scale = scale;
        //        this.frame = frame;
        //    }
        //}


        double period = 0.05;
        double startline = 0.5;

        KalmanFilter framefilter = new KalmanFilter(1, 1, 0.125, 1, 0.1, 0);
        protected override void Draw(GameTime time)
        {
            if (mapeditor == null)
            {
                return;
            }
            if (!mapeditor.IsLoad)
            {
                return;
            }

            int.TryParse(Global.Setting.Vals[Global.Setting.Settings.Render_MaxFrame], out MaxFrame);
            if (MaxFrame == 0)
            {
                MaxFrame = 60;
            }

            ////현재 프레임 기록
            double scale = mapeditor.opt_scalepercent;


            //비율 0.05


            double minTick = 1f / MaxFrame * 10000000;
            //프레임이 원하는 만큼 나오지 않을 경우
            if (Global.Setting.Vals[Global.Setting.Settings.Render_UseVFR] == "true")
            {
                double countedFrame = MaxFrame;// scaleframe[scale] * 0.1);// scaleframe[scale] - 5);

                if (scale <= startline)
                {
                    countedFrame = MaxFrame * ((scale - period) / startline - period);
                }



                minTick = 1f / countedFrame * 10000000;



                //TimeSpan gab = time.TotalGameTime - _LastDrawTime;
                //_LastDrawTime = time.TotalGameTime;

                //drawTick = framefilter.Output(gab.Ticks);

                //////_frames == 현재 프레임
                ////if(MaxFrame > _frames + 4)
                ////{
                ////    //프레임 드랍 발생

                ////    if (VarFrame > _frames + 4)
                ////    {
                ////        //가변 프레임에서도 드랍발생
                ////        VarFrame = _frames - 5;
                ////    }
                ////    else
                ////    {
                ////        //가변 프레임에서는 멀쩡
                ////        if(VarFrame < 4)
                ////        {
                ////            VarFrame = 4;
                ////        }
                ////        VarFrame += 1;
                ////    }


                ////    updateTime = 1f / VarFrame;
                ////}
                ////else
                ////{
                ////    //발생안함.
                ////    updateTime = 1f / MaxFrame;
                ////}


                //if(updateTick < (drawTick * 2))
                //{
                //    //프레임 드랍
                //    if(drawTick <= 167000)
                //    {
                //        if (droptimer > 400)
                //        {
                //            updateTick *= 0.995;//;
                //        }
                //        else
                //        {
                //            droptimer += 1;
                //        }

                //    }
                //    else
                //    {
                //        droptimer = 0;
                //        updateTick = drawTick * 2;//10 ms추가;
                //    }
                //}
                //else
                //{
                //    //프레임 정상화
                //    if(drawTick * 3 < updateTick)
                //    {
                //        updateTick = drawTick * 3;
                //    }
                //    updateTick *= 0.995;//;
                //}

                //if (updateTick < minTick)
                //{
                //    updateTick = minTick;
                //}

                //updateTick = drawTick + 100000;//1f / MaxFrame;



                updateTick = minTick;
            }
            else
            {
                updateTick = minTick;
            }


            drawtimer += (float)time.ElapsedGameTime.Ticks;
            if (drawtimer >= updateTick)
            {
                drawtimer -= updateTick;
                //drawtimer = 0;
            }
            else
            {
                if (MaxFrame != 0)
                {
                    return;
                }
            }


            RightToolBarStreachValue = (int)(mapeditor.GetRightToolBarWidth());
            LeftToolBarStreachValue = (int)(mapeditor.GetLeftToolBarWidth());
            BottomToolBarStreachValue = (int)(mapeditor.GetBottomToolBarWidth());


            screenwidth = (float)this.ActualWidth - RightToolBarStreachValue;
            screenheight = (float)this.ActualHeight;




            bool IsDrawGrp = (Global.Setting.Vals[Global.Setting.Settings.Program_GRPLoad] == "true");
            GraphicsDevice.Clear(Color.DimGray);

            ImageList.Clear();

            RenderTile(IsDrawGrp);
            GridDraw();
            if (mapeditor.view_Doodad)
            {
                RenderDoodad(IsDrawGrp);
            }

            if (IsDrawGrp)
            {
                if (mapeditor.view_Unit)
                {
                    RenderUnit(IsDrawGrp);
                }

                if (mapeditor.view_Sprite)
                {
                    RenderSprite(IsDrawGrp);
                }


                DrawImageList(ImageList);




                DrawPalleteCursor();
            }

            RenderTileOverlay(IsDrawGrp);
            if (mapeditor.PalleteLayer == Control.MapEditor.Layer.Location | mapeditor.view_Location)
            {
                RenderLocation();
            }
            if (mapeditor.PalleteLayer == Control.MapEditor.Layer.FogOfWar)
            {
                RenderFogofWar();
            }
            DrawConnect();


            DrawPallet(IsDrawGrp);


            _liveFrames++;
            //TODO 시스템드로우
            if (mapeditor.opt_sysdraw)
                SystemDraw();

            base.Draw(time);
        }




        private void SystemDraw()
        {
            _spriteBatch.Begin();
            //_spriteBatch.DrawString(_font, mapeditor.mapdata.FilePath, new Vector2(5), Color.White);
            //string status = "화면 좌표(" + mapeditor.opt_xpos.ToString() + "," + mapeditor.opt_ypos.ToString() + ")" + " 마우스좌표(" + MousePos.X.ToString() + "," + MousePos.Y.ToString() + ")" +
            //    "T(" + mapeditor.PosScreenToMap(MousePos).ToString() + ")";
            string status = "FPS:" + _frames + " 드로우타임 : " + Math.Floor(drawTick / 10000) + " 업데이트타임 : " + Math.Floor(updateTick / 10000) + "\n";

            status += mapeditor.mapdata.FilePath + "\n";
            status += mapeditor.mapdata.TILETYPE.ToString() + ":";
            status += mapeditor.mapdata.WIDTH + "," + mapeditor.mapdata.HEIGHT + "\n";




            Vector2 MapMouse = mapeditor.PosScreenToMap(MousePos);
            status += "(" + (int)MapMouse.X + "," + (int)MapMouse.X + ")" + "(" + (int)(MapMouse.X / 32) + "," + (int)(MapMouse.Y / 32) + ")";


            IInputElement focusedElement = FocusManager.GetFocusedElement(mapeditor);
            if (focusedElement != null)
            {
                status += "\n" + focusedElement.ToString();
            }


            //_spriteBatch.Draw(HD_GRP[3], new Vector2(20, 20), new Rectangle(0, 0, 300, 300), Color.White, 0.5f, Vector2.Zero, 0.5f, SpriteEffects.FlipVertically, 0);
            //_spriteBatch.DrawString(_font, mapeditor.opt_xpos.ToString() + "," + mapeditor.opt_ypos.ToString(), new Vector2(5, 30), Color.White);
            _spriteBatch.DrawString(_font, status, new Vector2(5), Color.White);
            _spriteBatch.End();
        }
    }
}
