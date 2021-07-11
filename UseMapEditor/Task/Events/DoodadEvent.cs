using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseMapEditor.Control;
using static Data.Map.MapData;

namespace UseMapEditor.Task.Events
{
    public class DoodadEvent : TaskEvent
    {
        MapEditor mapEditor;
        private CDD2 cDD2;
        private bool IsCreate;
        private bool IsCreateAction;

        public DoodadEvent(MapEditor mapEditor, CDD2 cDD2, bool IsCreate)
        {
            this.mapEditor = mapEditor;
            this.cDD2 = cDD2;

            this.IsCreate = IsCreate;
            IsCreateAction = true;
        }


        public override void Redo()
        {
            if (IsCreateAction)
            {
                if (IsCreate)
                {
                    mapEditor.mapdata.DD2ToMTXM(cDD2);
                    mapEditor.mapdata.DD2.Add(cDD2);
                }
                else
                {
                    mapEditor.mapdata.DD2DeleteMTXM(cDD2);
                    mapEditor.mapdata.DD2.Remove(cDD2);
                }
            }
        }

        public override void Undo()
        {
            if (IsCreateAction)
            {
                if (IsCreate)
                {
                    mapEditor.mapdata.DD2DeleteMTXM(cDD2);
                    mapEditor.mapdata.DD2.Remove(cDD2);
                }
                else
                {
                    mapEditor.mapdata.DD2ToMTXM(cDD2);
                    mapEditor.mapdata.DD2.Add(cDD2);
                }
            }
        }
    }
}
