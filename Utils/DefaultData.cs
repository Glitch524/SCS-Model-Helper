using SCS_Mod_Helper.Accessory.AccAddon.Items;
using SCS_Mod_Helper.Language;
using System.Collections.ObjectModel;

namespace SCS_Mod_Helper.Utils {
	class DefaultData {
		public static string TruckVersion = "1.56";
		public static List<Truck> GetDefaultTrucksETS2() {
			return [
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
				new("volvo.fh_2024", 2024, "Volvo FH6")
				];
		}

		public static List<Truck> GetDefaultTrucksATS() {
			return [
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
				new("volvo.vnr_electric", 2021, "Volvo VNR Electric"),
				new("volvo.vnl2024", 2024, "Volvo VNL2024"),
				new("westernstar.5700xe", 2015, "Western Star 5700XE"),
				new("westernstar.49x", 2020, "Western Star 49X"),
				new("westernstar.57x", 2022, "Western Star 57X")
				];
		}

		private static ObservableCollection<ModelTypeInfo>? mModelTypes = null;
		public static ObservableCollection<ModelTypeInfo> ModelTypes {
			get {
				mModelTypes ??= GetModelTypes();
				LanguageUtil.ChangeLanguage += AccessoryChangeLanguage;
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
		public const string TypeToySeat = "toyseat";
		public const string TypeToyBed = "toybed";
		public const string TypeToyBig = "toybig";
		public const string TypeToyBack = "toyback";
		public const string TypeToySofa = "toysofa";
		public const string TypeToyTable = "toytable";
		public static ObservableCollection<ModelTypeInfo> GetModelTypes() {
			return[
				new(TypeCupHolder, true, true),
				new(TypeCurtainF, true, false),
				new(TypeDrvPlate, true, true),
				new(TypeCoDrvPlate, true, true),
				new(TypeFlagL, true, false),
				new(TypeFlagR, true, false),
				new(TypeFlagFL, false, true),
				new(TypeFlagFR, false, true),
				new(TypeInlightBck, true, false),
				new(TypeBckPanel, false, true),
				new(TypeLPillow, true, true),
				new(TypeSetCabinet, false, true),
				new(TypeSetGlass, true, true),
				new(TypeSetLGlass, true, true),
				new(TypeSterringW, true, true),
				new(TypeToyHang, true, true),
				new(TypeToyAc, true, true),
				new(TypeToyStand, true, true),
				new(TypeToySeat, true, true),
				new(TypeToyBed, true, true),
				new(TypeToyBig, true, true),
				new(TypeToyBack, false, true),
				new(TypeToySofa, false, true),
				new(TypeToyTable, false, true),
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
}
