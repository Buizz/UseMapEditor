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


            foreach(string t in paths)
            {
                if (!System.IO.Directory.Exists(currentPath))
                {
                    System.IO.Directory.CreateDirectory(currentPath);
                }
                currentPath = currentPath + "\\" + t;
            }



            System.IO.File.WriteAllBytes(currentPath, bytes);
        }



        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            int percent = 0;

            Casc.Data data = new Casc.Data();


            data.OpenCascStorage();

            SaveToFile(data.ReadFileCascStorage("SD/mainSD.anim"), @"SD/mainSD.anim");
            worker.ReportProgress(percent++);

            for (int i = 0; i < 999; i++)
            {
                string num = String.Format("{0:000}", i);
                {
                    string fname = $"HD2/anim/main_{num}.anim";

                    string tname = $"HD/anim/main_{num}.anim";
                    SaveToFile(data.ReadFileCascStorage(fname), tname);
                }
                {
                    string fname = $"HD2/anim/Carbot/main_{num}.anim";

                    string tname = $"Carbot/anim/main_{num}.anim";
                    SaveToFile(data.ReadFileCascStorage(fname), tname);
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
                    string fname = $"SD/TileSet/{tile}.dds.grp";
                    SaveToFile(data.ReadFileCascStorage(fname), fname);
                }
                {
                    string fname = $"HD2/TileSet/{tile}.dds.vr4";
                    string tname = $"HD/TileSet/{tile}.dds.vr4";
                    SaveToFile(data.ReadFileCascStorage(fname), tname);
                }
                {
                    string fname = $"HD2/TileSet/{tile}.dds.grp";
                    string tname = $"HD/TileSet/{tile}.dds.grp";
                    SaveToFile(data.ReadFileCascStorage(fname), tname);
                }
                {
                    string fname = $"HD2/Carbot/TileSet/{tile}.dds.vr4";
                    string tname = $"Carbot/TileSet/{tile}.dds.vr4";
                    SaveToFile(data.ReadFileCascStorage(fname), tname);
                }
                {
                    string fname = $"HD2/Carbot/TileSet/{tile}.dds.grp";
                    string tname = $"Carbot/TileSet/{tile}.dds.grp";
                    SaveToFile(data.ReadFileCascStorage(fname), tname);
                }
                worker.ReportProgress(percent++);
            }





            data.CloseCascStorage();
        }
        string[] tilelist = { "badlands", "Desert", "Twilight", "Ice", "Jungle", "AshWorld", "platform", "install" };


    }
}
