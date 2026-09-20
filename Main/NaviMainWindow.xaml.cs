using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.ConverterPix;
using SCS_Mod_Helper.Localization;
using SCS_Mod_Helper.Modding.PaintJob;
using SCS_Mod_Helper.Setting;
using SCS_Mod_Helper.Trucks;
using SCS_Mod_Helper.Utils;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Wpf.Ui.Input;

namespace SCS_Mod_Helper.Main;
/// <summary>
/// NaviMainWindow.xaml 的交互逻辑
/// </summary>
public partial class NaviMainWindow: BaseWindow {
	readonly MainWindowBinding binding;
	public NaviMainWindow() {
		InitializeComponent();
		binding = new(new(OpenWindow), new(Test));
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
					var result = MessageBox.Show(GetString("MessagePixNotSet"), GetString("MessageTitleNotice"));
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

	private void Test(string? value) {
		Debug.WriteLine(Paths.SiiFile("project", "id", "type", "name"));
	}

	private void NaviMainWindowLoaded(object sender, RoutedEventArgs e) => Navigation.Navigate("ModManifest");

}

public class MainWindowBinding(RelayCommand<string> windowCommand, RelayCommand<string> testCommand): BaseBinding {

	public RelayCommand<string> WindowCommand { get; } = windowCommand;

	public RelayCommand<string> TestCommand { get; } = testCommand;

	public Visibility TestVisibility => Debugger.IsAttached ? Visibility.Visible : Visibility.Collapsed;
}
