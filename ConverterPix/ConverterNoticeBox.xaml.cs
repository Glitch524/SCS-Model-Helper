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

namespace SCS_Mod_Helper.ConverterPix
{
    /// <summary>
    /// ExtractMessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class ConverterNoticeBox : Window
    {
        public ConverterNoticeBox()
        {
            InitializeComponent();
        }

        public bool DontShowAgain = false;
		private void ButtonResult(object sender, RoutedEventArgs e) {
            DialogResult = true;
			DontShowAgain = CheckBoxDontShowAgain.IsChecked == true;
            this.Close();
		}
	}
}
