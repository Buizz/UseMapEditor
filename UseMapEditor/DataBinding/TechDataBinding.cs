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
    public class TechDataBinding : INotifyPropertyChanged
    {
        MapEditor mapEditor;
        public int ObjectID { get; set; }



        public TechDataBinding(MapEditor _mapEditor, int _ObjectID)
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
                return mapEditor.mapdata.GetCodeName(Codetype.Tech, ObjectID);
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

        public void UIPropertyChange()
        {
            OnPropertyChanged("SecondName");
            OnPropertyChanged("MainName");

            OnPropertyChanged("MainNameSize");
            OnPropertyChanged("SecondNameVisble");
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
                        int icon = (ushort)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.techdata, "Icon", ObjectID).Data;
                        using (FileStream fileStream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"\CascData\cmdicons\" + icon + ".png", FileMode.Open))
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





        public bool USEDEFAULT
        {
            get
            {

                return (mapEditor.mapdata.TECx.USEDEFAULT[ObjectID] == 0);
            }
            set
            {
                if (value)
                {
                    mapEditor.mapdata.TECx.USEDEFAULT[ObjectID] = 0;
                    //값들을 초기화해야됨.
                }
                else
                {
                    mapEditor.mapdata.TECx.USEDEFAULT[ObjectID] = 1;
                }

                MIN = (ushort)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.techdata, "Mineral Cost", ObjectID).Data;
                GAS = (ushort)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.techdata, "Vespene Cost", ObjectID).Data;
                BASETIME = (ushort)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.techdata, "Resarch Time", ObjectID).Data;
                ENERGY = (ushort)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.techdata, "Energy Required", ObjectID).Data;


                mapEditor.SetDirty();


                OnPropertyChanged("USEDEFAULT");
            }
        }


        public ushort MIN
        {
            get
            {
                return mapEditor.mapdata.TECx.MIN[ObjectID];
            }
            set
            {
                mapEditor.mapdata.TECx.MIN[ObjectID] = value;
                mapEditor.SetDirty();
                OnPropertyChanged("MIN");
            }
        }
        public ushort GAS
        {
            get
            {
                return mapEditor.mapdata.TECx.GAS[ObjectID];
            }
            set
            {
                mapEditor.mapdata.TECx.GAS[ObjectID] = value;
                mapEditor.SetDirty();
                OnPropertyChanged("GAS");
            }
        }
        public ushort BASETIME
        {
            get
            {
                return mapEditor.mapdata.TECx.BASETIME[ObjectID];
            }
            set
            {
                mapEditor.mapdata.TECx.BASETIME[ObjectID] = value;
                mapEditor.SetDirty();
                OnPropertyChanged("BASETIME");
            }
        }
        public ushort ENERGY
        {
            get
            {
                return mapEditor.mapdata.TECx.ENERGY[ObjectID];
            }
            set
            {
                mapEditor.mapdata.TECx.ENERGY[ObjectID] = value;
                mapEditor.SetDirty();
                OnPropertyChanged("ENERGY");
            }
        }



        public void AddDEFAULTCOLOR()
        {
            byte mvar = mapEditor.mapdata.PTEx.DEFAULTMAXLEVEL[ObjectID];
            byte svar = mapEditor.mapdata.PTEx.DEFAULTSTARTLEVEL[ObjectID];


            if (mvar == 0 & svar == 0)
            {
                mvar = 1; svar = 0;
            }
            else if (mvar == 1 & svar == 1)
            {
                mvar = 0; svar = 0;
            }
            else if (mvar == 1 & svar == 0)
            {
                mvar = 1; svar = 1;
            }


            mapEditor.mapdata.PTEx.DEFAULTMAXLEVEL[ObjectID] = mvar;
            mapEditor.mapdata.PTEx.DEFAULTSTARTLEVEL[ObjectID] = svar;


            OnPropertyChanged("UNITDEFAULTCOLOR");
        }

        public System.Windows.Media.Brush UNITDEFAULTCOLOR
        {
            get
            {
                byte mvar = mapEditor.mapdata.PTEx.DEFAULTMAXLEVEL[ObjectID];
                byte svar = mapEditor.mapdata.PTEx.DEFAULTSTARTLEVEL[ObjectID];


                if(mvar == 0 & svar  == 0)
                {
                    return System.Windows.Media.Brushes.Red;
                }
                else if (mvar == 1 & svar == 1)
                {
                    return System.Windows.Media.Brushes.Green;
                }
                else if (mvar == 1 & svar == 0)
                {
                    return System.Windows.Media.Brushes.Orange;
                }

                return System.Windows.Media.Brushes.Red;
            }
            set { }
        }



        public void AddPLAYERCOLOR(int Player)
        {
            byte mvar = mapEditor.mapdata.PTEx.MAXLEVEL[Player][ObjectID];
            byte svar = mapEditor.mapdata.PTEx.STARTLEVEL[Player][ObjectID];

            byte dvar = mapEditor.mapdata.PTEx.USEDEFAULT[Player][ObjectID];





            if (mvar == 0 & svar == 0 & dvar == 0)
            {
                dvar = 1;
            }
            else if (dvar == 1)
            {
                mvar = 1; svar = 0; dvar = 0;
            }
            else if (mvar == 1 & svar == 0)
            {
                mvar = 1; svar = 1;
            }
            else if (mvar == 1 & svar == 1)
            {
                mvar = 0; svar = 0;
            }
            else
            {
                mvar = 0; svar = 0; dvar = 0;
            }



            mapEditor.mapdata.PTEx.MAXLEVEL[Player][ObjectID] = mvar;
            mapEditor.mapdata.PTEx.STARTLEVEL[Player][ObjectID] = svar;

            mapEditor.mapdata.PTEx.USEDEFAULT[Player][ObjectID] = dvar;




            OnPropertyChanged("UNITENABLECOLOR" + Player);
        }


        public System.Windows.Media.Brush UNITENABLECOLOR0
        {
            get
            {
                int PLAYERID = 0;


                byte svar = mapEditor.mapdata.PTEx.STARTLEVEL[PLAYERID][ObjectID];
                byte mvar = mapEditor.mapdata.PTEx.MAXLEVEL[PLAYERID][ObjectID];


                byte dvar = mapEditor.mapdata.PTEx.USEDEFAULT[PLAYERID][ObjectID];

                if (dvar == 1)
                {
                    return System.Windows.Media.Brushes.Gray;
                }



                if (mvar == 0 & svar == 0)
                {
                    return System.Windows.Media.Brushes.Red;
                }
                else if (mvar == 1 & svar == 1)
                {
                    return System.Windows.Media.Brushes.Green;
                }
                else if (mvar == 1 & svar == 0)
                {
                    return System.Windows.Media.Brushes.Orange;
                }


                return System.Windows.Media.Brushes.Red;
            }
            set { }
        }
        public System.Windows.Media.Brush UNITENABLECOLOR1
        {
            get
            {
                int PLAYERID = 1;


                byte svar = mapEditor.mapdata.PTEx.STARTLEVEL[PLAYERID][ObjectID];
                byte mvar = mapEditor.mapdata.PTEx.MAXLEVEL[PLAYERID][ObjectID];


                byte dvar = mapEditor.mapdata.PTEx.USEDEFAULT[PLAYERID][ObjectID];

                if (dvar == 1)
                {
                    return System.Windows.Media.Brushes.Gray;
                }



                if (mvar == 0 & svar == 0)
                {
                    return System.Windows.Media.Brushes.Red;
                }
                else if (mvar == 1 & svar == 1)
                {
                    return System.Windows.Media.Brushes.Green;
                }
                else if (mvar == 1 & svar == 0)
                {
                    return System.Windows.Media.Brushes.Orange;
                }


                return System.Windows.Media.Brushes.Red;
            }
            set { }
        }


        public System.Windows.Media.Brush UNITENABLECOLOR2
        {
            get
            {
                int PLAYERID = 2;


                byte svar = mapEditor.mapdata.PTEx.STARTLEVEL[PLAYERID][ObjectID];
                byte mvar = mapEditor.mapdata.PTEx.MAXLEVEL[PLAYERID][ObjectID];


                byte dvar = mapEditor.mapdata.PTEx.USEDEFAULT[PLAYERID][ObjectID];

                if (dvar == 1)
                {
                    return System.Windows.Media.Brushes.Gray;
                }



                if (mvar == 0 & svar == 0)
                {
                    return System.Windows.Media.Brushes.Red;
                }
                else if (mvar == 1 & svar == 1)
                {
                    return System.Windows.Media.Brushes.Green;
                }
                else if (mvar == 1 & svar == 0)
                {
                    return System.Windows.Media.Brushes.Orange;
                }


                return System.Windows.Media.Brushes.Red;
            }
            set { }
        }

        public System.Windows.Media.Brush UNITENABLECOLOR3
        {
            get
            {
                int PLAYERID = 3;


                byte svar = mapEditor.mapdata.PTEx.STARTLEVEL[PLAYERID][ObjectID];
                byte mvar = mapEditor.mapdata.PTEx.MAXLEVEL[PLAYERID][ObjectID];


                byte dvar = mapEditor.mapdata.PTEx.USEDEFAULT[PLAYERID][ObjectID];

                if (dvar == 1)
                {
                    return System.Windows.Media.Brushes.Gray;
                }



                if (mvar == 0 & svar == 0)
                {
                    return System.Windows.Media.Brushes.Red;
                }
                else if (mvar == 1 & svar == 1)
                {
                    return System.Windows.Media.Brushes.Green;
                }
                else if (mvar == 1 & svar == 0)
                {
                    return System.Windows.Media.Brushes.Orange;
                }


                return System.Windows.Media.Brushes.Red;
            }
            set { }
        }


        public System.Windows.Media.Brush UNITENABLECOLOR4
        {
            get
            {
                int PLAYERID = 4;


                byte svar = mapEditor.mapdata.PTEx.STARTLEVEL[PLAYERID][ObjectID];
                byte mvar = mapEditor.mapdata.PTEx.MAXLEVEL[PLAYERID][ObjectID];


                byte dvar = mapEditor.mapdata.PTEx.USEDEFAULT[PLAYERID][ObjectID];

                if (dvar == 1)
                {
                    return System.Windows.Media.Brushes.Gray;
                }



                if (mvar == 0 & svar == 0)
                {
                    return System.Windows.Media.Brushes.Red;
                }
                else if (mvar == 1 & svar == 1)
                {
                    return System.Windows.Media.Brushes.Green;
                }
                else if (mvar == 1 & svar == 0)
                {
                    return System.Windows.Media.Brushes.Orange;
                }


                return System.Windows.Media.Brushes.Red;
            }
            set { }
        }

        public System.Windows.Media.Brush UNITENABLECOLOR5
        {
            get
            {
                int PLAYERID = 5;


                byte svar = mapEditor.mapdata.PTEx.STARTLEVEL[PLAYERID][ObjectID];
                byte mvar = mapEditor.mapdata.PTEx.MAXLEVEL[PLAYERID][ObjectID];


                byte dvar = mapEditor.mapdata.PTEx.USEDEFAULT[PLAYERID][ObjectID];

                if (dvar == 1)
                {
                    return System.Windows.Media.Brushes.Gray;
                }



                if (mvar == 0 & svar == 0)
                {
                    return System.Windows.Media.Brushes.Red;
                }
                else if (mvar == 1 & svar == 1)
                {
                    return System.Windows.Media.Brushes.Green;
                }
                else if (mvar == 1 & svar == 0)
                {
                    return System.Windows.Media.Brushes.Orange;
                }


                return System.Windows.Media.Brushes.Red;
            }
            set { }
        }


        public System.Windows.Media.Brush UNITENABLECOLOR6
        {
            get
            {
                int PLAYERID = 6;


                byte svar = mapEditor.mapdata.PTEx.STARTLEVEL[PLAYERID][ObjectID];
                byte mvar = mapEditor.mapdata.PTEx.MAXLEVEL[PLAYERID][ObjectID];


                byte dvar = mapEditor.mapdata.PTEx.USEDEFAULT[PLAYERID][ObjectID];

                if (dvar == 1)
                {
                    return System.Windows.Media.Brushes.Gray;
                }



                if (mvar == 0 & svar == 0)
                {
                    return System.Windows.Media.Brushes.Red;
                }
                else if (mvar == 1 & svar == 1)
                {
                    return System.Windows.Media.Brushes.Green;
                }
                else if (mvar == 1 & svar == 0)
                {
                    return System.Windows.Media.Brushes.Orange;
                }


                return System.Windows.Media.Brushes.Red;
            }
            set { }
        }

        public System.Windows.Media.Brush UNITENABLECOLOR7
        {
            get
            {
                int PLAYERID = 7;


                byte svar = mapEditor.mapdata.PTEx.STARTLEVEL[PLAYERID][ObjectID];
                byte mvar = mapEditor.mapdata.PTEx.MAXLEVEL[PLAYERID][ObjectID];


                byte dvar = mapEditor.mapdata.PTEx.USEDEFAULT[PLAYERID][ObjectID];

                if (dvar == 1)
                {
                    return System.Windows.Media.Brushes.Gray;
                }



                if (mvar == 0 & svar == 0)
                {
                    return System.Windows.Media.Brushes.Red;
                }
                else if (mvar == 1 & svar == 1)
                {
                    return System.Windows.Media.Brushes.Green;
                }
                else if (mvar == 1 & svar == 0)
                {
                    return System.Windows.Media.Brushes.Orange;
                }


                return System.Windows.Media.Brushes.Red;
            }
            set { }
        }











        public void PropertyChangeAll()
        {
            UIPropertyChange();
            OnPropertyChanged("MIN");
            OnPropertyChanged("GAS");
            OnPropertyChanged("BASETIME");
            OnPropertyChanged("ENERGY");
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
