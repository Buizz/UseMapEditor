using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UseMapEditor.Dialog;
using UseMapEditor.FileData;

namespace UseMapEditor.Windows
{
    /// <summary>
    /// Preprocessing.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Preprocessing : MetroWindow
    {
        public bool IsClose;

        BackgroundWorker worker;
        public Preprocessing()
        {
            InitializeComponent();

            LoadStart();
        }

        private void LoadStart()
        {
            worker = new BackgroundWorker();

            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.WorkerReportsProgress = true;


            worker.RunWorkerAsync();
        }



        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //1008
            progres.Value = ((double)e.ProgressPercentage / (double)1308) * 100;
            //throw new NotImplementedException();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(e.Error == null)
            {
                IsClose = true;
            }
            else
            {
                MsgDialog msgDialog = new MsgDialog("다음과 같은 오류로 그래픽을 불러오지 못했습니다.\n다시 시도하겠습니까?\n" + e.Error.Message, MessageBoxButton.OKCancel, MessageBoxImage.Error);
                msgDialog.ShowDialog();

                if(msgDialog.msgresult == MessageBoxResult.OK)
                {
                    LoadStart();
                    return;
                }

            }

            Close();
        }


        private void SaveToFile(byte[] bytes, string filename)
        {
            string[] paths = filename.Split('/');



            string currentPath = AppDomain.CurrentDomain.BaseDirectory + @"\CascData";


            foreach (string t in paths)
            {
                if (!System.IO.Directory.Exists(currentPath))
                {
                    System.IO.Directory.CreateDirectory(currentPath);
                }
                currentPath = currentPath + "\\" + t;
            }

            System.IO.File.WriteAllBytes(currentPath, bytes);
        }

        private bool SaveFromAnim(byte[] bytes, string filename, int img = -1)
        {
            string[] paths = filename.Split('/');



            string currentPath = AppDomain.CurrentDomain.BaseDirectory + @"CascData";
            string savePath = AppDomain.CurrentDomain.BaseDirectory + @"CascData\" + filename.Replace("/","\\");


            for (int i = 0; i < paths.Length; i++)
            {
                string map = paths[i];

                if (!System.IO.Directory.Exists(currentPath))
                {
                    System.IO.Directory.CreateDirectory(currentPath);
                }
                currentPath = currentPath + "\\" + map;
            }


            return FileData.Anim.ReadAnim(bytes, savePath, img);
            //System.IO.File.WriteAllBytes(currentPath, bytes);
        }

        private void SaveFromTile(byte[] bytes, string filename)
        {
            string[] paths = filename.Split('/');



            string currentPath = AppDomain.CurrentDomain.BaseDirectory + @"\CascData";
            string savePath = AppDomain.CurrentDomain.BaseDirectory + @"CascData\" + filename.Replace("/", "\\");


            foreach (string t in paths)
            {
                if (!System.IO.Directory.Exists(currentPath))
                {
                    System.IO.Directory.CreateDirectory(currentPath);
                }
                currentPath = currentPath + "\\" + t;
            }


            FileData.TileSet.ReadTile(bytes, savePath);
        }


        private void SaveToDdsgrp(byte[] bytes, string filename)
        {
            string[] paths = filename.Split('/');



            string currentPath = AppDomain.CurrentDomain.BaseDirectory + @"\CascData";


            foreach (string t in paths)
            {
                if (!System.IO.Directory.Exists(currentPath))
                {
                    System.IO.Directory.CreateDirectory(currentPath);
                }
                currentPath = currentPath + "\\" + t;
            }

            BinaryReader br = new BinaryReader(new MemoryStream(bytes));
            uint filesize = br.ReadUInt32();
            ushort frame = br.ReadUInt16(); //count
            ushort unknown = br.ReadUInt16();// (file version ?); -- value appears to always be 0x1001 in the files I've seen.

            for (int i = 0; i < frame; i++)
            {            //File Entry:
                uint unk = br.ReadUInt32(); // --always zero ?
                ushort width = br.ReadUInt16();
                ushort height = br.ReadUInt16();
                uint size = br.ReadUInt32();



                byte[] textureData = br.ReadBytes((int)size);
                //System.IO.File.WriteAllBytes(currentPath + i + ".dds", textureData);



                using (var image = Pfim.Pfim.FromStream(new MemoryStream(textureData)))
                {
                    System.Drawing.Imaging.PixelFormat format;

                    // Convert from Pfim's backend agnostic image format into GDI+'s image format
                    switch (image.Format)
                    {
                        case Pfim.ImageFormat.Rgba32:
                            format = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
                            break;
                        case Pfim.ImageFormat.Rgb24:
                            format = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
                            break;
                        default:
                            // see the sample for more details
                            throw new NotImplementedException();
                    }

                    // Pin pfim's data array so that it doesn't get reaped by GC, unnecessary
                    // in this snippet but useful technique if the data was going to be used in
                    // control like a picture box
                    var handle = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
                    try
                    {
                        var data = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);
                        var bitmap = new System.Drawing.Bitmap(image.Width, image.Height, image.Stride, format, data);

                        bitmap.MakeTransparent(System.Drawing.Color.Black);

                        for (int y = 0; y < bitmap.Height; y++)
                        {
                            for (int x = 0; x < bitmap.Width; x++)
                            {
                                System.Drawing.Color co = bitmap.GetPixel(x, y);

                                double[] a = new double[3];
                                a[0] = co.R / 256d;
                                a[1] = co.G / 256d;
                                a[2] = co.B / 256d;

                                double[] b = new double[3];
                                b[0] = 255 / 256d;
                                b[1] = 228 / 256d;
                                b[2] = 0;


                                double[] c = new double[3];


                                for (int t = 0; t < 3; t++)
                                {
                                    if(a[t] < 0.5)
                                    {
                                        c[t] = 2 * a[t] * b[t];
                                    }
                                    else
                                    {
                                        c[t] = 1 - 2 * (1 - a[t]) * (1 - b[t]);
                                    }
                                }




                                c[0] *= 256;
                                c[1] *= 256;
                                c[2] *= 256;


                                bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb((int)c[0], (int)c[1], (int)c[2]));
                            }
                        }



                        bitmap.Save(currentPath + i + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    }
                    finally
                    {
                        handle.Free();
                    }
                }
            }

