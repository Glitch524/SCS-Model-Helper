using System.Windows;

namespace SCS_Mod_Helper.ConverterPix {
	/// <summary>
	/// ExtractMessageBox.xaml 的交互逻辑
	/// </summary>
	public partial class ConverterNoticeBox: Window {
		public ConverterNoticeBox() {
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
