using Microsoft.Win32;
using SCS_Mod_Helper.Accessory;
using SCS_Mod_Helper.Accessory.AccAddon;
using SCS_Mod_Helper.Accessory.AccAddon.CreatedSii;
using SCS_Mod_Helper.Accessory.AccHookup;
using SCS_Mod_Helper.Accessory.PhysicsToy;
using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Hookups;
using SCS_Mod_Helper.Language;
using SCS_Mod_Helper.Localization;
using SCS_Mod_Helper.Main;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace SCS_Mod_Helper.Manifest;

/// <summary>
/// ProjectPrepare.xaml 的交互逻辑
/// </summary>
public partial class ModManifestWindow: BaseWindow {
	private readonly ModProject ModProject = Instances.ModelProject;

	string ProjectLocation {
		get => ModProject.ProjectLocation; set => ModProject.ProjectLocation = value;
	}
	string Author {
		get => ModProject.Author; set => ModProject.Author = value;
	}
	string IconName {
		get => ModProject.IconName; set => ModProject.IconName = value;
	}
	string DescriptionName {
		get => ModProject.DescriptionName; set => ModProject.DescriptionName = value;
	}

	DescLocale CurrentLocale {
		get => ModProject.CurrentLocale; set => ModProject.CurrentLocale = value;
	}
	string DescContent {
		get => ModProject.DescContent; set => ModProject.DescContent = value;
	}

	ObservableCollection<LanguageItem> Languages => ModProject.Languages;
	Dictionary<string, DescLocale> LocaleDict => ModProject.LocaleDict;

	public ModManifestWindow() {
		InitializeComponent();

		LoadLanguage();

		LoadFiles();

		GridMain.DataContext = ModProject;
	}

	private void SaveOnClosing(object? sender, CancelEventArgs e) {
		ModBasic.Default.ProjectLocation = ProjectLocation;
		ModBasic.Default.Author = Author;
		ModBasic.Default.Save();

		AccAppIO.SavePhysicsList();
	}

	private void LoadFiles() {
		ModProject.InitData();
		var manifest = Paths.ManifestFile(ProjectLocation);
		if (!File.Exists(manifest)) 
			return;
		AccDataIO.LoadManifest(ModProject);
	}

	private void LoadLanguage() {
		LanguageUtil.InitLanguage();
		foreach (var l in LanguageUtil.langs) {
			Languages.Add(new(l.Key, l.Value.First, l.Value.Second));
		}
	}

	private void ChooseProjectLocation(object sender, RoutedEventArgs e) {
		var folderDialog = new OpenFolderDialog {
			Multiselect = false
		};
		var history = ModProject.ProjectLocation;
		if (history.Length > 0) {
			do {
				history = new DirectoryInfo(history).Parent!.FullName;
				if (Directory.Exists(history)) {
					folderDialog.InitialDirectory = history;
					break;
				}
			} while (history.Length > 0);
		}
		if (folderDialog.ShowDialog() == true) {
			if (Directory.Exists(folderDialog.FolderName)) {
				void ExecuteLoad() {
					ModBasic.Default.ProjectLocation = folderDialog.FolderName;
					ModBasic.Default.Save();
					ProjectLocation = folderDialog.FolderName;
					LoadFiles();
				}
				if (File.Exists(Paths.ManifestFile(folderDialog.FolderName))) {
					ExecuteLoad();
					return;
				}
				var result = MessageBox.Show(this, GetString("MessageNoManifest"), GetString("MessageTitleNotice"), MessageBoxButton.YesNo);
				if (result == MessageBoxResult.Yes) {
					ExecuteLoad();
				}
			}
		}
	}

