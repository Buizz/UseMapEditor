using Microsoft.Win32;
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
    }
}
