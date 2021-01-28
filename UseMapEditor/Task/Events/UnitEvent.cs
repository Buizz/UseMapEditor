using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseMapEditor.Control;
using static Data.Map.MapData;

namespace UseMapEditor.Task.Events
{
    public class UnitEvent : TaskEvent
    {
        MapEditor mapEditor;
        private CUNIT cUNIT;
        private bool IsCreate;
        private bool IsCreateAction;

        public UnitEvent(MapEditor mapEditor, CUNIT cUNIT, bool IsCreate)
        {
            this.mapEditor = mapEditor;
            this.cUNIT = cUNIT;

            this.IsCreate = IsCreate;
            IsCreateAction = true;
        }


        public override void Redo()
        {
            if (IsCreateAction)
            {
                if (IsCreate)
                {
                    mapEditor.mapdata.UNITListAdd(cUNIT);
                }
                else
                {
                    mapEditor.mapdata.UNITListRemove(cUNIT);
                }
            }
        }

        public override void Undo()
        {
            if (IsCreateAction)
            {
                if (IsCreate)
                {
                    mapEditor.mapdata.UNITListRemove(cUNIT);
                }
                else
                {
                    mapEditor.mapdata.UNITListAdd(cUNIT);
                }
            }
        }
    }
}