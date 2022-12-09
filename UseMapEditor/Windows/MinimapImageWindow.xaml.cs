using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static UseMapEditor.FileData.ExcelData;
using UseMapEditor.FileData;
using Microsoft.Win32;
using UseMapEditor.Control;
using ControlzEx.Standard;
using System.Runtime.CompilerServices;
using UseMapEditor.Global;
using System.Drawing;

namespace UseMapEditor.Windows
{
    /// <summary>
    /// MinimapImageWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MinimapImageWindow : MetroWindow
    {
        private MapEditor mapEditor;

        double dpi = 96;
        private string ImagePath;
        BitmapImage orgbitmap;
        BitmapImage palbitmap;
        public MinimapImageWindow(MapEditor mapEditor)
        {
            InitializeComponent();

            this.mapEditor = mapEditor;
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image|*.png|Image|*.*";

            if ((bool)openFileDialog.ShowDialog())
            {
                OkayBtn.IsEnabled = true;
                imagepathtb.Text = openFileDialog.FileName;
                OpenImage(openFileDialog.FileName);
            }
        }

        private void OpenImage(string imgpath)
        {
            ImagePath = imgpath;

            // BitmapImage.UriSource must be in a BeginInit/EndInit block
            orgbitmap = new BitmapImage();
            orgbitmap.BeginInit();
            orgbitmap.UriSource = new Uri(imgpath);

            // To save significant application memory, set the DecodePixelWidth or
            // DecodePixelHeight of the BitmapImage value of the image source to the desired
            // height or width of the rendered image. If you don't do this, the application will
            // cache the image as though it were rendered as its normal size rather then just
            // the size that is displayed.
            // Note: In order to preserve aspect ratio, set DecodePixelWidth
            // or DecodePixelHeight but not both.
            //myBitmapImage.DecodePixelWidth = 200;
            orgbitmap.EndInit();
            orgbitmap.Freeze();

            orgwidth = orgbitmap.PixelWidth;
            orgheight = orgbitmap.PixelHeight;
            double orgmax = Math.Max(orgwidth, orgheight);
            orgWidth.Text = orgwidth.ToString();
            orgHeight.Text = orgheight.ToString();


            int mapwidth = mapEditor.mapdata.WIDTH;
            int mapheight = mapEditor.mapdata.HEIGHT;
            double mapmax = Math.Max(mapwidth, mapheight);

            rwidth = (int)(orgwidth / orgmax * mapmax);
            rheight = (int)(orgheight / orgmax * mapmax);
            if (rwidth == 0 || rheight == 0)
            {
                return;
            }
            afterWidth.Text = rwidth.ToString();
            afterHeight.Text = rheight.ToString();
            afterWidth.IsReadOnly = false;
            afterHeight.IsReadOnly = false;

            orgImage.Source = orgbitmap;

         

            afterimage.Source = GetminimapImage();
            IsLodingCmp = true;
        }

        int orgwidth;
        int orgheight;
        int rwidth;
        int rheight;


        private BitmapSource GetminimapImage()
        {
            Bitmap bitmap = WindowTool.GetBitmapFromBitmapSource(orgbitmap);


            Bitmap rbmp = new Bitmap(rwidth, rheight);
            Graphics g = Graphics.FromImage(rbmp);
            g.DrawImage(bitmap,new RectangleF(0, 0, rwidth, rheight));

            for (int y = 0; y < rheight; y++)
            {
                for (int x = 0; x < rwidth; x++)
                {
                    Color color = rbmp.GetPixel(x, y);

                    ushort mtxt;
                    Color ncolor = WindowTool.MapViewer.tileSet.GetMiniMapMTXM(MapEditor.DrawType.SD, mapEditor.mapdata.TILETYPE, color, out mtxt);

                    
                    rbmp.SetPixel(x, y, ncolor);
                }
            }


            return WindowTool.GetBitmapSourceFromBitmap(rbmp);
        }

        bool IsLodingCmp = false;
        private void afterWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsLodingCmp)
            {
                IsLodingCmp = false;

                int w;
                if(!int.TryParse(afterWidth.Text, out w))
                {
                    IsLodingCmp = true;
                    return;
                }
                if (w > 256)
                {
                    IsLodingCmp = true;
                    return;
                }
                rwidth = w;
                rheight = (int)((double)orgheight / orgwidth * w);
                afterHeight.Text = rheight.ToString();



                if (rwidth == 0 || rheight == 0)
                {
                    IsLodingCmp = true;
                    return;
                }
                afterimage.Source = GetminimapImage();
                IsLodingCmp = true;
            }

        }
        private void afterHeight_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsLodingCmp)
            {
                IsLodingCmp = false;

                int h;
                if (!int.TryParse(afterHeight.Text, out h))
                {
                    IsLodingCmp = true;
                    return;
                }
                if (h > 256)
                {
                    IsLodingCmp = true;
                    return;
                }
                rwidth = (int)((double)orgwidth / orgheight * h);
                rheight = h;
                afterWidth.Text = rwidth.ToString();



                if (rwidth == 0 || rheight == 0)
                {
                    IsLodingCmp = true;
                    return;
                }
                afterimage.Source = GetminimapImage();
                IsLodingCmp = true;
            }
        }



        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            


            Close();
        }
    }
}
