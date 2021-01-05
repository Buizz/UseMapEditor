using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Data.Map.MapData;
using static UseMapEditor.Control.MapEditor;

namespace UseMapEditor.Control.MapEditorControl
{
    /// <summary>
    /// UpgradeSetting.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UpgradeSetting : UserControl
    {
        private MapEditor mapEditor;
        public void SetMapEditor(MapEditor _mapEditor)
        {
            mapEditor = _mapEditor;

            codeselecter.SelectionChanged += Codeselecter_SelectionChanged;
            codeselecter.SetCodeType(Codetype.Upgrade, mapEditor);

            PlayerDatas.Children.Clear();

            for (int i = 0; i < 12; i++)
            {
                UniformGrid uniformGrid = new UniformGrid();
                uniformGrid.Columns = 3;


                Button button = new Button();
                button.Content = "P" + (i + 1);
                button.Height = 24;
                button.Padding = new Thickness(16, 0, 16, 0);
                int index = i;
                button.Click += delegate (object sender, RoutedEventArgs e)
                {
                    CurrentBinding.playerbind[index].AddDefault();
                };

                {
                    Binding binding = new Binding();
                    binding.Path = new PropertyPath("UPGRADECOLOR");
                    binding.Mode = BindingMode.TwoWay;
                    binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                    button.SetBinding(Button.BackgroundProperty, binding);
                }




                uniformGrid.Children.Add(button);
                {
                    TextBox textBox = new TextBox();
                    textBox.Margin = new Thickness(4, 0, 4, 0);
                    {
                        Binding binding = new Binding();
                        binding.Path = new PropertyPath("STARTLEVEL");
                        binding.Mode = BindingMode.TwoWay;
                        binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                        textBox.SetBinding(TextBox.TextProperty, binding);
                    }
                    {
                        Binding binding = new Binding();
                        binding.Path = new PropertyPath("LEVELENABLED");
                        binding.Mode = BindingMode.TwoWay;
                        binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                        textBox.SetBinding(TextBox.IsEnabledProperty, binding);
                    }
                    uniformGrid.Children.Add(textBox);
                }
                {
                    TextBox textBox = new TextBox();
                    textBox.Margin = new Thickness(4, 0, 4, 0);
                    {
                        Binding binding = new Binding();
                        binding.Path = new PropertyPath("MAXLEVEL");
                        binding.Mode = BindingMode.TwoWay;
                        binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                        textBox.SetBinding(TextBox.TextProperty, binding);
                    }
                    {
                        Binding binding = new Binding();
                        binding.Path = new PropertyPath("LEVELENABLED");
                        binding.Mode = BindingMode.TwoWay;
                        binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                        textBox.SetBinding(TextBox.IsEnabledProperty, binding);
                    }
                    uniformGrid.Children.Add(textBox);
                }

                PlayerDatas.Children.Add(uniformGrid);
                
            }

            CurrentBinding = mapEditor.mapDataBinding.upgradeDataBindings[0];
            ContentPanel.DataContext = CurrentBinding;

            for (int i = 0; i < 12; i++)
            {
                ((UniformGrid)PlayerDatas.Children[i]).DataContext = CurrentBinding.playerbind[i];
            }
        }


        private DataBinding.UpgradeDataBinding CurrentBinding;
        private void Codeselecter_SelectionChanged(object sender, EventArgs e)
        {
            int Selectindex = (int)sender;

            if (Selectindex != -1)
            {
                ContentPanel.Visibility = Visibility.Visible;
            }
            else
            {
                ContentPanel.Visibility = Visibility.Hidden;
            }
            if (Selectindex == -1)
            {
                return;
            }

            CurrentBinding = mapEditor.mapDataBinding.upgradeDataBindings[Selectindex];
            ContentPanel.DataContext = CurrentBinding;

            for (int i = 0; i < 12; i++)
            {
                ((UniformGrid)PlayerDatas.Children[i]).DataContext = CurrentBinding.playerbind[i];
            }
        }

        public UpgradeSetting()
        {
            InitializeComponent();
        }
    }
}
