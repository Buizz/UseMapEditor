using Data.Map;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseMapEditor.FileData
{
    public class TriggerManger
    {

        public TriggerManger()
        {
            Actions = JsonConvert.DeserializeObject<List<TriggerDefine>>(System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"Data\TriggerEditor\Actions.json"));
            Conditions = JsonConvert.DeserializeObject<List<TriggerDefine>>(System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"Data\TriggerEditor\Conditions.json"));
            BrifngActions = JsonConvert.DeserializeObject<List<TriggerDefine>>(System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"Data\TriggerEditor\Briefing.json"));


            Actions.Insert(0, null);
            Conditions.Insert(0, null);
            BrifngActions.Insert(0, null);



            ArgDic = JsonConvert.DeserializeObject<Dictionary<ArgType, Dictionary<uint, string>>>(System.IO.File.ReadAllText(
                AppDomain.CurrentDomain.BaseDirectory + @"Data\TriggerEditor\Args\TEP.json"));
            TranArgDic = JsonConvert.DeserializeObject<Dictionary<ArgType, Dictionary<uint, string>>>(System.IO.File.ReadAllText(
                AppDomain.CurrentDomain.BaseDirectory + @"Data\TriggerEditor\Args\ko-KR.json"));
        }



        public bool IsArgParseable(ArgType argType)
        {
            return ArgDic.ContainsKey(argType);
        }


        public Dictionary<uint, string> GetArgList(ArgType argType, bool IsTran = false)
        {
            if (ArgDic.ContainsKey(argType))
            {
                if (IsTran)
                {
                    return TranArgDic[argType];
                }
                else
                {
                    return ArgDic[argType];
                }
            }

            return null;
        }

        public string ArgParse(MapData mapData, ArgType argType , uint value, bool IsTran = false)
        {
            if (ArgDic.ContainsKey(argType))
            {
                if (ArgDic[argType].ContainsKey(value))
                {
                    Dictionary<ArgType, Dictionary<uint, string>> dic;

                    if (IsTran)
                    {
                        dic = TranArgDic;
                    }
                    else
                    {
                        dic = ArgDic;
                    }
                    if (dic[argType].ContainsKey(value))
                    {
                        return dic[argType][value];
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "???";
                }
            }
            else
            {
                switch (argType)
                {
                    case ArgType.UNIT:
                        if (mapData.IsCustomUnitName((int)value))
                        {
                            return mapData.GetUnitName((int)value);
                        }
                        else
                        {
                            return mapData.GetMapUnitName((int)value);
                        }
                    case ArgType.WAV:

                        break;
                    case ArgType.LOCATION:

                        break;
                    case ArgType.SWITCH:

                        break;
                }
            }




            return "";
        }


        public List<TriggerDefine> Actions;
        public List<TriggerDefine> Conditions;
        public List<TriggerDefine> BrifngActions;



        public enum ArgType
        {
            VALUE,
            COUNT,
            STRING,
            LOCATION,
            WAV,
            UPRP,
            UNIT,
            CONDITIONSWITCHSTATES,
            NUMERICCOMPARISONS,
            SCORETYPES,
            RESOURCETYPES,
            ALLIANCESTATUSES,
            UNITORDERS,
            ACTIONSWITCHSTATES,
            ACTIONSTATES,
            NUMBERMODIFIERS,
            NONE,
            PLAYER,
            ALWAYSDISPLAY,
            SWITCH,
            AISCRIPT,
            SLOT
        }

        public Dictionary<ArgType, Dictionary<uint, string>> ArgDic;
        public Dictionary<ArgType, Dictionary<uint, string>> TranArgDic;






        public class TriggerDefine
        {

            public class ArgDefine
            {
                //Arg가 들어간 데이터다.
                //ARG를 정의한다.
                //ARG가 들어있는 위치와 타입을 정의.
                public ArgType argtype;
                public string argname;
                public int pos;
            }

            //이거를 액션,조건마다 만들어준다.
            public string NAME;
            public string SUMMARY;
            public int TYPE;
            public int FLAG;

            public List<ArgDefine> argDefines = new List<ArgDefine>();
        }




    }
}
