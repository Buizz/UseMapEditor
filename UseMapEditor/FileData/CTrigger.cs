using Data.Map;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using UseMapEditor.Tools;
using static Data.Map.MapData;

namespace UseMapEditor.FileData
{
    public class CTrigger : INotifyPropertyChanged
    {
        MapData mapData;

        bool IsTrigger;
        public CTrigger(MapData _mapData, RAWTRIGMBRF rawdata, bool IsTrigger)
        {
            mapData = _mapData;
            this.IsTrigger = IsTrigger;
            exeflag = rawdata.exeflag;
            playerlist = (byte[])rawdata.playerlist.Clone();

            for (int i = 0; i < rawdata.conditions.Length; i++)
            {
                if (rawdata.conditions[i].condtype != 0)
                {
                    conditions.Add(new TrigItem(mapData, rawdata.conditions[i]));
                }
            }
            for (int i = 0; i < rawdata.actions.Length; i++)
            {
                if (rawdata.actions[i].acttype != 0)
                {
                    actions.Add(new TrigItem(mapData, rawdata.actions[i], IsTrigger));
                }
            }
        }

        public CTrigger(MapData _mapData, bool IsTrigger)
        {
            mapData = _mapData;
            this.IsTrigger = IsTrigger;
        }


        public CTrigger Clone()
        {
            CTrigger cTrigger = new CTrigger(mapData, IsTrigger);
            cTrigger.exeflag = exeflag;
            cTrigger.playerlist = (byte[])playerlist.Clone();

            foreach (TrigItem item in conditions)
            {
                cTrigger.conditions.Add(item.Clone());
            }
            foreach (TrigItem item in actions)
            {
                cTrigger.actions.Add(item.Clone());
            }


            return cTrigger;
        }


        public ObservableCollection<TrigItem> conditions = new ObservableCollection<TrigItem>();
        public ObservableCollection<TrigItem> actions = new ObservableCollection<TrigItem>();


        //Player Execution
        //Following the 16 conditions and 64 actions, every trigger also has this structure
        public uint exeflag;//u32: execution flags
                            //Bit 0 - All conditions are met, executing actions, cleared on the next trigger loop.
                            //Bit 1 - Ignore the following actions: Defeat, Draw.
                            //Bit 2 - Preserve trigger. (Can replace Preserve Trigger action)
                            //Bit 3 - Ignore execution.
                            //Bit 4 - Ignore all of the following actions for this trigger until the next trigger loop: Wait, PauseGame, Transmission, PlayWAV, DisplayTextMessage, CenterView, MinimapPing, TalkingPortrait, and MuteUnitSpeech.
                            //Bit 5 - This trigger has paused the game, ignoring subsequent calls to Pause Game(Unpause Game clears this flag only in the same trigger), may automatically call unpause at the end of action execution ?
                            //Bit 6 - Wait skipping disabled for this trigger, cleared on next trigger loop.
                            //Bit 7 - 31 - Unknown / unused
        public byte[] playerlist = new byte[27];//u8[27]: 1 byte for each player in the #List of Players/Group IDs
                                                //00 - Trigger is not executed for player
                                                //01 - Trigger is executed for player




