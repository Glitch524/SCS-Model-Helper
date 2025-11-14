using SCS_Mod_Helper.Localization;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace SCS_Mod_Helper.Accessory;

class StringResUtil {
	private static readonly ObservableCollection<StringResItem> mStringResList = [];
	public static ObservableCollection<StringResItem> StringResList {
		get {
			if (mStringResList.Count == 0)
				SetupStringResList();
			return mStringResList;
		}
	}
	public static void CleanStringResList() => mStringResList.Clear();

	private static void SetupStringResList() {
		StringResItem head, separator;
		StringResItem separator2, openLocalization;
		if (mStringResList.Count == 0) {
			head = StringResItem.Head();
			separator = StringResItem.Separator();
			separator2 = StringResItem.Separator();
			openLocalization = StringResItem.OpenLocalization();
		} else {
			head = mStringResList.First();
			separator = mStringResList[1];
			separator2 = mStringResList[^2];
			openLocalization = mStringResList.Last();
			mStringResList.Clear();
		}
		mStringResList.Add(head);
		mStringResList.Add(separator);
		var dict = Instances.LocaleDict;

		if (dict.Count == 0) {
			mStringResList.Add(StringResItem.Empty());
		} else {
			foreach (var pair in dict) {
				StringBuilder tips = new();
				tips.AppendLine(Util.GetString("ResourceValueHeader"));
				for (int i = 0; i < pair.Value.Count; i++) {
					var value = pair.Value.ElementAt(i);
					tips.Append($"    {value}");
					if (i < pair.Value.Count - 1)
						tips.Append('\n');
				}
				StringResItem item = new(pair.Key, pair.Key, tips.ToString());
				StringResList.Add(item);
			}
		}

		mStringResList.Add(separator2);
		mStringResList.Add(openLocalization);
	}

	public static void CheckLocaleDict(Dictionary<string, List<string>> localeDict) {
		var localeModules = Instances.LocaleModules;
		foreach (var module in localeModules) {
			foreach (var dict in module.LocaleList) {
				if (dict.Dictionary.Count == 0)
					continue;
				foreach (var pair in dict.Dictionary) {
					if (localeDict.TryGetValue(pair.Key, out List<string>? valueDict)) {
						valueDict.Add(pair.Value);
					} else {
						valueDict = [];
						localeDict.Add(pair.Key, valueDict);
						valueDict.Add(pair.Value);
					}
				}
			}
		}
	}

	public static string GetStringResResults(string displayName, out string localizedName) {
		var localeModules = Instances.LocaleModules;
		var split = displayName.Split("@@");//根据游戏内测试，只有分割后偶数部分是普通文本，奇数部分是资源Key
		if (!split.Contains("@@")) {//displayName没有@@的话，直接返回原始文本，没必要各种进行各种foreach
			localizedName = displayName;
			return localizedName;
		} else {
			Dictionary<string, string[]> values = [];//key: locale名, value: string数组 与split对应
			foreach (var localeModule in localeModules) {
				foreach (var dict in localeModule.LocaleList) {
					if (dict.Dictionary.Count == 0)
						continue;
					var localeValue = dict.LocaleValue;
					if (values.TryGetValue(localeValue, out string[]? splitValues)) {//多个Module可能会有相同的Locale，如果前面有未找到结果的key，则在另一个module的相同locale内寻找
						for (int i = 0; i < split.Length; i++) {
							if (i % 2 == 1) {//偶数部分不需要修改
								var resultValue = dict.GetValue(splitValues[i]);
								if (resultValue != null) {
									splitValues[i] = resultValue;//如果找到对应Key，就替换为对应文本
								}
							}
						}
					} else {
						splitValues = new string[split.Length];//初始化数组
						for (int i = 0; i < split.Length; i++) {
							if (i % 2 == 1) {
								var resultValue = dict.GetValue(split[i]);
								splitValues[i] = resultValue ?? split[i];//如果找到对应Key，就替换为对应文本，否则保持原样
							} else 
								splitValues[i] = split[i];//偶数部分不需要处理，直接赋值
						}
						values.Add(localeValue, splitValues);
					}
				}
			}
			if (values.Count == 0) {//没有获得任何语言数据，直接返回原始文本
				localizedName = displayName.Replace("@@", "");
				return localizedName;
			}

			string? localized = null;
			StringBuilder finalValue = new();
			for (int i = 0; i < values.Count; i++) {
				if (i > 0)
					finalValue.Append('\n');
				var pair = values.ElementAt(i);
				var final = string.Join("", pair.Value);
				finalValue.Append(final);

				if (localized == null) {
					if (pair.Key == "universal" || pair.Key.StartsWith("en")) {//然后优先选择universal或en开头的语言
						localized = final;
					}
				} else {
					if (pair.Key == Util.SystemLocaleForSCS()) {//最后选择与系统语言相同的语言
						localized = final;
					}
				}
			}
			localized ??= displayName.Replace("@@", "");//如果最终还是没有赋值，就使用原始文本
			localizedName = localized;
			return finalValue.ToString();
		}
	}

	public static void OpenLocalization(Window window) {
		var modLocalization = new ModLocalizationWindow() {
			Owner = window
		};
		modLocalization.ShowDialog();
		if (modLocalization.HasChanges) {
			SetupStringResList();
		}
	}

	public static void ApplyStringRes(TextBox TextDisplayName, string resId) {
		var start = TextDisplayName.SelectionStart;
		TextDisplayName.SelectedText = "";
		var insert = $"@@{resId}@@";
		TextDisplayName.Text = TextDisplayName.Text.Insert(start, insert);
		start += insert.Length;
		TextDisplayName.SelectionStart = start;
		TextDisplayName.Focus();
	}
}
