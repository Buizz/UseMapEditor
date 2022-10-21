using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseMapEditor.FileData;

namespace Data.Map
{
    public partial class MapData
    {
        private bool AddMPQFile(ulong hmpq, string filename, string chkfilename)
        {

            bool chkfilesave = StromLib.SFileAddFileEx(hmpq, filename, chkfilename, StromLib.MPQ_FILE_COMPRESS | StromLib.MPQ_FILE_ENCRYPTED | StromLib.MPQ_FILE_REPLACEEXISTING, StromLib.MPQ_COMPRESSION_ZLIB, StromLib.MPQ_COMPRESSION_ZLIB);


            return chkfilesave;
        }

        private void RemoveMPQFile(ulong hmpq, string filename)
        {
            StromLib.SFileRemoveFile(hmpq, filename, 0);
        }









        private byte[] ReadMPQFile(string filename)
        {
            ulong hmpq = 0;
            ulong hfile = 0;
            byte[] buffer = new byte[0];
            ulong filesize = 0;

            ulong pdwread = 0;

            UseMapEditor.FileData.StromLib.SFileOpenArchive(filepath, 0, 0, ref hmpq);
            string openFilename = filename;

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
                return buffer;
            }

            return buffer;
        }




        private ulong OpenArchive()
        {
            ulong hmpq = 0;

            UseMapEditor.FileData.StromLib.SFileOpenArchive(filepath, 0, 0, ref hmpq);

            return hmpq;
        }
        private void CloseArchive(ulong hmpq)
        {
            UseMapEditor.FileData.StromLib.SFileCloseArchive(hmpq);
        }
        private byte[] ReadMPQFileC(ulong hmpq, string filename)
        {
            ulong hfile = 0;
            byte[] buffer = new byte[0];
            ulong filesize = 0;

            ulong pdwread = 0;

            string openFilename = filename;

            UseMapEditor.FileData.StromLib.SFileOpenFileEx(hmpq, openFilename, 0, ref hfile);
            if (hfile != 0)
            {
                filesize = UseMapEditor.FileData.StromLib.SFileGetFileSize(hfile, ref filesize);
                buffer = new byte[filesize];
                UseMapEditor.FileData.StromLib.SFileReadFile(hfile, buffer, filesize, ref pdwread, new IntPtr());
                UseMapEditor.FileData.StromLib.SFileCloseFile(hfile);
            }
            else
            {
                return buffer;
            }

            return buffer;
        }







        //private void ReadFileList()
        //{
        //    byte[] buffer = ReadMPQFile(@"(listfile)");
        //    if (buffer.Length == 0)
        //    {
        //        throw new Exception("listfile을 열지 못했습니다.");
        //    }

        //    string str = Encoding.GetEncoding(949).GetString(buffer);





        //    mpqfileList.Clear();
        //    str = str.Replace("\r", "");
        //    mpqfileList = str.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();

        //    mpqfileList.Remove(@"staredit\scenario.chk");
        //}
    }
}