        public void GetTEPText(StringBuilder sb)
        {
            var argdic = Global.WindowTool.triggerManger.ArgDic;


            sb.AppendLine("Trigger {");
            sb.Append("	players = {");
            int t = 0;
            for (int i = 0; i < 27; i++)
            {
                if(playerlist[i] == 1)
                {
                    if(t != 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(argdic[TriggerManger.ArgType.PLAYER][(uint)i]);
                    t++;
                }
            }


            sb.AppendLine("},");


            if (IsTrigger)
            {
                sb.AppendLine("	conditions = {");
                for (int i = 0; i < conditions.Count; i++)
                {
                    sb.Append("		");
                    conditions[i].CodeText(sb);
                    sb.AppendLine(";");
                }
                sb.AppendLine("	},");
            }




            sb.AppendLine("	actions = {");
            for (int i = 0; i < actions.Count; i++)
            {
                sb.Append("		");
                actions[i].CodeText(sb);
                sb.AppendLine(";");
            }


            sb.AppendLine("	},");

            if(exeflag > 0)
            {
                sb.Append("	flag = {");
                //actexec, preserved, disabled
                //0,2,3
                t = 0;
                for (int i = 0; i < 7; i++)
                {
                    if ((exeflag & (0b1 << i)) > 0)
                    {
                        if (t != 0)
                        {
                            sb.Append(", ");
                        }
                        switch (i)
                        {
                            case 0:
                                sb.Append("actexec");
                                break;
                            case 2:
                                sb.Append("preserved");
                                break;
                            case 3:
                                sb.Append("disabled");
                                break;
                            default:
                                sb.Append("flag" + i);
                                break;
                        }
                        t++;
                    }
                }
                sb.AppendLine("},");
            }


            sb.AppendLine("}");


        }


        public string CommentString
        {
            get
            {
                for (int i = actions.Count - 1; i >= 0; i--)
                {
                    if (actions[i].name == "Comment")
                    {
                        return actions[i].args[0].STRING.String;
                    }
                }
                return "주석없음";
            }
        }


        public System.Windows.Visibility HaveComment
        {
            get
            {
                for (int i = 0; i < actions.Count; i++)
                {
                    if(actions[i].name == "Comment")
                    {
                        return System.Windows.Visibility.Visible;
                    }
                }
                return System.Windows.Visibility.Collapsed;
            }
        }
        public System.Windows.Visibility NotHaveComment
        {
            get
            {
                for (int i = 0; i < actions.Count; i++)
                {
                    if (actions[i].name == "Comment")
                    {
                        return System.Windows.Visibility.Collapsed;
                    }
                }
                return System.Windows.Visibility.Visible;
            }
        }


        public string ConditionString
        {
            get
            {
                string rval = "";

                for (int i = 0; i < conditions.Count; i++)
                {
                    if(conditions[i].type == 13)
                    {
                        continue;
                    }
                    if (i != 0)
                    {
                        rval += "\n";
                    }

                    rval += conditions[i].ItemText;
                }
                return rval;
            }
        }

        public string ActionsString
        {
            get
            {
                string rval = "";

                for (int i = 0; i < actions.Count; i++)
                {
                    if(i != 0)
                    {
                        rval += "\n";
                    }

                    rval += actions[i].ItemText;
                }
                return rval;
            }
        }




        public void PropertyChangeAll()
        {
            OnPropertyChanged("ConditionString");
            OnPropertyChanged("ActionsString");
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
    }


    public class TrigItem : INotifyPropertyChanged
    {
        MapData mapData;

        public List<Arg> args = new List<Arg>();
        public bool IsAction;
        public bool IsTrigger;
        public string name;
        public bool IsEnable { get; set; }
        public TriggerManger.TriggerDefine triggerDefine;

        public int type;


        public TrigItem Clone()
        {
            TrigItem item = new TrigItem(mapData);
            item.IsAction = IsAction;
            item.IsTrigger = IsTrigger;
            item.IsEnable = IsEnable;
            item.name = name;
            item.triggerDefine = triggerDefine;
            item.type = type;


            foreach (Arg argitem in args)
            {
                item.args.Add(argitem.Clone());
            }


            return item;
        }


        public TrigItem(MapData _mapData)
        {
            mapData = _mapData;
            IsEnable = true;
        }


        public TrigItem(MapData _mapData, RAWTRIGMBRF.RawCondition rawdata)
        {
            IsAction = false;
            mapData = _mapData;

            type = rawdata.condtype;

            IsEnable = (rawdata.flags & (0b1 << 1)) == 0;
            Init(rawdata.values, UseMapEditor.Global.WindowTool.triggerManger.Conditions[type]);
        }

        public TrigItem(MapData _mapData, RAWTRIGMBRF.RawAction rawdata, bool IsTrigger)
        {
            IsAction = true;
            this.IsTrigger = IsTrigger;
            mapData = _mapData;

            type = rawdata.acttype;

            IsEnable = (rawdata.flags & (0b1 << 1)) == 0;
            if (IsTrigger)
            {
                Init(rawdata.values, UseMapEditor.Global.WindowTool.triggerManger.Actions[type]);
            }
            else
            {
                Init(rawdata.values, UseMapEditor.Global.WindowTool.triggerManger.BrifngActions[type]);
            }
        }
        public void Init(long[] values, TriggerManger.TriggerDefine td)
        {
            triggerDefine = td;
            name = td.NAME;
            for (int i = 0; i < td.argDefines.Count; i++)
            {
                Arg arg = new Arg(mapData);
                arg.IsInit = false;

                arg.argDefine = td.argDefines[i];

                arg.ARGTYPE = td.argDefines[i].argtype;
                long value = values[td.argDefines[i].pos];

                switch (arg.ARGTYPE)
                {
                    case TriggerManger.ArgType.WAV:
                    case TriggerManger.ArgType.STRING:
                        arg.STRING = new StringData(mapData, (int)value);
                        break;
                    case TriggerManger.ArgType.LOCATION:
                        arg.LOCATION = mapData.LocationDatas.SingleOrDefault((x) => x.INDEX == (int)value);
                        if(arg.LOCATION == null)
                        {
                            LocationData locationData = new LocationData(mapData.mapEditor);

                            locationData.INDEX = (int)value;
                            locationData.STRING.String = "자동생성 로케이션 " + value;



                            mapData.LocationDatas.Add(locationData);
                            arg.LOCATION = locationData;
                        }
                        break;
                    case TriggerManger.ArgType.UPRP:
                        arg.UPRP = mapData.UPRP[value - 1];
                        break;
                    default:
                        arg.VALUE = value;
                        break;
                }
                args.Add(arg);
            }

            if(name == "SetDeaths")
            {
                MemoryFunc.ApplyMemoryFunc(this);
            }
            else if (name == "Deaths")
            {
                MemoryFunc.ApplyMemoryFunc(this);
            }

        }


        public void Init(TriggerManger.TriggerDefine td)
        {
            args.Clear();
            triggerDefine = td;
            name = td.NAME;
            type = td.TYPE;
            for (int i = 0; i < td.argDefines.Count; i++)
            {
                Arg arg = new Arg(mapData);

                arg.argDefine = td.argDefines[i];

                arg.ARGTYPE = td.argDefines[i].argtype;
                args.Add(arg);
            }
        }




        public string ItemText
        {
            get
            {
                string sm = triggerDefine.SUMMARY;

                if (!IsEnable)
                {
                    sm = "(사용안함)" + sm;
                }

                for (int i = 0; i < args.Count; i++)
                {
                    sm = sm.Replace("[" + triggerDefine.argDefines[i].argname + "]", "[" + args[i].GetValue + "]");
                }



                //sb.Append(name);
                //sb.Append("(");
                //for (int i = 0; i < args.Count; i++)
                //{
                //    if (i != 0)
                //    {
                //        sb.Append(",");
                //    }
                //    sb.Append(args[i].GetValue);
                //}

                //sb.Append(")");

                return sm;
            }
        }
        public void CodeText(StringBuilder sb)
        {
            if (!IsEnable)
            {
                sb.Append("Disabled(");
            }

            if (!IsTrigger & IsAction)
            {
                sb.Append("Briefing");
            }

            if (name == "SetDeaths")
            {
                sb.Append(MemoryFunc.GetTEPDeathText(this));
                if (!IsEnable)
                {
                    sb.Append(")");
                }
                return;

            }
            else if (name == "Deaths")
            {
                sb.Append(MemoryFunc.GetTEPDeathText(this));
                if (!IsEnable)
                {
                    sb.Append(")");
                }
                return;
            }
            else if (name == "SetMemory")
            {
                sb.Append(MemoryFunc.GetTEPMemoryText(this));
                if (!IsEnable)
                {
                    sb.Append(")");
                }
                return;

            }
            else if (name == "Memory")
            {
                sb.Append(MemoryFunc.GetTEPMemoryText(this));
                if (!IsEnable)
                {
                    sb.Append(")");
                }
                return;
            }

            {
                sb.Append(name + "(");

                for (int i = 0; i < args.Count; i++)
                {
                    if (i != 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(args[i].GetCode);
                }

                if (!IsEnable)
                {
                    sb.Append(")");
                }

                sb.Append(")");
            }
        }


        public void PropertyChangeAll()
        {
            OnPropertyChanged("ItemText");
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }

    }
    public class Arg
    {
        //Arg가 들어간 데이터다.
        //값, 스트링, 로케이션 등이 들어간다.
        public TriggerManger.TriggerDefine.ArgDefine argDefine;
        public long VALUE;
        public TriggerManger.ArgType ARGTYPE;
        public StringData STRING;
        public LocationData LOCATION;
        public bool IsInit = true;
        public CUPRP UPRP = new CUPRP();

        public MapData mapData;
        public Arg(MapData mapData)
        {
            this.mapData = mapData;
            STRING = new StringData(mapData, "새 문자열");
        }




        public Arg Clone()
        {
            Arg arg = new Arg(mapData);
            arg.argDefine = argDefine;
            arg.VALUE = VALUE;
            arg.ARGTYPE = ARGTYPE;
            arg.STRING = STRING;
            arg.LOCATION = LOCATION;
            arg.IsInit = IsInit;
            arg.UPRP = UPRP;

            return arg;
        }


        public uint GetCHKValue
        {
            get
            {
                switch (ARGTYPE)
                {
                    case TriggerManger.ArgType.SWITCH:
                        return (uint)VALUE;
                    case TriggerManger.ArgType.WAV:
                    case TriggerManger.ArgType.STRING:
                        return (uint)STRING.ResultIndex;
                    case TriggerManger.ArgType.LOCATION:
                        if(mapData.LocationDatas.IndexOf(LOCATION) != -1)
                        {
                            return (uint)LOCATION.INDEX;
                        }
                        else
                        {
                            LocationData location = mapData.LocationDatas.SingleOrDefault((x) => x.NAME == LOCATION.NAME);
                            if(location != null)
                            {
                                LOCATION = location;
                                return (uint)LOCATION.INDEX;
                            }
                        }

                        return 0;
                }


                return (uint)VALUE;
            }
        }


        public string GetCode
        {
            get
            {
                if (IsInit)
                {
                    return argDefine.argname;
                }

                switch (ARGTYPE)
                {
                    case TriggerManger.ArgType.SLOT:
                        return VALUE.ToString();
                    case TriggerManger.ArgType.SWITCH:
                        string d = mapData.SWNM[VALUE].String;
                        if (mapData.SWNM[VALUE].IsLoaded)
                        {
                            return "\"" +d + "\"";
                        }
                        else
                        {
                            return "\"Switch " + (VALUE + 1) + "\"";
                        }

                    case TriggerManger.ArgType.WAV:
                    case TriggerManger.ArgType.STRING:
                        return "\"" + STRING.String + "\"";
                    case TriggerManger.ArgType.LOCATION:
                        return "\"" + LOCATION.STRING.String + "\"";
                    case TriggerManger.ArgType.OFFSET:
                        return "0x" + VALUE.ToString("X");
                    case TriggerManger.ArgType.UPRP:
                        StringBuilder sb = new StringBuilder();

                        sb.AppendLine("{");

                        ushort sfalg = UPRP.STATUSFLAG;
                        ushort svalid = UPRP.STATUSVALID;
                        {
                            string[] headname = { "clocked", "burrowed", "intransit", "hallucinated", "invincible" };

                            for (int bit = 0; bit < 5; bit++)
                            {
                                if ((svalid & (0b1 << bit)) > 0)
                                {
                                    if ((sfalg & (0b1 << bit)) > 0)
                                    {
                                        sb.AppendLine("			" + headname[bit] + " = " + "true" + ",");
                                    }
                                    else
                                    {
                                        sb.AppendLine("			" + headname[bit] + " = " + "false" + ",");
                                    }
                                }
                            }
                        }


                        ushort pvalid = UPRP.POINTVALID;
                        {
                            if ((pvalid & (0b1 << 1)) > 0)
                            {
                                sb.AppendLine("			hitpoint = " + UPRP.HITPOINT + ",");
                            }
                            if ((pvalid & (0b1 << 2)) > 0)
                            {
                                sb.AppendLine("			shield = " + UPRP.SHIELDPOINT + ",");
                            }
                            if ((pvalid & (0b1 << 3)) > 0)
                            {
                                sb.AppendLine("			energy = " + UPRP.ENERGYPOINT + ",");
                            }
                            if ((pvalid & (0b1 << 4)) > 0)
                            {
                                sb.AppendLine("			resource = " + UPRP.RESOURCE + ",");
                            }
                            if ((pvalid & (0b1 << 5)) > 0)
                            {
                                sb.AppendLine("			hanger = " + UPRP.HANGAR + ",");
                            }
                        }

                        sb.Append("		}");


                        return sb.ToString();
                    case TriggerManger.ArgType.COUNT:
                        if (VALUE == 0)
                        {
                            return "All";
                        }
                        break;
                    case TriggerManger.ArgType.ALWAYSDISPLAY:
                        return VALUE.ToString();
                }

                TriggerManger tm = Global.WindowTool.triggerManger;





                string rval = tm.ArgParse(mapData, ARGTYPE, (uint)VALUE, false, true);
                if (ARGTYPE == TriggerManger.ArgType.UNIT)
                {
                    rval = Tools.StringTool.RemoveCtrlChar(rval);

                    rval = "\"" + rval + "\"";
                }
                if (ARGTYPE == TriggerManger.ArgType.AISCRIPT)
                {
                    rval = "\"" + rval + "\"";
                }

                if (rval == "")
                {
                    //해석 실패
                    return VALUE.ToString();
                }
                else
                {
                    return rval;
                }
            }
            set
            {

            }
        }



        //값을 수정하고 가져오는 함수가 필요
        public string GetValue
        {
            get
            {
                if (IsInit)
                {
                    return argDefine.argname;
                }

                switch (ARGTYPE)
                {
                    case TriggerManger.ArgType.SWITCH:
                        string d = mapData.SWNM[VALUE].String;
                        if (mapData.SWNM[VALUE].IsLoaded)
                        {
                            return d;
                        }
                        else
                        {
                            return "스위치" + (VALUE + 1);
                        }

                    case TriggerManger.ArgType.WAV:
                    case TriggerManger.ArgType.STRING:
                        return STRING.String;
                    case TriggerManger.ArgType.LOCATION:
                        return LOCATION.STRING.String;
                    case TriggerManger.ArgType.OFFSET:
                        return "0x" + VALUE.ToString("X");
                    case TriggerManger.ArgType.UPRP:
                        return "상태설정";
                    case TriggerManger.ArgType.COUNT:
                       if(VALUE == 0)
                        {
                            return "모두";
                        }
                        break;
                }

                TriggerManger tm = Global.WindowTool.triggerManger;

                string rval = tm.ArgParse(mapData, ARGTYPE, (uint)VALUE, true);
                if(rval == "")
                {
                    //해석 실패
                    return VALUE.ToString();
                }
                else
                {
                    return rval;
                }
            }
            set
            {

            }
        }
    }
}