	private void ImageButtonClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonChooseIcon) {
			OpenFileDialog ofd = new() {
				Multiselect = false,
				Filter = Util.GetFilter("DialogFilterIcon"),
				DefaultExt = "jpg"
			};
			if (ofd.ShowDialog() != true)
				return;
			try {
				Bitmap bitmap = new(ofd.FileName);
				if (bitmap.Width == 276 && bitmap.Height == 162) {
					ModProject.ModIcon = Util.LoadIcon(ofd.FileName);
					ModProject.NewIcon = true;
				} else {
					var result = MessageBox.Show(this, GetString("MessageIconErrWrongSize"), GetString("MessageTitleNotice"), MessageBoxButton.YesNo);
					if (result == MessageBoxResult.Yes)
						Process.Start("explorer.exe", "/select," + ofd.FileName);
				}
			} catch (Exception ex) {
				if (ex.Message.Equals("Parameter is not valid."))
					MessageBox.Show(this, GetString("MessageIconErrNotValidImage"));
			}
		}
	}

	private void ButtonColorClick(object sender, RoutedEventArgs e) {
		if (sender is Button button) {
			string insert = (string)button.Tag;
			var sStart = TextDescription.SelectionStart;
			DescContent = DescContent.Insert(sStart, insert);
			sStart += insert.Length;
			TextDescription.SelectionStart = sStart;
			TextDescription.Focus();
		}
	}

	private void DeleteLocaleClick(object sender, RoutedEventArgs e) {
		if (sender is Button button) {
			DescLocale locale = (DescLocale)button.DataContext;
			if (locale.LocaleValue.Equals(Locale.LocaleValueUni)) {
				MessageBox.Show(this, GetString("MessageDeleteLocaleErrUni"));
				return;
			} else {
				var result = MessageBox.Show(this, GetString("MessageDeleteLocaleDoubleCheck"), GetString("MessageTitleNotice"), MessageBoxButton.YesNo);
				if (result == MessageBoxResult.Yes) {
					locale.DescContent = "";
				}
			}
		}
	}

	private void CopyFromUniversalClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonCopyFromUniversal) {
			if (CurrentLocale.LocaleValue.Equals(Locale.LocaleValueUni))
				return;
			if (DescContent.Length > 0) {
				var result = MessageBox.Show(GetString("MessageCopyDescNotEmpty"), GetString("MessageTitleNotice"), MessageBoxButton.YesNo);
				if (result == MessageBoxResult.No)
					return;
			}
			DescContent = LocaleDict[Locale.LocaleValueUni].DescContent;
		}
	}

	private void ButtonResultClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonSave) {
			AccDataIO.SaveManifest(ModProject);
		} else if (sender == ButtonAbout) {
			var window = new AboutWindow {
				Owner = this
			};
			window.ShowDialog();
		}
	}

	private void TextFilenameChanged(object sender, TextChangedEventArgs e) {
		if (sender == TextIconName) {
			if (IconName.EndsWith(".jpg"))
				return;
			var ss = TextIconName.SelectionStart;
			IconName += ".jpg";
			TextIconName.SelectionStart = ss;
		} else if (sender == TextDescriptionName) {
			if (DescriptionName.EndsWith(".txt"))
				return;
			var ss = TextDescriptionName.SelectionStart;
			DescriptionName += ".txt";
			TextDescriptionName.SelectionStart = ss;
		}
	}

	private void ButtonWindowsClick(object sender, RoutedEventArgs e) {
		if (ProjectLocation.Length == 0) {
			MessageBox.Show(GetString("MessageProjectLocationFirst"));
			return;
		}
		Window window;
		if (sender == ButtonAccAddon) {
			window = new AccAddonWindow();
		} else if (sender == ButtonAccHookup) {
			window = new AccHookupWindow();
		} else if (sender == ButtonPhysics) {
			window = new PhysicsToyWindow();
		} else if (sender == ButtonCleanSii) {
			window = new CreatedModelWindow();
		} else if (sender == ButtonCreateHookup) {
			window = new HookupsWindow();
		} else if (sender == ButtonLocalization) {
			window = new ModLocalizationWindow();
		} else if (sender == ButtonAbout) {
			window = new AboutWindow();
		} else
			return;
		window.Owner = this;
		window.ShowDialog();
	}
}