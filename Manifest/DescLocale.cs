using SCS_Mod_Helper.Localization;
using System.Collections.ObjectModel;

namespace SCS_Mod_Helper.Manifest {

	public class DescLocale(string localeValue, string localeDisplay): Locale(localeValue, localeDisplay) {

		public static ObservableCollection<DescLocale> GetLocales(out Dictionary<string, DescLocale> localeDict) {
			ObservableCollection<DescLocale> l = [];
			localeDict = [];
			foreach (var locale in SupportedLocales) {
				DescLocale descLocale = new(locale[0], locale[1]);
				l.Add(descLocale);
				localeDict.Add(locale[0], descLocale);
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
				InvokeChange(nameof(HasDesc));
				InvokeChange(nameof(DescContent));
			}
		}
	}
}
