using Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using SpriteFontPlus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using UseMapEditor.FileData;
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


        private IGraphicsDeviceService _graphicsDeviceManager;
        private WpfKeyboard _keyboard;
        private WpfMouse _mouse;

        private SpriteBatch _spriteBatch;
        private SpriteBatch _colorBatch;
        private SpriteFont _font;

        BlendState ColorBlend;

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
            _mouse = new WpfMouse(this);


            //Components.Add(new FpsComponent(this));

            // must be called after the WpfGraphicsDeviceService instance was created
            base.Initialize();
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _colorBatch = new SpriteBatch(GraphicsDevice);

            ColorBlend = new BlendState();
            ColorBlend.ColorBlendFunction = BlendFunction.Add;
            ColorBlend.ColorSourceBlend = Blend.DestinationColor;
            ColorBlend.ColorDestinationBlend = Blend.InverseSourceAlpha;

            DownKeys = new List<Microsoft.Xna.Framework.Input.Keys>();

            BinaryReader br = new BinaryReader(new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\Data\\SDGRPSIZE", FileMode.Open));

            SDGRPSIZE = new Vector2[999];
            for (int i = 0; i < 999; i++)
            {
                SDGRPSIZE[i] = new Vector2(br.ReadUInt32(), br.ReadUInt32());
            }

            br.Close();
        }

        private Texture2D gridtexture;
        protected override void LoadContent()
        {
            base.LoadContent();


            CharacterRange Hangle = new CharacterRange((char)44032, (char)55203);
            // content loading now possible
            var fontBakeResult = TtfFontBaker.Bake(File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "\\Font\\NanumSquareRoundB.ttf"), 25, 4096, 4096, new[]{
                 CharacterRange.BasicLatin, Hangle, new CharacterRange((char) 12593, (char) 12643), new CharacterRange((char) 8200, (char) 9900)});

            CharacterRange characterRange = CharacterRange.BasicLatin;


            _font = fontBakeResult.CreateSpriteFont(GraphicsDevice);


            gridtexture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            gridtexture.SetData(new[] {Color.White});



            tileSet = new TileSet();


            if (Global.Setting.Vals[Global.Setting.Settings.Program_GRPLoad] == "true")
            {
                GrpLoad();
            }
        }




        public TileSet tileSet;
        private void GrpLoad()
        {
            //texture = Content.Load<Texture2D>("Test");


            //byte[] textureData = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + @"\Content\main_000-000-diffuse.dds");

            //int dxtHeaderOffset = 0x80;
            //texture = new Texture2D(GraphicsDevice, 1024, 888, false, SurfaceFormat.Dxt5);
            //texture.SetData(textureData, dxtHeaderOffset, textureData.Length - dxtHeaderOffset);


            //texture = Texture2D.FromFile(GraphicsDevice, "Content\\AnyConv.com__main_000-000-diffuse.png");


            minimap = new Texture2D(GraphicsDevice, 256, 256);
            minimapUnit = new Texture2D(GraphicsDevice, 256, 256);

            //Anim anim = new Anim(this);

            //FileData.TileSet.ReadTileSet(this);

            tileSet.TextureLoad(this);



            //SD_GRP = new List<Texture2D>();
            //SD_Color = new List<Texture2D>();
            //HD_GRP = new List<Texture2D>();
            //HD_Color = new List<Texture2D>();
            //CB_GRP = new List<Texture2D>();
            //CB_Color = new List<Texture2D>();



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
        }





        private Texture2D LoadFromFile(string fname)
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


        public Vector2 MousePos;
        private int LastScroll;
        protected override void Update(GameTime time)
        {
            // every update we can now query the keyboard & mouse for our WpfGame
            var mouseState = _mouse.GetState();
            var keyboardState = _keyboard.GetState();

            if (mapeditor == null)
            {
                return;
            }


            MousePos = mouseState.Position.ToVector2();


            int step = 10;
            if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W))
            {
                mapeditor.opt_ypos -= (int)Math.Ceiling(step / mapeditor.opt_scalepercent);
            }
            if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.S))
            {
                mapeditor.opt_ypos += (int)Math.Ceiling(step / mapeditor.opt_scalepercent);
            }
            if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A))
            {
                mapeditor.opt_xpos -= (int)Math.Ceiling(step / mapeditor.opt_scalepercent);
            }
            if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D))
            {
                mapeditor.opt_xpos += (int)Math.Ceiling(step / mapeditor.opt_scalepercent);
            }


            if (mouseState.ScrollWheelValue != LastScroll)
            {
                if((LastScroll - mouseState.ScrollWheelValue) > 0)
                {
                    mapeditor.ScaleDown(MousePos);
                }
                else
                {
                    mapeditor.ScaleUp(MousePos);
                }
                LastScroll = mouseState.ScrollWheelValue;
            }

            KeyboardEvent(keyboardState);
            MiniMapClick(mouseState);

            screenwidth = (float)this.ActualWidth;
            screenheight = (float)this.ActualHeight;
            Vector2 MapMin = mapeditor.PosMapToScreen(new Vector2(0, 0));
            Vector2 MapMax = mapeditor.PosMapToScreen(new Vector2(mapeditor.mapdata.WIDTH, mapeditor.mapdata.HEIGHT) * 32);
            Vector2 MapSize = MapMax - MapMin;

            if ((MapSize.X < screenwidth) & (MapSize.Y < screenheight))
            {
                //화면이 충분히 작을 경우
                mapeditor.opt_xpos = -(int)((screenwidth - (int)MapSize.X) / 2 / mapeditor.opt_scalepercent);
                mapeditor.opt_ypos = -(int)((screenheight - (int)MapSize.Y) / 2 / mapeditor.opt_scalepercent);
            }

            if (mapeditor.IsToolBarOpen())
            {
                if (MousePos.X > (screenwidth - CloseTimer))
                {
                    this.IsEnabled = false;
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



        float screenwidth;
        float screenheight;


        float drawtimer;



        float VarFrame = 0;
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


            int MaxFrame = 60;
            int.TryParse(Global.Setting.Vals[Global.Setting.Settings.Render_MaxFrame], out MaxFrame);



            float updateTime;
            //프레임이 원하는 만큼 나오지 않을 경우
            if (Global.Setting.Vals[Global.Setting.Settings.Render_UseVFR] == "true")
            {
                float framediffer = MaxFrame - _frames;
                if(framediffer > 5)
                {
                    VarFrame = (float)(100 * mapeditor.opt_scalepercent);
                }
                else
                {
                    VarFrame = MaxFrame;
                }



                updateTime = 1f / VarFrame;
            }
            else
            {
                updateTime = 1f / MaxFrame;
            }


            drawtimer += (float)time.ElapsedGameTime.TotalSeconds;
            if (drawtimer >= updateTime)
            {
                drawtimer -= updateTime;
            }
            else
            {
                return;
            }
            screenwidth = (float)this.ActualWidth;
            screenheight = (float)this.ActualHeight;



            bool IsDrawGrp = (Global.Setting.Vals[Global.Setting.Settings.Program_GRPLoad] == "true");
            GraphicsDevice.Clear(Color.DimGray);

            ImageList.Clear();


            RenderTile(IsDrawGrp);
            RenderDoodad(IsDrawGrp);
            GridDraw();

            if (IsDrawGrp)
            {
                RenderUnit(IsDrawGrp);
                RenderSprite(IsDrawGrp);


                //모아진 유닛들을 그린다
                ImageList.Sort((x1, x2) => x1.drawsort.CompareTo(x2.drawsort));
                float scale = 1;
                float grpscale = 1;
                switch (mapeditor.opt_drawType)
                {
                    case Control.MapEditor.DrawType.SD:
                        scale = (float)mapeditor.opt_scalepercent;
                        grpscale = 1;

                        break;
                    case Control.MapEditor.DrawType.HD:
                    case Control.MapEditor.DrawType.CB:
                        scale = (float)mapeditor.opt_scalepercent;
                        grpscale = 2;
                        break;
                }
                for (int i = 0; i < ImageList.Count; i++)
                {
                    DrawImage(mapeditor.opt_drawType, ImageList[i], scale, grpscale);
                }
            }
            DrawPallet(IsDrawGrp);



            SystemDraw();

            base.Draw(time);
        }

        private void SystemDraw()
        {
            _liveFrames++;
            _spriteBatch.Begin();
            //_spriteBatch.DrawString(_font, mapeditor.mapdata.FilePath, new Vector2(5), Color.White);
            //string status = "화면 좌표(" + mapeditor.opt_xpos.ToString() + "," + mapeditor.opt_ypos.ToString() + ")" + " 마우스좌표(" + MousePos.X.ToString() + "," + MousePos.Y.ToString() + ")" +
            //    "T(" + mapeditor.PosScreenToMap(MousePos).ToString() + ")";
            string status = "FPS:" + _frames + "\n";

            status += mapeditor.mapdata.FilePath + "\n";
            status += mapeditor.mapdata.TILETYPE.ToString() + ":";
            status += mapeditor.mapdata.WIDTH + "," + mapeditor.mapdata.HEIGHT + "\n";


            Vector2 MapMouse = mapeditor.PosScreenToMap(MousePos);
            status += "(" + (int)MapMouse.X + "," + (int)MapMouse.X + ")" + "(" + (int)(MapMouse.X / 32) + "," + (int)(MapMouse.Y / 32) + ")";



            //_spriteBatch.Draw(HD_GRP[3], new Vector2(20, 20), new Rectangle(0, 0, 300, 300), Color.White, 0.5f, Vector2.Zero, 0.5f, SpriteEffects.FlipVertically, 0);
            //_spriteBatch.DrawString(_font, mapeditor.opt_xpos.ToString() + "," + mapeditor.opt_ypos.ToString(), new Vector2(5, 30), Color.White);
            _spriteBatch.DrawString(_font, status, new Vector2(5), Color.White);
            _spriteBatch.End();
        }
    }
}
