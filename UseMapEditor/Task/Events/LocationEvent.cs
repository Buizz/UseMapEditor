using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseMapEditor.Control;
using static Data.Map.MapData;

namespace UseMapEditor.Task
{
    public class LocationEvent : TaskEvent
    {
        bool IsCreate;
        bool IsDelete;

        MapEditor mapEditor;
        LocationData loc;


        string keyword;
        long newvalue;
        long oldvalue;
        string newstring;
        string oldstring;
        public LocationEvent(MapEditor mapEditor, LocationData loc, bool IsCreate)
        {
            this.mapEditor = mapEditor;
            this.loc = loc;
            //생성,삭제
            if (IsCreate)
            {
                this.IsCreate = true;
            }
            else
            {
                this.IsDelete = true;
            }
        }

        public LocationEvent(MapEditor mapEditor, LocationData loc, string keyword, long newvalue, long oldvalue)
        {
            this.mapEditor = mapEditor;
            this.loc = loc;

            this.keyword = keyword;
            this.newvalue = newvalue;
            this.oldvalue = oldvalue;
        }
        public LocationEvent(MapEditor mapEditor, LocationData loc, string keyword, string newvalue, string oldvalue)
        {
            this.mapEditor = mapEditor;
            this.loc = loc;

            this.keyword = keyword;
            newstring = newvalue;
            oldstring = oldvalue;
        }


        public override void Redo()
        {
            if (IsCreate)
            {
                //생성
                mapEditor.mapdata.LocationDatas.Add(loc);
                return;
            }
            if (IsDelete)
            {
                //삭제
                mapEditor.mapdata.LocationDatas.Remove(loc);
                return;
            }

            switch (keyword)
            {
                case "INDEX":
                    loc.INDEX = (int)newvalue;
                    mapEditor.LocationListSort();
                    break;
                case "X":
                    loc.X = (uint)newvalue;
                    break;
                case "Y":
                    loc.Y = (uint)newvalue;
                    break;
                case "WIDTH":
                    loc.WIDTH = (int)newvalue;
                    break;
                case "HEIGHT":
                    loc.HEIGHT = (int)newvalue;
                    break;
                case "FLAG":
                    loc.FLAG = (ushort)newvalue;
                    break;
                case "NAME":
                    loc.NAME = newstring;
                    break;
            }
        }

        public override void Undo()
        {
            if (IsCreate)
            {
                //삭제
                mapEditor.mapdata.LocationDatas.Remove(loc);
                return;
            }
            if (IsDelete)
            {
                //생성
                mapEditor.mapdata.LocationDatas.Add(loc);
                return;
            }

            switch (keyword)
            {
                case "INDEX":
                    loc.INDEX = (int)oldvalue;
                    mapEditor.LocationListSort();
                    break;
                case "X":
                    loc.X = (uint)oldvalue;
                    break;
                case "Y":
                    loc.Y = (uint)oldvalue;
                    break;
                case "WIDTH":
                    loc.WIDTH = (int)oldvalue;
                    break;
                case "HEIGHT":
                    loc.HEIGHT = (int)oldvalue;
                    break;
                case "FLAG":
                    loc.FLAG = (ushort)oldvalue;
                    break;
                case "NAME":
                    loc.NAME = oldstring;
                    break;
            }
        }
    }
}
