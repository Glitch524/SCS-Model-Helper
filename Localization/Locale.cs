﻿using SCS_Mod_Helper.Utils;
using System.ComponentModel;

namespace SCS_Mod_Helper.Localization; 

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

	public void RefreshName() {
		LocaleDisplay = Util.GetString($"locale.{LocaleValue}");
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
