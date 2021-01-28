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

namespace UseMapEditor.Control
{
    /// <summary>
    /// ColorSelectButton.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ColorSelectButton : UserControl
    {
        public event EventHandler ColorSelectEvent;

        private Color MainColor;
        private Microsoft.Xna.Framework.Color XnaColor;
        public ColorSelectButton()
        {
            InitializeComponent();
        }


        public void SetColor(Color MainColor)
        {
            this.MainColor = MainColor;
            Colorize.Background = new SolidColorBrush(Color.FromRgb(MainColor.R, MainColor.G, MainColor.B));
        }
        public void SetColor(Microsoft.Xna.Framework.Color XnaColor)
        {
            this.XnaColor = XnaColor;
            Colorize.Background = new SolidColorBrush(Color.FromRgb(XnaColor.R, XnaColor.G, XnaColor.B));
        }


        private void Colorize_Click(object sender, RoutedEventArgs e)
        {
            if (MainColor != null)
            {
                ColorPicker.InitColor(MainColor);
            }
            else
            {
                ColorPicker.InitColor(Color.FromRgb(XnaColor.R, XnaColor.G, XnaColor.B));
            }


            ColorPickerPopup.IsOpen = true;
        }

        private void ColorPicker_ColorSelect(object sender, RoutedEventArgs e)
        {
            Color color = (Color)sender;

            if(XnaColor == null)
            {
                MainColor = Color.FromRgb(color.R, color.G, color.B);
                Colorize.Background = new SolidColorBrush(MainColor);
                ColorSelectEvent.Invoke(MainColor, e);
            }
            else
            {
                XnaColor = new Microsoft.Xna.Framework.Color(color.R, color.G, color.B);
                Colorize.Background = new SolidColorBrush(Color.FromRgb(color.R, color.G, color.B));
                ColorSelectEvent.Invoke(XnaColor, e);
            }

        }
    }
}
