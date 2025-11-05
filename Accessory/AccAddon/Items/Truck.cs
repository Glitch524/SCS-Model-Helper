using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Utils;
using System.Runtime.CompilerServices;

namespace SCS_Mod_Helper.Accessory.AccAddon.Items; 

public class Truck(
	string truckID,
	int productionYear,
	string ingameName,
	string description,
	bool check = false,
	string modelType = "",
	string look = "",
	string variant = "",
	bool ets2 = false): BaseBinding, IComparable {

	public Truck(
		string truckID,
		int productionYear,
		string ingameName,
		[CallerMemberName] string caller = "") : this(
			truckID,
			productionYear,
			ingameName,
			Util.GetString("TruckDesc." + truckID),
			caller.EndsWith("ETS2")) {
		//给默认列表使用，备注在字典里
	}

	public static Truck? LineParse(string line) {
		var items = line.Trim().Split(DefaultData.ItemSplit);
		if (items.Length < 8)
			return null;
		return new Truck(items[0], int.Parse(items[1]), items[2], items[3], bool.Parse(items[4]), items[5], items[6], items[7]);
	}

	private string mTruckID = truckID;
	public string TruckID {
		get => mTruckID;
		set {
			mTruckID = value;
			InvokeChange();
		}
	}

	private string? mManifaturer = null;
	public string Manifaturer {
		get {
			if (mManifaturer == null) {
				var truckIDDot = TruckID.IndexOf('.');
				mManifaturer = truckIDDot == -1 ? "" : TruckID[..truckIDDot];
				mTruckName = truckIDDot == -1 ? TruckID : TruckID[(truckIDDot + 1)..];
			}
			return mManifaturer;
		}
	}
	private string? mTruckName = null;
	public string TruckName {
		get {
			if (mTruckName == null) {
				var truckIDDot = TruckID.IndexOf('.');
				mManifaturer = truckIDDot == -1 ? "" : TruckID[..truckIDDot];
				mTruckName = truckIDDot == -1 ? TruckID : TruckID[(truckIDDot + 1)..];
			}
			return mTruckName;
		}
	}

	public int ProductionYear = productionYear;

	private bool mCheck = check;
	public bool Check {
		get => mCheck;
		set {
			mCheck = value;
			InvokeChange();
		}
	}
	private string mIngameName = ingameName;
	public string IngameName {
		get => mIngameName;
		set {
			mIngameName = value;
			InvokeChange();
		}
	}
	private string mDescription = description;
	public string Description {
		get => mDescription;
		set {
			mDescription = value;
			InvokeChange();
		}
	}
	private string mModelType = modelType;
	public string ModelType {
		get => mModelType;
		set {
			mModelType = value;
			InvokeChange();
		}
	}

	private string mLook = look;
	public string Look {
		get => mLook;
		set {
			mLook = value;
			InvokeChange();
		}
	}

	private string mVariant = variant;
	public string Variant {
		get => mVariant;
		set {
			mVariant = value;
			InvokeChange();
		}
	}

	private bool mIsETS2 = ets2;
	public bool IsETS2 {
		get => mIsETS2;
		set {
			mIsETS2 = value;
			InvokeChange();
		}
	}

	public const int IndexDTruckID = 0;
	public const int IndexDModelType = 1;
	public const int IndexDLook = 2;
	public const int IndexDVariant = 3;
	public static string DEDHeader() => string.Join(DefaultData.ItemSplit, [nameof(TruckID), nameof(ModelType), nameof(Look), nameof(Variant)]);

	public string ToDEDLine() => string.Join(DefaultData.ItemSplit, [TruckID, ModelType, Look, Variant]);

	public const int IndexTTruckID = 0;
	public const int IndexTProductionYear = 1;
	public const int IndexTIngameName = 2;
	public const int IndexTDescription = 3;
	public const int IndexTCheck = 4;
	public const int IndexTModelType = 5;
	public const int IndexTLook = 6;
	public const int IndexTVariant = 7;
	public static int[] Indexes => [-1, -1, -1, -1, -1, -1, -1, -1];
	public static string TruckHeader() => string.Join(DefaultData.ItemSplit, [
		nameof(TruckID),
		nameof(ProductionYear),
		nameof(IngameName),
		nameof(Description),
		nameof(Check),
		nameof(ModelType), 
		nameof(Look), 
		nameof(Variant)
		]);

	public string ToTruckLine() => string.Join(DefaultData.ItemSplit, [
		TruckID,
		ProductionYear,
		IngameName,
		Description,
		Check,
		ModelType,
		Look,
		Variant
		]);

	public int CompareTo(object? obj) {
		if (obj is Truck other) {
			var c = Manifaturer.CompareTo(other.Manifaturer);
			if (c == 0) {
				c = ProductionYear.CompareTo(other.ProductionYear);
				if (c == 0) 
					c = TruckName.CompareTo(other.TruckName);
			}
			return c;
		} else {
			throw new ArgumentException("Object is not a Truck");
		}
	}
}
