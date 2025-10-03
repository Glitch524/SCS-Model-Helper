using System.Windows;
using System.Windows.Controls;

namespace SCS_Mod_Helper.Accessory.AccAddon
{
    /// <summary>
    /// AccHideInWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AccHideInWindow : Window
    {
        public AccHideInWindow()
        {
            InitializeComponent();
		}

		private uint GetNumber(CheckBox checkBox) {
			if (checkBox == CheckMainView) {
				return 0x7;
			} else if (checkBox == CheckReflectionCube) {
				return 0x1F8;
			} else if (checkBox == CheckCloseMirror) {
				return 0x600;
			} else if (checkBox == CheckFarMirror) {
				return 0x1800;
			} else if (checkBox == CheckSideMirror) {
				return 0x2000;
			} else if (checkBox == CheckFrontMirror) {
				return 0x4000;
			} else if (checkBox == CheckShadows) {
				return 0x1FFE0000;
			} else if (checkBox == CheckRainReflection) {
				return 0xE0000000;
			} else
				return 0;
		}

		private uint viewSum = 0;

		private void ViewChecked(object sender, RoutedEventArgs e) {
			if (sender is CheckBox cb) {
				viewSum += GetNumber(cb);
				TextValueHideIn.Text = "0x" + Convert.ToInt64(viewSum).ToString("X");
			}
		}

		private void ViewUnchecked(object sender, RoutedEventArgs e) {
			if (sender is CheckBox cb) {
				viewSum -= GetNumber(cb);
				TextValueHideIn.Text = "0x" + Convert.ToInt64(viewSum).ToString("X");
			}
		}

		public string? HideInResult = null;
		private void ButtonOKClick(object sender, RoutedEventArgs e) {
			HideInResult = $"0x{Convert.ToInt64(viewSum):X}";
			DialogResult = true;
			Close();
		}
	}
}
