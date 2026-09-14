using SCS_Mod_Helper.Accessory.AccAddon;
using SCS_Mod_Helper.Accessory.AccAddon.CreatedSii;
using SCS_Mod_Helper.Accessory.AccHookup;
using SCS_Mod_Helper.Accessory.PaintJob;
using SCS_Mod_Helper.Accessory.Physics;
using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.ConverterPix;
using SCS_Mod_Helper.Hookups;
using SCS_Mod_Helper.Localization;
using SCS_Mod_Helper.Utils;
using System.IO;
using System.Windows;
using Wpf.Ui.Input;

namespace SCS_Mod_Helper.Main; 
/// <summary>
/// NaviMainWindow.xaml 的交互逻辑
/// </summary>
public partial class NaviMainWindow : BaseWindow
{
	readonly MainWindowBinding binding;
	public NaviMainWindow()
    {
        InitializeComponent();
		binding = new(new(OpenWindow));
		GridMain.DataContext = binding;
	}

	private void OpenWindow(string? type) {
		if (type == null)
			return;
		Window window;
		switch (type) {
			case "ModLocalization":
				window = new ModLocalizationWindow();
				break;
			case "PaintJob":
				window = new PaintJobWindow();
				break;
			case "ConverterPIX":
				string pixPath = Instances.ConverterPixPath;
				if (string.IsNullOrEmpty(pixPath) || !File.Exists(pixPath)) {
					var result = MessageBox.Show(GetString("MessagePixNotSet"),GetString("MessageTitleNotice"));
					if (result == MessageBoxResult.OK) {
						Navigation.Navigate("Settings");
					}
					return;
				}
				if (Settings.Default.ConverterPixNotice) {
					ConverterNoticeBox notice = new() {
						Owner = this
					};
					if (notice.ShowDialog() == true) {
						if (notice.DontShowAgain) {
							Settings.Default.ConverterPixNotice = false;
							Settings.Default.Save();
						}
					} else
						return;
				}
				window = new ConverterPixWindow();
				break;
			default:
				return;
		}
		window.Owner = this;
		window.ShowDialog();
	}

	private void NaviMainWindowLoaded(object sender, RoutedEventArgs e) => Navigation.Navigate("ModManifest");

}

public class MainWindowBinding(RelayCommand<string> windowCommand): BaseBinding {

	public RelayCommand<string> WindowCommand { get; } = windowCommand;
}
