using Data.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UseMapEditor.Control.MapEditorControl;
using UseMapEditor.Control;
using ControlzEx.Standard;
using SharpDX.Direct3D9;
using UseMapEditor.DataBinding;
using System.Windows.Media.Media3D;
using System.Web.Configuration;
using Microsoft.Office.Interop.Excel;
using static UseMapEditor.FileData.DatFile.CDatFile.CParamater;
using System.Data;
using System.Reflection;
using System.Windows;
using System.Runtime.InteropServices.ComTypes;
using Application = Microsoft.Office.Interop.Excel.Application;
using static Data.Map.MapData;
using static UseMapEditor.Control.MapEditorControl.SoundSetting;
using Microsoft.Xna.Framework.Media;
using System.Web.UI.WebControls;
using System.Web.UI;
using static UseMapEditor.FileData.TileSet;
using UseMapEditor.MonoGameControl;
using static UseMapEditor.FileData.ExcelData;
using UseMapEditor.Global;
using System.Runtime.CompilerServices;
using UseMapEditor.Windows;
using UseMapEditor.Dialog;

namespace UseMapEditor.FileData
{
    public class ExcelData
    {
        MapEditor mapEditor;
        string baseExcelPath = AppDomain.CurrentDomain.BaseDirectory + @"Data\StarCraftExcelTemplate.xlsx";
        string te = AppDomain.CurrentDomain.BaseDirectory + @"Data\tt.xlsx";

        const int UnitMaxCount = 1700;
        const int DoodadMaxCount = 1000;
        const int SpriteMaxCount = 499;
        const int LocationMaxCount = 256;

        public enum ExcelType
        {
            All,
            Map,
            Player,
            Force,
            Unit,
            Upgrage,
            Tech,
            Sound,
            Trigger,
            MissionBriefing,
            UnitLayout,
            DoodadLayout,
            SpriteLayout,
            LocationLayout,
            TileLayout,
            FogLayout,
            Code
            //플레이어
            //세력
            //유닛
            //업그레이드
            //테크
            //사운드
            //스트링
            //사운드
            //트리거
            //브리핑
            //지형배치
            //유닛배치
            //두데드배치
            //스프라이트배치
            //로케이션배치
            //전장의안개배치
        }

        public static string GetHeader(ExcelType excelType)
        {
            switch (excelType)
            {
                case ExcelType.All:
                    return "전체";
                case ExcelType.Map:
                    return "맵";
                case ExcelType.Player:
                    return "플레이어";
                case ExcelType.Force:
                    return "세력";
                case ExcelType.Unit:
                    return "유닛";
                case ExcelType.Upgrage:
                    return "업그레이드";
                case ExcelType.Tech:
                    return "테크";
                case ExcelType.Sound:
                    return "사운드";
                case ExcelType.Trigger:
                    return "트리거";
                case ExcelType.MissionBriefing:
                    return "브리핑";
                case ExcelType.TileLayout:
                    return "지형배치";
                case ExcelType.UnitLayout:
                    return "유닛배치";
                case ExcelType.DoodadLayout:
                    return "두데드배치";
                case ExcelType.SpriteLayout:
                    return "스프라이트배치";
                case ExcelType.LocationLayout:
                    return "로케이션배치";
                case ExcelType.FogLayout:
                    return "전장의안개배치";
            }
            return "";
        }


        public ExcelData(MapEditor mapEditor)
        {
            this.mapEditor = mapEditor;
        }


        public void Dispos()
        {
            ReleaseExcelObject(this);
        }

        public static void ReleaseExcelObject(object obj)
        {
            try
            {
                if (obj != null)
                {
                    //System.Runtime.InteropServices.Marshal.FinalReleaseComObject(obj);
                    Marshal.ReleaseComObject(obj);
                    obj = null;
                }
            }
            catch (Exception)
            {
                obj = null;
                //throw ex;
            }
            finally
            {
                GC.Collect();
            }
        }


