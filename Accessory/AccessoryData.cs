using SCS_Mod_Helper.Accessory.AccAddon.Items;
using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Utils;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SCS_Mod_Helper.Accessory {
    public abstract class AccessoryData(
			string modelName,
			string displayName,
			long? price,
			uint? unlockLevel,
			string iconName,
			string partType,
			string modelColl,
			string look,
			string variant): BaseBinding {

		protected string mModelName = modelName;
		public string ModelName {
			get => mModelName;
			set {
				mModelName = value;
				InvokeChange();

				InvokeChange(nameof(NameOver12));
			}
		}

		public bool NameOver12 => ModelName.Length > 12;

		protected string mDisplayName = displayName;
		public string DisplayName {
			get => mDisplayName;
			set {
				mDisplayName = value;
				InvokeChange();

				InvokeChange(nameof(CheckResVisibility));
			}
		}

		public Visibility CheckResVisibility => DisplayName.Contains("@@") ? Visibility.Visible : Visibility.Collapsed;

		protected long? mPrice = price;
		public long? Price {
			get => mPrice;
			set {
				mPrice = value;
				InvokeChange();
			}
		}

		protected uint? mUnlockLevel = unlockLevel;
		public uint? UnlockLevel {
			get => mUnlockLevel;
			set {
				mUnlockLevel = value;
				InvokeChange();
			}
		}

		protected string mIconName = iconName;
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

		protected string mPartType = partType;
		public string PartType {
			get => mPartType;
			set {
				mPartType = value;
				InvokeChange();
			}
		}

		public readonly static List<PartTypeItem> PartTypes = [
			new("unknown", Util.GetString("PartTypeUnknown")),
			new("aftermarket", Util.GetString("PartTypeAftermarket")),
			new("factory", Util.GetString("PartTypeFactory")),
			new("licensed", Util.GetString("PartTypeLicensed"))];

		protected string mModelColl = modelColl;
		public string CollPath {
			get => mModelColl;
			set {
				mModelColl = value;
				InvokeChange();
			}
		}

		protected string mLook = look;
		public string Look {
			get => mLook;
			set {
				mLook = value;
				InvokeChange();
			}
		}

		protected string mVariant = variant;
		public string Variant {
			get => mVariant;
			set {
				mVariant = value;
				InvokeChange();
			}
		}
	}
}
