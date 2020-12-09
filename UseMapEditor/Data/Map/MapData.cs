using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class MapData
    {
        public MapData(string _filepath)
        {
            filepath = _filepath;
        }

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


    }
}
