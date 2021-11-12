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
    /// ShortCutEditControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ShortCutEditControl : UserControl
    {
        string keyname;


        bool IsLoad = false;
        public ShortCutEditControl(string key)
        {
            InitializeComponent();

            keyname = key;
            var keypair = Global.Setting.ShortCutKeys[keyname];



            KeyName.Text = keyname;
            Key.Text = keypair.keys.ToString();

            cbAlt.IsChecked = (keypair.modifierKeys & ModifierKeys.Alt) > 0;
            cbCtrl.IsChecked = (keypair.modifierKeys & ModifierKeys.Control) > 0;
            cbShift.IsChecked = (keypair.modifierKeys & ModifierKeys.Shift) > 0;

            IsLoad = true;
        }

        private void Key_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            string strKey = e.Key.ToString();

            var keypair = Global.Setting.ShortCutKeys[keyname];


            foreach (BondTech.HotKeyManagement.WPF._4.Keys settings in Enum.GetValues(typeof(BondTech.HotKeyManagement.WPF._4.Keys)))
            {
                if(strKey.ToLower() == settings.ToString().ToLower())
                {
                    if (CheckValidity(settings, keypair.modifierKeys))
                    {
                        Key.Text = strKey;
                        keypair.keys = settings;
                    }

                    return;
                }
            }
        }


        private void cb_Checked(object sender, RoutedEventArgs e)
        {
            if (IsLoad)
            {
                int flag = 0;

                if (cbAlt.IsChecked == true) flag += 1;
                if (cbCtrl.IsChecked == true) flag += 2;
                if (cbShift.IsChecked == true) flag += 4;


                var keypair = Global.Setting.ShortCutKeys[keyname];

                if (CheckValidity(keypair.keys, (ModifierKeys)flag))
                {
                    keypair.modifierKeys = (ModifierKeys)flag;
                }
                else
                {
                    //중복 키이므로 다시 되돌리기
                    IsLoad = false;

                    cbAlt.IsChecked = (keypair.modifierKeys & ModifierKeys.Alt) > 0;
                    cbCtrl.IsChecked = (keypair.modifierKeys & ModifierKeys.Control) > 0;
                    cbShift.IsChecked = (keypair.modifierKeys & ModifierKeys.Shift) > 0;

                    IsLoad = true;
                }
            }

        }

        private bool CheckValidity(BondTech.HotKeyManagement.WPF._4.Keys key, ModifierKeys modifierKeys)
        {
            foreach (var item in Global.Setting.ShortCutKeys)
            {
                if(item.Value.keys == key && item.Value.modifierKeys == modifierKeys)
                {
                    //키가 중복
                    return false;
                }
            }


            return true;
        }
    }
}
