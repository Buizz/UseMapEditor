﻿using Data;
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

        private bool mouse_IsLeftDrag;

        public void MouseEvent(MouseState mouseState)
        {
            if (!mouse_IsDrag)
            {
                MiniMapClick(mouseState);
                if (IsMiniMapDrag)
                {
                    return;
                }
            }

            if(mouse_IsDrag | mouseState.MiddleButton == ButtonState.Pressed)
            {
                if (MouseOuter.X < 16)
                {
                    mapeditor.ScrollLeft();
                }
                if (MouseOuter.X > screenwidth - 16)
                {
                    mapeditor.ScrollRight();
                }
                if (MouseOuter.Y < 16)
                {
                    mapeditor.ScrollUp();
                }
                if (MouseOuter.Y > screenheight - 16)
                {
                    mapeditor.ScrollDown();
                }
            }


            if (mouseState.RightButton == ButtonState.Pressed)
            {
                if (!mouse_RightDown)
                {
                    RightClickStart();
                }

                Vector2 differ = mouse_DragMapStart - mapeditor.PosScreenToMap(MousePos);
                if ((Math.Abs(differ.X) > 0) | (Math.Abs(differ.Y) > 0))
                {
                    if (!mouse_IsDrag)
                    {
                        mouse_IsLeftDrag = false;
                        DragStart();
                    }
                }
            }
            if (mouseState.RightButton == ButtonState.Released | !mapeditor.IsRightMouseDown)
            {
                if (mouse_RightDown)
                {
                    mouse_DragScreenStart = new Vector2();
                    if (mouse_IsDrag & !mouse_IsLeftDrag)
                    {
                        DragEnd();
                        mouse_IsDrag = false;
                    }
                    else
                    {
                        RightClickEnd();
                    }
                }

                mouse_RightDown = false;
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
                        mouse_IsLeftDrag = true;
                        DragStart();
                    }
                }

            }
            if (mouseState.LeftButton == ButtonState.Released | !mapeditor.IsLeftMouseDown)
            {
                if (mouse_LeftDown)
                {
                    mouse_DragScreenStart = new Vector2();
                    if (mouse_IsDrag & mouse_IsLeftDrag)
                    {
                        DragEnd();
                        mouse_IsDrag = false;
                    }
                    else
                    {
                        LeftClickEnd();
                    }
                }

                mouse_LeftDown = false;
            }
        }

        public void RightClickStart()
        {
            mouse_RightDown = true;
            mouse_DragScreenStart = MousePos;
            mouse_DragMapStart = mapeditor.PosScreenToMap(mouse_DragScreenStart);
        }

        public void RightClickEnd()
        {
            switch (mapeditor.PalleteLayer)
            {
                case Control.MapEditor.Layer.Location:
                    LocationRightMouseClick();
                    break;
                case Control.MapEditor.Layer.Doodad:
                    DoodadRightMouseClick();
                    break;
                case Control.MapEditor.Layer.Unit:
                    UnitRightMouseClick();
                    break;
                case Control.MapEditor.Layer.Sprite:
                    SpriteRightMouseClick();
                    break;
            }
        }


        public void LeftClickStart()
        {
            mouse_LeftDown = true;
            mouse_DragScreenStart = MousePos;
            mouse_DragMapStart = mapeditor.PosScreenToMap(mouse_DragScreenStart);

            switch (mapeditor.PalleteLayer)
            {
                case Control.MapEditor.Layer.Doodad:
                    DoodadTaskStart();
                    break;
                case Control.MapEditor.Layer.Unit:
                    UnitTaskStart();
                    break;
                case Control.MapEditor.Layer.Sprite:
                    SpriteTaskStart();
                    break;
            }
            
        }

        public void LeftClickEnd()
        {
            switch (mapeditor.PalleteLayer)
            {
                case Control.MapEditor.Layer.Location:
                    LocationLeftMouseClick();
                    break;
                case Control.MapEditor.Layer.Doodad:
                    DoodadClickEnd();
                    break;
                case Control.MapEditor.Layer.Unit:
                    UnitClickEnd();
                    break;
                case Control.MapEditor.Layer.Sprite:
                    SpriteClickEnd();
                    break;
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
            switch (mapeditor.PalleteLayer)
            {
                case Control.MapEditor.Layer.Location:
                    LocationDragEnd();
                    break;
                case Control.MapEditor.Layer.Doodad:
                    DoodadDragEnd();
                    break;
                case Control.MapEditor.Layer.Unit:
                    UnitDragEnd();
                    break;
                case Control.MapEditor.Layer.Sprite:
                    SpriteDragEnd();
                    break;
            }

        }

    }
}
