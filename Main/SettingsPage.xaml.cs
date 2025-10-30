using Microsoft.Win32;
using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Windows;

namespace SCS_Mod_Helper.Main; 
/// <summary>
/// SettingsPage.xaml 的交互逻辑
/// </summary>
public partial class SettingsPage: BasePage {
	private readonly SettingsBinding binding = new();
	public SettingsPage() {
        InitializeComponent();
		binding.ISetThemeWatcher += (watch) => {
			var window = (BaseWindow)Window.GetWindow(this);
			window.SetThemeWatcher(watch);
		};
		PanelMain.DataContext = binding;

		binding.LoadLanguage();
	}

	private void ButtonClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonDirLanguage) {
			var psi = new ProcessStartInfo(Paths.LanguageDir()) { UseShellExecute = true };
			Process.Start(psi);
		} else if (sender == ButtonConverterPix) {
			var fileDialog = new OpenFileDialog {
				Multiselect = false,
				DefaultExt = "converter_pix.exe",
				Title = GetString("ConverterPixPath"),
				Filter = Util.GetFilter("FilterCPix")
			};
			if (fileDialog.ShowDialog() == true) {
				if (fileDialog.SafeFileName != "converter_pix.exe") {
					MessageBox.Show("MessageErrNotCPix");
					return;
				}
				var path = fileDialog.FileName;
				var parent = Directory.GetParent(path)!;
				var is64Bit = Environment.Is64BitOperatingSystem;
				switch (parent.Name) {
					case "win_x86":
						if (is64Bit) {
							path = path.Replace("win_x86", "win_x64");
						}
						break;
					case "win_x64":
						if (!is64Bit) {
							path = path.Replace("win_x64", "win_x86");
						}
						break;
					default:
						var result = MessageBox.Show(
							GetString("MessageErrDifferentStructure"),
							GetString("MessageTitleNotice"),
							MessageBoxButton.YesNo);
						if (result != MessageBoxResult.Yes) {
							return;
						}
						break;
				}
				binding.ConverterPixPath = path;
			}
		}
	}

	private void HyperlinkClick(object sender, System.Windows.Navigation.RequestNavigateEventArgs e) {
		Process.Start(new ProcessStartInfo {
			FileName = e.Uri.ToString(),
			UseShellExecute = true
		});
		e.Handled = true;
	}
}

public class SettingsBinding: BaseBinding {

	private readonly ObservableCollection<LanguageItem> mLanguages = [];
	public ObservableCollection<LanguageItem> Languages => mLanguages;

	public string CurrentLanguage {
		get => Instances.CurrentLanguage;
		set {
			Instances.CurrentLanguage = value;
			InvokeChange();
			DictionaryUtil.SetLanguage(value);
		}
	}

	public void LoadLanguage() {
		foreach (var l in DictionaryUtil.langs) {
			Languages.Add(new(l.Key, l.Value.First, l.Value.Second));
		}
	}

	public List<ThemeItem> ThemeList { get; set; } = [
		new(Util.GetString("ThemeFollowSystem"), "FollowSystem"),
		new(Util.GetString("ThemeLight"), "Light"),
		new(Util.GetString("ThemeDark"), "Dark")
		];

	public delegate void SetThemeWatcher(bool watch);
	public SetThemeWatcher? ISetThemeWatcher = null;
	public string CurrentTheme {
		get => Instances.CurrentTheme;
		set {
			Instances.CurrentTheme = value;
			InvokeChange();
			ISetThemeWatcher?.Invoke(value == "FollowSystem");
			DictionaryUtil.SetTheme(value);
		}
	}

	public string ConverterPixPath {
		get => Instances.ConverterPixPath;
		set {
			Instances.ConverterPixPath = value;
			InvokeChange();
		}
	}
}
public class LanguageItem(string locale, string languageName, ResourceDictionary dictionary) {
	private string mLocale = locale;
	private string mLanguageName = languageName;
	private ResourceDictionary mDictionary = dictionary;

	public string Locale {
		get => mLocale; set => mLocale = value;
	}
	public string LanguageName {
		get => mLanguageName; set => mLanguageName = value;
	}
	public ResourceDictionary Dictionary {
		get => mDictionary; set => mDictionary = value;
	}
}

public class ThemeItem(string themeName, string themeValue): BaseBinding {
	private string mThemeName = themeName;
	public string ThemeName {
		get => mThemeName;
		set {
			mThemeName = value;
			InvokeChange();
		}
	}

	private string mThemeValue = themeValue;
	public string ThemeValue {
		get => mThemeValue;
		set {
			mThemeValue = value;
			InvokeChange();
		}
	}
}
