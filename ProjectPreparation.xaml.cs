using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Def_Writer;

/// <summary>
/// ProjectPrepare.xaml 的交互逻辑
/// </summary>
public partial class ProjectPreparation: BaseWindow {
	readonly BaseFileInfo BaseFileInfo = new();
	string Version {
		get => BaseFileInfo.Version; 
		set => BaseFileInfo.Version = value;
	}
	string ModName {
		get => BaseFileInfo.ModName;
		set => BaseFileInfo.ModName = value;
	}
	string Author {
		get => BaseFileInfo.Author;
		set => BaseFileInfo.Author = value;
	}
	bool MPOptional {
		get => BaseFileInfo.MPOptional;
		set => BaseFileInfo.MPOptional = value;
	}
	string IconName {
		get => BaseFileInfo.IconName;
		set => BaseFileInfo.IconName = value;
	}
	string? OldIconName = null;
	BitmapImage? ModIcon {
		get => BaseFileInfo.ModIcon;
		set => BaseFileInfo.ModIcon = value;
	}
	bool NewIcon {
		get => BaseFileInfo.NewIcon;
		set => BaseFileInfo.NewIcon = value;
	}
	string DescriptionName {
		get => BaseFileInfo.DescriptionName;
		set => BaseFileInfo.DescriptionName = value;
	}
	string? OldDescriptionName = null;
	public string CurrentDesc {
		get => BaseFileInfo.CurrentDesc; 
		set => BaseFileInfo.CurrentDesc = value;
	}
	ObservableCollection<DescLocale> Locales {
		get => BaseFileInfo.Locales;
	}
	readonly Dictionary<string, DescLocale> LocaleDict = [];

	readonly ObservableCollection<CategoryItem> categories = [
		new("truck"),
		new("trailer"),
		new("interior"),
		new("tuning_parts"),
		new("ai_traffic"),
		new("sound"),
		new("paint_job"),
		new("cargo_pack"),
		new("map"),
		new("ui"),
		new("weather_setup"),
		new("physics"),
		new("graphics"),
		new("models"),
		new("movers"),
		new("walkers"),
		new("prefabs"),
		new("other")
		];
	readonly Dictionary<string, CategoryItem> CateDict = [];

	readonly string ProjectLocation;

	public ProjectPreparation(string projectLocation) {
		InitializeComponent();

		foreach (var c in categories) {
			CateDict.Add(c.Category, c);
		}
		foreach(var l in Locales) {
			LocaleDict.Add(l.LocaleValue, l);
		}

		ProjectLocation = projectLocation;
		LoadFiles();



		TextVersion.DataContext = BaseFileInfo;
		TextModName.DataContext = BaseFileInfo;
		TextAuthor.DataContext = BaseFileInfo;
		CheckBoxMP.DataContext = BaseFileInfo;

		TextIconName.DataContext = BaseFileInfo;
		ImageModIcon.DataContext = BaseFileInfo;

		TableCategory.ItemsSource = categories;

		TextDescriptionName.DataContext = BaseFileInfo;
		TableLocale.ItemsSource = BaseFileInfo.Locales;
		ButtonCopyFromUniversal.DataContext = BaseFileInfo;
	}

	private void LoadFiles() {
		var manifest = Utils.ManifestFile(ProjectLocation);
		if (File.Exists(manifest)) {
			static string GetContent(string line) {
				var start = line.IndexOf('"');
				var end = line.LastIndexOf('"');
				return line[(start + 1)..end];
			}
			try {
				using StreamReader sr = new(manifest);
				string? line = sr.ReadLine()?.Trim();
				if (line == null || !line.Equals("SiiNunit"))
					throw new(GetString("MessageLoadManifestErrNotManifest"));
				while ((line = sr.ReadLine()?.Trim()) != null) {
					if (line.StartsWith("package_version")) {
						Version = GetContent(line);
					} else if (line.StartsWith("display_name")) {
						ModName = GetContent(line);
					} else if (line.StartsWith("author")) {
						Author = GetContent(line);
					} else if (line.StartsWith("category")) {
						var category = GetContent(line);
						CateDict[category].Check = true;
					} else if (line.StartsWith("icon")) {
						IconName = GetContent(line);
						OldIconName = IconName;
						var iconFile = $@"{ProjectLocation}\{IconName}";
						if (File.Exists(iconFile)) {
							ModIcon = LoadIcon(iconFile);
							ImageModIcon.Source = ModIcon;
						}
					} else if (line.StartsWith("description_file")) {
						DescriptionName = GetContent(line);
						OldDescriptionName = DescriptionName;
						LoadDescription();
					}
				}
			} catch (Exception ex) {
				MessageBox.Show(this, GetString("MessageLoadManifestErrFail") + "\n" + ex.Message);
			}
		}
	}

	private static BitmapImage LoadIcon(string filename) {
		var image = new BitmapImage();
		image.BeginInit();
		image.CacheOption = BitmapCacheOption.OnLoad;
		image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
		image.UriSource = new(filename);
		image.EndInit();
		return image;
	}

