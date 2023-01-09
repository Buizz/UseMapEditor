using Microsoft.Office.Interop.Excel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using WpfTest.Components;

namespace MonoGame.Forms.Tests
{
    public class MyGame : WpfGame
    {
        private IGraphicsDeviceService _graphicsDeviceManager;
        private WpfKeyboard _keyboard;
        private WpfMouse _mouse;

        private SpriteBatch _spriteBatch;
        private SpriteFont _font;

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

            // content loading now possible
            _font = Content.Load<SpriteFont>("DefaultFont");
            
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime time)
        {
            // every update we can now query the keyboard & mouse for our WpfGame
            var mouseState = _mouse.GetState();
            var keyboardState = _keyboard.GetState();
        }

        protected override void Draw(GameTime time)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.DrawString(_font, $"FPS: {time.TotalGameTime.Milliseconds}", new Vector2(5), Color.White);
            _spriteBatch.End();

            base.Draw(time);
        }
    }
}
