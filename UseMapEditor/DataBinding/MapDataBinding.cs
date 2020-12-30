using Data.Map;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseMapEditor.Control;

namespace UseMapEditor.DataBinding
{
    public class MapDataBinding : INotifyPropertyChanged
    {
        MapEditor mapEditor;

        public MapDataBinding(MapEditor _mapEditor)
        {
            mapEditor = _mapEditor;


            for (int i = 0; i < 228; i++)
                unitdataBindings.Add(new UnitDataBinding(mapEditor, i));
            for (int i = 0; i < 61; i++)
                upgradeDataBindings.Add(new UpgradeDataBinding(mapEditor, i));
            for (int i = 0; i < 44; i++)
                techDataBindings.Add(new TechDataBinding(mapEditor, i));
        }


        public List<PlayerBinding> playerBindings = new List<PlayerBinding>();
        public List<ForceBinding> forceBindings = new List<ForceBinding>();



        public List<UnitDataBinding> unitdataBindings = new List<UnitDataBinding>();
        public List<UpgradeDataBinding> upgradeDataBindings = new List<UpgradeDataBinding>();
        public List<TechDataBinding> techDataBindings = new List<TechDataBinding>();



        public void PropertyChangeAll()
        {
            for (int i = 0; i < playerBindings.Count; i++)
            {
                playerBindings[i].PropertyChangeAll();
            }
            for (int i = 0; i < forceBindings.Count; i++)
            {
                forceBindings[i].PropertyChangeAll();
            }
            for (int i = 0; i < unitdataBindings.Count; i++)
            {
                unitdataBindings[i].PropertyChangeAll();
            }
            for (int i = 0; i < upgradeDataBindings.Count; i++)
            {
                upgradeDataBindings[i].PropertyChangeAll();
            }
            for (int i = 0; i < techDataBindings.Count; i++)
            {
                techDataBindings[i].PropertyChangeAll();
            }

            OnPropertyChanged("WIDTH");
            OnPropertyChanged("HEIGHT");
            OnPropertyChanged("TileSet");
            OnPropertyChanged("Title");
            OnPropertyChanged("Description");
        }




        public ushort WIDTH
        {
            get { return mapEditor.mapdata.WIDTH; }

            set
            {
                mapEditor.mapdata.WIDTH = value;
                mapEditor.IsMinimapLoad = false;

                mapEditor.SetDirty();
                OnPropertyChanged("WIDTH");
            }
        }
        public ushort HEIGHT
        {
            get { return mapEditor.mapdata.HEIGHT; }

            set
            {
                mapEditor.mapdata.HEIGHT = value;
                mapEditor.IsMinimapLoad = false;

                mapEditor.SetDirty();
                OnPropertyChanged("HEIGHT");
            }
        }


        public int TileSet
        {
            get { return (int)mapEditor.mapdata.TILETYPE; }

            set
            {
                mapEditor.mapdata.TILETYPE = (FileData.TileSet.TileType)value;
                mapEditor.IsMinimapLoad = false;

                mapEditor.SetDirty();
                OnPropertyChanged("TileSet");
            }
        }



        public string Title
        {
            get {return mapEditor.mapdata.SCEARIONAME.String;}
            set
            {
                mapEditor.mapdata.SCEARIONAME.String = value;

                mapEditor.SetDirty();
                OnPropertyChanged("Title");
            }
        }

        public string Description
        {
            get {return mapEditor.mapdata.SCEARIODES.String;}
            set
            {
                mapEditor.mapdata.SCEARIODES.String = value;

                mapEditor.SetDirty();
                OnPropertyChanged("Description");
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
