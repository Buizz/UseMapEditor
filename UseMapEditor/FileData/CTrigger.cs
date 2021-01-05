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
using static Data.Map.MapData;

namespace UseMapEditor.FileData
{
    public class CTrigger : INotifyPropertyChanged
    {
        MapData mapData;

        public CTrigger(MapData _mapData, RAWTRIGMBRF rawdata, bool IsTrigger)
        {
            mapData = _mapData;

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

        public CTrigger(MapData _mapData)
        {
            mapData = _mapData;
        }


        public CTrigger Clone()
        {
            CTrigger cTrigger = new CTrigger(mapData);
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
        public string name;
        public TriggerManger.TriggerDefine triggerDefine;

        public int type;


        public TrigItem Clone()
        {
            TrigItem item = new TrigItem(mapData);
            item.IsAction = IsAction;
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
        }


        public TrigItem(MapData _mapData, RAWTRIGMBRF.RawCondition rawdata)
        {
            IsAction = false;
            mapData = _mapData;

            type = rawdata.condtype;

            Init(rawdata.values, UseMapEditor.Global.WindowTool.triggerManger.Conditions[type]);
        }

        public TrigItem(MapData _mapData, RAWTRIGMBRF.RawAction rawdata, bool IsTrigger)
        {
            IsAction = true;
            mapData = _mapData;

            type = rawdata.acttype;
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
                        arg.LOCATION = mapData.LocationDatas.Find((x) => x.INDEX == (int)value);
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



        //TODO:데이터로 다시 반환해야됨.
        public string ItemText
        {
            get
            {
                string sm = triggerDefine.SUMMARY;


                for (int i = 0; i < args.Count; i++)
                {
                    sm = sm.Replace("[" + triggerDefine.argDefines[i].argname + "]",  "[" + args[i].GetValue + "]");
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
                        return (uint)LOCATION.INDEX;
                }


                return (uint)VALUE;
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
