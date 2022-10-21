using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseMapEditor.Dialog;
using UseMapEditor.FileData;

namespace Data.Map
{
    public partial class MapData
    {
        public bool SaveMap(string _filepath = "")
        {
            if (_filepath == "")
            {
                _filepath = filepath;
            }

            //지정된 파일 없는 새로 저장일 경우
            if (!File.Exists(_filepath))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "맵 파일|*.scx";


                if ((bool)saveFileDialog.ShowDialog())
                {
                    filepath = saveFileDialog.FileName;
                    _filepath = filepath;


                }
                else
                {
                    return false;
                }
            }

            ulong hmpq = 0;


            if (!File.Exists(_filepath))
            {
                //지정한 경로가 새로 만들기 일 경우
                StromLib.SFileCreateArchive(_filepath, 0, 24, ref hmpq);
                StromLib.SFileAddListFile(hmpq, StromLib.LISTFILE_NAME);
            }
            else
            {
                //덮어쓰기 저장일 경우
                //MsgDialog msgDialog = new MsgDialog("본 버전은 테스트 버전으로 맵이 날라 갈 수 있습니다.\r\n백업맵을 저장하시겠습니까?", System.Windows.MessageBoxButton.YesNoCancel, System.Windows.MessageBoxImage.Warning);
                //msgDialog.ShowDialog();
                //if (msgDialog.msgresult == System.Windows.MessageBoxResult.Cancel)
                //{
                //    return false;
                //}else if (msgDialog.msgresult == System.Windows.MessageBoxResult.Yes)
                //{
                //    if (!Directory.Exists(UseMapEditor.Global.Setting.backFolder))
                //    {
                //        Directory.CreateDirectory(UseMapEditor.Global.Setting.backFolder);
                //    }
                //    FileInfo fileInfo = new FileInfo(_filepath);
                //    File.Copy(_filepath, UseMapEditor.Global.Setting.backFolder + @"\" + fileInfo.Name + DateTime.Now.ToString("yyMMddHHmmss") + ".scx");
                //}
                if (!Directory.Exists(UseMapEditor.Global.Setting.backFolder))
                {
                    Directory.CreateDirectory(UseMapEditor.Global.Setting.backFolder);
                }
                FileInfo fileInfo = new FileInfo(_filepath);
                File.Copy(_filepath, UseMapEditor.Global.Setting.backFolder + @"\" + fileInfo.Name + DateTime.Now.ToString("yyMMddHHmmss") + ".scx");




                StromLib.SFileOpenArchive(_filepath, 0, 0, ref hmpq);


                //여기서 리스트파일을 읽는다.


                List<string> mpqfileList = new List<string>();


                byte[] buffer = ReadMPQFileC(hmpq, @"(listfile)");
                if (buffer.Length == 0)
                {
                    StromLib.SFileCloseArchive(hmpq);

                    File.Delete(_filepath);


                    StromLib.SFileCreateArchive(_filepath, 0, 24, ref hmpq);
                    StromLib.SFileAddListFile(hmpq, StromLib.LISTFILE_NAME);
                    //throw new Exception("listfile을 열지 못했습니다.");
                }
                else
                {
                    string str = Encoding.GetEncoding(949).GetString(buffer);
                    mpqfileList.Clear();
                    str = str.Replace("\r", "");
                    mpqfileList = str.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();




                    for (int i = 0; i < mpqfileList.Count; i++)
                    {
                        RemoveMPQFile(hmpq, mpqfileList[i]);
                    }
                }
            }




            {
                string chkFilename = @"staredit\scenario.chk";
                string SaveFilename = UseMapEditor.Dialog.ProgramStart.tempfolder + @"\scenario.chk";
                BinaryWriter bw = new BinaryWriter(new FileStream(SaveFilename, FileMode.Create));
                GetCHKAll(bw);
                bw.Close();
                AddMPQFile(hmpq, SaveFilename, chkFilename);
            }

            for (int i = 0; i < soundDatas.Count; i++)
            {
                string chkFilename = soundDatas[i].path;
                string SaveFilename = UseMapEditor.Dialog.ProgramStart.tempfolder + "\\temp" + i;
                BinaryWriter bw = new BinaryWriter(new FileStream(SaveFilename, FileMode.Create));
                bw.Write(soundDatas[i].bytes);
                bw.Close();
                bool rbool = AddMPQFile(hmpq, SaveFilename, chkFilename);
            }






            StromLib.SFileCompactArchive(hmpq, null, false);


            StromLib.SFileCloseArchive(hmpq);



            return true;
        }
    }
}
