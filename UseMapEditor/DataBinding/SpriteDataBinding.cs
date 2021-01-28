using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using UseMapEditor.Control;
using UseMapEditor.FileData;
using static Data.Map.MapData;

namespace UseMapEditor.DataBinding
{
    public class SpriteDataBinding : INotifyPropertyChanged
    {
        MapEditor mapEditor;
        public int ObjectID { get; set; }



        public SpriteDataBinding(MapEditor _mapEditor, int _ObjectID)
        {
            mapEditor = _mapEditor;
            ObjectID = _ObjectID;
        }



        public double MainNameSize
        {
            get
            {
                return 15;
            }
            set { }
        }
        public System.Windows.Visibility SecondNameVisble
        {
            get
            {
                return System.Windows.Visibility.Collapsed;
            }
            set { }
        }


        public string SecondName
        {
            get
            {
                return "";
            }
            set { }
        }
        public string MainName
        {
            get
            {
                return mapEditor.mapdata.GetCodeName(Codetype.Sprite, ObjectID);
            }
            set { }
        }

        public string AlphaName
        {
            get
            {
                return MainName + "[" + ObjectID.ToString().PadLeft(3, '0') + "]";
            }
            set { }
        }


        private BitmapSource imageIcon;
        public BitmapSource ImageIcon
        {
            get
            {
                if (Global.Setting.Vals[Global.Setting.Settings.Program_GRPLoad] == "true")
                {
                    if (imageIcon == null)
                    {
                        int image = (ushort)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.sprites, "Image File", ObjectID).Data;
                        int grp = (ushort)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.images, "GRP File", image).Data;
                        if(image == 651)
                        {
                            grp = 612;
                        }else if (image == 921)
                        {
                            grp = 867;
                        }


                        using (FileStream fileStream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"\CascData\icon\" + grp + ".png", FileMode.Open))
                        {
                            imageIcon = Global.WindowTool.GetBitmapSource(fileStream);
                        }
                    }


                    return imageIcon;
                }
                else
                {
                    return null;
                }
            }
            set
            {
            }
        }





        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