        public bool SaveExcel(string filename, ExcelType excelType)
        {
            Application excelApp = null;
            Workbook template = null;

            try
            {
                // Excel 첫번째 워크시트 가져오기                
                excelApp = new Application();
                excelApp.DisplayAlerts = false;
                template = excelApp.Workbooks.Open(baseExcelPath, Type.Missing, true);

                GetWorksheetData(ExcelType.Code, template);
                SetWorksheetData(ExcelType.Code, template);

                if (excelType == ExcelType.All)
                {
                    foreach (ExcelType item in Enum.GetValues(typeof(ExcelType)))
                    {
                        if (item != ExcelType.All && item != ExcelType.Code)
                        {
                            SetWorksheetData(item, template);
                        }
                    }
                }
                else
                {
                    SetWorksheetData(excelType, template);


                    foreach (ExcelType item in Enum.GetValues(typeof(ExcelType)))
                    {
                        if (item != excelType && item != ExcelType.All && item != ExcelType.Code)
                        {

                            template.Worksheets.Item[item.ToString()].Delete();
                        }
                    }
                }





                //((Worksheet)wb.Worksheets.Item[1]).Delete();

                //ws = wb.Worksheets.get_Item(1) as Worksheet;

                // 데이타 넣기

                if (File.Exists(filename)) File.Delete(filename);
                object misValue = System.Reflection.Missing.Value;
                // 엑셀파일 저장
                template.SaveAs(filename);
                template.Close(false, misValue, misValue);
                excelApp.Quit();
            }
            catch (Exception e){
                MsgDialog msgDialog = new MsgDialog("엑셀 저장에 실패했습니다.\n" + e.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                msgDialog.ShowDialog();
            }
            finally
            {
                // Clean up
                ReleaseExcelObject(template);
                ReleaseExcelObject(excelApp);
            }

            return true;
        }



        string[] tilesetlist = { "배드랜드", "군사기지", "우주", "화산", "정글", "사막", "얼음", "황혼" };
        string[] ownerlist = { "사용안함", "구조가능", "컴퓨터", "사람", "중립" };
        string[] racelist = { "저그","테란","프로토스","유저선택","비활성"};
        string[] colorlist = { "빨강","파랑","청록","보라","주황","갈색","흰색","노랑","초록","옅은노랑","황갈색","중립","담록","푸른회색","시안","분홍","황록","라임","남색","자홍","회색","검정","랜덤","플레이어선택","커스텀RGB"};
        string[] ableflag = {"불가", "기본", "사용" };
        string[] forcelist = { "세력 1", "세력 2", "세력 3","세력 4" };
        string[] useflag = { "사용", "사용안함" };
        string[] defualtflag = { "기본", "수정" };
        string[] techflag = { "기본값", "연구불가", "연구가능", "연구완료" };

        public enum CodeType
        {
            TileSet,
            Owner,
            Race,
            Color,
            Able,
            Default,
            Use,
            Tech,
            Number,
            HexColor
        }
        public object DecodeValue(CodeType codeType, string value)
        {
            int rval = 0;
            switch (codeType)
            {
                case CodeType.TileSet:
                    for (int i = 0; i < tilesetlist.Length; i++)
                    {
                        if (tilesetlist[i] == value)
                        {
                            return i;
                        }
                    }
                    return 0;
                case CodeType.Owner:
                    for (int i = 0; i < ownerlist.Length; i++)
                    {
                        if (ownerlist[i] == value)
                        {
                            return i;
                        }
                    }
                    return 0;
                case CodeType.Race:
                    for (int i = 0; i < racelist.Length; i++)
                    {
                        if (racelist[i] == value)
                        {
                            return i;
                        }
                    }
                    return 0;
                case CodeType.Color:
                    for (int i = 0; i < colorlist.Length; i++)
                    {
                        if (colorlist[i] == value)
                        {
                            return i;
                        }
                    }
                    return 0;
                case CodeType.Able:

                    for (int i = 0; i < ableflag.Length; i++)
                    {
                        if (ableflag[i] == value)
                        {
                            rval = i;
                            break;
                        }
                    }

                    MapDataBinding.UserDefaultCode dfcode = (MapDataBinding.UserDefaultCode)rval;
                    return dfcode;
                case CodeType.Default: //반대로 되어있음
                    if (defualtflag[0] == value)
                    {
                        return false;
                    }
                    else if(defualtflag[1] == value)
                    {
                        return true;
                    }

                    return false;
                case CodeType.Use:
                    if (useflag[0] == value)
                    {
                        return true;
                    }
                    else if (useflag[1] == value)
                    {
                        return false;
                    }

                    return false;
                case CodeType.Tech:
                    if (techflag[1] == value)
                    {
                        return MapDataBinding.UserDefaultCode.Unusable;
                    }
                    else if (techflag[0] == value)
                    {
                        return MapDataBinding.UserDefaultCode.Default;
                    }
                    else if (techflag[2] == value)
                    {
                        return MapDataBinding.UserDefaultCode.Usable;
                    }
                    else if (techflag[3] == value)
                    {
                        return MapDataBinding.UserDefaultCode.Complete;
                    }

                    return MapDataBinding.UserDefaultCode.Unusable;
                case CodeType.Number:
                    return int.Parse(value);
            }
            return "";
        }

        public string EncodeValue(CodeType codeType, object obj)
        {
            switch (codeType)
            {
                case CodeType.TileSet:
                    return tilesetlist[(int)obj];
                case CodeType.Owner:
                    return ownerlist[(int)obj];
                case CodeType.Race:
                    return racelist[(int)obj];
                case CodeType.Color:
                    return colorlist[(int)obj];
                case CodeType.Able:
                    MapDataBinding.UserDefaultCode dfcode = (MapDataBinding.UserDefaultCode)(int)obj;

                    switch (dfcode)
                    {
                        case MapDataBinding.UserDefaultCode.Unusable:
                            return ableflag[0];
                        case MapDataBinding.UserDefaultCode.Default:
                            return ableflag[1];
                        case MapDataBinding.UserDefaultCode.Usable:
                            return ableflag[2];
                    }

                    return "";
                case CodeType.Default: //반대로 되어있음
                    return (bool)obj ? defualtflag[1] : defualtflag[0];
                case CodeType.Use:
                    if((bool)obj)
                    {
                        return useflag[0];
                    }
                    else
                    {
                        return useflag[1];
                    }
                case CodeType.Tech:
                    MapDataBinding.UserDefaultCode tecode = (MapDataBinding.UserDefaultCode)(int)obj;

                    switch (tecode)
                    {
                        case MapDataBinding.UserDefaultCode.Unusable:
                            return techflag[1];
                        case MapDataBinding.UserDefaultCode.Default:
                            return techflag[0];
                        case MapDataBinding.UserDefaultCode.Usable:
                            return techflag[2];
                        case MapDataBinding.UserDefaultCode.Complete:
                            return techflag[3];
                    }

                    return "";
            }
            return "";
        }


        public void SetWorksheetData(ExcelType excelType, Workbook wb)
        {
            Worksheet ws = wb.Worksheets.Item[excelType.ToString()];
            ws.Select();
            CellManager cm = new CellManager(ws);

            switch (excelType)
            {
                case ExcelType.Map:
                    {
                        /*
                        * 지형   2,1
                        * 크기X  2,2
                        * 크기Y  3,2
                        * 제목   1,4
                        * 설명   1,7
                        */
                        string tileset = EncodeValue( CodeType.TileSet, mapEditor.mapDataBinding.TileSet);
                        string width = mapEditor.mapDataBinding.WIDTH.ToString();
                        string height = mapEditor.mapDataBinding.HEIGHT.ToString();
                        string title = mapEditor.mapDataBinding.Title;
                        string infor = mapEditor.mapDataBinding.Description;

                        cm.SetCells(2, 1, tileset);
                        cm.SetCells(2, 2, width);
                        cm.SetCells(3, 2, height);
                        cm.SetCells(1, 4, title);
                        cm.SetCells(1, 7, infor);
                    }
                    break;
                case ExcelType.Player:
                    /*
                    * 제어    2,2 + i
                    * 종족    3,2 + i
                    * 색상    4,2 + i
                    * RGB코드 5,2 + i
                    */
                    cm.BeginUpdate();
                    for (int i = 0; i < 8; i++)
                    {
                        string control = EncodeValue(CodeType.Owner, mapEditor.mapDataBinding.playerBindings[i].Owner);
                        string race = EncodeValue(CodeType.Race, mapEditor.mapDataBinding.playerBindings[i].Race);
                        string color = EncodeValue(CodeType.Color, mapEditor.mapDataBinding.playerBindings[i].Color);
                        string rgbcode = mapEditor.mapDataBinding.playerBindings[i].IconColor.ToString();

                        cm.AddCells(2, 2 + i, control);
                        cm.AddCells(3, 2 + i, race);
                        cm.AddCells(4, 2 + i, color);
                        cm.AddCells(5, 2 + i, rgbcode);
                    }
                    cm.EndUpdate();

                    break;
                case ExcelType.Force:
                    /*
                    * 세력        2,2 + i
                    * 세력이름    2,11 + i
                    */

                    cm.BeginUpdate();
                    for (int i = 0; i < 8; i++)
                    {
                        string playerForce = forcelist[mapEditor.mapdata.FORCE[i]];

                        cm.AddCells(2, 2 + i, playerForce);

                    }
                    cm.EndUpdate();
                    cm.BeginUpdate();
                    for (int i = 0; i < 4; i++)
                    {
                        string forcename = mapEditor.mapDataBinding.forceBindings[i].ForceName;

                        string forceAllied = EncodeValue(CodeType.Use, mapEditor.mapDataBinding.forceBindings[i].Allied);
                        string forceVictory = EncodeValue(CodeType.Use, mapEditor.mapDataBinding.forceBindings[i].AlliedVictory);
                        string forceVision = EncodeValue(CodeType.Use, mapEditor.mapDataBinding.forceBindings[i].ShareVision);
                        string forceRandomize = EncodeValue(CodeType.Use, mapEditor.mapDataBinding.forceBindings[i].Randomize);

                        cm.AddCells(2, 11 + i, forcename);

                        cm.AddCells(3, 11 + i, forceAllied);
                        cm.AddCells(4, 11 + i, forceVictory);
                        cm.AddCells(5, 11 + i, forceVision);
                        cm.AddCells(6, 11 + i, forceRandomize);
                    }
                    cm.EndUpdate();
                    break;
                case ExcelType.Unit:
                    cm.BeginUpdate();
                    for (int i = 0; i < 228; i++)
                    {
                        string usedefault = EncodeValue(CodeType.Default, mapEditor.mapDataBinding.unitdataBindings[i].USEDEFAULT);

                        string unitname = "";
                        if(mapEditor.mapDataBinding.unitdataBindings[i].SecondNameVisble == Visibility.Visible)
                        {
                            unitname = mapEditor.mapDataBinding.unitdataBindings[i].SecondName;
                        }
                        else
                        {
                            unitname = mapEditor.mapDataBinding.unitdataBindings[i].MainName;
                        }

                        string hp = mapEditor.mapDataBinding.unitdataBindings[i].HIT;
                        string shild = mapEditor.mapDataBinding.unitdataBindings[i].SHIELD.ToString();
                        string armor = mapEditor.mapDataBinding.unitdataBindings[i].ARMOR.ToString();
                        string time = mapEditor.mapDataBinding.unitdataBindings[i].BUILDTIME.ToString();
                        string min = mapEditor.mapDataBinding.unitdataBindings[i].MIN.ToString();
                        string gas = mapEditor.mapDataBinding.unitdataBindings[i].GAS.ToString();
                        string ground = mapEditor.mapDataBinding.unitdataBindings[i].GDMG.ToString();
                        string groundplus = mapEditor.mapDataBinding.unitdataBindings[i].GBDMG.ToString();
                        string air = mapEditor.mapDataBinding.unitdataBindings[i].ADMG.ToString();
                        string airplus = mapEditor.mapDataBinding.unitdataBindings[i].ABDMG.ToString();

                        string buliddefault = EncodeValue(CodeType.Able, mapEditor.mapDataBinding.unitdataBindings[i].UseDefaultCode);


                        cm.AddCells(3, 2 + i, usedefault);

                        cm.AddCells(4, 2 + i, unitname);
                        cm.AddCells(5, 2 + i, hp);
                        cm.AddCells(6, 2 + i, shild);
                        cm.AddCells(7, 2 + i, armor);
                        cm.AddCells(8, 2 + i, time);
                        cm.AddCells(9, 2 + i, min);
                        cm.AddCells(10, 2 + i, gas);
                        cm.AddCells(11, 2 + i, ground);
                        cm.AddCells(12, 2 + i, groundplus);
                        cm.AddCells(13, 2 + i, air);
                        cm.AddCells(14, 2 + i, airplus);

                        cm.AddCells(15, 2 + i, buliddefault);

                        for (int j = 0; j < 8; j++)
                        {
                            string bulidPlayer = EncodeValue(CodeType.Able, mapEditor.mapDataBinding.unitdataBindings[i].UsePlayerCode[j]);

                            cm.AddCells(16 + j, 2 + i, bulidPlayer);
                        }
                    }
                    cm.EndUpdate();

                    break;
                case ExcelType.Upgrage:
                    cm.BeginUpdate();
                    for (int i = 0; i < 61; i++)
                    {
                        string usedefault = EncodeValue(CodeType.Default, mapEditor.mapDataBinding.upgradeDataBindings[i].USEDEFAULT);

                        string min = mapEditor.mapDataBinding.upgradeDataBindings[i].BASEMIN.ToString();
                        string bsmin = mapEditor.mapDataBinding.upgradeDataBindings[i].BONUSMIN.ToString();
                        string gas = mapEditor.mapDataBinding.upgradeDataBindings[i].BASEGAS.ToString();
                        string bsgas = mapEditor.mapDataBinding.upgradeDataBindings[i].BONUSGAS.ToString();
                        string time = mapEditor.mapDataBinding.upgradeDataBindings[i].BASETIME.ToString();
                        string bstime = mapEditor.mapDataBinding.upgradeDataBindings[i].BONUSTIME.ToString();

                        cm.AddCells(3, 3 + i, usedefault);

                        cm.AddCells(4, 3 + i, min);
                        cm.AddCells(5, 3 + i, bsmin);
                        cm.AddCells(6, 3 + i, gas);
                        cm.AddCells(7, 3 + i, bsgas);
                        cm.AddCells(8, 3 + i, time);
                        cm.AddCells(9, 3 + i, bstime);


                        string startdefault = mapEditor.mapDataBinding.upgradeDataBindings[i].DEFAULTSTARTLEVEL.ToString();
                        string maxdefault = mapEditor.mapDataBinding.upgradeDataBindings[i].DEFAULTMAXLEVEL.ToString();

                        cm.AddCells(10, 3 + i, startdefault);
                        cm.AddCells(11, 3 + i, maxdefault);


                        for (int j = 0; j < 12; j++)
                        {
                            string startplayer = mapEditor.mapDataBinding.upgradeDataBindings[i].playerbind[j].STARTLEVEL.ToString();
                            string maxplayer = mapEditor.mapDataBinding.upgradeDataBindings[i].playerbind[j].MAXLEVEL.ToString();

                            if (!mapEditor.mapDataBinding.upgradeDataBindings[i].playerbind[j].LEVELENABLED)
                            {
                                startplayer = "-1";
                                maxplayer = "-1";
                            }


                            cm.AddCells(12 + j * 2, 3 + i, startplayer);
                            cm.AddCells(13 + j * 2, 3 + i, maxplayer);
                        }
                    }
                    cm.EndUpdate();

                    break;
                case ExcelType.Tech:
                    cm.BeginUpdate();
                    for (int i = 0; i < 44; i++)
                    {
                        string usedefault = EncodeValue(CodeType.Default, mapEditor.mapDataBinding.techDataBindings[i].USEDEFAULT);

                        string min = mapEditor.mapDataBinding.techDataBindings[i].MIN.ToString();
                        string gas = mapEditor.mapDataBinding.techDataBindings[i].GAS.ToString();
                        string time = mapEditor.mapDataBinding.techDataBindings[i].BASETIME.ToString();
                        string energy = mapEditor.mapDataBinding.techDataBindings[i].ENERGY.ToString();

                        cm.AddCells(3, 2 + i, usedefault);

                        cm.AddCells(4, 2 + i, min);
                        cm.AddCells(5, 2 + i, gas);
                        cm.AddCells(6, 2 + i, time);
                        cm.AddCells(7, 2 + i, energy);


                        string sdefault = EncodeValue(CodeType.Tech, mapEditor.mapDataBinding.techDataBindings[i].UseDefaultCode);

                        cm.AddCells(8, 2 + i, sdefault);


                        for (int j = 0; j < 8; j++)
                        {
                            string playerdefault = EncodeValue(CodeType.Tech, mapEditor.mapDataBinding.techDataBindings[i].UsePlayerCode[j]);


                            cm.AddCells(9 + j, 2 + i, playerdefault);
                        }
                    }
                    cm.EndUpdate();

                    break;
                case ExcelType.Sound:
                    cm.BeginUpdate();
                    //사운드 파일들을 임시파일로 만들어야됨?
                    for (int i = 0; i < mapEditor.mapdata.WAV.Count; i++)
                    {
                        string d = mapEditor.mapdata.WAV[i].String;
                        if (mapEditor.mapdata.WAV[i].IsLoaded)
                        {
                            string filname = d;
                            string path = "???";


                            SoundData soundData = mapEditor.mapdata.soundDatas.Find((x) => x.path == d);
                            if (soundData == null)
                            {
                                path = "StarCraftSound";
                            }
                            else
                            {
                                long s = soundData.bytes.Length;
                                s /= 1024;

                                path = "INCHK";
                            }
                            cm.AddCells(1, 2 + i, filname);
                            cm.AddCells(2, 2 + i, path);
                        }
                    }

                    cm.EndUpdate();
                    break;
                case ExcelType.Trigger:
                    break;
                case ExcelType.MissionBriefing:
                    break;
                case ExcelType.UnitLayout:
                    cm.BeginUpdate();
                    for (int i = 0; i < mapEditor.mapdata.UNIT.Count; i++)
                    {
                        CUNIT cUNIT = mapEditor.mapdata.UNIT[i];

                        string unitclass = cUNIT.unitclass.ToString();
                        string X = cUNIT.X.ToString();
                        string Y = cUNIT.Y.ToString();
                        string unitID = cUNIT.unitID.ToString();
                        string linkFlag = cUNIT.linkFlag.ToString();
                        string validstatusFlag = cUNIT.validstatusFlag.ToString();
                        string validunitFlag = cUNIT.validunitFlag.ToString();
                        string player = cUNIT.player.ToString();
                        string hitPoints = cUNIT.hitPoints.ToString();
                        string shieldPoints = cUNIT.shieldPoints.ToString();
                        string energyPoints = cUNIT.energyPoints.ToString();
                        string resoruceAmount = cUNIT.resoruceAmount.ToString();
                        string hangar = cUNIT.hangar.ToString();
                        string stateFlag = cUNIT.stateFlag.ToString();
                        string unused = cUNIT.unused.ToString();
                        string linkedUnit = cUNIT.linkedUnit.ToString();

                        cm.AddCells(1, 2 + i, unitclass);

                        cm.AddCells(2, 2 + i, "=VLOOKUP(C" + (i + 2) + ", Unit!$A$1:$B$229, 2, FALSE)");

                        cm.AddCells(3, 2 + i, unitID);
                        cm.AddCells(4, 2 + i, X);
                        cm.AddCells(5, 2 + i, Y);
                        cm.AddCells(6, 2 + i, player);
                        cm.AddCells(7, 2 + i, hitPoints);
                        cm.AddCells(8, 2 + i, shieldPoints);
                        cm.AddCells(9, 2 + i, energyPoints);
                        cm.AddCells(10, 2 + i, resoruceAmount);
                        cm.AddCells(11, 2 + i, hangar);
                        cm.AddCells(12, 2 + i, stateFlag);
                        cm.AddCells(13, 2 + i, linkFlag);
                        cm.AddCells(14, 2 + i, linkedUnit);
                        cm.AddCells(15, 2 + i, validstatusFlag);
                        cm.AddCells(16, 2 + i, validunitFlag);
                        cm.AddCells(17, 2 + i, unused);
                    }
                    cm.EndUpdate();

                    break;
                case ExcelType.DoodadLayout:
                    cm.BeginUpdate();
                    for (int i = 0; i < mapEditor.mapdata.DD2.Count; i++)
                    {
                        CDD2 cDD2 = mapEditor.mapdata.DD2[i];

                        string ID     = cDD2.ID.ToString();
                        string X      = cDD2.X.ToString();
                        string Y      = cDD2.Y.ToString();
                        string PLAYER = cDD2.PLAYER.ToString();
                        string FLAG = cDD2.FLAG.ToString();

                        cm.AddCells(2, 2 + i, ID);
                        cm.AddCells(3, 2 + i, X);
                        cm.AddCells(4, 2 + i, Y);
                        cm.AddCells(5, 2 + i, PLAYER);
                        cm.AddCells(6, 2 + i, FLAG);
                    }
                    cm.EndUpdate();

                    break;
                case ExcelType.SpriteLayout:
                    cm.BeginUpdate();
                    for (int i = 0; i < mapEditor.mapdata.THG2.Count; i++)
                    {
                        CTHG2 cTHG2 = mapEditor.mapdata.THG2[i];

                        string ID     = cTHG2.ID.ToString();
                        string X      = cTHG2.X.ToString();
                        string Y      = cTHG2.Y.ToString();
                        string PLAYER = cTHG2.PLAYER.ToString();
                        string UNUSED = cTHG2.UNUSED.ToString();
                        string FLAG = cTHG2.FLAG.ToString();

                        cm.AddCells(2, 2 + i, ID);
                        cm.AddCells(3, 2 + i, X);
                        cm.AddCells(4, 2 + i, Y);
                        cm.AddCells(5, 2 + i, PLAYER);
                        cm.AddCells(6, 2 + i, FLAG);
                        cm.AddCells(7, 2 + i, UNUSED);
                    }
                    cm.EndUpdate();

                    break;
                case ExcelType.LocationLayout:
                    cm.BeginUpdate();
                    for (int i = 1; i < mapEditor.mapdata.GetLocationCount(); i++)
                    {
                        LocationData locationData = mapEditor.mapdata.GetLocationFromListIndex(i);

                        string X = locationData.X.ToString();
                        string Y = locationData.Y.ToString();
                        string WIDTH = locationData.WIDTH.ToString();
                        string HEIGHT = locationData.HEIGHT.ToString();
                        string NAME = locationData.NAME;
                        string FLAG = locationData.FLAG.ToString(); ;

                        cm.AddCells(1, 1 + i, locationData.INDEX.ToString());
                        cm.AddCells(2, 1 + i, NAME);
                        cm.AddCells(3, 1 + i, X);
                        cm.AddCells(4, 1 + i, Y);
                        cm.AddCells(5, 1 + i, WIDTH);
                        cm.AddCells(6, 1 + i, HEIGHT);
                        cm.AddCells(7, 1 + i, FLAG);
                    }
                    cm.EndUpdate();

                    break;
                case ExcelType.TileLayout:
                    cm.BeginUpdate();
                    for (int y = 0; y < mapEditor.mapdata.HEIGHT; y++)
                    {
                        for (int x = 0; x < mapEditor.mapdata.WIDTH; x++)
                        {
                            ushort mtxm = mapEditor.mapdata.TILE[x + y * mapEditor.mapdata.WIDTH];
                            cm.AddCells(1 + x, 1 + y, mtxm.ToString());
                        }
                    }
                    cm.EndUpdate();

                    break;
                case ExcelType.FogLayout:
                    cm.BeginUpdate();
                    for (int y = 0; y < mapEditor.mapdata.HEIGHT; y++)
                    {
                        for (int x = 0; x < mapEditor.mapdata.WIDTH; x++)
                        {
                            ushort mtxm = mapEditor.mapdata.MASK[x + y * mapEditor.mapdata.WIDTH];
                            cm.AddCells(1 + x, 1 + y, mtxm.ToString());
                        }
                    }
                    cm.EndUpdate();

                    break;
                case ExcelType.Code:
                    //코드데이터 먼저 읽어오기   9 / 10
                    Dictionary<ushort, DoodadPallet> t = Global.WindowTool.MapViewer.tileSet.DoodadPallets[mapEditor.mapdata.TILETYPE];
                    cm.BeginUpdate();
                    int index = 0;
                    foreach (var item in t)
                    {
                        cm.AddCells(10, 3 + index, item.Key.ToString());
                        cm.AddCells(11, 3 + index, item.Value.tblString + "#" + (index + 1));
                        index++;
                    }

                    index = 0;
                    foreach (var item in mapEditor.mapDataBinding.spriteDataBindings)
                    {
                        cm.AddCells(12, 3 + index, index.ToString());
                        cm.AddCells(13, 3 + index, item.MainName);
                        index++;
                    }


                    cm.EndUpdate();
                    break;
            }





            ReleaseExcelObject(ws);
        }


        public void GetWorksheetData(ExcelType excelType, Workbook wb)
        {
            Worksheet ws = wb.Worksheets.Item[excelType.ToString()];
            CellManager cm = new CellManager(ws);
            
            switch (excelType)
            {
                case ExcelType.Map:
                    {
                        /*
                        * 지형   2,1
                        * 크기X  2,2
                        * 크기Y  3,2
                        * 제목   1,4
                        * 설명   1,7
                        */
                        cm.ReadCells(3, 7);


                        mapEditor.mapDataBinding.TileSet = (int)DecodeValue(CodeType.TileSet, cm.GetCells(2, 1));
                        mapEditor.mapDataBinding.WIDTH = ushort.Parse(cm.GetCells(2, 2));
                        mapEditor.mapDataBinding.HEIGHT = ushort.Parse(cm.GetCells(3, 2));
                        mapEditor.mapDataBinding.Title = cm.GetCells(1, 4);
                        mapEditor.mapDataBinding.Description = cm.GetCells(1, 7);
                    }
                    break;
                case ExcelType.Player:
                    /*
                    * 제어    2,2 + i
                    * 종족    3,2 + i
                    * 색상    4,2 + i
                    * RGB코드 5,2 + i
                    */
                    cm.ReadCells(5, 10);
                    for (int i = 0; i < 8; i++)
                    {
                        mapEditor.mapDataBinding.playerBindings[i].Owner = (int)DecodeValue(CodeType.Owner, cm.GetCells(2, 2 + i));
                        mapEditor.mapDataBinding.playerBindings[i].Race = (int)DecodeValue(CodeType.Race, cm.GetCells(3, 2 + i));
                        mapEditor.mapDataBinding.playerBindings[i].Color = (int)DecodeValue(CodeType.Color, cm.GetCells(4, 2 + i));

                        if(mapEditor.mapDataBinding.playerBindings[i].Color == 24)
                        {
                            //사용자 지정 컬러
                            System.Drawing.ColorConverter d = new System.Drawing.ColorConverter();
                            System.Drawing.Color c = (System.Drawing.Color)d.ConvertFromString(cm.GetCells(5, 2 + i));

                            mapEditor.mapDataBinding.playerBindings[i].CustomColor = new Microsoft.Xna.Framework.Color(c.R, c.G, c.B);
                        }
                    }

                    break;
                case ExcelType.Force:
                    /*
                    * 세력        2,2 + i
                    * 세력이름    2,11 + i
                    */
                    cm.ReadCells(6, 15);


                    for (int i = 0; i < 8; i++)
                    {
                        string force = cm.GetCells(2, 2 + i);

                        if (force == forcelist[0])
                        {
                            //세력 1
                            mapEditor.mapdata.FORCE[i] = 0;
                        }
                        else if (force == forcelist[1])
                        {
                            //세력 1
                            mapEditor.mapdata.FORCE[i] = 1;
                        }
                        else if (force == forcelist[2])
                        {
                            //세력 1
                            mapEditor.mapdata.FORCE[i] = 2;
                        }
                        else if (force == forcelist[3])
                        {
                            //세력 1
                            mapEditor.mapdata.FORCE[i] = 3;
                        }
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        mapEditor.mapDataBinding.forceBindings[i].ForceName = cm.GetCells(2, 11 + i);
                        mapEditor.mapDataBinding.forceBindings[i].Allied = (bool) DecodeValue(CodeType.Use, cm.GetCells(3, 11 + i));
                        mapEditor.mapDataBinding.forceBindings[i].AlliedVictory = (bool) DecodeValue(CodeType.Use, cm.GetCells(4, 11 + i));
                        mapEditor.mapDataBinding.forceBindings[i].ShareVision = (bool) DecodeValue(CodeType.Use, cm.GetCells(5, 11 + i));
                        mapEditor.mapDataBinding.forceBindings[i].Randomize = (bool)DecodeValue(CodeType.Use, cm.GetCells(6, 11 + i));
                    }
                    break;
                case ExcelType.Unit:
                    cm.ReadCells(24, 230);
                    for (int i = 0; i < 228; i++)
                    {
                        mapEditor.mapDataBinding.unitdataBindings[i].STRING = cm.GetCells(4, 2 + i);
                        mapEditor.mapDataBinding.unitdataBindings[i].USEDEFAULT = (bool) DecodeValue(CodeType.Default, cm.GetCells(3, 2 + i));
                       
                       
                        mapEditor.mapDataBinding.unitdataBindings[i].HIT = cm.GetCells(5, 2 + i);
                        mapEditor.mapDataBinding.unitdataBindings[i].SHIELD = ushort.Parse(cm.GetCells(6, 2 + i));
                        mapEditor.mapDataBinding.unitdataBindings[i].ARMOR = byte.Parse(cm.GetCells(7, 2 + i));
                        mapEditor.mapDataBinding.unitdataBindings[i].BUILDTIME = ushort.Parse(cm.GetCells(8, 2 + i));
                        mapEditor.mapDataBinding.unitdataBindings[i].MIN = ushort.Parse(cm.GetCells(9, 2 + i));
                        mapEditor.mapDataBinding.unitdataBindings[i].GAS = ushort.Parse(cm.GetCells(10, 2 + i));
                        mapEditor.mapDataBinding.unitdataBindings[i].GDMG = ushort.Parse(cm.GetCells(11, 2 + i));
                        mapEditor.mapDataBinding.unitdataBindings[i].GBDMG = ushort.Parse(cm.GetCells(12, 2 + i));
                        mapEditor.mapDataBinding.unitdataBindings[i].ADMG = ushort.Parse(cm.GetCells(13, 2 + i));
                        mapEditor.mapDataBinding.unitdataBindings[i].ABDMG = ushort.Parse(cm.GetCells(14, 2 + i));

                        mapEditor.mapDataBinding.unitdataBindings[i].UseDefaultCode = (MapDataBinding.UserDefaultCode)DecodeValue(CodeType.Able, cm.GetCells(15, 2 + i));

                        for (int j = 0; j < 8; j++)
                        {
                            mapEditor.mapDataBinding.unitdataBindings[i].UsePlayerCode[j] = (MapDataBinding.UserDefaultCode)DecodeValue(CodeType.Able, cm.GetCells(16 + j, 2 + i));
                        }
                    }

                    break;
                case ExcelType.Upgrage:
                    cm.ReadCells(37, 64);
                    for (int i = 0; i < 61; i++)
                    {
                        mapEditor.mapDataBinding.upgradeDataBindings[i].USEDEFAULT = (bool)DecodeValue(CodeType.Default, cm.GetCells(3, 3 + i));

                        mapEditor.mapDataBinding.upgradeDataBindings[i].BASEMIN = ushort.Parse(cm.GetCells(4, 3 + i));
                        mapEditor.mapDataBinding.upgradeDataBindings[i].BONUSMIN = ushort.Parse(cm.GetCells(5, 3 + i));
                        mapEditor.mapDataBinding.upgradeDataBindings[i].BASEGAS = ushort.Parse(cm.GetCells(6, 3 + i));
                        mapEditor.mapDataBinding.upgradeDataBindings[i].BONUSGAS = ushort.Parse(cm.GetCells(7, 3 + i));
                        mapEditor.mapDataBinding.upgradeDataBindings[i].BASETIME = ushort.Parse(cm.GetCells(8, 3 + i));
                        mapEditor.mapDataBinding.upgradeDataBindings[i].BONUSTIME = ushort.Parse(cm.GetCells(9, 3 + i));

                        mapEditor.mapDataBinding.upgradeDataBindings[i].DEFAULTSTARTLEVEL = byte.Parse(cm.GetCells(10, 3 + i));
                        mapEditor.mapDataBinding.upgradeDataBindings[i].DEFAULTMAXLEVEL = byte.Parse(cm.GetCells(11, 3 + i));

                        for (int j = 0; j < 12; j++)
                        {
                            int startplayer = int.Parse(cm.GetCells(12 + j * 2, 3 + i));
                            int maxplayer = int.Parse(cm.GetCells(13 + j * 2, 3 + i));

                            if(startplayer == -1 && maxplayer == -1)
                            {
                                //초기값 사용일 경우
                                mapEditor.mapDataBinding.upgradeDataBindings[i].playerbind[j].SetDefault(true);
                            }
                            else
                            {
                                //아닐 경우
                                mapEditor.mapDataBinding.upgradeDataBindings[i].playerbind[j].SetDefault(false);
                                mapEditor.mapDataBinding.upgradeDataBindings[i].playerbind[j].STARTLEVEL = (byte)startplayer;
                                mapEditor.mapDataBinding.upgradeDataBindings[i].playerbind[j].MAXLEVEL = (byte)maxplayer;
                            }
                        }
                    }

                    break;
                case ExcelType.Tech:
                    cm.ReadCells(17, 46);
                    for (int i = 0; i < 44; i++)
                    {
                        mapEditor.mapDataBinding.techDataBindings[i].USEDEFAULT = (bool)DecodeValue(CodeType.Default, cm.GetCells(3, 2 + i));

                        mapEditor.mapDataBinding.techDataBindings[i].MIN = ushort.Parse(cm.GetCells(4, 2 + i));
                        mapEditor.mapDataBinding.techDataBindings[i].GAS = ushort.Parse(cm.GetCells(5, 2 + i));
                        mapEditor.mapDataBinding.techDataBindings[i].BASETIME = ushort.Parse(cm.GetCells(6, 2 + i));
                        mapEditor.mapDataBinding.techDataBindings[i].ENERGY = ushort.Parse(cm.GetCells(7, 2 + i));

                        mapEditor.mapDataBinding.techDataBindings[i].UseDefaultCode = (MapDataBinding.UserDefaultCode)DecodeValue(CodeType.Tech, cm.GetCells(8, 2 + i));


                        for (int j = 0; j < 8; j++)
                        {
                            mapEditor.mapDataBinding.techDataBindings[i].UsePlayerCode[j] = (MapDataBinding.UserDefaultCode)DecodeValue(CodeType.Tech, cm.GetCells(9 + j, 2 + i));
                        }
                    }

                    break;
                case ExcelType.Sound:
                    int count = cm.ReadAllCell(2);
                    List<string> newchksounds = new List<string>();
                  
                    mapEditor.mapdata.WAV.Clear();
                    for (int i = 0; i < count; i++)
                    {
                        string filename = cm.GetCells(1, 2 + i);
                        string path = cm.GetCells(2, 2 + i);

                        if (string.IsNullOrEmpty(filename)) break;

                        if(path == "INCHK")
                        {
                            if(mapEditor.mapdata.soundDatas.Find(x => x.path == filename) == null)
                            {
                                throw new Exception(filename + "은 Chk안에 저장된 사운드가 아닙니다.");
                            }
                            newchksounds.Add(filename);
                            mapEditor.mapdata.WAV.Add(new StringData(mapEditor.mapdata, filename));
                        }
                        else if (path == "StarCraftSound")
                        {
                            mapEditor.mapdata.WAV.Add(new StringData(mapEditor.mapdata, filename));
                        }
                    }

                    //new에 없고 old에만 있는 파일 삭제
                    int index = 0;
                    for (int i = 0; i < mapEditor.mapdata.soundDatas.Count; i++)
                    {
                        if (!newchksounds.Contains(mapEditor.mapdata.soundDatas[index].path)){
                            //새로 리프레시될 목록에 사운드가 없을 경우 삭제
                            mapEditor.mapdata.soundDatas.RemoveAt(index);
                        }
                        else
                        {
                            index++;
                        }
                    }
                    Dictionary<string, string> soundpaths = new Dictionary<string, string>();

                    for (int i = 0; i < count; i++)
                    {
                        string filename = cm.GetCells(1, 2 + i);
                        string path = cm.GetCells(2, 2 + i);

                        if (string.IsNullOrEmpty(filename)) break;

                        if (path != "INCHK" && path != "StarCraftSound")
                        {
                            if (!File.Exists(path))
                            {
                                throw new Exception(path + "을 찾을 수 없습니다.");
                            }
                            soundpaths.Add(filename, path);
                        }
                    }
                  
                    if (soundpaths.Count != 0)
                    {
                        SoundImport soundImport = new SoundImport(mapEditor);
                        foreach (var item in soundpaths)
                        {
                            soundImport.LoadSound(item.Value, item.Key);
                        }
                        //soundImport.ShowDialog();
                    }
                    mapEditor.Scenario.soundSetting.RefreshListBox();
                    break;
                case ExcelType.Trigger:
                    break;
                case ExcelType.MissionBriefing:
                    break;
                case ExcelType.UnitLayout:
                    cm.ReadCells(17, 2 + UnitMaxCount);
                    mapEditor.mapdata.UNIT.Clear();
                    for (int i = 0; i < UnitMaxCount; i++)
                    {
                        if (string.IsNullOrEmpty( cm.GetCells(1, 2 + i))) break;

                        CUNIT cUNIT = new CUNIT();
                        cUNIT.SetMapEditor(mapEditor);
                        cUNIT.unitclass = uint.Parse(cm.GetCells(1, 2 + i));
                        cUNIT.unitID = ushort.Parse(cm.GetCells(3, 2 + i));
                        cUNIT.X = ushort.Parse(cm.GetCells(4, 2 + i));
                        cUNIT.Y = ushort.Parse(cm.GetCells(5, 2 + i));

                        cUNIT.player = byte.Parse(cm.GetCells(6, 2 + i));
                        cUNIT.hitPoints = byte.Parse(cm.GetCells(7, 2 + i));
                        cUNIT.shieldPoints = byte.Parse(cm.GetCells(8, 2 + i));
                        cUNIT.energyPoints = byte.Parse(cm.GetCells(9, 2 + i));
                        cUNIT.resoruceAmount = uint.Parse(cm.GetCells(10, 2 + i));
                        cUNIT.hangar = ushort.Parse(cm.GetCells(11, 2 + i));
                        cUNIT.stateFlag = ushort.Parse(cm.GetCells(12, 2 + i));
                        cUNIT.linkFlag = ushort.Parse(cm.GetCells(13, 2 + i));
                        cUNIT.linkedUnit = uint.Parse(cm.GetCells(14, 2 + i));
                        cUNIT.validstatusFlag = ushort.Parse(cm.GetCells(15, 2 + i));
                        cUNIT.validunitFlag = ushort.Parse(cm.GetCells(16, 2 + i));
                        cUNIT.unused = uint.Parse(cm.GetCells(17, 2 + i));


                        cUNIT.ImageReset();
                        mapEditor.mapdata.UNIT.Add(cUNIT);
                        //WindowTool.MapViewer.miniUnitUpdate(cUNIT);

                    }
                    mapEditor.IndexedUnitCancel();
                    mapEditor.MinimapUnitInitRefresh();
                    //mapEditor.ChangeMiniMap = true;
                    break;
                case ExcelType.DoodadLayout:
                    cm.ReadCells(6, 2 + DoodadMaxCount);
                    mapEditor.mapdata.DD2.Clear();
                    for (int i = 0; i < DoodadMaxCount; i++)
                    {
                        if (string.IsNullOrEmpty(cm.GetCells(2, 2 + i))) break;

                        CDD2 cDD2 = new CDD2(mapEditor.mapdata);

                        cDD2.ID = ushort.Parse(cm.GetCells(2, 2 + i));
                        cDD2.X = ushort.Parse(cm.GetCells(3, 2 + i));
                        cDD2.Y = ushort.Parse(cm.GetCells(4, 2 + i));
                        cDD2.PLAYER = byte.Parse(cm.GetCells(5, 2 + i));
                        cDD2.FLAG = byte.Parse(cm.GetCells(6, 2 + i));

                        mapEditor.mapdata.DD2.Add(cDD2);
                    }

                    break;
                case ExcelType.SpriteLayout:
                    cm.ReadCells(7, 2 + SpriteMaxCount);
                    mapEditor.mapdata.THG2.Clear();
                    for (int i = 0; i < SpriteMaxCount; i++)
                    {
                        if (string.IsNullOrEmpty(cm.GetCells(2, 2 + i))) break;

                        CTHG2 cTHG2 = new CTHG2();

                        cTHG2.ID = ushort.Parse(cm.GetCells(2, 2 + i));
                        cTHG2.X = ushort.Parse(cm.GetCells(3, 2 + i));
                        cTHG2.Y = ushort.Parse(cm.GetCells(4, 2 + i));
                        cTHG2.PLAYER = byte.Parse(cm.GetCells(5, 2 + i));
                        cTHG2.FLAG = ushort.Parse(cm.GetCells(6, 2 + i));
                        cTHG2.UNUSED = byte.Parse(cm.GetCells(7, 2 + i));

                        mapEditor.mapdata.THG2.Add(cTHG2);
                    }

                    break;
                case ExcelType.LocationLayout:
                    cm.ReadCells(7, 2 + LocationMaxCount);
                    mapEditor.mapdata.LocationReset();
                    for (int i = 1; i < LocationMaxCount; i++)
                    {
                        if (string.IsNullOrEmpty(cm.GetCells(1, 1 + i))) continue;

                        int locindex = int.Parse(cm.GetCells(1, 1 + i));
                        string locname= cm.GetCells(2, 1 + i);
                        if (mapEditor.mapdata.IsLocationExist(locindex))
                        {
                            throw new Exception("로케이션 " + locname + ":" + locindex + "가 중복되었습니다.");
                        }


                        LocationData locationData = new LocationData(mapEditor);


                        locationData.INDEX = locindex;


                        locationData.NAME = locname;
                        locationData.X = uint.Parse(cm.GetCells(3, 1 + i));
                        locationData.Y = uint.Parse(cm.GetCells(4, 1 + i));
                        locationData.WIDTH = int.Parse(cm.GetCells(5, 1 + i));
                        locationData.HEIGHT = int.Parse(cm.GetCells(6, 1 + i));
                        locationData.FLAG = ushort.Parse(cm.GetCells(7, 1 + i));

                        mapEditor.mapdata.AddLocation(locationData);
                    }

                    break;
                case ExcelType.TileLayout:
                    cm.ReadCells(mapEditor.mapdata.WIDTH, mapEditor.mapdata.HEIGHT);
                    for (int y = 0; y < mapEditor.mapdata.HEIGHT; y++)
                    {
                        for (int x = 0; x < mapEditor.mapdata.WIDTH; x++)
                        {
                            mapEditor.mapdata.TILE[x + y * mapEditor.mapdata.WIDTH] = ushort.Parse(cm.GetCells(1 + x, 1 + y));
                        }
                    }

                    break;
                case ExcelType.FogLayout:
                    cm.ReadCells(mapEditor.mapdata.WIDTH, mapEditor.mapdata.HEIGHT);
                    for (int y = 0; y < mapEditor.mapdata.HEIGHT; y++)
                    {
                        for (int x = 0; x < mapEditor.mapdata.WIDTH; x++)
                        {
                            mapEditor.mapdata.MASK[x + y * mapEditor.mapdata.WIDTH] = byte.Parse(cm.GetCells(1 + x, 1 + y));
                        }
                    }

                    break;
                case ExcelType.Code:
                    //코드데이터 먼저 읽어오기   9 / 10
                    Dictionary<ushort, DoodadPallet> t = Global.WindowTool.MapViewer.tileSet.DoodadPallets[mapEditor.mapdata.TILETYPE];
                    cm.BeginUpdate();
                    int cindex = 0;
                    foreach (var item in t)
                    {
                        cm.AddCells(10, 3 + cindex, item.Key.ToString());
                        cm.AddCells(11, 3 + cindex, item.Value.tblString + "#" + (cindex + 1));
                        cindex++;
                    }

                    cindex = 0;
                    foreach (var item in mapEditor.mapDataBinding.spriteDataBindings)
                    {
                        cm.AddCells(12, 3 + cindex, cindex.ToString());
                        cm.AddCells(13, 3 + cindex, item.MainName);
                        cindex++;
                    }


                    cm.EndUpdate();
                    break;
            }





            ReleaseExcelObject(ws);
        }

        public bool LoadExcel(string filename, ExcelType excelType)
        {
            Application excelApp = null;
            Workbook template = null;
            Workbook loaded = null;

            try
            {
                // Excel 첫번째 워크시트 가져오기                
                excelApp = new Application();
                excelApp.DisplayAlerts = false;
                template = excelApp.Workbooks.Open(baseExcelPath, Type.Missing, true);
                loaded = excelApp.Workbooks.Open(filename, Type.Missing, true);

                GetWorksheetData(ExcelType.Code, template);

                List<string> sheetlist = new List<string>();


                foreach (Worksheet sheet in loaded.Sheets)
                {
                    sheetlist.Add(sheet.Name);
                }


                if (excelType == ExcelType.All)
                {
                    foreach (ExcelType item in Enum.GetValues(typeof(ExcelType)))
                    {
                        if (item != ExcelType.All && item != ExcelType.Code)
                        {
                            if (sheetlist.Contains(item.ToString()))
                            {
                                GetWorksheetData(item, loaded);
                            }
                        }
                    }
                }
                else
                {
                    GetWorksheetData(excelType, loaded);
                }


                //((Worksheet)wb.Worksheets.Item[1]).Delete();

                //ws = wb.Worksheets.get_Item(1) as Worksheet;

                // 데이타 넣기

                object misValue = System.Reflection.Missing.Value;
                // 엑셀파일 저장
                template.Close(false, misValue, misValue);
                loaded.Close(false, misValue, misValue);
                excelApp.Quit();
            }
            catch (Exception e)
            {
                MsgDialog msgDialog = new MsgDialog("엑셀 불러오기에 실패했습니다.\n" + e.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                msgDialog.ShowDialog();
            }
            finally
            {
                // Clean up
                ReleaseExcelObject(template);
                ReleaseExcelObject(loaded);
                ReleaseExcelObject(excelApp);
            }

            return true;
        }
    }

    public class CellManager
    {
        Worksheet ws;

        public CellManager(Worksheet ws)
        {
            this.ws = ws;
        }

        public int ReadAllCell(int width, int size = 1024)
        {
            cells.Clear();

            int currentheight = 1;
            int maxheight = 1;
            while (true)
            {
                //For populating only a single row with 'n' no. of columns.
                var startCell = (Range)ws.Cells[currentheight, 1];

                //For 2d data, with 'n' no. of rows and columns.
                var endCell = (Range)ws.Cells[currentheight + size, width];
                var writeRange = ws.Range[startCell, endCell];

                object[,] obj = writeRange.Value;

                for (int y = currentheight; y <= currentheight + size; y++)
                {
                    for (int x = 1; x <= width; x++)
                    {
                        if (obj[y, x] != null)
                        {
                            cells.Add(new Vector(x, y), obj[y, x].ToString());
                            if(y > maxheight)
                            {
                                maxheight = y;
                            }
                        }
                    }
                }
                currentheight += size;


                if(cells.ContainsKey(new Vector(1, currentheight)))
                {
                    if (string.IsNullOrEmpty(cells[new Vector(1, currentheight)])) break;
                }
                else
                {
                    //데이터가 끝남
                    return maxheight - 1;
                }
            }

            return maxheight - 1;
        }

        public void ReadCells(int width, int height)
        {
            cells.Clear();


            //For populating only a single row with 'n' no. of columns.
            var startCell = (Range)ws.Cells[1, 1];
            //For 2d data, with 'n' no. of rows and columns.
            var endCell = (Range)ws.Cells[height, width];
            var writeRange = ws.Range[startCell, endCell];

            object[,] obj = writeRange.Value;

            for (int y = 1; y <= height; y++)
            {
                for (int x = 1; x <= width; x++)
                {
                    if(obj[y, x] != null)
                    {
                        cells.Add(new Vector(x, y), obj[y, x].ToString());
                    }
                }
            }
        }

        public string GetCells(int x, int y)
        {
            if(cells.ContainsKey(new Vector(x, y)))
            {
                return cells[new Vector(x, y)];
            }

            return "";
        }



        private Dictionary<Vector, string> cells = new Dictionary<Vector, string>();
        public void BeginUpdate()
        {
            cells.Clear();
        }

        public void AddCells(int x, int y, string value)
        {
            cells.Add(new Vector(x, y), value);
        }

        public void EndUpdate()
        {
            if (cells.Count == 0) return;

            Vector min = new Vector(int.MaxValue, int.MaxValue);
            Vector max = new Vector(int.MinValue, int.MinValue);

            foreach (var item in cells)
            {
                if (item.Key.X < min.X) min.X = item.Key.X;
                if (item.Key.Y < min.Y) min.Y = item.Key.Y;
                if (item.Key.X > max.X) max.X = item.Key.X;
                if (item.Key.Y > max.Y) max.Y = item.Key.Y;
            }

            object[,] objects = new object[(int)(max.Y - min.Y + 1), (int)(max.X - min.X + 1)];


            foreach (var item in cells)
            {
                objects[(int)(item.Key.Y - min.Y), (int)(item.Key.X - min.X)] = item.Value;
            }


            SetCells((int)min.X, (int)min.Y, (int)max.X, (int)max.Y, objects);
        }



        private void SetCells(int startCol, int startRow, int endCol, int endRow, object[,] data)
        {
            int row, col;
            row = endRow - startRow;
            col = endCol - startCol;

            //For populating only a single row with 'n' no. of columns.
            var startCell = (Range)ws.Cells[startRow, startCol];
            //For 2d data, with 'n' no. of rows and columns.
            var endCell = (Range)ws.Cells[endRow, endCol];
            var writeRange = ws.Range[startCell, endCell];
            writeRange.Value2 = data;
        }

        public void SetCells(int x, int y, string value)
        {
            Range cell1 = ws.Cells[y, x];
            cell1.Value = value;
            ExcelData.ReleaseExcelObject(cell1);
        }


    }
}
