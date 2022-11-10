using ControlzEx.Standard;
using Data.Map;
using MaterialDesignThemes.Wpf;
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
            for (int i = 0; i < 517; i++)
                spriteDataBindings.Add(new SpriteDataBinding(mapEditor, i));
        }

        public enum UserDefaultCode
        {
            Unusable,
            Default,
            Usable,
            Complete,
        }

        public List<PlayerBinding> playerBindings = new List<PlayerBinding>();
        public List<ForceBinding> forceBindings = new List<ForceBinding>();



        public List<UnitDataBinding> unitdataBindings = new List<UnitDataBinding>();
        public List<UpgradeDataBinding> upgradeDataBindings = new List<UpgradeDataBinding>();
        public List<TechDataBinding> techDataBindings = new List<TechDataBinding>();

        public List<SpriteDataBinding> spriteDataBindings = new List<SpriteDataBinding>();


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


        public int OPTFOGOFWARPLAYER
        {
            get
            {
                return mapEditor.opt_FogofWarplayer;
            }
            set
            {
                mapEditor.opt_FogofWarplayer = value;
            }
        }



        public PackIconKind TILE_BRUSHICON
        {
            get
            {
                switch (mapEditor.tile_PaintType)
                {
                    case MapEditor.TileSetPaintType.SELECTION:
                        return PackIconKind.HandRight;
                    case MapEditor.TileSetPaintType.PENCIL:
                        return PackIconKind.Pencil;
                    case MapEditor.TileSetPaintType.RECT:
                        return PackIconKind.VectorRectangle;
                }

                return PackIconKind.HandRight;
            }
        }



        public MapEditor.TileSetPaintType TILE_PAINTTYPE
        {
            get
            {
                return mapEditor.tile_PaintType;
            }
            set
            {
                mapEditor.tile_PaintType = value;
                OnPropertyChanged("TILE_BRUSHMODE");
                OnPropertyChanged("TILE_BRUSHICON");
            }
        }

        public bool TILE_TRANSPARENTBLACK
        {
            get
            {
                return mapEditor.TilePalleteTransparentBlack;
            }
            set
            {
                mapEditor.TilePalleteTransparentBlack = value;
                OnPropertyChanged("TILE_TRANSPARENTBLACK");
            }
        }

        public bool DOODAD_BRUSHMODE
        {
            get
            {
                return mapEditor.doodad_BrushMode;
            }
            set
            {
                mapEditor.doodad_BrushMode = value;
                OnPropertyChanged("DOODAD_BRUSHMODE");
            }
        }
        public bool DOODAD_SELECTMODE
        {
            get
            {
                return mapEditor.doodad_SelectMode;
            }
            set
            {
                mapEditor.doodad_SelectMode = value;
                OnPropertyChanged("DOODAD_SELECTMODE");
            }
        }


        public bool DOODAD_STACKALLOW
        {
            get
            {
                return mapEditor.DoodadPalleteStackAllow;
            }
            set
            {
                mapEditor.DoodadPalleteStackAllow = value;
                OnPropertyChanged("DOODAD_STACKALLOW");
            }
        }
        public bool DOODAD_TOTILE
        {
            get
            {
                return mapEditor.DoodadPalleteToTile;
            }
            set
            {
                mapEditor.DoodadPalleteToTile = value;
                OnPropertyChanged("DOODAD_TOTILE");
            }
        }
        public bool DOODAD_SIZEUP
        {
            get
            {
                return mapEditor.DoodadPalleteSizeUp;
            }
            set
            {
                mapEditor.DoodadPalleteSizeUp = value;
                OnPropertyChanged("DOODAD_SIZEUP");
            }
        }






        public int SPRITEPLAYER
        {
            get
            {
                return mapEditor.sprite_player;
            }
            set
            {
                mapEditor.sprite_player = value;
            }
        }



        public bool SPRITE_BRUSHMODE
        {
            get
            {
                return mapEditor.sprite_BrushMode;
            }
            set
            {
                mapEditor.sprite_BrushMode = value;
                OnPropertyChanged("SPRITE_BRUSHMODE");
            }
        }
        public bool SPRITE_UNITBRUSH
        {
            get
            {
                return mapEditor.sprite_UnitBrush;
            }
            set
            {
                mapEditor.sprite_UnitBrush = value;
                OnPropertyChanged("SPRITE_UNITBRUSH");
            }
        }
        public bool SPRITE_SPRITEBRUSH
        {
            get
            {
                return mapEditor.sprite_SpritBrush;
            }
            set
            {
                mapEditor.sprite_SpritBrush = value;
                OnPropertyChanged("SPRITE_SPRITEBRUSH");
            }
        }

        public bool SPRITE_SELECTMODE
        {
            get
            {
                return mapEditor.sprite_SelectMode;
            }
            set
            {
                mapEditor.sprite_SelectMode = value;
                OnPropertyChanged("SPRITE_SELECTMODE");
            }
        }


        public bool SPRITE_GRIDFIX
        {
            get
            {
                return mapEditor.SpritePalleteGridFix;
            }
            set
            {
                mapEditor.SpritePalleteGridFix = value;
                OnPropertyChanged("SPRITE_GRIDFIX");
            }
        }
        public bool SPRITE_COPYTILEPOS
        {
            get
            {
                return mapEditor.SpritePalleteCopyTileFix;
            }
            set
            {
                mapEditor.SpritePalleteCopyTileFix = value;
                OnPropertyChanged("SPRITE_COPYTILEPOS");
            }
        }




        public int UNITPLAYER
        {
            get
            {
                return mapEditor.unit_player;
            }
            set
            {
                mapEditor.unit_player = value;
            }
        }


        public bool UNIT_BRUSHMODE
        {
            get
            {
                return mapEditor.unit_BrushMode;
            }
            set
            {
                mapEditor.unit_BrushMode = value;
                OnPropertyChanged("UNIT_BRUSHMODE");
            }
        }

        public bool UNIT_SELECTMODE
        {
            get
            {
                return mapEditor.unit_SelectMode;
            }
            set
            {
                mapEditor.unit_SelectMode = value;
                OnPropertyChanged("UNIT_SELECTMODE");
            }
        }



        public bool UNIT_GRIDFIX
        {
            get
            {
                return mapEditor.UnitPalleteGridFix;
            }
            set
            {
                mapEditor.UnitPalleteGridFix = value;
                OnPropertyChanged("UNIT_GRIDFIX");
            }
        }
        public bool UNIT_BUILDINGFIX
        {
            get
            {
                return mapEditor.UnitPalleteBuildingFix;
            }
            set
            {
                mapEditor.UnitPalleteBuildingFix = value;
                OnPropertyChanged("UNIT_BUILDINGFIX");
            }
        }

        public bool UNIT_ALLOWSTACK
        {
            get
            {
                return mapEditor.UnitPalleteStackAllow;
            }
            set
            {
                mapEditor.UnitPalleteStackAllow = value;
                OnPropertyChanged("UNIT_ALLOWSTACK");
            }
        }
        public bool UNIT_COPYTILEPOS
        {
            get
            {
                return mapEditor.UnitPalleteCopyTileFix;
            }
            set
            {
                mapEditor.UnitPalleteCopyTileFix = value;
                OnPropertyChanged("UNIT_COPYTILEPOS");
            }
        }




        public byte BRUSHX
        {
            get
            {
                return mapEditor.brush_x;
            }
            set
            {
                if(value == 0)
                {
                    mapEditor.brush_x = 1;
                }
                else
                {
                    mapEditor.brush_x = value;
                }
            }
        }
        public byte BRUSHY
        {
            get
            {
                return mapEditor.brush_y;
            }
            set
            {
                if (value == 0)
                {
                    mapEditor.brush_y = 1;
                }
                else
                {
                    mapEditor.brush_y = value;
                }
            }
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
                for (int i = 0; i < mapEditor.mapdata.DD2.Count; i++)
                {
                    mapEditor.mapdata.DD2[i].ImageReset();
                }

                mapEditor.TileSetUIRefresh();
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
            get
            {
                string desc = mapEditor.mapdata.SCEARIODES.String;
                desc = desc.Replace("\\r", "");
                desc = desc.Replace("\\n", "\n");

                return desc;
            }
            set
            {
                string desc = value;
                desc = desc.Replace("\n", "\\n");


                mapEditor.mapdata.SCEARIODES.String = desc;

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
