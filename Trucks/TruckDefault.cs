using SCS_Mod_Helper.Modding.Accessories.AccAddon.Items;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Xml;

namespace SCS_Mod_Helper.Trucks; 
class TruckDefault {
	public static string TruckVersion = "1.61";

	private static ResourceDictionary? mTruckDictionary = null;
	public static ResourceDictionary TruckDictionary {
		get {
			if (mTruckDictionary == null) {
				mTruckDictionary = [];
				var lang = Instances.CurrentLanguage;
				Uri source;
				if (lang.StartsWith(DictionaryUtil.ExtLangPreffix)) {
					lang = lang[DictionaryUtil.ExtLangPreffix.Length..];
					var path = Path.Combine(Paths.TrucksLanguageDir(), $"{lang}.xaml");
					if (File.Exists(path)) 
						source = new Uri(path);
					else
						source = new Uri($"pack://application:,,,/Language/Trucks/en-US.xaml", UriKind.Absolute);
				} else {
					source = new Uri($"pack://application:,,,/Language/Trucks/{lang}.xaml", UriKind.Absolute);
				}
				mTruckDictionary.Source = source;
			}
			return mTruckDictionary;
		}
	}
	public static void ClearTruckDict() {
		mTruckDictionary = null;
	}

	public static List<Truck> DefaultTrucksETS2O => [
		new("daf.xf", 2005, "DAF XF105"), 
		new("daf.xf_euro6", 2013, "DAF XF"), 
		new("daf.2021", 2021, "DAF NGD"),
		new("daf.xd", 2022, "DAF XD"),
		new("daf.xf_electric", 2025, "DAF XF Electric"),
		new("iveco.stralis", 2002, "Iveco Stralis"), 
		new("iveco.hiway", 2012, "Iveco Stralis Hi-Way"),
		new("iveco.sway", 2019, "Iveco S-Way"),
		new("man.tgx", 2008, "MAN TGX Euro 5"),
		new("man.tgx_euro6", 2015, "MAN TGX Euro 6"), 
		new("man.tgx_2020", 2020, "MAN TGX"),
		new("mercedes.actros", 2009, "Mercedes-Benz Actros"), 
		new("mercedes.actros2014", 2014, "Mercedes-Benz New Actros"),
		new("renault.magnum", 1990, "Renault Magnum"), 
		new("renault.premium", 1996, "Renault Premium"),
		new("renault.t", 2013, "Renault T"), 
		new("renault.etech_t", 2023, "Renault E-Tech T"),
		new("scania.r", 2009, "Scania R 2009"),
		new("scania.streamline", 2013, "Scania Streamline"),
		new("scania.r_2016", 2016, "Scania R"), 
		new("scania.s_2016", 2016, "Scania S"),
		new("scania.s_2024e", 2024, "Scania S BEV"), 
		new("volvo.fh16", 2009, "Volvo FH3"),
		new("volvo.fh16_2012", 2012, "Volvo FH4"),
		new("volvo.fh_2021", 2021, "Volvo FH5"),
		new("volvo.fh_2024", 2024, "Volvo FH6")];

	private static readonly string[] showingTrucksETS2 = [
		"daf.xf",
		"daf.xf_euro6",
		"daf.2021",
		"daf.xd",
		"daf.xf_electric",
		"iveco.stralis",
		"iveco.hiway",
		"iveco.sway",
		"man.tgx",
		"man.tgx_euro6",
		"man.tgx_2020",
		"mercedes.actros",
		"mercedes.actros2014",
		"renault.magnum",
		"renault.premium",
		"renault.t",
		"renault.etech_t",
		"scania.r",
		"scania.streamline",
		"scania.r_2016",
		"scania.s_2016",
		"scania.s_2024e",
		"volvo.fh16",
		"volvo.fh16_2012",
		"volvo.fh_2021",
		"volvo.fh_2024"
		];

	private static List<Truck>? mDefaultTrucksETS2 = null;

	public static List<Truck> DefaultTrucksETS2 {
		get {
			if (mDefaultTrucksETS2 == null) {
				mDefaultTrucksETS2 = [];
				foreach (string truckID in showingTrucksETS2) {
					CollectTruck(mDefaultTrucksETS2, truckID);
				}
			}
			return mDefaultTrucksETS2;
		}
	}


	public static List<Truck> DefaultTrucksATSO => [
		new("freightliner.cascadia2019", 2019, "Freightliner Cascadia 2019"),
		new("freightliner.cascadia2024", 2024, "Freightliner Cascadia"),
		new("freightliner.ecascadia", 2022, "Freightliner eCascadia"),
		new("intnational.9900i", 2000, "International 9900i"),
		new("intnational.lonestar", 2008, "International LoneStar"),
		new("intnational.lt", 2017, "International LT"),
		new("kenworth.t680", 2014, "Kenworth T680 2014"),
		new("kenworth.w990", 2018, "Kenworth W990"),
		new("kenworth.t680_2022", 2022, "Kenworth T680"),
		new("kenworth.w900", 2023, "Kenworth W900"),
		new("mack.anthem", 2018, "Mack Anthem"),
		new("mack.pinnacle", 2006, "Mack Pinnacle"),
		new("mack.pioneer", 2025, "Mack Pioneer"),
		new("mack.anthem2026", 2026, "Mack Anthem"),
		new("peterbilt.389", 2007, "Peterbilt 389"),
		new("peterbilt.579", 2012, "Peterbilt 579"),
		new("peterbilt.579_2022", 2022, "Peterbilt 579"),
		new("peterbilt.589", 2025, "Peterbilt 589"),
		new("volvo.vnl", 2014, "Volvo VNL 2014"),
		new("volvo.vnl2018", 2018, "Volvo VNL"),
		new("volvo.vnr_e", 2021, "Volvo VNR Electric"),
		new("volvo.vnl2025", 2025, "Volvo VNL2024"),
		new("westernstar.5700xe", 2015, "Western Star 5700XE"),
		new("westernstar.49x", 2020, "Western Star 49X"),
		new("westernstar.57x", 2022, "Western Star 57X")];


