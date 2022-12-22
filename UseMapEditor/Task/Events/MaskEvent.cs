using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseMapEditor.Control;

namespace UseMapEditor.Task.Events
{
    public class MaskEvent : TaskEvent
    {
        MapEditor mapEditor;
        byte newvalue, oldvalue;
        int tileindex;
        public MaskEvent (MapEditor mapEditor, int tileindex, byte newvalue, byte oldvalue)
        {
            this.mapEditor = mapEditor;

            this.newvalue = newvalue;
            this.oldvalue = oldvalue;
            this.tileindex = tileindex;
        }





        public override void Redo()
        {
            mapEditor.mapdata.MASK[tileindex] = newvalue;
        }

        public override void Undo()
        {
            mapEditor.mapdata.MASK[tileindex] = oldvalue;
        }

        public override void Complete()
        {
        }
    }
}
