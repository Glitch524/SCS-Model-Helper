using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Def_Writer;

/// <summary>
/// ModLocalization.xaml 的交互逻辑
/// </summary>
public partial class ModLocalization : BaseWindow
{
	private readonly LocaleInfo LocaleInfo = new();
	public string ProjectLocation;

	public string LocaleName {
		get => LocaleInfo.LocaleName;
		set => LocaleInfo.LocaleName = value;
	}
	public string CurrentLocale {
		get => LocaleInfo.CurrentLocale;
		set => LocaleInfo.CurrentLocale = value;
	}
	public ObservableCollection<Localization> Locales => LocaleInfo.Locales;

	Dictionary<string, Localization> LocaleDict => LocaleInfo.LocaleDict;

	public ObservableCollection<LocalePair> CurrentDictionary => LocaleDict[CurrentLocale].Dictionary;
	public ObservableCollection<LocalePair> UniversalDictionary => LocaleDict[Localization.LocaleValueUni].Dictionary;

	public ModLocalization(string projectLocation)
    {
        InitializeComponent();
		ProjectLocation = projectLocation;
		LoadLocale();

		TextLocaleName.DataContext = LocaleInfo;
		ButtonSyncWithUni.DataContext = LocaleInfo;
		ButtonSyncOrder.DataContext = LocaleInfo;

		TableLocale.ItemsSource = Locales;
    }

	private void LoadLocale() {//如果更新了多文件，就和LocaleUtil的GetStringRes合并
		var localeDir = new DirectoryInfo($@"{ProjectLocation}\locale");
		if (localeDir.Exists) {
			static string GetContent(string line, char indexValue = '"') {
				var start = line.IndexOf(indexValue);
				var end = line.LastIndexOf(indexValue);
				return line[(start + 1)..end];
			}
			string? localeWithValue = null;
			foreach (var dir in localeDir.GetDirectories()) {
				if (dir.GetFiles().Length > 0) {
					var lFile = dir.GetFiles()[0];
					if (lFile.Name.StartsWith("local_module")) {
						LocaleName = GetContent(lFile.Name, '.');
						try {
							var l = LocaleDict[dir.Name];
							using StreamReader sr = new(lFile.FullName);
							string? line = sr.ReadLine()?.Trim();
							if (line == null || !line.Equals("SiiNunit"))
								throw new(GetString("MessageLoadErrNotLocale"));
							string? key = null;
							while ((line = sr.ReadLine()?.Trim()) != null) {
								if (line.StartsWith("key[]")) {
									key = GetContent(line);
								} else if (line.StartsWith("val[]")) {
									if (localeWithValue == null || dir.Name.StartsWith("en_")) {
										localeWithValue = dir.Name;
									}
									var value = GetContent(line);
									if (key != null) {
										var pair = new LocalePair(key, value);
										l.Dictionary.Add(pair);
										key = null;
									}
								}
							}
						} catch (Exception ex) {
							MessageBox.Show(this, GetString("MessageLoadErr") + "\n" + ex.Message);
						}
					}
				}
			}
			DictionaryCopy(localeWithValue);
			TableLocale.SelectedItem = LocaleDict[Localization.LocaleValueUni];
		}
	}

	private void DictionaryCopy(string? targetLocale) {
		if (targetLocale == null)
			return;
		var targetDict = LocaleDict[targetLocale].Dictionary;
		foreach(var pair in targetDict) {
			UniversalDictionary.Add(new(pair.Key, pair.Value));
		}
	}

	private void TableLocaleChanged(object sender, SelectionChangedEventArgs e) {
		if (sender == TableLocale) {
			if (TableLocale.SelectedItem == null) {
				TableLocale.SelectedItem = Locales.First();
			} else {
				Localization locale = (Localization)TableLocale.SelectedItem;
				CurrentLocale = locale.LocaleValue;
				TableDict.ItemsSource = locale.Dictionary;
			}
		}
	}

