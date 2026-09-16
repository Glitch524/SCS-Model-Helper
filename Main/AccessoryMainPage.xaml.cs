using SCS_Mod_Helper.Modding.Accessories.AccAddon;
using SCS_Mod_Helper.Modding.Accessories.AccAddon.CreatedSii;
using SCS_Mod_Helper.Modding.Accessories.AccHookup;
using SCS_Mod_Helper.Modding.Accessories.Physics;
using SCS_Mod_Helper.Modding.Hookups;
using System.Windows;
using System.Windows.Controls;

namespace SCS_Mod_Helper.Main {
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
