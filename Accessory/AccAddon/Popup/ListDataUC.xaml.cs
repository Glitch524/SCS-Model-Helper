using SCS_Mod_Helper.Accessory.AccHookup;
using SCS_Mod_Helper.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SCS_Mod_Helper.Accessory.AccAddon.Popup;

/// <summary>
/// ListDataUC.xaml 的交互逻辑
/// </summary>
public partial class ListDataUC: UserControl {
	public ListDataUC() {
		InitializeComponent();
	}

	private void DeleteItemClick(object sender, RoutedEventArgs e) {
		Button button = (Button)sender;
		ObservableCollection<string>? popupCollection;
		if (OtherList.DataContext is AccAddonBinding addonBinding) {
			popupCollection = addonBinding.PopupCollection;
		} else if (OtherList.DataContext is AccHookupBinding hookupBinding) {
			popupCollection = hookupBinding.PopupCollection;
		} else
			return;
		popupCollection?.Remove((string)button.DataContext);
	}
}