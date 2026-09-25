using SCS_Mod_Helper.Trucks;

namespace SCS_Mod_Helper.Modding.Accessories;

public class AccessoryTruck(
	string truckID,
	int productionYear,
	string ingameName,
	string description,
	string manifaturer,
	bool ets2 = false,
	bool check = false,
	string modelType = "",
	string look = "",
	string variant = ""): Truck(truckID, productionYear, ingameName, description, manifaturer, ets2) {

	private bool mCheck = check;
	public bool Check {
		get => mCheck;
		set {
			mCheck = value;
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
}
