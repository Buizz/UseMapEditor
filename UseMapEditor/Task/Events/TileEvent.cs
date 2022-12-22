﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseMapEditor.Control;
using static Data.Map.MapData;

namespace UseMapEditor.Task.Events
{
    public class TileEvent : TaskEvent
    {
        MapEditor mapEditor;
        private ushort newMTXM;
        private ushort oldMTXM;
        private int X;
        private int Y;


        public TileEvent(MapEditor mapEditor, ushort newMTXM, ushort oldMTXM, int X, int Y)
        {
            this.mapEditor = mapEditor;
            this.newMTXM = newMTXM;
            this.oldMTXM = oldMTXM;
            this.X = X;
            this.Y = Y;
        }



        public override void Redo()
        {
            mapEditor.mapdata.TILEChange(X, Y, newMTXM);
        }

        public override void Undo()
        {
            mapEditor.mapdata.TILEChange(X, Y, oldMTXM);
        }

        public override void Complete()
        {
            mapEditor.mapdata.TILEChangeComplete();
        }
    }
}