using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UseMapEditor.Control;

namespace UseMapEditor.Global
{
    public static class WindowTool
    {
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
                programStart.Close();
            }
        }





        private static Grid currentView;
        private static MapEditor currentMapEditor;
        public static MonoGameControl.MapDrawer MapViewer;
        public static bool GrpLoadCmp = false;


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


            if(strlist.Count > 4)
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
                
                //배경 지우기
                //currentMapEditor.MapTrace.Source = null;

                if(MapViewer.Parent != null)
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
