using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Map
{
    public partial class MapData
    {

        public List<SoundData> soundDatas;
        public class SoundData
        {
            public SoundData(string path)
            {
                this.path = path;
            }
            public SoundData()
            {
            }

            public string path;
            public byte[] bytes;
        }

        public bool DeleteSound(string soundname)
        {
            SoundData soundData = soundDatas.Find((x) => x.path == soundname);
            if (soundData == null)
            {
                //맵에 없는 데이터
                for (int i = 0; i < WAV.Count; i++)
                {
                    string d = WAV[i].String;
                    if (WAV[i].IsLoaded)
                    {
                        if (d == soundname)
                        {
                            //이 파일 일 경우
                            WAV[i].UnLoaded();
                            return true;
                        }
                    }
                }
            }
            else
            {
                //맵에 있는 데이터
                for (int i = 0; i < WAV.Count; i++)
                {
                    string d = WAV[i].String;
                    if (WAV[i].IsLoaded)
                    {
                        if (d == soundname)
                        {
                            //이 파일 일 경우
                            WAV[i].UnLoaded();

                            soundDatas.Remove(soundData);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public SoundData AddSound(byte[] bytes, string chkname)
        {
            chkname = System.IO.Path.ChangeExtension(chkname, "ogg");


            SoundData fsound = soundDatas.Find((x) => x.path == chkname);
            if(fsound != null)
            {
                fsound.path = chkname;
                fsound.bytes = bytes;

                
                return fsound;
            }




            SoundData soundData = new SoundData();

            soundData.path = chkname;
            soundData.bytes = bytes;

            soundDatas.Add(soundData);
            WAV.Add(new StringData(mapEditor.mapdata, chkname));

           
            return null;
        }

    }
}
