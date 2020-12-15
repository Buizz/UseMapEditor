using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
            progres.Value = ((double)e.ProgressPercentage / (double)1008) * 100;
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
                MessageBox.Show(e.Error.ToString());
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

        private void SaveToAnim(byte[] bytes, string filename, int img = -1)
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


            FileData.Anim.ReadAnim(bytes, savePath, img);
            //System.IO.File.WriteAllBytes(currentPath, bytes);
        }






        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            int percent = 0;

            Casc.Data data = new Casc.Data();


            data.OpenCascStorage();

            SaveToAnim(data.ReadFileCascStorage("SD/mainSD.anim"), @"SD/anim/");
            worker.ReportProgress(percent++);

            for (int i = 0; i < 999; i++)
            {
                string num = String.Format("{0:000}", i);
                {
                    string fname = $"HD2/anim/main_{num}.anim";

                    string tname = $"HD/anim/" + i + "/";
                    SaveToAnim(data.ReadFileCascStorage(fname), tname, i);
                }
                {
                    string fname = $"HD2/anim/Carbot/main_{num}.anim";

                    string tname = $"CB/anim/" + i + "/";
                    SaveToAnim(data.ReadFileCascStorage(fname), tname, i);
                }
                worker.ReportProgress(percent++);
            }

            foreach (string tile in tilelist)
            {
                {
                    string fname = $"SD/TileSet/{tile}.dds.vr4";
                    SaveToFile(data.ReadFileCascStorage(fname), fname);
                }
                {
                    string fname = $"HD2/TileSet/{tile}.dds.vr4";
                    string tname = $"HD/TileSet/{tile}.dds.vr4";
                    SaveToFile(data.ReadFileCascStorage(fname), tname);
                }
                {
                    string fname = $"HD2/Carbot/TileSet/{tile}.dds.vr4";
                    string tname = $"CB/TileSet/{tile}.dds.vr4";
                    SaveToFile(data.ReadFileCascStorage(fname), tname);
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
