using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SCS_Mod_Helper.Accessory.AccAddon {

	public class Truck(string truckID, string ingameName, string description, bool check = false, string modelType = "", string look = "", string variant = ""): INotifyPropertyChanged {
		public event PropertyChangedEventHandler? PropertyChanged;
		private string MTruckID = truckID;
		private bool MCheck = check;
		private string MIngameName = ingameName;
		private string MDescription = description;
		private string MModelType = modelType;
		private string MLook = look;
		private string MVariant = variant;

		public Truck(string truckID, string ingameName) : this(truckID, ingameName, Util.GetString("TruckDesc." + truckID)) {
			//给默认列表使用，备注在字典里
		}

		public static Truck? LineParse(string line) {
			var items = line.Trim().Split(DefaultData.ItemSplit);
			if (items.Length < 7)
				return null;
			return new Truck(items[0], items[1], items[2], bool.Parse(items[3]), items[4], items[5], items[6]);
		}

		public const int IndexTruckID = 0;
		public const int IndexModelType = 1;
		public const int IndexLook = 2;
		public const int IndexVariant = 3;
		public static string DEDHeader() => string.Join(DefaultData.ItemSplit, [nameof(TruckID), nameof(ModelType), nameof(Look), nameof(Variant)]);

		public string ToDEDLine() => string.Join(DefaultData.ItemSplit, [TruckID, ModelType, Look, Variant]);

		public string ToTruckLine() => string.Join(DefaultData.ItemSplit, [TruckID, IngameName, Description, Check, ModelType, MLook, Variant]);
		public static string JoinDED(ObservableCollection<Truck> trucks) {
			return Util.Join(trucks, (t) => t.Check && (t.ModelType.Length > 0 || t.Look.Length > 0 || t.Variant.Length > 0), (t) => t.ToDEDLine());
		}

		public static string JoinTruck(ObservableCollection<Truck> trucks) {
			return Util.Join(trucks, (t) => true, (t) => t.ToTruckLine());
		}

		public string TruckID {
			get => MTruckID;
			set {
				MTruckID = value;
				InvokeChange(nameof(TruckID));
			}
		}
		public bool Check {
			get => MCheck;
			set {
				MCheck = value;
				InvokeChange(nameof(Check));
			}
		}
		public string IngameName {
			get => MIngameName;
			set {
				MIngameName = value;
				InvokeChange(nameof(IngameName));
			}
		}
		public string Description {
			get => MDescription;
			set {
				MDescription = value;
				InvokeChange(nameof(Description));
			}
		}
		public string ModelType {
			get => MModelType;
			set {
				MModelType = value;
				InvokeChange(nameof(ModelType));
			}
		}

		public string Look {
			get => MLook;
			set {
				MLook = value;
				InvokeChange(nameof(Look));
			}
		}

		public string Variant {
			get => MVariant;
			set {
				MVariant = value;
				InvokeChange(nameof(Variant));
			}
		}

		public void InvokeChange(string name) {
			PropertyChanged?.Invoke(this, new(name));
		}

	}
}
