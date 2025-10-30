using SCS_Mod_Helper.Accessory.AccAddon.Items;
using SCS_Mod_Helper.Accessory.Physics;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;

namespace SCS_Mod_Helper.Accessory.AccAddon;
public class AccessoryAddonData: AccessoryData {
	public AccessoryAddonData() : base(
		AccAddonHistory.Default.ModelName,
		AccAddonHistory.Default.DisplayName,
		AccAddonHistory.Default.Price,
		AccAddonHistory.Default.UnlockLevel,
		AccAddonHistory.Default.IconName,
		AccAddonHistory.Default.PartType ?? "unknown",
		AccAddonHistory.Default.CollisionPath,
		AccAddonHistory.Default.Look,
		AccAddonHistory.Default.Variant) {
		mModelType = AccAddonHistory.Default.ModelType;
		mModelPath = AccAddonHistory.Default.ModelPath;
		mModelPathUK = AccAddonHistory.Default.ModelPathUK;
		mHideIn = AccAddonHistory.Default.HideIn;
	}

	public void SaveHistory() {
		AccAddonHistory.Default.ModelName = ModelName;
		AccAddonHistory.Default.DisplayName = DisplayName;
		try {
			AccAddonHistory.Default.Price = Price ?? 1;
			AccAddonHistory.Default.UnlockLevel = UnlockLevel ?? 0;
		} catch {}
		AccAddonHistory.Default.IconName = IconName;
		AccAddonHistory.Default.PartType = PartType;
		AccAddonHistory.Default.ModelPath = ModelPath;
		AccAddonHistory.Default.ModelPathUK = ModelPathUK;
		AccAddonHistory.Default.CollisionPath = CollPath;
		AccAddonHistory.Default.ModelType = ModelType;
		AccAddonHistory.Default.Look = Look;
		AccAddonHistory.Default.Variant = Variant;
		AccAddonHistory.Default.HideIn = HideIn;

		AccAddonHistory.Default.Others = OthersItem.JoinOthers(OthersList);

		AccAddonHistory.Default.Save();

		AccAppIO.SaveTruckList(true, TrucksETS2);
		AccAppIO.SaveTruckList(false, TrucksATS);
	}

	public static string ProjectLocation => Instances.ProjectLocation;

	private string mModelPath;
	public string ModelPath {
		get => mModelPath;
		set {
			mModelPath = value;
			InvokeChange(nameof(ModelPath));
		}
	}

	private string mModelPathUK;
	public string ModelPathUK {
		get => mModelPathUK;
		set {
			mModelPathUK = value;
			InvokeChange(nameof(ModelPathUK));
		}
	}

	private string mModelType;
	public string ModelType {
		get => mModelType;
		set {
			mModelType = value;
			InvokeChange(nameof(ModelType));
		}
	}

	protected uint mHideIn;
	public uint HideIn {
		get => mHideIn;
		set {
			mHideIn = value;
			InvokeChange(nameof(HideIn));
		}
	}

	public List<PhysicsData> PhysicsList = [];

		


	public ObservableCollection<string> LookList = [];
	public ObservableCollection<string> VariantList = [];
	public ObservableCollection<OthersItem> OthersList = [];

	//卡车列表
	public ObservableCollection<Truck> TrucksETS2 = [];
	public ObservableCollection<Truck> TrucksATS = [];
}
