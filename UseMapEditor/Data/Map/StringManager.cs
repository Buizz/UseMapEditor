using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Map
{
    public partial class MapData
    {
        public List<StringData> stringDatas = new List<StringData>();
        public void LoadString()
        {
            stringDatas.Clear();
            SCEARIONAME.AddToLisT(stringDatas);
            SCEARIODES.AddToLisT(stringDatas);


            FORCENAME[0].AddToLisT(stringDatas);
            FORCENAME[1].AddToLisT(stringDatas);
            FORCENAME[2].AddToLisT(stringDatas);
            FORCENAME[3].AddToLisT(stringDatas);

            for (int i = 1; i < LocationDatas.Count; i++)
            {
                LocationDatas[i].STRING.AddToLisT(stringDatas);
            }


            for (int i = 0; i < WAV.Length; i++)
            {
                WAV[i].AddToLisT(stringDatas);
            }
            for (int i = 0; i < SWNM.Length; i++)
            {
                SWNM[i].AddToLisT(stringDatas);
            }


            for (int i = 0; i < UNIx.STRING.Length; i++)
            {
                UNIx.STRING[i].AddToLisT(stringDatas);
            }

            UPRP = new CUPRP[64];
            UPUS = new byte[64];

            for (int i = 0; i < Triggers.Count; i++)
            {
                for (int t = 0; t < Triggers[i].actions.Count; t++)
                {
                    for (int a = 0; a < Triggers[i].actions[t].args.Count; a++)
                    {
                        switch (Triggers[i].actions[t].args[a].ARGTYPE)
                        {
                            case UseMapEditor.FileData.TriggerManger.ArgType.STRING:
                            case UseMapEditor.FileData.TriggerManger.ArgType.WAV:
                                Triggers[i].actions[t].args[a].STRING.AddToLisT(stringDatas);
                                break;
                            case UseMapEditor.FileData.TriggerManger.ArgType.UPRP:
                                Triggers[i].actions[t].args[a].VALUE = SetUPRP(Triggers[i].actions[t].args[a].UPRP);
                                break;
                        }


                    }
                }
            }


            for (int i = 0; i < Brifings.Count; i++)
            {
                for (int t = 0; t < Brifings[i].actions.Count; t++)
                {
                    for (int a = 0; a < Brifings[i].actions[t].args.Count; a++)
                    {
                        switch (Brifings[i].actions[t].args[a].ARGTYPE)
                        {
                            case UseMapEditor.FileData.TriggerManger.ArgType.STRING:
                            case UseMapEditor.FileData.TriggerManger.ArgType.WAV:
                                Brifings[i].actions[t].args[a].STRING.AddToLisT(stringDatas);
                                break;
                        }


                    }
                }
            }
            //for (int i = 0; i < MBRF.Count; i++)
            //{
            //    for (int t = 0; t < MBRF[i].actions.Length; t++)
            //    {
            //        if (MBRF[i].actions[t].HasString())
            //        {
            //            MBRF[i].actions[t].STRING.AddToLisT(stringDatas);
            //        }
            //    }
            //}







            //TRIG,//Triggers                      STR사용
            //MBRF,//Mission Briefings             STR사용

            //WAV,//WAV String Indexes             STR사용
            //SWNM,//Switch Names                  STR사용

            //UNIx,//BW Unit Settings              STR사용
        }


        public class StringData
        {
            private MapData mapData;

            public int LoadedIndex;
            public bool IsLoaded;


            public int ResultIndex;

            public StringData(MapData _mapData)
            {
                mapData = _mapData;
                IsLoaded = false;
                LoadedIndex = -1;
            }
            public StringData(MapData _mapData, string _val)
            {
                mapData = _mapData;
                IsLoaded = true;
                val = _val;
            }
            public StringData(MapData _mapData, int _index)
            {
                mapData = _mapData;
                IsLoaded = false;
                LoadedIndex = _index-1;
            }

            public void AddToLisT(List<StringData> stringDatas)
            {
                string str = String;
                if (!IsLoaded)
                {
                    ResultIndex = 0;
                    return;
                }


                if(stringDatas.Exists(x => x.String == str))
                {
                    StringData s = stringDatas.Find(x => x.String.Contains(String));
                    ResultIndex = s.ResultIndex;

                    return;
                }


                stringDatas.Add(this);
                ResultIndex = stringDatas.Count;
            }

            public void UnLoaded()
            {
                LoadedIndex = -1;
                IsLoaded = false;
            }

            private string val;
            public string String
            {
                get
                {
                    if (!IsLoaded)
                    {
                        if(mapData.LOADSTRx == null)
                        {
                            return "NotLoad";
                        }

                        if(LoadedIndex == -1 | LoadedIndex >= mapData.LOADSTRx.Length)
                        {
                            return "???";
                        }
                        val = mapData.LOADSTRx[LoadedIndex];
                        IsLoaded = true;
                    }
                    return val;
                }
                set
                {
                    IsLoaded = true;
                    val = value;
                }
            }



            public string CodeString
            {
                get
                {
                    if (!IsLoaded)
                    {
                        if (LoadedIndex == -1 | LoadedIndex >= mapData.LOADSTRx.Length)
                        {
                            return "???";
                        }
                        val = mapData.LOADSTRx[LoadedIndex];
                        IsLoaded = true;
                    }
                    return UseMapEditor.Tools.StringTool.WriteRawString(val);
                }
            }
        }

    }
}
