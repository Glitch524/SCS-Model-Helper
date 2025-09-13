using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;

namespace Def_Writer {
	static class DefaultData {
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
				return MAccessories;
			}
		}

		public static ObservableCollection<AccessoryInfo> GetAccessories() {
			return[
				new("cup_holder", true, true),
				new("curtain_f", true, false),
				new("drvplate", true, true),
				new("codrv_plate", true, true),
				new("flag_l", true, false),
				new("flag_r", true, false),
				new("flag_f_l", false, true),
				new("flag_f_r", false, true),
				new("inlight_bck", true, false),
				new("bckpanel", false, true),
				new("l_pillow", true, true),
				new("set_cabinet", false, true),
				new("set_glass", true, true),
				new("set_lglass", true, true),
				new("sterring_w", true, true),
				new("toyhang", true, true),
				new("toyac", true, true),
				new("toystand", true, true),
				new("toyseat", true, true),
				new("toybed", true, true),
				new("toybig", true, true),
				new("toyback", false, true),
				new("toysofa", false, true),
				new("toytable", false, true),
			];
		}

		public static void AccessoryChangeLanguage() {
			foreach(var acc in Accessories) {
				acc.RefreshName();
			}
		}

		public static readonly string lineSplit = "|-|";
		public static readonly string itemSplit = "|_|";
	}

	static class Utils {
		private static readonly Dictionary<string, Double<string, ResourceDictionary>> langs = [];
		public static void SetupLanguage() {
			if (langs.Count == 0) {
				var md = Application.Current.Resources.MergedDictionaries;
				foreach (var lang in md) {
					var locale = lang.Source.OriginalString;
					var start = locale.LastIndexOf('/');
					var end = locale.LastIndexOf('.');
					locale = locale[(start + 1)..end];
					var s = locale.Split('_');
					langs.Add(s[0], new(s[1], lang));
				}
			}
		}

		public const string LANG_ZH_CN = "zh-CN";
		public const string LANG_EN_US = "en-US";

		public static void SwitchLanguage(string locale) {
			var md = Application.Current.Resources.MergedDictionaries;
			SetupLanguage();
			var selected = langs[locale];
			md.Remove(selected.Second);
			md.Add(selected.Second);
			DefaultData.AccessoryChangeLanguage();
		}

		public static void InitLanguage() {
			var md = Application.Current.Resources.MergedDictionaries;
			SetupLanguage();
			var currentLang = System.Globalization.CultureInfo.InstalledUICulture.Name;
			//var rd = langs[currentLang] ?? langs["en-US"];
			var rd = langs["en-US"];
			md.Remove(rd.Second);
			md.Add(rd.Second);
		}



		public static Window? MainWindow = null;
		public static string GetString(string key, params object[] args) {
			string res;
			var md = Application.Current.Resources.MergedDictionaries;
			try {
				res = Application.Current.FindResource(key).ToString()!;
			} catch (NullReferenceException) {
				//"Object reference not set to an instance of an object." 如果字典的值为空就会出现这个报错
				res = "";
			}
			if (args.Length > 0) {
				res = string.Format(res, args);
			}
			return res;
		}

		public static string ManifestFile(string projectLocation) => 
			$@"{projectLocation}\manifest.sii";
		public static string DefTruckDir(string projectLocation) =>
			$@"{projectLocation}\def\vehicle\truck";
		public static string SiiFile(string projectLocation, string truckId, string modelType, string modelName) {
			string dir = @$"{projectLocation}\def\vehicle\truck\{truckId}\accessory\{modelType}";
			Directory.CreateDirectory(dir);
			return @$"{dir}\{modelName}.sii";
		}
		public static string LocaleFile(string projectLocation, string locale, string localeName) {
			string dir = $@"{projectLocation}\locale\{locale}";
			Directory.CreateDirectory(dir);
			return $@"{dir}\local_module.{localeName}.sii";
		}
		public static string AccessoryDir(string projectLocation) => 
			$@"{projectLocation}\material\ui\accessory";
		public static string IntDecorsDir(string projectLocation) => 
			$@"{projectLocation}\vehicle\truck\upgrade\interior_decors";
		public static string HookupFile(string projectLocation, string hookupName) {
			string dir = $@"{projectLocation}\unit\hookup\vehicle";
			Directory.CreateDirectory(dir);
			return $@"{dir}\{hookupName}.sii";
		}

		public static void OpenFile(string filename) {
			Process pro = new() {
				StartInfo = new(filename) { UseShellExecute = true }
			};
			pro.Start();
		}


		public static string JoinDED(ObservableCollection<Truck> trucks) {
			return Join(trucks, (t) => t.Check && (t.ModelType.Length > 0 || t.Look.Length > 0 || t.Variant.Length > 0), (t) => t.ToDEDLine());
		}

		public static string JoinTruck(ObservableCollection<Truck> trucks) {
			return Join(trucks, (t) => true, (t) => t.ToTruckLine());
		}

		public static string Join(ObservableCollection<Truck> trucks, Func<Truck, bool> condition, Func<Truck, string> toLine) {
			StringBuilder sb = new();
			foreach (var truck in trucks) {
				if (condition(truck)) {
					if (sb.Length > 0)
						sb.Append(DefaultData.lineSplit);
					sb.Append(toLine(truck));
				}
			}
			return sb.ToString();
		}
	}

	public class Double<T1, T2>(T1 first, T2 second) {
		public T1 First = first;
		public T2 Second = second;
	}

	public class Truck(string truckID, string ingameName, string description, bool check = false, string modelType = "", string look = "", string variant = ""): INotifyPropertyChanged {
		public event PropertyChangedEventHandler? PropertyChanged;
		private string MTruckID = truckID;
		private bool MCheck = check;
		private string MIngameName = ingameName;
		private string MDescription = description;
		private string MModelType = modelType;
		private string MLook = look;
		private string MVariant = variant;

		public ObservableCollection<AccessoryInfo>? AccessoryType;

		public Truck(string truckID, string ingameName) : this(truckID, ingameName, Utils.GetString("TruckDesc." + truckID)) {
			//给默认列表使用，备注在字典里
		}

		public static Truck? LineParse(ObservableCollection<AccessoryInfo> accessories, string line) {
			var items = line.Trim().Split(DefaultData.itemSplit);
			if (items.Length < 7)
				return null;
			Truck t = new(items[0], items[1], items[2], bool.Parse(items[3]), items[4], items[5], items[6]) {
				AccessoryType = accessories,
			};
			return t;
		}

		public static string DEDHeader() {
			var spliter = DefaultData.itemSplit;
			return nameof(TruckID) + spliter + nameof(ModelType) + spliter + nameof(Look) + spliter + nameof(Variant);
		}

		public string ToDEDLine() {
			var spliter = DefaultData.itemSplit;
			return TruckID + spliter + ModelType + spliter + Look + spliter + Variant;
		}

		public string ToTruckLine() {
			var spliter = DefaultData.itemSplit;
			return TruckID + spliter + IngameName + spliter + Description + spliter + Check + spliter + ModelType + spliter + MLook + spliter + Variant;
		}

		public string TruckID {
			get {
				return MTruckID;
			}
			set {
				MTruckID = value;
				InvokeChange(nameof(TruckID));
			}
		}
		public bool Check {
			get {
				return MCheck;
			}
			set {
				MCheck = value;
				InvokeChange(nameof(Check));
			}
		}
		public string IngameName {
			get {
				return MIngameName;
			}
			set {
				MIngameName = value;
				InvokeChange(nameof(IngameName));
			}
		}
		public string Description {
			get {
				return MDescription;
			}
			set {
				MDescription = value;
				InvokeChange(nameof(Description));
			}
		}
		public string ModelType {
			get {
				return MModelType;
			}
			set {
				MModelType = value;
				InvokeChange(nameof(ModelType));
			}
		}

		public string Look {
			get {
				return MLook;
			}
			set {
				MLook = value;
				InvokeChange(nameof(Look));
			}
		}

		public string Variant {
			get {
				return MVariant;
			}
			set {
				MVariant = value;
				InvokeChange(nameof(Variant));
			}
		}

		public void InvokeChange(string name) {
			PropertyChanged?.Invoke(this, new(name));
		}

	}

	public class AccessoryInfo(string accessory, string name, bool ets2, bool ats): INotifyPropertyChanged {
		private string MAccessory = accessory;
		private string MName = name;
		private bool METS2 = ets2;
		private bool MATS = ats;

		public event PropertyChangedEventHandler? PropertyChanged;

		public AccessoryInfo(string accessory, bool ets2, bool ats) : this(accessory, Utils.GetString($"Acc.{accessory}"), ets2, ats) {

		}

		public string Accessory {
			get {
				return MAccessory;
			}
			set {
				MAccessory = value;
				InvokeChange(nameof(Accessory));
			}
		}

		public string Name {
			get {
				return MName;
			}
			set {
				MName = value;
				InvokeChange(nameof(Name));
			}
		}
		public void RefreshName() {
			Name = Utils.GetString($"Acc.{Accessory}");
		}

		public bool ETS2 {
			get {
				return METS2;
			}
			set {
				METS2 = value;
				InvokeChange(nameof(ETS2));
			}
		}

		public bool ATS {
			get {
				return MATS;
			}
			set {
				MATS = value;
				InvokeChange(nameof(ATS));
			}
		}

		public void InvokeChange(string name) {
			PropertyChanged?.Invoke(this, new(name));
		}
	}
}
