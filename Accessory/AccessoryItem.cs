using System.ComponentModel;

namespace SCS_Mod_Helper.Accessory {
    public abstract class AccessoryItem(
			string modelName,
			string displayName,
			long? price,
			uint? unlockLevel,
			string iconName,
			string partType,
			string modelColl,
			string look,
			string variant): INotifyPropertyChanged {
		public const string NameDisplayName = "name";
		public const string NamePrice = "price";
		public const string NameUnlockLevel = "unlock";
		public const string NameIconName = "icon";
		public const string NamePartType = "part_type";
		public const string NameCollPath = "coll";
		public const string NameLook = "look";
		public const string NameVariant = "variant";

		public const string NameData = "data";
		public const string NameSuitableFor = "suitable_for";
		public const string NameConflictWith = "conflict_with";
		public const string NameDefaults = "defaults";
		public const string NameOverrides = "overrides";
		public const string NameRequire = "require";

		public static List<string> ArrayItems = [
			NameData, 
			NameSuitableFor, 
			NameConflictWith, 
			NameDefaults, 
			NameOverrides, 
			NameRequire];



		protected string mModelName = modelName;
		public string ModelName {
			get => mModelName;
			set {
				mModelName = value;
				InvokeChange(nameof(ModelName));
			}
		}

		protected string mDisplayName = displayName;
		public string DisplayName {
			get => mDisplayName;
			set {
				mDisplayName = value;
				InvokeChange(nameof(DisplayName));
			}
		}

		protected long? mPrice = price;
		public long? Price {
			get => mPrice;
			set {
				mPrice = value;
				InvokeChange(nameof(Price));
			}
		}

		protected uint? mUnlockLevel = unlockLevel;
		public uint? UnlockLevel {
			get => mUnlockLevel;
			set {
				mUnlockLevel = value;
				InvokeChange(nameof(UnlockLevel));
			}
		}

		protected string mIconName = iconName;
		public string IconName {
			get => mIconName;
			set {
				mIconName = value;
				InvokeChange(nameof(IconName));
			}
		}

		protected string mPartType = partType;
		public string PartType {
			get => mPartType;
			set {
				mPartType = value;
				InvokeChange(nameof(PartType));
			}
		}


		private bool mUseCollPath = false;
		public bool UseCollPath {
			get => mUseCollPath;
			set {
				mUseCollPath = value;
				InvokeChange(nameof(UseCollPath));
			}
		}

		protected string mModelColl = modelColl;
		public string CollPath {
			get => mModelColl;
			set {
				mModelColl = value;
				InvokeChange(nameof(CollPath));
			}
		}

		protected string mLook = look;
		public string Look {
			get => mLook;
			set {
				mLook = value;
				InvokeChange(nameof(Look));
			}
		}

		protected string mVariant = variant;
		public string Variant {
			get => mVariant;
			set {
				mVariant = value;
				InvokeChange(nameof(Variant));
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		public void InvokeChange(string name) => PropertyChanged?.Invoke(this, new(name));
	}
}
