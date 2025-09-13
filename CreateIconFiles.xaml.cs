using System.Windows;

namespace Def_Writer
{
    /// <summary>
    /// CreateIconFiles.xaml 的交互逻辑
    /// </summary>
    public partial class CreateIconFiles : Window
    {
        public CreateIconFiles(bool showCheckbox)
        {
            InitializeComponent();
            if (showCheckbox) {
				BoxCreateOnAccessory.Visibility = Visibility.Visible;
                BoxCreateOnAccessory.IsChecked = History.Default.IconDefCreateOnAccessory;
			} else
                BoxCreateOnAccessory.Visibility = Visibility.Collapsed;
        }

        public bool CreateOnAccessory {
            get {
                return BoxCreateOnAccessory.IsChecked == true;
            }
        }

		private void Button_Click(object sender, RoutedEventArgs e) {
            History.Default.IconDefCreateOnAccessory = BoxCreateOnAccessory.IsChecked == true;
            History.Default.Save();
            DialogResult = sender == ButtonYes;
		}
	}
}
