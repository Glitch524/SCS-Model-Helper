using System.ComponentModel;
using System.IO;

namespace Def_Writer.Locale; 

public abstract class Locale(string localeValue, string localeDisplay): INotifyPropertyChanged {
	public static readonly string LocaleValueUni = "universal";
	public static readonly string LocaleDisplayUni = "通用";

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

	protected static string[][] SupportedLocales = [
		["en_gb", "英语-英国"],
		["en_us", "英语-美国"],
		["zh_cn", "简体中文"],
		["zh_tw", "繁体中文"],
		["ja_jp", "日语-日本"],
		["gb_gb", "保加利亚语-保加利亚"],
		["ca_es", "嘉泰罗尼亚"],
		["cs_cz", "捷克语-捷克"],
		["da_dk", "丹麦语-丹麦"],
		["de_de", "德语-德国"],
		["el_gr", "希腊语-希腊"],
		["es_es", "西班牙语-西班牙"],
		["es_la", "西班牙语-？"],
		["et_ee", "爱沙尼亚语-爱沙尼亚"],
		["eu_es", "巴斯克语-巴斯克"],
		["fi_fi", "芬兰语-芬兰"],
		["fr_fr", "法语-法国"],
		["gl_es", "加利西亚语-加利西亚"],
		["hr_hr", "克罗埃西亚语-克罗埃西亚"],
		["hu_hu", "匈牙利语-匈牙利"],
		["it_it", "意大利语-意大利"],
		["ka_ge", "格鲁吉亚语-格鲁吉亚"],
		["ko_kr", "韩语-韩国"],
		["lt_lt", "立陶宛语-立陶宛"],
		["lv_lv", "拉脱维亚语-拉脱维亚"],
		["mk_mk", "马其顿语-马其顿"],
		["nl_nl", "荷兰语-荷兰"],
		["no_no", "挪威语-挪威"],
		["pl_pl", "波兰语-波兰"],
		["pl_si", "波兰语-？"],
		["pt_br", "葡萄牙语-巴西"],
		["pt_pt", "葡萄牙语-葡萄牙"],
		["ro_ro", "罗马尼亚语-罗马尼亚"],
		["ru_ru", "俄语-俄国"],
		["sk_sk", "斯洛伐克语-斯洛伐克"],
		["sl_sl", "斯洛文尼亚语-斯洛文尼亚"],
		["sr_sp", "塞尔维亚语-？"],
		["sr_sr", "塞尔维亚语-另一个？"],
		["sv_se", "瑞典语-瑞典"],
		["tr_tr", "土耳其语-土耳其"],
		["uk_uk", "乌克兰语-乌克兰（uk_uk）"],
		["vi_vn", "越南语-越南"]];
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
								throw new("这个文件似乎不是配置文件");
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
