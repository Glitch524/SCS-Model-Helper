using System.Windows;

namespace Def_Writer.Utils
{
    class LanguageUtil {
		private static readonly Dictionary<string, Double<string, ResourceDictionary>> langs = [];
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
			}
		}

		public const string LANG_ZH_CN = "zh-CN";
		public const string LANG_EN_US = "en-US";

		public static void SwitchLanguage(string locale) {
			var md = Application.Current.Resources.MergedDictionaries;
			SetupLanguage();
			var selected = langs[locale];
			md.Remove(selected.Second);
			md.Add(selected.Second);
			DefaultData.AccessoryChangeLanguage();
		}

		public static void InitLanguage() {
			var md = Application.Current.Resources.MergedDictionaries;
			SetupLanguage();
			var currentLang = System.Globalization.CultureInfo.InstalledUICulture.Name;
			//var rd = langs[currentLang] ?? langs["en-US"];
			var rd = langs["en-US"];
			md.Remove(rd.Second);
			md.Add(rd.Second);
		}
	}
}
