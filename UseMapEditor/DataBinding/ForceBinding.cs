using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using UseMapEditor.Control;

namespace UseMapEditor.DataBinding
{
    public class ForceBinding : INotifyPropertyChanged
    {
        MapEditor mapEditor;
        int ForceID;



        public ForceBinding(MapEditor _mapEditor, int _ForceID)
        {
            mapEditor = _mapEditor;
            ForceID = _ForceID;
        }

        public void PropertyChangeAll()
        {
            OnPropertyChanged("ForceName");
            OnPropertyChanged("Allied");
            OnPropertyChanged("AlliedVictory");
            OnPropertyChanged("ShareVision");
            OnPropertyChanged("Randomize");
            OnPropertyChanged("Players");
            
        }

        public string ForceName
        {
            get
            {
                if(mapEditor.mapdata.FORCENAME[ForceID] != null)
                {
                    return mapEditor.mapdata.FORCENAME[ForceID].String;
                }
                return "";
            }
            set
            {
                mapEditor.mapdata.FORCENAME[ForceID].String = value;
                mapEditor.SetDirty();
                OnPropertyChanged("ForceName");
            }
        }


        public bool Allied
        {
            get
            {
                return (mapEditor.mapdata.FORCEFLAG[ForceID] & 0b10) > 0;
            }
            set
            {
                if (value)
                {
                    mapEditor.mapdata.FORCEFLAG[ForceID] |= 0b10;
                }
                else
                {
                    mapEditor.mapdata.FORCEFLAG[ForceID] &= unchecked((byte)~0b10);
                }
                mapEditor.SetDirty();
                OnPropertyChanged("Allied");
            }
        }
        public bool AlliedVictory
        {
            get
            {
                return (mapEditor.mapdata.FORCEFLAG[ForceID] & 0b100) > 0;
            }
            set
            {
                if (value)
                {
                    mapEditor.mapdata.FORCEFLAG[ForceID] |= 0b100;
                }
                else
                {
                    mapEditor.mapdata.FORCEFLAG[ForceID] &= unchecked((byte)~0b100);
                }
                mapEditor.SetDirty();
                OnPropertyChanged("AlliedVictory");
            }
        }
        public bool ShareVision
        {
            get
            {
                return (mapEditor.mapdata.FORCEFLAG[ForceID] & 0b1000) > 0;
            }
            set
            {
                if (value)
                {
                    mapEditor.mapdata.FORCEFLAG[ForceID] |= 0b1000;
                }
                else
                {
                    mapEditor.mapdata.FORCEFLAG[ForceID] &= unchecked((byte)~0b1000);
                }
                mapEditor.SetDirty();
                OnPropertyChanged("ShareVision");
            }
        }
        public bool Randomize
        {
            get
            {
                return (mapEditor.mapdata.FORCEFLAG[ForceID] & 0b1) > 0;
            }
            set
            {
                if (value)
                {
                    mapEditor.mapdata.FORCEFLAG[ForceID] |= 0b1;
                }
                else
                {
                    mapEditor.mapdata.FORCEFLAG[ForceID] &= unchecked((byte)~0b1);
                }
                mapEditor.SetDirty();
                OnPropertyChanged("Randomize");
            }
        }



        public List<ListBoxItem> Players
        {
            get
            {
                List<ListBoxItem> listBoxItems = new List<ListBoxItem>();


                for (int i = 0; i < 8; i++)
                {
                    if(mapEditor.mapdata.FORCE[i] == ForceID)
                    {
                        ListBoxItem listBoxItem = new ListBoxItem();


                        StackPanel stackPanel = new StackPanel();
                        {
                            stackPanel.Orientation = Orientation.Horizontal;

                            Border border = new Border();
                            border.Width = 16;
                            border.Height = 16;
                            border.Background = mapEditor.mapDataBinding.playerBindings[i].BackColor;
                            stackPanel.Children.Add(border);


                            TextBlock textBlock = new TextBlock();
                            textBlock.Text = "플레이어 " + (i + 1) + " - " + mapEditor.mapdata.GetIOWNTYPEName((Data.Map.MapData.IOWNTYPE)mapEditor.mapdata.IOWN[i]) + " - " +
                                mapEditor.mapdata.GetSIDETYPEName((Data.Map.MapData.SIDETYPE)mapEditor.mapdata.SIDE[i]);
                            stackPanel.Children.Add(textBlock);

                        }




                        listBoxItem.Content = stackPanel;



                        listBoxItem.Padding = new System.Windows.Thickness(8, 2 ,8 ,2);
                        listBoxItem.Tag = i;

                        listBoxItems.Add(listBoxItem);
                    }
                }


                return listBoxItems;
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
