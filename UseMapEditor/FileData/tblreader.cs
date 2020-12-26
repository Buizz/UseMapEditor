using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseMapEditor.FileData
{
    public class tblreader
    {
        public readonly List<TBLString> Strings;

        public struct TBLString
        {
            public string val1;
            public string val2;

            public TBLString(string tval1, string tval2)
            {
                val1 = tval1;
                val2 = tval2;
            }
        }


        public tblreader(string tblFile)
        {
            Strings = new List<TBLString>();


            FileStream fs = new FileStream(tblFile, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);

            UInt16[] header;


            UInt16 count = br.ReadUInt16();
            header = new ushort[count - 1 + 1];


            for (int i = 0; i <= count - 1; i++)
                header[i] = br.ReadUInt16();

            for (int i = 0; i <= count - 1; i++)
            {
                fs.Position = header[i];
                List<byte> bytes1 = new List<byte>();
                List<byte> bytes2 = new List<byte>();

                int charCount = 0;
                while (true)
                {
                    byte val = br.ReadByte(); // 바이트씩 읽으면서 0이 나오면 종료.
                    if (val == 0 & charCount >= 2)
                    {
                        if (i == count - 1)
                            break;
                        else
                            while (fs.Position < header[i + 1]) // 문자 끝까지 읽기.
                            {
                                if (val == 0)
                                    bytes2.Add(124);
                                else
                                    bytes2.Add(val);
                                val = br.ReadByte();
                            }





                        // If fs.Position < fs.Length Then
                        // val = br.ReadByte()
                        // Else
                        // Exit While
                        // End If


                        break;
                    }
                    else
                    {
                        if (val < 32 & val != 10 & val != 13)
                        {
                            bytes1.Add(60);

                            bytes1.AddRange(System.Text.Encoding.Default.GetBytes(val.ToString("X").PadLeft(2, '0')));

                            bytes1.Add(62);
                        }
                        else
                            bytes1.Add(val);
                        bytes2.Add(val);
                    }
                    charCount += 1;
                }
                string val1 = System.Text.Encoding.Default.GetString(bytes1.ToArray());
                string val2 = System.Text.Encoding.Default.GetString(bytes2.ToArray());

                TBLString ttbl = new TBLString(val1, val2);


                Strings.Add(ttbl);
            }


            br.Close();
            fs.Close();
        }
    }
}
