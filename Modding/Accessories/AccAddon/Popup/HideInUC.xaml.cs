using System.Windows;
using System.Windows.Controls;

namespace SCS_Mod_Helper.Modding.Accessories.AccAddon.Popup;

/// <summary>
/// HideInUC.xaml 的交互逻辑
/// </summary>
public partial class HideInUC: UserControl {
	readonly AccAddonBinding Binding;
	public HideInUC(AccAddonBinding binding) {
		InitializeComponent();
		Binding = binding;
	}

	private void HideInChecked(object sender, RoutedEventArgs e) {
		if (sender is CheckBox cb)
			Binding.HideIn += GetNumber(cb);
	}

	private void HideInUnchecked(object sender, RoutedEventArgs e) {
		if (sender is CheckBox cb)
			Binding.HideIn -= GetNumber(cb);
	}

	private static uint GetNumber(CheckBox checkBox) => Convert.ToUInt32(checkBox.Tag.ToString()!, 16);
}