using Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using UseMapEditor.Control;
using UseMapEditor.FileData;
using UseMapEditor.Lua;

namespace UseMapEditor.Global
{
    public static class WindowTool
    {
        public static TriggerManger triggerManger = new TriggerManger();
        public static LuaManager lua = new LuaManager();


        public static string[] unitgroup = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"\Data\Group\Unit.txt");
        public static string[] upgradegroup = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"\Data\Group\Upgrade.txt");
        public static string[] techgroup = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"\Data\Group\Tech.txt");
        public static string[] spritegroup = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"\Data\Group\Sprite.txt");

        public static string[] imagename = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"\Data\Images.txt");

        public static string[] soundlist = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"\Data\SoundList.txt");

        public static BitmapSource GetBitmapSourceFromStream(System.IO.Stream stream)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();

            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = stream;

            bitmapImage.EndInit();
            bitmapImage.Freeze();

            BitmapSource bitmapSource = new FormatConvertedBitmap(bitmapImage, PixelFormats.Pbgra32, null, 0);
            WriteableBitmap writeableBitmap = new WriteableBitmap(bitmapSource);

            int width = writeableBitmap.PixelWidth;
            int height = writeableBitmap.PixelHeight;

            int[] pixelArray = new int[width * height];
            int stride = writeableBitmap.PixelWidth * (writeableBitmap.Format.BitsPerPixel / 8);
            writeableBitmap.CopyPixels(pixelArray, stride, 0);
            writeableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixelArray, stride, 0);
            bitmapImage = null;
            return writeableBitmap;
        }



        public static System.Drawing.Bitmap GetBitmapFromBitmapSource(BitmapSource bitmapSource)
        {
            System.Drawing.Bitmap bitmap;


            using (MemoryStream memoryStream = new MemoryStream())
            {
                BitmapEncoder bitmapEncoder = new BmpBitmapEncoder();
                bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                bitmapEncoder.Save(memoryStream);


                bitmap = new System.Drawing.Bitmap(memoryStream);
            }


            return bitmap;
        }

        public static BitmapSource GetBitmapSourceFromBitmap(System.Drawing.Bitmap bitmap)
        {
            BitmapSource bitmapSource;


            IntPtr hBitmap = bitmap.GetHbitmap();
            BitmapSizeOptions sizeOptions = BitmapSizeOptions.FromEmptyOptions();
            bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, sizeOptions);
            bitmapSource.Freeze();


            return bitmapSource;
        }


        public static Dialog.ProgramStart programStart;
        public static void CloseProgram()
        {
            int MainCount = 0;

            foreach(Window window in Application.Current.Windows)
            {
                if (window.GetType().Equals(typeof(MainWindow)))
                {
                    MainCount++;
                }
            }
            if(MainCount == 0)
            {
                string foldername = AppDomain.CurrentDomain.BaseDirectory + @"Data\Excel";
                if(Directory.Exists(foldername))
                {
                    foreach (var item in Directory.GetFiles(foldername))
                    {
                        try
                        {
                            File.Delete(item);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
             

                excelProcessExcute.Close();
                excelProcessExcute = null;
                programStart.Close();
                Application.Current.Shutdown(); 
            }
        }




        public static ExcelProcessExcute excelProcessExcute = new ExcelProcessExcute();
        public static void NewExcelProcess(MapEditor mapEditor, string fname)
        {
            excelProcessExcute.AddProcess(mapEditor, fname);
        }





        public static Random random = new Random();



        public static SCData scdata;
        public static Iscript iscript;
        public static Iscript iscriptX;


        public static tblreader stat_txt;
        public static tblreader stat_txt_kor_eng;
        public static tblreader stat_txt_kor_kor;
        public static string GetStat_txt(int index)
        {
            if(index == -1)
            {
                return "없음";
            }
            switch (Global.Setting.Vals[Setting.Settings.language_StatLan])
            {
                case "stat_txt":
                    return stat_txt.Strings[index].val1;
                case "stat_txt_kor_eng":
                    return stat_txt_kor_eng.Strings[index].val1;
                case "stat_txt_kor_kor":
                    return stat_txt_kor_kor.Strings[index].val1;
            }
            return stat_txt.Strings[index].val1;
        }

        public static string GetEngStat_txt(int index)
        {
            if (index == -1)
            {
                return "None";
            }

            string[] s = stat_txt.Strings[index].val2.Split('|');

            string rstr = stat_txt.Strings[index].val1;
            if(s[1] != "*")
            {
                rstr += " (" + s[1] + ")";
            }

            return rstr;
        }



        private static Grid currentView;
        private static MapEditor currentMapEditor;
        public static MonoGameControl.MapDrawer MapViewer;
        public static bool GrpLoadCmp = false;

        public static bool IsEnabledMapEditor(MapEditor mapEditor)
        {
            return currentMapEditor == mapEditor;
        }



        public static string OpenedFilePath;

        public static bool LoadGrp()
        {
            MapViewer = new MonoGameControl.MapDrawer();
            GrpLoadCmp = true;
            return true;
        }


        public static void AddLastOpenFile(string filepath)
        {
            List<string> strlist = new List<string>();

            strlist.AddRange(Properties.Settings.Default.LastOpend.Split('|'));


            if(strlist.Last() == filepath)
            {
                return;
            }

            if (strlist.Contains(filepath))
            {
                strlist.Remove(filepath);
            }
            if (strlist.Count > 10)
            {
                strlist.RemoveAt(0);

            }
            strlist.Add(filepath);



            Properties.Settings.Default.LastOpend = String.Join("|", strlist.ToArray());
        }
        public static string[] GetLastOpenfile()
        {
            return Properties.Settings.Default.LastOpend.Split('|');
        }












        public static List<MapEditor> GetMapEditorList()
        {
            List<MapEditor> mapEditors = new List<MapEditor>();

            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType().Equals(typeof(MainWindow)))
                {
                    MainWindow mainWindow = (MainWindow)window;
              
                }
            }





            return mapEditors;
        }




        public static System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
        public static string OpenMap()
        {
            openFileDialog.Filter = "맵 파일|*.scx;*.scp";



            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return openFileDialog.FileName;
            }
            else
            {
                return "";
            }
        }


        public static void ChangeView(MapEditor mapEditor, bool IsClose)
        {
            if (!GrpLoadCmp)
            {
                return;
            }


            if(MapViewer == null)
            {
                return;
            }


            if (IsClose)
            {
                List<MapEditor> mapEditors = GetMapEditorList();

                mapEditors.Remove(mapEditor);
                if(mapEditors.Count != 0)
                {
                    mapEditor = mapEditors.First();
                }
                else
                {
                    return;
                }


                //if (currentView != null)
                //{
                //    currentView.Children.Clear();
                //}
                //if (MapViewer.Parent != null)
                //{
                //    Grid grid = (Grid)MapViewer.Parent;
                //    grid.Children.Clear();
                //}
                //currentMapEditor = null;
                //currentView = null;
            }




            Grid MapGrid = mapEditor.MapViewer;
            if (MapGrid != currentView)
            {
                //다른 오브젝트일 경우
                //현재 이미지를 저장
                if (currentMapEditor != null)
                {
                    currentMapEditor.MapTrace.Source = SaveControlImage(MapViewer);
                }


                //비우기
                if (currentView != null)
                {
                    currentView.Children.Clear();
                }



                currentMapEditor = mapEditor;
                currentView = MapGrid;
                mapEditor.TileMapRefresh();

                //배경 지우기
                //currentMapEditor.MapTrace.Source = null;

                if (MapViewer.Parent != null)
                {
                    Grid pgrid = (Grid)MapViewer.Parent;
                    pgrid.Children.Clear();
                }




                MapViewer.ChangeMap(mapEditor);
                MapGrid.Children.Add(MapViewer);
                MapViewer.IsEnabled = true;
            }
        }




        private static ImageSource SaveControlImage(FrameworkElement control)
        {
            // Make a bitmap and draw on it.
            int width = (int)control.ActualWidth;
            int height = (int)control.ActualHeight;

            if (width == 0 || height == 0)
            {
                return null;
            }


            // Get the size of the Visual and its descendants.
            Rect rect = VisualTreeHelper.GetDescendantBounds(control);

            // Make a DrawingVisual to make a screen
            // representation of the control.
            DrawingVisual dv = new DrawingVisual();

            // Fill a rectangle the same size as the control
            // with a brush containing images of the control.
            using (DrawingContext ctx = dv.RenderOpen())
            {
                VisualBrush brush = new VisualBrush(control);
                ctx.DrawRectangle(brush, null, new Rect(rect.Size));
            }



            RenderTargetBitmap rtb = new RenderTargetBitmap(
                width, height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(dv);
            rtb.Freeze();
            return rtb;
            //bg.Source = rtb;
        }
    }
}
