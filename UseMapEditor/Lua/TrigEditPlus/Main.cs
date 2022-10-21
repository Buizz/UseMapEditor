using Data.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UseMapEditor.Control;
using UseMapEditor.FileData;
using static Data.Map.MapData;
using static UseMapEditor.FileData.TriggerManger;

namespace UseMapEditor.Lua.TrigEditPlus
{
    public class Main
    {
        public NLua.Lua Lua;
        public MapData mapData;


        private bool IsTrigger;

        public Main()
        {
            Lua = new NLua.Lua();

            Lua.DoFile(AppDomain.CurrentDomain.BaseDirectory + "Lua\\TrigEditPlus\\constparser.lua");
            Lua.DoFile(AppDomain.CurrentDomain.BaseDirectory + "Lua\\TrigEditPlus\\condition.lua");
            Lua.DoFile(AppDomain.CurrentDomain.BaseDirectory + "Lua\\TrigEditPlus\\action.lua");
            Lua.DoFile(AppDomain.CurrentDomain.BaseDirectory + "Lua\\TrigEditPlus\\briefing.lua");
            Lua.DoFile(AppDomain.CurrentDomain.BaseDirectory + "Lua\\TrigEditPlus\\tool.lua");


            Lua.RegisterFunction("msgbox", this, this.GetType().GetMethod("MsgBox"));



            Lua.RegisterFunction("Trigger", this, this.GetType().GetMethod("Triggerinterpreter"));
            Lua.RegisterFunction("ParseString", this, this.GetType().GetMethod("ParseString"));
            Lua.RegisterFunction("ParseLocation", this, this.GetType().GetMethod("ParseLocation"));
            Lua.RegisterFunction("ParseUPRP", this, this.GetType().GetMethod("ParseUPRP"));
            Lua.RegisterFunction("ParseUnit", this, this.GetType().GetMethod("ParseUnit"));
            Lua.RegisterFunction("ParseSwitchName", this, this.GetType().GetMethod("ParseSwitchName"));
        }



        private void ParseToolInit()
        {
            unitdic.Clear();
            locdic.Clear();
            switchdic.Clear();

            for (int i = 0; i < 232; i++)
            {
                string tunit;
                if (mapData.IsCustomUnitName(i))
                {
                    tunit = mapData.GetUnitName(i);
                }
                else
                {
                    tunit = mapData.GetEditorUnitName(i);
                }

                tunit = Tools.StringTool.RemoveCtrlChar(tunit);
                unitdic.Add(tunit, i);
            }

            for (int i = 1; i < mapData.LocationDatas.Count; i++)
            {
                locdic.Add(mapData.LocationDatas[i].STRING.String, mapData.LocationDatas[i]);
            }

            for (int i = 0; i < mapData.SWNM.Length; i++)
            {
                string d = mapData.SWNM[i].String;
                if (!mapData.SWNM[i].IsLoaded)
                {
                    d = "Switch " + (i + 1);
                }

                switchdic.Add(d, i);
            }

        }

        Dictionary<string, int> unitdic = new Dictionary<string, int>();
        Dictionary<string, LocationData> locdic = new Dictionary<string, LocationData>();
        Dictionary<string, int> switchdic = new Dictionary<string, int>();
        public int ParseUnit(string unitname)
        {
            return unitdic[unitname];
        }
        public string ParseString(string str)
        {
            return str;
        }

        public LocationData ParseLocation(string locname)
        {
            return locdic[locname];
        }
        public int ParseSwitchName(string swtchname)
        {
            return switchdic[swtchname];
        }


        public CUPRP ParseUPRP(NLua.LuaTable t)
        {
            CUPRP cUPRP = new CUPRP();



            ushort STATUSFLAG = 0;
            ushort STATUSVALID = 0;
            if (t["clocked"] != null)
            {
                bool v = (bool)t["clocked"];
                STATUSVALID += (ushort)(0b1 << 0);
                if(v)
                {
                    STATUSFLAG += (ushort)(0b1 << 0);
                }
            }
            if (t["burrowed"] != null)
            {
                bool v = (bool)t["burrowed"];
                STATUSVALID += (ushort)(0b1 << 1);
                if (v)
                {
                    STATUSFLAG += (ushort)(0b1 << 1);
                }
            }
            if (t["intransit"] != null)
            {
                bool v = (bool)t["intransit"];
                STATUSVALID += (ushort)(0b1 << 2);
                if (v)
                {
                    STATUSFLAG += (ushort)(0b1 << 2);
                }
            }
            if (t["hallucinated"] != null)
            {
                bool v = (bool)t["hallucinated"];
                STATUSVALID += (ushort)(0b1 << 3);
                if (v)
                {
                    STATUSFLAG += (ushort)(0b1 << 3);
                }
            }
            if (t["invincible"] != null)
            {
                bool v = (bool)t["invincible"];
                STATUSVALID += (ushort)(0b1 << 4);
                if (v)
                {
                    STATUSFLAG += (ushort)(0b1 << 4);
                }
            }


            ushort POINTVALID = 0;
            if (t["hitpoint"] != null)
            {
                double v = (double)t["hitpoint"];
                POINTVALID += (ushort)(0b1 << 1);
                cUPRP.HITPOINT = (byte)v;
            }
            if (t["shield"] != null)
            {
                double v = (double)t["shield"];
                POINTVALID += (ushort)(0b1 << 2);
                cUPRP.SHIELDPOINT = (byte)v;
            }
            if (t["energy"] != null)
            {
                double v = (double)t["energy"];
                POINTVALID += (ushort)(0b1 << 3);
                cUPRP.ENERGYPOINT = (byte)v;
            }
            if (t["resource"] != null)
            {
                double v = (double)t["resource"];
                POINTVALID += (ushort)(0b1 << 4);
                cUPRP.RESOURCE = (uint)v;
            }
            if (t["hanger"] != null)
            {
                double v = (double)t["hanger"];
                POINTVALID += (ushort)(0b1 << 5);
                cUPRP.HANGAR = (ushort)v;
            }


            cUPRP.STATUSVALID = STATUSVALID;
            cUPRP.POINTVALID = POINTVALID;
            cUPRP.STATUSFLAG = STATUSFLAG;


            return cUPRP;
        }

