using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseMapEditor.Control;

namespace UseMapEditor.DataBinding
{
    public class UIBinding : INotifyPropertyChanged
    {
        private MapEditor mapEditor;
        public UIBinding(MapEditor mapEditor)
        {
            this.mapEditor = mapEditor;
        }


        public bool view_Unit
        {
            get{return mapEditor.view_Unit;}
            set{mapEditor.view_Unit = value;
                mapEditor.MinimapUnitInitRefresh();
                OnPropertyChanged("view_Unit");
            }
        }


        public bool view_Unit_StartLoc
        {
            get { return mapEditor.view_Unit_StartLoc; }
            set { mapEditor.view_Unit_StartLoc = value;
                mapEditor.MinimapUnitInitRefresh();
                OnPropertyChanged("view_Unit_StartLoc");
            }
        }
        public bool view_Unit_Maprevealer
        {
            get { return mapEditor.view_Unit_Maprevealer; }
            set { mapEditor.view_Unit_Maprevealer = value;
                mapEditor.MinimapUnitInitRefresh();
                OnPropertyChanged("view_Unit_Maprevealer");
            }
        }


        public bool view_Doodad
        {
            get { return mapEditor.view_Doodad; }
            set { mapEditor.view_Doodad = value;
                OnPropertyChanged("view_Doodad");
            }
        }
        public bool view_DoodadColor
        {
            get { return mapEditor.view_DoodadColor; }
            set { mapEditor.view_DoodadColor = value;
                OnPropertyChanged("view_DoodadColor");
            }
        }

        public bool view_Sprite
        {
            get { return mapEditor.view_Sprite; }
            set { mapEditor.view_Sprite = value;
                OnPropertyChanged("view_Sprite");
            }
        }
        public bool view_SpriteColor
        {
            get { return mapEditor.view_SpriteColor; }
            set { mapEditor.view_SpriteColor = value;
                OnPropertyChanged("view_SpriteColor");
            }
        }


        public bool view_Tile
        {
            get { return mapEditor.view_Tile; }
            set { mapEditor.view_Tile = value;
                OnPropertyChanged("view_Tile");
            }
        }


        public bool view_Location
        {
            get { return mapEditor.view_Location; }
            set { mapEditor.view_Location = value;
                OnPropertyChanged("view_Location");
            }
        }







        public void PropertyChangeAll()
        {
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
