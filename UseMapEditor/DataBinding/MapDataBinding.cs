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
        }


        public List<PlayerBinding> playerBindings = new List<PlayerBinding>();
        public List<ForceBinding> forceBindings = new List<ForceBinding>();
        

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
            mapEditor.SetDirty();
            OnPropertyChanged("TileSet");
            OnPropertyChanged("Title");
            OnPropertyChanged("Description");
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
