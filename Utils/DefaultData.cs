using SCS_Mod_Helper.Accessory.AccAddon;
using SCS_Mod_Helper.Language;
using System.Collections.ObjectModel;

namespace SCS_Mod_Helper.Utils {
	    class DefaultData{
		public static List<Truck> GetDefaultTrucks(bool isETS2) {
			return isETS2 ? GetDefaultTrucksETS2() : GetDefaultTrucksATS();
		}
		public static List<Truck> GetDefaultTrucksETS2() {
			return [
				new("daf.2021", "DAF NGD"),
				new("daf.xd", "DAF XD"),
				new("daf.xf", "DAF XF105"),
				new("daf.xf_euro6", "DAF XF"),
				new("iveco.hiway", "Iveco Stralis Hi-Way"),
				new("iveco.stralis", "Iveco Stralis"),
				new("iveco.sway", "Iveco S-Way"),
				new("man.tgx", "MAN TGX Euro 5"),
				new("man.tgx_2020", "MAN TGX"),
				new("man.tgx_euro6", "MAN TGX Euro 6"),
				new("mercedes.actros", "Mercedes-Benz Actros"),
				new("mercedes.actros2014", "Mercedes-Benz New Actros"),
				new("renault.etech_t", "Renault E-Tech T"),
				new("renault.magnum", "Renault Magnum"),
				new("renault.premium", "Renault Premium"),
				new("renault.t", "Renault T"),
				new("scania.r", "Scania R 2009"),
				new("scania.r_2016", "Scania R"),
				new("scania.s_2016", "Scania S"),
				new("scania.s_2024e", "Scania S BEV"),
				new("scania.streamline", "Scania Streamline"),
				new("volvo.fh16", "Volvo FH3"),
				new("volvo.fh16_2012", "Volvo FH4"),
				new("volvo.fh_2021", "Volvo FH5"),
				new("volvo.fh_2024", "Volvo FH6"),
			];
		}

		public static List<Truck> GetDefaultTrucksATS() {
			return [
				new("freightliner.cascadia2019", "Freightliner Cascadia 2019"),
				new("freightliner.cascadia2024", "Freightliner Cascadia"),
				new("freightliner.ecascadia", "Freightliner eCascadia"),
				new("intnational.9900i", "International 9900i"),
				new("intnational.lonestar", "International LoneStar"),
				new("intnational.lt", "International LT"),
				new("kenworth.t680", "Kenworth T680 2014"),
				new("kenworth.t680_2022", "Kenworth T680"),
				new("kenworth.w900", "Kenworth W900"),
				new("kenworth.w990", "Kenworth W990"),
				new("mack.anthem", "Mack Anthem"),
				new("mack.pinnacle", "Mack Pinnacle"),
				new("mack.pioneer", "Mack Pioneer"),
				new("peterbilt.389", "Peterbilt 389"),
				new("peterbilt.579", "Peterbilt 579"),
				new("peterbilt.579_2022", "Peterbilt 579"),
				new("volvo.vnl", "Volvo VNL 2014"),
				new("volvo.vnl2018", "Volvo VNL"),
				new("volvo.vnl2024", "Volvo VNL?"),
				new("volvo.vnr_electric", "Volvo VNR Electric"),
				new("westernstar.5700xe", "Western Star 5700XE"),
				new("westernstar.49x", "Western Star 49X"),
				new("westernstar.57x", "Western Star 57X"),
				new("westernstar.4900", "Western Star 4900")
			];
		}

		private static ObservableCollection<AccessoryInfo>? MAccessories = null;
		public static ObservableCollection<AccessoryInfo> Accessories {
			get {
				MAccessories ??= GetAccessories();
				LanguageUtil.ChangeLanguage += AccessoryChangeLanguage;
				return MAccessories;
			}
		}


		public const string TypeCupHolder = "cup_holder";
		public const string TypeCurtainF = "curtain_f";
		public const string TypeDrvPlate = "drvplate";
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
		public static ObservableCollection<AccessoryInfo> GetAccessories() {
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
			foreach(var acc in Accessories) {
				acc.RefreshName();
			}
		}

		public static string LineSplit => "|-|";
		public static string ItemSplit => "|_|";
	}
}
