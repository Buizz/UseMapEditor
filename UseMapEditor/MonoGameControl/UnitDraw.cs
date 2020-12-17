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
        private void DrawUnit(bool IsDrawGrp)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(GetImageTexture(Control.MapEditor.DrawType.SD,0).MainGRP, new Vector2(30, 30), new Rectangle(0, 0, 30, 30), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            _spriteBatch.End();

            
        }
    }
}
