using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseMapEditor.FileData;

namespace Data.Map
{
    public partial class MapData
    {

        public enum Codetype
        {
            Unit,
            Upgrade,
            Tech,
            Sprite
        }

        public bool IsCustomUnitName(int index)
        {
            if(index >= 228)
            {
                return false;
            }


            string d = UNIx.STRING[index].String;
            return UNIx.STRING[index].IsLoaded;
        }
        public string GetUnitName(int index)
        {
            if (index >= 228)
            {
                string[] addstring = { "없음", "유닛과 건물", "유닛", "건물", "생산건물" };
                return addstring[index - 228];
            }



            string d = UNIx.STRING[index].String;
            if (UNIx.STRING[index].IsLoaded)
            {
                return d;
            }

            return "???";
        }
        public string GetMapUnitName(int index)
        {
            if (index >= 228)
            {
                string[] addstring = { "없음", "유닛과 건물", "유닛", "건물", "생산건물" };
                return addstring[index - 228];
            }
            string org = UseMapEditor.Global.WindowTool.GetStat_txt(index);
            return org;
        }




        public string GetCodeName(Codetype codetype, int index)
        {
            int label;
            switch (codetype)
            {
                case Codetype.Unit:
                    if (index >= 228)
                    {
                        string[] addstring = { "없음", "유닛과 건물", "유닛", "건물", "생산건물" };
                        return addstring[index - 228];
                    }
                    string d = UNIx.STRING[index].String;
                    string org = UseMapEditor.Global.WindowTool.GetStat_txt(index);
                    if (UNIx.STRING[index].IsLoaded)
                    {
                        return d + "\n" + org;
                    }

                    return org;
                case Codetype.Upgrade:
                    label = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(UseMapEditor.FileData.DatFile.DatFiles.upgrades, "Label", index).Data - 1;


                    return UseMapEditor.Global.WindowTool.GetStat_txt(label);
                case Codetype.Tech:
                    label = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.techdata, "Label", index).Data - 1;


                    return UseMapEditor.Global.WindowTool.GetStat_txt(label);
                case Codetype.Sprite:
                    return "Sprite";
            }
            return "???";
        }

    }
}
