using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Modding.PaintJob;
using System.Windows;

namespace SCS_Mod_Helper.Trucks;

public class Truck(
	string truckID,
	int productionYear,
	string displayName,
	string description,
	string manifaturer,
	bool ets2 = false): BaseBinding, IComparable {

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

	private string mManifaturer = manifaturer;
	public string Manifaturer => mManifaturer;
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
	private string mDisplayName = displayName;
	public string DisplayName {
		get => mDisplayName;
		set {
			mDisplayName = value;
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

	public override string ToString() => $"{TruckID} {DisplayName} {ProductionYear}";
}

public class Cabin(string truckID, string cabinID, string cabinName): BaseBinding {
	public string mTruckID = truckID;
	public string TruckID {
		get => mTruckID;
		set {
			mTruckID = value;
			InvokeChange();
		}
	}
	public string mCabinID = cabinID;
	public string CabinID {
		get => mCabinID;
		set {
			mCabinID = value;
			InvokeChange();
		}
	}
	public string mCabinName = cabinName;
	public string CabinName {
		get => mCabinName;
		set {
			mCabinName = value;
			InvokeChange();
		}
	}
}

public class Accessory(string truckID, string accID, string accName): BaseBinding {
	private PaintJobOverrideData? mBelongingOverride = null;
	public PaintJobOverrideData? BelongingOverride {
		get => mBelongingOverride;
		set {
			mBelongingOverride = value;
			InvokeChange();
		}
	}

	private Visibility mIndexVisibility = Visibility.Visible;
	public Visibility IndexVisibility {
		get => mIndexVisibility;
		set {
			mIndexVisibility = value;
			InvokeChange();
		}
	}

	public bool IndexVisible {
		get => IndexVisibility == Visibility.Visible;
		set {
			if (BelongingOverride != null && value)
				IndexVisibility = Visibility.Visible;
			else
				IndexVisibility = Visibility.Hidden;
		}
	}

	private bool mCheck = false;
	public bool Check {
		get => mCheck;
		set {
			mCheck = value;
			if (value || BelongingOverride == null) {
				IndexVisible = false;
			} else
				IndexVisible = true;
			InvokeChange();
		}
	}

	public string mTruckID = truckID;
	public string TruckID {
		get => mTruckID;
		set {
			mTruckID = value;
			InvokeChange();
		}
	}
	public string mAccID = accID;
	public string AccID {
		get => mAccID;
		set {
			mAccID = value;
			InvokeChange();
		}
	}
	public string mAccName = accName;
	public string AccName {
		get => mAccName;
		set {
			mAccName = value;
			InvokeChange();
		}
	}
}
