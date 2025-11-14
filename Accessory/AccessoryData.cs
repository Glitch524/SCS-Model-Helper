using SCS_Mod_Helper.Accessory.AccAddon.Items;
using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SCS_Mod_Helper.Accessory {
    public abstract class AccessoryData: BaseBinding {

		public AccessoryData(
			string modelName,
			string displayName,
			long? price,
			uint? unlockLevel,
			string iconName,
			string partType,
			string modelColl,
			string look,
			string variant,
			string electricType) {
			mModelName = modelName;
			mDisplayName = displayName;
			mPrice = price;
			mUnlockLevel = unlockLevel;
			mIconName = iconName;
			mPartType = partType;
			mModelColl = modelColl;
			mLook = look;
			mVariant = variant;
			mElectricType = electricType;

		}

		protected string mModelName;
		public string ModelName {
			get => mModelName;
			set {
				mModelName = value;
				InvokeChange();

				InvokeChange(nameof(NameOver12));
			}
		}

		public bool NameOver12 => ModelName.Length > 12;

		protected string mDisplayName;
		public string DisplayName {
			get => mDisplayName;
			set {
				mDisplayName = value;
				InvokeChange();

				InvokeChange(nameof(CheckResVisibility));
			}
		}

		public Visibility CheckResVisibility => DisplayName.Contains("@@") ? Visibility.Visible : Visibility.Collapsed;

		protected long? mPrice;
		public long? Price {
			get => mPrice;
			set {
				mPrice = value;
				InvokeChange();
			}
		}

		protected uint? mUnlockLevel;
		public uint? UnlockLevel {
			get => mUnlockLevel;
			set {
				mUnlockLevel = value;
				InvokeChange();
			}
		}

		protected string mIconName;
		public string IconName {
			get => mIconName;
			set {
				mIconName = value;
				InvokeChange();
			}
		}

		protected BitmapSource? mModelIcon = null;
		public BitmapSource? ModelIcon {
			get => mModelIcon;
			set {
				mModelIcon?.Freeze();
				mModelIcon = value;
				InvokeChange();
			}
		}

		protected string mPartType;
		public string PartType {
			get => mPartType;
			set {
				mPartType = value;
				InvokeChange();
			}
		}

		public static List<PartTypeItem> PartTypes => [
			new("unknown", Util.GetString("PartTypeUnknown")),
			new("aftermarket", Util.GetString("PartTypeAftermarket")),
			new("factory", Util.GetString("PartTypeFactory")),
			new("licensed", Util.GetString("PartTypeLicensed"))];

		protected string mModelColl;
		public string CollPath {
			get => mModelColl;
			set {
				mModelColl = value;
				InvokeChange();
			}
		}

		protected string mLook;
		public string Look {
			get => mLook;
			set {
				mLook = value;
				InvokeChange();
			}
		}

		protected string mVariant;
		public string Variant {
			get => mVariant;
			set {
				mVariant = value;
				InvokeChange();
			}
		}

		private string mElectricType;
		public string ElectricType {
			get => mElectricType;
			set {
				mElectricType = value;
				InvokeChange();
			}
		}

		private ObservableCollection<string> data = [];
		private ObservableCollection<string> suitableFor = [];
		private ObservableCollection<string> conflictWith = [];
		private ObservableCollection<string> defaults = [];
		private ObservableCollection<string> overrides = [];
		private ObservableCollection<string> require = [];
		public ObservableCollection<string> Data { get => data; set => data = value; }
		public ObservableCollection<string> SuitableFor { get => suitableFor; set => suitableFor = value; }
		public ObservableCollection<string> ConflictWith { get => conflictWith; set => conflictWith = value; }
		public ObservableCollection<string> Defaults { get => defaults; set => defaults = value; }
		public ObservableCollection<string> Overrides { get => overrides; set => overrides = value; }
		public ObservableCollection<string> Require { get => require; set => require = value; }
		public string DataListContent => JoinList(Data);
		public string SuitableForListContent => JoinList(SuitableFor);
		public string ConflictWithListContent => JoinList(ConflictWith);
		public string DefaultsListContent => JoinList(Defaults);
		public string OverridesListContent => JoinList(Overrides);
		public string RequireListContent => JoinList(Require);

		private static string JoinList(ObservableCollection<string>? list) {
			if (list == null || list.Count == 0)
				return string.Empty;
			return string.Join(", ", list);
		}

		private string? mOpeningList = null;

		public string? OpeningList {
			get => mOpeningList;
			set {
				mOpeningList = value;
				InvokeChange(nameof(PopupCollection));
			}
		}
		public ObservableCollection<string>? PopupCollection {
			get {
				return OpeningList switch {
					"TextData" => Data,
					"TextSuitableFor" => SuitableFor,
					"TextConflictWith" => ConflictWith,
					"TextDefaults" => Defaults,
					"TextOverrides" => Overrides,
					"TextRequire" => Require,
					_ => null,
				};
			}
		}

	}
}
