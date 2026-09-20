using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Utils;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;

namespace SCS_Mod_Helper.Trucks;

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
		bool isETS2,
		string truckID,
		string manifaturer,
		int productionYear,
		string ingameName,
		string description) : this(
			truckID,
			productionYear,
			ingameName,
			description,
			isETS2) {
		IsETS2 = isETS2;
		mManifaturer = manifaturer;
	}

	private string mTruckID = truckID;
	public string TruckID {
		get => mTruckID;
		set {
			mTruckID = value;
			InvokeChange();
		}
	}

	private bool mDefaultTruck = false;
	public bool DefaultTruck {
		get => mDefaultTruck;
		set {
			mDefaultTruck = value;
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
	private string TruckName {
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

	public List<Cabin> Cabins = [];
	public List<Accessory> Accessories = [];

	public override string ToString() => $"{TruckID} {IngameName} {ProductionYear}";
}

public class Cabin(string truckID, string cabinID, string cabinName) {
	public string TruckID = truckID;
	public string CabinID = cabinID;
	public string CabinName = cabinName;
}

public class Accessory(string truckID, string accID, string accName) {
	public string TruckID = truckID;
	public string AccID = accID;
	public string AccName = accName;
}
