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


        private Vector2 mouse_DragMapStart;
        private Vector2 mouse_DragScreenStart;

        private Vector2 mouse_DragMapEnd;
        private Vector2 mouse_DragScreenEnd;


        private bool mouse_LeftDown;
        private bool mouse_RightDown;
        private bool mouse_IsDrag;

        public void MouseEvent(MouseState mouseState)
        {

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                if (!mouse_RightDown)
                {
                    RightClickStart();
                }
            }
            else if (mouseState.RightButton == ButtonState.Released)
            {
                if (mouse_RightDown)
                {
                    RightClickEnd();
                    mouse_RightDown = false;
                }
            }



            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (!mouse_LeftDown)
                {
                    LeftClickStart();
                }

                Vector2 differ = mouse_DragMapStart - mapeditor.PosScreenToMap(MousePos);
                if ((Math.Abs(differ.X) > 0) | (Math.Abs(differ.Y) > 0))
                {
                    if (!mouse_IsDrag)
                    {
                        DragStart();
                    }
                }

            }
            else if (mouseState.LeftButton == ButtonState.Released)
            {
                if (mouse_LeftDown)
                {
                    mouse_DragScreenStart = new Vector2();
                    if (mouse_IsDrag)
                    {
                        DragEnd();
                    }
                    else
                    {
                        LeftClickEnd();
                    }
                }

                mouse_IsDrag = false;
                mouse_LeftDown = false;
            }
        }

        public void RightClickStart()
        {
            mouse_RightDown = true;
        }

        public void RightClickEnd()
        {
            if (mapeditor.PalleteLayer == Control.MapEditor.Layer.Location)
            {
                LocationRightMouseClick();
            }
        }


        public void LeftClickStart()
        {
            mouse_LeftDown = true;
            mouse_DragScreenStart = MousePos;
            mouse_DragMapStart = mapeditor.PosScreenToMap(mouse_DragScreenStart);
        }

        public void LeftClickEnd()
        {
            if (mapeditor.PalleteLayer == Control.MapEditor.Layer.Location)
            {
                LocationLeftMouseClick();
            }
        }
        public void DragStart()
        {
            mouse_IsDrag = true;
            //mouse_DragScreenStart = MousePos;
            //mouse_DragMapStart = mapeditor.PosScreenToMap(mouse_DragScreenStart);


            if (mapeditor.PalleteLayer == Control.MapEditor.Layer.Location)
            {
                LocationDragStart();
            }
        }
        public void DragEnd()
        {
            mouse_DragScreenEnd = MousePos;
            mouse_DragMapEnd = mapeditor.PosScreenToMap(mouse_DragScreenEnd);
            if (mapeditor.PalleteLayer == Control.MapEditor.Layer.Location)
            {
                LocationDragEnd();
                //LocationSelect();
            }
        }

    }
}
