using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Map
{
    public partial class MapData
    {

        public bool LoadMap(string _filepath)
        {
            filepath = _filepath;
            //맵 파일의 이름이 ""일 경우 새맵
            if (filepath == "")
            {
                filepath = "제목없음";
                return true;
            }
            MapDataReset();

            //맵 파일을 연다.
            //chk를 추출한다.
            uint hmpq = 0;
            uint hfile = 0;
            byte[] buffer = null;
            uint filesize = 0;

            uint pdwread = 0;

            UseMapEditor.FileData.StromLib.SFileOpenArchive(filepath, 0, 0, ref hmpq);
            string openFilename = @"staredit\scenario.chk";

            UseMapEditor.FileData.StromLib.SFileOpenFileEx(hmpq, openFilename, 0, ref hfile);
            if (hfile != 0)
            {
                filesize = UseMapEditor.FileData.StromLib.SFileGetFileSize(hfile, ref filesize);
                buffer = new byte[filesize];
                UseMapEditor.FileData.StromLib.SFileReadFile(hfile, buffer, filesize, ref pdwread, new IntPtr());
                UseMapEditor.FileData.StromLib.SFileCloseFile(hfile);
                UseMapEditor.FileData.StromLib.SFileCloseArchive(hmpq);
            }
            else
            {
                UseMapEditor.FileData.StromLib.SFileCloseArchive(hmpq);
                //TODO:MPQ오픈 실패
                throw new Exception("scenario.chk를 열지 못했습니다.");
            }

            //음원파일을 추출한다.

            using (BinaryReader br = new BinaryReader(new MemoryStream(buffer)))
            {
                if (!ApplychkAll(br))
                {
                    return false;
                }

            }
                






            return true;
        }
    }
}
