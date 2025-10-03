using SCS_Mod_Helper.Utils;
using System.IO;
using System.Windows;

namespace SCS_Mod_Helper.Language {
    class LanguageUtil {
		public static readonly Dictionary<string, Double<string, ResourceDictionary>> langs = [];

		public static void InitLanguage() {
			var md = Application.Current.Resources.MergedDictionaries;
			SetupLanguage();
			var rd = langs[CurrentLanguage] ?? langs["en-US"];
			md.Remove(rd.Second);
			md.Add(rd.Second);
		}

		public static void SetupLanguage() {
			if (langs.Count == 0) {
				var md = Application.Current.Resources.MergedDictionaries;
				foreach (var lang in md) {
					var locale = lang.Source.OriginalString;
					var start = locale.LastIndexOf('/');
					var end = locale.LastIndexOf('.');
					locale = locale[(start + 1)..end];
					var s = locale.Split('_');
					langs.Add(s[0], new(s[1], lang));
				}
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
							langs.Add(s[0], new(s[1], dict));
						}
					}
				}
			}
		}

		public delegate void ChangeLanguageDelegate();
		public static event ChangeLanguageDelegate? ChangeLanguage;
		public static void SwitchLanguage(string locale) {
			var md = Application.Current.Resources.MergedDictionaries;
			SetupLanguage();
			var selected = langs[locale];
			md.Remove(selected.Second);
			md.Add(selected.Second);
			ChangeLanguage?.Invoke();
		}

		public static string CurrentLanguage => System.Globalization.CultureInfo.InstalledUICulture.Name;
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
}
