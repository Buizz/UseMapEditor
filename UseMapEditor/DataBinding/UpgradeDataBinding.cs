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

namespace UseMapEditor.DataBinding
{
    public class UpgradeDataBinding : INotifyPropertyChanged
    {
        MapEditor mapEditor;
        public int ObjectID { get; set; }



        public UpgradeDataBinding(MapEditor _mapEditor, int _ObjectID)
        {
            mapEditor = _mapEditor;
            ObjectID = _ObjectID;

            for (int i = 0; i < 12; i++)
            {
                playerbind.Add(new UpgradePlayerDataBinding(mapEditor, i, ObjectID, this));
            }
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
                return mapEditor.GetCodeName(MapEditor.Codetype.Upgrade, ObjectID);
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
                        int icon = (ushort)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.upgrades, "Icon", ObjectID).Data;
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

                return (mapEditor.mapdata.UPGx.USEDEFAULT[ObjectID] == 0);
            }
            set
            {
                if (value)
                {
                    mapEditor.mapdata.UPGx.USEDEFAULT[ObjectID] = 0;
                    //값들을 초기화해야됨.
                     }
                else
                {
                    mapEditor.mapdata.UPGx.USEDEFAULT[ObjectID] = 1;
                }

                BASEMIN = (ushort)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.upgrades, "Mineral Cost Base", ObjectID).Data;
                BONUSMIN = (ushort)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.upgrades, "Mineral Cost Factor", ObjectID).Data;
                BASEGAS = (ushort)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.upgrades, "Vespene Cost Base", ObjectID).Data;
                BONUSGAS = (ushort)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.upgrades, "Vespene Cost Factor", ObjectID).Data;
                BASETIME = (byte)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.upgrades, "Research Time Base", ObjectID).Data;
                BONUSTIME = (ushort)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.upgrades, "Research Time Factor", ObjectID).Data;



                mapEditor.SetDirty();


                OnPropertyChanged("USEDEFAULT");
            }
        }

        public ushort BASEMIN
        {
            get
            {
                return mapEditor.mapdata.UPGx.BASEMIN[ObjectID];
            }
            set
            {
                mapEditor.mapdata.UPGx.BASEMIN[ObjectID] = value;
                mapEditor.SetDirty();
                OnPropertyChanged("BASEMIN");
            }
        }
        public ushort BONUSMIN
        {
            get
            {
                return mapEditor.mapdata.UPGx.BONUSMIN[ObjectID];
            }
            set
            {
                mapEditor.mapdata.UPGx.BONUSMIN[ObjectID] = value;
                mapEditor.SetDirty();
                OnPropertyChanged("BONUSMIN");
            }
        }
        public ushort BASEGAS
        {
            get
            {
                return mapEditor.mapdata.UPGx.BASEGAS[ObjectID];
            }
            set
            {
                mapEditor.mapdata.UPGx.BASEGAS[ObjectID] = value;
                mapEditor.SetDirty();
                OnPropertyChanged("BASEGAS");
            }
        }
        public ushort BONUSGAS
        {
            get
            {
                return mapEditor.mapdata.UPGx.BONUSGAS[ObjectID];
            }
            set
            {
                mapEditor.mapdata.UPGx.BONUSGAS[ObjectID] = value;
                mapEditor.SetDirty();
                OnPropertyChanged("BONUSGAS");
            }
        }
        public ushort BASETIME
        {
            get
            {
                return mapEditor.mapdata.UPGx.BASETIME[ObjectID];
            }
            set
            {
                mapEditor.mapdata.UPGx.BASETIME[ObjectID] = value;
                mapEditor.SetDirty();
                OnPropertyChanged("BASETIME");
            }
        }
        public ushort BONUSTIME
        {
            get
            {
                return mapEditor.mapdata.UPGx.BONUSTIME[ObjectID];
            }
            set
            {
                mapEditor.mapdata.UPGx.BONUSTIME[ObjectID] = value;
                mapEditor.SetDirty();
                OnPropertyChanged("BONUSTIME");
            }
        }



        public byte DEFAULTMAXLEVEL
        {
            get
            {
                return mapEditor.mapdata.PUPx.DEFAULTMAXLEVEL[ObjectID];
            }
            set
            {
                mapEditor.mapdata.PUPx.DEFAULTMAXLEVEL[ObjectID] = value;
                mapEditor.SetDirty();
                OnPropertyChanged("DEFAULTMAXLEVEL");
                PlayerDataPropertyChangeAll();
            }
        }
        public byte DEFAULTSTARTLEVEL
        {
            get
            {
                return mapEditor.mapdata.PUPx.DEFAULTSTARTLEVEL[ObjectID];
            }
            set
            {
                mapEditor.mapdata.PUPx.DEFAULTSTARTLEVEL[ObjectID] = value;
                mapEditor.SetDirty();
                OnPropertyChanged("DEFAULTSTARTLEVEL");
                PlayerDataPropertyChangeAll();
            }
        }


        public List<UpgradePlayerDataBinding> playerbind = new List<UpgradePlayerDataBinding>();
        public class UpgradePlayerDataBinding : INotifyPropertyChanged
        {
            public int player;
            public int objid;
            public MapEditor mapEditor;
            UpgradeDataBinding binding;

            public void AddDefault()
            {
                int evar = mapEditor.mapdata.PUPx.USEDEFAULT[player][objid];

                if (evar == 0)
                {
                    mapEditor.mapdata.PUPx.USEDEFAULT[player][objid] = 1;

                    STARTLEVEL = binding.DEFAULTSTARTLEVEL;
                    MAXLEVEL = binding.DEFAULTMAXLEVEL;
                }
                else
                {
                    mapEditor.mapdata.PUPx.USEDEFAULT[player][objid] = 0;
                }
                mapEditor.SetDirty();
                OnPropertyChanged("LEVELENABLED");
                OnPropertyChanged("UPGRADECOLOR");
            }

            public System.Windows.Media.Brush UPGRADECOLOR
            {
                get
                {
                    int evar = mapEditor.mapdata.PUPx.USEDEFAULT[player][objid];

                    if (evar == 1)
                    {
                        return System.Windows.Media.Brushes.Gray;
                    }
                    else
                    {
                        return System.Windows.Media.Brushes.Green;
                    }
                }
                set { }
            }

            public byte STARTLEVEL
            {
                get
                {
                    int evar = mapEditor.mapdata.PUPx.USEDEFAULT[player][objid];

                    if (evar == 1)
                        {
                        return binding.DEFAULTSTARTLEVEL;
                    }
                    else
                    {
                        return mapEditor.mapdata.PUPx.STARTLEVEL[player][objid];
                    }
                }
                set
                {
                    mapEditor.mapdata.PUPx.STARTLEVEL[player][objid] = value;
                    mapEditor.SetDirty();
                    OnPropertyChanged("STARTLEVEL");
                }
            }
            public byte MAXLEVEL
            {
                get
                {
                    int evar = mapEditor.mapdata.PUPx.USEDEFAULT[player][objid];

                    if (evar == 1)
                    {
                        return binding.DEFAULTMAXLEVEL;
                    }
                    else
                    {
                        return mapEditor.mapdata.PUPx.MAXLEVEL[player][objid];
                    }

                }
                set
                {
                    mapEditor.mapdata.PUPx.MAXLEVEL[player][objid] = value;
                    mapEditor.SetDirty();
                    OnPropertyChanged("MAXLEVEL");
                }
            }

            public bool LEVELENABLED
            {
                get
                {
                    int evar = mapEditor.mapdata.PUPx.USEDEFAULT[player][objid];

                    if (evar == 1)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                set
                {
                }
            }




            public UpgradePlayerDataBinding(MapEditor _mapEditor, int _player, int _objid, UpgradeDataBinding _binding)
            {
                mapEditor = _mapEditor;
                player = _player;
                objid = _objid;
                binding = _binding;
            }


            public void PropertyChangeAll()
            {
                OnPropertyChanged("UPGRADECOLOR");
                OnPropertyChanged("MAXLEVEL");
                OnPropertyChanged("STARTLEVEL");
                OnPropertyChanged("LEVELENABLED");
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









        public void PropertyChangeAll()
        {
            UIPropertyChange();
            OnPropertyChanged("BASEMIN");
            OnPropertyChanged("BONUSMIN");
            OnPropertyChanged("BASEGAS");
            OnPropertyChanged("BONUSGAS");
            OnPropertyChanged("BASETIME");
            OnPropertyChanged("BONUSTIME");

            OnPropertyChanged("DEFAULTSTARTLEVEL");
            OnPropertyChanged("DEFAULTMAXLEVEL");

            PlayerDataPropertyChangeAll();
        }

        public void PlayerDataPropertyChangeAll()
        {
            for (int i = 0; i < playerbind.Count; i++)
            {
                playerbind[i].PropertyChangeAll();
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
