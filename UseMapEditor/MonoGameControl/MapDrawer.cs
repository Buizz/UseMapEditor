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
    public class MapDrawer : WpfGame
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

        protected override void LoadContent()
        {
            base.LoadContent();

            // content loading now possible
            var fontBakeResult = TtfFontBaker.Bake(File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "\\Font\\NanumSquareRoundB.ttf"), 25, 1024, 1024, new[]{CharacterRange.BasicLatin,
        new CharacterRange((char) 44032, (char) 55203)});

            _font = fontBakeResult.CreateSpriteFont(GraphicsDevice);

            GrpLoad();
        }



        public List<Texture2D> SD_GRP;
        public List<Texture2D> SD_Color;
        public List<Texture2D> HD_GRP;
        public List<Texture2D> HD_Color;
        public List<Texture2D> CB_GRP;
        public List<Texture2D> CB_Color;




        private void GrpLoad()
        {
            //texture = Content.Load<Texture2D>("Test");


            //byte[] textureData = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + @"\Content\main_000-000-diffuse.dds");

            //int dxtHeaderOffset = 0x80;
            //texture = new Texture2D(GraphicsDevice, 1024, 888, false, SurfaceFormat.Dxt5);
            //texture.SetData(textureData, dxtHeaderOffset, textureData.Length - dxtHeaderOffset);


            texture = Texture2D.FromFile(GraphicsDevice, "Content\\AnyConv.com__main_000-000-diffuse.png");



            Anim anim = new Anim(this);



            SD_GRP = new List<Texture2D>();
            SD_Color = new List<Texture2D>();
            HD_GRP = new List<Texture2D>();
            HD_Color = new List<Texture2D>();
            CB_GRP = new List<Texture2D>();
            CB_Color = new List<Texture2D>();


            anim.ReadUnitData();



            //anim.ReadAnim(@"CascData\HD\anim\main_000.anim");
        }




        protected override void Update(GameTime time)
        {
            // every update we can now query the keyboard & mouse for our WpfGame
            var mouseState = _mouse.GetState();
            var keyboardState = _keyboard.GetState();

            if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A))
            {
                test--;
            }

            if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D))
            {
                test++;
            }
        }


        int test = 0;
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


            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;




            _spriteBatch.Begin();
            _spriteBatch.DrawString(_font, mapeditor.mapdata.FilePath, new Vector2(5), Color.White);


            _spriteBatch.Draw(texture, Vector2.Zero, Color.White);

            //if (SD_GRP[(test/10) % 999] != null)
            {
                //_spriteBatch.Draw(SD_GRP[(test / 10) % 999], Vector2.Zero, Color.White);
                //_spriteBatch.Draw(HD_GRP[(test / 10) % 999], Vector2.Zero, Color.White);
                //_spriteBatch.Draw(CB_GRP[(test / 10) % 999], Vector2.Zero, Color.White);


                
                //_spriteBatch.Draw(HD_GRP[(test / 10) % 999], new Vector2(20, 20), new Rectangle(0, 0, 300, 300), Color.White, 0.5f, Vector2.Zero, 0.5f, SpriteEffects.FlipVertically, 0);
            }
            _spriteBatch.End();


            base.Draw(time);
        }
    }
}
