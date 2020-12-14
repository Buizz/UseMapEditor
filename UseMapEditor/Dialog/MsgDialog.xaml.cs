using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

namespace UseMapEditor.Dialog
{
    /// <summary>
    /// ErrorDialog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MsgDialog : MetroWindow
    {
        public MessageBoxResult msgresult = MessageBoxResult.None; 

        public MsgDialog(string ErrorMsg, MessageBoxButton boxButton, MessageBoxImage boxImage)
        {
            InitializeComponent();


            ErrorText.Text = ErrorMsg;


            switch (boxImage)
            {
                case MessageBoxImage.Error:
                    msgIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Error;
                    System.Media.SystemSounds.Hand.Play();
                    break;
                case MessageBoxImage.Asterisk:
                    msgIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Information;
                    System.Media.SystemSounds.Exclamation.Play();
                    break;
                case MessageBoxImage.Warning:
                    msgIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Warning;
                    System.Media.SystemSounds.Hand.Play();
                    break;
                case MessageBoxImage.Question:
                    msgIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.QuestionMark;
                    System.Media.SystemSounds.Question.Play();
                    break;
            }

            switch (boxButton)
            {
                case MessageBoxButton.OK:
                    {
                        Button button = new Button();
                        button.Content = "확인";
                        button.Style = (Style)Application.Current.Resources["MaterialDesignFlatButton"];

                        button.Click += new RoutedEventHandler(delegate (object sender, RoutedEventArgs e)
                        {
                            msgresult = MessageBoxResult.OK;
                            Close();
                        });

                        BtnPanel.Children.Add(button);
                    }
                    break;
                case MessageBoxButton.OKCancel:
                    {
                        Button button = new Button();
                        button.Content = "확인";
                        button.Style = (Style)Application.Current.Resources["MaterialDesignFlatButton"];

                        button.Click += new RoutedEventHandler(delegate (object sender, RoutedEventArgs e)
                        {
                            msgresult = MessageBoxResult.OK;
                            Close();
                        });

                        BtnPanel.Children.Add(button);
                    }
                    {
                        Button button = new Button();
                        button.Content = "취소";
                        button.Style = (Style)Application.Current.Resources["MaterialDesignFlatButton"];

                        button.Click += new RoutedEventHandler(delegate (object sender, RoutedEventArgs e)
                        {
                            msgresult = MessageBoxResult.Cancel;
                            Close();
                        });

                        BtnPanel.Children.Add(button);
                    }
                    break;
                case MessageBoxButton.YesNo:
                    {
                        Button button = new Button();
                        button.Content = "예";
                        button.Style = (Style)Application.Current.Resources["MaterialDesignFlatButton"];

                        button.Click += new RoutedEventHandler(delegate (object sender, RoutedEventArgs e)
                        {
                            msgresult = MessageBoxResult.Yes;
                            Close();
                        });

                        BtnPanel.Children.Add(button);
                    }
                    {
                        Button button = new Button();
                        button.Content = "아니요";
                        button.Style = (Style)Application.Current.Resources["MaterialDesignFlatButton"];

                        button.Click += new RoutedEventHandler(delegate (object sender, RoutedEventArgs e)
                        {
                            msgresult = MessageBoxResult.No;
                            Close();
                        });

                        BtnPanel.Children.Add(button);
                    }
                    break;
                case MessageBoxButton.YesNoCancel:
                    {
                        Button button = new Button();
                        button.Content = "예";
                        button.Style = (Style)Application.Current.Resources["MaterialDesignFlatButton"];

                        button.Click += new RoutedEventHandler(delegate (object sender, RoutedEventArgs e)
                        {
                            msgresult = MessageBoxResult.Yes;
                            Close();
                        });

                        BtnPanel.Children.Add(button);
                    }
                    {
                        Button button = new Button();
                        button.Content = "아니요";
                        button.Style = (Style)Application.Current.Resources["MaterialDesignFlatButton"];

                        button.Click += new RoutedEventHandler(delegate (object sender, RoutedEventArgs e)
                        {
                            msgresult = MessageBoxResult.No;
                            Close();
                        });

                        BtnPanel.Children.Add(button);
                    }
                    {
                        Button button = new Button();
                        button.Content = "취소";
                        button.Style = (Style)Application.Current.Resources["MaterialDesignFlatButton"];

                        button.Click += new RoutedEventHandler(delegate (object sender, RoutedEventArgs e)
                        {
                            msgresult = MessageBoxResult.Cancel;
                            Close();
                        });

                        BtnPanel.Children.Add(button);
                    }
                    break;
            }
        }

    }
}
