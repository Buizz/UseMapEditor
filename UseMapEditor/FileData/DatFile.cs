using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseMapEditor.FileData
{
    public class DatFile
    {
        private Dictionary<DatFiles, CDatFile> DatfileDic;
        public enum DatFiles
        {
            units = 0,
            weapons = 1,
            flingy = 2,
            sprites = 3,
            images = 4,
            upgrades = 5,
            techdata = 6,
            orders = 7,
            portdata = 8,
            sfxdata = 9
        }
        public enum EParamInfo
        {
            Size = 0,
            VarStart = 1,
            VarEnd = 2,
            VarCount = 3,
            ValueType = 4,
            IsEnabled = 5,
            VarArray = 6
        }

        public DatFile(bool TemporyData = false, bool IsBindingData = false)
        {
            DatfileDic = new Dictionary<DatFiles, CDatFile>();

            foreach (DatFiles datFiles in Enum.GetValues(typeof(DatFiles)))
            {
                string str = datFiles.ToString();

                CDatFile tDatfile = new CDatFile(str);
                DatfileDic.Add(datFiles, tDatfile);
            }
        }



        public CDatFile.CParamater.Value Values(DatFiles key, string paramName, int index)
        {
            return DatfileDic[key].GetParamValue(paramName, index);
        }









        public class CDatFile
        {
            private string FIleName; // ex sprites
            private List<CParamater> Paramaters;


            public CParamater.Value GetParamValue(string name, int index)
            {
                return ParamDic[name].GetValue(index);
            }





            int ReadValue(string str)
            {
                int rv;
                int.TryParse(str.Split('=').Last(), out rv);
                return rv;
            }

            public CDatFile(string tFIleName)
            {
                ParamDic = new Dictionary<string, CParamater>();

                FIleName = tFIleName;
                Paramaters = new List<CParamater>();


                string filepath = AppDomain.CurrentDomain.BaseDirectory + @"\Data\DatFiles\" + FIleName;


                FileStream fs = new FileStream(filepath + ".dat", FileMode.Open);
                BinaryReader br = new BinaryReader(fs);
                StreamReader sr = new StreamReader(filepath + ".def");

                sr.ReadLine(); // 헤더
                int varcount = ReadValue(sr.ReadLine()); // Varcount
                int InputEntrycount = ReadValue(sr.ReadLine()); // InputEntrycount
                int OutputEntrycount = ReadValue(sr.ReadLine()); // OutputEntrycount

                sr.ReadLine(); // 빈공간
                sr.ReadLine(); // 값

                for (var i = 0; i <= varcount - 1; i++)
                {
                    Paramaters.Add(new CParamater(FIleName, sr, br, Paramaters.Count, InputEntrycount));
                    ParamDic.Add(Paramaters.Last().GetParamname, Paramaters.Last());
                }

                sr.Close();


                br.Close();
                fs.Close();
            }

            private Dictionary<string, CParamater> ParamDic;
            // 피라미터들
            public class CParamater
            {
                public long GetData(int index)
                {
                    index -= (int)VarStart;
                    if (index < Values.Count)
                        return Values[index].Data + InitVar;
                    else
                        return 0;
                }
                public void SetData(int index, int value)
                {
                    value -= InitVar;
                    index -= (int)VarStart;
                    if ((0 <= value) & (value < Math.Pow(256, Size)))
                        Values[index].Data = value;
                    else if (value < 0)
                        Values[index].Data = 0;
                    else
                        Values[index].Data = (long)(Math.Pow(256, Size) - 1);
                }




                public Value GetValue(int index)
                {
                    int realIndex = (int)(index - VarStart);

                    if (Values.Count > realIndex & realIndex >= 0)
                        return Values[realIndex];
                    else
                        return null;// ew Value(0, False)
                }



                public CParamater(string _FIleName, StreamReader sr, BinaryReader br, int prcount, int encount)
                {
                    FIleName = _FIleName;
                    VarEnd = (uint)(encount - 1);
                    while (!sr.EndOfStream)
                    {
                        string text = sr.ReadLine();
                        if (text.IndexOf("=") == -1)
                            break;
                        string key = text.Split('=').First().Replace(prcount.ToString(), "");
                        string value = text.Split('=').Last();
                        if (value.IndexOf(":") >= 0)
                            value = value.Split(':').First();

                        int valuenumber;
                        int.TryParse(value, out valuenumber);



                        switch (key)
                        {
                            case "Name":
                                {
                                    ParamaterName = value;
                                    break;
                                }

                            case "Size":
                                {
                                    Size = (byte)valuenumber;
                                    break;
                                }

                            case "VarStart":
                                {
                                    VarStart = (uint)valuenumber;
                                    break;
                                }

                            case "VarEnd":
                                {
                                    VarEnd = (uint)valuenumber;
                                    break;
                                }

                            case "VarArray":
                                {
                                    VarArray = (uint)valuenumber;
                                    break;
                                }

                            case "VarArrayIndex":
                                {
                                    VarIndex = (uint)valuenumber;
                                    break;
                                }

                            case "Type":
                                {
                                    ValueType = (DatFiles)valuenumber;
                                    break;
                                }

                            case "InitVar":
                                {
                                    InitVar = valuenumber;
                                    break;
                                }

                            default:
                                {
                                    break;
                                }
                        }
                    }



                    Values = new List<Value>();

                    long currentpos = br.BaseStream.Position; // 베이스 스트림을 기억하고
                    br.BaseStream.Position -= (VarIndex - 1) * Size * (VarEnd - VarStart + 1);
                    for (int i = 0; i <= VarEnd - VarStart; i++)
                    {
                        br.BaseStream.Position += (VarIndex - 1) * Size; // 인덱스 만큼 앞으로 간다.

                        uint value = 0;


                        switch (Size)
                        {
                            case 4:
                                {
                                    value = br.ReadUInt32();
                                    break;
                                }

                            case 2:
                                {
                                    value = br.ReadUInt16();
                                    break;
                                }

                            case 1:
                                {
                                    value = br.ReadByte();
                                    break;
                                }
                        }

                        Values.Add(new Value(value));


                        // If VarArray = 4 Then
                        // MsgBox(Hex(br.BaseStream.Position - Size) & vbCrLf & "유닛코드 : " & i & " 이름 : " & ParamaterName & " 값 : " & Values.Last)
                        // End If


                        br.BaseStream.Position += (VarArray - VarIndex) * Size; // 인덱스 만큼 앞으로 간다.
                    }

                    // 베이스 스트림에서 Size * encount만큼 간 곳으로 되돌린다.
                    br.BaseStream.Position = currentpos + Size * (VarEnd - VarStart + 1);
                }


                private string FIleName;
                private string ParamaterName;
                public string GetParamname
                {
                    get
                    {
                        return ParamaterName;
                    }
                }


                private byte Size;

                private uint VarStart = 0;
                private uint VarEnd = 0;

                private uint VarArray = 1;
                private uint VarIndex = 1;


                private DatFiles ValueType;
                private int InitVar = 0;

                private bool Enabled = true;

                private List<Value> Values;
                public class Value
                {
                    public long Data { get; set; }
                    public long MapData { get; set; }

                    public bool Enabled { get; set; }
                    public bool IsDefault { get; set; }

                    public Value(long _Data, bool _Enabled = true)
                    {
                        Data = _Data;
                        Enabled = _Enabled;
                        IsDefault = true;
                    }
                }

                public int GetValueCount
                {
                    get
                    {
                        return Values.Count;
                    }
                }
            }
        }
    }
}
