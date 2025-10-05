using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SCS_Mod_Helper.Localization;
public class LocaleModule: INotifyPropertyChanged {
	private string mModuleName;
	public string ModuleName {
		get => mModuleName;
		set {
			mModuleName = value;
			InvokeChange(nameof(ModuleName));
		}
	}

	public readonly ObservableCollection<ModLocale> LocaleList;

	public ObservableCollection<LocalePair> UniversalDict;

	public LocaleModule(string moduleName) {
		mModuleName = moduleName;
		LocaleList = ModLocale.GetLocales(out ModLocale universal);
		UniversalDict = universal.Dictionary;

	}

	public ModLocale GetLocale(string locale) {
		foreach (var item in LocaleList) {
			if (locale == item.LocaleValue)
				return item;
		}
		throw new ArgumentException("wrong locale: " + locale);
	}

	public event PropertyChangedEventHandler? PropertyChanged;
	private void InvokeChange(string name) => PropertyChanged?.Invoke(this, new(name));
}

public class ModLocale: Locale {
	public static ObservableCollection<ModLocale> GetLocales(out ModLocale universal) {
		ObservableCollection<ModLocale> l = [];
		ModLocale? uni = null;
		foreach (var locale in SupportedLocales) {
			ModLocale m = new(locale[0], locale[1]);
			l.Add(m);
			if (locale[0] == LocaleValueUni)
				uni = m;
		}
		universal = uni!;
		return l;
	}

	public bool HasLocale {
		get => Dictionary.Count > 0;
		set => InvokeChange(nameof(HasLocale));
	}

	public readonly ObservableCollection<LocalePair> Dictionary = [];

	public void AddPair(string key, string value) => Dictionary.Add(new(key, value));
	public void ClearDict() => Dictionary.Clear();

	public ModLocale(string localeValue, string localeDisplay) : base(localeValue, localeDisplay) {
		Dictionary.CollectionChanged += (s, e) => {
			InvokeChange(nameof(HasLocale));
		};
	}
}


public class LocalePair(string key, string value): INotifyPropertyChanged {
	private string mKey = key;
	private string mValue = value;
	public string Key {
		get => mKey;
		set {
			mKey = value;
			InvokeChange(nameof(Key));
		}
	}
	public string Value {
		get => mValue;
		set {
			mValue = value;
			InvokeChange(nameof(Value));
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;
	private void InvokeChange(string name) => PropertyChanged?.Invoke(this, new(name));
}
