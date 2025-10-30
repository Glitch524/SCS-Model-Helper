using SCS_Mod_Helper.Accessory;
using SCS_Mod_Helper.Main;
using System.IO;
using System.Windows;

namespace SCS_Mod_Helper.Utils; 
public static class Instances {

	public static string CurrentLanguage {
		get {
			var l = Settings.Default.Language;
			if (l.Length == 0)
				l = System.Globalization.CultureInfo.InstalledUICulture.Name;
			return l;
		}
		set {
			Settings.Default.Language = value;
			Settings.Default.Save();
		}
	}

	public static string CurrentTheme {
		get => Settings.Default.Theme;
		set {
			Settings.Default.Theme = value;
			Settings.Default.Save();
		}
	}

	public static bool FollowSystem => CurrentTheme == "FollowSystem";

	private static string mConverterPixPath = Settings.Default.ConverterPixPath;
	public static string ConverterPixPath {
		get {
			if (mConverterPixPath.Length == 0)
				return Util.GetString("StatusEmpty");
			return mConverterPixPath;
		}

		set {
			mConverterPixPath = value;
			Settings.Default.ConverterPixPath = value;
			Settings.Default.Save();
		}
	}


	private static string mProjectLocation = Settings.Default.ProjectLocation;
	public static string ProjectLocation {
		get => mProjectLocation;
		set {
			mProjectLocation = value;
			Settings.Default.ProjectLocation = value;
			Settings.Default.Save();
		}
	}


	private static string projectLocationOfDict = "";

	private static Dictionary<string, List<string>>? mLocaleDict = null;
	public static Dictionary<string, List<string>> LocaleDict {
		get {
			if (mLocaleDict == null || ProjectLocation != projectLocationOfDict) {
				var projectLocation = ProjectLocation;
				if (mLocaleDict == null)
					mLocaleDict = [];
				else
					mLocaleDict.Clear();
				projectLocationOfDict = projectLocation;
				AccDataIO.CheckLocaleDict(mLocaleDict);
			}
			return mLocaleDict;
		}
	}

	public static void LocaleDictReset(string projectLocation) {
		projectLocationOfDict = projectLocation;
		if (mLocaleDict == null) {
			mLocaleDict = [];
		} else
			mLocaleDict.Clear();
	}

	public static void LocaleDictAdd(string key, string value) {
		if (mLocaleDict!.TryGetValue(key, out List<string>? list)) {
			if (!list.Contains(value))
				list.Add(value);
		} else {
			list = [value];
			mLocaleDict.Add(key, list);
		}
	}
}

class DictionaryUtil {
	public static readonly Dictionary<string, Double<string, ResourceDictionary>> langs = [];
	public static readonly Dictionary<string, ResourceDictionary> themes = [];

	private static void CollectDictionaries() {
		var md = Application.Current.Resources.MergedDictionaries;
		foreach (var dict in md) {
			var name = dict.Source.OriginalString;
			var start = name.LastIndexOf('/');
			var end = name.LastIndexOf('.');
			if (name.StartsWith("/Language")) {
				name = name[(start + 1)..end];
				var s = name.Split('_');
				langs.Add(s[0], new(s[1], dict));
			} else if (name.StartsWith("/Theme")) {
				name = name[(start + 1)..end];
				themes.Add(name, dict);
			}
		}
	}

	public static void SetupDictionary() {
		if (langs.Count == 0 || themes.Count == 0) {
			CollectDictionaries();
			var langDir = new DirectoryInfo(Paths.LanguageDir());
			if (langDir.Exists) {
				foreach (var file in langDir.GetFiles()) {
					if (file.Extension.Equals(".xaml")) {
						var locale = file.Name;
						var end = locale.LastIndexOf('.');
						locale = locale[0..end];
						var s = locale.Split('_');
						var dict = new ResourceDictionary {
							Source = new Uri(file.FullName)
						};
						langs.Add("ext_" + s[0], new("Ext:" + s[1], dict));
					}
				}
			}
		}
	}

	public delegate void ChangeLanguageDelegate();
	public static event ChangeLanguageDelegate? ChangeLanguage;
	public static void SetLanguage(string? locale = null) {
		SetupDictionary();
		var selected = langs[locale ?? Instances.CurrentLanguage];
		if (selected == null) {
			selected = langs["en-US"];
			Instances.CurrentLanguage = "en-US";
		}
		var md = Application.Current.Resources.MergedDictionaries;
		md.Remove(selected.Second);
		md.Add(selected.Second);
		ChangeLanguage?.Invoke();
	}

	public static void SetTheme(string? theme = null) {
		theme ??= Instances.CurrentTheme;
		if (theme == "FollowSystem")
			theme = GetSystemTheme();
		switch (theme) {
			case "Light":
				Wpf.Ui.Appearance.ApplicationThemeManager.Apply(Wpf.Ui.Appearance.ApplicationTheme.Light);
				UpdateThemeDict(theme);
				break;
			case "Dark":
				Wpf.Ui.Appearance.ApplicationThemeManager.Apply(Wpf.Ui.Appearance.ApplicationTheme.Dark);
				UpdateThemeDict(theme);
				break;
		}
	}

	public static void UpdateThemeDict(string theme) {
		SetupDictionary();
		if (themes.TryGetValue(theme ?? Instances.CurrentTheme, out var selected)) {
			var md = Application.Current.Resources.MergedDictionaries;
			md.Remove(selected);
			md.Add(selected);
		}
	}

	public static string GetSystemTheme() {
		Wpf.Ui.Appearance.SystemThemeManager.UpdateSystemThemeCache();
		return Wpf.Ui.Appearance.SystemThemeManager.GetCachedSystemTheme().ToString();
	}
}