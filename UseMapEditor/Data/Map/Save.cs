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
            //파일이 존재하지 않을 경우 경고문 뜨기
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

            uint hmpq = 0;


            if (!File.Exists(_filepath))
            {
                StromLib.SFileCreateArchive(_filepath, 0, 24, ref hmpq);
                StromLib.SFileAddListFile(hmpq, StromLib.LISTFILE_NAME);
            }
            else
            {
                StromLib.SFileOpenArchive(_filepath, 0, 0, ref hmpq);
            }




            {
                string chkFilename = @"staredit\scenario.chk";
                string SaveFilename = UseMapEditor.Dialog.ProgramStart.tempfolder + @"\scenario.chk";
                BinaryWriter bw = new BinaryWriter(new FileStream(SaveFilename, FileMode.Create));
                GetCHKAll(bw);
                bw.Close();
                AddMPQFile(hmpq, SaveFilename, chkFilename);
            }


            for (int i = 0; i < mpqfileList.Count; i++)
            {
                SoundData soundData = soundDatas.Find((x) => x.path == mpqfileList[i]);
                if(soundData == null)
                {
                    RemoveMPQFile(hmpq, mpqfileList[i]);
                }
            }


            mpqfileList.Clear();

            for (int i = 0; i < soundDatas.Count; i++)
            {
                string chkFilename = soundDatas[i].path;
                string SaveFilename = UseMapEditor.Dialog.ProgramStart.tempfolder + "\\temp" + i;
                BinaryWriter bw = new BinaryWriter(new FileStream(SaveFilename, FileMode.Create));
                bw.Write(soundDatas[i].bytes);
                bw.Close();
                bool rbool = AddMPQFile(hmpq, SaveFilename, chkFilename);

                mpqfileList.Add(soundDatas[i].path);
            }










            StromLib.SFileCompactArchive(hmpq, null, false);


            StromLib.SFileCloseArchive(hmpq);



            return true;
        }
    }
}