        public void Triggerinterpreter(NLua.LuaTable t)
        {
            CTrigger cTrigger = new CTrigger(mapData, IsTrigger);

            if (t["players"] != null)
            {
                NLua.LuaTable players = (NLua.LuaTable)t["players"];
                for (int i = 1; i <= players.Values.Count; i++)
                {
                    double v = (double)Lua.GetFunction("ParsePlayer").Call(players[i])[0];
                    int pnum = (int)v;

                    cTrigger.playerlist[pnum] = 1;
                }
            }
            if (t["flag"] != null)
            {
                NLua.LuaTable flags = (NLua.LuaTable)t["flag"];

                ushort flag = 0;
                for (int i = 1; i <= flags.Values.Count; i++)
                {
                    double v = (double)Lua.GetFunction("ParseTrigFlag").Call(flags[i])[0];
                    int flagv = (int)v;

                    flag += (ushort)(0b1 << flagv);
                }

                cTrigger.exeflag = flag;
            }
            if (t["conditions"] != null)
            {
                NLua.LuaTable conds = (NLua.LuaTable)t["conditions"];
                for (int i = 1; i <= conds.Values.Count; i++)
                {
                    NLua.LuaTable con = (NLua.LuaTable)conds[i];

                    bool tEnable = true;
                    if(con["Disable"] != null)
                    {
                        con = (NLua.LuaTable)con["item"];
                        tEnable = false;
                    }

                    TrigItem trigItem = GetTrigItem(con);
                    if(trigItem == null)
                    {
                        continue;
                    }


                    trigItem.IsEnable = tEnable;

                    cTrigger.conditions.Add(trigItem);
                }
            }
            if (t["actions"] != null)
            {
                NLua.LuaTable acts = (NLua.LuaTable)t["actions"];
                for (int i = 1; i <= acts.Values.Count; i++)
                {
                    NLua.LuaTable act = (NLua.LuaTable)acts[i];

                    bool tEnable = true;
                    if (act["Disable"] != null)
                    {
                        act = (NLua.LuaTable)act["item"];
                        tEnable = false;
                    }

                    TrigItem trigItem = GetTrigItem(act);
                    if (trigItem == null)
                    {
                        continue;
                    }

                    trigItem.IsEnable = tEnable;

                    cTrigger.actions.Add(trigItem);
                }
            }

            Triggers.Add(cTrigger);
        }

        public TrigItem GetTrigItem(NLua.LuaTable t)
        {
            bool IsAction = true;


            int type = (int)(double)t[1];

            List<object> args = new List<object>();
            for (int c = 2; c <= t.Values.Count - 1; c++)
            {
                args.Add(t[c]);
            }

            TrigItem trigItem = new TrigItem(mapData);

            TriggerDefine td = null;

            switch ((string)t["type"])
            {
                case "Briefing":
                    td = Global.WindowTool.triggerManger.BrifngActions[type];
                    break;
                case "Condition":
                    IsAction = false;
                    td = Global.WindowTool.triggerManger.Conditions[type];
                    break;
                case "Action":
                    td = Global.WindowTool.triggerManger.Actions[type];
                    break;
            }




            trigItem.triggerDefine = td;
            trigItem.IsAction = IsAction;
            trigItem.IsTrigger = IsTrigger;
            trigItem.name = td.NAME;
            trigItem.type = td.TYPE;

            if (td.argDefines.Count != args.Count)
            {
                throw new Exception(td.NAME + "은 인자수가 " + td.argDefines.Count + "개 입니다. 하지만 " + args.Count + "개의 인자가 들어왔습니다.");
            }



            for (int i = 0; i < td.argDefines.Count; i++)
            {
                TriggerDefine.ArgDefine ad = td.argDefines[i];

                Arg arg = new Arg(mapData);
                arg.argDefine = ad;
                arg.ARGTYPE = ad.argtype;
                arg.IsInit = false;



                switch (ad.argtype)
                {
                    case ArgType.STRING:
                    case ArgType.WAV:
                        arg.STRING.String = (string)args[i];
                        break;
                    case ArgType.LOCATION:
                        arg.LOCATION = (LocationData)args[i];
                        break;
                    case ArgType.UPRP:
                        arg.UPRP = (CUPRP)args[i];
                        break;
                    default:
                        arg.VALUE = (long)(double)args[i];
                        break;
                }
                trigItem.args.Add(arg);
            }



            return trigItem;
        }





        //Trigger {
        //	players = {AllPlayers
        //    },
        //	conditions = {
        //		MostKills("Terran Ghost");
        //},
        //	actions = {
        //    SetDeaths(P1, SetTo, 10, "Terran Ghost");
        //},
        //	flag = { preserved},
        //}


        public void MsgBox(string str)
        {
            MessageBox.Show(str);
        }


        List<CTrigger> Triggers = new List<CTrigger>();
        public List<CTrigger> exec(string str, MapEditor mapEditor, bool IsTrigger)
        {
            this.IsTrigger = IsTrigger;

            str = str.Replace("\\", "\\\\");
            mapData = mapEditor.mapdata;
            ParseToolInit();

            Triggers.Clear();
            try
            {
                Lua.DoString(str);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "TEP 컴파일 오류");
                return null;
            }
            return Triggers;
        }
    }
}