	private void ImageButtonClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonChooseIcon) {
			OpenFileDialog ofd = new() {
				Multiselect = false,
				Filter = GetString("DialogIconFilter"),
				DefaultExt = "jpg"
			};
			if (ofd.ShowDialog() != true)
				return;
			try {
				Bitmap bitmap = new(ofd.FileName);
				if (bitmap.Width == 276 && bitmap.Height == 162) {
					ModIcon = LoadIcon(ofd.FileName);
					ImageModIcon.Source = ModIcon;
					NewIcon = true;
				} else {
					var result = MessageBox.Show(this, GetStringMultiLine("MessageIconErrWrongSize", "MessageIconErrWrongSize2", "MessageIconErrWrongSize3"), GetString("MessageTitleNotice"), MessageBoxButton.YesNo);
					if (result == MessageBoxResult.Yes)
						Process.Start("explorer.exe", "/select," + ofd.FileName);
				}
			} catch (Exception ex) {
				if (ex.Message.Equals("Parameter is not valid."))
					MessageBox.Show(this, GetString("MessageIconErrNotValidImage"));
			}
		}
	}

	private void LoadDescription() {
		var descriptionFile = new FileInfo($@"{ProjectLocation}\{DescriptionName}");
		var ext = descriptionFile.Extension;
		var deExt = DescriptionName[..^(ext.Length - 1)];
		DirectoryInfo project = new(ProjectLocation);
		foreach (var file in project.GetFiles()) {
			var filename = file.Name;
			var fileExt = file.Extension;
			var fileDeExt = filename[..^(fileExt.Length - 1)];
			if (fileDeExt.Length >= deExt.Length && fileDeExt.StartsWith(deExt)) {
				using StreamReader sr = new(file.FullName);
				StringBuilder sb = new();
				string? line = null;
				while((line = sr.ReadLine()) != null) {
					sb.AppendLine(line);
				}
				string locale;
				if (fileDeExt.Length == deExt.Length)
					locale = DescLocale.LocaleValueUni;
				else
					locale = filename[deExt.Length..^fileExt.Length];
				var descLocale = LocaleDict[locale];
				descLocale.DescContent = sb.ToString();
			}
		}
		TableLocale.SelectedItem = LocaleDict[DescLocale.LocaleValueUni];
	}

	private void ButtonColorClick(object sender, RoutedEventArgs e) {
		if (sender is Button button) {
			string insert = (string)button.Content;
			var sStart = TextDescription.SelectionStart;
			LocaleDict[CurrentDesc].DescContent = LocaleDict[CurrentDesc].DescContent.Insert(sStart, insert);
			sStart += insert.Length;
			TextDescription.SelectionStart = sStart;
		}
	}

	private void TableLocaleChanged(object sender, SelectionChangedEventArgs e) {
		if (sender == TableLocale) {
			if (TableLocale.SelectedItem == null) {
				TableLocale.SelectedItem = Locales.First();
			} else {
				DescLocale locale = (DescLocale)TableLocale.SelectedItem;
				CurrentDesc = locale.LocaleValue;
				TextDescription.DataContext = locale;
			}
		}
	}

	private void DeleteLocaleClick(object sender, RoutedEventArgs e) {
		if (sender is Button button) {
			DescLocale locale = (DescLocale)button.DataContext;
			if (locale.LocaleValue.Equals(DescLocale.LocaleValueUni)) {
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
			if (CurrentDesc.Equals(DescLocale.LocaleValueUni))
				return;
			LocaleDict[CurrentDesc].DescContent = LocaleDict[DescLocale.LocaleValueUni].DescContent;
		}
	}

	private void ButtonResultClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonCancel) {
			Close();
			return;
		}
		if (sender == ButtonSave) {
			var saveLocation = ProjectLocation;
			if (!IconName.EndsWith(".jpg"))
				IconName += ".jpg";

			if (ModIcon == null) {
				MessageBox.Show(this, GetString("MessageSaveResultNoIcon"));
				ModIcon = new BitmapImage(new("pack://application:,,,/Images/IconPlaceholder.png"));
				NewIcon = true;
			}
			if (OldIconName != null) {
				var oldIconFile = $@"{ProjectLocation}\{OldIconName}";
				if (File.Exists(oldIconFile))
					File.Delete(oldIconFile);
			}
			var iconFile = $@"{saveLocation}\{IconName}";
			if (NewIcon || !File.Exists(iconFile)) {
				JpegBitmapEncoder encoder = new();
				encoder.Frames.Add(BitmapFrame.Create(ModIcon));
				if (File.Exists(iconFile))
					File.Delete(iconFile);
				using FileStream fs = new(iconFile, FileMode.CreateNew, FileAccess.ReadWrite);
				encoder.Save(fs);
			}

			if (OldDescriptionName != null && DescriptionName != OldDescriptionName) {
				var descDeExt = OldDescriptionName[..^4];
				foreach (var file in new DirectoryInfo(ProjectLocation).GetFiles()) {
					var name = file.Name;
					if (name.StartsWith(descDeExt) && name.EndsWith(".txt")) {
						file.Delete();
					}
				}
			}
			var deExt = DescriptionName[..^4];
			foreach(var locale in Locales) {
				if (locale.HasDesc) {
					var isUniversal = locale.LocaleValue.Equals(DescLocale.LocaleValueUni);
					var dFile = $@"{saveLocation}\{deExt}";
					if (!isUniversal)
						dFile += $".{locale.LocaleValue}";
					dFile += ".txt";
					using StreamWriter dWriter = new(dFile);
					dWriter.Write(locale.DescContent);
				}
			}

			var manifestFile = Utils.ManifestFile(saveLocation);
			using StreamWriter mWriter = new(manifestFile);
			mWriter.WriteLine("SiiNunit");
			mWriter.WriteLine("{");
			mWriter.WriteLine("\tmod_package : .package_name");
			mWriter.WriteLine("\t{");
			mWriter.WriteLine($"\t\tpackage_version: \"{Version}\"");
			mWriter.WriteLine($"\t\tdisplay_name: \"{ModName}\"");
			mWriter.WriteLine($"\t\tauthor: \"{Author}\"");
			foreach (var cat in categories) {
				if (cat.Check)
					mWriter.WriteLine($"\t\tcategory[]: \"{cat.Category}\"");
			}
			mWriter.WriteLine($"\t\ticon: \"{IconName}\"");
			mWriter.WriteLine($"\t\tdescription_file: \"{DescriptionName}\"");
			if (MPOptional)
				mWriter.WriteLine($"\t\tmp_mod_optional: \"{MPOptional}\"");
			mWriter.WriteLine("\t}");
			mWriter.WriteLine("}");
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

public class BaseFileInfo: INotifyPropertyChanged {

	private string mVersion = "";

	public string Version {
		get => mVersion;
		set {
			mVersion = value;
			InvokeChange(nameof(Version));
		}
	}

	private string mModName = "";

	public string ModName {
		get => mModName;
		set {
			mModName = value;
			InvokeChange(nameof(ModName));
		}
	}

	private string mAuthor = "";

	public string Author {
		get => mAuthor;
		set {
			mAuthor = value;
			InvokeChange(nameof(Author));
		}
	}

	private bool mMPOptional = false;

	public bool MPOptional {
		get => mMPOptional;
		set {
			mMPOptional = value;
			InvokeChange(nameof(MPOptional));
		}
	}

	private string mIconName = "";
	public string IconName {
		get => mIconName;
		set {
			mIconName = value;
			InvokeChange(nameof(IconName));
		}
	}

	private BitmapImage? mModIcon = null;
	public BitmapImage? ModIcon {
		get => mModIcon;
		set {
			mModIcon?.Freeze();
			mModIcon = value;
			InvokeChange(nameof(ModIcon));
		}
	}
	public bool NewIcon = false;

	private string mDescriptionName = "";
	public string DescriptionName {
		get => mDescriptionName;
		set {
			mDescriptionName = value;
			if (OldDescriptionName.Length == 0)
				OldDescriptionName = value;
			InvokeChange(nameof(DescriptionName));
		}
	}
	public string OldDescriptionName = "";

	private string mCurrentDesc = "";
	public string CurrentDesc {
		get => mCurrentDesc;
		set {
			mCurrentDesc = value;
			InvokeChange(nameof(CurrentDesc));
		}
	}

	public readonly ObservableCollection<DescLocale> Locales = DescLocale.GetLocales();

	public event PropertyChangedEventHandler? PropertyChanged;
	public void InvokeChange(string name) => PropertyChanged?.Invoke(this, new(name));
}

public class CategoryItem(string category): INotifyPropertyChanged {
	private bool mCheck = false;
	public bool Check {
		get => mCheck;
		set {
			mCheck = value;
			InvokeChange(nameof(Check));
		}
	}

	private string mCategory = category;
	public string Category {
		get => mCategory;
		set {
			mCategory = value;
			InvokeChange(nameof(Category));
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;
	private void InvokeChange(string name) => PropertyChanged?.Invoke(this, new(name));
}

public class DescLocale(string localeValue, string localeDisplay): Locale.Locale(localeValue, localeDisplay) {

	public static ObservableCollection<DescLocale> GetLocales() {
		ObservableCollection<DescLocale> l = [];
		l.Add(new(LocaleValueUni, LocaleDisplayUni));
		foreach(var locale in SupportedLocales) {
			l.Add(new(locale[0], locale[1]));
		}
		return l;
	}

	public bool HasDesc {
		get => DescContent.Length > 0;
		set {
			InvokeChange(nameof(HasDesc));
		}
	}

	private string mDescContent = "";
	public string DescContent {
		get => mDescContent;
		set {
			mDescContent = value;
			HasDesc = value.Length > 0;
			InvokeChange(nameof(DescContent));
		}
	}
}
