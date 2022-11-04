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
using FastExcel;

namespace UseMapEditor.FileData
{
    public class ExcelData
    {
        MapEditor mapEditor;
        string baseExcel = AppDomain.CurrentDomain.BaseDirectory + @"Data\StarCraftExcelTemplate.xlsx";


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
            TileLayout,
            UnitLayout,
            DoodadLayout,
            SpriteLayout,
            LocationLayout,
            FogLayout
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



        private void ReleaseExcelObject(object obj)
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
            catch (Exception ex)
            {
                obj = null;
                throw ex;
            }
            finally
            {
                GC.Collect();
            }
        }




        public bool SaveExcel(string finename, ExcelType excelType)
        {
            FastExcel.FastExcel fe = new FastExcel.FastExcel(new FileStream(baseExcel, FileMode.Open));



            fe.Dispose();



            //Application excelApp = null;
            //Workbook template = null;

            //try
            //{
            //    // Excel 첫번째 워크시트 가져오기                
            //    excelApp = new Application();
            //    excelApp.DisplayAlerts = false;
            //    template = excelApp.Workbooks.Open(baseExcel, Type.Missing, true);


            //    if (excelType == ExcelType.All)
            //    {
            //        foreach (ExcelType item in Enum.GetValues(typeof(ExcelType)))
            //        {
            //            if (item != ExcelType.All)
            //            {
            //                SetWorksheetData(item, template);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        SetWorksheetData(excelType, template);


            //        foreach (ExcelType item in Enum.GetValues(typeof(ExcelType)))
            //        {
            //            if (item != excelType && item != ExcelType.All)
            //            {

            //                template.Worksheets.Item[item.ToString()].Delete();
            //            }
            //        }
            //    }





            //    //((Worksheet)wb.Worksheets.Item[1]).Delete();

            //    //ws = wb.Worksheets.get_Item(1) as Worksheet;

            //    // 데이타 넣기

            //    if (File.Exists(finename)) File.Delete(finename);
            //    object misValue = System.Reflection.Missing.Value;
            //    // 엑셀파일 저장
            //    template.SaveAs(finename);
            //    template.Close(false, misValue, misValue);
            //    excelApp.Quit();
            //}
            //finally
            //{
            //    // Clean up
            //    ReleaseExcelObject(template);
            //    ReleaseExcelObject(excelApp);
            //}



            return true;
        }


        string[] ownerlist = { "사용안함", "구조가능", "컴퓨터", "사람", "중립" };
        string[] racelist = { "저그","테란","프로토스","유저선택","비활성"};
        string[] colorlist = { "빨강","파랑","청록","보라","주황","갈색","흰색","노랑","초록","옅은노랑","황갈색","중립","담록","푸른회색","시안","분홍","황록","라임","남색","자홍","회색","검정","랜덤","플레이어선택","커스텀RGB"};

        //public void SetWorksheetData(ExcelType excelType, Workbook wb)
        //{
        //    Worksheet ws = wb.Worksheets.Item[excelType.ToString()];
        //    CellManager cm = new CellManager(ws);

        //    switch (excelType)
        //    {
        //        case ExcelType.Map:
        //            {
        //                /*
        //                * 지형   2,1
        //                * 크기X  2,2
        //                * 크기Y  3,2
        //                * 제목   1,4
        //                * 설명   1,7
        //                */
        //                string tileset = ((UseMapEditor.FileData.TileSet.TileType)mapEditor.mapDataBinding.TileSet).ToString();
        //                string width = mapEditor.mapDataBinding.WIDTH.ToString();
        //                string height = mapEditor.mapDataBinding.HEIGHT.ToString();
        //                string title = mapEditor.mapDataBinding.Title;
        //                string infor = mapEditor.mapDataBinding.Description;

        //                cm.SetCells(2, 1, tileset);
        //                cm.SetCells(2, 2, width);
        //                cm.SetCells(3, 2, height);
        //                cm.SetCells(1, 4, title);
        //                cm.SetCells(1, 7, infor);
        //            }
        //            break;
        //        case ExcelType.Player:
        //            /*
        //            * 제어    2,2 + i
        //            * 종족    3,2 + i
        //            * 색상    4,2 + i
        //            * RGB코드 5,2 + i
        //            */
        //            for (int i = 0; i < 8; i++)
        //            {
        //                string control = ownerlist[mapEditor.mapDataBinding.playerBindings[i].Owner];
        //                string race = racelist[mapEditor.mapDataBinding.playerBindings[i].Race];
        //                string color = colorlist[mapEditor.mapDataBinding.playerBindings[i].Color];
        //                string rgbcode = mapEditor.mapDataBinding.playerBindings[i].IconColor.ToString();

        //                cm.SetCells(2, 2 + i, control);
        //                cm.SetCells(3, 2 + i, race);
        //                cm.SetCells(4, 2 + i, color);
        //                cm.SetCells(5, 2 + i, rgbcode);
        //            }

