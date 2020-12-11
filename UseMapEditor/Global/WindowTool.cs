﻿using Microsoft.Win32;
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
        private static Grid currentView;
        private static MapEditor currentMapEditor;
        public static MonoGameControl.MapDrawer MapViewer;
        public static bool GrpLoadCmp = false;




        public static bool LoadGrp()
        {
            MapViewer = new MonoGameControl.MapDrawer();
            return true;
        }


        public static List<MapEditor> GetMapEditorList()
        {
            List<MapEditor> mapEditors = new List<MapEditor>();

            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType().Equals(typeof(MainWindow)))
                {
                    MainWindow mainWindow = (MainWindow)window;
                    for (int i = 0; i < mainWindow.MainTab.Items.Count; i++)
                    {
                        mapEditors.Add((MapEditor)((TabItem)mainWindow.MainTab.Items[i]).Content);
                    }
                }
            }





            return mapEditors;
        }




        public static string OpenMap()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "맵 파일|*.scx;*.scp";

            if ((bool)openFileDialog.ShowDialog())
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

                MapViewer.ChangeMap(mapEditor);
                MapGrid.Children.Add(MapViewer);
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
