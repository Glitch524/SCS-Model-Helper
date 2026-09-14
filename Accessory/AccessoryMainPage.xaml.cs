using SCS_Mod_Helper.Accessory.AccAddon;
using SCS_Mod_Helper.Accessory.AccAddon.CreatedSii;
using SCS_Mod_Helper.Accessory.AccHookup;
using SCS_Mod_Helper.Accessory.Physics;
using SCS_Mod_Helper.Hookups;
using System.Windows;
using System.Windows.Controls;

namespace SCS_Mod_Helper.Accessory {
	/// <summary>
	/// AccessoryMainPage.xaml 的交互逻辑
	/// </summary>
	public partial class AccessoryMainPage: Page {
		public AccessoryMainPage() {
			InitializeComponent();
		}

		private void OpenWindows(object sender, RoutedEventArgs e) {
			Window window;
			if (sender == ButtonAccAddon) {
				window = new AccAddonWindow();
			} else if (sender == ButtonAccHookup) {
				window = new AccHookupWindow();
			} else if (sender == ButtonPhysicsToyData) {
				window = new PhysicsWindow();
			} else if (sender == ButtonCreatedSii) {
				window = new CreatedModelWindow();
			} else if (sender == ButtonCreateHookupSii) {
				window = new HookupsWindow();
			} else
				return;
			window.Owner = Window.GetWindow(this);
			window.ShowDialog();
		}
    }
}
