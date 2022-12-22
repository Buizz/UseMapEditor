using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseMapEditor.Control;
using static Data.Map.MapData;

namespace UseMapEditor.Task.Events
{
    public class SpriteEvent : TaskEvent
    {
        MapEditor mapEditor;
        private CTHG2 cTHG2;
        private bool IsCreate;
        private bool IsCreateAction;

        public SpriteEvent(MapEditor mapEditor, CTHG2 cTHG2, bool IsCreate)
        {
            this.mapEditor = mapEditor;
            this.cTHG2 = cTHG2;

            this.IsCreate = IsCreate;
            IsCreateAction = true;
        }


        public override void Redo()
        {
            if (IsCreateAction)
            {
                if (IsCreate)
                {
                    mapEditor.mapdata.THG2.Add(cTHG2);
                }
                else
                {
                    mapEditor.mapdata.THG2.Remove(cTHG2);
                }
            }
        }

        public override void Undo()
        {
            if (IsCreateAction)
            {
                if (IsCreate)
                {
                    mapEditor.mapdata.THG2.Remove(cTHG2);
                }
                else
                {
                    mapEditor.mapdata.THG2.Add(cTHG2);
                }
            }
        }

        public override void Complete()
        {
        }
    }
}
