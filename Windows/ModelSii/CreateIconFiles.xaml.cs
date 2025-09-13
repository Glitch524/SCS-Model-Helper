using System.Windows;

namespace Def_Writer
{
    /// <summary>
    /// CreateIconFiles.xaml 的交互逻辑
    /// </summary>
    public partial class CreateIconFiles : Window
    {
		bool NotInAcc = false;
        public CreateIconFiles(bool notInAcc, bool atAcc)// notInAcc表示图标没有在配件文件夹内
		{
            InitializeComponent();
			if (notInAcc) {
				TextNotInAcc.Visibility = Visibility.Visible;
			} else if (atAcc)
				BoxCreateOnAccessory.Visibility = Visibility.Collapsed;
			else {
				BoxCreateOnAccessory.Visibility = Visibility.Visible;
				BoxCreateOnAccessory.IsChecked = History.Default.IconDefCreateOnAccessory;
			}
		}

		public bool CreateOnAccessory => BoxCreateOnAccessory.IsChecked == true || NotInAcc;

		private void Button_Click(object sender, RoutedEventArgs e) {
            History.Default.IconDefCreateOnAccessory = BoxCreateOnAccessory.IsChecked == true;
            History.Default.Save();
            DialogResult = sender == ButtonYes;
		}
	}
}