            br.Close();




            //System.IO.File.WriteAllBytes(currentPath, bytes);
        }



        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            int percent = 0;

            Casc.Data data = new Casc.Data();


            if (!data.OpenCascStorage())
            {
                throw new Exception("스타크래프트가 실행중입니다.\n스타크래프트를 종료하고 다시 시도하세요.");
            }




            SaveToDdsgrp(data.ReadFileCascStorage("SD/unit/cmdicons/cmdicons.dds.grp"), @"cmdicons/");
            SaveFromAnim(data.ReadFileCascStorage("SD/mainSD.anim"), @"SD/anim/");
            worker.ReportProgress(percent++);

            byte[] ExistFlag = new byte[999];
            for (int i = 0; i < 999; i++)
            {
                string num = String.Format("{0:000}", i);
                {
                    string fname = $"HD2/anim/main_{num}.anim";

                    string tname = $"HD/anim/" + i + "/";

                    bool exist = SaveFromAnim(data.ReadFileCascStorage(fname), tname, i);
                    if (exist)
                    {
                        ExistFlag[i] = 1;
                    }
                    else
                    {
                        ExistFlag[i] = 0;
                    }
                }
                {
                    string fname = $"HD2/anim/Carbot/main_{num}.anim";

                    string tname = $"CB/anim/" + i + "/";
                    SaveFromAnim(data.ReadFileCascStorage(fname), tname, i);
                }
                worker.ReportProgress(percent++);
            }
            BinaryWriter bw = new BinaryWriter(new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"\CascData\grplist", FileMode.Create));
            bw.Write(ExistFlag);
            bw.Close();



            foreach (string tile in tilelist)
            {
                {
                    string fname = $"SD/TileSet/{tile}.dds.vr4";
                    //SaveToFile(data.ReadFileCascStorage(fname), fname);
                    SaveFromTile(data.ReadFileCascStorage(fname), fname + ".png");
                    BMP.ImageSizeChange(fname + ".png", fname + "s.png", TileSet.miniScale);
                    percent += 100;
                    worker.ReportProgress(percent);
                }
                {
                    string fname = $"TileSet/{tile}.dds.vr4";
                    string tname = $"HD/TileSet/{tile}.dds.vr4";
                    //SaveToFile(data.ReadFileCascStorage(fname), tname);
                    SaveFromTile(data.ReadFileCascStorage(fname), tname + "b.png");
                    BMP.ImageSizeChange(tname + "b.png", tname + ".png", 0.5);
                    BMP.ImageSizeChange(tname + ".png", tname + "s.png", TileSet.miniScale);
                    percent += 100;
                    worker.ReportProgress(percent);
                }
                {
                    string fname = $"Carbot/TileSet/{tile}.dds.vr4";
                    string tname = $"CB/TileSet/{tile}.dds.vr4";
                    //SaveToFile(data.ReadFileCascStorage(fname), tname);
                    SaveFromTile(data.ReadFileCascStorage(fname), tname + "b.png");
                    BMP.ImageSizeChange(tname + "b.png", tname + ".png", 0.5);
                    BMP.ImageSizeChange(tname + ".png", tname + "s.png", TileSet.miniScale);
                    percent += 100;
                    worker.ReportProgress(percent);
                }
                worker.ReportProgress(percent++);

                {
                    string cv5name = $"TileSet/{tile}.cv5";
                    string vf4name = $"TileSet/{tile}.vf4";

                    SaveToFile(data.ReadFileCascStorage(cv5name), cv5name);
                    SaveToFile(data.ReadFileCascStorage(vf4name), vf4name);
                }
            }





            data.CloseCascStorage();
        }
        string[] tilelist = { "badlands", "Desert", "Twilight", "Ice", "Jungle", "AshWorld", "platform", "install" };


    }
}
