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


           
            string chkFilename = @"staredit\scenario.chk";
            string SaveFilename = ProgramStart.tempfolder + @"\scenario.chk";
            BinaryWriter bw = new BinaryWriter(new FileStream(SaveFilename, FileMode.Create));

            GetCHKAll(bw);

            bw.Close();



            if (!File.Exists(_filepath))
            {
                StromLib.SFileCreateArchive(_filepath, 0, 65535, ref hmpq);
            }
            else
            {
                StromLib.SFileOpenArchive(_filepath, 0, 0, ref hmpq);
            }


            bool chkfilesave = StromLib.SFileAddFileEx(hmpq, SaveFilename, chkFilename, StromLib.MPQ_FILE_COMPRESS | StromLib.MPQ_FILE_ENCRYPTED | StromLib.MPQ_FILE_REPLACEEXISTING, StromLib.MPQ_COMPRESSION_ZLIB, StromLib.MPQ_COMPRESSION_ZLIB);
            StromLib.SFileCompactArchive(hmpq, null, false);


            StromLib.SFileCloseArchive(hmpq);



            return chkfilesave;
        }
    }
}
