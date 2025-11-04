using System.Windows;
using System.Windows.Controls;

namespace SCS_Mod_Helper.Accessory.AccAddon.Popup;

/// <summary>
/// HideInUC.xaml 的交互逻辑
/// </summary>
public partial class HideInUC: UserControl {
	readonly AccAddonBinding binding;
	public HideInUC(AccAddonBinding binding) {
		InitializeComponent();
		this.binding = binding;
	}

	private void HideInChecked(object sender, RoutedEventArgs e) {
		if (sender is CheckBox cb) 
			binding.HideIn += GetNumber(cb);
	}

	private void HideInUnchecked(object sender, RoutedEventArgs e) {
		if (sender is CheckBox cb) 
			binding.HideIn -= GetNumber(cb);
	}

	private static uint GetNumber(CheckBox checkBox) => Convert.ToUInt32(checkBox.Tag.ToString()!, 16);
}