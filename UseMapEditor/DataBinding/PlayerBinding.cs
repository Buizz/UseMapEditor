using Data.Map;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using UseMapEditor.Control;
using static Data.Map.MapData;

namespace UseMapEditor.DataBinding
{
    public class PlayerBinding : INotifyPropertyChanged
    {
        MapEditor mapEditor;
        int PlayerID;



        public PlayerBinding(MapEditor _mapEditor, int _PlayerID)
        {
            mapEditor = _mapEditor;
            PlayerID = _PlayerID;
        }



        public void PropertyChangeAll()
        {
            OnPropertyChanged("Owner");
            OnPropertyChanged("Race");
            OnPropertyChanged("Color");
            OnPropertyChanged("BackColor");
        }


        public int Owner
        {
            get {
                byte cvalue = mapEditor.mapdata.IOWN[PlayerID];

                int cindex = 0;
                foreach (IOWNTYPE iOWNTYPE in Enum.GetValues(typeof(IOWNTYPE)))
                {
                    if(cvalue == (byte)iOWNTYPE)
                    {
                        return cindex;
                    }
                    cindex++;
                }
                return -1;
            }
            set
            {
                int cvalue = value;
                byte rvalue = 0;
                int cindex = 0;
                foreach (IOWNTYPE iOWNTYPE in Enum.GetValues(typeof(IOWNTYPE)))
                {
                    if (cvalue == cindex)
                    {
                        rvalue = (byte)iOWNTYPE;
                        break;
                    }
                    cindex++;
                }

                mapEditor.mapdata.IOWN[PlayerID] = rvalue;
                mapEditor.SetDirty();
                OnPropertyChanged("Owner");
            }
        }

        public int Race
        {
            get
            {
                byte cvalue = mapEditor.mapdata.SIDE[PlayerID];

                switch (cvalue)
                {
                    case 0:
                        return 0;
                    case 1:
                        return 1;
                    case 2:
                        return 2;
                    case 5:
                        return 3;
                    case 7:
                        return 4;
                }
                return -1;

                //int cindex = 0;
                //foreach (SIDETYPE sIDETYPE in Enum.GetValues(typeof(SIDETYPE)))
                //{
                //    if (cvalue == (byte)sIDETYPE)
                //    {
                //        return cindex;
                //    }
                //    cindex++;
                //}
                //return -1;
            }
            set
            {
                int cvalue = value;
                //byte rvalue = 0;
                //int cindex = 0;
                //foreach (IOWNTYPE sIDETYPE in Enum.GetValues(typeof(SIDETYPE)))
                //{
                //    if (cvalue == cindex)
                //    {
                //        rvalue = (byte)sIDETYPE;
                //        break;
                //    }
                //    cindex++;
                //}

                switch (cvalue)
                {
                    case 0:
                        cvalue = 0;
                        break;
                    case 1:
                        cvalue = 1;
                        break;
                    case 2:
                        cvalue = 2;
                        break;
                    case 3:
                        cvalue = 5;
                        break;
                    case 4:
                        cvalue = 7;
                        break;
                }


                mapEditor.mapdata.SIDE[PlayerID] = (byte)cvalue;
                mapEditor.SetDirty();
                OnPropertyChanged("Race");
            }
        }
        public int Color
        {
            get
            {
                if ((CRGBINDTYPE)mapEditor.mapdata.CRGBIND[PlayerID] == CRGBINDTYPE.UseCOLRselection)
                {
                    return mapEditor.mapdata.CRGB[PlayerID].B;
                }
                else
                {
                    return mapEditor.mapdata.CRGBIND[PlayerID] + 22;
                }
            }
            set
            {
                if (value <= 21)
                {
                    mapEditor.mapdata.CRGB[PlayerID].R = (byte)0;
                    mapEditor.mapdata.CRGB[PlayerID].G = (byte)0;
                    mapEditor.mapdata.CRGB[PlayerID].B = (byte)value;

                    //mapEditor.mapdata.COLR[PlayerID] = (byte)value;
                    mapEditor.mapdata.CRGBIND[PlayerID] = (byte)CRGBINDTYPE.UseCOLRselection;
                }
                else
                {
                    mapEditor.mapdata.CRGBIND[PlayerID] = (byte)(value - 22);
                }

                mapEditor.IsMinimapUnitRefresh = false;
                mapEditor.SetDirty();
                OnPropertyChanged("Color");
                OnPropertyChanged("BackColor");
            }
        }
        public Brush BackColor
        {
            get
            {
                if ((CRGBINDTYPE)mapEditor.mapdata.CRGBIND[PlayerID] == CRGBINDTYPE.UseCOLRselection)
                {
                    Microsoft.Xna.Framework.Color color = MapData.PlayerColors[mapEditor.mapdata.CRGB[PlayerID].B];


                    return new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
                }
                else
                {
                    if((CRGBINDTYPE)mapEditor.mapdata.CRGBIND[PlayerID] == CRGBINDTYPE.CustomRGBColor)
                    {

                        Microsoft.Xna.Framework.Color color = mapEditor.mapdata.CRGB[PlayerID];


                        return new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));

                    }
                    else
                    {
                        return null;
                    }


                }
            }
        }

        public Color IconColor
        {
            get
            {
                if ((CRGBINDTYPE)mapEditor.mapdata.CRGBIND[PlayerID] == CRGBINDTYPE.UseCOLRselection)
                {
                    Microsoft.Xna.Framework.Color color = MapData.PlayerColors[mapEditor.mapdata.CRGB[PlayerID].B];


                    return System.Windows.Media.Color.FromRgb(color.R, color.G, color.B);
                }
                else
                {
                    if ((CRGBINDTYPE)mapEditor.mapdata.CRGBIND[PlayerID] == CRGBINDTYPE.CustomRGBColor)
                    {

                        Microsoft.Xna.Framework.Color color = mapEditor.mapdata.CRGB[PlayerID];


                        return System.Windows.Media.Color.FromRgb(color.R, color.G, color.B);

                    }
                    else
                    {
                        return System.Windows.Media.Colors.Transparent;
                    }
                }
            }
        }




        public Microsoft.Xna.Framework.Color CustomColor
        {
            set
            {
                Color = 24;
                mapEditor.mapdata.CRGB[PlayerID] = value;
                mapEditor.IsMinimapUnitRefresh = false;
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
