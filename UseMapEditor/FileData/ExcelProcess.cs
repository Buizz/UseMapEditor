using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using UseMapEditor.Control;
using static UseMapEditor.FileData.ExcelData;

namespace UseMapEditor.FileData
{
    public class ExcelProcessExcute
    {
        public DispatcherTimer excelTimer;



        public void Close()
        {
            for (int i = 0; i < processlist.Count; i++)
            {
                ExcelProcess excelProcess = processlist[i];
                try
                {
                    excelProcess.process.Kill();
                }
                catch (Exception)
                {
                }
            }

            if(excelTimer != null)
            {
                excelTimer.Stop();
                excelTimer = null;
            }
        }

        private void InitTimer()
        {
            excelTimer = new DispatcherTimer( DispatcherPriority.Normal);
            excelTimer.Interval = TimeSpan.FromMilliseconds(1000);
            excelTimer.Tick += ExcelTimer_Tick;

            excelTimer.Start();
        }

        private void ExcelTimer_Tick(object sender, EventArgs e)
        {
            int index = 0;
            for (int i = 0; i < processlist.Count; i++)
            {
                ExcelProcess excelProcess = processlist[index];

                if (File.Exists(excelProcess.fname))
                {
                    FileInfo fileInfo = new FileInfo(excelProcess.fname);

                    if(excelProcess.dateTime != fileInfo.LastWriteTime)
                    {
                        ////파일이 변형되어 있을 경우
                        ExcelData excelData = new ExcelData(excelProcess.mapEditor);
                        excelData.LoadExcel(excelProcess.fname, ExcelType.All);

                        excelProcess.dateTime = fileInfo.LastWriteTime;
                        excelData.Dispos();
                    }


                    if (excelProcess.process.HasExited)
                    {
                        //프로세스가 종료되었을 경우
                        //삭제플래그
                        File.Delete(excelProcess.fname);
                        processlist.RemoveAt(index);
                    }
                    else
                    {
                        index++;
                    }
                }
                else
                {
                    //파일 삭제됨
                    excelProcess.process.Kill();
                    processlist.RemoveAt(index);
                    continue;
                }
            }

            if(processlist.Count == 0)
            {
                //타이머스탑
                excelTimer.Stop();
                excelTimer = null;
            }
        }

        public void AddProcess(MapEditor mapEditor, string fname)
        {
            processlist.Add(new ExcelProcess(mapEditor, fname));
            if(excelTimer == null)
            {
                InitTimer();
            }
        }


        private List<ExcelProcess> processlist = new List<ExcelProcess>();

        public class ExcelProcess
        {
            public MapEditor mapEditor;
            public Process process;
            public string fname;
            public DateTime dateTime;

            public ExcelProcess(MapEditor mapEditor, string fname)
            {
                this.mapEditor = mapEditor;
                this.fname = fname;

                process = new Process();
                process.StartInfo.FileName = fname;

                FileInfo fileInfo = new FileInfo(fname);
                dateTime = fileInfo.LastWriteTime;

                process.Start();
            }
        }
    }
}
