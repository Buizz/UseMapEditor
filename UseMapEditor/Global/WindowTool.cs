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
        private static MonoGameControl.MapDrawer MapViewer = new MonoGameControl.MapDrawer();



        public static void ChangeView(MapEditor mapEditor)
        {
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
                    currentView.Children.Remove(MapViewer);
                }



                currentMapEditor = mapEditor;
                currentView = MapGrid;
                
                //배경 지우기
                currentMapEditor.MapTrace.Source = null;

                MapViewer.ChangeMap(mapEditor.mapdata);
                MapGrid.Children.Add(MapViewer);
            }
        }



        private static ImageSource SaveControlImage(FrameworkElement control)
        {
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

            // Make a bitmap and draw on it.
            int width = (int)control.ActualWidth;
            int height = (int)control.ActualHeight;
            RenderTargetBitmap rtb = new RenderTargetBitmap(
                width, height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(dv);
            rtb.Freeze();
            return rtb;
            //bg.Source = rtb;
        }
    }
}
