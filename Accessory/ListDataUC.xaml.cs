using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace SCS_Mod_Helper.Accessory;

/// <summary>
/// ListDataUC.xaml 的交互逻辑
/// </summary>
public partial class ListDataUC: UserControl {
	public ListDataUC() {
		InitializeComponent();
	}

	private void DeleteItemClick(object sender, RoutedEventArgs e) {
		Button button = (Button)sender;
		if (OtherList.DataContext is IListDataInterface ldi) {
			ldi.PopupCollection?.Remove((string)button.DataContext);
			ldi.UpdateContent();
		} else
			return;
	}
}

public interface IListDataInterface {
	public ObservableCollection<string>? PopupCollection { get; }

	public void UpdateContent();
}