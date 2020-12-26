using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseMapEditor.FileData
{
    public class Iscript
    {
        public List<Opcode> Opcodedata;
        public struct Opcode
        {
            public string name;
            public byte[] parmsize;
            public string commnet;
        }

        public void readOpcodes()
        {
            Opcodedata = new List<Opcode>();

            FileStream filestream = new FileStream(System.AppDomain.CurrentDomain.BaseDirectory + @"\Data\AnimOpcodes.txt", FileMode.Open);
            StreamReader streamreader = new StreamReader(filestream);
            string temptext = streamreader.ReadToEnd();

            string[] lines = temptext.Split('\n');

            for (var k = 0; k <= lines.Count() - 2; k++)
            {
                string line = lines[k].Replace("\r", "");

                string[] content = line.Split('	');
                Opcode topcode = new Opcode();
                topcode.name = content[1];
                topcode.commnet = content[4];

                if(content[2] == "None")
                {
                    topcode.parmsize = new byte[0];
                    Opcodedata.Add(topcode);
                    continue;
                }
                int parmlen = content[2].Split(',').Count() - 1;

                topcode.parmsize = new byte[parmlen];
                for (var i = 0; i <= topcode.parmsize.Length - 1; i++)
                {
                    string value = content[2].Split(',')[i].Trim();

                    byte v;
                    byte.TryParse(value.Split(' ')[0].Replace("u", ""), out v);
                    topcode.parmsize[i] = v;
                }

                Opcodedata.Add(topcode);
            }
            streamreader.Close();
            filestream.Close();
        }




        public Dictionary<int, Entry> iscriptEntries;


        byte[] buffer;
        public Iscript(string filename, bool xscript)
        {
            readOpcodes();
            iscriptEntries = new Dictionary<int, Entry>();

            buffer = File.ReadAllBytes(filename);


            int index = 0;
            if (!xscript)
            {
                index = ReadUint16(ref index);
            }

            ushort id, headeroffset;

            while (true)
            {
                id = ReadUint16(ref index);
                if(id == 0xFFFF)
                {
                    break;
                }
                headeroffset = ReadUint16(ref index);

                Entry entry = new Entry();
                entry.Parrent = this;
                entry.IScriptID = id;
                entry.headeroffset = headeroffset;

                iscriptEntries.Add(id, entry);
            }

            List<Entry> entries = iscriptEntries.Values.ToList();

            for (int i = 0; i < entries.Count; i++)
            {
                int fi = entries[i].headeroffset + 4;

                uint entrytype = ReadUint32(ref fi);
                ushort opcodecount = ANIMTYPE[entrytype];

                entries[i].EntryType = entrytype;

                entries[i].AnimHeader = new ushort[opcodecount];

                for (int t = 0; t < opcodecount; t++)
                {
                    entries[i].AnimHeader[t] = ReadUint16(ref fi);
                }

            }
        }


        public byte ReadByte(ref int index)
        {
            byte rval = buffer[index];
            index += 1;

            return rval;
        }
        public ushort ReadUint16(ref int index)
        {
            ushort rval = (ushort)buffer[index];
            rval += (ushort)((ushort)buffer[index + 1] << 8);
            index += 2;

            return rval;
        }
        public uint ReadUint32(ref int index)
        {
            uint rval = (ushort)buffer[index];
            rval += (ushort)((ushort)buffer[index + 1] << 8);
            rval += (ushort)((ushort)buffer[index + 2] << 16);
            rval += (ushort)((ushort)buffer[index + 3] << 24);
            index += 4;

            return rval;
        }





        private readonly byte[] ANIMTYPE = { 2, 2, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 14, 14, 16, 16, 0, 0, 0, 0, 22, 22, 0, 24, 26, 0, 28, 28, 28, 28 };
        public class Entry
        {

            public Iscript Parrent;

            public int IScriptID;
            public ushort headeroffset;

            public uint EntryType;
            public ushort[] AnimHeader;
        }
    }
}