	private void ButtonSyncingClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonSyncWithUni) {
			var currentList = new List<LocalePair>(CurrentDictionary);
			var universalList = new List<LocalePair>(UniversalDictionary);
			int c = 0;
			while(c < currentList.Count) {
				var cPair = currentList[c];
				if (cPair.Key.Length == 0 && cPair.Value.Length == 0) {
					currentList.RemoveAt(c);
					continue;
				}
				for(int u = 0; u < universalList.Count; u++) {
					var uPair = universalList[u];
					if (uPair.Key.Length == 0 && uPair.Value.Length == 0) {
						universalList.RemoveAt(c);
						continue;
					}
					if (cPair.Key.Equals(uPair.Key)) {
						currentList.RemoveAt(c);
						universalList.RemoveAt(u);
						c--;//因为不能直接continue外面的循环，只能break，但是c会+1，先-1保持c不变
						break;
					}
				}
				c++;
			}
			//最后剩下的就是对方没有的pair，交换插入即可
			static void AddAll(ObservableCollection<LocalePair> dict, List<LocalePair> list) {
				foreach(var pair in list) {
					if (pair.Key.Length == 0 && pair.Value.Length == 0)
						continue;
					dict.Add(new(pair.Key, pair.Value));
				}
			}
			AddAll(CurrentDictionary, universalList);
			AddAll(UniversalDictionary, currentList);
			if (currentList.Count == 0 && universalList.Count == 0)
				MessageBox.Show(this, GetString("MessageSyncResultZero"), GetString("MessageTitleSyncResult"));
			else
				MessageBox.Show(this, GetString("MessageSyncResultTo", currentList.Count) + "\n" + GetString("MessageSyncResultFrom", universalList.Count), GetString("MessageTitleSyncResult"));
		} else if (sender == ButtonSyncOrder) {
			List<string> currentKeys = [];
			foreach(var pair in CurrentDictionary) {
				currentKeys.Add(pair.Key);
			}
			foreach(var locale in Locales) {
				if (locale.LocaleValue.Equals(CurrentLocale))
					continue;
				var dict = locale.Dictionary;
				var dMin = 0;
				for (int c = 0; c < currentKeys.Count; c++) {
					for (int d = dMin; d < dict.Count; d++) {
						var foundPair = dict[d];
						if (currentKeys[c].Equals(foundPair.Key)) {
							if (c != d) { 
								dict.RemoveAt(d);
								dict.Insert(c, foundPair);
							}
							break;
						}
					}
				}
			}
		}
	}

	private void DeleteLocalClick(object sender, RoutedEventArgs e) {
		if (sender is Button button) {
			Localization locale = (Localization)button.DataContext;
			if (locale.LocaleValue.Equals(Localization.LocaleValueUni)) {
				MessageBox.Show(this, GetString("MessageDeleteErr"));
				return;
			}
			locale.Dictionary.Clear();
		}
	}

	private void OperateButtonClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonAdd) {
			if (TableDict.SelectedIndex == -1) {
				CurrentDictionary.Add(new("", ""));
			} else
				CurrentDictionary.Insert(TableDict.SelectedIndex, new("",""));
			if (CurrentDictionary.Count == 1) {

			}

		} else if (sender == ButtonDelete) {
			if (TableDict.SelectedIndex != -1) {
				while (TableDict.SelectedItem != null) {
					CurrentDictionary.Remove((LocalePair)TableDict.SelectedItem);
				}
			}
		} else if (sender == ButtonUp || sender == ButtonDown) {
			MoveItem(sender == ButtonUp);
		}
	}

	private void MoveItem(bool up) {
		if (TableDict.SelectedIndex == -1)
			return;
		int target = TableDict.SelectedIndex;
		List<LocalePair> l = [];
		while (TableDict.SelectedIndex != -1) {
			if (TableDict.SelectedIndex < target) {
				target--;
			}
			l.Add(CurrentDictionary[TableDict.SelectedIndex]);
			CurrentDictionary.RemoveAt(TableDict.SelectedIndex);
		}
		if (up) {
			if (target > 0)
				target--;
		} else if (target < CurrentDictionary.Count)
			target++;
		var IList = TableDict.SelectedItems;
		foreach (LocalePair ri in l) {
			CurrentDictionary.Insert(target, ri);
			IList.Add(ri);
			target++;
		}
	}

	private void ButtonCloseClick(object sender, RoutedEventArgs e) {
		if (sender == ButtonCancel) {
			Close();
		} else if (sender == ButtonSave) {
			SaveLocale();
		}
	}

	private void SaveLocale() {
		if (LocaleName.Length == 0) {
			MessageBox.Show(this, GetString("MessageSaveErrNoName"));
			return;
		}
		foreach(var locale in Locales) {
			if (locale.LocaleValue.Equals(Localization.LocaleValueUni))
				continue;
			if (locale.Dictionary.Count > 0) {
				CreateLocaleSii(locale);
			} else {
				CreateLocaleSii(locale, UniversalDictionary);
			}
		}
		MessageBox.Show(this, GetString("MessageSaveSuccess"));
	}

	private void CreateLocaleSii(Localization locale, ObservableCollection<LocalePair>? dict = null) {
		dict ??= locale.Dictionary;
		var localeFile = Utils.LocaleFile(ProjectLocation, locale.LocaleValue, LocaleName);
		var hasValue = false;
		{
			using StreamWriter sw = new(localeFile);
			sw.WriteLine("SiiNunit");
			sw.WriteLine("{");
			sw.WriteLine("\tlocalization_db : .localization");
			sw.WriteLine("\t{");
			foreach (var pair in dict) {
				if (pair.Key.Length == 0)
					continue;
				if (hasValue)
					sw.WriteLine("");
				else
					hasValue = true;
				sw.WriteLine($"\t\tkey[]: \"{pair.Key}\"");
				sw.WriteLine($"\t\tval[]: \"{pair.Value}\"");
			}
			sw.WriteLine("\t}");
			sw.WriteLine("}");
		}
		if(!hasValue)
			File.Delete(localeFile);
	}

	private void ButtonCleanClick(object sender, RoutedEventArgs e) {
		var universalDict = UniversalDictionary;
		foreach (var locale in Locales) {
			if (locale.LocaleValue.Equals(Localization.LocaleValueUni))
				continue;
			var dict = locale.Dictionary;
			var same = true;
			for (int i = 0; i< universalDict.Count; i++) {
				if (universalDict[i].Key != dict[i].Key || universalDict[i].Value != dict[i].Value) {
					same = false;
					break;
				}
			}
			if (same)
				dict.Clear();
		}
	}
}

