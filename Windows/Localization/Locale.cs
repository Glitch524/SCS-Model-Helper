using Def_Writer.Utils;
using System.ComponentModel;
using System.IO;
using System.Printing;

namespace Def_Writer.Locale; 

public abstract class Locale(string localeValue, string localeDisplay): INotifyPropertyChanged {
	public static readonly string LocaleValueUni = "universal";

	private string mLocaleValue = localeValue;
	public string LocaleValue {
		get => mLocaleValue;
		set {
			mLocaleValue = value;
			InvokeChange(nameof(LocaleValue));
		}
	}

	private string mLocaleDisplay = localeDisplay;
	public string LocaleDisplay {
		get => mLocaleDisplay;
		set {
			mLocaleDisplay = value;
			InvokeChange(nameof(LocaleDisplay));
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;
	protected void InvokeChange(string name) => PropertyChanged?.Invoke(this, new(name));

	protected static List<string[]> SupportedLocales {
		get => GetLocalePair(
			LocaleValueUni,
			"en_gb", "en_us", "zh_cn", "zh_tw", "ja_jp", "bg_bg", 
			"ca_es", "cs_cz", "da_dk", "de_de", "el_gr", "es_es", 
			"es_la", "et_ee", "eu_es", "fi_fi", "fr_fr", "gl_es", 
			"hr_hr", "hu_hu", "it_it", "ka_ge", "ko_kr", "lt_lt",
			"lv_lv", "mk_mk", "nl_nl", "no_no", "pl_pl", "pl_si",
			"pt_br", "pt_pt", "ro_ro", "ru_ru", "sk_sk", "sl_sl", 
			"sr_sp", "sr_sr", "sv_se", "tr_tr", "uk_uk", "vi_vn");
	}

	private static List<string[]> GetLocalePair(params string[] locales) {
		List<string[]> localeList = [];
		foreach(var locale in locales) {
			localeList.Add([locale, Util.GetString($"locale.{locale}")]);
		}
		return localeList;
	}
}

public static class LocaleUtil {
	public static Dictionary<string, Dictionary<string, int>> GetStringRes(string ProjectLocation) {
		Dictionary<string, Dictionary<string, int>> resouces = [];//dict<语句key, dict<语句value, 相同次数>
		var localeDir = new DirectoryInfo($@"{ProjectLocation}\locale");
		if (localeDir.Exists) {
			static string GetContent(string line, char indexValue = '"') {
				var start = line.IndexOf(indexValue);
				var end = line.LastIndexOf(indexValue);
				return line[(start + 1)..end];
			}
			foreach (var dir in localeDir.GetDirectories()) {
				foreach (var file in dir.GetFiles()) {
					if (file.Name.StartsWith("local_module")) {
						try {
							var locale = dir.Name;
							using StreamReader sr = new(file.FullName);
							string? line = sr.ReadLine()?.Trim();
							if (line == null || !line.Equals("SiiNunit"))
								throw new(Util.GetString("MessageLoadErrNotLocale"));
							string? stringKey = null;
							while((line = sr.ReadLine()?.Trim()) != null) {
								if (line.StartsWith("key[]")) {
									stringKey = GetContent(line);
								} else if (line.StartsWith("val[]")) {
									var stringValue = GetContent(line);
									if (stringKey != null) {
										Dictionary<string, int> valueDict;
										if (resouces.TryGetValue(stringKey, out Dictionary<string, int>? dict)) {
											valueDict = dict;
										} else {
											valueDict = [];
											resouces.Add(stringKey, valueDict);
										}
										if (valueDict.TryGetValue(stringValue, out int value)) {
											valueDict[stringValue] = value + 1;
										} else {
											valueDict.Add(stringValue, 1);
										}
										stringKey = null;
									}
								}
							}

						} catch (Exception) {
						}
					}
				}
			}
		}
		return resouces;
	}
}
