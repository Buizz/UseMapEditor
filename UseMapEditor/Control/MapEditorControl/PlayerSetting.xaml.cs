using Data.Map;
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
    /// PlayerSetting.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PlayerSetting : UserControl
    {
        PlayerBinding playerBinding;
        public PlayerSetting(MapEditor _mapEditor, int playerID)
        {
            InitializeComponent();

            PlayerIndex.Text = "플레이어 " + (playerID + 1).ToString();

            playerBinding = new PlayerBinding(_mapEditor, playerID);
            _mapEditor.mapDataBinding.playerBindings.Add(playerBinding);


            this.DataContext = playerBinding;




            for (int i = 0; i < MapData.ColorName.Length; i++)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();

                DockPanel dockPanel = new DockPanel();
                dockPanel.Width = 80;

                TextBlock textBlock = new TextBlock();
                textBlock.Text = MapData.ColorName[i];
                textBlock.VerticalAlignment = VerticalAlignment.Center;

                Border border = new Border();
                border.Width = 24;
                border.Height = 24;
                border.Margin = new Thickness(-10,-4,4,-4);


                Color color = Color.FromRgb(MapData.PlayerColors[i].R, MapData.PlayerColors[i].G, MapData.PlayerColors[i].B);
                border.Background = new SolidColorBrush(color);



                dockPanel.Children.Add(border);
                dockPanel.Children.Add(textBlock);




                comboBoxItem.Content = dockPanel;

                ColorCB.Items.Add(comboBoxItem);
            }
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = "랜덤 색상";
                ColorCB.Items.Add(comboBoxItem);
            }
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = "플레이어 선택";
                ColorCB.Items.Add(comboBoxItem);
            }
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = "커스텀 RGB";
                ColorCB.Items.Add(comboBoxItem);
            }



            {
                Binding myBinding = new Binding("Owner");
                myBinding.Mode = BindingMode.TwoWay;
                myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                OwnerCB.SetBinding(ComboBox.SelectedIndexProperty, myBinding);
            }

            {
                Binding myBinding = new Binding("Race");
                myBinding.Mode = BindingMode.TwoWay;
                myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                RaceCB.SetBinding(ComboBox.SelectedIndexProperty, myBinding);
            }

            {
                Binding myBinding = new Binding("Color");
                myBinding.Mode = BindingMode.TwoWay;
                myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                ColorCB.SetBinding(ComboBox.SelectedIndexProperty, myBinding);
            }
            {
                Binding myBinding = new Binding("BackColor");
                myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                Colorize.SetBinding(Button.BackgroundProperty, myBinding);
            }
        }

        private void Colorize_Click(object sender, RoutedEventArgs e)
        {
            ColorPicker.InitColor(playerBinding.IconColor);

            ColorPickerPopup.IsOpen = true;
        }

        private void ColorPicker_ColorSelect(object sender, RoutedEventArgs e)
        {
            Color color = (Color)sender;


            playerBinding.CustomColor = new Microsoft.Xna.Framework.Color(color.R, color.G, color.B);
            playerBinding.PropertyChangeAll();
        }
    }
}