public class LocaleInfo: INotifyPropertyChanged {

    private string mLocaleName = "";
    public string LocaleName {
        get => mLocaleName;
        set {
            mLocaleName = value;
            InvokeChange(nameof(LocaleName));
        }
    }

	private string mCurrentLocale = "";
	public string CurrentLocale {
		get => mCurrentLocale;
		set {
			mCurrentLocale = value;
			InvokeChange(nameof(CurrentLocale));
		}
	}


	private string mCurrentDict = "";
	public string CurrentDict {
		get => mCurrentDict;
		set {
			mCurrentDict = value;
			InvokeChange(nameof(CurrentDict));
		}
	}

	public readonly ObservableCollection<Localization> Locales = Localization.GetLocales();


	public readonly Dictionary<string, Localization> LocaleDict = [];

	public LocaleInfo() {
		foreach (var l in Locales) {
			LocaleDict.Add(l.LocaleValue, l);
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;
    private void InvokeChange(string name) {
        PropertyChanged?.Invoke(this, new(name));
    }
}

public class Localization: Locale.Locale {
	public static ObservableCollection<Localization> GetLocales() {
		ObservableCollection<Localization> l = [];
		l.Add(new(LocaleValueUni, LocaleDisplayUni));
		foreach (var locale in SupportedLocales) {
			l.Add(new(locale[0], locale[1]));
		}
		return l;
	}

	public bool HasLocale { 
		get => Dictionary.Count > 0;
		set => InvokeChange(nameof(HasLocale));
	}

	public readonly ObservableCollection<LocalePair> Dictionary = [];
	public Localization(string localeValue, string localeDisplay) : base(localeValue, localeDisplay) {
		Dictionary.CollectionChanged += (s, e) => {
			HasLocale = Dictionary.Count > 0;
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
	private void InvokeChange(string name) {
		PropertyChanged?.Invoke(this, new(name));
	}
}