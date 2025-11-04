using Microsoft.Win32;
using SCS_Mod_Helper.Accessory;
using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Localization;
using SCS_Mod_Helper.Utils;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace SCS_Mod_Helper.Manifest; 
/// <summary>
/// ModManifestPage.xaml 的交互逻辑
/// </summary>
public partial class ModManifestPage : BasePage {
	private readonly ManifestBinding binding;

	string IconName {
		get => binding.IconName; set => binding.IconName = value;
	}
	string DescriptionName {
		get => binding.DescriptionName; set => binding.DescriptionName = value;
	}

	DescLocale CurrentLocale {
		get => binding.CurrentLocale; set => binding.CurrentLocale = value;
	}
	string DescContent {
		get => binding.DescContent; set => binding.DescContent = value;
	}

	Dictionary<string, DescLocale> LocaleDict => binding.LocaleDict;

	public ModManifestPage()
    {
        InitializeComponent();

		binding = ManifestBinding.Instance;

		GridMain.DataContext = binding;

		binding.InsertColor = InsertColor;
	}

	private void OnLoaded(object sender, RoutedEventArgs e) {
	}

	private void ChooseProjectLocation(object sender, RoutedEventArgs e) {
		var folderDialog = new OpenFolderDialog {
			Multiselect = false
		};
		var history = binding.ProjectLocation;
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
					binding.ProjectLocation = folderDialog.FolderName;
					binding.LoadFiles();
				}
				if (File.Exists(Paths.ManifestFile(folderDialog.FolderName))) {
					ExecuteLoad();
					return;
				}
				var result = MessageBox.Show(GetString("MessageNoManifest"), GetString("MessageTitleNotice"), MessageBoxButton.YesNo);
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
					binding.ModIcon = Util.LoadIcon(ofd.FileName);
					binding.NewIcon = true;
				} else {
					var result = MessageBox.Show(GetString("MessageIconErrWrongSize"), GetString("MessageTitleNotice"), MessageBoxButton.YesNo);
					if (result == MessageBoxResult.Yes)
						Process.Start("explorer.exe", "/select," + ofd.FileName);
				}
			} catch (Exception ex) {
				if (ex.Message.Equals("Parameter is not valid."))
					MessageBox.Show(GetString("MessageIconErrNotValidImage"));
			}
		}
	}

	private void InsertColor(string? color) {
		if (color == null)
			return;
		var sStart = TextDescription.SelectionStart;
		var sLength = TextDescription.SelectionLength;

		DescContent = DescContent.Insert(sStart, color);
		sStart += color.Length;

		if (sLength > 0) {
			var sEnd = sStart + sLength;
			DescContent = DescContent.Insert(sEnd, "[normal]");
			sEnd += "[normal]".Length;
			TextDescription.SelectionStart = sEnd;
		} else 
			TextDescription.SelectionStart = sStart;
		TextDescription.Focus();
	}

	private void DeleteLocaleClick(object sender, RoutedEventArgs e) {
		if (sender is Button button) {
			DescLocale locale = (DescLocale)button.DataContext;
			if (locale.LocaleValue.Equals(Locale.LocaleValueUni)) {
				MessageBox.Show(GetString("MessageDeleteLocaleErrUni"));
				return;
			} else {
				var result = MessageBox.Show(GetString("MessageDeleteLocaleDoubleCheck"), GetString("MessageTitleNotice"), MessageBoxButton.YesNo);
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
			AccDataIO.SaveManifest(binding);
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
}
