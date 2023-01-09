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
using UseMapEditor.Control;
using UseMapEditor.FileData;
using WpfTest.Components;

namespace UseMapEditor.MonoGameControl
{
    public partial class MapDrawer : WpfGame
    {
        private List<Keys> DownKeys;
        private List<Keys> UpKeys;


        private bool key_LeftCtrl;
        private bool key_LeftShiftDown;
        private bool key_QDown;


        //private bool key_WDown;
        //private bool key_ADown;
        //private bool key_SDown;
        //private bool key_DDown;

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

            index = 0;
            for (int i = 0; i < UpKeys.Count; i++)
            {
                Keys keys = UpKeys[index];
                if (keyboardState.IsKeyDown(keys))
                {
                    if (UpKeys.Contains(keys))
                    {
                        UpKeys.Remove(keys);
                        continue;
                    }
                }
                index++;
            }



            key_LeftCtrl = keyboardState.IsKeyDown(Keys.LeftControl);
            key_LeftShiftDown = keyboardState.IsKeyDown(Keys.LeftShift);
            key_QDown = keyboardState.IsKeyDown(Keys.Q);





            if (EnterKey(keyboardState, Keys.Delete))
            {
                if (mapeditor.PalleteLayer == Control.MapEditor.Layer.Location)
                {
                    LocationDelete();
                }
            }


            //if (EnterKey(keyboardState, Keys.F5))
            //{
            //    mapeditor.NextgrpType();
            //}



            //if (EnterKey(keyboardState, Keys.F1))
            //{
            //    mapeditor.SetGrpType(Control.MapEditor.DrawType.SD);
            //}

            //if (EnterKey(keyboardState, Keys.F2))
            //{
            //    mapeditor.SetGrpType(Control.MapEditor.DrawType.HD);
            //}

            //if (EnterKey(keyboardState, Keys.F3))
            //{
            //    mapeditor.SetGrpType(Control.MapEditor.DrawType.CB);
            //}


            switch (mapeditor.PalleteLayer)
            {
                case Control.MapEditor.Layer.Unit:
                    if (EnterKey(keyboardState, Keys.D1))
                    {
                        mapeditor.mapDataBinding.UNIT_GRIDFIX = !mapeditor.mapDataBinding.UNIT_GRIDFIX;
                    }
                    if (EnterKey(keyboardState, Keys.D2))
                    {
                        mapeditor.mapDataBinding.UNIT_BUILDINGFIX = !mapeditor.mapDataBinding.UNIT_BUILDINGFIX;
                    }
                    if (EnterKey(keyboardState, Keys.D3))
                    {
                        mapeditor.mapDataBinding.UNIT_ALLOWSTACK = !mapeditor.mapDataBinding.UNIT_ALLOWSTACK;
                    }
                    if (EnterKey(keyboardState, Keys.D4))
                    {
                        mapeditor.mapDataBinding.UNIT_COPYTILEPOS = !mapeditor.mapDataBinding.UNIT_COPYTILEPOS;
                    }

                    break;
                case Control.MapEditor.Layer.Sprite:
                    if (EnterKey(keyboardState, Keys.D1))
                    {
                        mapeditor.mapDataBinding.SPRITE_GRIDFIX = !mapeditor.mapDataBinding.SPRITE_GRIDFIX;
                    }
                    if (EnterKey(keyboardState, Keys.D2))
                    {
                        mapeditor.mapDataBinding.SPRITE_COPYTILEPOS = !mapeditor.mapDataBinding.SPRITE_COPYTILEPOS;
                    }
                    break;
                case Control.MapEditor.Layer.Doodad:
                    if (EnterKey(keyboardState, Keys.D1))
                    {
                        mapeditor.mapDataBinding.DOODAD_STACKALLOW = !mapeditor.mapDataBinding.DOODAD_STACKALLOW;
                    }
                    if (EnterKey(keyboardState, Keys.D2))
                    {
                        mapeditor.mapDataBinding.DOODAD_TOTILE = !mapeditor.mapDataBinding.DOODAD_TOTILE;
                    }
                    break;
                case Control.MapEditor.Layer.Tile:
                    if (EnterKey(keyboardState, Keys.D1))
                    {
                        if (mapeditor.mapDataBinding.TILE_PAINTTYPE == MapEditor.TileSetPaintType.CIRCLE)
                        {
                            mapeditor.mapDataBinding.TILE_PAINTTYPE = 0;
                        }
                        else
                        {
                            mapeditor.mapDataBinding.TILE_PAINTTYPE += 1;
                        }
                    }
                    if (EnterKey(keyboardState, Keys.D2))
                    {
                        mapeditor.mapDataBinding.TILE_TRANSPARENTBLACK = !mapeditor.mapDataBinding.TILE_TRANSPARENTBLACK;
                    }
                    break;
            }


            //if (EnterKey(keyboardState, Keys.W))
            //{
            //    key_WDown = true;
            //}
            //if (EnterKey(keyboardState, Keys.A))
            //{
            //    key_ADown = true;
            //}
            //if (EnterKey(keyboardState, Keys.S))
            //{
            //    key_SDown = true;
            //}
            //if (EnterKey(keyboardState, Keys.D))
            //{
            //    key_DDown = true;
            //}

            //if (UpKey(keyboardState, Keys.W))
            //{
            //    key_WDown = false;
            //}
            //if (UpKey(keyboardState, Keys.A))
            //{
            //    key_ADown = false;
            //}
            //if (UpKey(keyboardState, Keys.S))
            //{
            //    key_SDown = false;
            //}
            //if (UpKey(keyboardState, Keys.D))
            //{
            //    key_DDown = false;
            //}


            if (key_LeftCtrl)
            {
                return;
            }


            if (mapeditor.key_WDown)
            {
                mapeditor.ScrollUp();
            }
            if (mapeditor.key_SDown)
            {
                mapeditor.ScrollDown();
            }
            if (mapeditor.key_ADown)
            {
                mapeditor.ScrollLeft();
            }
            if (mapeditor.key_DDown)
            {
                mapeditor.ScrollRight();
            }
        }





        private bool EnterKey(KeyboardState keyboardState, Keys keys)
        {
            if (keyboardState.IsKeyDown(keys))
            {
                if (!DownKeys.Contains(keys))
                {
                    DownKeys.Add(keys);
                    if (key_LeftCtrl)
                    {
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }


        private bool UpKey(KeyboardState keyboardState, Keys keys)
        {
            if (keyboardState.IsKeyUp(keys))
            {
                if (!UpKeys.Contains(keys))
                {
                    UpKeys.Add(keys);
                    if (key_LeftCtrl)
                    {
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
