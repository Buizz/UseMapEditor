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
using UseMapEditor.Windows;

namespace UseMapEditor.Control.MapEditorControl
{
    /// <summary>
    /// DialogableControlPanel.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DialogableControlPanel : UserControl
    {
        private UserControl MainContent;

        public DialogableControlPanel(UserControl control)
        {
            InitializeComponent();

            MainContent = control;
            MainGrid.Child = MainContent;
            IsPopup = false;
        }


        private PopupWindow popupWindow;

        public Rect GetRect()
        {
            Rect rect = new Rect(popupWindow.Left, popupWindow.Top, popupWindow.ActualWidth, popupWindow.ActualHeight);

            return rect;
        }



        public bool IsPopup;
        public void PopupWindowOpen()
        {
            this.MinWidth = MainContent.ActualWidth;

            PopupBackground.Visibility = Visibility.Visible;

            MainGrid.Child = null;
            popupWindow = new PopupWindow(MainContent, this);

            IsPopup = true;
            popupWindow.Show();
        }
        public void PopupWindowClose()
        {
            if (popupWindow == null)
                return;
            try
            {
                popupWindow.Close();
            }
            catch (Exception)
            {
            }
        }
        public void PopupWindowCloseEvent()
        {
            IsPopup = false;
            PopupBackground.Visibility = Visibility.Collapsed;
            popupWindow = null;
            MainGrid.Child = MainContent;
        }




        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PopupWindowOpen();
        }

        private void PopupClose_BtnClick(object sender, RoutedEventArgs e)
        {
            PopupWindowClose();
        }
    }
}
