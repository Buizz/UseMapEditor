using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseMapEditor.Casc
{
    public class Data
    {
        private string datapath;
        private IntPtr hStorage;
        private IntPtr hfile;
        private Dictionary<string, string> FileHash;

        string rootpath = AppDomain.CurrentDomain.BaseDirectory + @"\Data\ROOT";

        public Data()
        {
            // 해시코드 읽어서 저장하기




            datapath = Global.Setting.Vals[Global.Setting.Settings.Program_StarCraftPath];
            FileHash = new Dictionary<string, string>();

            FileStream filestream = new FileStream(rootpath, FileMode.Open);
            StreamReader sr = new StreamReader(filestream);



            string[] strs = sr.ReadToEnd().Split('\n');

            sr.Close();
            filestream.Close();



            for (var i = 0; i <= strs.Count() - 1; i++)
            {
                if (strs[i].Trim().Split('|').Count() == 2)
                    FileHash.Add(strs[i].Trim().Split('|').First().ToLower(), strs[i].Trim().Split('|').Last().ToLower());
            }
        }




        public byte[] ReadFile(string filename) // 파일 읽기
        {
            filename = filename.Replace(@"\", "/").ToLower();

            string Hash;

            if (FileHash.ContainsKey(filename))
                Hash = FileHash[filename];
            else
                return new byte[0];


            CascLib.CascOpenStorage(datapath, 0x200, ref hStorage);

            MemoryStream memstream = new MemoryStream();

            BinaryWriter bytewriter = new BinaryWriter(memstream);
            BinaryReader bytereader = new BinaryReader(memstream);
            byte[] Buffer = new byte[1025];

            //CascLib.CascOpenFile(hStorage, Hash, 0, 0x1, ref hfile);
            CascLib.CascOpenFile(hStorage, Hash, 0, 0, ref hfile);

            while ((true))
            {
                IntPtr dwBytesRead = new IntPtr();
                CascLib.CascReadFile(hfile, Buffer,(uint) Buffer.Length, ref dwBytesRead);
                if ((dwBytesRead == new IntPtr()))
                    break;
                bytewriter.Write(Buffer);
            }
            memstream.Position = 0;
            byte[] Bytes = memstream.ToArray();

            CascLib.CascCloseFile(hfile);

            bytereader.Close();
            bytewriter.Close();
            memstream.Close();



            CascLib.CascCloseStorage(hStorage);

            return Bytes;
        }

        public bool OpenCascStorage()
        {
            CascLib.CascOpenStorage(datapath, 0x200, ref hStorage);
            return (hStorage.ToInt64() != 0);
        }
        byte[] Buffer = new byte[1025];
        public byte[] ReadFileCascStorage(string filename)
        {
            filename = filename.Replace(@"\", "/").ToLower();

            string Hash;

            if (FileHash.ContainsKey(filename))
                Hash = FileHash[filename];
            else
                return new byte[0];

            MemoryStream memstream = new MemoryStream();

            BinaryWriter bytewriter = new BinaryWriter(memstream);

            bool r = CascLib.CascOpenFile(hStorage, Hash, 0, 0, ref hfile);


            if (hfile.ToInt64() == 0)
            {
                throw new Exception(filename + " 파일을 열 수 없었습니다.");
            }

            while ((true))
            {
                IntPtr dwBytesRead = new IntPtr(); ;
                CascLib.CascReadFile(hfile, Buffer, (uint) Buffer.Length, ref dwBytesRead);
                if ((dwBytesRead == new IntPtr()))
                    break;
                bytewriter.Write(Buffer);
            }
            memstream.Position = 0;
            byte[] Bytes = memstream.ToArray();

            CascLib.CascCloseFile(hfile);

            bytewriter.Close();
            memstream.Close();
            memstream.Dispose();
            return Bytes;
        }
        public void CloseCascStorage()
        {
            CascLib.CascCloseStorage(hStorage);
        }
    }
}
