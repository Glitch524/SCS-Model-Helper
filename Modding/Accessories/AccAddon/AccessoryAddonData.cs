using SCS_Mod_Helper.Modding.Accessories.Physics;
using SCS_Mod_Helper.Trucks;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;

namespace SCS_Mod_Helper.Modding.Accessories.AccAddon;
public class AccessoryAddonData: AccessoryIntData {
	public AccessoryAddonData() : base(
		AccAddonHistory.Default.ModelName,
		AccAddonHistory.Default.DisplayName,
		AccAddonHistory.Default.Price,
		AccAddonHistory.Default.UnlockLevel,
		AccAddonHistory.Default.IconName,
		AccAddonHistory.Default.PartType,
		AccAddonHistory.Default.CollisionPath,
		AccAddonHistory.Default.Look,
		AccAddonHistory.Default.Variant,
		AccAddonHistory.Default.ElectricType) {
		mModelType = AccAddonHistory.Default.ModelType;
		mModelPath = AccAddonHistory.Default.ModelPath;
		mModelPathUK = AccAddonHistory.Default.ModelPathUK;
		mExtModelPath = AccAddonHistory.Default.ExtModelPath;
		mExtModelPathUK = AccAddonHistory.Default.ExtModelPathUK;

		mHideIn = AccAddonHistory.Default.HideIn;

		HistoryToCollection(Data, AccAddonHistory.Default.Data);
		HistoryToCollection(SuitableFor, AccAddonHistory.Default.SuitableFor);
		HistoryToCollection(ConflictWith, AccAddonHistory.Default.ConflictWith);
		HistoryToCollection(Defaults, AccAddonHistory.Default.Defaults);
		HistoryToCollection(Overrides, AccAddonHistory.Default.Overrides);
		HistoryToCollection(Require, AccAddonHistory.Default.Require);
	}

	private static void HistoryToCollection(ObservableCollection<string> collection, string history) {
		if (history.Length == 0)
			return;
		var array = history.Split(TruckDefault.ItemSplit);
		foreach (var a in array) {
			collection.Add(a);
		}
	}

	public void SaveHistory() {
		AccAddonHistory.Default.ModelName = ModelName;
		AccAddonHistory.Default.DisplayName = DisplayName;
		try {
			AccAddonHistory.Default.Price = Price ?? 1;
			AccAddonHistory.Default.UnlockLevel = UnlockLevel ?? 0;
		} catch { }
		AccAddonHistory.Default.IconName = IconName;
		AccAddonHistory.Default.PartType = PartType;
		AccAddonHistory.Default.ModelPath = ModelPath;
		AccAddonHistory.Default.ModelPathUK = ModelPathUK;
		AccAddonHistory.Default.ExtModelPath = ExtModelPath;
		AccAddonHistory.Default.ExtModelPathUK = ExtModelPathUK;
		AccAddonHistory.Default.CollisionPath = CollPath;
		AccAddonHistory.Default.ModelType = ModelType;
		AccAddonHistory.Default.Look = Look;
		AccAddonHistory.Default.Variant = Variant;

		AccAddonHistory.Default.HideIn = HideIn;
		AccAddonHistory.Default.ElectricType = ElectricType;

		AccAddonHistory.Default.Data = string.Join(TruckDefault.ItemSplit, Data);
		AccAddonHistory.Default.SuitableFor = string.Join(TruckDefault.ItemSplit, SuitableFor);
		AccAddonHistory.Default.ConflictWith = string.Join(TruckDefault.ItemSplit, ConflictWith);
		AccAddonHistory.Default.Defaults = string.Join(TruckDefault.ItemSplit, Defaults);
		AccAddonHistory.Default.Overrides = string.Join(TruckDefault.ItemSplit, Overrides);
		AccAddonHistory.Default.Require = string.Join(TruckDefault.ItemSplit, Require);

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

	private string mExtModelPath;
	public string ExtModelPath {
		get => mExtModelPath;
		set {
			mExtModelPath = value;
			InvokeChange(nameof(ExtModelPath));
		}
	}

	private string mExtModelPathUK;
	public string ExtModelPathUK {
		get => mExtModelPathUK;
		set {
			mExtModelPathUK = value;
			InvokeChange(nameof(ExtModelPathUK));
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

	public List<PhysicsData> PhysicsList = [];

	public ObservableCollection<string> LookList = [];
	public ObservableCollection<string> VariantList = [];


	protected uint mHideIn;
	public uint HideIn {
		get => mHideIn;
		set {
			mHideIn = value;
			InvokeChange(nameof(HideIn));
		}
	}

	//卡车列表
	public ObservableCollection<Truck> TrucksETS2 = [];
	public ObservableCollection<Truck> TrucksATS = [];
}
