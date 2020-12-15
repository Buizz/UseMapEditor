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
        public enum CHKTYPE
        {
            ERA,
            DIM,
            MTXM,
            aaq
        }


        private byte[] GetchkAll()
        {
            //에디터로부터 CHK데이터를 가져오는 함수

            return new byte[10];
        }
        private byte[] Getchk(CHKTYPE cHKTYPE)
        {
            //에디터로부터 CHK데이터를 가져오는 함수

            return new byte[10];
        }

        private bool ApplychkAll(BinaryReader br)
        {
            //Byte로부터 에디터로 넣는 함수
            foreach (CHKTYPE cHKTYPE in Enum.GetValues(typeof(CHKTYPE)))
            {
                Applychk(br, cHKTYPE);
            }

            return false;
        }
        private bool Applychk(BinaryReader br, CHKTYPE cHKTYPE)
        {
            bool IsFind = getchkcontext(br, cHKTYPE);
            if (!IsFind)
            {
                //데이터가 없을 경우 넘기기
                return false;
            }


            //Byte로부터 에디터로 넣는 함수
            switch (cHKTYPE)
            {
                case CHKTYPE.ERA:
                    TILETYPE = (UseMapEditor.FileData.TileSet.TileType)br.ReadUInt16();

                    break;
                case CHKTYPE.DIM:
                    WIDTH = br.ReadUInt16();
                    HEIGHT = br.ReadUInt16();
                    break;
                case CHKTYPE.MTXM:
                    MTXM = new ushort[WIDTH * HEIGHT];
                    for (int i = 0; i < WIDTH * HEIGHT; i++)
                    {
                        MTXM[i] = br.ReadUInt16();
                    }

                    break;
            }



            return true;
        }



        private bool getchkcontext(BinaryReader br, CHKTYPE cHKTYPE)
        {
            br.BaseStream.Position = 0;
            while (true)
            {
                if(br.BaseStream.Position == br.BaseStream.Length)
                {
                    return false;
                }

                string chkcode = System.Text.Encoding.ASCII.GetString(br.ReadBytes(4)).Replace(" ","");

                int size = br.ReadInt32();
                if(chkcode == cHKTYPE.ToString())
                {
                    return true;
                }
                br.BaseStream.Seek(size, SeekOrigin.Current);
            }
        }






    }
}
