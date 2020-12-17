using Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        private List<Keys> DownKeys;


        public void KeyboardEvent(KeyboardState keyboardState)
        {
            int index = 0;
            for (int i = 0; i < DownKeys.Count; i++)
            {
                Keys keys = DownKeys[index];
                if (keyboardState.IsKeyUp(keys))
                {
                    if (DownKeys.Contains(keys))
                    {
                        DownKeys.Remove(keys);
                        continue;
                    }
                }
                index++;
            }

          

            if (EnterKey(keyboardState, Keys.F5))
            {
                mapeditor.NextgrpType();
            }



            if (EnterKey(keyboardState, Keys.F1))
            {
                mapeditor.SetGrpType(Control.MapEditor.DrawType.SD);
            }

            if (EnterKey(keyboardState, Keys.F2))
            {
                mapeditor.SetGrpType(Control.MapEditor.DrawType.HD);
            }

            if (EnterKey(keyboardState, Keys.F3))
            {
                mapeditor.SetGrpType(Control.MapEditor.DrawType.CB);
            }

        }


        private bool EnterKey(KeyboardState keyboardState, Keys keys)
        {
            if (keyboardState.IsKeyDown(keys))
            {
                if (!DownKeys.Contains(keys))
                {
                    DownKeys.Add(keys);
                    return true;
                }
            }
            return false;
        }

    }
}
