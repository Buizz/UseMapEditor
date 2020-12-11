using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class MapData
    {


        private string filepath;
        public string FilePath
        {
            get
            {
                return filepath;
            }
            set
            {
                filepath = value;
            }
        }
        public string SafeFileName
        {
            get
            {
                return filepath.Split('\\').Last();
            }
        }


        public bool LoadMap(string _filepath)
        {
            filepath = _filepath;
            //맵 파일의 이름이 ""일 경우 새맵
            if (filepath == "")
            {
                filepath = "제목없음";
            }


            //맵 파일을 연다.


            //chk를 추출한다.


            //음원파일을 추출한다.






            return true;
        }
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








            return true;
        }







        public enum CHKTYPE
        {
            ALL,
            UNIx
        }
        /// <summary>
        /// CHK를 반환하는 코드
        /// </summary>
        /// <param name="cHKTYPE">들어올 데이터</param>
        /// <returns></returns>
        private byte[] GetCHK(CHKTYPE cHKTYPE = CHKTYPE.ALL)
        {
            return new byte[10];
        }



        /// <summary>
        /// CHK로 부터 데이터를 로드하는 코드
        /// 
        /// </summary>
        /// <param name="chadata">byte로 된 데이터</param>
        /// <param name="cHKTYPE">특정 지정한 타입만 인식</param>
        /// <returns></returns>
        private bool ReadCHK(byte[] chadata, CHKTYPE cHKTYPE = CHKTYPE.ALL)
        {
            return false;
        }






    }
}
