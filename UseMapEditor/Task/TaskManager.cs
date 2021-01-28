using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseMapEditor.Control;

namespace UseMapEditor.Task
{
    public class TaskManager
    {
        MapEditor mapEditor;

        public TaskManager(MapEditor mapEditor)
        {
            this.mapEditor = mapEditor;
            tasks = new List<Task>();


        }

        List<Task> tasks;
        int taskindex = - 1;


        private Task temptask;
        //테스크시작
        public void TaskStart()
        {
            //테스트 쌓기 시작
            temptask = new Task();
        }
        
        public void TaskAdd(TaskEvent taskEvent)
        {
            if(temptask != null)
            {
                temptask.AddEvent(taskEvent);
            }
        }
        public void TaskEnd()
        {
            if (temptask == null)
            {
                return;
            }
            int taskcount = tasks.Count;

            if(temptask.GetEventCount() == 0)
            {
                temptask = null;
                return;
            }

            //테스크 끝 부분
            for (int i = taskindex + 1; i < taskcount; i++)
            {
                tasks.RemoveAt(taskindex + 1);
            }



            tasks.Add(temptask);
            taskindex += 1;
            UndoRedoBtnRefresh();
            mapEditor.SetDirty();
            temptask = null;
        }


        public void TaskReset()
        {
            tasks.Clear();
            taskindex = -1;
            UndoRedoBtnRefresh();
        }


        private void UndoRedoBtnRefresh()
        {
            mapEditor.UndoBtn.IsEnabled = (taskindex >= 0);
            mapEditor.RedoBtn.IsEnabled = (taskindex < tasks.Count - 1);
        }
        public void Undo()
        {
            if(taskindex >= 0)
            {
                tasks[taskindex].Undo();
                taskindex -= 1;
            }
            UndoRedoBtnRefresh();
            mapEditor.SetDirty();
        }
        public void Redo()
        {
            if (taskindex < tasks.Count - 1)
            {
                taskindex += 1;
                tasks[taskindex].Redo();
            }
            UndoRedoBtnRefresh();
            mapEditor.SetDirty();
        }








        public class Task
        {
            //작업이 뭉쳐있는 것
            List<TaskEvent> events = new List<TaskEvent>();

            public int GetEventCount()
            {
                return events.Count;
            }
            public void AddEvent(TaskEvent taskEvent)
            {
                events.Add(taskEvent);
            }

            public void Redo()
            {
                for (int i = 0; i < events.Count; i++)
                {
                    events[i].Redo();
                }
            }
            public void Undo()
            {
                for (int i = events.Count - 1; i >= 0; i--)
                {
                    events[i].Undo();
                }
            }
        }
    }
}