        //            break;
        //        case ExcelType.Force:
        //            /*
        //            * 세력        2,2 + i
        //            * 세력이름    2,11 + i
        //            */

        //            for (int i = 0; i < 8; i++)
        //            {
        //                string playerForce = "세력 " + (mapEditor.mapdata.FORCE[i] + 1).ToString();

        //                cm.SetCells(2, 2 + i, playerForce);
                        
        //            }
        //            for (int i = 0; i < 4; i++)
        //            {
        //                string forcename = mapEditor.mapDataBinding.forceBindings[i].ForceName;

        //                string forceAllied = mapEditor.mapDataBinding.forceBindings[i].Allied ? "시용": "사용안함";
        //                string forceVictory = mapEditor.mapDataBinding.forceBindings[i].AlliedVictory ? "시용" : "사용안함";
        //                string forceVision = mapEditor.mapDataBinding.forceBindings[i].ShareVision ? "시용" : "사용안함";
        //                string forceRandomize = mapEditor.mapDataBinding.forceBindings[i].Randomize ? "시용" : "사용안함";

        //                cm.SetCells(2, 11 + i, forcename);

        //                cm.SetCells(3, 11 + i, forceAllied);
        //                cm.SetCells(4, 11 + i, forceVictory);
        //                cm.SetCells(5, 11 + i, forceVision);
        //                cm.SetCells(6, 11 + i, forceRandomize);
        //            }
        //            break;
        //        case ExcelType.Unit:
        //            for (int i = 0; i < 228; i++)
        //            {
        //                string usedefault = mapEditor.mapDataBinding.unitdataBindings[i].USEDEFAULT ? "수정" : "기본";

        //                string unitname = mapEditor.mapDataBinding.unitdataBindings[i].SecondName;
        //                string hp = mapEditor.mapDataBinding.unitdataBindings[i].HIT;
        //                string shild = mapEditor.mapDataBinding.unitdataBindings[i].SHIELD.ToString();
        //                string armor = mapEditor.mapDataBinding.unitdataBindings[i].ARMOR.ToString();
        //                string time = mapEditor.mapDataBinding.unitdataBindings[i].BUILDTIME.ToString();
        //                string min = mapEditor.mapDataBinding.unitdataBindings[i].MIN.ToString();
        //                string gas = mapEditor.mapDataBinding.unitdataBindings[i].GAS.ToString();
        //                string ground = mapEditor.mapDataBinding.unitdataBindings[i].GDMG.ToString();
        //                string groundplus = mapEditor.mapDataBinding.unitdataBindings[i].GBDMG.ToString();
        //                string air = mapEditor.mapDataBinding.unitdataBindings[i].ADMG.ToString();
        //                string airplus = mapEditor.mapDataBinding.unitdataBindings[i].ABDMG.ToString();

        //                string buliddefault;



        //                cm.SetCells(3, 2 + i, usedefault);

        //                cm.SetCells(4, 2 + i, unitname);
        //                cm.SetCells(5, 2 + i, hp);
        //                cm.SetCells(6, 2 + i, shild);
        //                cm.SetCells(7, 2 + i, armor);
        //                cm.SetCells(8, 2 + i, time);
        //                cm.SetCells(9, 2 + i, min);
        //                cm.SetCells(10, 2 + i, gas);
        //                cm.SetCells(11, 2 + i, ground);
        //                cm.SetCells(12, 2 + i, groundplus);
        //                cm.SetCells(13, 2 + i, air);
        //                cm.SetCells(14, 2 + i, airplus);

        //            }

        //            break;
        //        case ExcelType.Upgrage:
        //            break;
        //        case ExcelType.Tech:
        //            break;
        //        case ExcelType.Sound:
        //            break;
        //        case ExcelType.Trigger:
        //            break;
        //        case ExcelType.MissionBriefing:
        //            break;
        //        case ExcelType.TileLayout:
        //            break;
        //        case ExcelType.UnitLayout:
        //            break;
        //        case ExcelType.DoodadLayout:
        //            break;
        //        case ExcelType.SpriteLayout:
        //            break;
        //        case ExcelType.LocationLayout:
        //            break;
        //        case ExcelType.FogLayout:
        //            break;
        //    }





        //    ReleaseExcelObject(ws);
        //}


        //public void GetWorksheetData(ExcelType excelType, Workbook wb)
        //{
       
        //}

        //public bool LoadExcel(string filename)
        //{
        //    return true;
        //}
    }

    public class CellManager
    {
        //Worksheet ws;

        //public CellManager(Worksheet ws)
        //{
        //    this.ws = ws;
        //}

        //public void SetCells(int x, int y, string value)
        //{
        //    Range cell1 = ws.Cells[y, x];
        //    cell1.Value = value;
        //}

        //public string GetCellString(int x, int y)
        //{
        //    Range cell1 = ws.Cells[y, x];
        //    return cell1.Value;
        //}
        //public long GetCellNumber(int x, int y)
        //{
        //    Range cell1 = ws.Cells[y, x];
        //    return cell1.Value;
        //}
    }
}
