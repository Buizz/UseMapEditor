using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseMapEditor.Task
{
    public abstract class TaskEvent
    {
        //작업 조각.


        //작업 전 값과 작업 후 값을 저장.
        public abstract void Redo();
        public abstract void Undo();
    }
}
