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
using static UseMapEditor.DataBinding.MapDataBinding;

namespace UseMapEditor.DataBinding
{
    public class UnitDataBinding : INotifyPropertyChanged
    {
        MapEditor mapEditor;
        public int ObjectID { get; set; }



        public UnitDataBinding(MapEditor _mapEditor, int _ObjectID)
        {
            mapEditor = _mapEditor;
            ObjectID = _ObjectID;

            UsePlayerCode = new PlayerCode(mapEditor, this, ObjectID);
        }


        public double MainNameSize
        {
            get
            {
                if (mapEditor.mapdata.IsCustomUnitName(ObjectID))
                {
                    return 8;
                }
                else
                {
                    return 15;
                }

            }
            set { }
        }
        public System.Windows.Visibility SecondNameVisble
        {
            get
            {
                if (mapEditor.mapdata.IsCustomUnitName(ObjectID))
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                {
                    return System.Windows.Visibility.Collapsed;
                }
            }
            set { }
        }


        public string SecondName
        {
            get
            {
                return mapEditor.mapdata.GetUnitName(ObjectID);
            }
            set { }
        }
        public string MainName
        {
            get
            {
                return mapEditor.mapdata.GetMapUnitName(ObjectID);
            }
            set { }
        }

        public string AlphaName
        {
            get
            {
                return UnitName + "[" + ObjectID.ToString().PadLeft(3, '0') + "]";
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
                        using (FileStream fileStream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"\CascData\cmdicons\" + ObjectID + ".png", FileMode.Open))
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



        public string UnitName
        {
            get
            {
                return mapEditor.mapdata.GetCodeName(Data.Map.MapData.Codetype.Unit, ObjectID);
            }
            set
            {
            }
        }




        public bool USESTRDEFAULT
        {
            get
            {
                return mapEditor.mapdata.IsCustomUnitName(ObjectID);
            }
            set
            {
                if (value)
                {
                    mapEditor.mapdata.UNIx.STRING[ObjectID].String = mapEditor.mapdata.GetMapUnitName(ObjectID);
                }
                else
                {
                    mapEditor.mapdata.UNIx.STRING[ObjectID].UnLoaded();
                }

                mapEditor.SetDirty();
                OnPropertyChanged("USESTRDEFAULT");
                OnPropertyChanged("STRING");
                OnPropertyChanged("UnitName");
                UIPropertyChange();
            }
        }
        public string STRING
        {
            get
            {
                string d = mapEditor.mapdata.UNIx.STRING[ObjectID].String;

                if (mapEditor.mapdata.UNIx.STRING[ObjectID].IsLoaded)
                {
                    return d;
                }
                else
                {
                    return mapEditor.mapdata.GetMapUnitName(ObjectID);
                }
            }
            set
            {
                mapEditor.mapdata.UNIx.STRING[ObjectID].String = value;

                mapEditor.SetDirty();

                OnPropertyChanged("STRING");
                OnPropertyChanged("UnitName");
                UIPropertyChange();
            }
        }




        public bool USEDEFAULT
        {
            get
            {

                return (mapEditor.mapdata.UNIx.USEDEFAULT[ObjectID] == 0);
            }
            set
            {
                if (value)
                {
                    mapEditor.mapdata.UNIx.USEDEFAULT[ObjectID] = 0;
                    //값들을 초기화해야됨.
                    HIT = "S" + Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Hit Points", ObjectID).Data.ToString();
                    BUILDTIME = (ushort)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Build Time", ObjectID).Data;
                    SHIELD = (ushort)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Shield Amount", ObjectID).Data;
                    MIN = (ushort)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Mineral Cost", ObjectID).Data;
                    ARMOR = (byte)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Armor Upgrade", ObjectID).Data;
                    GAS = (ushort)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Vespene Cost", ObjectID).Data;
                }
                else
                {
                    mapEditor.mapdata.UNIx.USEDEFAULT[ObjectID] = 1;
                    HIT = "S" + Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Hit Points", ObjectID).Data.ToString();
                    BUILDTIME = (ushort)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Build Time", ObjectID).Data;
                    SHIELD = (ushort)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Shield Amount", ObjectID).Data;
                    MIN = (ushort)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Mineral Cost", ObjectID).Data;
                    ARMOR = (byte)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Armor Upgrade", ObjectID).Data;
                    GAS = (ushort)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Vespene Cost", ObjectID).Data;


                    mapEditor.mapdata.UNIx.STRING[ObjectID].UnLoaded();
                    OnPropertyChanged("STRING");
                    UIPropertyChange();

                    OnPropertyChanged("UnitName");
                    OnPropertyChanged("USESTRDEFAULT");
                }
                mapEditor.SetDirty();


                OnPropertyChanged("USEDEFAULT");
            }
        }


        public string HIT
        {
            get
            {
                uint point = mapEditor.mapdata.UNIx.HIT[ObjectID];

                uint HP = point >> 8;
                uint DOT = point & 0xFF;
                if (DOT == 0)
                {
                    return HP.ToString();
                }
                else
                {
                    return HP + "." + DOT;
                }

            }
            set
            {
                string rvalue = value;


                if (rvalue.Length > 0)
                {
                    if (rvalue[0] == 'S')
                    {
                        rvalue = rvalue.Substring(1);

                        uint rval;
                        if (uint.TryParse(rvalue, out rval))
                        {
                            uint HP = rval >> 8;
                            uint DOT = rval & 0xFF;
                            if (DOT == 0)
                            {
                                rvalue = HP.ToString();
                            }
                            else
                            {
                                rvalue = HP + "." + DOT;
                            }
                        }
                    }
                }




                string[] vals = rvalue.Split('.');

                if (vals.Length == 1)
                {
                    uint rval;
                    if (uint.TryParse(vals[0], out rval))
                    {
                        if ((rval) > 0xFFFFFFL)
                        {
                            rval = 0xFFFFFF;
                        }

                        mapEditor.mapdata.UNIx.HIT[ObjectID] = rval * 256;
                        mapEditor.SetDirty();
                        OnPropertyChanged("HIT");
                    }
                }
                else if (vals.Length == 2)
                {
                    uint rval;
                    if (uint.TryParse(vals[0], out rval))
                    {
                        uint rdotval;
                        if (uint.TryParse(vals[1], out rdotval))
                        {
                            mapEditor.mapdata.UNIx.HIT[ObjectID] = rval * 256 + rdotval;
                            mapEditor.SetDirty();
                            OnPropertyChanged("HIT");
                        }
                    }
                }
            }
        }
        public ushort SHIELD
        {
            get
            {
                return mapEditor.mapdata.UNIx.SHIELD[ObjectID];
            }
            set
            {
                mapEditor.mapdata.UNIx.SHIELD[ObjectID] = value;
                mapEditor.SetDirty();
                OnPropertyChanged("SHIELD");
            }
        }
        public byte ARMOR
        {
            get
            {
                return mapEditor.mapdata.UNIx.ARMOR[ObjectID];
            }
            set
            {
                mapEditor.mapdata.UNIx.ARMOR[ObjectID] = value;
                mapEditor.SetDirty();
                OnPropertyChanged("ARMOR");
            }
        }
        public ushort BUILDTIME
        {
            get
            {
                return mapEditor.mapdata.UNIx.BUILDTIME[ObjectID];
            }
            set
            {
                mapEditor.mapdata.UNIx.BUILDTIME[ObjectID] = value;
                mapEditor.SetDirty();
                OnPropertyChanged("BUILDTIME");
            }
        }
        public ushort MIN
        {
            get
            {
                return mapEditor.mapdata.UNIx.MIN[ObjectID];
            }
            set
            {
                mapEditor.mapdata.UNIx.MIN[ObjectID] = value;
                mapEditor.SetDirty();
                OnPropertyChanged("MIN");
            }
        }
        public ushort GAS
        {
            get
            {
                return mapEditor.mapdata.UNIx.GAS[ObjectID];
            }
            set
            {
                mapEditor.mapdata.UNIx.GAS[ObjectID] = value;
                mapEditor.SetDirty();
                OnPropertyChanged("GAS");
            }
        }


        public System.Windows.Visibility GWEAPONEXIST
        {
            get
            {
                long WeaponNum = Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Ground Weapon", ObjectID).Data;

                if (WeaponNum == 130)
                {
                    return System.Windows.Visibility.Collapsed;
                }
                else
                {
                    return System.Windows.Visibility.Visible;
                }
            }
        }
        public System.Windows.Visibility AWEAPONEXIST
        {
            get
            {
                long WeaponNum = Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Air Weapon", ObjectID).Data;

                if (WeaponNum == 130)
                {
                    return System.Windows.Visibility.Collapsed;
                }
                else
                {
                    return System.Windows.Visibility.Visible;
                }
            }
        }


        public ushort GDMG
        {
            get
            {
                long WeaponNum = Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Ground Weapon", ObjectID).Data;
                if (WeaponNum == 130)
                {
                    return 0;
                }

                long Dmg = mapEditor.mapdata.UNIx.DMG[WeaponNum];

                return (ushort)Dmg;
            }
            set
            {
                long WeaponNum = Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Ground Weapon", ObjectID).Data;
                if (WeaponNum == 130)
                {
                    return;
                }
                mapEditor.mapdata.UNIx.DMG[WeaponNum] = value;
                mapEditor.SetDirty();
                OnPropertyChanged("GDMG");
                OnPropertyChanged("GBDMG");
                OnPropertyChanged("ADMG");
                OnPropertyChanged("ABDMG");
            }
        }
        public ushort GBDMG
        {
            get
            {
                long WeaponNum = Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Ground Weapon", ObjectID).Data;
                if (WeaponNum == 130)
                {
                    return 0;
                }
                long BounsDmg = mapEditor.mapdata.UNIx.BONUSDMG[WeaponNum];

                return (ushort)BounsDmg;
            }
            set
            {
                long WeaponNum = Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Ground Weapon", ObjectID).Data;
                if (WeaponNum == 130)
                {
                    return;
                }
                mapEditor.mapdata.UNIx.BONUSDMG[WeaponNum] = value;
                mapEditor.SetDirty();
                OnPropertyChanged("GDMG");
                OnPropertyChanged("GBDMG");
                OnPropertyChanged("ADMG");
                OnPropertyChanged("ABDMG");
            }
        }


        public ushort ADMG
        {
            get
            {
                long WeaponNum = Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Air Weapon", ObjectID).Data;
                if (WeaponNum == 130)
                {
                    return 0;
                }
                long Dmg = mapEditor.mapdata.UNIx.DMG[WeaponNum];

                return (ushort)Dmg;
            }
            set
            {
                long WeaponNum = Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Air Weapon", ObjectID).Data;
                if (WeaponNum == 130)
                {
                    return;
                }
                mapEditor.mapdata.UNIx.DMG[WeaponNum] = value;
                mapEditor.SetDirty();
                OnPropertyChanged("GDMG");
                OnPropertyChanged("GBDMG");
                OnPropertyChanged("ADMG");
                OnPropertyChanged("ABDMG");
            }
        }
        public ushort ABDMG
        {
            get
            {
                long WeaponNum = Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Air Weapon", ObjectID).Data;
                if (WeaponNum == 130)
                {
                    return 0;
                }
                long BounsDmg = mapEditor.mapdata.UNIx.BONUSDMG[WeaponNum];

                return (ushort)BounsDmg;
            }
            set
            {
                long WeaponNum = Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Air Weapon", ObjectID).Data;
                if (WeaponNum == 130)
                {
                    return;
                }
                mapEditor.mapdata.UNIx.BONUSDMG[WeaponNum] = value;
                mapEditor.SetDirty();
                OnPropertyChanged("GDMG");
                OnPropertyChanged("GBDMG");
                OnPropertyChanged("ADMG");
                OnPropertyChanged("ABDMG");
            }
        }



        public void AddDEFAULTCOLOR()
        {
            if (mapEditor.mapdata.PUNI.DEFAULT[ObjectID] == 0)
            {
                mapEditor.mapdata.PUNI.DEFAULT[ObjectID] = 1;
            }
            else
            {
                mapEditor.mapdata.PUNI.DEFAULT[ObjectID] = 0;
            }


            mapEditor.SetDirty();
            OnPropertyChanged("UNITDEFAULTCOLOR");
        }

        public void AddPLAYERCOLOR(int Player)
        {
            int evar = mapEditor.mapdata.PUNI.UNITENABLED[Player][ObjectID];
            int dvar = mapEditor.mapdata.PUNI.USEDEFAULT[Player][ObjectID];

            if (evar == 0 & dvar == 0)
            {
                evar = 1; dvar = 1;
            }
            else if (evar == 1 & dvar == 0)
            {
                evar = 0; dvar = 0;
            }
            else
            {
                evar = 1; dvar = 0;
            }
            mapEditor.mapdata.PUNI.UNITENABLED[Player][ObjectID] = (byte)evar;
            mapEditor.mapdata.PUNI.USEDEFAULT[Player][ObjectID] = (byte)dvar;


            mapEditor.SetDirty();
            OnPropertyChanged("UNITENABLECOLOR" + Player);
        }




        public UserDefaultCode UseDefaultCode
        {
            get
            {
                if (mapEditor.mapdata.PUNI.DEFAULT[ObjectID] == 0)
                {
                    return UserDefaultCode.Unusable;
                }
                else
                {
                    return UserDefaultCode.Usable;
                }
            }
            set
            {
                if (value == UserDefaultCode.Unusable)
                {
                    mapEditor.mapdata.PUNI.DEFAULT[ObjectID] = 0;
                }
                else
                {
                    mapEditor.mapdata.PUNI.DEFAULT[ObjectID] = 1;
                }
                OnPropertyChanged("UNITDEFAULTCOLOR");
            }
        }

        public class PlayerCode
        {
            private UnitDataBinding UnitDataBinding;
            private MapEditor mapEditor;
            private int ObjectID;
            public PlayerCode(MapEditor mapEditor, UnitDataBinding UnitDataBinding, int ObjectID)
            {
                this.UnitDataBinding = UnitDataBinding;
                this.mapEditor = mapEditor;
                this.ObjectID = ObjectID;
            }

            public UserDefaultCode this[int player]
            {
                get
                {
                    int evar = mapEditor.mapdata.PUNI.UNITENABLED[player][ObjectID];
                    int dvar = mapEditor.mapdata.PUNI.USEDEFAULT[player][ObjectID];

                    if (evar == 0 & dvar == 0)
                    {
                        return UserDefaultCode.Unusable;
                    }
                    else if (evar == 1 & dvar == 0)
                    {
                        return UserDefaultCode.Usable;
                    }
                    else
                    {
                        return UserDefaultCode.Default;
                    }
                }
                set
                {
                    switch (value)
                    {
                        case UserDefaultCode.Unusable:
                            mapEditor.mapdata.PUNI.UNITENABLED[player][ObjectID] = 0;
                            mapEditor.mapdata.PUNI.USEDEFAULT[player][ObjectID] = 0;
                            break;
                        case UserDefaultCode.Usable:
                            mapEditor.mapdata.PUNI.UNITENABLED[player][ObjectID] = 1;
                            mapEditor.mapdata.PUNI.USEDEFAULT[player][ObjectID] = 0;
                            break;
                        case UserDefaultCode.Default:
                            mapEditor.mapdata.PUNI.USEDEFAULT[player][ObjectID] = 1;
                            break;
                    }

                    UnitDataBinding.OnPropertyChanged("UNITENABLECOLOR" + player);
                }
            }
        }

        public PlayerCode UsePlayerCode;



        public System.Windows.Media.Brush UNITDEFAULTCOLOR
        {
            get
            {
                if (mapEditor.mapdata.PUNI.DEFAULT[ObjectID] == 0)
                {
                    return System.Windows.Media.Brushes.Red;
                }
                else
                {
                    return System.Windows.Media.Brushes.Green;
                }
            }
            set{}
        }
        public System.Windows.Media.Brush UNITENABLECOLOR0
        {
            get
            {
                int PLAYERID = 0;

                int evar = mapEditor.mapdata.PUNI.UNITENABLED[PLAYERID][ObjectID];
                int dvar = mapEditor.mapdata.PUNI.USEDEFAULT[PLAYERID][ObjectID];

                if (evar == 0 & dvar == 0)
                {
                    return System.Windows.Media.Brushes.Red;
                }
                else if (evar == 1 & dvar == 0)
                {
                    return System.Windows.Media.Brushes.Green;
                }
                else
                {
                    return System.Windows.Media.Brushes.Gray;
                }
            }
            set { }
        }
        public System.Windows.Media.Brush UNITENABLECOLOR1
        {
            get
            {
                int PLAYERID = 1;

                int evar = mapEditor.mapdata.PUNI.UNITENABLED[PLAYERID][ObjectID];
                int dvar = mapEditor.mapdata.PUNI.USEDEFAULT[PLAYERID][ObjectID];

                if (evar == 0 & dvar == 0)
                {
                    return System.Windows.Media.Brushes.Red;
                }
                else if (evar == 1 & dvar == 0)
                {
                    return System.Windows.Media.Brushes.Green;
                }
                else
                {
                    return System.Windows.Media.Brushes.Gray;
                }
            }
            set { }
        }
        public System.Windows.Media.Brush UNITENABLECOLOR2
        {
            get
            {
                int PLAYERID = 2;

                int evar = mapEditor.mapdata.PUNI.UNITENABLED[PLAYERID][ObjectID];
                int dvar = mapEditor.mapdata.PUNI.USEDEFAULT[PLAYERID][ObjectID];

                if (evar == 0 & dvar == 0)
                {
                    return System.Windows.Media.Brushes.Red;
                }
                else if (evar == 1 & dvar == 0)
                {
                    return System.Windows.Media.Brushes.Green;
                }
                else
                {
                    return System.Windows.Media.Brushes.Gray;
                }
            }
            set { }
        }
        public System.Windows.Media.Brush UNITENABLECOLOR3
        {
            get
            {
                int PLAYERID = 3;

                int evar = mapEditor.mapdata.PUNI.UNITENABLED[PLAYERID][ObjectID];
                int dvar = mapEditor.mapdata.PUNI.USEDEFAULT[PLAYERID][ObjectID];

                if (evar == 0 & dvar == 0)
                {
                    return System.Windows.Media.Brushes.Red;
                }
                else if (evar == 1 & dvar == 0)
                {
                    return System.Windows.Media.Brushes.Green;
                }
                else
                {
                    return System.Windows.Media.Brushes.Gray;
                }
            }
            set { }
        }
        public System.Windows.Media.Brush UNITENABLECOLOR4
        {
            get
            {
                int PLAYERID = 4;

                int evar = mapEditor.mapdata.PUNI.UNITENABLED[PLAYERID][ObjectID];
                int dvar = mapEditor.mapdata.PUNI.USEDEFAULT[PLAYERID][ObjectID];

                if (evar == 0 & dvar == 0)
                {
                    return System.Windows.Media.Brushes.Red;
                }
                else if (evar == 1 & dvar == 0)
                {
                    return System.Windows.Media.Brushes.Green;
                }
                else
                {
                    return System.Windows.Media.Brushes.Gray;
                }
            }
            set { }
        }
        public System.Windows.Media.Brush UNITENABLECOLOR5
        {
            get
            {
                int PLAYERID = 5;

                int evar = mapEditor.mapdata.PUNI.UNITENABLED[PLAYERID][ObjectID];
                int dvar = mapEditor.mapdata.PUNI.USEDEFAULT[PLAYERID][ObjectID];

                if (evar == 0 & dvar == 0)
                {
                    return System.Windows.Media.Brushes.Red;
                }
                else if (evar == 1 & dvar == 0)
                {
                    return System.Windows.Media.Brushes.Green;
                }
                else
                {
                    return System.Windows.Media.Brushes.Gray;
                }
            }
            set { }
        }
        public System.Windows.Media.Brush UNITENABLECOLOR6
        {
            get
            {
                int PLAYERID = 6;

                int evar = mapEditor.mapdata.PUNI.UNITENABLED[PLAYERID][ObjectID];
                int dvar = mapEditor.mapdata.PUNI.USEDEFAULT[PLAYERID][ObjectID];

                if (evar == 0 & dvar == 0)
                {
                    return System.Windows.Media.Brushes.Red;
                }
                else if (evar == 1 & dvar == 0)
                {
                    return System.Windows.Media.Brushes.Green;
                }
                else
                {
                    return System.Windows.Media.Brushes.Gray;
                }
            }
            set { }
        }
        public System.Windows.Media.Brush UNITENABLECOLOR7
        {
            get
            {
                int PLAYERID = 7;

                int evar = mapEditor.mapdata.PUNI.UNITENABLED[PLAYERID][ObjectID];
                int dvar = mapEditor.mapdata.PUNI.USEDEFAULT[PLAYERID][ObjectID];

                if (evar == 0 & dvar == 0)
                {
                    return System.Windows.Media.Brushes.Red;
                }
                else if (evar == 1 & dvar == 0)
                {
                    return System.Windows.Media.Brushes.Green;
                }
                else
                {
                    return System.Windows.Media.Brushes.Gray;
                }
            }
            set { }
        }








        public void PropertyChangeAll()
        {
            OnPropertyChanged("UnitName");
            UIPropertyChange();

            OnPropertyChanged("USEDEFAULT");


            OnPropertyChanged("HIT");
            OnPropertyChanged("SHIELD");
            OnPropertyChanged("ARMOR");
            OnPropertyChanged("BUILDTIME");
            OnPropertyChanged("MIN");
            OnPropertyChanged("GAS");

            OnPropertyChanged("GDMG");
            OnPropertyChanged("GBDMG");
            OnPropertyChanged("ADMG");
            OnPropertyChanged("ABDMG");

            OnPropertyChanged("UNITDEFAULTCOLOR");

            for (int i = 0; i < 8; i++)
                OnPropertyChanged("UNITENABLECOLOR" + i);
        }



        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
