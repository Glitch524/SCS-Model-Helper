using SCS_Mod_Helper.Base;
using System.Windows;

namespace SCS_Mod_Helper.Accessory.PhysicsToy; 
/// <summary>
/// PhysicsToyWindow.xaml 的交互逻辑
/// </summary>
/// 
public partial class PhysicsToyWindow: BaseWindow {

	private PhysicsToyData? mSelectedPhysicsData = null;
	public PhysicsToyData SelectedPhysicsData => mSelectedPhysicsData!;

	public bool ChooseMode = false;

	public PhysicsToyWindow() {
		InitializeComponent();
	}

	private void OnLoaded(object sender, RoutedEventArgs e) {
		ButtonSelect.Visibility = ChooseMode ? Visibility.Visible : Visibility.Collapsed;
	}

	private void ButtonResultClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonCancel) {
			Close();
		} else if (sender == ButtonSelect) {
			PhysicsToyUC.SavePhysicsList();
			mSelectedPhysicsData = UCPhysics.CurrentPhysicsItem;
			DialogResult = true;
			Close(); 
		} else if (sender == ButtonSave) {
			PhysicsToyUC.SavePhysicsList();
		}
	}
}