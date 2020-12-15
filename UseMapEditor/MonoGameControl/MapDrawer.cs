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
        private SpriteFont _font;

        private Texture2D texture;


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
        }

        private Texture2D gridtexture;
        protected override void LoadContent()
        {
            base.LoadContent();


            CharacterRange Hangle = new CharacterRange((char)44032, (char)55203);
            // content loading now possible
            var fontBakeResult = TtfFontBaker.Bake(File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "\\Font\\NanumSquareRoundB.ttf"), 25, 4096, 4096, new[]{
                 CharacterRange.BasicLatin, Hangle, new CharacterRange((char) 12593, (char) 12643), new CharacterRange((char) 8200, (char) 9900)});
            //new CharacterRange((char) 44032, (char) 55203)});

            CharacterRange characterRange = CharacterRange.BasicLatin;


            _font = fontBakeResult.CreateSpriteFont(GraphicsDevice);


            gridtexture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            gridtexture.SetData(new[] {Color.White});
            




            if (Global.Setting.Vals[Global.Setting.Settings.Program_GRPLoad] == "true")
            {
                GrpLoad();
            }
        }



        public List<Texture2D> SD_GRP;
        public List<Texture2D> SD_Color;
        public List<Texture2D> HD_GRP;
        public List<Texture2D> HD_Color;
        public List<Texture2D> CB_GRP;
        public List<Texture2D> CB_Color;




        public Dictionary<FileData.TileSet.TileType, List<Texture2D>> SDTileSet;
        public Dictionary<FileData.TileSet.TileType, List<Texture2D>> HDTileSet;
        public Dictionary<FileData.TileSet.TileType, List<Texture2D>> CBTileSet;


        public TileSet tileSet;
        private void GrpLoad()
        {
            //texture = Content.Load<Texture2D>("Test");


            //byte[] textureData = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + @"\Content\main_000-000-diffuse.dds");

            //int dxtHeaderOffset = 0x80;
            //texture = new Texture2D(GraphicsDevice, 1024, 888, false, SurfaceFormat.Dxt5);
            //texture.SetData(textureData, dxtHeaderOffset, textureData.Length - dxtHeaderOffset);


            texture = Texture2D.FromFile(GraphicsDevice, "Content\\AnyConv.com__main_000-000-diffuse.png");


            minimap = new Texture2D(GraphicsDevice, 128, 128);


            //Anim anim = new Anim(this);

            SDTileSet = new Dictionary<TileSet.TileType, List<Texture2D>>();
            HDTileSet = new Dictionary<TileSet.TileType, List<Texture2D>>();
            CBTileSet = new Dictionary<TileSet.TileType, List<Texture2D>>();
            FileData.TileSet.ReadTileSet(this);

            tileSet = new TileSet();



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



            screenwidth = (float)this.ActualWidth;
            screenheight = (float)this.ActualHeight;
            Vector2 MapMin = mapeditor.PosMapToScreen(new Vector2(0, 0));
            Vector2 MapMax = mapeditor.PosMapToScreen(new Vector2(mapeditor.mapdata.WIDTH, mapeditor.mapdata.HEIGHT) * 32);
            Vector2 MapSize = MapMax - MapMin;

            if ((MapSize.X < screenwidth) & (MapSize.Y < screenheight))
            {
                //화면이 충분히 작을 경우
                mapeditor.opt_xpos = -(int)(((screenwidth - MapSize.X)) / 2 / mapeditor.opt_scalepercent);
                mapeditor.opt_ypos = -(int)(((screenheight - MapSize.Y)) / 2 / mapeditor.opt_scalepercent);
            }

            if (mapeditor.IsToolBarOpen())
            {
                if (MousePos.X > (screenwidth - 256))
                {
                    this.IsEnabled = false;
                }
            }


        }

        float screenwidth;
        float screenheight;
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

            screenwidth = (float)this.ActualWidth;
            screenheight = (float)this.ActualHeight;



            bool IsDrawGrp = (Global.Setting.Vals[Global.Setting.Settings.Program_GRPLoad] == "true");

            GraphicsDevice.Clear(Color.LightGray);



            //_spriteBatch.Draw(HD_GRP[(test / 10) % 999], new Vector2(20, 20), new Rectangle(0, 0, 300, 300), Color.White, 0.5f, Vector2.Zero, 0.5f, SpriteEffects.FlipVertically, 0);


            TileDraw(IsDrawGrp);
            GridDraw();
            DrawUnit();
            //두데드 그리기


            DrawPallet();



            _spriteBatch.Begin();
            _spriteBatch.DrawString(_font, mapeditor.mapdata.FilePath, new Vector2(5), Color.White);
            //string status = "화면 좌표(" + mapeditor.opt_xpos.ToString() + "," + mapeditor.opt_ypos.ToString() + ")" + " 마우스좌표(" + MousePos.X.ToString() + "," + MousePos.Y.ToString() + ")" +
            //    "T(" + mapeditor.PosScreenToMap(MousePos).ToString() + ")";
            string status =  mapeditor.PosScreenToMap(MousePos).ToString() ;


            //_spriteBatch.Draw(HD_GRP[3], new Vector2(20, 20), new Rectangle(0, 0, 300, 300), Color.White, 0.5f, Vector2.Zero, 0.5f, SpriteEffects.FlipVertically, 0);
            //_spriteBatch.DrawString(_font, mapeditor.opt_xpos.ToString() + "," + mapeditor.opt_ypos.ToString(), new Vector2(5, 30), Color.White);
            _spriteBatch.DrawString(_font, status, new Vector2(5, 30), Color.White);
            _spriteBatch.End();

            base.Draw(time);
        }
    }
}
