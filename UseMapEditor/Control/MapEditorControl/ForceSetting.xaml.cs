using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UseMapEditor.DataBinding;

namespace UseMapEditor.Control.MapEditorControl
{
    /// <summary>
    /// ForceSetting.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ForceSetting : UserControl
    {
        private MapEditor mapEditor;
        ForceBinding forceBinding;
        private int forceID;

        public void SetMapEditor(MapEditor _mapEditor)
        {
            mapEditor = _mapEditor;
        }

        public ForceSetting(MapEditor _mapEditor, int _forceID)
        {
            InitializeComponent();
            mapEditor = _mapEditor;

            forceID = _forceID;
            forceBinding = new ForceBinding(mapEditor, forceID);
            _mapEditor.mapDataBinding.forceBindings.Add(forceBinding);

            this.DataContext = forceBinding;
            HintAssist.SetHint(MainTB, "세력 " + (forceID + 1));
        }

        public void Refresh()
        {
            forceBinding.PropertyChangeAll();
        }



        private void ListBox_MouseEnter(object sender, MouseEventArgs e)
        {
            mapEditor.ForceSelectID = forceID;

            if (mapEditor.ForceSelectPlayer.Count != 0)
            {
                if (mapEditor.ForceStartID != forceID)
                {
                    PlayerBox.Background = new SolidColorBrush(Color.FromArgb(128, 128, 128, 128));
                }
            }
        }

        private void PlayerBox_MouseLeave(object sender, MouseEventArgs e)
        {
            PlayerBox.Background = null;
        }



        private void PlayerBox_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mapEditor.ForceSelectPlayer.Count != 0)
            {
                if (mapEditor.ForceStartID != forceID)
                {
                    for (int i = 0; i < mapEditor.ForceSelectPlayer.Count; i++)
                    {
                        mapEditor.mapdata.FORCE[mapEditor.ForceSelectPlayer[i]] = (byte)mapEditor.ForceSelectID;
                    }

                    

                    mapEditor.SetDirty();
                    mapEditor.mapDataBinding.forceBindings[mapEditor.ForceStartID].PropertyChangeAll();
                    mapEditor.mapDataBinding.forceBindings[mapEditor.ForceSelectID].PropertyChangeAll();

                    mapEditor.ForceSelectPlayer.Clear();
                    mapEditor.ForceSelectID = -1;
                    mapEditor.ForceStartID = -1;
                    PlayerBox.Background = null;
                }
            }
        }

        private void PlayerBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mapEditor.ForceSelectPlayer.Count != 0)
            {
                if (mapEditor.ForceStartID != forceID)
                {
                    for (int i = 0; i < mapEditor.ForceSelectPlayer.Count; i++)
                    {
                        mapEditor.mapdata.FORCE[mapEditor.ForceSelectPlayer[i]] = (byte)mapEditor.ForceSelectID;
                    }



                    mapEditor.SetDirty();
                    mapEditor.mapDataBinding.forceBindings[mapEditor.ForceStartID].PropertyChangeAll();
                    mapEditor.mapDataBinding.forceBindings[mapEditor.ForceSelectID].PropertyChangeAll();

                    mapEditor.ForceSelectPlayer.Clear();
                    mapEditor.ForceSelectID = -1;
                    mapEditor.ForceStartID = -1;
                    PlayerBox.Background = null;
                    return;
                }
            }


            if (PlayerBox.SelectedItems != null)
            {
                mapEditor.ForceStartID = forceID;
                mapEditor.ForceSelectPlayer.Clear();
                for (int i = 0; i < PlayerBox.SelectedItems.Count; i++)
                {
                    mapEditor.ForceSelectPlayer.Add((int)((ListBoxItem)PlayerBox.SelectedItems[i]).Tag);
                }
            }
            else
            {
                mapEditor.ForceSelectPlayer.Clear();
            }


        }

    }
}
