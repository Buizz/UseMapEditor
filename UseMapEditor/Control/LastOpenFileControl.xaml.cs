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
    /// LastOpenFileControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LastOpenFileControl : UserControl
    {
        public LastOpenFileControl(string filepath)
        {
            InitializeComponent();

            SavePath.Text = System.IO.Path.GetFileName(filepath);


            string folder = System.IO.Path.GetDirectoryName(filepath);

            if(folder.Length > 33)
            {
                folder = folder.Substring(0, 30) + " ...";
            }

            FullPath.Text = folder;
        }
    }
}
