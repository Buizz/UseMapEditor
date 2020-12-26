﻿using System;
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


            //TRIG,//Triggers                      STR사용
            //MBRF,//Mission Briefings             STR사용
            //WAV,//WAV String Indexes             STR사용
            //SWNM,//Switch Names                  STR사용
            //MRGN,//Locations                     STR사용
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
                if(stringDatas.Exists(x => x.String == String))
                {
                    StringData s = stringDatas.Find(x => x.String.Contains(String));
                    ResultIndex = s.ResultIndex;

                    return;
                }


                stringDatas.Add(this);
                ResultIndex = stringDatas.Count;
            }



            private string val;
            public string String
            {
                get
                {
                    if (!IsLoaded)
                    {
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
        }

    }
}