	private static readonly string[] showingTrucksATS = [
		"freightliner.cascadia2019",
		"freightliner.cascadia2024",
		"freightliner.ecascadia",
		"intnational.9900i", 
		"intnational.lonestar", 
		"intnational.lt", 
		"kenworth.t680", 
		"kenworth.w990", 
		"kenworth.t680_2022", 
		"kenworth.w900", 
		"mack.anthem", 
		"mack.pinnacle", 
		"peterbilt.389", 
		"peterbilt.579", 
		"peterbilt.579_2022", 
		"peterbilt.589", 
		"volvo.vnl", 
		"volvo.vnl2018", 
		"volvo.vnr_e", 
		"volvo.vnl2025", 
		"westernstar.5700xe", 
		"westernstar.49x", 
		"westernstar.57x"
		];

	private static List<Truck>? mDefaultTrucksATS = null;

	public static List<Truck> DefaultTrucksATS {
		get {
			if (mDefaultTrucksATS == null) {
				mDefaultTrucksATS = [];
				foreach (string truckID in showingTrucksATS) {
					CollectTruck(mDefaultTrucksATS, truckID);
				}
			}
			return mDefaultTrucksATS;
		}
	}
	private static void CollectTruck(List<Truck> trucks, string truckID) {
		string name = (string)TruckDictionary["name." + truckID];
		int prodYear = (int)TruckDictionary["prodYear." + truckID];
		string desc = (string)TruckDictionary["desc." + truckID];
		Truck truck = new(false, truckID, prodYear, name, desc);
		trucks.Add(truck);
	}

	private static ObservableCollection<ModelTypeInfo>? mModelTypes = null;
	public static ObservableCollection<ModelTypeInfo> ModelTypes {
		get {
			mModelTypes ??= GetModelTypes();
			DictionaryUtil.ChangeLanguage += AccessoryChangeLanguage;
			return mModelTypes;
		}
	}

	public const string TypeCupHolder = "cup_holder";
	public const string TypeCurtainF = "curtain_f";
	public const string TypeDrvPlate = "drv_plate";
	public const string TypeCoDrvPlate = "codrv_plate";
	public const string TypeFlagL = "flag_l";
	public const string TypeFlagR = "flag_r";
	public const string TypeFlagFL = "flag_f_l";
	public const string TypeFlagFR = "flag_f_r";
	public const string TypeInlightBck = "inlight_bck";
	public const string TypeBckPanel = "bckpanel";
	public const string TypeLPillow = "l_pillow";
	public const string TypeSetCabinet = "set_cabinet";
	public const string TypeSetGlass = "set_glass";
	public const string TypeSetLGlass = "set_lglass";
	public const string TypeSterringW = "sterring_w";
	public const string TypeToyHang = "toyhang";
	public const string TypeToyAc = "toyac";
	public const string TypeToyStand = "toystand";
	public const string TypeToyDash = "toydash";
	public const string TypeToySeat = "toyseat";
	public const string TypeToyBed = "toybed";
	public const string TypeToyBig = "toybig";
	public const string TypeToyBack = "toyback";
	public const string TypeToySofa = "toysofa";
	public const string TypeToyTable = "toytable";
	public static ObservableCollection<ModelTypeInfo> GetModelTypes() {
		return[
			new(TypeCupHolder),
			new(TypeCurtainF, true),
			new(TypeDrvPlate),
			new(TypeCoDrvPlate),
			new(TypeFlagL, TypeFlagFL, "Acc.flag_l"),
			new(TypeFlagR, TypeFlagFR, "Acc.flag_r"),
			new(TypeInlightBck, TypeBckPanel, "Acc.back_wall"),
			new(TypeLPillow),
			new(TypeSetCabinet, false),
			new(TypeSetGlass),
			new(TypeSetLGlass),
			new(TypeSterringW),
			new(TypeToyHang),
			new(TypeToyAc),
			new(TypeToyStand),
			new(TypeToyDash, false),
			new(TypeToySeat),
			new(TypeToyBed),
			new(TypeToyBig),
			new(TypeToyBack, false),
			new(TypeToySofa, false),
			new(TypeToyTable, false),
		];
	}

	public static void AccessoryChangeLanguage() {
		foreach(var acc in ModelTypes) {
			acc.RefreshName();
		}
	}

	public static string LineSplit = "|-|";
	public static string ItemSplit = "|_|";
}