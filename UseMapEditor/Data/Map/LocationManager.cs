using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseMapEditor.Control;
using UseMapEditor.Task;

namespace Data.Map
{
    public partial class MapData
    {
        public ObservableCollection<LocationData> LocationDatas = new ObservableCollection<LocationData>();

        public bool CheckIndexDuplication(int index)
        {
            return LocationDatas.Select(x => x.INDEX == index).Count() == 0;
        }

        public class LocationData : INotifyPropertyChanged
        {
            public MapEditor mapEditor;


            public void PropertyChangeAll()
            {
                OnPropertyChanged("X");
                OnPropertyChanged("Y");
                OnPropertyChanged("WIDTH");
                OnPropertyChanged("HEIGHT");
                OnPropertyChanged("NAME");

                OnPropertyChanged("LowGround");
                OnPropertyChanged("MediGround");
                OnPropertyChanged("HighGround");
                OnPropertyChanged("LowAir");
                OnPropertyChanged("MediAir");
                OnPropertyChanged("HighAir");
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




            private int index;
            public int INDEX
            {
                get
                {
                    return index;
                }
                set
                {
                    mapEditor.taskManager.TaskAdd(new LocationEvent(mapEditor, this, "INDEX", value, index));
                    index = value;
                    mapEditor.SetDirty();
                    OnPropertyChanged("INDEX");
                }
            }




            public uint L, R, T, B;

            public uint X
            {
                get
                {
                    return Math.Min(L, R);
                }
                set
                {
                    uint oldvalue = X;
                    if(L < R)
                    {
                        int w = (int)(R - L);
                        L = value;
                        R = (uint)(L + w);
                    }
                    else
                    {
                        int w = (int)(L - R);
                        R = value;
                        L = (uint)(R + w);
                    }


                    mapEditor.taskManager.TaskAdd(new LocationEvent(mapEditor, this, "X", X, oldvalue));
                    mapEditor.SetDirty();
                    OnPropertyChanged("X");
                }
            }

            public uint Y
            {
                get
                {
                    return Math.Min(T, B);
                }
                set
                {
                    uint oldvalue = Y;
                    if (T < B)
                    {
                        int w = (int)(B - T);
                        T = value;
                        B = (uint)(T + w);
                    }
                    else
                    {
                        int w = (int)(T - B);
                        B = value;
                        T = (uint)(B + w);
                    }

                    mapEditor.taskManager.TaskAdd(new LocationEvent(mapEditor, this, "Y", Y, oldvalue));
                    mapEditor.SetDirty();
                    OnPropertyChanged("Y");
                }
            }
            public int WIDTH
            {
                get
                {
                    return (int)R - (int)L;
                }
                set
                {
                    int oldvalue = WIDTH;
                    int x = (int)X;

                    if (value > 0)
                    {
                        //정방향
                        L = (uint)x;
                        R = (uint)(value + L);
                    }
                    else
                    {
                        //역방향
                        R = (uint)x;
                        L = (uint)(Math.Abs(value) + R);
                    }


                    mapEditor.taskManager.TaskAdd(new LocationEvent(mapEditor, this, "WIDTH", WIDTH, oldvalue));
                    mapEditor.SetDirty();
                    OnPropertyChanged("WIDTH");
                }
            }
            public int HEIGHT
            {
                get
                {
                    return (int)B - (int)T;
                }
                set
                {
                    int oldvalue = HEIGHT;
                    int y = (int)Y;

                    if (value > 0)
                    {
                        //정방향
                        T = (uint)y;
                        B = (uint)(value + T);
                    }
                    else
                    {
                        //역방향
                        B = (uint)y;
                        T = (uint)(Math.Abs(value) + B);
                    }


                    mapEditor.taskManager.TaskAdd(new LocationEvent(mapEditor, this, "HEIGHT", HEIGHT, oldvalue));
                    mapEditor.SetDirty();
                    OnPropertyChanged("HEIGHT");
                }
            }





            public StringData STRING;

            public string NAME
            {
                get
                {
                    return STRING.String;
                }
                set
                {
                    mapEditor.taskManager.TaskAdd(new LocationEvent(mapEditor, this, "NAME", value, STRING.String));
                    STRING.String = value;
                    mapEditor.SetDirty();
                    OnPropertyChanged("NAME");
                }
            }


            private ushort _flag;
            public ushort FLAG
            {
                get
                {
                    return _flag;
                }
                set
                {
                    mapEditor.taskManager.TaskAdd(new LocationEvent(mapEditor, this, "FLAG", value, _flag));
                    _flag = value;
                    mapEditor.SetDirty();
                    OnPropertyChanged("LowGround");
                    OnPropertyChanged("MediGround");
                    OnPropertyChanged("HighGround");

                    OnPropertyChanged("LowAir");
                    OnPropertyChanged("MediAir");
                    OnPropertyChanged("HighAir");
                }
            }


            public bool LowGround
            {
                get
                {
                    return ((FLAG & 0b1 << 0) == 0);
                }
                set
                {
                    if (!value)
                    {
                        FLAG |= 0b1 << 0;
                    }
                    else
                    {
                        FLAG = (ushort)(FLAG & ~(0b1 << 0));
                    }
                }
            }

            public bool MediGround
            {
                get
                {
                    return ((FLAG & 0b1 << 1) == 0);
                }
                set
                {
                    if (!value)
                    {
                        FLAG |= 0b1 << 1;
                    }
                    else
                    {
                        FLAG = (ushort)(FLAG & ~(0b1 << 1));
                    }
                }
            }

            public bool HighGround
            {
                get
                {
                    return ((FLAG & 0b1 << 2) == 0);
                }
                set
                {
                    if (!value)
                    {
                        FLAG |= 0b1 << 2;
                    }
                    else
                    {
                        FLAG = (ushort)(FLAG & ~(0b1 << 2));
                    }
                }
            }





            public bool LowAir
            {
                get
                {
                    return ((FLAG & 0b1 << 3) == 0);
                }
                set
                {
                    if (!value)
                    {
                        FLAG |= 0b1 << 3;
                    }
                    else
                    {
                        FLAG = (ushort)(FLAG & ~(0b1 << 3));
                    }
                }
            }

            public bool MediAir
            {
                get
                {
                    return ((FLAG & 0b1 << 4) == 0);
                }
                set
                {
                    if (!value)
                    {
                        FLAG |= 0b1 << 4;
                    }
                    else
                    {
                        FLAG = (ushort)(FLAG & ~(0b1 << 4));
                    }
                }
            }

            public bool HighAir
            {
                get
                {
                    return ((FLAG & 0b1 << 5) == 0);
                }
                set
                {
                    if (!value)
                    {
                        FLAG |= 0b1 << 5;
                    }
                    else
                    {
                        FLAG = (ushort)(FLAG & ~(0b1 << 5));
                    }
                }
            }







            public Color RnColor;

            public LocationData(MapEditor mapEditor)
            {
                this.mapEditor = mapEditor;

                STRING = new StringData(mapEditor.mapdata);
                RnColor = new Color((uint)UseMapEditor.Global.WindowTool.random.Next());

                RnColor.A = 32;
            }

            //Bit 0 - Low elevation
            //Bit 1 - Medium elevation
            //Bit 2 - High elevation
            //Bit 3 - Low air
            //Bit 4 - Medium air
            //Bit 5 - High air
            //Bit 6 - 15 - Unused
        }

    }
}
